using GlassixSharp.Models;
using System;
using System.Text.Json.Serialization;
namespace GlassixSharp.Models
{
    /// <summary>
    /// A Transaction represents an event that happened in a ticket such as a message, ticket state change,
    /// ticket was assigned to another agent and more.
    /// </summary>
    public class Transaction
    {
        /// <summary>
        /// The unique identifier for the transaction
        /// </summary>
        public Guid guidTransactionId { get; set; }

        /// <summary>
        /// The transaction identifier
        /// </summary>
        public long id { get; set; }

        /// <summary>
        /// The date and time this transaction was created
        /// </summary>
        public DateTime dateTime { get; set; }

        /// <summary>
        /// Participant's communication channel. For user type 'User' only Web is applicable.
        /// </summary>
        public ProtocolType fromProtocolType { get; set; }

        /// <summary>
        /// The type of transaction (Message, TicketClosed, TicketOpened, etc.)
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// The plain text content of the transaction
        /// </summary>
        public string text { get; set; }

        /// <summary>
        /// The HTML content of the transaction
        /// </summary>
        public string html { get; set; }

        /// <summary>
        /// The status of this transaction. If all the participants have read this message, then the status will be "Read".
        /// If only one has read it, and another only received it, then the status will be "Delivered"
        /// </summary>
        public string status { get; set; }

        /// <summary>
        /// The participant who created this transaction
        /// </summary>
        public Participant fromParticipant { get; set; }

        /// <summary>
        /// Files or documents attached to this transaction
        /// </summary>
        public Attachment[] attachments { get; set; }

        /// <summary>
        /// Additional data associated with this transaction
        /// </summary>
        public object payload { get; set; }

        /// <summary>
        /// An Attachment is part of a transaction, for example an image sent along with a message from a customer.
        /// </summary>
        public class Attachment
        {
            /// <summary>
            /// The attachment identifier
            /// </summary>
            public int id { get; set; }

            /// <summary>
            /// The type of the attachment (Image, Video, Document, File, Solution, InteractiveDocument, Audio)
            /// </summary>
            public string type { get; set; }

            /// <summary>
            /// The file name
            /// </summary>
            public string fileIdentifier { get; set; }

            /// <summary>
            /// The original file name. If an email was sent with the file name "example-image.jpg", 
            /// then the original file name will be "example-image.jpg". The fileIdentifier will be a random file name.
            /// </summary>
            public string originalFileName { get; set; }

            /// <summary>
            /// Thumbnails that were created for that file. A thumbnail of a video will be a frame from the video. 
            /// A thumbnail for a PDF file will be the first page.
            /// </summary>
            public Uri[] thumbnails { get; set; }

            /// <summary>
            /// The file identifiers for the thumbnails
            /// </summary>
            public string[] thumbnailsFileIdentifiers { get; set; }

            /// <summary>
            /// The URI of this file. Note that this URI has an expiry of 3 hours.
            /// </summary>
            public Uri uri { get; set; }

            /// <summary>
            /// If true, the file is embedded in the message and doesn't count as an inbound/outbound attachment.
            /// If false, the file is attached to the message and counted in the inbound/outbound attachments.
            /// </summary>
            public bool isEmbedded { get; set; }

            /// <summary>
            /// The current sanitization state of the attachment
            /// </summary>
            public SanitizationState sanitizationState { get; set; }

            /// <summary>
            /// Represents the possible sanitization states of an attachment
            /// </summary>
            public enum SanitizationState
            {
                /// <summary>
                /// Sanitization state is not defined
                /// </summary>
                Undefined,

                /// <summary>
                /// Attachment is allowed
                /// </summary>
                Allowed,

                /// <summary>
                /// Attachment is blocked
                /// </summary>
                Blocked,

                /// <summary>
                /// Attachment is being processed
                /// </summary>
                Processing,
            }
        }
    }
}