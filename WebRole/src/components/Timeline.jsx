var React = require('react');
var ReactDOM = require('react-dom');
var routie = require('../lib/routie');
var events = require('../lib/events');
var $ = require('jquery');

function formatDate(dateString) {
    var date = new Date(dateString);
    return date.getHours() + ":" + date.getMinutes();
}

module.exports = React.createClass({

    getInitialState: function () {
        return {Posts: []};
    },

    onTimeline: function (timeline) {
        this.setState(timeline);
    },

    componentDidMount: function () {
        events.emit('TimelineSubscribe', this.props.userName);
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