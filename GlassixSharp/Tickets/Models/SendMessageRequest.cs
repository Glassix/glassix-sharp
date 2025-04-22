using System.Collections.Generic;

namespace GlassixSharp.Tickets.Models
{
    /// <summary>
    /// Request to send a message in a ticket
    /// </summary>
    public class SendMessageRequest
    {
        /// <summary>
        /// Plain text message content
        /// </summary>
        public string text { get; set; }
        
        /// <summary>
        /// HTML message content
        /// </summary>
        public string html { get; set; }
        
        /// <summary>
        /// Quick replies to include with the message
        /// </summary>
        public List<QuickReply> quickReplies { get; set; }
        
        /// <summary>
        /// Templates to include with the message
        /// </summary>
        public List<Template> templates { get; set; }
        
        /// <summary>
        /// Files to attach to the message
        /// </summary>
        public List<string> files { get; set; }
        
        /// <summary>
        /// Whether to enable free text input for web chat customers
        /// </summary>
        public bool enableFreeTextInput { get; set; } = false;
    }
    
    /// <summary>
    /// Quick reply button for a message
    /// </summary>
    public class QuickReply
    {
        /// <summary>
        /// Title of the quick reply button
        /// </summary>
        public string title { get; set; }
        
        /// <summary>
        /// Image URL for the quick reply button
        /// </summary>
        public string imageUrl { get; set; }
    }
    
    /// <summary>
    /// Template for a structured message
    /// </summary>
    public class Template
    {
        /// <summary>
        /// Title of the template
        /// </summary>
        public string title { get; set; }
        
        /// <summary>
        /// Subtitle of the template
        /// </summary>
        public string subtitle { get; set; }
        
        /// <summary>
        /// Cover image URL
        /// </summary>
        public string coverImageUrl { get; set; }
        
        /// <summary>
        /// Buttons to include in the template
        /// </summary>
        public List<Button> buttons { get; set; }
        
        /// <summary>
        /// HTML title (for web chat)
        /// </summary>
        public string titleHtml { get; set; }
        
        /// <summary>
        /// HTML subtitle (for web chat)
        /// </summary>
        public string subtitleHtml { get; set; }
    }
    
    /// <summary>
    /// Button for a template
    /// </summary>
    public class Button
    {
        /// <summary>
        /// Title of the button
        /// </summary>
        public string title { get; set; }
        
        /// <summary>
        /// Type of button (text, web_url, postback, phone_number)
        /// </summary>
        public string type { get; set; }
        
        /// <summary>
        /// URI for web_url buttons
        /// </summary>
        public string uri { get; set; }
        
        /// <summary>
        /// Custom data for postback buttons
        /// </summary>
        public string data { get; set; }
    }
}
