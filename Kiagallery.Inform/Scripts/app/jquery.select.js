(function ($) {

    $.fn.fillSelect = function (options) {

        var baseOptions = {
            url: '', // url for make request
            callback: null, // callback function when request finished
            firstItem: '...',
            firstValue: null,
            lastItem: null,
            lastValue: null,
            entity: {}
        };

        baseOptions.url = $(this).attr("action") || $(this).attr("data-url");
        baseOptions.callback = $(this).attr("data-callback");
        baseOptions.callback = $(this).attr("data-firstItem") || '...';
        baseOptions.callback = $(this).attr("data-firstValue") || null;
        baseOptions.callback = $(this).attr("data-lastItem") || null;
        baseOptions.callback = $(this).attr("data-lastValue") || null;

        var settings = $.extend(baseOptions, options);

        var element = this;
        $.ajax({
            url: baseUrl + settings.url,
            type: 'GET',
            data: settings.entity,
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (response) {

                if (response.status === 403 || response.status === 401) {
                    document.location.href = "/login?redirect=" + document.location.pathname;
                    return;
                }

                if (response.status === 200) {
                    $(element).empty();

                    if (settings.firstItem) {
                        $(element).append("<option value=\"" + (settings.firstValue ? settings.firstValue : "")  + "\">" + settings.firstItem + "</option>");
                    }

                    var data = response.data;
                    for (var i = 0; i < data.length; i++) {
                        var item = data[i];
                        $(element).append("<option value=\"" + item["id"] + "\">" + item["title"] + "</option>")
                    }

                    if (settings.lastItem) {
                        $(element).append("<option value=\"" + (settings.lastValue ? settings.lastValue : "") + "\">" + settings.lastItem + "</option>");
                    }

                }

                if (settings.callback) {
                    if (typeof settings.callback === "string") {
                        window[settings.callback](response);
                    }
                    else {
                        settings.callback(response);
                    }
                }
            }
        }).fail(function (jXHR) {
            if (jXHR.status === 401 || jXHR.status === 403) {
                document.location.href = "/login?redirect=" + document.location.pathname;
            }
            else {
                alert("در هنگام ارتباط با سرور خطایی رخ داد.");
            }
        });

        return this;
    };



}(jQuery));