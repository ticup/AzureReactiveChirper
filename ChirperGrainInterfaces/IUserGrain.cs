using System.Threading.Tasks;
using Orleans;
using System.Collections.Generic;
using System;
using ChirperGrainInterfaces;

namespace GrainInterfaces
{ 
	public interface IUserGrain : IGrainWithStringKey
    {
        Task<FollowerList> GetFollowersList();
        Task<List<Message>> GetMessages(int amount);
        Task<Timeline> GetTimeline(int amount);

        Task Follow(string userName);
        Task Unfollow(string userName);
        Task<bool> PostText(string text);
        Task<bool> RemoveMessage(Guid messageId);
    }
}