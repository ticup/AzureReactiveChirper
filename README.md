# Azure Reactive Chirper Sample #

An Azure-hosted version of a web-based twitter-like application, using the Reactive Computations.

People can "login" by entering their name, which brings them to their timeline feed and their followers list.
They can post messages and follow other people. The timeline consists of the first 100 messages of the people they follow, ordered by timestamp.
The key idea feature is that the timeline and the follower list are reactively/automatically updated whenever its value changes in the system.


A demo of this app is running on Azure [here](https://github.com/ticup/AzureReactiveChirper) (It can take a while before it loads if no one has visited it in a while)

## Running ##
Runs an Orleans silo in an Azure worker role, and an Azure web role acting as a client talking to the grains in the silo.
The sample is configured to run inside of the Azure Compute Emulator on your desktop by default.

More info about the hello world azure sample, from which this sample is derived is available here:
http://dotnet.github.io/orleans/Samples-Overview/Azure-Web-Sample


## Building ##
The web client is written using [react](https://facebook.github.io/react/) and browserify, which require compilation.
If you change the client, run one of the following commands from the **WebRole/src** directory

Unix:

    make

Windows:

    Build.cmd

any platform:

    node_modules\browserify\bin\cmd.js --outfile ../Scripts/chirper.min.js -t [babelify --presets [es2015 react] ]

## Structure ##
The core of this application looks as follows

1) The server has a [Fleck](https://www.nuget.org/packages/Fleck/) WebSocket server that dispatches incoming clients messages. Whenever the client wants to see its timeline (the fist 100 posts), the server executes the following code for that client:

````c#
public void TimelineSubscribe(string username)
        {
                TimelineSubscriptions.Add(username);

                var grain = GrainClient.GrainFactory.GetGrain<IUserGrain>(username);
                var Rc = GrainClient.GrainFactory.StartReactiveComputation(() =>
                    grain.GetTimeline(100));

                var It = Rc.GetResultEnumerator();

                while (ClientConnected && TimelineSubscriptions.Contains(username))
                {
                    var result = await It.NextResultAsync();
                    Send(new { Type = "TimelineResult", Timeline = result });
                }
            });
        }
`````

When the client is no longer interested its as simple as executing the following for that client:

````c#
 void FollowerUnsubscribe(string username)
        {
            FollowerSubscriptions.Remove(username);
        }
`````

When somebody posts a message this is as simple as:
````c#
public void NewMessage(string username, string text)
        {
                var grain = GrainClient.GrainFactory.GetGrain<IUserGrain>(username);
                var result = await grain.PostText(text);
        }
````

The timelines that now need to include this message will be automatically pushed the new timeline by the reactive computation in ``TimelineSubscribe``!



Whenever the client receives a new follower list or timeline pushed by the server, it only has to set this new state in the react component.
React will then recalculate the html and do the necessary changes with resepct to the current view.
````javascript
module.exports = React.createClass({

    getInitialState: function () {
        return {Posts: []};
    },

    onTimeline: function (timeline) {
        this.setState(timeline);
    },

    componentDidMount: function () {
        events.emit('TimelineSubscribe', this.props.username);
        events.on('TimelineResult', this.onTimeline);
    },

    componentWillUnmount: function () {
        events.emit('TimelineUnsubscribe');
        events.removeListener('TimelineResult', this.onTimeline);
    },

    render: function () {
        var posts = this.state.Posts.map((post) =>
            <li className="list-group-item" key={post.MessageId}>
                [{formatDate(post.Timestamp)}] {post.Username} : {post.Text}
            </li>);

        return (
            <div>
                <h2>
                    Your Timeline
                </h2>
                <ul className="list-group">
                    {posts}
                </ul>
            </div>
        );
    }
});
````

## Acknowledgements ##
This sample was put together using the [Azure Sample](http://dotnet.github.io/orleans/Samples-Overview/Azure-Web-Sample) app and the [Dashboard](https://github.com/OrleansContrib/OrleansDashboard) contrib app as a guideline.
