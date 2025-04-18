using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GlassixSharp.Models
{
    /// <summary>
    /// Represents a webhook event
    /// </summary>
    public class WebhookEvent
    {
        /// <summary>
        /// Unique identifier for the event
        /// </summary>
        public string Key { get; set; }
        
        /// <summary>
        /// When the event occurred
        /// </summary>
        public DateTime DateTime { get; set; }
        
        /// <summary>
        /// Changes included in the event
        /// </summary>
        public List<WebhookChange> Changes { get; set; }
    }
    
    /// <summary>
    /// Represents a change in a webhook event
    /// </summary>
    public class WebhookChange
    {
        /// <summary>
        /// Type of event (NEW_TICKET, TICKET_STATE_CHANGE, etc.)
        /// </summary>
        [JsonPropertyName("_event")]
        public string Event { get; set; }
        
        /// <summary>
        /// Ticket ID associated with the event
        /// </summary>
        public int? TicketId { get; set; }
        
        /// <summary>
        /// New ticket state (for TICKET_STATE_CHANGE events)
        /// </summary>
        public string TicketState { get; set; }
        
        /// <summary>
        /// Full ticket object (for NEW_TICKET and some other events)
        /// </summary>
        public Ticket Ticket { get; set; }
        
        /// <summary>
        /// Owner ID (for TICKET_OWNER_CHANGE events)
        /// </summary>
        public string OwnerId { get; set; }
        
        /// <summary>
        /// Owner username (for TICKET_OWNER_CHANGE events)
        /// </summary>
        public string OwnerUserName { get; set; }
        
        /// <summary>
        /// User ID (for USER_STATUS_CHANGE events)
        /// </summary>
        public string UserId { get; set; }
        
        /// <summary>
        /// Username (for USER_STATUS_CHANGE events)
        /// </summary>
        public string UserName { get; set; }
        
        /// <summary>
        /// User status (for USER_STATUS_CHANGE events)
        /// </summary>
        public string UserStatus { get; set; }
        
        /// <summary>
        /// Whether Do Not Disturb is enabled (for USER_STATUS_CHANGE events)
        /// </summary>
        public bool? DoNotDisturb { get; set; }
        
        /// <summary>
        /// Transaction object (for NEW_MESSAGE events)
        /// </summary>
        public Transaction Transaction { get; set; }
        
        /// <summary>
        /// Document object (for DOCUMENT_SIGNED events)
        /// </summary>
        public object Document { get; set; }
        
        /// <summary>
        /// Message object (for NON_TICKET_MESSAGE_STATUS events)
        /// </summary>
        public object Message { get; set; }
        
        /// <summary>
        /// Participant object (for SURVEY_UPDATE events)
        /// </summary>
        public Participant Participant { get; set; }
        
        /// <summary>
        /// Survey responses (for SURVEY_UPDATE events)
        /// </summary>
        public List<object> Surveys { get; set; }
    }
}
