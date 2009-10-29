// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Sven Erik Matzen">
//   Copyright (c) Sven Erik Matzen. GNU Library General Public License (LGPL) Version 2.1.
// </copyright>
// <summary>
//   This program reads and writes project settings from project files
//   to a flat file in csv format. This was it's easy to compare the
//   settings between different parts of a solution.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sem.GenericTools.ProjectSettings
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Xml;

    /// <summary>
    /// This program reads and writes project settings from project files
    /// to a flat file in csv format. This was it's easy to compare the 
    /// settings between different parts of a solution.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// contains the xpath selectors to extract project data
        /// </summary>
        private static readonly Dictionary<string, NodeDescription> Selectors = new Dictionary<string, NodeDescription> 
            {
                { "NameSpace", new NodeDescription(@"//cs:Project/cs:PropertyGroup/cs:RootNamespace", null) },
                { "AssemblyName", new NodeDescription(@"//cs:Project/cs:PropertyGroup/cs:AssemblyName", null) },
                { "Target", new NodeDescription(@"//cs:Project/cs:PropertyGroup/cs:TargetFrameworkVersion", null) },
                { "DebugSymbols", NodeDescription.FromElementNameInPropertyGroup("DebugSymbols") }, 
                { "OutputPath", NodeDescription.FromElementNameInPropertyGroup("OutputPath") }, 
                { "Constants", NodeDescription.FromElementNameInPropertyGroup("DefineConstants") }, 
                { "DebugType", NodeDescription.FromElementNameInPropertyGroup("DebugType") }, 
                { "RunCode-Analysis", NodeDescription.FromElementNameInPropertyGroup("RunCodeAnalysis") }, 
                { "Optimize", NodeDescription.FromElementNameInPropertyGroup("Optimize") },
                { "DocumentationFile", NodeDescription.FromElementNameInPropertyGroup("DocumentationFile") },
                { "PlatformTarget", NodeDescription.FromElementNameInPropertyGroup("PlatformTarget") },
                { "CodeAnalysisUseTypeNameInSuppression", NodeDescription.FromElementNameInPropertyGroup("CodeAnalysisUseTypeNameInSuppression") },
                { "CodeAnalysisModuleSuppressionsFile", NodeDescription.FromElementNameInPropertyGroup("CodeAnalysisModuleSuppressionsFile") },
                { "ErrorReport", NodeDescription.FromElementNameInPropertyGroup("ErrorReport") },
            };

        /// <summary>
        /// contains xpath selectors with parameters to extract configuration specific data
        /// </summary>
        private static readonly Dictionary<string, string> ConfigurationConditions = new Dictionary<string, string>
            {
                { "default", @"'$(Configuration)' == ''" },
                { "debug", @"'$(Configuration)|$(Platform)' == 'Debug|AnyCPU'" },
                { "release", @"'$(Configuration)|$(Platform)' == 'Release|AnyCPU'" },
                { "ca-build", @"'$(Configuration)|$(Platform)' == 'Debug %28CodeAnalysis%29|AnyCPU'" },
                { "ca build", @"'$(Configuration)|$(Platform)' == 'Debug %28Code Analysis%29|AnyCPU'" },
                { "exclude non standard", @"'$(Configuration)|$(Platform)' == 'Exclude Non-Standard-Projects|AnyCPU'" },
            };

        /// <summary>
        /// Performs an export into a tab seperated file and an import back into the project files - the import can be skipped
        /// </summary>
        public static void Main()
        {
            var rootFolderPath = AppDomain.CurrentDomain.BaseDirectory;
            var nameIndex = rootFolderPath.IndexOf(
                Assembly.GetExecutingAssembly().GetName().Name, StringComparison.Ordinal);
            if (nameIndex > -1)
            {
                rootFolderPath = rootFolderPath.Substring(0, nameIndex);
            }

            while (true)
            {
                Console.WriteLine("(C)opy project file settings to CSV file");
                Console.WriteLine("(O)pen file in standard program for *.CSV");
                Console.WriteLine("(W)rite settings from CSV to project files");
                Console.WriteLine("(E)xit program");
                var input = (Console.ReadLine() ?? string.Empty).ToUpperInvariant();
                switch (input)
                {
                    case "C":
                        CopyProjectFilesToCsv(rootFolderPath);
                        break;

                    case "O":
                        System.Diagnostics.Process.Start(Path.Combine(rootFolderPath, "projectsettings.csv"));
                        break;

                    case "W":
                        CopyCsvToProjectFiles(rootFolderPath);
                        break;

                    case "E":
                        return;
                }
            }
        }

        /// <summary>
        /// updates all project files in a folder including the sub folders with the settings of a csv file
        /// </summary>
        /// <param name="rootFolderPath"> The root folder path. </param>
        private static void CopyCsvToProjectFiles(string rootFolderPath)
        {
            using (var inStream = new StreamReader(Path.Combine(rootFolderPath, "projectsettings.csv")))
            {
                var line = inStream.ReadLine();
                var headers = line.Split(';');

                while (line.Length > 0)
                {
                    line = inStream.ReadLine();
                    if (line == null)
                    {
                        break;
                    }

                    var changeApplied = false;
                    var columns = line.Split(';');

                    var projectSettings = GetProjectSettings(columns[0]);
                    var namespaceManager = NodeTools.CreateNamespaceManager(projectSettings.NameTable);

                    for (var i = 1; i < headers.Length; i++)
                    {
                        if (columns.Length <= i || string.IsNullOrEmpty(columns[i]))
                        {
                            continue;
                        }

                        var selector = Selectors.NewIfNull(headers[i]);
                        var parameter = string.Empty;

                        if (headers[i].Contains("..."))
                        {
                            var parts = headers[i].Split(new[] { "..." }, StringSplitOptions.None);
                            selector = Selectors[parts[0]];
                            parameter = ConfigurationConditions[parts[1]];
                        }

                        var value = projectSettings.SelectSingleNode(selector.ProcessedSelector(parameter), namespaceManager);

                        if (value == null)
                        {
                            Console.WriteLine("nonexisting value in file " + Path.GetFileName(columns[0]) + ": " + headers[i]);
                            if (selector.DefaultContent != null)
                            {
                                value = CreateXml(projectSettings, selector, parameter, namespaceManager);
                            }
                        }

                        var valueText = columns[i].Replace("+", ";");
                        if (value != null && value.InnerText != valueText)
                        {
                            value.InnerText = valueText;
                            changeApplied = true;
                        }
                    }

                    if (changeApplied)
                    {
                        try
                        {
                            projectSettings.Save(columns[0]);
                        }
                        catch (UnauthorizedAccessException)
                        {
                            Console.WriteLine("access denied: " + columns[0].Replace(rootFolderPath, string.Empty));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Creates a node inside a document including the hierarchy.
        /// </summary>
        /// <param name="document"> The document in which the node should be created. </param>
        /// <param name="selector"> The selector that contains information about the node. </param>
        /// <param name="defaultNodeParameter"> The default node parameter for the selector. </param>
        /// <param name="nameSpaceManager"> The namespace manager. </param>
        /// <returns> the newly creaded xml node </returns>
        private static XmlNode CreateXml(XmlDocument document, NodeDescription selector, string defaultNodeParameter, XmlNamespaceManager nameSpaceManager)
        {
            XmlNode node = document.DocumentElement;
            var selectorString = selector.ProcessedSelector(defaultNodeParameter);

            if (selectorString.StartsWith("//", StringComparison.Ordinal))
            {
                selectorString = selectorString.Substring(2);
                GetFragment(ref selectorString, "/");
            }

            var index = 0;
            do
            {
                if (node == null)
                {
                    return null;
                }

                var localSelector = GetFragment(ref selectorString, "/");
                node = node.SelectSingleNode(localSelector, nameSpaceManager) ??
                       node.AppendChild(selector.DefaultContent[index].Invoke(document, defaultNodeParameter));

                index++;
            }
            while (selectorString.Length > 0);

            return node;
        }

        /// <summary>
        /// Returns a fragment of the string - the first character sequence until the occurance of the seperator. Also cuts this
        /// sequence from the parameter <paramref name="combinedString"/>.
        /// </summary>
        /// <param name="combinedString"> The combined string including the leading fragment and the seperator. </param>
        /// <param name="separator"> The separator. </param>
        /// <returns> the leading fragment without the seperator </returns>
        private static string GetFragment(ref string combinedString, string separator)
        {
            var pos = combinedString.IndexOf(separator, StringComparison.Ordinal);
            var value = combinedString;

            if (pos > 0)
            {
                value = combinedString.Substring(0, pos);
                combinedString = combinedString.Substring(pos + 1);
                return value;
            }

            combinedString = string.Empty;
            return value;
        }

        /// <summary>
        /// reads the projects files in a folder including all sub folders and exports selected properties into a csv file
        /// </summary>
        /// <param name="rootFolderPath"> The root folder path. </param>
        private static void CopyProjectFilesToCsv(string rootFolderPath)
        {
            using (var outStream = new StreamWriter(Path.Combine(rootFolderPath, "projectsettings.csv")))
            {
                outStream.Write("filename;");
                foreach (var selector in Selectors)
                {
                    if (!selector.Value.XPathSelector.Contains("{0}"))
                    {
                        outStream.Write(selector.Key.Replace(';', '+'));
                        outStream.Write(";");
                    }
                    else
                    {
                        foreach (var config in ConfigurationConditions)
                        {
                            outStream.Write((selector.Key + "..." + config.Key).Replace(';', '+'));
                            outStream.Write(";");
                        }
                    }
                }

                outStream.WriteLine();

                foreach (var projectFile in Directory.GetFiles(rootFolderPath, "*.csproj", SearchOption.AllDirectories))
                {
                    var projectSettings = GetProjectSettings(projectFile);
                    var namespaceManager = NodeTools.CreateNamespaceManager(projectSettings.NameTable);

                    outStream.Write(projectFile + ";");
                    foreach (var selector in Selectors)
                    {
                        if (!selector.Value.XPathSelector.Contains("{0}"))
                        {
                            var value = projectSettings.SelectSingleNode(selector.Value.XPathSelector, namespaceManager);
                            outStream.Write(value != null ? value.InnerXml.Replace(';', '+') : string.Empty);
                            outStream.Write(";");
                        }
                        else
                        {
                            foreach (var config in ConfigurationConditions)
                            {
                                var value = projectSettings.SelectSingleNode(string.Format(CultureInfo.CurrentCulture, selector.Value.XPathSelector, config.Value), namespaceManager);
                                outStream.Write(value != null ? value.InnerXml.Replace(';', '+') : string.Empty);
                                outStream.Write(";");
                            }
                        }
                    }

                    outStream.WriteLine();
                }

                outStream.Close();
            }
        }

        /// <summary>
        /// reads the project file into a document
        /// </summary>
        /// <param name="projectFile"> The project file. </param>
        /// <returns> an xml document with the content of the project file </returns>
        private static XmlDocument GetProjectSettings(string projectFile)
        {
            var projectSettings = new XmlDocument();
            projectSettings.Load(projectFile);
            return projectSettings;
        }
    }
}
