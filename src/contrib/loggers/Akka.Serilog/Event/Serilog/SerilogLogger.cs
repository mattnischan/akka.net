﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;
using Serilog;

namespace Akka.Serilog.Event.Serilog
{
    public class SerilogLogger:ReceiveActor
    {
        private readonly LoggingAdapter log = Logging.GetLogger(Context);

        private void WithSerilog(Action<ILogger> logStatement)
        {
            var logger = Log.Logger.ForContext(GetType());
            logStatement(logger);
        }

        private ILogger SetContextFromLogEvent(ILogger logger, LogEvent logEvent)
        {
            logger.ForContext("Timestamp", logEvent.Timestamp);
            logger.ForContext("LogSource", logEvent.LogSource);
            logger.ForContext("Thread", logEvent.Thread);
            return logger;
        }

        protected SerilogLogger()
        {
            Receive<Error>(m => WithSerilog(logger => SetContextFromLogEvent(logger,m).Error(m.Cause,"{Message}", m.Message)));
            Receive<Warning>(m => WithSerilog(logger => SetContextFromLogEvent(logger, m).Warning("{Message}", m.Message)));
            Receive<Info>(m => WithSerilog(logger => SetContextFromLogEvent(logger, m).Information("{Message}", m.Message)));
            Receive<Debug>(m => WithSerilog(logger => SetContextFromLogEvent(logger, m).Debug("{Message}", m.Message)));
            Receive<InitializeLogger>(m =>
            {
                log.Info("SerilogLogger started");
                Sender.Tell(new LoggerInitialized());
            });
        }
    }
}
