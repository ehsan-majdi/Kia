$(document).ready(function () {
    $(document).on("keyup", ".element-error", function (event) {
        $(this).removeClass("element-error");
    });

    $(document).on("change", ".element-error", function (event) {
        $(this).removeClass("element-error");
    });
});

function checkForm(form) {
    form = form || document;

    var valid = true;
    $(form).find("[data-validate]").each(function (index, element) {
        if (checkElement($(element))) {
            // do when element validate success
            $(element).removeClass("element-error");
        }
        else {
            //do when element validation faild
            $(element).addClass("element-error");
            valid = false;
        }
    });

    return valid;
}

function checkElement(element) {

    if (typeof element === "string" || !element.attr) {
        element = $(element);
    }

    var expression = element.attr("data-validate");
    var rules = expression.replace(/^\s+|\s+$/g, "").split(/\s*,\s*/);

    var state = true;

    var _value = element.val();
    for (var i = 0; i < rules.length; i++) {
        if (rules[i] === "required") {
            if (!_value)
                state = false;
        }
        else if (rules[i].startsWith("minLength")) {
            var length = parseInt(rules[i].match(/\(([^)]+)\)/)[1]);
            if (_value.length < length)
                state = false;
        }
        else if (rules[i].startsWith("maxLength")) {
            var length = parseInt(rules[i].match(/\(([^)]+)\)/)[1]);
            if (_value.length > length)
                state = false;
        }
        else if (rules[i].startsWith("length")) {
            var length = rules[i].match(/\(([^)]+)\)/)[1];
            var min = length.split("-")[0];
            var max = length.split("-")[1];
            if (_value.length > max || _value.length < min)
                state = false;
        }
        else if (rules[i].startsWith("min")) {
            var number = parseInt(rules[i].match(/\(([^)]+)\)/)[1]);
            if (parseInt(_value) < number)
                state = false;
        }
        else if (rules[i].startsWith("max")) {
            var number = parseInt(rules[i].match(/\(([^)]+)\)/)[1]);
            if (parseInt(_value) > number)
                state = false;
        }
        else if (rules[i] === "float") {
            if (_value.length > 0) {
                if (isNaN(_value))
                    state = false;
            }
        }
        else if (rules[i] === "number") {
            while (_value.indexOf(",") >= 0) {
                _value = _value.replace(",", "");
            }

            if (_value.length > 0) {
                var numberRegex = new RegExp(/^\d+$/);
                if (!numberRegex.test(_value)) {
                    state = false;
                }
            }
        }
        else if (rules[i] === "persianText") {
            if (_value.length > 0) {
                var persianTextRegex = new RegExp("^[\u0600-\u06FF 0-9۰-۹]+$", 'i');
                if (!_value.match(persianTextRegex))
                    state = false;
            }
        }
        else if (rules[i] === "englishText") {
            if (_value.length > 0) {
                var persianTextRegex = new RegExp("^[A-Za-z0-9 ]+$", 'i');
                if (!_value.match(persianTextRegex))
                    state = false;
            }
        }
        else if (rules[i] === "username") {
            if (_value.length > 0) {
                var usernameRegex = new RegExp("^[A-Za-z0-9_.]+$", 'i');
                if (!_value.match(usernameRegex)) {
                    state = false;
                }
            }
        }
        else if (rules[i] === "nationalityCode") {
            if (_value.length > 0) {
                if (!checkNationalityCode(_value))
                    state = false;
            }
        }
        //else if (rules[i] === "mobileNumber") {
        //    if (_value.length > 0) {
        //        var mobileNumberRegex = new RegExp("09(1[0-9]|3[1-9])[0-9]{7}");
        //        if (!_value.match(mobileNumberRegex))
        //            state = false;
        //    }
        //}
        else if (rules[i] === "mobileNumber") {
            if (_value.length > 0) {
                var mobileNumberRegex = new RegExp("09(0[1-5]|1[0-9]|2[0-2]|3[0-9]|9[0-2])[0-9]{7}");
                if (!_value.match(mobileNumberRegex))
                    state = false;
            }
        }

        else if (rules[i] === "mobileNumberCL") {
            if (_value.length > 0) {
                var mobileNumberRegexe = new RegExp("09(0[1-9]|1[0-9]|2[0-9]|3[0-9]|9[0-9])[0-9]{7}|0(2[0-9])[0-9]{8}");
                var telephoneRegexe = new RegExp("0(2[0-9])[0-9]{8}");
                if (!_value.match(mobileNumberRegexe))
                    state = false;
            }
        }

        //else if (rules[i] === "persianDate") {
        //    if (_value.length > 0) {
        //        var _regex = /^1[34][0-9][0-9]\/((0[1-6]\/((3[0-1])|([1-2][0-9])|(0[1-9])))|((1[0-2]|(0[7-9]))\/(30|([1-2][0-9])|(0[1-9]))))$/gi;
        //        _validate = _regex.test(_value);
        //        if (!_validate)
        //            state = false;
        //    }
        //}
        else if (rules[i] === "email") {
            if (_value.length > 0) {
                var _regex = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/gi;
                _validate = _regex.test(_value);
                if (!_validate)
                    state = false;
            }
        }
        else if (rules[i] === "persianDate") {
            if (_value.length > 0) {
                if (!checkPersianDate(_value)) {
                    state = false;
                }
            }
        }
    }

    return state;
}

function checkNationalityCode(nationalityCode) {
    if (typeof nationalityCode === "number") {
        nationalityCode = nationalityCode.toString();
    }

    if (nationalityCode.length == 10) {
        if (nationalityCode === '1111111111' ||
            nationalityCode === '0000000000' ||
            nationalityCode === '2222222222' ||
            nationalityCode === '3333333333' ||
            nationalityCode === '4444444444' ||
            nationalityCode === '5555555555' ||
            nationalityCode === '6666666666' ||
            nationalityCode === '7777777777' ||
            nationalityCode === '8888888888' ||
            nationalityCode === '9999999999') {
            return false;
        }
        c = parseInt(nationalityCode.charAt(9));
        n = parseInt(nationalityCode.charAt(0)) * 10 +
            parseInt(nationalityCode.charAt(1)) * 9 +
            parseInt(nationalityCode.charAt(2)) * 8 +
            parseInt(nationalityCode.charAt(3)) * 7 +
            parseInt(nationalityCode.charAt(4)) * 6 +
            parseInt(nationalityCode.charAt(5)) * 5 +
            parseInt(nationalityCode.charAt(6)) * 4 +
            parseInt(nationalityCode.charAt(7)) * 3 +
            parseInt(nationalityCode.charAt(8)) * 2;
        r = n - parseInt(n / 11) * 11;
        if ((r === 0 && r === c) || (r === 1 && c === 1) || (r > 1 && c === 11 - r)) {
            return true;
        } else {
            return false;
        }
    } else {
        return false;
    }
}

function checkPersianDate(date) {
    if (date === null || date.toString().indexOf("/") < 0) {
        return false;
    }

    var parts = date.split('/');
    if (parts === null || parts.length !== 3) {
        return false;
    }

    if (parts[0].length !== 4 || !(parts[0].startsWith("13") || parts[0].startsWith("14"))) {
        return false;
    }

    if (!(parts[1].length === 1 || parts[1].length === 2) || (parseInt(parts[1]) > 12 && parseInt(parts[1]) < 1)) {
        return false;
    }

    if (parseInt(parts[1]) < 6) {
        if (parseInt(parts[2]) > 31 || parseInt(parts[2]) < 1) {
            return false;
        }
    }
    else if (parseInt(parts[1]) < 12) {
        if (parseInt(parts[2]) > 30 || parseInt(parts[2]) < 1) {
            return false;
        }
    }
    else if (parseInt(parts[1]) === 12) {
        var a = 0, b = 1309, c = parseInt(parts[0]);
        for (var i = 1309; i <= c - 4; i += 4) {
            b += 4;
            a += 1;
            if (a % 8 === 0)
                b++;
        }

        if (c === b) {
            if (parseInt(parts[2]) > 30 || parseInt(parts[2]) < 1) {
                return false;
            }
        }
        else {
            if (parseInt(parts[2]) > 29 || parseInt(parts[2]) < 1) {
                return false;
            }
        }
    }

    return true;
}