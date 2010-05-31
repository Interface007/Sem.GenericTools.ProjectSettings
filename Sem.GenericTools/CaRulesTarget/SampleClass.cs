// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SampleClass.cs" company="Sven Erik Matzen">
//   Copyright (c) Sven Erik Matzen. GNU Library General Public License (LGPL) Version 2.1.
// </copyright>
// <summary>
//   Defines the SampleClass type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CaRulesTarget
{
    using System;

    public class SampleClass
    {
        public void NonStaticMethodOneParameter(string message)
        {
            GuardClass.EnsureNotNullOrEmpty(message, "message");
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
