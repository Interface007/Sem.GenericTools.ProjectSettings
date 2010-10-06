// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SampleWithCodeContracts.cs" company="Sven Erik Matzen">
//   Copyright (c) Sven Erik Matzen. GNU Library General Public License (LGPL) Version 2.1.
// </copyright>
// <summary>
//   Defines the SampleWithCodeContracts type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CaRulesTarget
{
    using System;
    using System.Diagnostics.Contracts;

    public class SampleWithCodeContracts
    {
        public void NonStaticMethodOneParameter(string message)
        {
            Contract.Requires(!string.IsNullOrEmpty(message), "message is null or empty");
            Console.WriteLine(message);
        }

        public void NonStaticMethodOneParameterInt(int message)
        {
            GuardClass.EnsureNotZero(message, "message");
            Console.WriteLine(message);
        }

        public static void StaticMethodOneParameter(string message)
        {
            GuardClass.EnsureNotNullOrEmpty(message, "message");
            Console.WriteLine(message);
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
    }
}
