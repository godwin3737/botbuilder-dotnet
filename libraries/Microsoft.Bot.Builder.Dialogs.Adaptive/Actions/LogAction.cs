﻿// Licensed under the MIT License.
// Copyright (c) Microsoft Corporation. All rights reserved.

using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Templates;
using Microsoft.Bot.Expressions;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Microsoft.Bot.Builder.Dialogs.Adaptive.Actions
{
    /// <summary>
    /// Write entry into application trace logs (Trace.TraceInformation).
    /// </summary>
    public class LogAction : Dialog
    {
        [JsonProperty("$kind")]
        public const string DeclarativeType = "Microsoft.LogAction";

        [JsonConstructor]
        public LogAction(string text = null, [CallerFilePath] string callerPath = "", [CallerLineNumber] int callerLine = 0)
        {
            this.RegisterSourceLocation(callerPath, callerLine);
            if (text != null)
            {
                Text = new TextTemplate(text);
            }
        }

        /// <summary>
        /// Gets or sets an optional expression which if is true will disable this action.
        /// </summary>
        /// <example>
        /// "user.age > 18".
        /// </example>
        /// <value>
        /// A boolean expression. 
        /// </value>
        [JsonProperty("disabled")]
        public BoolExpression Disabled { get; set; }

        /// <summary>
        /// Gets or sets lG expression to log.
        /// </summary>
        /// <value>
        /// LG expression to log.
        /// </value>
        [JsonProperty("text")]
        public ITemplate<string> Text { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a TraceActivity will be sent in addition to console log.
        /// </summary>
        /// <value>
        /// Whether a TraceActivity will be sent in addition to console log.
        /// </value>
        [JsonProperty("traceActivity")]
        public BoolExpression TraceActivity { get; set; } = false;

        /// <summary>
        /// Gets or sets a label to use when describing a trace activity.
        /// </summary>
        /// <value>The label to use. (default is the id of the action).</value>
        [JsonProperty]
        public StringExpression Label { get; set; }

        public override async Task<DialogTurnResult> BeginDialogAsync(DialogContext dc, object options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (options is CancellationToken)
            {
                throw new ArgumentException($"{nameof(options)} cannot be a cancellation token");
            }

            var dcState = dc.GetState();

            if (this.Disabled != null && this.Disabled.GetValue(dcState) == true)
            {
                return await dc.EndDialogAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
            }

            var text = await Text.BindToData(dc.Context, dcState).ConfigureAwait(false);

            System.Diagnostics.Trace.TraceInformation(text);

            if (this.TraceActivity.GetValue(dcState))
            {
                var traceActivity = Activity.CreateTraceActivity(name: "Log", valueType: "Text", value: text, label: this.Label?.GetValue(dcState) ?? dc.Parent?.ActiveDialog?.Id);
                await dc.Context.SendActivityAsync(traceActivity, cancellationToken).ConfigureAwait(false);
            }

            return await dc.EndDialogAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        protected override string OnComputeId()
        {
            return $"{this.GetType().Name}({Text?.ToString()})";
        }
    }
}
