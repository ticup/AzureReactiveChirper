var React = require('react');
var ReactDOM = require('react-dom');
var routie = require('../lib/routie');
var $ = require('jquery');
var events = require('../lib/events');

module.exports = React.createClass({

    onSubmit: function (e) {
        e.preventDefault();
        var toFollow = ReactDOM.findDOMNode(this.refs.newFollowerInput).value;
        console.log("following " + toFollow);
        events.emit('Follow', this.props.username, toFollow);
    },

    render: function () {
        return <form onSubmit={this.onSubmit}>
            <div className="form-group">
                <input type="text" className="form-control" ref="newFollowerInput" placeholder="Name..."/>
            </div>
            <button type="submit" className="btn btn-default">Follow</button>
        </form>
    }
});