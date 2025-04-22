using System;
using System.Collections.Generic;
using System.Text;

namespace GlassixSharp.Tickets.Models
{
    public class CardData
    {
        public Guid departmentId { get; set; }
        public Guid botConversationId { get; set; }
        public Guid flowId { get; set; }
        public double flowVersion { get; set; }
        public string flowName { get; set; }
        public string stepType { get; set; }
        public Guid uniqueCardId { get; set; }
        public string cardFriendlyName { get; set; }
        public DateTime visitDatetime { get; set; }
    }
}
