var React = require('react');
var ReactDOM = require('react-dom');
var routie = require('../lib/routie');

module.exports = React.createClass({


    onSubmit: function (e) {
        e.preventDefault();
        var username = ReactDOM.findDOMNode(this.refs.usernameInput).value;
        console.log("going to " + "/user/" + username);
        routie('/user/' + username);
    },

    render: function () {
        return <form onSubmit={this.onSubmit}>
            <h2>Login</h2>
            <div className="form-group">
                <label htmlFor="username">Username</label>
                <input type="text" className="form-control" ref="usernameInput" placeholder="Username"/>
            </div>
            <button type="submit" className="btn btn-default">Login</button>
        </form>
    }
});