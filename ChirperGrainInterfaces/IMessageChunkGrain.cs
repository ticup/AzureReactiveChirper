using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrainInterfaces
{
    public interface IMessageChunkGrain : Orleans.IGrainWithStringKey
    {
        Task<List<Message>> GetMessages();

        Task<bool> AddMessage(Message message);
        Task<bool> RemoveMessage(Guid messageId);
    }
}
