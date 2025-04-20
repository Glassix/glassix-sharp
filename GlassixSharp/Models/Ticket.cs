using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GlassixSharp.Models
{
    /// <summary>
    /// Represents a ticket in the Glassix system
    /// </summary>
    public class Ticket
    {
        /// <summary>
        /// Unique identifier for the ticket
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// The department this ticket belongs to
        /// </summary>
        public string DepartmentId { get; set; }
        
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
        /// Type of the ticket
        /// </summary>
        public string TicketType { get; set; }
        
        /// <summary>
        /// Tags associated with the ticket
        /// </summary>
        public List<string> Tags { get; set; }
        
        /// <summary>
        /// The owner of the ticket
        /// </summary>
        public User Owner { get; set; }
        
        /// <summary>
        /// Current state of the ticket
        /// </summary>
        public string State { get; set; }
        
        /// <summary>
        /// When the ticket was opened
        /// </summary>
        public DateTime Open { get; set; }
        
        /// <summary>
        /// When the ticket was closed
        /// </summary>
        public DateTime? Close { get; set; }
        
        /// <summary>
        /// Last activity timestamp
        /// </summary>
        public DateTime? LastActivity { get; set; }
        
        /// <summary>
        /// Custom parameter to assist with CRM sync
        /// </summary>
        public string UniqueArgument { get; set; }
        
        /// <summary>
        /// Primary protocol used in the ticket
        /// </summary>
        public ProtocolType PrimaryProtocolType { get; set; }
        
        /// <summary>
        /// Additional details about the ticket
        /// </summary>
        public TicketDetails Details { get; set; }
        
        /// <summary>
        /// Participants in the ticket
        /// </summary>
        public List<Participant> Participants { get; set; }
        
        /// <summary>
        /// Transactions (messages, status changes, etc.) in the ticket
        /// </summary>
        public List<Transaction> Transactions { get; set; }
        
        /// <summary>
        /// ID of the ticket this was moved to
        /// </summary>
        public int MovedToTicketId { get; set; }
        
        /// <summary>
        /// ID of the department this ticket was moved to
        /// </summary>
        public string MovedToDepartmentId { get; set; }
        
        /// <summary>
        /// When the first customer message was sent
        /// </summary>
        public DateTime? FirstCustomerMessageDateTime { get; set; }
        
        /// <summary>
        /// When the first agent message was sent
        /// </summary>
        public DateTime? FirstAgentMessageDateTime { get; set; }
        
        /// <summary>
        /// When the last customer message was sent
        /// </summary>
        public DateTime? LastCustomerMessageDateTime { get; set; }
        
        /// <summary>
        /// Time the ticket spent in queue (gross)
        /// </summary>
        public string QueueTimeGross { get; set; }
        
        /// <summary>
        /// Time the ticket spent in queue (net)
        /// </summary>
        public string QueueTimeNet { get; set; }
        
        /// <summary>
        /// First agent response time (gross)
        /// </summary>
        public string FirstAgentResponseTimeGross { get; set; }
        
        /// <summary>
        /// First agent response time (net)
        /// </summary>
        public string FirstAgentResponseTimeNet { get; set; }
        
        /// <summary>
        /// Number of media items sent by agents
        /// </summary>
        public int AgentMediaCount { get; set; }
        
        /// <summary>
        /// Number of media items sent by customers
        /// </summary>
        public int CustomerMediaCount { get; set; }
        
        /// <summary>
        /// Number of messages sent by agents
        /// </summary>
        public int AgentMessagesCount { get; set; }
        
        /// <summary>
        /// Number of messages sent by customers
        /// </summary>
        public int CustomerMessagesCount { get; set; }
        
        /// <summary>
        /// Total duration of the ticket (net)
        /// </summary>
        public string DurationNet { get; set; }
        
        /// <summary>
        /// Total duration of the ticket (gross)
        /// </summary>
        public string DurationGross { get; set; }
        
        /// <summary>
        /// Average agent response time (net)
        /// </summary>
        public string AgentResponseAverageTimeNet { get; set; }
        
        /// <summary>
        /// Number of canned replies used by agents
        /// </summary>
        public int AgentCannedRepliesCount { get; set; }
        
        /// <summary>
        /// ID of the bot conversation
        /// </summary>
        public string BotConversationId { get; set; }
    }
}