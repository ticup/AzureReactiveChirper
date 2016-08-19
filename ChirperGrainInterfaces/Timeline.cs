using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrainInterfaces
{
    public class Timeline
    {
        public List<Message> Posts { get; set; }
        public Timeline(List<Message> posts)
        {
            Posts = posts;
        }
    }
}
