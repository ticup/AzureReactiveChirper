using GrainInterfaces;
using Orleans;
using Orleans.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grains
{
    class MessageChunkGrainState
    {
        public List<Message> Msgs { get; set; }
    }

    [StorageProvider(ProviderName = "MemoryStore")]
    class MessageChunkGrain : Grain<MessageChunkGrainState>, IMessageChunkGrain
    {
        public static double MessageChunkSize = 10;



        public Task<List<Message>> getMessages()
        {
            return Task.FromResult(State.Msgs);
        }


        /// <summary>
        /// Add a message to the MessageList.
        /// </summary>
        /// <param name="message"></param>
        /// <returns>
        /// False if the message could not be added because the list was full,
        /// True otherwise.
        /// </returns>
        public async Task<bool> AddMessage(Message message)
        {
            if (State.Msgs.Count > MessageChunkSize)
            {
                return false;
            }
            State.Msgs.Add(message);
            await WriteStateAsync();
            return true;
        }



        public override Task OnActivateAsync()
        {
            if (State.Msgs == null)
            {
                State.Msgs = new List<Message>();
            }
            return TaskDone.Done;
        }
    }
}