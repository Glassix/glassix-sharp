using System.Collections.Generic;

namespace GlassixSharp.Models
{
    /// <summary>
    /// Represents a user (agent) in the Glassix system
    /// </summary>
    public class User
    {
        /// <summary>
        /// Unique identifier for the user
        /// </summary>
        public string Id { get; set; }
        
        /// <summary>
        /// Gender of the user
        /// </summary>
        public string Gender { get; set; }
        
        /// <summary>
        /// Email address of the user
        /// </summary>
        public string UserName { get; set; }
        
        /// <summary>
        /// Culture code for the user (e.g., "en-US")
        /// </summary>
        public string Culture { get; set; }
        
        /// <summary>
        /// Whether the user is anonymous
        /// </summary>
        public bool IsAnonymous { get; set; }
        
        /// <summary>
        /// Custom argument for the user
        /// </summary>
        public string UniqueArgument { get; set; }
        
        /// <summary>
        /// Type of user (AGENT, BOT, API)
        /// </summary>
        public string Type { get; set; }
        
        /// <summary>
        /// Full name of the user
        /// </summary>
        public string FullName { get; set; }
        
        /// <summary>
        /// Short name of the user
        /// </summary>
        public string ShortName { get; set; }
        
        /// <summary>
        /// Job title of the user
        /// </summary>
        public string JobTitle { get; set; }
        
        /// <summary>
        /// Current status of the user
        /// </summary>
        public string Status { get; set; }
        
        /// <summary>
        /// Roles assigned to the user
        /// </summary>
        public List<string> Roles { get; set; }
    }
}
