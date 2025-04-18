using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GlassixSharp.Utilities
{
    /// <summary>
    /// Helper class for building URL query strings
    /// </summary>
    internal static class QueryStringBuilder
    {
        /// <summary>
        /// Builds a query string from a dictionary of parameters
        /// </summary>
        /// <param name="parameters">Dictionary of parameter name/value pairs</param>
        /// <returns>URL-encoded query string</returns>
        public static string Build(Dictionary<string, string> parameters)
        {
            if (parameters == null || parameters.Count == 0)
                return string.Empty;

            var queryParams = parameters
                .Where(p => !string.IsNullOrEmpty(p.Value))
                .Select(p => $"{Uri.EscapeDataString(p.Key)}={Uri.EscapeDataString(p.Value)}");

            return string.Join("&", queryParams);
        }
    }
}
