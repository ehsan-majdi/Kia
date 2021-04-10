function getEntity(form) {
    var entity = {};
    var context = form ? form : document;
    $(context).find("[name]").each(function (index, element) {
        var key = $(element).attr("name");
        var value = "";

        var tagName = $(element).tagName().toLowerCase();
        if (tagName == "input") {
            var type = $(element).attr("type");
            switch (type) {
                case "text":
                default:
                    value = $(element).val();

                    if ($(element).hasClass("money-separator"))
                        value = removeSeparator(value);

                    break;
                case "checkbox":
                    if ($(context).find("input[type=checkbox][name=" + key + "]").length > 1) {
                        value = $(context).find("input[type=checkbox][name=" + key + "]:checked").map(function () {
                            return $(this).val();
                        }).get();
                    }
                    else {
                        value = $(context).find("input[type=checkbox][name=" + key + "]").prop("checked");
                    }
                    break;
                case "radio":
                    value = $(context).find("input[type=radio][name=" + key + "]:checked").val();
                    break;
            }
        } else if (tagName == "select") {
            value = $(element).find('option:selected').val();
        } else if (tagName == "textarea") {
            value = $(element).val();
        } else {
            value = $(element).html();
        }
        entity[key] = value;
    });

    return entity;
}

function setEntity(entity, form) {
    var context = form ? form : document;
    $(context).find("[name]").each(function (index, element) {
        var key = $(element).attr("name");
        var value = entity[key];

        var tagName = $(element).tagName().toLowerCase();
        if (tagName == "input") {
            var type = $(element).attr("type");
            switch (type) {
                case "text":
                default:
                    $(element).val(value);

                    if ($(element).hasClass("money-separator"))
                        moneySeparator(element);

                    break;
                case "checkbox":
                case "radio":
                    var val = $(element).val();
                    if (typeof value == "boolean") {
                        $(element).prop("checked", value);
                    }
                    else if (typeof value == "object") {
                        if (value != null && findInArray(value, val)) {
                            $(element).prop("checked", true);
                        } else {
                            $(element).prop("checked", false);
                        }
                    }
                    else {
                        if (value != null && value == val) {
                            $(element).prop("checked", true);
                        } else {
                            $(element).prop("checked", false);
                        }
                    }
                    break;
            }
        } else if (tagName == "select" || tagName == "textarea") {
            if (value != null)
                $(element).val(value.toString());
            else
                $(element).val("");
        } else {
            $(element).html(value);
        }
    });
}

function findInArray(arr, item) {
    for (var i = 0; i < arr.length; i++) {
        if (arr[i].toString() == item.toString()) {
            return true;
        }
    }
    return false;

}

function clearEntity(form) {
    var context = form ? form : document;
    $(context).find("[name]").each(function (index, element) {
        var tagName = $(element).tagName().toLowerCase();
        if (tagName == "input") {
            var type = $(element).attr("type");
            switch (type) {
                case "text":
                case "tel":
                default:
                    $(element).val("");
                    break;
                case "checkbox":
                case "radio":
                    $(element).prop("checked", false);
                    break;
            }
        } else if (tagName == "select") {
            $(element).val($(element).find("option:first").val());
        } else if (tagName == "textarea") {
            $(element).val("");
        } else {
            $(element).html("");
        }
        $(element).removeClass('errorClass');
    });
}
