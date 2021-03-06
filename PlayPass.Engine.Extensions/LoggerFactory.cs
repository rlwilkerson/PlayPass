﻿using System;
using System.Collections.Generic;
using System.Data.Common;

namespace PlayPass.Engine.Extensions
{
    /// <summary>
    ///     A factory class that registers and initializes logger classes
    /// </summary>
    public static class LoggerFactory
    {
        private static readonly Dictionary<string, Type> Classes = new Dictionary<string, Type>();

        public static ILogger GetLogger(string connectionString, bool debugMode, bool verboseMode)
        {
            var parser = new DbConnectionStringBuilder {ConnectionString = connectionString};
            if (!parser.ContainsKey("Provider"))
                throw new Exception("Logger Provider Type is not specified");

            var providerType = parser["Provider"].ToString().ToUpper();

            if (!Classes.ContainsKey(providerType))
                throw new Exception(String.Format("Unregistered Logger Provider Type: {0}", providerType));

            var type = Classes[providerType];
            var instance = (ILogger) Activator.CreateInstance(type);
            instance.DebugMode = debugMode;
            instance.VerboseMode = verboseMode;
            instance.Initialize(connectionString);
            return instance;
        }

        public static void RegisterClass(Type type)
        {
            Classes[type.Name.ToUpper()] = type;
        }
    }
}