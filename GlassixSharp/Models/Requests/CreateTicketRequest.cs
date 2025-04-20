using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GlassixSharp.Models.Requests
{
    /// <summary>
    /// Request to create a new ticket
    /// </summary>
    public class CreateTicketRequest
    {
        /// <summary>
        /// Free text field for ticket subject
        /// </summary>
        public string Field1 { get; set; }
        
        /// <summary>
        /// Dynamic field
        /// </summary>
        public string Field2 { get; set; }
        
        /// <summary>
        /// Dynamic field
        /// </summary>
        public string Field3 { get; set; }
        
        /// <summary>
        /// Dynamic field
        /// </summary>
        public string Field4 { get; set; }
        
        /// <summary>
        /// Dynamic field
        /// </summary>
        public string Field5 { get; set; }
        
        /// <summary>
        /// Dynamic field
        /// </summary>
        public string Field6 { get; set; }
        
        /// <summary>
        /// Dynamic field
        /// </summary>
        public string Field7 { get; set; }
        
        /// <summary>
        /// Dynamic field
        /// </summary>
        public string Field8 { get; set; }
        
        /// <summary>
        /// Dynamic field
        /// </summary>
        public string Field9 { get; set; }
        
        /// <summary>
        /// Dynamic field
        /// </summary>
        public string Field10 { get; set; }
        
        /// <summary>
        /// Culture code (e.g., "en-US")
        /// </summary>
        public string Culture { get; set; }
        
        /// <summary>
        /// Participants in the ticket
        /// </summary>
        public List<CreateTicketParticipant> Participants { get; set; }
        
        /// <summary>
        /// Tags for the ticket
        /// </summary>
        public List<string> Tags { get; set; }
        
        /// <summary>
        /// Custom argument for CRM sync
        /// </summary>
        public string UniqueArgument { get; set; }
        
        /// <summary>
        /// Ticket state
        /// </summary>
        public string State { get; set; } = "Open";
        
        /// <summary>
        /// When the ticket was opened
        /// </summary>
        public DateTime? Open { get; set; }
        
        /// <summary>
        /// When the ticket was closed
        /// </summary>
        public DateTime? Close { get; set; }
        
        /// <summary>
        /// Whether to assign the ticket to an available user
        /// </summary>
        public bool GetAvailableUser { get; set; } = true;
        
        /// <summary>
        /// Whether to add an introduction message
        /// </summary>
        public bool AddIntroductionMessage { get; set; } = true;
        
        /// <summary>
        /// Whether to enable webhook for this ticket
        /// </summary>
        public bool EnableWebhook { get; set; } = true;
        
        /// <summary>
        /// Whether to mark the ticket as read
        /// </summary>
        public bool MarkAsRead { get; set; } = false;
        
        /// <summary>
        /// Additional details about the ticket
        /// </summary>
        public TicketDetails Details { get; set; }
    }
    
    /// <summary>
    /// Participant to add to a new ticket
    /// </summary>
    public class CreateTicketParticipant
    {
        /// <summary>
        /// Name of the participant
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Protocol type (e.g., WhatsApp, SMS, Web)
        /// </summary>
        public ProtocolType ProtocolType { get; set; }
        
        /// <summary>
        /// Sub-protocol type
        /// </summary>
        public string SubProtocolType { get; set; } = "Undefined";
        
        /// <summary>
        /// Whether the participant is active
        /// </summary>
        public bool IsActive { get; set; } = true;
        
        /// <summary>
        /// Whether the participant is deleted
        /// </summary>
        public bool IsDeleted { get; set; } = false;
        
        /// <summary>
        /// Identifier for the participant (e.g., phone number, email)
        /// </summary>
        public string Identifier { get; set; }
        
        /// <summary>
        /// Type of participant (Client or User)
        /// </summary>
        public string Type { get; set; } = "Client";
    }
}