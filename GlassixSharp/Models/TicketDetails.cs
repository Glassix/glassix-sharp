using System.Collections.Generic;

namespace GlassixSharp.Models
{
    /// <summary>
    /// Additional details about a ticket
    /// </summary>
    public class TicketDetails
    {
        /// <summary>
        /// User agent of the customer
        /// </summary>
        public string UserAgent { get; set; }
        
        /// <summary>
        /// IP address of the customer
        /// </summary>
        public string IPAddress { get; set; }
        
        /// <summary>
        /// Geographic location of the customer
        /// </summary>
        public string Location { get; set; }
        
        /// <summary>
        /// Source information for the ticket
        /// </summary>
        public TicketSource Source { get; set; }
        
        /// <summary>
        /// External link (e.g., to a CRM system)
        /// </summary>
        public string ExternalLink { get; set; }
        
        /// <summary>
        /// Custom claims from an identity token
        /// </summary>
        public Dictionary<string, string> IdentityTokenClaims { get; set; }
        
        /// <summary>
        /// Referral information
        /// </summary>
        public ReferralInfo Referral { get; set; }
        
        /// <summary>
        /// Whether the ticket was opened from a mobile device
        /// </summary>
        public bool IsMobile { get; set; }
    }
    
    /// <summary>
    /// Source information for a ticket
    /// </summary>
    public class TicketSource
    {
        /// <summary>
        /// Title of the source
        /// </summary>
        public string Title { get; set; }
        
        /// <summary>
        /// URI of the source
        /// </summary>
        public string Uri { get; set; }
    }
    
    /// <summary>
    /// Referral information for a ticket
    /// </summary>
    public class ReferralInfo
    {
        /// <summary>
        /// UTM parameters
        /// </summary>
        public Dictionary<string, string> UtmParameters { get; set; }
        
        /// <summary>
        /// Ad ID
        /// </summary>
        public string AdId { get; set; }
        
        /// <summary>
        /// Ad title
        /// </summary>
        public string AdTitle { get; set; }
        
        /// <summary>
        /// Product ID
        /// </summary>
        public string ProductId { get; set; }
        
        /// <summary>
        /// Reference data
        /// </summary>
        public string RefData { get; set; }
        
        /// <summary>
        /// Facebook post ID
        /// </summary>
        public string FbPostId { get; set; }
        
        /// <summary>
        /// Instagram story ID
        /// </summary>
        public string InstagramStoryId { get; set; }
        
        /// <summary>
        /// Apple intent ID
        /// </summary>
        public string AppleIntentId { get; set; }
        
        /// <summary>
        /// Apple group ID
        /// </summary>
        public string AppleGroupId { get; set; }
    }
}
