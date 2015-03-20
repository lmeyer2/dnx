using System.Collections.Generic;
using System.Linq;
using Microsoft.Framework.Runtime.Compilation;

namespace Microsoft.Framework.Runtime
{
    public struct DiagnosticResult : IDiagnosticResult
    {
        public static readonly DiagnosticResult Successful = new DiagnosticResult(success: true,
                                                                                  diagnostics: Enumerable.Empty<ICompilationMessage>());

        public DiagnosticResult(bool success, IEnumerable<ICompilationMessage> diagnostics)
        {
            Success = success;
            Diagnostics = diagnostics.ToList();
        }

        public bool Success { get; }

        public IEnumerable<ICompilationMessage> Diagnostics { get; }
    }
}