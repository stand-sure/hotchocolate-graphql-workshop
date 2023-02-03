namespace ConferencePlanner.Service;

using System;
using System.Diagnostics;
using System.Linq;
using System.Text;

using HotChocolate.Execution;
using HotChocolate.Execution.Instrumentation;
using HotChocolate.Types;

using Microsoft.Extensions.Logging;

internal class QueryLoggerDiagnosticEventListener : ExecutionDiagnosticEventListener
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="QueryLoggerDiagnosticEventListener" /> class.
    /// </summary>
    public QueryLoggerDiagnosticEventListener(ILogger<QueryLoggerDiagnosticEventListener> logger)
    {
        this.Logger = logger;
    }

    private ILogger<QueryLoggerDiagnosticEventListener> Logger { get; }

    public override IDisposable ExecuteRequest(IRequestContext context)
    {
        return new RequestExecutionScope(this.Logger, context);
    }

    private sealed class RequestExecutionScope : IDisposable
    {
        private static readonly string Pad = new(' ', 10);

        private readonly Stopwatch queryTimer;

        /// <summary>
        ///     Initializes a new instance of the <see cref="RequestExecutionScope" /> class.
        /// </summary>
        public RequestExecutionScope(
            ILogger<QueryLoggerDiagnosticEventListener> logger,
            IRequestContext context)
        {
            this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.Context = context ?? throw new ArgumentNullException(nameof(context));

            this.queryTimer = new Stopwatch();
            this.queryTimer.Start();
        }

        public void Dispose()
        {
            if (this.Context.Document is null)
            {
                this.queryTimer.Stop();
                return;
            }

            var builder = new StringBuilder(this.Context.Document.ToString(true));
            builder.AppendLine();

            foreach (VariableValue contextVariable in this.Context.Variables?.AsEnumerable() ?? Enumerable.Empty<VariableValue>())
            {
                try
                {
                    builder.AppendLine($"{RequestExecutionScope.Pad}${contextVariable.Name} [{contextVariable.Type} {contextVariable.Type.TypeName()}]");
                    builder.AppendLine($"{RequestExecutionScope.Pad}{RequestExecutionScope.Pad}{contextVariable.Value}");
                }
                catch
                {
                    builder.AppendLine();
                }
            }

            this.queryTimer.Stop();

            if (this.Context.ContextData.TryGetValue("HotChocolate.Execution.OperationComplexity", out object? complexity))
            {
                builder.AppendLine($"Complexity: {complexity}");
            }

            builder.AppendLine($"Elapsed time: {this.queryTimer.Elapsed.TotalMilliseconds} ms");

            this.Logger.LogInformation("{message}", builder.ToString());
        }

        private IRequestContext Context { get; }

        private ILogger<QueryLoggerDiagnosticEventListener> Logger { get; }
    }
}