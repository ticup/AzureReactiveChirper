using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orleans.Azure.Samples.ReactiveChirper.Communication
{
    class JsonException: JsonMessage
    {
        public new string Type = "Exception";
        public string Text;
    }
}
