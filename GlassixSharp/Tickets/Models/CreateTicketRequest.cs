using GlassixSharp.Models;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GlassixSharp.Tickets.Models
{
    /// <summary>
    /// Request to create a new ticket
    /// </summary>
    public class CreateTicketRequest
    {
        /// <summary>
        /// Free text field for ticket subject
        /// </summary>
        public string field1 { get; set; }
        
        /// <summary>
        /// Dynamic field
        /// </summary>
        public string field2 { get; set; }
        
        /// <summary>
        /// Dynamic field
        /// </summary>
        public string field3 { get; set; }
        
        /// <summary>
        /// Dynamic field
        /// </summary>
        public string field4 { get; set; }
        
        /// <summary>
        /// Dynamic field
        /// </summary>
        public string field5 { get; set; }
        
        /// <summary>
        /// Dynamic field
        /// </summary>
        public string field6 { get; set; }
        
        /// <summary>
        /// Dynamic field
        /// </summary>
        public string field7 { get; set; }
        
        /// <summary>
        /// Dynamic field
        /// </summary>
        public string field8 { get; set; }
        
        /// <summary>
        /// Dynamic field
        /// </summary>
        public string field9 { get; set; }
        
        /// <summary>
        /// Dynamic field
        /// </summary>
        public string field10 { get; set; }

        /// <summary>
        /// Culture code (e.g., "en-US")
        /// </summary>
        public string culture { get; set; } = "en-US";
        
        /// <summary>
        /// Participants in the ticket
        /// </summary>
        public List<Participant> participants { get; set; }
        
        /// <summary>
        /// Tags for the ticket
        /// </summary>
        public List<string> tags { get; set; }
        
        /// <summary>
        /// Custom argument for CRM sync
        /// </summary>
        public string uniqueArgument { get; set; }

        /// <summary>
        /// Ticket state
        /// </summary>
        public Ticket.State state { get; set; } = Ticket.State.Open;
        
        /// <summary>
        /// When the ticket was opened
        /// </summary>
        public DateTime? open { get; set; }
        
        /// <summary>
        /// When the ticket was closed
        /// </summary>
        public DateTime? close { get; set; }
        
        /// <summary>
        /// Whether to assign the ticket to an available user
        /// </summary>
        public bool getAvailableUser { get; set; } = true;
        
        /// <summary>
        /// Whether to add an introduction message
        /// </summary>
        public bool addIntroductionMessage { get; set; } = false;
        
        /// <summary>
        /// Whether to enable webhook for this ticket
        /// </summary>
        public bool enableWebhook { get; set; } = true;
        
        /// <summary>
        /// Whether to mark the ticket as read
        /// </summary>
        public bool markAsRead { get; set; } = false;
        
        /// <summary>
        /// Additional details about the ticket
        /// </summary>
        public Ticket.Details details { get; set; }
    }
}