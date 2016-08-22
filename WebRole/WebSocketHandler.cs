using Newtonsoft.Json;
using Orleans.Providers;
using System;
using System.Threading;
using System.Threading.Tasks;
using Fleck;
using GrainInterfaces;
using Orleans.Runtime;
using Orleans.Runtime.Host;
using System.Collections.Generic;

namespace Orleans.Azure.Samples.ReactiveChirper
{
    public class WebSocketHandler
    {

        bool ClientConnected = false;
        HashSet<string> TimelineSubscriptions = new HashSet<string>();
        HashSet<string> FollowerSubscriptions = new HashSet<string>();

        TaskScheduler Scheduler;
        IWebSocketConnection Socket;
        Logger Logger
        {
            get
            {
                return GrainClient.Logger.GetLogger("Reactive Chirper");
            }
        }


        public WebSocketHandler(IWebSocketConnection socket, TaskScheduler scheduler)
        {
            Socket = socket;
            Scheduler = scheduler;
        }


        public void OnMessage(string jsonstring)
        {
            try
            {
                dynamic json = JsonConvert.DeserializeObject(jsonstring);
                string type = json.Type;
                switch (type)
                {
                    case "TimelineSubscribe":
                        TimelineSubscribe((string)json.Username);
                        break;

                    case "FollowerSubscribe":
                        FollowerSubscribe((string)json.Username);
                        break;

                    case "Follow":
                        Follow((string)json.Username, (string)json.ToFollow);
                        break;

                    case "NewMessage":
                        NewMessage((string)json.Username, (string)json.Text);
                        break;

                    default:
                        Send(new { Type = "Exception", Text = "unknown request" });
                        break;
                }
            } catch (Exception e)
            {
                Send(new { Type = "Exception", Text = e.ToString() });
            }
        }


        /* Timeline API */
        public void TimelineSubscribe(string username)
        {
            Dispatch(async () =>
            {
                Logger.Verbose("Subscribing to timeline of {0}", username);
                TimelineSubscriptions.Add(username);

                var grain = GrainClient.GrainFactory.GetGrain<IUserGrain>(username);
                var Rc = GrainClient.GrainFactory.StartReactiveComputation(() =>
                    grain.GetTimeline(100));

                var It = Rc.GetResultEnumerator();

                while (ClientConnected && TimelineSubscriptions.Contains(username))
                {
                    var result = await It.NextResultAsync();
                    Logger.Verbose("Pushing new timeline result of {0}: {1}", username, result);
                    Send(new { Type = "TimelineResult", Timeline = result });
                }

                Rc.Dispose();
            });
        }

        void TimelineUnsubscribe(string username)
        {
            TimelineSubscriptions.Remove(username);
        }


        /* 
         * Follower API
         */
        public void FollowerSubscribe(string username)
        {
            //FollowerSubscriptions.Add(username);
            Dispatch(async () =>
            {
                Logger.Verbose("Subscribing to followers of {0}", username);
                FollowerSubscriptions.Add(username);

                var grain = GrainClient.GrainFactory.GetGrain<IUserGrain>(username);
                var Rc = GrainClient.GrainFactory.StartReactiveComputation(() =>
                    grain.GetFollowersList());


                var It = Rc.GetResultEnumerator();

                while (ClientConnected && FollowerSubscriptions.Contains(username))
                {
                    var result = await It.NextResultAsync();
                    Logger.Verbose("Pushing new followers of {0}: {1}", username, result);

                    Send(new { Type = "FollowerResult", Followers = result });
                }

                Rc.Dispose();
            });
        }

        void FollowerUnsubscribe(string username)
        {
            FollowerSubscriptions.Remove(username);
        }

        public void Follow(string username, string toFollow)
        {
            Dispatch(async () =>
            {
                Logger.Verbose("Adding {0} following {1}", username, toFollow);
                var grain = GrainClient.GrainFactory.GetGrain<IUserGrain>(username);
                await grain.Follow(toFollow);
            });
        }

        public void NewMessage(string username, string text)
        {
            Dispatch(async () =>
            {
                Logger.Verbose("Adding message of {0} : {1}", username, text);
                var grain = GrainClient.GrainFactory.GetGrain<IUserGrain>(username);
                var result = await grain.PostText(text);
            });
        }




        /* 
         * Helper Functions
         */

        Task Dispatch(Func<Task> func)
        {
            if (!AzureClient.IsInitialized)
            {
                var config = AzureClient.DefaultConfiguration();
                AzureClient.Initialize(config);
            }

            return Task.Factory.StartNew(() => {
                Task task = TaskDone.Done;
                try
                {
                    task = func();
                }
                catch (Exception e)
                {
                    Send(e);
                }
                return task;
            }, CancellationToken.None, TaskCreationOptions.None, scheduler: Scheduler).Result;
        }

        public void Send(object message)
        {
            var str = JsonConvert.SerializeObject(message);
            Console.WriteLine("Sending : " + str);
            Socket.Send(str);
        }


        /*
         * Socket Lifetime
         */

        public void OnOpen()
        {
            ClientConnected = true;
            Console.WriteLine("Socket was opened!");
        }

        public void OnClose()
        {
            ClientConnected = false;
            Console.WriteLine("Socket was closed!");
        }

        //public bool Authenticate(IOwinRequest request)
        //{
        //    return true;
        //}
    }

}