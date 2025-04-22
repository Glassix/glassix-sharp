using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlassixSharp.Users.Models
{
    /// <summary>
    /// Response from delete user endpoint
    /// </summary>
    public class DeleteUserResponse
    {
        public string Message { get; set; }
        public List<string> DeletedFromDepartments { get; set; }
    }
}
