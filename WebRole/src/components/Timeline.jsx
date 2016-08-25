var React = require('react');
var ReactDOM = require('react-dom');
var routie = require('../lib/routie');
var events = require('../lib/events');
var $ = require('jquery');

function padZero(nr) {
    if (nr < 10) {
        return "0" + nr;
    }
    else return nr;
}

function formatDate(dateString) {
    var date = new Date(dateString);
    return padZero(date.getHours()) + ":" + padZero(date.getMinutes());
}

module.exports = React.createClass({

    getInitialState: function () {
        return {Posts: []};
    },

    onTimeline: function (timeline) {
        if (this.props.username == timeline.Username) {
            this.setState(timeline);
        }
    },

    removeMessage: function (messageId, e) {
        e.preventDefault();
        console.log('emitting remove');
        events.emit('RemoveMessage', this.props.username, messageId);
    },

    componentDidMount: function () {
        events.emit('TimelineSubscribe', this.props.username);
        events.on('TimelineResult', this.onTimeline);
    },

    componentWillUnmount: function () {
        events.emit('TimelineUnsubscribe', this.props.username);
        events.removeListener('TimelineResult', this.onTimeline);
    },

    render: function () {
        var posts = this.state.Posts.map((post) => {
            var remove;
            if (post.Username == this.props.username) {
               remove = <a href="#" className="pull-right" onClick={(e)=>this.removeMessage(post.MessageId, e)}><span className="glyphicon glyphicon-remove"></span></a>
            }
            return (<li className="list-group-item" key={post.MessageId }>
                    <span>[{formatDate(post.Timestamp)}] {post.Username} : {post.Text}</span>
                    {remove}
                </li>);
        });

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