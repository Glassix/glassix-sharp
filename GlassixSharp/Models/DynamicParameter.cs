using System;

namespace GlassixSharp.Models
{
    /// <summary>
    /// Custom-made parameters that are defined by the department. The values for these parameters will be collected during a chatbot conversation.
    /// </summary>
    public class DynamicParameter
    {
        /// <summary>
        /// The id of the parameter.
        /// </summary>
        public Guid id { get; set; }

        /// <summary>
        /// The flow Id on which this parameter was defined.
        /// </summary>
        public Guid flowId { get; set; }

        /// <summary>
        /// The parameter name.
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// String data that was filled by the customer during the chatbot conversation.
        /// </summary>
        public string strValue { get; set; }

        /// <summary>
        /// Boolean data that was filled by the customer during the chatbot conversation.
        /// </summary>
        public bool boolValue { get; set; }

        /// <summary>
        /// Number data that was filled by the customer during the chatbot conversation.
        /// </summary>
        public decimal numValue { get; set; }

        /// <summary>
        /// Integer data that was filled by the customer during the chatbot conversation.
        /// </summary>
        public int intValue { get; set; }

        /// <summary>
        /// The expected type of the data that is stored on the value property of this parameter.
        /// Possible values: String, Number, Boolean, Location, Attachment
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// Description of the parameter.
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// ISO-8601-formatted datetime of the creation date; as UTC.
        /// Example: 2021-12-30T08:58:27Z
        /// </summary>
        public DateTime creationDatetime { get; set; }
    }
}