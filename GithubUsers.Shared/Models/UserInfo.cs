using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GithubUsers.Shared.Models
{
    public class UserInfo
    {
        public string Name { get; set; }
        public string Login { get; set; }
        public string Company { get; set; }
        public int Followers { get; set; }
        public int Public_Repos { get; set; }
        public int Average_Public_Repos { get; set; }
    }

}
