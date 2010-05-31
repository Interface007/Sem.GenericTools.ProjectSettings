namespace Sem.GenericTools.CodeAnalysis.BestPractice
{
    using System;
    using System.Collections.Generic;
    using Microsoft.FxCop.Sdk;

    public class PublicMethodParametersMustBeCheckedByGuardClassMethod : SemRule
    {
        public PublicMethodParametersMustBeCheckedByGuardClassMethod()
            : base(typeof(PublicMethodParametersMustBeCheckedByGuardClassMethod).Name)
        {
        }

        public override ProblemCollection Check(Member member)
        {
            var method = member as Method;
            if (method == null)
            {
                return null;
            }

            if (method.DeclaringType.DeclaringModule.ContainingAssembly == FrameworkAssemblies.Mscorlib)
            {
                return null;
            }

            if (IsGuardClass(method.DeclaringType))
            {
                return null;
            }

            var checkedParameters = new List<string>();
            try
            {
                checkedParameters = this.GetCheckedParameters(method);
            }
            catch (Exception ex)
            {
                AddProblem(">>" + ex.Message + "<< " + ex.StackTrace, method);
            }

            foreach (var parameter in method.Parameters)
            {
                var parameterName = parameter.Name.Name;
                if (checkedParameters.Contains(parameterName))
                {
                    continue;
                }

                this.AddProblem(parameterName, member);
            }

            return this.Problems;
        }

        private List<string> GetCheckedParameters(Method method)
        {
            var checkedParameters = new List<string>();
            var enumerator = method.Instructions.GetEnumerator();

            var isStatic = method.IsStatic;

            var parameterName = string.Empty;
            while (enumerator.MoveNext())
            {
                var current = enumerator.Current;
                var parameterCollection = method.Parameters;
                
                switch (current.OpCode)
                {
                    case OpCode.Nop:
                    case OpCode.Box:
                    case OpCode.Ldstr:
                        break;

                    case OpCode.Ldarg_0:
                        parameterName = GetParameterName(parameterCollection, 0, isStatic);
                        break;

                    case OpCode.Ldarg_1:
                        parameterName = GetParameterName(parameterCollection, 1, isStatic);
                        break;

                    case OpCode.Ldarg_2:
                        parameterName = GetParameterName(parameterCollection, 2, isStatic);
                        break;

                    case OpCode.Ldarg_3:
                        parameterName = GetParameterName(parameterCollection, 3, isStatic);
                        break;

                    case OpCode.Ldarg_S:
                        var parameter = (Parameter)current.Value;
                        parameterName = parameter.Name.Name;
                        break;

                    case OpCode.Call:
                    case OpCode.Callvirt:
                        var call = (Method)current.Value;
                        if (!IsGuardClass(call.DeclaringType))
                        {
                            return checkedParameters;
                        }

                        checkedParameters.Add(parameterName);
                        parameterName = string.Empty;
                        break;
                }
            }

            return checkedParameters;
        }

        private static string GetParameterName(ParameterCollection parameterCollection, int index, bool isStatic)
        {
            var listIndex = isStatic ? index : index - 1;
            
            return
                parameterCollection.Count > listIndex && listIndex > -1
                ? parameterCollection[listIndex].Name.Name 
                : "(undefined)";
        }

        private static bool IsGuardClass(TypeNode declaringType)
        {
            return
                declaringType.FullName.EndsWith(".GuardClass")
                || 
                (declaringType.BaseType != null
                && declaringType.BaseType.FullName.EndsWith(".GuardClass"));
        }
    }
}