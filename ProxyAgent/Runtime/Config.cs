﻿// Copyright (c) Microsoft. All rights reserved.

// TODO: tests
// TODO: handle errors
// TODO: use binding

using System;
using Microsoft.Azure.IoTSolutions.ReverseProxy.Diagnostics;
using Microsoft.Azure.IoTSolutions.ReverseProxy.Exceptions;

namespace Microsoft.Azure.IoTSolutions.ReverseProxy.Runtime
{
    public interface IConfig
    {
        // The remote endpoint the proxy communicates with
        string Endpoint { get; }

        // The remote endpoint ssl certificate thumbprint the proxy communicates with
        string SslCertThumbprint { get; }

        // Whether to redirect HTTP requests to HTTPS
        bool RedirectHttpToHttps { get; }

        // Whether to tell clients to default to HTTPS in the future
        bool StrictTransportSecurityEnabled { get; }

        // How long clients should persist the HSTS rule, specified in seconds
        int StrictTransportSecurityPeriod { get; }

        // Maximum request payload size
        int MaxPayloadSize { get; }

        // Whether to expose configuration settings in /status
        bool StatusEndpointEnabled { get; }

        // Application logging level
        LogLevel LogLevel { get; }
    }

    /// <summary>Web application configuration</summary>
    public class Config : IConfig
    {
        private const bool REDIRECT_HTTP_DEFAULT = true;
        private const bool STS_ENABLED_DEFAULT = true;
        // By default browsers will persist the HSTS rule for 1 month.
        private const int STS_PERIOD_DEFAULT = 2592000;
        // By default requests with a payload bigger than 100Kb are refused.
        // In IoT scenarios the setting works fine because requests typically are limited to 64Kb.
        private const int MAX_PAYLOAD_SIZE_DEFAULT = 102400;
        private const bool STATUS_ENDPOINT_ENABLED_DEFAULT = false;
        private const LogLevel LOG_LEVEL_DEFAULT = LogLevel.Warn;

        private const string APPLICATION_KEY = "ReverseProxy:";
        private const string ENDPOINT_KEY = APPLICATION_KEY + "endpoint";
        private const string SSL_CERT_THUMBPRINT_KEY = APPLICATION_KEY + "ssl_cert_thumbprint";
        private const string REDIRECT_HTTP_KEY = APPLICATION_KEY + "redirectHttpToHttps";
        private const string STS_ENABLED_KEY = APPLICATION_KEY + "strictTransportSecurityEnabled";
        private const string STS_PERIOD_KEY = APPLICATION_KEY + "strictTransportSecurityPeriod";
        private const string MAX_PAYLOAD_SIZE_KEY = APPLICATION_KEY + "maxPayloadSize";
        private const string STATUS_ENDPOINT_ENABLED_KEY = APPLICATION_KEY + "statusEndpointEnabled";
        private const string LOG_LEVEL_KEY = APPLICATION_KEY + "loglevel";

        public string Endpoint { get; set; }
        public string SslCertThumbprint { get; set; }
        public bool RedirectHttpToHttps { get; set; }
        public bool StrictTransportSecurityEnabled { get; set; }
        public int StrictTransportSecurityPeriod { get; set; }
        public int MaxPayloadSize { get; set; }
        public bool StatusEndpointEnabled { get; set; }
        public LogLevel LogLevel { get; set; }

        public Config()
        {
            
        }
        
        public Config(IConfigData configData)
        {
            Enum.TryParse(configData.GetString(LOG_LEVEL_KEY, LOG_LEVEL_DEFAULT.ToString()), out LogLevel logLevel);
            this.LogLevel = logLevel;

            this.StatusEndpointEnabled = configData.GetBool(STATUS_ENDPOINT_ENABLED_KEY, STATUS_ENDPOINT_ENABLED_DEFAULT);
            this.MaxPayloadSize = configData.GetInt(MAX_PAYLOAD_SIZE_KEY, MAX_PAYLOAD_SIZE_DEFAULT);

            this.RedirectHttpToHttps = configData.GetBool(REDIRECT_HTTP_KEY, REDIRECT_HTTP_DEFAULT);
            this.StrictTransportSecurityEnabled = configData.GetBool(STS_ENABLED_KEY, STS_ENABLED_DEFAULT);
            this.StrictTransportSecurityPeriod = configData.GetInt(STS_PERIOD_KEY, STS_PERIOD_DEFAULT);

            this.Endpoint = configData.GetString(ENDPOINT_KEY);
            this.SslCertThumbprint = configData.GetString(SSL_CERT_THUMBPRINT_KEY);
            if (string.IsNullOrEmpty(this.Endpoint))
            {
                throw new InvalidConfigurationException("The remote endpoint hostname is empty.");
            }
        }
    }
}
