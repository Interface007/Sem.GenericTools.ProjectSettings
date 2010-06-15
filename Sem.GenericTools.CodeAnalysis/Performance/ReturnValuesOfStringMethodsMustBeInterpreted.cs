// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PublicMethodParametersMustBeCheckedByGuardClassMethod.cs" company="Sven Erik Matzen">
//   Copyright (c) Sven Erik Matzen. GNU Library General Public License (LGPL) Version 2.1.
// </copyright>
// <summary>
//   Defines the PublicMethodParametersMustBeCheckedByGuardClassMethod type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sem.GenericTools.CodeAnalysis.BestPractice
{
    using System;
    using System.Collections.Generic;

    using Microsoft.FxCop.Sdk;

    /// <summary>
    /// Implements a rule checking for GuardClass calls in public methods.
    /// Each parameter of a public method must be validated by a GuardClass method call before 
    /// any other code.
    /// </summary>
    public class ReturnValuesOfStringMethodsMustBeInterpreted : SemRule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReturnValuesOfStringMethodsMustBeInterpreted"/> class that
        /// defines a CA rule. This constructor takes the ReturnValuesOfStringMethodsMustBeInterpreted type to 
        /// look up definitions from the Rules.XML.
        /// </summary>
        public ReturnValuesOfStringMethodsMustBeInterpreted()
            : base(typeof(ReturnValuesOfStringMethodsMustBeInterpreted).Name)
        {
        }

        /// <summary>
        /// Gets the visibilty of the target nodes. In this case the target visibility is 
        /// "ExternallyVisible" because this rule only applies to public methods.
        /// </summary>
        public override TargetVisibilities TargetVisibility
        {
            get
            {
                return TargetVisibilities.ExternallyVisible;
            }
        }

        /// <summary>
        /// Performs the check for the members of a tested class. For each member of the class
        /// this method will be called once to search for a problem.
        /// </summary>
        /// <param name="member"> The member to be tested - will be filtered to non-abstract methods. </param>
        /// <returns> A collection of detected problems. </returns>
        public override ProblemCollection Check(Member member)
        {
            var method = member as Method;

            // only check methods, that are not abstract and do not come from the framework, 
            if (method == null 
                || method.IsAbstract
                || method.DeclaringType.DeclaringModule.ContainingAssembly == FrameworkAssemblies.Mscorlib)
            {
                return null;
            }

            var enumerator = method.Instructions.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var current = enumerator.Current;
                
                switch (current.OpCode)
                {
                    case OpCode.Call:
                    case OpCode.Callvirt:
                        // here we call a method, so we should check if this method call is a 
                        // guard class method call - if not, we will not continue searching
                        var call = (Method)current.Value;
                        if (call.DeclaringType.Name.Name == "String")
                        {
                            if (enumerator.MoveNext())
                            {
                                var nextStatement = enumerator.Current;
                                if (nextStatement.OpCode == OpCode.Pop)
                                {
                                    this.AddProblem(null, member);
                                }
                            }
                        }

                        break;

                    default:
                        break;
                }
            }

            return this.Problems;
        }
    }
}