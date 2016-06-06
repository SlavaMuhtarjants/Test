namespace Weezlabs.Storgage.RestApi.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Web.Http.Tracing;

    using NLog;
    using Newtonsoft.Json;

    /// <summary>
    /// Class to Log Error/info messages to the access Log file
    /// </summary>
    public sealed class NLogger : ITraceWriter
    {
        #region Public member methods.
        /// <summary>
        /// Instantiate NLogger class
        /// </summary>
        /// <param name="loggerName">Name of a logger in the NLog configuration file</param>
        public NLogger(String loggerName = null)
        {
            classLogger = loggerName == null ? LogManager.GetCurrentClassLogger() : LogManager.GetLogger(loggerName);
            loggingMap = new Lazy<Dictionary<TraceLevel, Action<String>>>(() =>
                new Dictionary<TraceLevel, Action<String>>
                {
                    {TraceLevel.Info, classLogger.Info},
                    {TraceLevel.Debug, classLogger.Debug},
                    {TraceLevel.Error, classLogger.Error},
                    {TraceLevel.Fatal, classLogger.Fatal},
                    {TraceLevel.Warn, classLogger.Warn}
                });
    }

    /// <summary>
    /// Implementation of TraceWriter to trace the logs.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="category"></param>
    /// <param name="level"></param>
    /// <param name="traceAction"></param>
    public void Trace(HttpRequestMessage request, String category, TraceLevel level, Action<TraceRecord> traceAction)
        {
            if (level != TraceLevel.Off)
            {
                if (traceAction?.Target != null)
                {
                    category = category + Environment.NewLine + "Action Parameters : " + JsonConvert.SerializeObject(traceAction.Target);
                }
                TraceRecord record = new TraceRecord(request, category, level);
                if (traceAction != null) traceAction(record);
                Log(record);
            }
        }

        #endregion

        #region Private member variables.

        private readonly Logger classLogger;

        private readonly Lazy<Dictionary<TraceLevel, Action<String>>> loggingMap;

        #endregion

        #region Private properties.

        /// <summary>
        /// Get property for Logger
        /// </summary>
        private Dictionary<TraceLevel, Action<String>> Logger => loggingMap.Value;

        #endregion
        
        #region Private member methods.

        /// <summary>
        /// Logs info/Error to Log file
        /// </summary>
        /// <param name="record"></param>
        private void Log(TraceRecord record)
        {
            StringBuilder message = new StringBuilder();

            if (!String.IsNullOrWhiteSpace(record.Message))
                message.Append("").Append(record.Message + Environment.NewLine);

            if (record.Request != null)
            {
                if (record.Request.Method != null)
                    message.Append("Method: " + record.Request.Method + Environment.NewLine);

                if (record.Request.RequestUri != null)
                    message.Append("").Append("URL: " + record.Request.RequestUri + Environment.NewLine);

                if (record.Request.Headers != null && record.Request.Headers.Contains("Token") &&
                    record.Request.Headers.GetValues("Token") != null &&
                    record.Request.Headers.GetValues("Token").FirstOrDefault() != null)
                    message.Append("")
                        .Append("Token: " + record.Request.Headers.GetValues("Token").FirstOrDefault() +
                                Environment.NewLine);
            }

            if (!String.IsNullOrWhiteSpace(record.Category))
                message.Append("").Append(record.Category);

            if (!String.IsNullOrWhiteSpace(record.Operator))
                message.Append(" ").Append(record.Operator).Append(" ").Append(record.Operation);

            if (!String.IsNullOrWhiteSpace(record.Exception?.GetBaseException().Message))
            {
                Type exceptionType = record.Exception.GetType();
                message.Append(Environment.NewLine);
                message.Append("").Append("Error: " + record.Exception.GetBaseException().Message + Environment.NewLine);
            }

            Logger[record.Level](Convert.ToString(message) + Environment.NewLine);
        }

        #endregion
    }
}