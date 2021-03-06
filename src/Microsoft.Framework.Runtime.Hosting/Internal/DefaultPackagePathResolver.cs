// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.IO;
using NuGet.Versioning;

namespace Microsoft.Framework.Runtime.Internal
{
    public class DefaultPackagePathResolver
    {
        private readonly string _path;

        public DefaultPackagePathResolver(string path)
        {
            _path = path;
        }
        
        public virtual string GetInstallPath(string packageId, NuGetVersion version)
        {
            return Path.Combine(_path, GetPackageDirectory(packageId, version));
        }

        public string GetHashPath(string packageId, NuGetVersion version)
        {
            return Path.Combine(GetInstallPath(packageId, version),
                                string.Format("{0}.{1}.nupkg.sha512", packageId, version));
        }

        public virtual string GetPackageDirectory(string packageId, NuGetVersion version)
        {
            return Path.Combine(packageId, version.ToString());
        }
    }
}
