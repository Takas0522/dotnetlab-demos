using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace E2etestingInPlaywright.Api.Models
{
    public class User
    {
        [DataMember(Name = "id")]
        public long Id { get; set; }

        [DataMember(Name = "email")]
        public string Email { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "role")]
        public RoleType Role { get; set; }
    }

    public enum RoleType
    {
        Admin,
        User
    }
}
