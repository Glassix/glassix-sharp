using System.Collections.Generic;

namespace GlassixSharp.Tickets.Models
{
    /// <summary>
    /// Request to set a ticket's state
    /// </summary>
    public class SetTicketStateRequest
    {
        /// <summary>
        /// Tags to add to the ticket
        /// </summary>
        public List<string> tags { get; set; }
        
        /// <summary>
        /// Summary to add to the ticket
        /// </summary>
        public string summary { get; set; }
    }
}
