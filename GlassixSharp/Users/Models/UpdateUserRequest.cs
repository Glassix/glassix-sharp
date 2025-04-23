using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlassixSharp.Users.Models
{
    /// <summary>
    /// Request model for updating a user
    /// </summary>
    public class UpdateUserRequest
    {
        public string shortName { get; set; }
        public string fullName { get; set; }
        public string jobTitle { get; set; }
    }
}
