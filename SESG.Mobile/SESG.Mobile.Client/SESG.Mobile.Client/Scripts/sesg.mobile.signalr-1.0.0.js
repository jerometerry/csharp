function SesgSignalrClient(signalrServerUrl, messageRecievedCallback) {

    var signalrClient = this;

    // Declare a proxy to reference the hub. 
    this.chat = $.connection.chatHub;

    this.messageRecievedCallback = messageRecievedCallback;

    this.signalrServerUrl = signalrServerUrl;
    this.ready = false;

    // Create a function that the hub can call to broadcast messages.
    this.chat.client.broadcastMessage = this.messageRecievedCallback;

    this.send = function (user, message) {
        this.chat.server.send(user, message);
    };

    // Start the connection.
    $.connection.hub.url = this.signalrServerUrl + '/signalr/hubs';
    $.connection.hub
        .error(function (data) {
        }).start({ jsonp: true }).done(function () {
            signalrClient.ready = true;
        });

    return this;
}