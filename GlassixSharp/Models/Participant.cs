using System;
using System.Text.Json.Serialization;
namespace GlassixSharp.Models
{
    /// <summary>
    /// A participant is someone who belongs to a specific ticket. Each ticket has at least 2 participants, who can be either an agent or a customer.
    /// </summary>
    public class Participant
    {
        /// <summary>
        /// Participant Id in the ticket.
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// The name of the participant.
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Client is the end user you're communicating with.
        /// User is a Glassix agent. This is used in cases where you want to add agent participants that are not the owners of the ticket or a conversation between agents only.
        /// </summary>
        public Type type { get; set; }

        /// <summary>
        /// The identifier of the customer.
        /// </summary>
        public string identifier { get; set; }

        /// <summary>
        /// The identifier of the department.
        /// </summary>
        public string departmentIdentifier { get; set; }

        /// <summary>
        /// The name that will be displayed for the customer.
        /// </summary>
        public string displayName { get; set; }

        /// <summary>
        /// Participant's communication channel. For user type 'User' only Web is applicable.
        /// </summary>
        public ProtocolType protocolType { get; set; }

        /// <summary>
        /// The sub-protocol type used for communication.
        /// </summary>
        public SubProtocolType subProtocolType { get; set; }

        /// <summary>
        /// Switch to determine if the participant is active in the conversation.
        /// </summary>
        public bool isActive { get; set; } = true;

        /// <summary>
        /// Switch to determine if the participant is deleted from the conversation.
        /// </summary>
        public bool isDeleted { get; set; }

        /// <summary>
        /// The Glassix contact id of this participant. If you don't know it keep this field empty.
        /// </summary>
        public Guid contactId { get; set; }

        /// <summary>
        /// Relevant only to agent participants.
        /// </summary>
        public string userName { get; set; }

        /// <summary>
        /// Defines the type of participant in the system.
        /// </summary>
        public enum Type
        {
            Undefind = 0,

            /// <summary>
            /// Client is the end user you're communicating with.
            /// </summary>
            Client = 1,

            /// <summary>
            /// User is a Glassix agent. This is used in cases where you want to add agent participants that are not the owners of the ticket or a conversation between agents only.
            /// </summary>
            User = 2,

            /// <summary>
            /// System generated participant.
            /// </summary>
            System = 3
        }
    }
}