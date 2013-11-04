function SesgUserClient(apiUrl) {
    this.apiUrl = apiUrl;
    this.usersUrl = this.apiUrl + '/api/Users';
    this.countUsersUrl = this.usersUrl + '/Count';

    this.getUsers = function (sender, offset, limit, callback, errorCallback) {
        var url = this.usersUrl;
        var data = { offset: offset, limit: limit };
        $.ajax({
            url: url,
            data: data,
            dataType: 'json',
            success: function (response) {
                if (typeof (callback) == "function") {
                    callback(sender, response);
                }
            },
            error: function (xhr, status, error) {
                if (typeof (errorCallback) == "function") {
                    errorCallback(sender, xhr, status, error);
                }
            }
        });
    };

    this.countUsers = function (sender, callback, errorCallback) {
        var url = this.countUsersUrl;
        $.ajax({
            url: url,
            dataType: 'json',
            success: function (response) {
                if (typeof (callback) == "function") {
                    callback(sender, response);
                }
            },
            error: function (xhr, status, error) {
                if (typeof (errorCallback) == "function") {
                    errorCallback(sender, xhr, status, error);
                }
            }
        });
    };

    return this;
}