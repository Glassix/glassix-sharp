using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GlassixSharp.Models;
using GlassixSharp.Models.Requests;
using GlassixSharp.Models.Responses;

namespace GlassixSharp
{
    /// <summary>
    /// Interface for interacting with the Glassix API
    /// </summary>
    public interface IGlassixClient
    {
        #region Tickets
        Task<(bool Success, Ticket Data, string Error)> CreateTicketAsync(CreateTicketRequest request, CancellationToken cancellationToken = default);

        Task<(bool Success, Ticket Data, string Error)> GetTicketAsync(int ticketId, CancellationToken cancellationToken = default);

        Task<(bool Success, TicketListResponse Data, string Error)> ListTicketsAsync(DateTime since, DateTime until, Ticket.State? ticketState = null, SortOrder? sortOrder = null, string page = null, CancellationToken cancellationToken = default);

        Task<(bool Success, Transaction Data, string Error)> SendMessageAsync(int ticketId, SendMessageRequest request, CancellationToken cancellationToken = default);

        Task<(bool Success, MessageResponse Data, string Error)> SetTicketStateAsync(int ticketId, Ticket.State nextState, bool getTicket = false, bool sendTicketStateChangedMessage = true, bool enableWebhook = true, SetTicketStateRequest body = null, CancellationToken cancellationToken = default);

        Task<(bool Success, string Error)> SetTicketFieldsAsync(int ticketId, SetTicketFieldsRequest request, CancellationToken cancellationToken = default);

        Task<(bool Success, List<string> Data, string Error)> AddTicketTagsAsync(int ticketId, List<string> tags, CancellationToken cancellationToken = default);

        Task<(bool Success, List<string> Data, string Error)> RemoveTicketTagAsync(int ticketId, string tag, CancellationToken cancellationToken = default);

        Task<(bool Success, string Error)> SetParticipantNameAsync(int ticketId, int participantId, string name, CancellationToken cancellationToken = default);

        Task<(bool Success, string Error)> SetTicketOwnerAsync(int ticketId, string nextOwnerUserName, bool keepCurrentOwnerInConversation = false, CancellationToken cancellationToken = default);

        Task<(bool Success, string Error)> AssignAvailableUserAsync(int ticketId, CancellationToken cancellationToken = default);

        Task<(bool Success, int NewTicketId, string Error)> SetDepartmentAsync(int ticketId, Guid departmentId, CancellationToken cancellationToken = default);

        Task<(bool Success, string Error)> AddNoteAsync(int ticketId, string text, string html = null, CancellationToken cancellationToken = default);

        Task<(bool Success, string Error)> ScrambleTicketAsync(int ticketId, CancellationToken cancellationToken = default);

        Task<(bool Success, byte[] PdfData, string Error)> GetTicketPdfAsync(int ticketId, TicketRenderOptions ticketRenderOptions, CancellationToken cancellationToken = default);

        Task<(bool Success, string HtmlContent, string Error)> GetTicketHtmlAsync(int ticketId, TicketRenderOptions ticketRenderOptions, CancellationToken cancellationToken = default);

        Task<(bool Success, Uri SurveyLink, string Error)> GenerateSurveyLinkAsync(int ticketId, int surveyId, int participantId = 0, CancellationToken cancellationToken = default);

        Task<(bool Success, string Error)> SetTicketSummaryAsync(int ticketId, string summary, CancellationToken cancellationToken = default);
        #endregion

        #region Users
        Task<(bool Success, List<User> Data, string Error)> GetAllUsersAsync(CancellationToken cancellationToken = default);

        Task<(bool Success, string Error)> SetUserStatusAsync(User.UserStatus nextStatus, CancellationToken cancellationToken = default);

        Task<(bool Success, UserStatusResponse Data, string Error)> GetUserStatusAsync(CancellationToken cancellationToken = default);
        #endregion

        #region Contacts
        Task<(bool Success, Contact Data, string Error)> GetContactAsync(Guid contactId, CancellationToken cancellationToken = default);

        Task<(bool Success, MessageResponse Data, string Error)> SetContactNameAsync(Guid contactId, string nextName, CancellationToken cancellationToken = default);
        #endregion

        #region Protocol
        Task<(bool Success, Message Data, string Error)> SendProtocolMessageAsync(Message request, CancellationToken cancellationToken = default);
        #endregion

        #region Webhooks
        Task<(bool Success, List<WebhookEvent> Data, string Error)> GetWebhookEventsAsync(bool deleteEvents = true, CancellationToken cancellationToken = default);
        #endregion
    }
}