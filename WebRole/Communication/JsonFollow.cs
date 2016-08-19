using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orleans.Azure.Samples.ReactiveChirper.Communication
{
    class JsonFollow: JsonMessage
    {
        public new string Type = "Follow";
        public string Username;
        public string ToFollow;
    }
}
