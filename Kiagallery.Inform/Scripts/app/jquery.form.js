(function ($) {

    $.fn.makeRequest = function (options) {

        var baseOptions = {
            url: '', // url for make request
            method: '', // method for request
            callback: null, // callback function when request finished
            error: null, // error callback function when request has error
            success: null, // success callback function when request compelete and success and server returend status code 200
            validate: true, // validate form before save
            loader: 2 // 0.None, 1.ElementLoader, 2.BoxLoder, 3.FullscreenLoader
        };

        baseOptions.url = $(this).attr("action") || $(this).attr("data-save");
        baseOptions.method = $(this).attr("method") || $(this).attr("data-method") || 'POST';
        baseOptions.success = $(this).attr("data-success");
        baseOptions.callback = $(this).attr("data-callback");
        baseOptions.error = $(this).attr("data-error");
        baseOptions.loader = parseInt($(this).attr("loader")) || 0;

        var settings = $.extend(baseOptions, options);
        if (!settings.validate || $(this).validate()) {
            var element = this;

            if (settings.loader === 1) {
                var button = $(element).find("[type=submit]");
                elementLoader(button, true);
                $(element).find('input, textarea, button, select').prop('disabled', true);
            }
            else if (settings.loader === 2) boxLoader(element, true);
            else if (settings.loader === 3) loader(true);

            var params = $(element).getEntity();

            if ($(element).find("[type=file]").length > 0 && $(element).find("[type=file]")[0].files.length > 0) {

                var fileInput = $(element).find("[type=file]")[0];

                var filesList = $(fileInput).prop("files");
                $(fileInput).fileupload({
                    dataType: 'json',
                    url: baseUrl + settings.url,
                    formData: params,
                    autoUpload: false,
                    start: function () {
                        $("#progressBar").removeClass("hide");
                    },
                    success: function (response, textStatus, jqXHR) {
                        if (settings.loader === 1) {
                            var button = $(element).find("[type=submit]");
                            elementLoader(button, false);
                            $(element).find('input, textarea, button, select').prop('disabled', false);
                        }
                        else if (settings.loader === 2) boxLoader(element, true);
                        else if (settings.loader === 3) loader(true);

                        if (response.status === 403 || response.status === 401) {
                            document.location.href = "/login?redirect=" + document.location.pathname;
                            return;
                        }
                        else if (!settings.callback && response.status === 200) {
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
                                alert(response.message);
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
                    error: function (jqXHR, textStatus, errorThrown) {
                        $(fileInput).fileupload("destroy");
                        $("#progressBar").removeClass("hide");
                    },
                    complete: function (result, textStatus, jqXHR) {
                        $(fileInput).fileupload("destroy");
                        $("#progressBar").addClass("hide");
                    },
                    progressall: function (e, data) {
                        var progress = parseInt(data.loaded / data.total * 100, 10);
                        $('#progressBar').val(progress);
                    }
                });

                $(fileInput).fileupload('send', { files: filesList });
            }
            else {
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
                        else if (settings.loader === 2) boxLoader(element, true);
                        else if (settings.loader === 3) loader(true);

                        if (response.status === 403 || response.status === 401) {
                            document.location.href = "/login?redirect=" + document.location.pathname;
                            return;
                        }
                        else if (!settings.callback && response.status === 200) {
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
                                alert(response.message);
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
                        else if (settings.loader === 2) boxLoader(element, false);
                        else if (settings.loader === 3) loader(false);
                    }
                }).fail(function (jXHR) {
                    if (jXHR.status === 401 || jXHR.status === 403) {
                        document.location.href = "/login?redirect=" + document.location.pathname;
                    }
                    else {
                        alert("در هنگام ارتباط با سرور خطایی رخ داد.");
                    }
                });
            }
        }

        return this;
    };

    $.fn.loadForm = function (options) {

        var baseOptions = {
            url: '', // url for make request
            method: '', // method for request
            callback: null, // callback function when request finished
            success: null, // success callback function when request compelete and success and server returend status code 200
            error: null, // error callback function when request has error
            loader: 2, // 0.None, 1.ElementLoader, 2.BoxLoder, 3.FullscreenLoader
            params: {}
        };

        baseOptions.url = $(this).attr("data-load");
        baseOptions.method = $(this).attr("data-load-method") || 'GET';
        baseOptions.callback = $(this).attr("data-load-callback");
        baseOptions.success = $(this).attr("data-load-success");
        baseOptions.error = $(this).attr("data-load-error");
        baseOptions.loader = parseInt($(this).attr("loader")) || 0;

        var settings = $.extend(baseOptions, options);

        var element = this;

        var id = $(this).find("[name=id]").val();
        if (!id) return;

        if (settings.loader === 1) {
            var button = $(element).find("[type=submit]");
            elementLoader(button, true);
            $(element).find('input, textarea, button, select').prop('disabled', true);
        }
        else if (settings.loader === 2) boxLoader(element, true);
        else if (settings.loader === 3) loader(true);

        $.ajax({
            url: baseUrl + settings.url + "/" + id,
            type: settings.method,
            data: settings.method.toUpperCase() === 'GET' ? settings.params : JSON.stringify(settings.params),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (response) {

                if (settings.loader === 1) {
                    var button = $(element).find("[type=submit]");
                    elementLoader(button, false);
                    $(element).find('input, textarea, button, select').prop('disabled', false);
                }
                else if (settings.loader === 2) boxLoader(element, true);
                else if (settings.loader === 3) loader(true);

                if (response.status === 403 || response.status === 401) {
                    document.location.href = "/login?redirect=" + document.location.pathname;
                    return;
                }
                else if (!settings.callback && response.status === 200) {
                    $(element).setEntity(response.data);

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
                        alert(response.message);
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
                else if (settings.loader === 2) boxLoader(element, false);
                else if (settings.loader === 3) loader(false);
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