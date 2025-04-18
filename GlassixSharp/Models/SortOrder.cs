using System.Runtime.Serialization;

namespace GlassixSharp.Models
{
    /// <summary>
    /// Represents the sort order for listing tickets
    /// </summary>
    public enum SortOrder
    {
        /// <summary>
        /// Sort in ascending order (oldest to newest)
        /// </summary>
        [EnumMember(Value = "ASC")]
        Ascending,
        
        /// <summary>
        /// Sort in descending order (newest to oldest)
        /// </summary>
        [EnumMember(Value = "DESC")]
        Descending
    }
}
