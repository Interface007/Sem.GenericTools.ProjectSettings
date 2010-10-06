// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicSample.cs" company="Sven Erik Matzen">
//   Copyright (c) Sven Erik Matzen. GNU Library General Public License (LGPL) Version 2.1.
// </copyright>
// <summary>
//   Defines the DynamicSample type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CaRulesTarget
{
    using System;

    public class DynamicSample
    {
        public void SomeDynamicVariables(dynamic myVariable)
        {
            myVariable.Hello = "Hello";
            Console.Write(myVariable.Hello);
        }
    }
}
