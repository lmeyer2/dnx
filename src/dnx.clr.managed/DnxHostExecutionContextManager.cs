// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Globalization;
using System.Threading;

namespace dnx.clr.managed
{
    internal class DnxHostExecutionContextManager : HostExecutionContextManager
    {
        private delegate void RevertAction();

        public override HostExecutionContext Capture()
        {
            return new DnxHostExecutionContext(base.Capture(), Thread.CurrentThread.CurrentCulture);
        }

        public override object SetHostExecutionContext(HostExecutionContext hostExecutionContext)
        {
            var castHostExecutionContext = hostExecutionContext as DnxHostExecutionContext;
            if (castHostExecutionContext != null)
            {
                object baseRevertParameter = null;
                if (castHostExecutionContext.BaseContext != null)
                {
                    baseRevertParameter = base.SetHostExecutionContext(castHostExecutionContext.BaseContext);
                }

                var originalCulture = Thread.CurrentThread.CurrentCulture;
                Thread.CurrentThread.CurrentCulture = castHostExecutionContext.ClientCulture;

                return (RevertAction)(() =>
                {
                    Thread.CurrentThread.CurrentCulture = originalCulture;
                    if (baseRevertParameter != null)
                    {
                        base.Revert(baseRevertParameter);
                    }
                });
            }
            else
            {
                return base.SetHostExecutionContext(hostExecutionContext);
            }
        }

        public override void Revert(object previousState)
        {
            var revertAction = previousState as RevertAction;
            if (revertAction != null)
            {
                revertAction();
            }
            else
            {
                base.Revert(previousState);
            }
        }

        private class DnxHostExecutionContext : HostExecutionContext
        {
            internal DnxHostExecutionContext(HostExecutionContext baseContext, CultureInfo clientCulture)
            {
                BaseContext = baseContext;
                ClientCulture = clientCulture;
            }

            private DnxHostExecutionContext(DnxHostExecutionContext original)
                : this(CreateCopyHelper(original.BaseContext), original.ClientCulture)
            {
            }

            public HostExecutionContext BaseContext { get; private set; }

            public CultureInfo ClientCulture { get; private set; }

            public override HostExecutionContext CreateCopy()
            {
                return new DnxHostExecutionContext(this);
            }

            private static HostExecutionContext CreateCopyHelper(HostExecutionContext hostExecutionContext)
            {
                return (hostExecutionContext != null) ? hostExecutionContext.CreateCopy() : null;
            }

            public override void Dispose(bool disposing)
            {
                if (disposing && BaseContext != null)
                {
                    BaseContext.Dispose();
                }
            }
        }
    }
}