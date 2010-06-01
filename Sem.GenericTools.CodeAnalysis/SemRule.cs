// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SemRule.cs" company="Sven Erik Matzen">
//   Copyright (c) Sven Erik Matzen. GNU Library General Public License (LGPL) Version 2.1.
// </copyright>
// <summary>
//   Defines the SemRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sem.GenericTools.CodeAnalysis
{
    using Microsoft.FxCop.Sdk;
    using Microsoft.VisualStudio.CodeAnalysis.Extensibility;

    /// <summary>
    /// implements some bas functionality for rule classes.
    /// </summary>
    public abstract class SemRule : BaseIntrospectionRule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SemRule"/> class.
        /// </summary>
        /// <param name="name"> The name of the class that inherits from this abstract class 
        /// - that must be the name of the rule, too. </param>
        protected SemRule(string name)
            : base(
                name,                                                                   // The name of the rule (must match the entry in the Rules.XML)
                string.Format("{0}.Rules", typeof(SemRule).Assembly.GetName().Name),    // The name of the Rules.XML file in this project
                typeof(SemRule).Assembly)                                               // The assembly to find the Rules XML in
        {
        }

        /// <summary>
        /// Adds a problem to the ProblemList. 
        /// Certainty, FixCategory and messageLevel are set automatically.
        /// </summary>
        /// <param name="faultyElement">the name of the fault causing element</param>
        /// <param name="target">the node of the element that has caused the error</param>
        protected void AddProblem(string faultyElement, Node target)
        {
            // Get the message text that is shown in the error-list. 
            // Works like string.Format and inserts the given faulty element string into 
            // the error text defined in the Rules.xml
            var resolution = GetResolution(faultyElement);

            // Create a new Problem with the resolution and the target node given
            var problem = new Problem(resolution, target)
            {
                Certainty = 100,
                FixCategory = FixCategories.NonBreaking,
                MessageLevel = MessageLevel.Warning,
            };

            // Add the Problem to the Problemlist
            Problems.Add(problem);
        }
    }
}
