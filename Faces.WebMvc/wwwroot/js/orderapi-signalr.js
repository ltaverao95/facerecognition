"use strict";

var timerId;
const connection = new signalR.HubConnectionBuilder()
    .withUrl('/orderhub')
    .configureLogging(signalR.LogLevel.Information)
    .withAutomaticReconnect()
    .build();

connection.on("UpdateOrders", (message, orderId) => {
    const encodedMsg = message + ":" + orderId;
    console.log(encodedMsg);

    if (!orderId &&
        !orderId.Length) {
        return;
    }

    toastr.success(orderId + 'Updated to status' + message);
    refreshPage();
});

function refreshPage() {
    clearTimeout(timerId);
    timerId = setTimeout(function () {
        window.location.reload();
    }, 3000);
}

connection.start().catch(err => console.error(err.toString()));