//-----------------------------------------------------------------------
// <copyright file="Extensions.cs" company="Sven Erik Matzen">
//     Copyright (c) Sven Erik Matzen. GNU Library General Public License (LGPL) Version 2.1.
// </copyright>
// <author>Sven Erik Matzen</author>
//-----------------------------------------------------------------------
namespace Sem.GenericTools.ProjectSettings
{
    using System.Collections.Generic;

    /// <summary>
    /// Extension methods to ease the working
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Tests a a Dictionary of values for being NULL or not containing the desired Key
        /// and returns a new object of the values type of the Dictionary if either the Dictionary
        /// is null or the element does not exist.
        /// Returns the Dictionary element if it does exist.
        /// </summary>
        /// <param name="testObject"> The Dictionary of elements.  </param>
        /// <param name="key"> The key of the element to search. </param>
        /// <typeparam name="T"> The type of elements inside the Dictionary </typeparam>
        /// <returns> the existing element or a new element, if the element does not exist  </returns>
        public static T NewIfNull<T>(this Dictionary<string, T> testObject, string key) where T : class, new()
        {
            var x = testObject ?? new Dictionary<string, T>();
            return x.ContainsKey(key) ? x[key] : new T();
        }
    }
}
