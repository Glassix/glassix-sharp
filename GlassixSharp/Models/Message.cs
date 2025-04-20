using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GlassixSharp.Models
{
    public class Message
    {
        public string text;
        public string html;
        public ProtocolType protocolType;
        public List<string> attachmentUris;
        public string from;
        public string to;
        public DateTime dateTime;
        public string providerMessageId;
        public Guid departmentId;
        public string status;
        public List<string> files;
        public int serviceProvider;
    }
}
