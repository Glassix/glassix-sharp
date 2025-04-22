using System;
using System.Collections.Generic;

namespace GlassixSharp.Contacts.Models
{
    /// <summary>
    /// Represents a contact in the Glassix system
    /// </summary>
    public class Contact
    {
        /// <summary>
        /// Unique identifier for the contact
        /// </summary>
        public Guid id { get; set; }
        
        /// <summary>
        /// Name of the contact
        /// </summary>
        public string name { get; set; }
        
        /// <summary>
        /// Identifiers associated with the contact (phone numbers, emails, etc.)
        /// </summary>
        public List<ContactIdentifier> identifiers { get; set; }
        
        /// <summary>
        /// Custom argument for the contact
        /// </summary>
        public string uniqueArgument { get; set; }
        
        /// <summary>
        /// File identifier for the contact's image
        /// </summary>
        public string imageFileIdentifier { get; set; }
        
        /// <summary>
        /// URI to access the contact's image
        /// </summary>
        public Uri imageUri { get; set; }
        
        /// <summary>
        /// When the contact was created
        /// </summary>
        public DateTime creationDateTime { get; set; }
        
        /// <summary>
        /// When the contact was last updated
        /// </summary>
        public DateTime updateDateTime { get; set; }
        
        /// <summary>
        /// Whether the contact has been deleted
        /// </summary>
        public bool isDeleted { get; set; }
        public bool isScrambled { get; set; }

        /// <summary>
        /// Each contact can have multiple tags.
        /// </summary>
        public List<string> tags { get; set; }
    }
    
    /// <summary>
    /// Represents an identifier for a contact
    /// </summary>
    public class ContactIdentifier
    {
        /// <summary>
        /// Unique identifier for the contact identifier
        /// </summary>
        public int id { get; set; }
        
        /// <summary>
        /// Contact ID this identifier belongs to
        /// </summary>
        public Guid contactId { get; set; }
        
        /// <summary>
        /// Type of identifier (PhoneNumber, MailAddress, etc.)
        /// </summary>
        public IdentifierType identifierType { get; set; }
        
        /// <summary>
        /// The actual identifier value
        /// </summary>
        public string identifier { get; set; }
        
        /// <summary>
        /// When the identifier was last accessed
        /// </summary>
        public DateTime lastAccessed { get; set; }
        
        /// <summary>
        /// Whether the identifier has been verified
        /// </summary>
        public bool isVerified { get; set; }
        
        /// <summary>
        /// Whether the identifier has been deleted
        /// </summary>
        public bool isDeleted { get; set; }
        public bool isScrambled { get; set; }

        public enum IdentifierType
        {
            Undefined = 0,
            PhoneNumber = 1,
            MailAddress = 2,
            FacebookId = 3,
            InstagramId = 4,
            Web = 5,
            InstagramIGSID = 6,
            ClientViberID = 7,
            ClientAppleBusinessChatID = 8,
            ClientTwitterId = 9,
            ClientGoogleBusinessMessagesId = 10,
            ClientGoogleBusinessReviewerId = 11,
            TikTokId = 12
        }
    }
}
