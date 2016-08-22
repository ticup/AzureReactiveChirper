using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.WindowsAzure.ServiceRuntime;
using System.Threading.Tasks;
using Fleck;
using System.Threading;
using Orleans.Runtime.Host;
using System.IO;
using Orleans.Runtime;

namespace Orleans.Azure.Samples.ReactiveChirper
{
    public class WebRole : RoleEntryPoint
    {
        public override bool OnStart()
        {
            Trace.WriteLine("OrleansAzureWeb-OnStart");

            // For information on handling configuration changes see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.
            RoleEnvironment.Changing += RoleEnvironmentChanging;

            Task.Factory.StartNew(StartWebSocketServer, TaskCreationOptions.LongRunning);
            bool ok = base.OnStart();
            //StartWebSocketServer();


            Trace.WriteLine("OrleansAzureWeb-OnStart completed with OK=" + ok);

            return ok;
        }

        public override void OnStop()
        {
            Trace.WriteLine("OrleansAzureWeb-OnStop");
            base.OnStop();
        }

        public override void Run()
        {
            Trace.WriteLine("OrleansAzureWeb-Run");
            try
            {
                base.Run();
            }
            catch (Exception exc)
            {
                Trace.WriteLine("Run() failed with " + exc.ToString());
            }
        }

        private void RoleEnvironmentChanging(object sender, RoleEnvironmentChangingEventArgs e)
        {
            // If a configuration setting is changing
            if (e.Changes.Any(change => change is RoleEnvironmentConfigurationSettingChange))
            {
                // Set e.Cancel to true to restart this role instance
                e.Cancel = true;
            }
        }

        private void StartWebSocketServer()
        {
            // Setup the websocket server
            var websocketserver = new WebSocketServer("ws://127.0.0.1:8181");
            var scheduler = TaskScheduler.Current;

            // Start the server and delegate messages to a handler per connection
            websocketserver.Start(socket =>
            {
                var wscontroller = new WebSocketHandler(socket, scheduler);
                socket.OnOpen = wscontroller.OnOpen;
                socket.OnClose = wscontroller.OnClose;
                socket.OnMessage = wscontroller.OnMessage;
            });
        }
    }
}
