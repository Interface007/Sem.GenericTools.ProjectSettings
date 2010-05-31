// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ToStringSample.cs" company="Sven Erik Matzen">
//   Copyright (c) Sven Erik Matzen. GNU Library General Public License (LGPL) Version 2.1.
// </copyright>
// <summary>
//   Defines the ToStringSample type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CaRulesTarget
{
    using System;

    public class ToStringSample
    {
        public void PrintMessage(string message)
        {
            GuardClass.EnsureNotNullOrEmpty(message, "message");
            
            // this would fire SEM1001 PublicMethodParametersMustBeCheckedByGuardClassMethod
            ////Console.WriteLine(message.ToString());

            // this would fire SEM2001 ToStringMustNotBeCalledForStringObjects
            ////Console.WriteLine("http://x/".ToString());
            
            // this does not
            Console.WriteLine(message, "message");
            Console.WriteLine(7.ToString());
            Console.WriteLine(EnvironmentVariableTarget.User.ToString());
            Console.WriteLine(new System.Uri("http://x/").ToString());
        }
    }
}
