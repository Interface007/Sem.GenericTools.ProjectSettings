// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ToStringMustNotBeCalledForStringObjects.cs" company="Sven Erik Matzen">
//   Copyright (c) Sven Erik Matzen. GNU Library General Public License (LGPL) Version 2.1.
// </copyright>
// <summary>
//   Defines the ToStringMustNotBeCalledForStringObjects type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sem.GenericTools.CodeAnalysis.BestPractice
{
    using System;

    using Microsoft.FxCop.Sdk;

    /// <summary>
    /// Implements a rule to check whether the method .ToString() is executed on a string.
    /// </summary>
    public class ToStringMustNotBeCalledForStringObjects : SemRule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ToStringMustNotBeCalledForStringObjects"/> class that
        /// defines a CA rule. This constructor takes the ToStringMustNotBeCalledForStringObjects type to 
        /// look up definitions from the Rules.XML.
        /// </summary>
        public ToStringMustNotBeCalledForStringObjects()
            : base(typeof(ToStringMustNotBeCalledForStringObjects).Name)
        {
        }

        /// <summary>
        /// Performs the check for the members of a tested class. For each member of the class
        /// this method will be called once to search for a problem.
        /// </summary>
        /// <param name="type"> The type to be analyzed. </param>
        /// <returns> A collection of detected problems. </returns>
        public override ProblemCollection Check(TypeNode type)
        {
            VisitTypeNode(type);
            return Problems;
        }

        /// <summary>
        /// This method will be called for each method call inside the type. Here we check if the method call is a "ToString()" 
        /// call on a string data type.
        /// </summary>
        /// <param name="call"> The method call reference. </param>
        public override void VisitMethodCall(MethodCall call)
        {
            // do we have a member binding and the member binding has a target object with 
            // a filled type property that is of type string?
            var mb = call.Callee as MemberBinding;
            if (mb == null 
                || mb.TargetObject == null 
                || mb.TargetObject.Type == null 
                || mb.TargetObject.Type.TypeCode != TypeCode.String)
            {
                base.VisitMethodCall(call);
                return;
            }

            // check the method name to be "ToString"
            var method = mb.BoundMember as Method;
            if (method != null 
                && method.Name != null 
                && !string.IsNullOrEmpty(method.Name.Name) 
                && method.Name.Name == "ToString")
            {
                this.AddProblem(null, call);
            }
            
            base.VisitMethodCall(call);
        }
    }
}
