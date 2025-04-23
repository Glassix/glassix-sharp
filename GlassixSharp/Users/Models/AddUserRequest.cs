using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlassixSharp.Users.Models
{
    /// <summary>
    /// Request model for adding a user
    /// </summary>
    public class AddUserRequest
    {
        public string userName { get; set; }
        public string uniqueArgument { get; set; }
    }
}
