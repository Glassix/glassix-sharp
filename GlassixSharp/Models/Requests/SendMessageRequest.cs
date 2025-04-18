using System.Collections.Generic;

namespace GlassixSharp.Models.Requests
{
    /// <summary>
    /// Request to send a message in a ticket
    /// </summary>
    public class SendMessageRequest
    {
        /// <summary>
        /// Plain text message content
        /// </summary>
        public string Text { get; set; }
        
        /// <summary>
        /// HTML message content
        /// </summary>
        public string Html { get; set; }
        
        /// <summary>
        /// Quick replies to include with the message
        /// </summary>
        public List<QuickReply> QuickReplies { get; set; }
        
        /// <summary>
        /// Templates to include with the message
        /// </summary>
        public List<Template> Templates { get; set; }
        
        /// <summary>
        /// Files to attach to the message
        /// </summary>
        public List<string> Files { get; set; }
        
        /// <summary>
        /// Whether to enable free text input for web chat customers
        /// </summary>
        public bool EnableFreeTextInput { get; set; } = false;
    }
    
    /// <summary>
    /// Quick reply button for a message
    /// </summary>
    public class QuickReply
    {
        /// <summary>
        /// Title of the quick reply button
        /// </summary>
        public string Title { get; set; }
        
        /// <summary>
        /// Image URL for the quick reply button
        /// </summary>
        public string ImageUrl { get; set; }
    }
    
    /// <summary>
    /// Template for a structured message
    /// </summary>
    public class Template
    {
        /// <summary>
        /// Title of the template
        /// </summary>
        public string Title { get; set; }
        
        /// <summary>
        /// Subtitle of the template
        /// </summary>
        public string Subtitle { get; set; }
        
        /// <summary>
        /// Cover image URL
        /// </summary>
        public string CoverImageUrl { get; set; }
        
        /// <summary>
        /// Buttons to include in the template
        /// </summary>
        public List<Button> Buttons { get; set; }
        
        /// <summary>
        /// HTML title (for web chat)
        /// </summary>
        public string TitleHtml { get; set; }
        
        /// <summary>
        /// HTML subtitle (for web chat)
        /// </summary>
        public string SubtitleHtml { get; set; }
    }
    
    /// <summary>
    /// Button for a template
    /// </summary>
    public class Button
    {
        /// <summary>
        /// Title of the button
        /// </summary>
        public string Title { get; set; }
        
        /// <summary>
        /// Type of button (text, web_url, postback, phone_number)
        /// </summary>
        public string Type { get; set; }
        
        /// <summary>
        /// URI for web_url buttons
        /// </summary>
        public string Uri { get; set; }
        
        /// <summary>
        /// Custom data for postback buttons
        /// </summary>
        public string Data { get; set; }
    }
}
