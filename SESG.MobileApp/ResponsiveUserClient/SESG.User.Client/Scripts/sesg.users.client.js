function SesgUserClient(apiUrl) {
    this.apiUrl = apiUrl;
    this.usersUrl = this.apiUrl + '/api/Users';

    this.getUsers = function (sender, offset, limit, callback, errorCallback) {
        var url = this.usersUrl;
        var data = { offset: offset, limit: limit };
        $.ajax({
            url: url,
            data: data,
            dataType: 'json',
            success: function (data) {
                if (typeof (callback) == "function") {
                    callback(sender, data);
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