function SesgSignalrClient(signalrServerUrl, messageRecievedCallback) {

    var signalrClient = this;
    signalrClient.signalrServerUrl = signalrServerUrl;
    var signalrHubsUrl = apiUrl + '/signalr/hubs'
    signalrClient.messageRecievedCallback = messageRecievedCallback;
    signalrClient.ready = false;

    signalrClient.send = function (user, message) {
        signalrClient.chat.server.send(user, message);
    };
    
    signalrClient.initialize = function () {
        // Declare a proxy to reference the hub. 
        signalrClient.chat = $.connection.chatHub;
        // Create a function that the hub can call to broadcast messages.
        signalrClient.chat.client.broadcastMessage = this.messageRecievedCallback;

        // Start the connection.
        $.connection.hub.url = signalrHubsUrl;
        $.connection.hub
            .error(function (data) {
            }).start({ jsonp: true }).done(function () {
                signalrClient.ready = true;
            });
    }

    return signalrClient;
}