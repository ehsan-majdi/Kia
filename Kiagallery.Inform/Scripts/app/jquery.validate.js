$(document).ready(function () {
    $(document).on("keyup", ".element-error", function () {
        $(this).removeClass("element-error");
    });

    $(document).on("change", ".element-error", function (event) {
        $(this).removeClass("element-error");
    });
});


(function ($) {

    $.fn.validate = function (options) {

        var settings = $.extend({
            hidden: false // check hidden input
        }, options);

        var valid = true;

        $(this).find("[data-validate]").each(function (index, element) {
            if (!checkElement(element)) {
                $(element).addClass("element-error");
                valid = false;
            }
            else {
                $(element).removeClass("element-error");
            }
        });

        return valid;
    };

    function checkElement(element) {

        if (typeof element === "string" || !element.attr) {
            element = $(element);
        }

        var expression = element.attr("data-validate");
        var rules = expression.replace(/^\s+|\s+$/g, "").split(/\s*,\s*/);

        var valid = true;

        var value = element.val().trim();
        for (var i = 0; i < rules.length; i++) {
            if (rules[i] === "required") {
                if (!value) {
                    valid = false;
                }
            }
            else if (rules[i] === "persianText") {
                if (value) {
                    var persianTextRegex = new RegExp("^[\u0600-\u06FF ]+$", 'i');
                    if (!value.match(persianTextRegex)) {
                        valid = false;
                    }
                }
            }
            else if (rules[i] === "englishText") {
                if (value) {
                    var englishTextRegex = new RegExp("^[A-Za-z ]+$", 'i');
                    if (!value.match(englishTextRegex)) {
                        valid = false;
                    }
                }
            }
            else if (rules[i] === "persian") {
                if (value) {
                    var persianRegex = new RegExp("^[\u0600-\u06FF 0-9۰-۹.()]+$", 'i');
                    if (!value.match(persianRegex)) {
                        valid = false;
                    }
                }
            }
            else if (rules[i] === "english") {
                if (value) {
                    var englishRegex = new RegExp("^[A-Za-z0-9 .()]+$", 'i');
                    if (!value.match(englishRegex)) {
                        valid = false;
                    }
                }
            }
            else if (rules[i] === "email") {
                if (value) {
                    var emailRegex = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/gi;
                    var validate = emailRegex.test(value);
                    if (!validate)
                        valid = false;
                }
            }
            else if (rules[i] === "date") {
                if (value) {
                    var dateRegex = new RegExp("^\d{4}\/\d{1,2}\/\d{1,2}$", 'i');
                    if (!value.match(dateRegex)) {
                        valid = false;
                    }
                }
            }
            else if (rules[i] === "persianDate") {
                if (value) {
                    if (!checkPersianDate(value)) {
                        state = false;
                    }
                }
            }
            else if (rules[i] === "time") {
                if (value) {
                    var timeRegex = new RegExp("^([0-9]|0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$", 'i');
                    if (!timeRegex.match(dateRegex)) {
                        valid = false;
                    }
                }
            }
            else if (rules[i] === "percent") {
                if (value) {
                    if (isNaN(value) || parseInt(value) > 100 || parseInt(value) < 0) {
                        valid = false;
                    }
                }
            }
            else if (rules[i] === "percent") {
                if (value) {
                    if (isNaN(value) || parseInt(value) > 100 || parseInt(value) < 0) {
                        valid = false;
                    }
                }
            }
            else if (rules[i].startsWith("minLength")) {
                var length = parseInt(rules[i].match(/\(([^)]+)\)/)[1]);
                if (value.length < length)
                    valid = false;
            }
            else if (rules[i].startsWith("maxLength")) {
                var length = parseInt(rules[i].match(/\(([^)]+)\)/)[1]);
                if (value.length > length)
                    valid = false;
            }
            else if (rules[i].startsWith("min(") || rules[i].startsWith("min[")) {
                var number = parseInt(rules[i].match(/\(([^)]+)\)/)[1]);
                if (parseInt(value) < number)
                    valid = false;
            }
            else if (rules[i].startsWith("max(") || rules[i].startsWith("max[")) {
                var number = parseInt(rules[i].match(/\(([^)]+)\)/)[1]);
                if (parseInt(value) > number)
                    valid = false;
            }
            else if (rules[i].startsWith("length")) {
                var length = rules[i].match(/\(([^)]+)\)/)[1];
                var min = length.split("-")[0];
                var max = length.split("-")[1];
                if (value.length > max || value.length < min)
                    valid = false;
            }
            else if (rules[i] === "number") {
                while (value.indexOf(",") >= 0) {
                    value = value.replace(",", "");
                }

                if (value) {
                    var numberRegex = new RegExp(/^\d+$/);
                    if (!numberRegex.test(value)) {
                        valid = false;
                    }
                }
            }
            else if (rules[i] === "float") {
                if (value) {
                    if (isNaN(value))
                        valid = false;
                }
            }
            else if (rules[i] === "nationalityCode") {
                if (value) {
                    if (!checkNationalityCode(value))
                        valid = false;
                }
            }
            else if (rules[i] === "mobileNumber") {
                if (value.length > 0) {
                    var mobileNumberRegex = new RegExp("09(1[0-9]|3[1-9])[0-9]{7}");
                    if (!value.match(mobileNumberRegex))
                        valid = false;
                }
            }
        }

        return valid;
    }

    // بررسی تاریخ وارد شده شمسی
    function checkPersianDate(date) {

        // تاریخ بایستی حتما شامل خط تیره یا اسلش باشه
        if (date || (date.toString().indexOf("/") < 0 && date.toString().indexOf("-") < 0)) {
            return false;
        }

        // جدا کردن تاریخ ورودی بر اساس اسلش و خط تیره
        var parts = date.split(/[/-]/);
        if (parts === null || parts.length !== 3) {
            return false;
        }

        // بررسی سال
        // سال حتما می بایستی 4 رقمی باشد و با 13 یا 14 شروع شود
        if (parts[0].length !== 4 || !(parts[0].startsWith("13") || parts[0].startsWith("14"))) {
            return false;
        }

        // بررسی ماه
        // ماه حتما بایستی عدد بوده و از 1 بزگتر و از 12 کوکتر باشد
        var month = parseInt(parts[1]);
        if (isNaN(parts[1]) || (month > 12 && month < 1)) {
            return false;
        }

        // بررسی روز
        // در صورتی که در شش ماه اول سال بود
        if (parseInt(parts[1]) < 6) {
            // روز حتما باید بین 1 و 31 باشد
            if (parseInt(parts[2]) > 31 || parseInt(parts[2]) < 1) {
                return false;
            }
        }
        // در صورتی که ماه از 12 کوچکتر بود
        else if (parseInt(parts[1]) < 12) {
            // تعداد روز بایستی بین 1 و 31 باشد
            if (parseInt(parts[2]) > 30 || parseInt(parts[2]) < 1) {
                return false;
            }
        }
        // در صورتی که ماه دقیقا 12 انتخاب شده بود، بایستی به کبیسه بودن سال توجه کرد
        else if (parseInt(parts[1]) === 12) {

            // بررسی کبیسه بودن سال
            // سال کبیسه هر چهار سال است و در مدت هر 32 سال این عدد به 5  میرسد یعنی8 دوره هر 4 سال و یک دوره 5 سله می شود
            var leapPoint = 0, leap = 1309, year = parseInt(parts[0]);
            for (var i = 1309; i <= year - 4; i += 4) {
                leap += 4;
                leapPoint += 1;
                if (leapPoint % 8 === 0)
                    leap++;
            }

            // بررسی نتیجه کبیسه بودن سال وارد شده
            if (year === leap) {
                // در صورتی که سال کبیسه باشد تعداد روز می بایست بین 1 و 30 باشد
                if (parseInt(parts[2]) > 30 || parseInt(parts[2]) < 1) {
                    return false;
                }
            }
            else {
                // در صورت کبیسه نبودن ساال وارد شده بایست روز وارد شده بین 1 و 29 باشد
                if (parseInt(parts[2]) > 29 || parseInt(parts[2]) < 1) {
                    return false;
                }
            }
        }

        // در صورت صحیح بودن تاریخ وارد شده و برقرار نشدن هر کدام از شرط های بالا، تاریخ وارد شده، صحیح و معتبر می باشد
        return true;
    }

    // بررسی کد ملی
    // مقادیر ورودی: کد ملی
    function checkNationalityCode(nationalityCode) {

        // در صورتی که کد ملی وارد شده عدد بود، آن را به رشته تبدیل میکند تا مورد بررسی قرار دهد
        if (typeof nationalityCode === "number") {
            nationalityCode = nationalityCode.toString();
        }

        // بررسی 10 رقمی بودن کد ملی
        if (nationalityCode.length === 10) {
            // بررسی تکراری بودن رقم های کد ملی
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

            // فرمول صحت اعتبار کد ملی
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

}(jQuery));