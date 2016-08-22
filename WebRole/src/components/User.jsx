var React = require('react');
var ReactDOM = require('react-dom');
var routie = require('../lib/routie');

var Timeline = require('./Timeline.jsx');
var NewMessage = require('./NewMessage.jsx');
var FollowerList = require('./FollowerList.jsx');
var NewFollower = require('./NewFollower.jsx');


module.exports = React.createClass({

    render: function () {
        return <div className="container-fluid">
           <div className="row">
               <div className="col-sm-9">
                    <Timeline username={this.props.username} />
                    <NewMessage username={this.props.username} />
               </div>
                <div className="col-sm-3">
                    <FollowerList username={this.props.username} />
                    <NewFollower username={this.props.username} />
                </div>
            </div>
           
        </div>
    }
});