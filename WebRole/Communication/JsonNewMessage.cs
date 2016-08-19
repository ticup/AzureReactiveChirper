using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orleans.Azure.Samples.ReactiveChirper.Communication
{
    class JsonNewMessage : JsonMessage
    {
        public new string Type = "NewMessage";
        public string Username;
        public string Message;
    }
}
