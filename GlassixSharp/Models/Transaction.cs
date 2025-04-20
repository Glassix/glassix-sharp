using System;
using System.Collections.Generic;

namespace GlassixSharp.Models
{
    /// <summary>
    /// Represents a transaction (message, state change, etc.) in a ticket
    /// </summary>
    public class Transaction
    {
        /// <summary>
        /// Unique identifier for the transaction
        /// </summary>
        public string GuidTransactionId { get; set; }
        
        /// <summary>
        /// Numeric identifier for the transaction
        /// </summary>
        public long Id { get; set; }
        
        /// <summary>
        /// Date and time of the transaction
        /// </summary>
        public DateTime DateTime { get; set; }
        
        /// <summary>
        /// Protocol type the transaction was sent from
        /// </summary>
        public ProtocolType FromProtocolType { get; set; }
        
        /// <summary>
        /// Type of transaction (Message, TicketOpened, etc.)
        /// </summary>
        public string Type { get; set; }
        
        /// <summary>
        /// Plain text content of the transaction
        /// </summary>
        public string Text { get; set; }
        
        /// <summary>
        /// HTML content of the transaction
        /// </summary>
        public string Html { get; set; }
        
        /// <summary>
        /// Status of the transaction (Pending, Delivered, Read, etc.)
        /// </summary>
        public string Status { get; set; }
        
        /// <summary>
        /// Participant who initiated the transaction
        /// </summary>
        public Participant FromParticipant { get; set; }
        
        /// <summary>
        /// Attachments included with the transaction
        /// </summary>
        public List<Attachment> Attachments { get; set; }
    }
}
