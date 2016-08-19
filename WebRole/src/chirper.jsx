var $ = require('jquery');
var React = require('react');
var ReactDOM = require('react-dom');
var routie = require('./lib/routie');
var events = require('./lib/events');

var WebSocketClient = require('./WebSocketClient.js');

var Login = require('./components/Login.jsx');
var User = require('./components/User.jsx');
var ThemeButtons = require('./components/theme-buttons.jsx');

var DomContainer = document.getElementById('content');

ReactDOM.render(<ThemeButtons/>, document.getElementById('button-toggles-content'));

// TODO: username <-> userName

/* UI Routing */
// Login
routie('', function () {
    console.log("loading login page");
    ReactDOM.render(<Login />, DomContainer);
});

// User page (timeline/followers)
routie('/user/:username', function (username) {
    console.log("arrived at user page for " + username);
    ReactDOM.render(<User userName={username} />, DomContainer);
});

/* Event Handling - Server Interaction */
var host = "ws://localhost:8181";
var client = WebSocketClient.connect(host, function (json) {
    switch (json.Type) {
        case 'TimelineResult':
            events.emit('TimelineResult', json.Timeline);
            break;

        case 'FollowerResult':
            events.emit('FollowerResult', json.Followers);
            break;
        default:
            console.log("Unknown server message ");
            console.log(json);
            break;
    }
});
var send = WebSocketClient.send.bind(WebSocketClient);

events.on('TimelineSubscribe', (username) =>
    send({Type: 'TimelineSubscribe', Username: username}));

events.on('TimelineUnsubscribe', (username) =>
    send({Type: 'TimelineUnsubscribe', Username: username }));

events.on('FollowerSubscribe', (username) =>
    send({Type: 'FollowerSubscribe', Username: username }));

events.on('FollowerUnsubscribe', (username) =>
    send({Type: 'FollowerUnsubscribe', Username: username }));


events.on('Follow', (username, toFollow) => {
    console.log("following");
    console.log({ username, toFollow });
    send({ Type: 'Follow', Username: username, ToFollow: toFollow });
});

events.on('NewMessage', (username, text) => {
    console.log("sending message");
    send({ Type: 'NewMessage', Username: username, Text: text });
});

// Go to login page
routie('');