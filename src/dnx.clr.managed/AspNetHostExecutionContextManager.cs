// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Globalization;
using System.Threading;

namespace dnx.clr.managed
{
    public class AspNetHostExecutionContextManager : HostExecutionContextManager
    {
        private delegate void RevertAction();

        public override HostExecutionContext Capture()
        {
            var currentThread = Thread.CurrentThread;
            Console.WriteLine("[{0}] Capture {1}|{2}", GetType().Name, currentThread.ManagedThreadId, currentThread.CurrentCulture.Name);

            return new AspNetHostExecutionContext(base.Capture(), currentThread.CurrentCulture);
        }

        public override object SetHostExecutionContext(HostExecutionContext hostExecutionContext)
        {
            var castHostExecutionContext = hostExecutionContext as AspNetHostExecutionContext;
            if (castHostExecutionContext != null)
            {
                object baseRevertParameter = null;
                if (castHostExecutionContext.BaseContext != null)
                {
                    baseRevertParameter = base.SetHostExecutionContext(castHostExecutionContext.BaseContext);
                }

                var originalCulture = Thread.CurrentThread.CurrentCulture;
                Thread.CurrentThread.CurrentCulture = castHostExecutionContext.ClientCulture;
                Console.WriteLine("[{0}] Set culture of Context {1} from {2} to {3}", GetType().Name, castHostExecutionContext._id, originalCulture.Name, castHostExecutionContext.ClientCulture.Name);

                return (RevertAction)(() =>
                {
                    Console.WriteLine("[{0}] Revert culture of Context {1} from {2} to {3}", GetType().Name, castHostExecutionContext._id, Thread.CurrentThread.CurrentCulture.Name, originalCulture.Name);
                    Thread.CurrentThread.CurrentCulture = originalCulture;
                    if (baseRevertParameter != null)
                    {
                        base.Revert(baseRevertParameter);
                    }
                });
            }
            else
            {
                Console.WriteLine("[{0}] Set Context {1}", GetType().Name, hostExecutionContext.GetType());
                return base.SetHostExecutionContext(hostExecutionContext);
            }
        }

        public override void Revert(object previousState)
        {
            var revertAction = previousState as RevertAction;
            if (revertAction != null)
            {
                Console.WriteLine("[{0}] Revert - run revert action", GetType().Name);
                revertAction();
            }
            else
            {
                Console.WriteLine("[{0}] Revert - call base", GetType().Name);
                base.Revert(previousState);
            }
        }

        private class AspNetHostExecutionContext : HostExecutionContext
        {
            // TODO: Remove debug only
            private static int s_accumlate = 0;
            public int _id = s_accumlate++;

            internal AspNetHostExecutionContext(HostExecutionContext baseContext, CultureInfo clientCulture)
            {
                BaseContext = baseContext;
                ClientCulture = clientCulture;
            }

            private AspNetHostExecutionContext(AspNetHostExecutionContext original)
                : this(CreateCopyHelper(original.BaseContext), original.ClientCulture)
            {
            }

            public HostExecutionContext BaseContext { get; private set; }

            public CultureInfo ClientCulture { get; private set; }

            public override HostExecutionContext CreateCopy()
            {
                return new AspNetHostExecutionContext(this);
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