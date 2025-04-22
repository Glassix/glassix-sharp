using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlassixSharp.CannedReplies.Models
{
    /// <summary>
    /// Represents a canned reply in the Glassix system
    /// </summary>
    public class CannedReply
    {
        /// <summary>
        /// Unique identifier for the canned reply
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// Title of the canned reply
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// Text content of the canned reply
        /// </summary>
        public string text { get; set; }

        /// <summary>
        /// HTML content of the canned reply
        /// </summary>
        public string html { get; set; }

        /// <summary>
        /// Ranking of the canned reply
        /// </summary>
        public int rank { get; set; }

        /// <summary>
        /// Tags that are automatically added when using this canned reply
        /// </summary>
        public List<string> autoTags { get; set; }

        /// <summary>
        /// Category name this canned reply belongs to
        /// </summary>
        public string categoryName { get; set; }

        /// <summary>
        /// Culture code for the canned reply (e.g., "en-US")
        /// </summary>
        public string culture { get; set; }
    }

}
