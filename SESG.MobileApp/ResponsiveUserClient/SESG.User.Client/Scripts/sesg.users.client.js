var sesg_user_client = (function ($) {

    // a private variable to hold settings
    var settings = {
        name: 'Fred',
        greeting: 'Hello',
        apiUrl: 'http://jerome7/SESG.UserWebService'
    };

    // a private function, used only within the IIFE
    function buildGreeting() {
        return settings.greeting + ', ' + settings.name + '!';
    }

    function getUsersUrl() {
        return settings.apiUrl + '/api/Users'
    }

    // an object with the module's public interface
    return {

        sayHello: function () {
            alert(buildGreeting());
        },

        getUsers: function(sender, offset, limit, callback, errorCallback) {
            var url = getUsersUrl();
            var data = { offset:offset, limit:limit };
            $.ajax({
                url: url,
                data: data,
                dataType: 'json',
                success: function (data) {
                    if (typeof(callback) == "function") {
                        callback(sender, data);
                    }
                },
                error: function (xhr, status, error) {
                    if (typeof (errorCallback) == "function") {
                        errorCallback(xhr, status, error);
                    }
                }
            });
        }

    }
})(jQuery);