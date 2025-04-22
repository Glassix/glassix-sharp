using GlassixSharp.Models;
using GlassixSharp.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
