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
            Method method = member as Method;
            if (method == null)
            {
                return null;
            }

            if (method.DeclaringType.DeclaringModule.ContainingAssembly == FrameworkAssemblies.Mscorlib)
            {
                return null;
            }

            List<string> checkedParameters = this.GetCheckedParameters(method);
            foreach (var checkedParameter in checkedParameters)
            {
                this.AddProblem(checkedParameter + " is a checked parameter name", member);
            }

            foreach (var parameter in method.Parameters)
            {
                var name = parameter.Name.Name;
                if (!checkedParameters.Contains(name))
                {
                    this.AddProblem(name + " is a not in the list", member);
                    this.AddProblem(name, member);
                }
            }

            return this.Problems;
        }

        private List<string> GetCheckedParameters(Method method)
        {
            List<string> checkedParameters = new List<string>();
            MetadataCollection<Instruction>.Enumerator enumerator = method.Instructions.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Instruction current = enumerator.Current;
                this.AddProblem("instruction " + current.OpCode + " value " + current.Value, null);
                switch (current.OpCode)
                {
                    case OpCode.Ldc_I4_0:
                        //lastField = (Method)current.Value;
                        this.AddProblem("instruction value " + current.Value, null);
                        break;

                    case OpCode.Call:
                    case OpCode.Callvirt:
                        Method call = (Method)current.Value;
                        if (!IsGuardClass(call.DeclaringType))
                        {
                            return checkedParameters;
                        }

                        //foreach (var parameter in callCall.Operands)
                        //{
                        //    this.AddProblem("adding parameter name " + parameter, null);
                        //    checkedParameters.Add(parameter.ToString());
                        //}

                        break;
                }
            }

            return checkedParameters;
        }

        private static bool IsGuardClass(TypeNode declaringType)
        {
            return declaringType.FullName.Contains("Guard");
        }
    }
}
