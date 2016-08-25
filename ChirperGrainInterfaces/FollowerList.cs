using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChirperGrainInterfaces
{
    public class FollowerList
    {
        public string Username { get; set; }
        public List<string> Followers { get; set; }

        public FollowerList(string username, List<string> followers)
        {
            Username = username;
            Followers = followers;
        }
    }
}
