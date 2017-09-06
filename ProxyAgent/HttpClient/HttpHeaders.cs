// Copyright (c) Microsoft. All rights reserved.

using Microsoft.Azure.IoTSolutions.ReverseProxy.Runtime;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Azure.IoTSolutions.ReverseProxy.HttpClient
{
    public class HttpHeaders : IEnumerable<KeyValuePair<string, List<string>>>
    {
        private readonly IConfig config;

        private readonly Dictionary<string, List<string>> data;

        /// <summary>
        /// Returns the correlation Id of the request if present,
        /// otherwise returns an empty string
        /// </summary>
        public string CorrelationId
        {
            get
            {
                if (this.data.ContainsKey(config.CorrelationHeader))
                {
                    this.data.TryGetValue(
                        config.CorrelationHeader,
                        out List<string> id);

                    if (id.Count > 0) return id[0];
                }
                return string.Empty;
            }
        }

        public HttpHeaders(IConfig config)
        {
            this.config = config;
            this.data = new Dictionary<string, List<string>>();
        }

        public void Add(string name, string value)
        {
            if (!this.data.ContainsKey(name))
            {
                this.data[name] = new List<string>();
            }

            this.data[name].Add(value);
        }

        public void Add(string name, IEnumerable<string> values)
        {
            foreach (var value in values)
            {
                this.Add(name, value);
            }
        }

        public IEnumerator<KeyValuePair<string, List<string>>> GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<string, List<string>>>) this.data).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}