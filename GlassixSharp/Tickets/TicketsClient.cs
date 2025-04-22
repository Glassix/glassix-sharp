using GlassixSharp.Models;
using GlassixSharp.Models.Responses;
using GlassixSharp.Tickets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlassixSharp.Tickets
{
    public class TicketsClient : BaseGlassixClient
    {
        public TicketsClient(Credentials credentials, Dictionary<string, string>? headers = null) : base(credentials, headers)
        {

        }

        /// <summary>
        /// Creates a new ticket
        /// </summary>
        /// <param name="request">The ticket creation request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The created ticket</returns>
        public async Task<(bool Success, Ticket Data, string Error)> CreateTicketAsync(CreateTicketRequest request, CancellationToken cancellationToken = default)
        {
            var response = await this.SendRequestAsync<Ticket>(
                HttpMethod.Post,
                $"{_baseUrl}/tickets/create",
                request,
                true,
                cancellationToken).ConfigureAwait(false);

            return (response.IsSuccess, response.Data, response.ErrorMessage);
        }

        /// <summary>
        /// Gets a ticket by ID
        /// </summary>
        /// <param name="ticketId">ID of the ticket to get</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The requested ticket</returns>
        public async Task<(bool Success, Ticket Data, string Error)> GetTicketAsync(int ticketId, CancellationToken cancellationToken = default)
        {
            var response = await SendRequestAsync<Ticket>(
                HttpMethod.Get,
                $"{_baseUrl}/tickets/get/{ticketId}",
                null,
                true,
                cancellationToken).ConfigureAwait(false);

            return (response.IsSuccess, response.Data, response.ErrorMessage);
        }

        /// <summary>
        /// Lists tickets within a specific time frame
        /// </summary>
        /// <param name="since">Start date/time for the query</param>
        /// <param name="until">End date/time for the query</param>
        /// <param name="ticketState">Filter by ticket state (optional)</param>
        /// <param name="sortOrder">Sort order (optional)</param>
        /// <param name="page">Page token for pagination (optional)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of tickets matching the criteria</returns>
        public async Task<(bool Success, TicketListResponse Data, string Error)> ListTicketsAsync(DateTime since, DateTime until, Ticket.State? ticketState = null, SortOrder? sortOrder = null, string page = null, CancellationToken cancellationToken = default)
        {
            var queryParams = new Dictionary<string, object>
            {
                ["since"] = since,
                ["until"] = until
            };

            if (ticketState.HasValue)
                queryParams["ticketState"] = ticketState.Value;

            if (sortOrder.HasValue)
                queryParams["sortOrder"] = sortOrder.Value;

            if (!string.IsNullOrEmpty(page))
                queryParams["page"] = page;

            var queryString = BuildQueryString(queryParams);
            var url = $"{_baseUrl}/tickets/list";

            if (!string.IsNullOrEmpty(queryString))
                url += $"?{queryString}";

            var response = await SendRequestAsync<TicketListResponse>(
                HttpMethod.Get,
                url,
                null,
                true,
                cancellationToken).ConfigureAwait(false);

            return (response.IsSuccess, response.Data, response.ErrorMessage);
        }

        /// <summary>
        /// Sends a message in a ticket
        /// </summary>
        /// <param name="ticketId">ID of the ticket</param>
        /// <param name="request">The message to send</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The message transaction</returns>
        public async Task<(bool Success, Transaction Data, string Error)> SendMessageAsync(int ticketId, SendMessageRequest request, CancellationToken cancellationToken = default)
        {
            var response = await SendRequestAsync<Transaction>(
                HttpMethod.Post,
                $"{_baseUrl}/tickets/send/{ticketId}",
                request,
                true,
                cancellationToken).ConfigureAwait(false);

            return (response.IsSuccess, response.Data, response.ErrorMessage);
        }

        /// <summary>
        /// Sets the state of a ticket
        /// </summary>
        /// <param name="ticketId">ID of the ticket</param>
        /// <param name="nextState">The new state for the ticket</param>
        /// <param name="getTicket">Whether to return the updated ticket</param>
        /// <param name="sendTicketStateChangedMessage">Whether to send a message about the state change</param>
        /// <param name="enableWebhook">Whether to trigger webhooks for this change</param>
        /// <param name="body">Additional parameters (optional)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result of the operation</returns>
        public async Task<(bool Success, MessageResponse Data, string Error)> SetTicketStateAsync(int ticketId, Ticket.State nextState, bool getTicket = false, bool sendTicketStateChangedMessage = true, bool enableWebhook = true, SetTicketStateRequest body = null, CancellationToken cancellationToken = default)
        {
            var queryParams = new Dictionary<string, object>
            {
                ["nextState"] = nextState,
                ["getTicket"] = getTicket,
                ["sendTicketStateChangedMessage"] = sendTicketStateChangedMessage,
                ["enableWebhook"] = enableWebhook
            };

            var queryString = BuildQueryString(queryParams);
            var url = $"{_baseUrl}/tickets/setstate/{ticketId}?{queryString}";

            var response = await SendRequestAsync<MessageResponse>(
                HttpMethod.Put,
                url,
                body,
                true,
                cancellationToken).ConfigureAwait(false);

            return (response.IsSuccess, response.Data, response.ErrorMessage);
        }

        /// <summary>
        /// Updates the fields of a ticket
        /// </summary>
        /// <param name="ticketId">ID of the ticket</param>
        /// <param name="request">The fields to update</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result of the operation</returns>
        public async Task<(bool Success, string Error)> SetTicketFieldsAsync(int ticketId, SetTicketFieldsRequest request, CancellationToken cancellationToken = default)
        {
            var response = await SendRequestAsync<EmptyResponse>(
                HttpMethod.Put,
                $"{_baseUrl}/tickets/setfields/{ticketId}",
                request,
                true,
                cancellationToken).ConfigureAwait(false);

            return (response.IsSuccess, response.ErrorMessage);
        }

        /// <summary>
        /// Adds tags to a ticket
        /// </summary>
        /// <param name="ticketId">ID of the ticket</param>
        /// <param name="tags">Tags to add</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The updated list of tags</returns>
        public async Task<(bool Success, List<string> Data, string Error)> AddTicketTagsAsync(int ticketId, List<string> tags, CancellationToken cancellationToken = default)
        {
            var response = await SendRequestAsync<List<string>>(
                HttpMethod.Post,
                $"{_baseUrl}/tickets/addtags/{ticketId}",
                tags,
                true,
                cancellationToken).ConfigureAwait(false);

            return (response.IsSuccess, response.Data, response.ErrorMessage);
        }

        /// <summary>
        /// Removes a tag from a ticket
        /// </summary>
        /// <param name="ticketId">ID of the ticket</param>
        /// <param name="tag">Tag to remove</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The updated list of tags</returns>
        public async Task<(bool Success, List<string> Data, string Error)> RemoveTicketTagAsync(int ticketId, string tag, CancellationToken cancellationToken = default)
        {
            var response = await SendRequestAsync<List<string>>(
                HttpMethod.Delete,
                $"{_baseUrl}/tickets/removetag/{ticketId}?tag={Uri.EscapeDataString(tag)}",
                null,
                true,
                cancellationToken).ConfigureAwait(false);

            return (response.IsSuccess, response.Data, response.ErrorMessage);
        }

        /// <summary>
        /// Updates a participant's name within a ticket
        /// </summary>
        /// <param name="ticketId">ID of the ticket</param>
        /// <param name="participantId">ID of the participant in the ticket</param>
        /// <param name="name">New name for the participant</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result of the operation</returns>
        public async Task<(bool Success, string Error)> SetParticipantNameAsync(int ticketId, int participantId, string name, CancellationToken cancellationToken = default)
        {
            var request = new
            {
                id = participantId,
                name = name
            };

            var response = await SendRequestAsync<EmptyResponse>(
                HttpMethod.Put,
                $"{_baseUrl}/tickets/setparticipantname/{ticketId}",
                request,
                true,
                cancellationToken).ConfigureAwait(false);

            return (response.IsSuccess, response.ErrorMessage);
        }

        /// <summary>
        /// Assigns a new owner to a ticket
        /// </summary>
        /// <param name="ticketId">ID of the ticket</param>
        /// <param name="keepCurrentOwnerInConversation">Whether to keep the current owner as a participant</param>
        /// <param name="nextOwnerUserName">Email address of the new owner</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result of the operation</returns>
        public async Task<(bool Success, string Error)> SetTicketOwnerAsync(int ticketId, string nextOwnerUserName, bool keepCurrentOwnerInConversation = false, CancellationToken cancellationToken = default)
        {
            Dictionary<string, object> queryParams = new Dictionary<string, object>
            {
                ["keepCurrentOwnerInConversation"] = keepCurrentOwnerInConversation,
                ["nextOwnerUserName"] = nextOwnerUserName
            };

            string queryString = BuildQueryString(queryParams);
            string url = $"{_baseUrl}/tickets/setowner/{ticketId}?{queryString}";

            var response = await SendRequestAsync<EmptyResponse>(
                HttpMethod.Put,
                url,
                null,
                true,
                cancellationToken).ConfigureAwait(false);

            return (response.IsSuccess, response.ErrorMessage);
        }

        /// <summary>
        /// Assigns an available user to a ticket
        /// </summary>
        /// <param name="ticketId">ID of the ticket</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result of the operation</returns>
        public async Task<(bool Success, string Error)> AssignAvailableUserAsync(int ticketId, CancellationToken cancellationToken = default)
        {
            var response = await SendRequestAsync<EmptyResponse>(
                HttpMethod.Put,
                $"{_baseUrl}/tickets/assignavailableuser/{ticketId}",
                null,
                true,
                cancellationToken).ConfigureAwait(false);

            return (response.IsSuccess, response.ErrorMessage);
        }

        /// <summary>
        /// Moves a ticket to another department
        /// </summary>
        /// <param name="ticketId">ID of the ticket</param>
        /// <param name="departmentId">ID of the target department</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>ID of the new ticket in the target department</returns>
        public async Task<(bool Success, int NewTicketId, string Error)> SetDepartmentAsync(int ticketId, Guid departmentId, CancellationToken cancellationToken = default)
        {
            var request = new
            {
                departmentId = departmentId.ToString()
            };

            var response = await SendRequestAsync<SetDepartmentResponse>(
                HttpMethod.Put,
                $"{_baseUrl}/tickets/setdepartment/{ticketId}",
                request,
                true,
                cancellationToken).ConfigureAwait(false);

            return (response.IsSuccess, response.Data?.ticketId ?? 0, response.ErrorMessage);
        }

        /// <summary>
        /// Adds a note to a ticket (visible only to agents)
        /// </summary>
        /// <param name="ticketId">ID of the ticket</param>
        /// <param name="text">Text content of the note</param>
        /// <param name="html">HTML content of the note</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result of the operation</returns>
        public async Task<(bool Success, string Error)> AddNoteAsync(int ticketId, string text = null, string html = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(text) && string.IsNullOrEmpty(html))
            {
                throw new ArgumentException("Either text or html must be provided.");
            }

            var request = new
            {
                text = text,
                html = html
            };

            var response = await SendRequestAsync<EmptyResponse>(
                HttpMethod.Post,
                $"{_baseUrl}/tickets/addnote/{ticketId}",
                request,
                true,
                cancellationToken).ConfigureAwait(false);

            return (response.IsSuccess, response.ErrorMessage);
        }

        /// <summary>
        /// Permanently deletes (scrambles) a ticket's data
        /// </summary>
        /// <param name="ticketId">ID of the ticket</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result of the operation</returns>
        public async Task<(bool Success, string Error)> ScrambleTicketAsync(int ticketId, CancellationToken cancellationToken = default)
        {
            var response = await SendRequestAsync<EmptyResponse>(
                HttpMethod.Delete,
                $"{_baseUrl}/tickets/scramble/{ticketId}",
                null,
                true,
                cancellationToken).ConfigureAwait(false);

            return (response.IsSuccess, response.ErrorMessage);
        }

        /// <summary>
        /// Generates a PDF document of the ticket
        /// </summary>
        /// <param name="ticketId">ID of the ticket</param>
        /// <param name="includeDetails">Whether to include ticket details</param>
        /// <param name="includeConversationLink">Whether to include conversation link</param>
        /// <param name="includeNotes">Whether to include notes</param>
        /// <param name="replaceContentId">Whether to replace content IDs</param>
        /// <param name="showParticipantType">Whether to show participant types</param>
        /// <param name="fontSizeInPixels">Font size in pixels</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>PDF file as byte array</returns>
        public async Task<(bool Success, byte[] PdfData, string Error)> GetTicketPdfAsync(int ticketId, TicketRenderOptions ticketRenderOptions, CancellationToken cancellationToken = default)
        {
            var response = await SendRequestAsync<byte[]>(
                HttpMethod.Post,
                $"{_baseUrl}/tickets/pdf/{ticketId}",
                ticketRenderOptions,
                true,
                cancellationToken).ConfigureAwait(false);

            return (response.IsSuccess, response.Data, response.ErrorMessage);
        }

        /// <summary>
        /// Generates an HTML document of the ticket
        /// </summary>
        /// <param name="ticketId">ID of the ticket</param>
        /// <param name="includeDetails">Whether to include ticket details</param>
        /// <param name="includeConversationLink">Whether to include conversation link</param>
        /// <param name="includeNotes">Whether to include notes</param>
        /// <param name="fontSizeInPixels">Font size in pixels</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>HTML content as string</returns>
        public async Task<(bool Success, string HtmlContent, string Error)> GetTicketHtmlAsync(int ticketId, TicketRenderOptions ticketRenderOptions, CancellationToken cancellationToken = default)
        {
            var response = await SendRequestAsync<string>(
                HttpMethod.Post,
                $"{_baseUrl}/tickets/html/{ticketId}",
                ticketRenderOptions,
                true,
                cancellationToken).ConfigureAwait(false);

            return (response.IsSuccess, response.Data, response.ErrorMessage);
        }

        /// <summary>
        /// Generates a survey link for a ticket
        /// </summary>
        /// <param name="ticketId">ID of the ticket</param>
        /// <param name="surveyId">ID of the survey</param>
        /// <param name="participantId">ID of the participant (0 for main participant)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The generated survey link</returns>
        public async Task<(bool Success, Uri SurveyLink, string Error)> GenerateSurveyLinkAsync(int ticketId, int surveyId, int participantId = 0, CancellationToken cancellationToken = default)
        {
            var request = new
            {
                surveyId = surveyId,
                participantId = participantId
            };

            var response = await SendRequestAsync<SurveyLinkResponse>(
                HttpMethod.Post,
                $"{_baseUrl}/tickets/generatesurveylink/{ticketId}",
                request,
                true,
                cancellationToken).ConfigureAwait(false);

            return (response.IsSuccess, response.Data?.surveyLink, response.ErrorMessage);
        }

        /// <summary>
        /// Sets a summary for a ticket
        /// </summary>
        /// <param name="ticketId">ID of the ticket</param>
        /// <param name="summary">The summary text</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result of the operation</returns>
        public async Task<(bool Success, string Error)> SetTicketSummaryAsync(int ticketId, string summary, CancellationToken cancellationToken = default)
        {
            var request = new
            {
                summary = summary
            };

            var response = await SendRequestAsync<EmptyResponse>(
                HttpMethod.Post,
                $"{_baseUrl}/tickets/setsummary/{ticketId}",
                request,
                true,
                cancellationToken).ConfigureAwait(false);

            return (response.IsSuccess, response.ErrorMessage);
        }
    }
}
