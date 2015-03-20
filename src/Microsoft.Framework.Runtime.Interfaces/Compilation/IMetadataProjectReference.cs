// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Microsoft.Framework.Runtime.Compilation
{
    public interface IMetadataProjectReference : IMetadataReference
    {
        string ProjectPath { get; }

        IDiagnosticResult GetDiagnostics();

        IList<ISourceReference> GetSources();

        Assembly Load(IAssemblyLoadContext loadContext);

        void EmitReferenceAssembly(Stream stream);

        IDiagnosticResult EmitAssembly(string outputPath);
    }
}
