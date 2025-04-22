namespace GlassixSharp.Tickets.Models
{
    /// <summary>
    /// Request to update ticket fields
    /// </summary>
    public class SetTicketFieldsRequest
    {
        /// <summary>
        /// Free text field for ticket subject
        /// </summary>
        public string field1 { get; set; }
        
        /// <summary>
        /// Dynamic field
        /// </summary>
        public string field2 { get; set; }
        
        /// <summary>
        /// Dynamic field
        /// </summary>
        public string field3 { get; set; }
        
        /// <summary>
        /// Dynamic field
        /// </summary>
        public string field4 { get; set; }
        
        /// <summary>
        /// Dynamic field
        /// </summary>
        public string field5 { get; set; }
        
        /// <summary>
        /// Dynamic field
        /// </summary>
        public string field6 { get; set; }
        
        /// <summary>
        /// Dynamic field
        /// </summary>
        public string field7 { get; set; }
        
        /// <summary>
        /// Dynamic field
        /// </summary>
        public string field8 { get; set; }
        
        /// <summary>
        /// Dynamic field
        /// </summary>
        public string field9 { get; set; }
        
        /// <summary>
        /// Dynamic field
        /// </summary>
        public string field10 { get; set; }
        
        /// <summary>
        /// Custom argument for CRM sync
        /// </summary>
        public string uniqueArgument { get; set; }
        
        /// <summary>
        /// Additional details about the ticket
        /// </summary>
        public Ticket.Details details { get; set; }
    }
}
