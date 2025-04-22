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
        public string ShortName { get; set; }
        public string FullName { get; set; }
        public string JobTitle { get; set; }
    }
}
