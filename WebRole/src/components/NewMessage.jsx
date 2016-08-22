var React = require('react');
var ReactDOM = require('react-dom');
var routie = require('../lib/routie');
var events = require('../lib/events');
var $ = require('jquery');

module.exports = React.createClass({

    onSubmit: function (e) {
        e.preventDefault();
        var msg = ReactDOM.findDOMNode(this.refs.newMessageInput).value;
        console.log("posting " + msg);
        events.emit('NewMessage', this.props.username, msg);
    },

    render: function () {
        return <form onSubmit={this.onSubmit}>
            <div className="form-group">
                <input type="text" className="form-control" ref="newMessageInput" placeholder="Message..."/>
            </div>
            <button type="submit" className="btn btn-default">Post</button>
        </form>
    }
});