var alertTemplate =
    '<div id="alert-modal" uk-modal>' +
    '   <div class="uk-modal-dialog">' +
    '        <div class="uk-modal-header"><h4 class="uk-modal-title">پیام سیستم</h4></div>' +
    '        <div class="uk-modal-body"><p id="alert-message-text"></p></div>' +
    '        <p class="uk-text-left uk-modal-footer">' +
    '            <button id="alert-ok-button" class="uk-button uk-button-primary" type="button">باشه</button>' +
    '        </p>' +
    '   </div>' +
    '</div>';

var loaderTemplate =
    '<div id="dialog-loader" class="box-loader">' +
    '<div class="txt-al-c">' +
    '<div class="dialog-loader-content">' +
    '<div uk-spinner></div>' +
    '</div>' +
    '</div>' +
    '</div>';

var confirmTemplate =
    '<div id="dialog-confirm" uk-modal>' +
    '   <div class="uk-modal-dialog">' +
    '        <div class="uk-modal-header"><h4 class="uk-modal-title">پیام سیستم</h4></div>' +
    '        <div class="uk-modal-body"><p id="dialog-confirm-message"></p></div>' +
    '        <p class="uk-text-left uk-modal-footer">' +
    '            <button id="confirm-cancel-button" class="uk-button" type="button">خیر</button>' +
    '            <button id="confirm-ok-button" class="uk-button uk-button-primary" type="button">بله</button>' +
    '        </p>' +
    '   </div>' +
    '</div>';

window.alert = function (message) {
    callbackAlert(message);
};

function callbackAlert(message, callback) {
    if (!$("#dialog-alert").length) {
        $("body").append(alertTemplate);
    }

    $("#alert-message-text").html(message);
    UIkit.modal($("#alert-modal"), { escClose: false, bgClose: false }).show();

    $("#alert-ok-button").on("click", null).off("click").on("click", function () {
        UIkit.modal($("#alert-modal")).hide();
        if (callback)
            callback();
    });
}

function confirmMessage(message, callback) {
    if (!$("#dialog-confirm").length) {
        $("body").append(confirmTemplate);
    }

    $("#dialog-confirm-message").html(message);
    $("#confirm-ok-button").on("click", null).off("click").on("click", function () {
        UIkit.modal($("#dialog-confirm")).hide();
        if (callback)
            callback();
    });

    $("#confirm-cancel-button").on("click", function () {
        UIkit.modal($("#dialog-confirm")).hide();
    });

    UIkit.modal($("#dialog-confirm"), { escClose: false, bgClose: false }).show();
}

var loaderCount = 0;
function loader(status) {
    if (!$("#dialog-loader").length) {
        $("body").append(loaderTemplate);
    }

    if (status) {
        UIkit.modal($("#dialog-loader"), { escClose: false, bgClose: false }).show();
        loaderCount++;
    }
    else {
        if (loaderCount <= 1)
            UIkit.modal($("#dialog-loader")).hide();
        if (loaderCount > 0)
            loaderCount--;
    }
}

function boxLoader(content, status) {
    if ($(content).find('.box-loader').length === 0) {
        $(content).append(loaderTemplate);
    }

    if (status) {
        $(content).find('.box-loader').removeClass("uk-hidden");
    }
    else {
        $(content).find('.box-loader').addClass("uk-hidden");
    }
}

function elementLoader(element, status) {
    var html = "";
    if (status) {
        html = $(element).html();
        $(element).attr("data-html", html);
        $(element).html("<div uk-spinner></div>");
        $(element).addClass("loader");
    }
    else {
        html = $(element).attr("data-html");
        $(element).html(html);
        $(element).removeClass("loader");
    }
}
