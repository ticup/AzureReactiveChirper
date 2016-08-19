using System.Threading.Tasks;
using Orleans;
using System.Collections.Generic;
using System;
namespace GrainInterfaces
{ 
	public interface IUserGrain : IGrainWithStringKey
    {
        Task<List<string>> GetFollowersList();
        Task<List<Message>> GetMessages(int amount);
        Task<Timeline> GetTimeline(int amount);

        Task Follow(string userName);
        Task<bool> PostText(string text);
    }
}