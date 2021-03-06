﻿using System;
using System.Diagnostics;
using System.Reflection;
using PlayPass.Engine;
using PlayPass.Engine.Extensions;

namespace PlayPass
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            var logManager = new LogManager();
            try
            {
                // Initialize Queue List Factory
                FileQueueList.RegisterClass();
                MemoryQueueList.RegisterClass();

                // Initialize Logger Factory
                TextFileLogger.RegisterClass();

                var commandLine = new CommandLineParser(args);
                var config = new ConfigReader(commandLine.ConfigFileName);

                // Initialize Loggers
                logManager.Loggers.Add(new ConsoleLogger {VerboseMode = commandLine.VerboseMode});
                config.GetLoggers(logManager.Loggers);

                logManager.Log("PlayPass Auto Queueing Engine Version {0}", Version());

                // Initialize PlayOn
                var playOn = config.GetPlayOn();
                logManager.LogVerbose("Connecting to {0}:{1}...", playOn.ServerHost, playOn.ServerPort);

                // Hook into XmlRequestEvent for debug logging
                playOn.XmlRequestEvent += logManager.XmlRequestEventHandler;

                // Initialize QueueValidator
                var queueList = config.GetQueueList();
                var queueValidator = config.GetQueueValidator(queueList);

                // Initialize PlayPassProcessor
                var playPass = new PlayPassProcessor(playOn, logManager, queueValidator)
                {
                    QueueMode = commandLine.QueueMode,
                    SkipMode = commandLine.SkipMode
                };

                // Load and process Passes
                var passes = new PassItems();
                config.GetPasses(passes);

                playPass.ProcessPasses(passes);
            }
            catch (Exception ex)
            {
                if (logManager.Loggers.Count == 0)
                {
                    Console.WriteLine("The following exception has occurred:\n   " + ex.Message);
                    if (!(ex is ApplicationException))
                        Console.WriteLine("Stack Trace: " + ex);
                }
                logManager.LogException(ex);
            }
        }

        /// <summary>
        ///     Returns the version number of the program
        /// </summary>
        private static string Version()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            return fileVersionInfo.ProductVersion;
        }
    }
}