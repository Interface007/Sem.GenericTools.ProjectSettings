// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReferenceInformation.cs" company="Sven Erik Matzen">
//   Copyright (c) Sven Erik Matzen. GNU Library General Public License (LGPL) Version 2.1.
// </copyright>
// <summary>
//   Defines the ReferenceInformation type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sem.GenericTools.ProjectSettings
{
    internal class ReferenceInformation
    {
        public string Target { get; set; }

        public bool SpecificVersion { get; set; }

        public string HintPath { get; set; }

        public string Project { get; set; }
    }
}