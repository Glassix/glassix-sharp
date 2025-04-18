using System.Runtime.Serialization;

namespace GlassixSharp.Models
{
    /// <summary>
    /// Represents a user's status
    /// </summary>
    public enum UserStatus
    {
        /// <summary>
        /// User is offline
        /// </summary>
        [EnumMember(Value = "Offline")]
        Offline,
        
        /// <summary>
        /// User is on a break
        /// </summary>
        [EnumMember(Value = "Break")]
        Break,
        
        /// <summary>
        /// User is on break type 2
        /// </summary>
        [EnumMember(Value = "Break2")]
        Break2,
        
        /// <summary>
        /// User is on break type 3
        /// </summary>
        [EnumMember(Value = "Break3")]
        Break3,
        
        /// <summary>
        /// User is on break type 4
        /// </summary>
        [EnumMember(Value = "Break4")]
        Break4,
        
        /// <summary>
        /// User is on break type 5
        /// </summary>
        [EnumMember(Value = "Break5")]
        Break5,
        
        /// <summary>
        /// User is online
        /// </summary>
        [EnumMember(Value = "Online")]
        Online
    }
}
