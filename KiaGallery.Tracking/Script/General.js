$(document).ready(function () {
    $(".nav-head").on("click", function (event) {
        event.preventDefault();

        var subListClass = $(this).attr("data-child");

        $(".nav-sub-list").not("." + subListClass).slideUp(function () {
            $(".nav-sub-list").not("." + subListClass).addClass("hidden");
        });

        if ($("." + subListClass).hasClass("hidden")) {
            $("." + subListClass).removeClass("hidden").slideDown();
        }
        else {
            $("." + subListClass).slideUp(function () {
                $("." + subListClass).addClass("hidden")
            });
        }
    });

    $('[data-toggle="tooltip"]').tooltip();

    $(document).on("click", ".clear-params", function (event) {
        if (window.location.href.indexOf("#!") >= 0)
            window.location.href = window.location.href.substring(0, window.location.href.indexOf("#!") + 2);
    });

});

/* Set the width of the side navigation to 250px and the left margin of the page content to 250px and add a black background color to body */
function openNav() {
    document.getElementById("sidenav").style.width = "280px";
    //document.getElementById("main").style.marginRight = "250px";
}

/* Set the width of the side navigation to 0 and the left margin of the page content to 0, and the background color of body to white */
function closeNav() {
    document.getElementById("sidenav").style.width = "0";
    //document.getElementById("main").style.marginRight = "0";
}

$(document).ready(function () {
    $(document).on("keyup change", ".money-separator", function (e) {
        moneySeparator(this);
    });
});

jQuery.fn.tagName = function () {
    return this.prop("tagName");
};

jQuery.each(["put", "delete"], function (i, method) {
    jQuery[method] = function (url, data, callback, type) {
        if (jQuery.isFunction(data)) {
            type = type || callback;
            callback = data;
            data = undefined;
        }

        return jQuery.ajax({
            url: url,
            type: method,
            dataType: type,
            data: data,
            success: callback
        });
    };
});

function setParam(key, value) {
    var params = getParam();
    if (typeof key == "string")
        params[key] = value;
    else
        $.extend(params, key);
    

    var url = window.location.href;
    var pos = url.indexOf('#!');

    if (pos > 0)
        url = url.slice(0, pos);

    window.location.href = url + "#!" + jsonToUrlParams(params);
}

function getParam() {
    var url = window.location.href;

    var pos = url.indexOf('#!');
    if (pos < 0) return {};

    var vars = {}, hashes = url.slice(pos + 2).split('/');
    for (var i = 0; i < hashes.length; i++) {
        var decode = decodeURIComponent(hashes[i]);
        var hash = decode.split('=');
        if (hash[0])
            vars[hash[0]] = hash.length > 1 ? getValue(hash[1]) : null;
    }
    return vars;
}

function removeParam(key) {
    var params = getParam();

    if (typeof key == "string") {
        delete params[key];
    }
    else {
        for (var i = 0; i < key.length; i++) {
            var item = key[i];
            delete params[item];
        }
    }

    var url = window.location.href;
    var pos = url.indexOf('#!');

    if (pos > 0)
        url = url.slice(0, pos);

    window.location.href = url + "#!" + jsonToUrlParams(params);
}

function manipulateParam(set, remove) {
    var params = getParam();
    if (typeof set == "string")
        params[set] = value;
    else
        $.extend(params, set);

    if (typeof remove == "string") {
        delete params[remove];
    }
    else {
        for (var i = 0; i < remove.length; i++) {
            var item = remove[i];
            delete params[item];
        }
    }

    var url = window.location.href;
    var pos = url.indexOf('#!');

    if (pos > 0)
        url = url.slice(0, pos);

    window.location.href = url + "#!" + jsonToUrlParams(params);
}

function getValue(value) {
    if (value.startsWith("[") && value.endsWith("]")) {
        return value.substring(1).substr(0, value.length - 2).split("-");
    }
    else {
        return value;
    }
}

function jsonToUrlParams(data) {
    var url = Object.keys(data).map(function (element) {
        if (typeof data[element] == "object")
            return encodeURIComponent(element) + '=' + encodeURIComponent("[" + data[element].join("-") + "]");
        else
            return encodeURIComponent(element) + '=' + encodeURIComponent(data[element]);
    }).join('/');
    return url;
}

function moneySeparator(field) {
    var price;
    price = "";
    while (field.value.indexOf(",") != -1) {
        field.value = field.value.replace(",", "");
    }
    var txt_field = document.getElementById("ttd");
    txt_field = field.value.length;
    var counter = 0;
    for (var i = txt_field - 1; i >= 0; i--) {
        price = field.value.charAt(i) + price;
        counter++;
        if (counter % 3 == 0 && i > 0) {
            price = "," + price;
            counter = 0;
        }
    }
    field.value = price;
}

function toSeparator(value) {
    if (!value) return "";

    if (typeof value != "string") value = value.toString();

    var price = "";
    while (value.indexOf(",") != -1) {
        value = value.replace(",", "");
    }

    var valueLength = value.length;
    var counter = 0;
    for (var i = valueLength - 1; i >= 0; i--) {
        price = value.charAt(i) + price;
        counter++;
        if (counter % 3 == 0 && i > 0) {
            price = "," + price;
            counter = 0;
        }
    }

    return price;
}

function removeSeparator(input) {
    return input.replaceAll(",", "");
}

function removeCardSeparator(input) {
    return input.replaceAll("-", "");
}


String.prototype.replaceAll = function (search, replacement) {
    var target = this;
    return target.split(search).join(replacement);
};

var loaderTemplate =
    '<div id="dialog-loader" class="modal" style="overflow-y: hidden;">' +
    '<div class="modal-dialog">' +
    '<div id="dialog-loader-content">' +
    '<img src="/content/image/loading.gif" />' +
    '</div>' +
    '</div>' +
    '</div>';

var alertTemplate =
    '<div id="dialog-alert" class="modal" role="dialog">' +
    '<div class="modal-dialog">' +
    '<div class="modal-content">' +
    '<div class="modal-header">' +
    '<button type="button" class="close" data-dismiss="modal">&times;</button>' +
    '<h4 class="modal-title">پیام سیستم</h4>' +
    '</div>' +
    '<div class="modal-body">' +
    '<p id="dialog-alert-title"></p>' +
    '</div>' +
    '<div class="modal-footer">' +
    '<button id="alert-ok-button" class="btn btn-primary">بستن</button>' +
    '</div>' +
    '</div>' +
    '</div>' +
    '</div>';

var confirmTemplate =
    '<div id="dialog-confirm" class="modal" style="overflow-y: hidden;" role="dialog">' +
    '<div class="modal-dialog">' +
    '<div class="modal-content">' +
    '<div class="modal-header">' +
    '<button type="button" class="close" data-dismiss="modal">&times;</button>' +
    '<h4 class="modal-title">پیام سیستم</h4>' +
    '</div>' +
    '<div class="modal-body">' +
    '<p id="dialog-confirm-title"></p>' +
    '</div>' +
    '<div class="modal-footer">' +
    '<button id="confirm-cancel-button" class="btn btn-primary">خیر</button>&nbsp;&nbsp;' +
    '<button id="confirm-ok-button" class="btn btn-primary">بله</button>' +
    '</div>' +
    '</div>' +
    '</div>' +
    '</div>';


var loaderCount = 0;
function loader(status) {
    if (!$("#dialog-loader").length) {
        $("body").append(loaderTemplate);
    }

    if (status) {
        $("#dialog-loader").modal({ backdrop: 'static', keyboard: false });
        loaderCount++;
    }
    else {
        if (loaderCount <= 1)
            $("#dialog-loader").modal('hide');
        if (loaderCount > 0)
            loaderCount--;
    }
}

window.alert = function (message) {
    callbackAlert(message);
};

function callbackAlert(message, callback) {
    if (!$("#dialog-alert").length) {
        $("body").append(alertTemplate);
    }

    $("#dialog-alert-title").html(message);
    $("#dialog-alert").modal('show');

    $("#alert-ok-button").on("click", null).off("click").on("click", function () {
        $("#dialog-alert").modal('hide');
        if (callback)
            callback();
    });
}

function confirmMessage(message, callback) {
    if (!$("#dialog-confirm").length) {
        $("body").append(confirmTemplate);
    }

    $("#dialog-confirm-title").html(message);
    $("#dialog-confirm").modal({ backdrop: 'static', keyboard: false });

    $("#confirm-ok-button").on("click", null).off("click").on("click", function () {
        $("#dialog-confirm").modal('hide');
        if (callback)
            callback();
    });
    $("#confirm-cancel-button").on("click", function () {
        $("#dialog-confirm").modal('hide');
    });
}

function loadRemoteSelect(target, url, params, key, value, firstItemValue, callback) {
    firstItemValue = firstItemValue ? firstItemValue : "...";
    $(target).empty();
    $(target).append("<option value=\"\">" + firstItemValue + "</option>");
    loader(true);
    $.get(url, params, function (response) {
        loader(false);
        if (response.status == 200) {
            var data = response.data;
            for (var i = 0; i < data.list.length; i++) {
                var item = data.list[i];
                $(target).append("<option value=\"" + item[key] + "\">" + item[value] + "</option>")
            }
            if (callback)
                callback(response.data);
        }
        else {
            alert(response.message);
        }
    });
}

function setUpStepWizard() {
    $('.setup-content').hide();

    $('ul.setup-panel li a').click(function (e) {
        e.preventDefault();
        var $target = $($(this).attr('href')),
            $item = $(this).closest('li');

        if (!$item.hasClass('disabled')) {
            $('ul.setup-panel li a').closest('li').removeClass('active');
            $item.addClass('active');
            $('.setup-content').hide();
            $target.show();
        }
    });
    $('ul.setup-panel li.active a').trigger('click');
}

function showStep(num) {
    $('ul.setup-panel li').removeClass('active');
    $('.setup-content').hide();
    $('#headerStep' + num).addClass('active')
    $("#step-" + num).show();
}

function setupUpload(path, callback) {
    $("#deleteFile").on("click", function () {
        $('#txtFileName').val("");
        checkForm("#form");
        $('.progress .progress-bar').css('width', '0%');
    });

    $("#openFile").on("click", function () {
        $("#file").click();
    });

    $('#file').fileupload({
        dataType: 'json',
        url: "/base/upload?path=" + path,
        autoUpload: true,
        done: function (e, data) {
            var response = data.result;
            if (response.status == 200) {
                $('#txtFileName').val(response.data.name);

                if (callback)
                    callback(response.data.name);

            }
            alert(response.message);
            loader(false);
        }
    }).on('fileuploadprogressall', function (e, data) {
        var progress = parseInt(data.loaded / data.total * 100, 10);
        $('.progress .progress-bar').css('width', progress + '%');
    });
}

$(document).on("click", ".page-number", function (event) {
    event.preventDefault();
    var page = parseInt($(this).attr("data-page"));
    if (page >= 0)
        setParam("page", page);
});

/*
Simple Utility Function This will allow you to call a utility function that accepts the element you're looking for and if you want the element to be fully in view or partially.
*/

function Utils() {

}

Utils.prototype = {
    constructor: Utils,
    isElementInView: function (element, fullyInView) {
        var pageTop = $(window).scrollTop();
        var pageBottom = pageTop + $(window).height();
        var elementTop = $(element).offset().top;
        var elementBottom = elementTop + $(element).height();

        if (fullyInView === true) {
            return ((pageTop < elementTop) && (pageBottom > elementBottom));
        } else {
            return ((elementTop <= pageBottom) && (elementBottom >= pageTop));
        }
    }
};

var Utils = new Utils();


function precision(num, decimal) {
    if (!num) {
        return 0;
    }

    var decimalPoint = 1;
    for (var i = 0; i < decimal; i++) {
        decimalPoint = decimalPoint * 10;
    }

    return Math.round(num * 100) / 100;
}


(function ($) {
    if ($.fn.style) {
        return;
    }

    // Escape regex chars with \
    var escape = function (text) {
        return text.replace(/[-[\]{}()*+?.,\\^$|#\s]/g, "\\$&");
    };

    // For those who need them (< IE 9), add support for CSS functions
    var isStyleFuncSupported = !!CSSStyleDeclaration.prototype.getPropertyValue;
    if (!isStyleFuncSupported) {
        CSSStyleDeclaration.prototype.getPropertyValue = function (a) {
            return this.getAttribute(a);
        };
        CSSStyleDeclaration.prototype.setProperty = function (styleName, value, priority) {
            this.setAttribute(styleName, value);
            var priority = typeof priority != 'undefined' ? priority : '';
            if (priority != '') {
                // Add priority manually
                var rule = new RegExp(escape(styleName) + '\\s*:\\s*' + escape(value) +
                    '(\\s*;)?', 'gmi');
                this.cssText =
                    this.cssText.replace(rule, styleName + ': ' + value + ' !' + priority + ';');
            }
        };
        CSSStyleDeclaration.prototype.removeProperty = function (a) {
            return this.removeAttribute(a);
        };
        CSSStyleDeclaration.prototype.getPropertyPriority = function (styleName) {
            var rule = new RegExp(escape(styleName) + '\\s*:\\s*[^\\s]*\\s*!important(\\s*;)?',
                'gmi');
            return rule.test(this.cssText) ? 'important' : '';
        }
    }

    // The style function
    $.fn.style = function (styleName, value, priority) {
        // DOM node
        var node = this.get(0);
        // Ensure we have a DOM node
        if (typeof node == 'undefined') {
            return this;
        }
        // CSSStyleDeclaration
        var style = this.get(0).style;
        // Getter/Setter
        if (typeof styleName != 'undefined') {
            if (typeof value != 'undefined') {
                // Set style property
                priority = typeof priority != 'undefined' ? priority : '';
                style.setProperty(styleName, value, priority);
                return this;
            } else {
                // Get style property
                return style.getPropertyValue(styleName);
            }
        } else {
            // Get CSSStyleDeclaration
            return style;
        }
    };
})(jQuery);

function fillRemoteCombo(target, url, params, key, value, firstItemValue, callback) {
    firstItemValue = firstItemValue ? firstItemValue : "...";
    $(target).empty();
    $(target).append("<option value=\"\">" + firstItemValue + "</option>");
    loader(true);
    $.get(url, params, function (response) {
        loader(false);
        if (response.status == 200) {
            var data = response.data;
            for (var i = 0; i < data.length; i++) {
                var item = data[i];
                $(target).append("<option value=\"" + item[key] + "\">" + item[value] + "</option>")
            }
            if (callback)
                callback(response.data);
        }
        else {
            alert(response.message);
        }
    });
}
