// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GuardClass.cs" company="Sven Erik Matzen">
//   Copyright (c) Sven Erik Matzen. GNU Library General Public License (LGPL) Version 2.1.
// </copyright>
// <summary>
//   Contains class definition GuardClass.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CaRulesTarget
{
    using System;

    /// <summary>
    /// This is a very simple implementation of a guard class.
    /// </summary>
    public static class GuardClass
    {
        public static void EnsureNotNull<T>(T parameter, string parameterName)
            where T : class
        {
            if (parameter == null)
            {
                throw new ArgumentNullException(parameterName);
            }
        }

        public static void EnsureNotNullOrEmpty(string parameter, string parameterName)
        {
            if (string.IsNullOrEmpty(parameter))
            {
                throw new ArgumentNullException(parameterName);
            }
        }

        public static void EnsureNotZero(int parameter, string parameterName)
        {
            if (parameter == 0)
            {
                throw new ArgumentNullException(parameterName);
            }
        }
    }
}
