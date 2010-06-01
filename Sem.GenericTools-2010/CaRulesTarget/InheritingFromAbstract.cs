// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InheritingFromAbstract.cs" company="Sven Erik Matzen">
//   Copyright (c) Sven Erik Matzen. GNU Library General Public License (LGPL) Version 2.1.
// </copyright>
// <summary>
//   Defines the InheritingFromAbstract type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CaRulesTarget
{
    public class InheritingFromAbstract : AbstractSampleClass
    {
        public override string NonStaticAbstractMethodWithResultOneParameter(string message)
        {
            GuardClass.EnsureNotNullOrEmpty(message, "message");
            return message;
        }
    }
}
