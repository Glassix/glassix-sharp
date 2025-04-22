using GlassixSharp.Protocols.Models;
using GlassixSharp.Tickets.Models;
using GlassixSharp.Users.Models;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GlassixSharp.Webhooks.Models
{
    /// <summary>
    /// Represents a webhook event
    /// </summary>
    public class WebhookEvent
    {
        /// <summary>
        /// Unique identifier for the event
        /// </summary>
        public string key { get; set; }
        
        /// <summary>
        /// When the event occurred
        /// </summary>
        public DateTime dateTime { get; set; }
        
        /// <summary>
        /// Changes included in the event
        /// </summary>
        public List<WebhookChange> changes { get; set; }
    }
    
    /// <summary>
    /// Represents a change in a webhook event
    /// </summary>
    public class WebhookChange
    {
        /// <summary>
        /// Type of event (NEW_TICKET, TICKET_STATE_CHANGE, etc.)
        /// </summary>
        public string _event { get; set; }
        
        /// <summary>
        /// Ticket ID associated with the event
        /// </summary>
        public int? ticketId { get; set; }
        
        /// <summary>
        /// New ticket state (for TICKET_STATE_CHANGE events)
        /// </summary>
        public string ticketState { get; set; }
        
        /// <summary>
        /// Full ticket object (for NEW_TICKET and some other events)
        /// </summary>
        public Ticket ticket { get; set; }
        
        /// <summary>
        /// Owner ID (for TICKET_OWNER_CHANGE events)
        /// </summary>
        public Guid? ownerId { get; set; }
        
        /// <summary>
        /// Owner username (for TICKET_OWNER_CHANGE events)
        /// </summary>
        public string ownerUserName { get; set; }
        
        /// <summary>
        /// User ID (for USER_STATUS_CHANGE events)
        /// </summary>
        public Guid? userId { get; set; }
        
        /// <summary>
        /// Username (for USER_STATUS_CHANGE events)
        /// </summary>
        public string userName { get; set; }
        
        /// <summary>
        /// User status (for USER_STATUS_CHANGE events)
        /// </summary>
        public User.UserStatus? userStatus { get; set; }
        
        /// <summary>
        /// Whether Do Not Disturb is enabled (for USER_STATUS_CHANGE events)
        /// </summary>
        public bool? doNotDisturb { get; set; }
        
        /// <summary>
        /// Transaction object (for NEW_MESSAGE events)
        /// </summary>
        public Transaction transaction { get; set; }
        
        /// <summary>
        /// Document object (for DOCUMENT_SIGNED events)
        /// </summary>
        public object document { get; set; }
        
        /// <summary>
        /// Message object (for NON_TICKET_MESSAGE_STATUS events)
        /// </summary>
        public Message message { get; set; }
        
        /// <summary>
        /// Participant object (for SURVEY_UPDATE events)
        /// </summary>
        public Participant participant { get; set; }
        
        /// <summary>
        /// Survey responses (for SURVEY_UPDATE events)
        /// </summary>
        public List<Survey> surveys { get; set; }
    }
}
