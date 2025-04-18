using System;
using System.Collections.Generic;

namespace GlassixSharp.Models
{
    /// <summary>
    /// Represents a contact in the Glassix system
    /// </summary>
    public class Contact
    {
        /// <summary>
        /// Unique identifier for the contact
        /// </summary>
        public string Id { get; set; }
        
        /// <summary>
        /// Name of the contact
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Identifiers associated with the contact (phone numbers, emails, etc.)
        /// </summary>
        public List<ContactIdentifier> Identifiers { get; set; }
        
        /// <summary>
        /// Custom argument for the contact
        /// </summary>
        public string UniqueArgument { get; set; }
        
        /// <summary>
        /// File identifier for the contact's image
        /// </summary>
        public string ImageFileIdentifier { get; set; }
        
        /// <summary>
        /// URI to access the contact's image
        /// </summary>
        public string ImageUri { get; set; }
        
        /// <summary>
        /// When the contact was created
        /// </summary>
        public DateTime CreationDateTime { get; set; }
        
        /// <summary>
        /// When the contact was last updated
        /// </summary>
        public DateTime UpdateDateTime { get; set; }
        
        /// <summary>
        /// Whether the contact has been deleted
        /// </summary>
        public bool IsDeleted { get; set; }
    }
    
    /// <summary>
    /// Represents an identifier for a contact
    /// </summary>
    public class ContactIdentifier
    {
        /// <summary>
        /// Unique identifier for the contact identifier
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Contact ID this identifier belongs to
        /// </summary>
        public string ContactId { get; set; }
        
        /// <summary>
        /// Type of identifier (PhoneNumber, MailAddress, etc.)
        /// </summary>
        public string IdentifierType { get; set; }
        
        /// <summary>
        /// The actual identifier value
        /// </summary>
        public string Identifier { get; set; }
        
        /// <summary>
        /// When the identifier was last accessed
        /// </summary>
        public DateTime LastAccessed { get; set; }
        
        /// <summary>
        /// Whether the identifier has been verified
        /// </summary>
        public bool IsVerified { get; set; }
        
        /// <summary>
        /// Whether the identifier has been deleted
        /// </summary>
        public bool IsDeleted { get; set; }
    }
}
