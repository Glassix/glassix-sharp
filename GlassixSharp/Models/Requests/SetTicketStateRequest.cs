using System.Collections.Generic;

namespace GlassixSharp.Models.Requests
{
    /// <summary>
    /// Request to set a ticket's state
    /// </summary>
    public class SetTicketStateRequest
    {
        /// <summary>
        /// Tags to add to the ticket
        /// </summary>
        public List<string> Tags { get; set; }
        
        /// <summary>
        /// Summary to add to the ticket
        /// </summary>
        public string Summary { get; set; }
    }
}
