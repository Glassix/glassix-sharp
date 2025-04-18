using System;
using System.Collections.Generic;

namespace GlassixSharp.Models.Responses
{
    /// <summary>
    /// Response from sending a message through a protocol
    /// </summary>
    public class SendProtocolMessageResponse
    {
        /// <summary>
        /// Text content of the message
        /// </summary>
        public string Text { get; set; }
        
        /// <summary>
        /// HTML content of the message
        /// </summary>
        public string Html { get; set; }
        
        /// <summary>
        /// Protocol type used
        /// </summary>
        public string ProtocolType { get; set; }
        
        /// <summary>
        /// URIs of attachments
        /// </summary>
        public List<string> AttachmentUris { get; set; }
        
        /// <summary>
        /// Sender's identifier
        /// </summary>
        public string From { get; set; }
        
        /// <summary>
        /// Recipient's identifier
        /// </summary>
        public string To { get; set; }
        
        /// <summary>
        /// When the message was sent
        /// </summary>
        public DateTime DateTime { get; set; }
        
        /// <summary>
        /// Provider's message ID
        /// </summary>
        public string ProviderMessageId { get; set; }
        
        /// <summary>
        /// Status of the message
        /// </summary>
        public string Status { get; set; }
        
        /// <summary>
        /// Service provider ID
        /// </summary>
        public int ServiceProvider { get; set; }
    }
}
