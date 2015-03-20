// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Framework.Runtime.Compilation;

namespace Microsoft.Framework.Runtime
{
    public interface IProjectReferenceProvider
    {
        IMetadataProjectReference GetProjectReference(
            Project project,
            ILibraryKey target,
            Func<ILibraryExport> referenceResolver);
    }
}