using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace GlassixSharp.Models
{
    public class User
    {
        public Guid id;
        public Gender gender;
        public string UserName;
        public string culture;
        public bool isAnonymous;
        public string uniqueArgument;
        public string type;
        public string fullName;
        public string shortName;
        public string jobTitle;
        public UserStatus status;
        public List<string> roles;

        /// <summary>
        /// Represents a user's status
        /// </summary>
        public enum UserStatus
        {
            Offline,
            Break,
            Break2,
            Break3,
            Break4,
            Break5,
            Online
        }

        public enum Gender
        {
            Undefined = 0,
            Male = 1,
            Female = 2,
            notApplicable = 9
        }
    }
}
