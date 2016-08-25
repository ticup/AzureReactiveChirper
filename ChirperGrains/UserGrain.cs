using System.Threading.Tasks;
using Orleans;
using GrainInterfaces;
using System.Collections.Generic;
using System;
using Orleans.Providers;
using System.Linq;
using ChirperGrainInterfaces;

namespace Grains
{

    public class UserGrainState
    {
        public HashSet<string> Subscriptions { get; set; }
        public int ChunkNumber { get; set; }
       
    }


    [StorageProvider(ProviderName = "MemoryStore")]
    public class UserGrain : Grain<UserGrainState>, IUserGrain
    {
        IMessageChunkGrain CurrentChunk;
        public string Name { get; set; }


        /// <summary>
        /// Add given user to the subscription list
        /// </summary>
        public Task Follow(string userName)
        {
            State.Subscriptions.Add(userName);
            return WriteStateAsync();
        }


        /// <summary>
        /// Remove given user from the subscription list
        /// </summary>
        public Task Unfollow(string userName)
        {
            State.Subscriptions.Remove(userName);
            return WriteStateAsync();
        }


        /// <summary>
        /// Get the Users this user is subscribed to
        /// </summary>
        public Task<FollowerList> GetFollowersList()
        {
            return Task.FromResult(new FollowerList(Name, State.Subscriptions.ToList()));
        }


        /// <summary>
        ///  Get range amount of messages posted by this user
        /// </summary>
        public async Task<List<Message>> GetMessages(int range)
        {
            var NoChunks = Math.Ceiling((double)range / MessageChunkGrain.MessageChunkSize);
            var Tasks = new List<Task<List<Message>>>();
            for (int i = 0; i <= State.ChunkNumber; i++) {
                if (i > NoChunks) break;
                var chunkGrain = GrainFactory.GetGrain<IMessageChunkGrain>(Name + "." + i);
                Tasks.Add(chunkGrain.GetMessages());
            }
            List<Message> Msgs = (await Task.WhenAll(Tasks)).SelectMany(x => x).ToList();
            return Msgs;
        }


        /// <summary>
        /// Post given text as a message
        /// </summary>
        public async Task<bool> PostText(string text)
        {
            //  add the message to the current chunk
            Message msg = new Message(text, Name);
            bool succeed = await CurrentChunk.AddMessage(msg);
            if (succeed) return true;

            // chunk was full, add new one and try again (only once, not recursive).
            await AddChunk();
            return await CurrentChunk.AddMessage(msg);
        }

        /// <summary>
        ///  Remove message with given MessageId
        /// </summary>
        public async Task<bool> RemoveMessage(Guid MessageId)
        {
            for (int i = 0; i <= State.ChunkNumber; i++)
            {
                var chunkGrain = GrainFactory.GetGrain<IMessageChunkGrain>(Name + "." + i);
                var removed = await chunkGrain.RemoveMessage(MessageId);
                if (removed) return true;
            }
            return false;
        }


        /// <summary>
        /// Get messages from all the user its subscriptions,
        /// combine them with its own messages,
        /// sort by timestamp and return the first limit number.
        /// </summary>
        public async Task<Timeline> GetTimeline(int limit)
        {
            // i) request a number of messages from each subscription
            var Tasks = State.Subscriptions.Select(UserName =>
            {
                IUserGrain user = GrainFactory.GetGrain<IUserGrain>(UserName);
                return user.GetMessages(limit);
            }).ToList();

            // ii) request your own messages
            Tasks.Add(GetMessages(limit));

            // iii) await for all the messages and flatten them
            List<Message> Msgs = (await Task.WhenAll(Tasks)).SelectMany((x) => x).ToList();

            // iv) order by Timestamp and take the first <limit> messages
            return new Timeline(Name, Msgs.OrderBy((m) => m.Timestamp).Take(limit).ToList());
        }


        /// <summary>
        /// "Create" a new chunk, by increment the ChunkNumber
        /// </summary>
        /// <returns></returns>
        private Task AddChunk()
        {
            State.ChunkNumber++;
            SetCurrentChunk();
            return WriteStateAsync();
        }


        /// <summary>
        ///  Sets the CurrentChunk to the grain instance that belongs to the current ChunkNumber
        /// </summary>
        private void SetCurrentChunk()
        {
            CurrentChunk = GrainFactory.GetGrain<IMessageChunkGrain>(Name + "." + State.ChunkNumber);
        }


        public override Task OnActivateAsync()
        {
            // First time activating this grain instance
            if (State.Subscriptions == null)
            {
                State.Subscriptions = new HashSet<string>();
            }

            Name = this.GetPrimaryKeyString();
            SetCurrentChunk();
            return TaskDone.Done;
        }

       
    }
}
