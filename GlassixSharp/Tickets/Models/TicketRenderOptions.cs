using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlassixSharp.Tickets.Models
{
    public class TicketRenderOptions
    {
        /// <summary>
        /// Switch to determine whether to include conversation fields, tags, opening and closing hours and ticket owner.
        /// </summary>
        public bool includeDetails { get; set; } = true;

        /// <summary>
        /// Switch to determine whether to include link to the conversation at the end of the document.
        /// </summary>
        public bool includeConversationLink { get; set; } = true;

        /// <summary>
        /// Switch to determine whether to include conversation notes to the document.
        /// </summary>
        public bool includeNotes { get; set; } = true;

        /// <summary>
        /// Define the desired font size in pixels.
        /// </summary>
        public int fontSizeInPixels { get; set; } = 14;

        /// <summary>
        /// Switch to determine whether to include embedded images.
        /// </summary>
        public bool replaceContentId { get; set; } = true;

        /// <summary>
        /// Switch to determine whether to include participant's type.
        /// </summary>
        public bool showParticipantType { get; set; } = true;
    }
}
