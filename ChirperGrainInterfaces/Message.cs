using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrainInterfaces
{
    [Serializable]
    public class Message
    {
        public Guid MessageId { get; private set; }
        public string Text { get; set; }
        public DateTime Timestamp { get; set; }
        public string Username { get; set; }


        public Message(string text, string userName)
        {
            this.MessageId = Guid.NewGuid();
            this.Text = text;
            this.Username = userName;
            this.Timestamp = DateTime.Now;
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            str.Append("Message: '").Append(Text).Append("'");
            str.Append(" from @").Append(Username);      
            return str.ToString();
        }
    }
}
