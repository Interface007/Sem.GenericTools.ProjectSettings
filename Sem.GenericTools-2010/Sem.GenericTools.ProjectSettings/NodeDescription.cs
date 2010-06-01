// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NodeDescription.cs" company="Sven Erik Matzen">
//     Copyright (c) Sven Erik Matzen. GNU Library General Public License (LGPL) Version 2.1.
// </copyright>
// <author>Sven Erik Matzen</author>
// <summary>
//   Defines the NodeDescription type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sem.GenericTools.ProjectSettings
{
    using System;
    using System.Globalization;
    using System.Xml;

    /// <summary>
    /// describes an xml node of the project file and provides an array of methods to create the
    /// node and its parent nodes.
    /// </summary>
    public class NodeDescription
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NodeDescription"/> class.
        /// </summary>
        public NodeDescription()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeDescription"/> class.
        /// </summary>
        /// <param name="xPathSelector"> The XPath selector. </param>
        /// <param name="defaultContent"> The default content creation methods. </param>
        public NodeDescription(string xPathSelector, params Func<XmlDocument, string, XmlNode>[] defaultContent)
        {
            this.DefaultContent = defaultContent;
            this.XPathSelector = xPathSelector;
        }

        /// <summary>
        /// Gets or sets the XPath selector to get this node.
        /// </summary>
        public string XPathSelector { get; set; }

        /// <summary>
        /// Gets or sets an array of functions that will create this node and its parent nodes.
        /// </summary>
        public Func<XmlDocument, string, XmlNode>[] DefaultContent { get; set; }

        /// <summary>
        /// Creates a <see cref="NodeDescription"/> assuming that a project files xml node inside a 
        /// property group node should be created.
        /// </summary>
        /// <param name="elementNameInPropertyGroup"> The element name inside the property group. </param>
        /// <returns> a <see cref="NodeDescription"/> for generating the node </returns>
        public static NodeDescription FromElementNameInPropertyGroup(string elementNameInPropertyGroup)
        {
            return new NodeDescription
            {
                XPathSelector = NodeTools.Condition + "cs:" + elementNameInPropertyGroup,
                DefaultContent =
                    new[] { NodeTools.CreatePropertyGroup(), NodeTools.CreateElement(elementNameInPropertyGroup) }
            };
        }

        /// <summary>
        /// Inserts a parameter into the XPath selector of this node description.
        /// </summary>
        /// <param name="parameter"> The parameter to insert. </param>
        /// <returns> the processed selector </returns>
        public string ProcessedSelector(string parameter)
        {
            return
                this.XPathSelector.Contains("{0}")
                ? string.Format(CultureInfo.CurrentCulture, this.XPathSelector, parameter)
                : this.XPathSelector;
        }
    }
}
