﻿using System;

namespace PlayPass.Engine.Extensions
{
    /// <summary>
    ///     A custom logger that outputs to the console.
    /// </summary>
    public class ConsoleLogger : ILogger
    {
        private int _logDepth;
        public bool DebugMode { private get; set; }
        public bool VerboseMode { private get; set; }

        public void DecrementLogDepth(bool verboseMode)
        {
            if (!verboseMode || VerboseMode)
                _logDepth--;
        }

        public void IncrementLogDepth(bool verboseMode)
        {
            if (!verboseMode || VerboseMode)
                _logDepth++;
        }

        public void Initialize(string connectionString)
        {
            // Do nothing
        }

        public void Log(DateTime dateTime, string msg)
        {
            Console.WriteLine(Padding() + msg);
        }

        public void LogDebug(DateTime dateTime, string msg)
        {
            if (DebugMode)
                Log(dateTime, msg);
        }

        public void LogException(DateTime dateTime, Exception exception)
        {
            Log(dateTime, "The following exception has occurred: " + exception.Message);
            if (!(exception is ApplicationException))
                Log(dateTime, "Stack Trace: " + exception);
        }

        public void LogVerbose(DateTime dateTime, string msg)
        {
            if (VerboseMode)
                Log(dateTime, msg);
        }

        private string Padding()
        {
            return new String(' ', _logDepth * 2);
        }
    }
}