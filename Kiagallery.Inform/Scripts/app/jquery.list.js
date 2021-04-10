(function ($) {

    var listen = false;

    $(window).on('hashchange', function () {
        if (listen) {
            $(document).find(".list-form").list();
        }
    });

    $(document).on("click", ".list-form .delete", function () {
        if (listen) {
            var url = $(this).attr("data-delete-url");

            deleteEntity(url, function (response) {
                if (response.status === 200) {
                    $(".list-form").list();
                }
                else {
                    alert(response.message);
                }
            });
        }
    });

    $.fn.list = function (options) {

        var baseOptions = {
            url: '', // url for make request
            template: '#template', // template for load data in form
            method: '', // method for request
            success: null, // success callback function when request compelete and success and server returend status code 200
            callback: null, // callback function when request finished
            error: null, // error callback function when request has error
            loader: 2, // 0.None, 1.ElementLoader, 2.BoxLoder, 3.Full Screen Loader
            search: null, // element for search sub items to make search entity
            entity: {
                page: 0, // default page
                count: 10 // default page size
            }
        };

        baseOptions.url = $(this).attr("action") || $(this).attr("data-url");
        baseOptions.template = $(this).attr("template") || $(this).attr("data-template") || '#template';
        baseOptions.method = $(this).attr("method") || $(this).attr("data-method") || 'GET';
        baseOptions.success = $(this).attr("data-success");
        baseOptions.callback = $(this).attr("data-callback");
        baseOptions.error = $(this).attr("data-error");
        baseOptions.loader = parseInt($(this).attr("loader")) || 2;
        baseOptions.search = $(this).attr("data-search") || '#search-form';
        baseOptions.entity.page = parseInt($(this).attr("page")) || 0;
        baseOptions.entity.count = parseInt($(this).attr("count")) || 10;

        var settings = $.extend(baseOptions, options);

        var element = this;
        $(element).addClass("list-form");
        listen = true;

        if (settings.loader === 1) {
            var button = $(element).find("[type=submit]");
            elementLoader(button, true);
            $(element).find('input, textarea, button, select').prop('disabled', true);
        }
        else if (settings.loader === 2) boxLoader(element, true);
        else if (settings.loader === 3) loader(true);

        var urlParam = getParam();
        $(settings.search).setEntity(urlParam);

        var params = $(baseOptions.search).getEntity();
        params = $.extend(params, urlParam);
        params = $.extend(baseOptions.entity, params);

        $.ajax({
            url: baseUrl + settings.url,
            type: settings.method,
            data: settings.method.toUpperCase() === 'GET' ? params : JSON.stringify(params),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (response) {

                if (settings.loader === 1) {
                    var button = $(element).find("[type=submit]");
                    elementLoader(button, false);
                    $(element).find('input, textarea, button, select').prop('disabled', false);
                }
                else if (settings.loader === 2) boxLoader(element, false);
                else if (settings.loader === 3) loader(false);

                if (response.status === 403 || response.status === 401) {
                    document.location.href = "/login?redirect=" + document.location.pathname;
                    return;
                }
                else if (!settings.callback && response.status === 200) {
                    var template = $.templates(settings.template);
                    var htmlOutput = template.render(response.data);
                    $(element).html(htmlOutput);

                    if (settings.success) {
                        if (typeof settings.success === "string") {
                            window[settings.success](response);
                        }
                        else {
                            settings.success(response);
                        }
                    }
                }
                else if (!settings.callback) {
                    if (settings.error) {
                        if (typeof settings.error === "string") {
                            window[settings.error](response);
                        }
                        else {
                            settings.error(response);
                        }
                    }
                    else {
                        $(element).html("<h4>" + response.message + "</h4>");
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
            },
            error: function () {
                if (settings.loader === 1) {
                    var button = $(element).find("[type=submit]");
                    elementLoader(button, false);
                    $(element).find('input, textarea, button, select').prop('disabled', false);
                }
                else if (settings.loader === 2) boxLoader(element, true);
                else if (settings.loader === 3) loader(true);
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