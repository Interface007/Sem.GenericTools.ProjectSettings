// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NodeTools.cs" company="Sven Erik Matzen">
//   Copyright (c) Sven Erik Matzen. GNU Library General Public License (LGPL) Version 2.1.
// </copyright>
// <author>Sven Erik Matzen</author>
// <summary>
//   Some tools to work with xml nodes of project files
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sem.GenericTools.ProjectSettings
{
    using System;
    using System.Globalization;
    using System.Xml;

    /// <summary>
    /// Some tools to work with xml nodes of project files
    /// </summary>
    public static class NodeTools
    {
        /// <summary>
        /// The XPath selector for a specific project group with a condition.
        /// </summary>
        public const string Condition = @"//cs:Project/cs:PropertyGroup[@Condition="" {0} ""]/";

        /// <summary>
        /// the default namespace of a ms-build file
        /// </summary>
        public const string MsbuildNamespace = "http://schemas.microsoft.com/developer/msbuild/2003";

        /// <summary>
        /// Creates a method for creating a property group with a condition.
        /// </summary>
        /// <returns> a method for creating a property group with a condition </returns>
        public static Func<XmlDocument, string, XmlNode> CreatePropertyGroup()
        {
            return (doc, para) =>
                {
                    var ret = doc.CreateElement("PropertyGroup", MsbuildNamespace);
                    ret.Attributes.Append(doc.CreateAttribute("Condition")).Value 
                        = string.Format(CultureInfo.CurrentCulture, @" {0} ", para);
                    return ret;
                };
        }

        /// <summary>
        /// Creates a method for creating an element with a specific name.
        /// </summary>
        /// <param name="elementName"> The element name. </param>
        /// <returns> a method for creating an element with a specific name </returns>
        public static Func<XmlDocument, string, XmlNode> CreateElement(string elementName)
        {
            return (doc, para) => doc.CreateElement(elementName, MsbuildNamespace);
        }

        /// <summary>
        /// Creates a namespace manager for the ms build namespace
        /// </summary>
        /// <param name="table"> The name table of the document.</param>
        /// <returns> a new namespace manager</returns>
        public static XmlNamespaceManager CreateNamespaceManager(XmlNameTable table)
        {
            var namespaceManager = new XmlNamespaceManager(table);
            namespaceManager.AddNamespace(string.Empty, MsbuildNamespace);
            namespaceManager.AddNamespace("cs", MsbuildNamespace);
            return namespaceManager;

        }
    }
}
