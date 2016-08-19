module.exports = {
    socket: null,

    connect: function connect(host, callback) {
        try {
            this.socket = new WebSocket(host);

            this.socket.onopen = function (openEvent) {
                console.log("Connected to websocket server " + host);
            };

            this.socket.onmessage = function (messageEvent) {
                var message = messageEvent.data;
                var json = JSON.parse(message);
                console.log("Received message " + message);
                callback(json);
            };

            this.socket.onerror = function (errorEvent) {
                console.log("Received websocket error: ");
                console.log(errorEvent);
            };

            this.socket.onclose = function (closeEvent) {
                console.log("WebSocket closed: ");
                console.log(closeEvent);
            };
        }
        catch (exception) { if (window.console) console.log(exception); }
    },

    send: function sendMessage(json) {
        if (this.socket.readyState != WebSocket.OPEN) return;
        var str = JSON.stringify(json);
        console.log("sending message " + str);
        this.socket.send(str);
    }
};