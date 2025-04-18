namespace GlassixSharp.Models.Requests
{
    /// <summary>
    /// Request to update ticket fields
    /// </summary>
    public class SetTicketFieldsRequest
    {
        /// <summary>
        /// Free text field for ticket subject
        /// </summary>
        public string Field1 { get; set; }
        
        /// <summary>
        /// Dynamic field
        /// </summary>
        public string Field2 { get; set; }
        
        /// <summary>
        /// Dynamic field
        /// </summary>
        public string Field3 { get; set; }
        
        /// <summary>
        /// Dynamic field
        /// </summary>
        public string Field4 { get; set; }
        
        /// <summary>
        /// Dynamic field
        /// </summary>
        public string Field5 { get; set; }
        
        /// <summary>
        /// Dynamic field
        /// </summary>
        public string Field6 { get; set; }
        
        /// <summary>
        /// Dynamic field
        /// </summary>
        public string Field7 { get; set; }
        
        /// <summary>
        /// Dynamic field
        /// </summary>
        public string Field8 { get; set; }
        
        /// <summary>
        /// Dynamic field
        /// </summary>
        public string Field9 { get; set; }
        
        /// <summary>
        /// Dynamic field
        /// </summary>
        public string Field10 { get; set; }
        
        /// <summary>
        /// Custom argument for CRM sync
        /// </summary>
        public string UniqueArgument { get; set; }
        
        /// <summary>
        /// Additional details about the ticket
        /// </summary>
        public TicketDetails Details { get; set; }
    }
}
