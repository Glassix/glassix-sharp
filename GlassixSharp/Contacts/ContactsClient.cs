using GlassixSharp.Contacts.Models;
using GlassixSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GlassixSharp.Contacts.Models.ContactIdentifier;

namespace GlassixSharp.Contacts
{
    public class ContactsClient : BaseGlassixClient
    {
        public ContactsClient(Credentials credentials, Dictionary<string, string>? headers = null) : base(credentials, headers)
        {

        }

        /// <summary>
        /// Gets a contact by ID
        /// </summary>
        /// <param name="contactId">ID of the contact</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The requested contact</returns>
        public async Task<(bool Success, Contact Data, string Error)> GetContactAsync(Guid contactId, CancellationToken cancellationToken = default)
        {
            var response = await SendRequestAsync<Contact>(
                HttpMethod.Get,
                $"{_baseUrl}/contacts/get/{contactId}",
                null,
                true,
                cancellationToken).ConfigureAwait(false);

            return (response.IsSuccess, response.Data, response.ErrorMessage);
        }

        /// <summary>
        /// Sets the name of a contact
        /// </summary>
        /// <param name="contactId">ID of the contact</param>
        /// <param name="nextName">The new name</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result of the operation</returns>
        public async Task<(bool Success, MessageResponse Data, string Error)> SetContactNameAsync(Guid contactId, string nextName, CancellationToken cancellationToken = default)
        {
            var response = await SendRequestAsync<MessageResponse>(
                HttpMethod.Put,
                $"{_baseUrl}/contacts/setname/{contactId}",
                new { nextName },
                true,
                cancellationToken).ConfigureAwait(false);

            return (response.IsSuccess, response.Data, response.ErrorMessage);
        }

        /// <summary>
        /// Adds an identifier to a contact
        /// </summary>
        /// <param name="contactId">ID of the contact</param>
        /// <param name="identifierType">Type of identifier (PhoneNumber, MailAddress, FacebookId, InstagramId)</param>
        /// <param name="identifier">The identifier value</param>
        /// <param name="forceMerge">Whether to merge contacts when the identifier belongs to another contact</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result of the operation</returns>
        public async Task<(bool Success, MessageResponse Data, string Error)> AddIdentifierAsync(
            Guid contactId,
            ContactIdentifier.IdentifierType identifierType,
            string identifier,
            bool forceMerge = false,
            CancellationToken cancellationToken = default)
        {
            var requestBody = new
            {
                forceMerge,
                identifierType,
                identifier
            };

            var response = await SendRequestAsync<MessageResponse>(
                HttpMethod.Post,
                $"{_baseUrl}/contacts/addidentifier/{contactId}",
                requestBody,
                true,
                cancellationToken).ConfigureAwait(false);

            return (response.IsSuccess, response.Data, response.ErrorMessage);
        }

        /// <summary>
        /// Sets a unique argument for a contact
        /// </summary>
        /// <param name="contactId">ID of the contact</param>
        /// <param name="nextUniqueArgument">The new unique argument</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result of the operation</returns>
        public async Task<(bool Success, MessageResponse Data, string Error)> SetUniqueArgumentAsync(
            Guid contactId,
            string nextUniqueArgument,
            CancellationToken cancellationToken = default)
        {
            var requestBody = new
            {
                nextUniqueArgument
            };

            var response = await SendRequestAsync<MessageResponse>(
                HttpMethod.Put,
                $"{_baseUrl}/contacts/setuniqueargument/{contactId}",
                requestBody,
                true,
                cancellationToken).ConfigureAwait(false);

            return (response.IsSuccess, response.Data, response.ErrorMessage);
        }

        /// <summary>
        /// Deletes an identifier from a contact
        /// </summary>
        /// <param name="contactId">ID of the contact</param>
        /// <param name="contactIdentifierId">ID of the identifier to delete</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result of the operation</returns>
        public async Task<(bool Success, MessageResponse Data, string Error)> DeleteIdentifierAsync(
            Guid contactId,
            int contactIdentifierId,
            CancellationToken cancellationToken = default)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "contactIdentifierId", contactIdentifierId }
            };

            string queryString = BuildQueryString(parameters);
            string url = $"{_baseUrl}/contacts/deleteidentifier/{contactId}";

            if (!string.IsNullOrEmpty(queryString))
            {
                url += $"?{queryString}";
            }

            var response = await SendRequestAsync<MessageResponse>(
                HttpMethod.Delete,
                url,
                null,
                true,
                cancellationToken).ConfigureAwait(false);

            return (response.IsSuccess, response.Data, response.ErrorMessage);
        }
    }
}
