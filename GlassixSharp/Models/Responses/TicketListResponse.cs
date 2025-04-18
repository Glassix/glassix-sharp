using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GlassixSharp.Models.Responses
{
    /// <summary>
    /// Response containing a list of tickets
    /// </summary>
    public class TicketListResponse
    {
        /// <summary>
        /// List of tickets
        /// </summary>
        [JsonPropertyName("")]
        public List<Ticket> Tickets { get; set; }
        
        /// <summary>
        /// Paging information
        /// </summary>
        public PagingInfo Paging { get; set; }
    }
    
    /// <summary>
    /// Paging information
    /// </summary>
    public class PagingInfo
    {
        /// <summary>
        /// URL for the next page of results
        /// </summary>
        public string Next { get; set; }
    }
}
