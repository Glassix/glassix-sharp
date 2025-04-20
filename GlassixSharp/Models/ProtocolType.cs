using System.Runtime.Serialization;

namespace GlassixSharp.Models
{
    /// <summary>
    /// Represents a communication protocol type
    /// </summary>
    public enum ProtocolType
    {
        /// <summary>
        /// SMS messaging
        /// </summary>
        [EnumMember(Value = "SMS")]
        SMS,
        
        /// <summary>
        /// WhatsApp messaging
        /// </summary>
        [EnumMember(Value = "WhatsApp")]
        WhatsApp,
        
        /// <summary>
        /// Email
        /// </summary>
        [EnumMember(Value = "Mail")]
        Mail,
        
        /// <summary>
        /// Web chat
        /// </summary>
        [EnumMember(Value = "Web")]
        Web,
        
        /// <summary>
        /// Facebook Messenger
        /// </summary>
        [EnumMember(Value = "FBmessenger")]
        FacebookMessenger,
        
        /// <summary>
        /// Web chat via SMS
        /// </summary>
        [EnumMember(Value = "WebViaSMS")]
        WebViaSMS,
        
        /// <summary>
        /// Telegram bot
        /// </summary>
        [EnumMember(Value = "TelegramBot")]
        TelegramBot,
        
        /// <summary>
        /// Facebook feed
        /// </summary>
        [EnumMember(Value = "FacebookFeed")]
        FacebookFeed,
        
        /// <summary>
        /// Instagram feed
        /// </summary>
        [EnumMember(Value = "InstagramFeed")]
        InstagramFeed,
        
        /// <summary>
        /// Phone call
        /// </summary>
        [EnumMember(Value = "PhoneCall")]
        PhoneCall,
        
        /// <summary>
        /// Viber messaging
        /// </summary>
        [EnumMember(Value = "Viber")]
        Viber,
        
        /// <summary>
        /// Apple Business Chat
        /// </summary>
        [EnumMember(Value = "AppleBusinessChat")]
        AppleBusinessChat,
        
        /// <summary>
        /// Google Business Messages
        /// </summary>
        [EnumMember(Value = "GoogleBusinessMessages")]
        GoogleBusinessMessages,
        
        /// <summary>
        /// Google Business Reviews
        /// </summary>
        [EnumMember(Value = "GoogleBusinessReviews")]
        GoogleBusinessReviews,
        
        /// <summary>
        /// Undefined protocol
        /// </summary>
        [EnumMember(Value = "Undefined")]
        Undefined
    }

    public enum SubProtocolType
    {
        Undefined = 0,
        MailTo = 1,
        MailCc = 2,
        MailBcc = 3
    }
}
