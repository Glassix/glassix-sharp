using GlassixSharp.Models;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GlassixSharp
{
    public class Ticket
    {
        /// <summary>
        /// Unique identifier for a ticket.
        /// </summary>
        public int id;

        /// <summary>
        /// The department this ticket belongs to. (this value is the same as the apiKey)
        /// </summary>
        public Guid departmentId;

        /// <summary>
        /// Free text field for ticket subject.
        /// </summary>
        public string field1;

        /// <summary>
        /// Dynamic field.
        /// </summary>
        public string field2;

        /// <summary>
        /// Dynamic field.
        /// </summary>
        public string field3;

        /// <summary>
        /// Dynamic field.
        /// </summary>
        public string field4;

        /// <summary>
        /// Dynamic field.
        /// </summary>
        public string field5;

        /// <summary>
        /// Dynamic field.
        /// </summary>
        public string field6;

        /// <summary>
        /// Dynamic field.
        /// </summary>
        public string field7;

        /// <summary>
        /// Dynamic field.
        /// </summary>
        public string field8;

        /// <summary>
        /// Dynamic field.
        /// </summary>
        public string field9;

        /// <summary>
        /// Dynamic field.
        /// </summary>
        public string field10;

        /// <summary>
        /// Restricted code list to note ticket/participant's language/culture. Can customize user's GUI.
        /// Example values: en-US (English), he-IL (Hebrew), pt-PT (Portuguese), es-ES (Spanish)
        /// </summary>
        public string culture;

        /// <summary>
        /// Determines whether the ticket is with an agent, a bot, or a Glassix bot based on the owner.
        /// Possible values: Regular, Bot, GlassixBot
        /// </summary>
        public string ticketType;

        /// <summary>
        /// List of tags associated with the ticket.
        /// </summary>
        public List<string> tags;

        /// <summary>
        /// The user (agent) who owns this ticket.
        /// </summary>
        public User owner;

        /// <summary>
        /// The state of the ticket.
        /// Possible values: Closed, Open, Snoozed, Pending
        /// </summary>
        public State state;

        /// <summary>
        /// The date and time of when this ticket was opened. UTC
        /// </summary>
        public DateTime open;

        /// <summary>
        /// The date and time of when this ticket was closed. UTC
        /// </summary>
        public DateTime close;

        /// <summary>
        /// The date and time of the last activity on this ticket.
        /// </summary>
        public DateTime lastActivity;

        /// <summary>
        /// Custom parameter to assist sys admins to keep the CRM's tickets in sync, e.g., CRM case id.
        /// Returned on every event or endpoint.
        /// </summary>
        public string uniqueArgument;

        /// <summary>
        /// Additional details about this ticket.
        /// </summary>
        public Details details;

        /// <summary>
        /// Participant's communication channel. For user type 'User' only Web is applicable.
        /// </summary>
        public ProtocolType primaryProtocolType;

        /// <summary>
        /// New ticket id after cloning to another department.
        /// </summary>
        public int movedToTicketId;

        /// <summary>
        /// This ticket was moved to another department with this id.
        /// </summary>
        public Guid movedToDepartmentId;

        /// <summary>
        /// The ticket Id of the original ticket before cloning to this department.
        /// </summary>
        public int movedFromTicketId;

        /// <summary>
        /// This ticket was moved from another department with this id.
        /// </summary>
        public Guid movedFromDepartmentId;

        /// <summary>
        /// The first time a customer has sent a message.
        /// </summary>
        public DateTime? firstCustomerMessageDateTime;

        /// <summary>
        /// The first time an agent has sent a message.
        /// </summary>
        public DateTime? firstAgentMessageDateTime;

        /// <summary>
        /// The first agent who sent a message.
        /// </summary>
        public Guid? firstAgentMessageUserId;

        /// <summary>
        /// Owner's response time to the customer's first inbound message (inbound tickets only).
        /// </summary>
        public TimeSpan? agentAssignToResponseTimeGross;

        /// <summary>
        /// Owner's net response time to the customer's first inbound message during working hours (inbound tickets only).
        /// </summary>
        public TimeSpan? agentAssignToResponseTimeNet;

        /// <summary>
        /// The last time a customer has sent a message.
        /// </summary>
        public DateTime? lastCustomerMessageDateTime;

        /// <summary>
        /// The last time an agent has sent a message.
        /// </summary>
        public DateTime? lastAgentMessageDateTime;

        /// <summary>
        /// Total time ticket was un-assigned.
        /// </summary>
        public TimeSpan? queueTimeGross;

        /// <summary>
        /// Total net time ticket was un-assigned during working hours.
        /// </summary>
        public TimeSpan? queueTimeNet;

        /// <summary>
        /// Agent's gross response time to the customer's first inbound message (inbound tickets only).
        /// </summary>
        public TimeSpan? firstAgentResponseTimeGross;

        /// <summary>
        /// Agent's net response time to the customer's first inbound message during working hours (inbound tickets only).
        /// </summary>
        public TimeSpan? firstAgentResponseTimeNet;

        /// <summary>
        /// The timestamp of the first time the ticket was assigned to an agent.
        /// </summary>
        public DateTime? firstAgentAllocationTimestamp;

        /// <summary>
        /// The timestamp of the last time the ticket was assigned to an agent.
        /// </summary>
        public DateTime? lastAgentAllocationTimestamp;

        /// <summary>
        /// How many images, files etc were sent by agents in this ticket.
        /// </summary>
        public int agentMediaCount;

        /// <summary>
        /// How many images, files etc were sent by customers in this ticket.
        /// </summary>
        public int customerMediaCount;

        /// <summary>
        /// How many messages were sent by agents in this ticket.
        /// </summary>
        public int agentMessagesCount;

        /// <summary>
        /// How many messages were sent by customers in this ticket.
        /// </summary>
        public int customerMessagesCount;

        /// <summary>
        /// Total net ticket's duration of this ticket from ticket open to closure during working hours.
        /// </summary>
        public TimeSpan durationNet;

        /// <summary>
        /// Total ticket's duration from ticket open to closure.
        /// </summary>
        public TimeSpan durationGross;

        /// <summary>
        /// The net average response time it took to the agent to respond for each message sent by the customer from ticket open to closure during working hours.
        /// </summary>
        public TimeSpan? agentResponseAverageTimeNet;

        /// <summary>
        /// The total time of the conversation.
        /// </summary>
        public TimeSpan? totalConversationTimeNet;

        /// <summary>
        /// How many canned replies did the agent sent in this ticket.
        /// </summary>
        public int agentCannedRepliesCount;

        /// <summary>
        /// Bot conversation identifier.
        /// </summary>
        public Guid botConversationId;

        /// <summary>
        /// List of bot conversation identifiers associated with this ticket.
        /// </summary>
        public List<Guid> botConversations;

        /// <summary>
        /// Indicates if this ticket was initiated by a customer (true) or an agent (false).
        /// </summary>
        public bool isIncoming;

        /// <summary>
        /// Indicates if the ticket data has been scrambled for privacy.
        /// </summary>
        public bool isScrambled;

        /// <summary>
        /// List of participants involved in this ticket.
        /// </summary>
        public List<Participant> participants;

        /// <summary>
        /// List of transactions (messages and events) in this ticket.
        /// </summary>
        public List<Transaction> transactions;

        /// <summary>
        /// List of dynamic parameters associated with this ticket.
        /// </summary>
        public List<DynamicParameter> dynamicParameters;

        /// <summary>
        /// Detailed information on the cards in the bot the customer went through.
        /// </summary>
        public List<CardData> botConversationSteps;

        /// <summary>
        /// Summary of the ticket.
        /// </summary>
        public TicketSummary ticketSummary;

        /// <summary>
        /// IDs of tickets nested under this ticket.
        /// </summary>
        public List<int> nestedTicketsIds;

        /// <summary>
        /// ID of the parent ticket if this is a nested ticket.
        /// </summary>
        public int parentNestedTicketId;

        /// <summary>
        /// Indicates if the ticket was closed automatically by the system.
        /// </summary>
        public bool wasClosedAutomatically;

        /// <summary>
        /// Total count of transactions in this ticket.
        /// </summary>
        public int transactionsCount;

        public Ticket() { }

        public Ticket(string field1, string field2, string field3, string field4, string field5, string field6, string field7, string field8, string field9, string field10, string culture, List<Participant> participants)
        {
            this.field1 = field1;
            this.field2 = field2;
            this.field3 = field3;
            this.field4 = field4;
            this.field5 = field5;
            this.field6 = field6;
            this.field7 = field7;
            this.field8 = field8;
            this.field9 = field9;
            this.field10 = field10;
            this.culture = culture;
            this.participants = participants;
        }

        /// <summary>
        /// Represents the possible states of a ticket.
        /// </summary>
        public enum State
        {
            Undefined = 0,
            /// <summary>The ticket is closed.</summary>
            Closed = 1,
            /// <summary>The ticket is open.</summary>
            Open = 2,
            /// <summary>The ticket is snoozed (temporarily suspended).</summary>
            Snoozed = 3,
            /// <summary>The ticket is pending.</summary>
            Pending = 4
        }

        /// <summary>
        /// Contains summary information about the ticket.
        /// </summary>
        public class TicketSummary
        {
            /// <summary>
            /// The ID of the user who created the summary.
            /// </summary>
            public Guid userId;

            /// <summary>
            /// The timestamp when the summary was last updated.
            /// </summary>
            public DateTime? lastUpdateTimestamp;

            /// <summary>
            /// The summary text.
            /// </summary>
            public string value;
        }

        /// <summary>
        /// Contains additional details about the ticket.
        /// </summary>
        public class Details
        {
            /// <summary>
            /// The user agent of the customer.
            /// </summary>
            public string userAgent;

            /// <summary>
            /// The IP of the customer.
            /// </summary>
            public string iPAddress;

            /// <summary>
            /// City, State, Country etc.
            /// </summary>
            public string location;

            /// <summary>
            /// Path of bot conversation cards.
            /// </summary>
            public string cardsPath;

            /// <summary>
            /// The source from which this ticket was opened. Can be a website address, a Facebook page, etc.
            /// </summary>
            public Link source;

            /// <summary>
            /// Link to the CRM ticket (referenced in the unique argument) or any other link you want the agent to see with the ticket details.
            /// </summary>
            public Uri externalLink;

            /// <summary>
            /// Was this ticket opened on the mobile. This value is valid only for chat tickets.
            /// </summary>
            public bool isMobile;

            /// <summary>
            /// Claims from identity token used for authentication.
            /// </summary>
            public Dictionary<string, string> identityTokenClaims;

            /// <summary>
            /// Referral information for this ticket.
            /// </summary>
            public Referral referral;

            /// <summary>
            /// Represents a link with title and URI.
            /// </summary>
            public class Link
            {
                /// <summary>
                /// Source title.
                /// </summary>
                public string title;

                /// <summary>
                /// Source URI.
                /// </summary>
                public Uri uri;
            }

            /// <summary>
            /// Contains referral information for the ticket.
            /// </summary>
            public class Referral
            {
                /// <summary>
                /// UTM parameters are short pieces of code (starts with utm_) added to links,
                /// including information about link placement and purpose for tracking clicks and traffic.
                /// </summary>
                public Dictionary<string, string> utmParameters;

                /// <summary>
                /// Facebook ad/Instagram ad id that this ticket originated from.
                /// </summary>
                public string adId;

                /// <summary>
                /// Facebook ad/Instagram ad title that this ticket originated from.
                /// </summary>
                public string adTitle;

                /// <summary>
                /// Data passed as a query string to messenger links.
                /// </summary>
                public string refData;

                /// <summary>
                /// Product ID from the Ad the user is interested in.
                /// </summary>
                public string productId;

                /// <summary>
                /// The fb post id of the ad.
                /// </summary>
                public string fbPostId;

                /// <summary>
                /// Instagram story identifier.
                /// </summary>
                public string instagramStoryId;

                /// <summary>
                /// URL to Instagram story media.
                /// </summary>
                public Uri instagramStoryMediaUrl;

                /// <summary>
                /// Apple business chat intent identifier for message routing.
                /// </summary>
                public string appleIntentId;

                /// <summary>
                /// Apple business chat group identifier for message routing.
                /// </summary>
                public string appleGroupId;

                /// <summary>
                /// Twitter root tweet identifier.
                /// </summary>
                public string twitterRootTweetId;

                /// <summary>
                /// TikTok video identifier.
                /// </summary>
                public string tikTokVideoId;

                /// <summary>
                /// TikTok comment identifier.
                /// </summary>
                public string tikTokCommentId;

                /// <summary>
                /// WhatsApp referral headline.
                /// </summary>
                public string whatsappHeadline;

                /// <summary>
                /// WhatsApp referral body.
                /// </summary>
                public string whatsappBody;

                /// <summary>
                /// WhatsApp referral source type.
                /// </summary>
                public string whatsappSourceType;

                /// <summary>
                /// WhatsApp referral source identifier.
                /// </summary>
                public string whatsappSourceId;

                /// <summary>
                /// WhatsApp referral source URL.
                /// </summary>
                public Uri whatsappSourceUrl;
            }
        }
    }
}