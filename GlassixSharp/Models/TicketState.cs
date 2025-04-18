using System.Runtime.Serialization;

namespace GlassixSharp.Models
{
    /// <summary>
    /// Represents the state of a ticket
    /// </summary>
    public enum TicketState
    {
        /// <summary>
        /// Ticket is open
        /// </summary>
        [EnumMember(Value = "Open")]
        Open,
        
        /// <summary>
        /// Ticket is closed
        /// </summary>
        [EnumMember(Value = "Closed")]
        Closed,
        
        /// <summary>
        /// Ticket is pending
        /// </summary>
        [EnumMember(Value = "Pending")]
        Pending,
        
        /// <summary>
        /// Ticket is snoozed
        /// </summary>
        [EnumMember(Value = "Snoozed")]
        Snoozed
    }
}
