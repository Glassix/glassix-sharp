using GlassixSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GlassixSharp.Protocols.Models
{
    public class Message
    {
        public string text { get; set; }
        public string html { get; set; }
        public ProtocolType protocolType { get; set; }
        public List<string> attachmentUris { get; set; }
        public string from { get; set; }
        public string to { get; set; }
        public DateTime dateTime { get; set; }
        public string providerMessageId { get; set; }
        public Guid departmentId { get; set; }
        public string status { get; set; }
        public List<string> files { get; set; }
        public int serviceProvider { get; set; }
    }
}
