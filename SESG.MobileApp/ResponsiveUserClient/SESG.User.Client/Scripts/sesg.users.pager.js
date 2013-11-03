function SesgUserPager(client, successCallback, failureCallback) {
    this.client = client;

    this.index = 0;
    this.pageSize = 10;
    this.successCallback = successCallback;
    this.failureCallback = failureCallback;

    this.next = function () {
        this.client.getUsers(this, this.index, this.pageSize, this.gotData, this.getUsersError);
    };

    this.gotData = function (sender, data) {
        sender.index += data.length;
        if (typeof (sender.successCallback) == 'function') {
            sender.successCallback(data);
        }
    };

    this.getUsersError = function (xhr, status, error) {
        if (typeof (sender.failureCallback) == 'function') {
            sender.failureCallback(xhr, status, error);
        }
    };

    return this;
}

