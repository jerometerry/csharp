var sesg_user_client = (function ($) {

    // a private variable to hold settings
    var settings = {
        name: 'Fred',
        greeting: 'Hello',
        apiUrl: 'http://localhost/SESG.UserWebService'
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

        getUsers: function(offset, limit, callback) {
            var url = getUsersUrl();
            var data = { offset:offset, limit:limit };
            $.ajax({
                url: url,
                data: data,
                dataType: 'json',
                success: function (data) {
                    callback(data);
                }
            });
        }

    }
})(jQuery);