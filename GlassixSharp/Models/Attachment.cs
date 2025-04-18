using System.Collections.Generic;

namespace GlassixSharp.Models
{
    /// <summary>
    /// Represents an attachment in a transaction
    /// </summary>
    public class Attachment
    {
        /// <summary>
        /// Unique identifier for the attachment
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Type of attachment (Image, Video, Document, etc.)
        /// </summary>
        public string Type { get; set; }
        
        /// <summary>
        /// File identifier
        /// </summary>
        public string FileIdentifier { get; set; }
        
        /// <summary>
        /// Original file name
        /// </summary>
        public string OriginalFileName { get; set; }
        
        /// <summary>
        /// Thumbnails for the attachment
        /// </summary>
        public List<string> Thumbnails { get; set; }
        
        /// <summary>
        /// URI to access the attachment
        /// </summary>
        public string Uri { get; set; }
        
        /// <summary>
        /// Whether the attachment is embedded in the message
        /// </summary>
        public bool IsEmbedded { get; set; }
    }
}
