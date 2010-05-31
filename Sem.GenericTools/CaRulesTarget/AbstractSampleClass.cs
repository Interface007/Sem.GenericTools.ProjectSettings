// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AbstractSampleClass.cs" company="Sven Erik Matzen">
//   Copyright (c) Sven Erik Matzen. GNU Library General Public License (LGPL) Version 2.1.
// </copyright>
// <summary>
//   Defines the AbstractSampleClass type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CaRulesTarget
{
    using System;

    /// <summary>
    /// Sample class for testing the Sem.GenericTools.CodeAnalysis for false positives.
    /// </summary>
    public abstract class AbstractSampleClass
    {
        public void NonStaticMethodOneParameter(string message)
        {
            GuardClass.EnsureNotNullOrEmpty(message, "message");
            Console.WriteLine(message, "message");
        }

        public static void StaticMethodOneParameter(string message)
        {
            GuardClass.EnsureNotNullOrEmpty(message, "message");
            Console.WriteLine(message, "message");
        }

        public string NonStaticMethodWithResultOneParameter(string message)
        {
            GuardClass.EnsureNotNullOrEmpty(message, "message");
            return message;
        }

        public static string StaticMethodWithResultOneParameter(string message)
        {
            GuardClass.EnsureNotNullOrEmpty(message, "message");
            return message;
        }

        public abstract string NonStaticAbstractMethodWithResultOneParameter(string message);
    }
}
