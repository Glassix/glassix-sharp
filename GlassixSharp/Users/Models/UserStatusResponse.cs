namespace GlassixSharp.Users.Models
{
    /// <summary>
    /// Response containing a user's status
    /// </summary>
    public class UserStatusResponse
    {
        /// <summary>
        /// Status string (Offline, Break, Online)
        /// </summary>
        public User.UserStatus Status { get; set; }
        
        /// <summary>
        /// Status code (1: Offline, 2: Break, 10: Online)
        /// </summary>
        public int _Status { get; set; }
    }
}
