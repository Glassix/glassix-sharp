using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlassixSharp.Tenants.Models
{
    /// <summary>
    /// Represents a ticket tag
    /// </summary>
    public class Tag
    {
        /// <summary>
        /// The name of the tag
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The color of the tag (Hex color code)
        /// </summary>
        public string Color { get; set; } = string.Empty;

        /// <summary>
        /// List of parent tags
        /// </summary>
        public List<string> ParentTags { get; set; } = new List<string>();

        /// <summary>
        /// Whether the tag is deleted
        /// </summary>
        public bool IsDeleted { get; set; }
    }
}
