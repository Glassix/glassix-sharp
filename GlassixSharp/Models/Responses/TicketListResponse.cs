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
        public List<Ticket> tickets { get; set; }
        
        /// <summary>
        /// Paging information
        /// </summary>
        public PagingInfo paging { get; set; }
    }
    
    /// <summary>
    /// Paging information
    /// </summary>
    public class PagingInfo
    {
        /// <summary>
        /// URL for the next page of results
        /// </summary>
        public string next { get; set; }
    }
}
