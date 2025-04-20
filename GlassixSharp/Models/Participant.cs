using System;

namespace GlassixSharp.Models
{
    /// <summary>
    /// Represents a participant in a ticket
    /// </summary>
    public class Participant
    {
        /// <summary>
        /// Participant ID in the ticket
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Type of participant (Client or User)
        /// </summary>
        public string Type { get; set; }
        
        /// <summary>
        /// Name of the participant
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Display name of the participant
        /// </summary>
        public string DisplayName { get; set; }
        
        /// <summary>
        /// Protocol type (e.g., WhatsApp, SMS, Web)
        /// </summary>
        public ProtocolType ProtocolType { get; set; }
        
        /// <summary>
        /// Sub-protocol type
        /// </summary>
        public string SubProtocolType { get; set; }
        
        /// <summary>
        /// Whether the participant is active
        /// </summary>
        public bool IsActive { get; set; }
        
        /// <summary>
        /// Whether the participant has been deleted
        /// </summary>
        public bool IsDeleted { get; set; }
        
        /// <summary>
        /// Identifier for the participant (e.g., phone number, email)
        /// </summary>
        public string Identifier { get; set; }
        
        /// <summary>
        /// Department identifier
        /// </summary>
        public string DepartmentIdentifier { get; set; }
        
        /// <summary>
        /// Contact ID for this participant
        /// </summary>
        public string ContactId { get; set; }
        
        /// <summary>
        /// Username (for agent participants)
        /// </summary>
        public string UserName { get; set; }
    }
}
