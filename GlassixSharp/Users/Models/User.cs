using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace GlassixSharp.Users.Models
{
    /// <summary>
    /// A user represents an agent and can be assigned to many departments.
    /// </summary>
    public class User
    {
        /// <summary>
        /// The Id of the user.
        /// </summary>
        public Guid id { get; set; }

        /// <summary>
        /// The gender of the user.
        /// </summary>
        public Gender gender { get; set; }

        /// <summary>
        /// User's email address.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Restricted code list to note ticket/participant's language/culture. Can customize user's GUI.
        /// - en-US; English
        /// - he-IL; Hebrew
        /// - pt-PT; Portuguese
        /// - es-ES; Spanish
        /// </summary>
        public string culture { get; set; }

        /// <summary>
        /// Switch to determine whether owner appears as participant in ticket.
        /// </summary>
        public bool isAnonymous { get; set; }

        /// <summary>
        /// Custom parameter to assist sys admins to keep the CRM's tickets in sync, e.g., CRM case id. 
        /// Returned on every event or endpoint.
        /// </summary>
        public string uniqueArgument { get; set; }

        /// <summary>
        /// The type of the user. AGENT is the standard type, for a human agent.
        /// Tickets assigned to BOT will not send any automatic messages from our platform.
        /// BOT and API users can't login into our platform, they can only use the REST API.
        /// </summary>
        public Type type { get; set; }

        /// <summary>
        /// Full name of the user. This value is not exposed to customers.
        /// </summary>
        public string fullName { get; set; }

        /// <summary>
        /// First name of the user. This value is exposed to customers.
        /// </summary>
        public string shortName { get; set; }

        /// <summary>
        /// Exposed in mail signatures.
        /// </summary>
        public string jobTitle { get; set; }

        /// <summary>
        /// The status of the user.
        /// </summary>
        public UserStatus status { get; set; }

        /// <summary>
        /// List of roles an agent can have.
        /// </summary>
        public List<string> roles { get; set; }

        /// <summary>
        /// Represents a user's status.
        /// </summary>
        public enum UserStatus
        {
            Offline,
            Break,
            // Note: The schema only lists Offline, Break, and Online,
            // but the code includes additional break states
            Break2,
            Break3,
            Break4,
            Break5,
            Online
        }

        /// <summary>
        /// The gender of the user.
        /// </summary>
        public enum Gender
        {
            Undefined = 0,
            Male = 1,
            Female = 2,
            notApplicable = 9
        }

        public enum Type
        {
            UNDEFINED,
            AGENT,
            BOT,
            API
        }
    }
}