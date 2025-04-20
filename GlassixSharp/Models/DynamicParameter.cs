using System;

namespace GlassixSharp.Models
{
    public class DynamicParameter
    {
        public Guid id;
        public Guid flowId;
        public string name;
        public string strValue;
        public bool boolValue;
        public decimal numValue;
        public int intValue;
        public string type;
        public string description;
        public DateTime creationDatetime;
    }
}
