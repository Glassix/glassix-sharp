using System.Collections.Generic;

namespace GlassixSharp.Models.Requests
{
    /// <summary>
    /// Request to send a message through a protocol (WhatsApp, SMS, etc.)
    /// </summary>
    public class SendProtocolMessageRequest
    {
        /// <summary>
        /// Text content of the message
        /// </summary>
        public string Text { get; set; }
        
        /// <summary>
        /// Protocol type (SMS, WhatsApp, AppleBusinessChat)
        /// </summary>
        public string ProtocolType { get; set; }
        
        /// <summary>
        /// Sender's phone number or Apple ID
        /// </summary>
        public string From { get; set; }
        
        /// <summary>
        /// Recipient's phone number or Apple identifier
        /// </summary>
        public string To { get; set; }
        
        /// <summary>
        /// Files to attach to the message
        /// </summary>
        public List<string> Files { get; set; }
    }
}
