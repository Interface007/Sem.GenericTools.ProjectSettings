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
    public class PublicMethodParametersMustBeCheckedByGuardClassMethod : SemRule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PublicMethodParametersMustBeCheckedByGuardClassMethod"/> class that
        /// defines a CA rule. This constructor takes the PublicMethodParametersMustBeCheckedByGuardClassMethod type to 
        /// look up definitions from the Rules.XML.
        /// </summary>
        public PublicMethodParametersMustBeCheckedByGuardClassMethod()
            : base(typeof(PublicMethodParametersMustBeCheckedByGuardClassMethod).Name)
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
            // also don't check the guard class itself
            if (method == null 
                || method.IsAbstract
                || method.DeclaringType.DeclaringModule.ContainingAssembly == FrameworkAssemblies.Mscorlib
                || IsGuardClass(method.DeclaringType))
            {
                return null;
            }

            // get all "checked" parameters
            var checkedParameters = GetCheckedParameters(method);

            // enumerate all parameters of the method 
            // and test if they are in the list of checked parameters
            foreach (var parameter in method.Parameters)
            {
                var parameterName = parameter.Name.Name;
                if (checkedParameters.Contains(parameterName))
                {
                    continue;
                }

                // add a problem for each non-checked parameter
                this.AddProblem(parameterName, member);
            }

            return this.Problems;
        }

        /// <summary>
        /// Analyzes the method for parameters that are checked correctly.
        /// </summary>
        /// <param name="method"> The method to be analyzed. </param>
        /// <returns> A list of parameter names that are checked by a GuardClass befor any other code is called. </returns>
        private static List<string> GetCheckedParameters(Method method)
        {
            var result = new List<string>();
            
            // for static methods the index is different, because the parameter[0] is the "this-pointer"
            var isStatic = method.IsStatic;

            var parameterName = string.Empty;
            var enumerator = method.Instructions.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var current = enumerator.Current;
                var parameterCollection = method.Parameters;
                
                switch (current.OpCode)
                {
                    case OpCode.Nop:
                    case OpCode._Locals:
                    case OpCode.Box:
                    case OpCode.Ldstr:
                        // simply skip expected opcodes that prepare the GuardClass.Method()-call
                        break;

                    case OpCode.Ldarg_0:
                        // set the parameter the next call will use
                        parameterName = GetParameterName(parameterCollection, 0, isStatic);
                        break;

                    case OpCode.Ldarg_1:
                        // set the parameter the next call will use
                        parameterName = GetParameterName(parameterCollection, 1, isStatic);
                        break;

                    case OpCode.Ldarg_2:
                        // set the parameter the next call will use
                        parameterName = GetParameterName(parameterCollection, 2, isStatic);
                        break;

                    case OpCode.Ldarg_3:
                        // set the parameter the next call will use
                        parameterName = GetParameterName(parameterCollection, 3, isStatic);
                        break;

                    case OpCode.Ldarg_S:
                        // set the parameter the next call will use
                        var parameter = (Parameter)current.Value;
                        parameterName = parameter.Name.Name;
                        break;

                    case OpCode.Call:
                    case OpCode.Callvirt:
                        // here we call a method, so we should check if this method call is a 
                        // guard class method call - if not, we will not continue searching
                        var call = (Method)current.Value;
                        if (!IsGuardClass(call.DeclaringType))
                        {
                            return result;
                        }

                        // the method call calls the guard class, so we add 
                        // this parameter to the list of checked parameters
                        result.Add(parameterName);
                        parameterName = string.Empty;
                        break;

                    case OpCode.Ret:
                        // known and expected returns
                        return result;

                    default:
                        // set a breakpoint here to stop at an unexpected opcode.
                        return result;
                }
            }

            return result;
        }

        /// <summary>
        /// Extracts the parameter name by the index of the IL parameter list. For the opcode
        /// <see cref="OpCode.Ldarg_3"/> the <paramref name="index"/> is 3. This method will
        /// adapt to static and nonstatic methods (c#) by skipping the "this" parameter of
        /// nonstatic methods.
        /// </summary>
        /// <param name="parameterCollection"> The parameter collection of the FXCop analyzed method. </param>
        /// <param name="index"> The index determined by the IL code analysis. </param>
        /// <param name="isStatic"> A value indicating whether the analyzed method is static (true) or not (false). </param>
        /// <returns> The name of the parameter - "(undefined)" is the <paramref name="parameterCollection"/> does not contain a name for the parameter index. </returns>
        private static string GetParameterName(MetadataCollection<Parameter> parameterCollection, int index, bool isStatic)
        {
            if (parameterCollection == null)
            {
                return "(undefined)";
            }

            var listIndex = isStatic ? index : index - 1;
            
            return
                parameterCollection.Count > listIndex && listIndex > -1
                ? parameterCollection[listIndex].Name.Name 
                : "(undefined)";
        }

        /// <summary>
        /// Analyzes a type to be a GuardClass or not. The current implementation does simply check the name
        /// of the class and its base class to be "GuardClass".
        /// </summary>
        /// <param name="type"> The type to be checked. </param>
        /// <returns> true in case of a GuardClass </returns>
        private static bool IsGuardClass(TypeNode type)
        {
            return
                type.FullName.EndsWith(".GuardClass")
                || (type.BaseType != null
                    && type.BaseType.FullName.EndsWith(".GuardClass"));
        }
    }
}