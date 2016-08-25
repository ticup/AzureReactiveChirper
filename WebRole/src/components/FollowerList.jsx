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
        return {Followers: []};
    },

    onFollowers: function (followers) {
        if (followers.Username == this.props.username) {
            this.setState(followers);
        }
    },

    unfollow: function (follower, e) {
        e.preventDefault();
        events.emit('Unfollow', this.props.username, follower);
    },

    componentDidMount: function () {
        events.emit('FollowerSubscribe', this.props.username);
        events.on('FollowerResult', this.onFollowers);
    },

    componentWillUnmount: function () {
        events.emit('FollowerUnsubscribe', this.props.username);
        events.removeListener('FollowerResult', this.onFollowers);
    },


    render: function () {
        var followers = this.state.Followers.map((followerName) =>
            <li className="list-group-item" key={followerName}>
                {followerName}
                <a className="pull-right" href="#" onClick={this.unfollow.bind(this, followerName)}><span className="glyphicon glyphicon-remove" ></span></a>
            </li>);

        return (
            <div>
                <h2>Subscriptions</h2>
                <ul className="list-group">{followers}</ul>
           </div>
        );
           
    }
});