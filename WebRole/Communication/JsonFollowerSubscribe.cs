using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orleans.Azure.Samples.ReactiveChirper.Communication
{
    class JsonFollowerSubscribe: JsonMessage
    {
        public new string Type = "FollowerSubscribe";
        public string Username;
    }
}
