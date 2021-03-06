/**
 * Name:Javascript Number To Persian Convertor.
 * Author:Mahmoud Eskanadri.
 * Copyright:2018 http://Webafrooz.com.
 * Licence: GNU Licence 2.4
 * version:2.0.0
 * Email:info@webafrooz.com,sbs8@yahoo.com
 * coded with ♥ in Webafrooz.
 * big numbers refrence: https://fa.wikipedia.org/wiki/%D9%86%D8%A7%D9%85_%D8%A7%D8%B9%D8%AF%D8%A7%D8%AF_%D8%A8%D8%B2%D8%B1%DA%AF
 **/

/**
 * A function for converting numbers to persian letters
 * @returns {*}
 * @constructor
 */
NumToPersian = (function () {
    /**
     *
     * @type {string}
     */
    spliter = " و ";

    /**
     *
     * @type {string}
     */
    zero = "صفر";

    /**
     *
     * @type {*[]}
     */
    Letters = [
        ["", "یك", "دو", "سه", "چهار", "پنج", "شش", "هفت", "هشت", "نه"],
        ["ده", "یازده", "دوازده", "سیزده", "چهارده", "پانزده", "شانزده", "هفده", "هجده", "نوزده", "بیست"],
        ["", "", "بیست", "سی", "چهل", "پنجاه", "شصت", "هفتاد", "هشتاد", "نود"],
        ["", "یكصد", "دویست", "سیصد", "چهارصد", "پانصد", "ششصد", "هفتصد", "هشتصد", "نهصد"],
        ['', " هزار ", " میلیون ", " میلیارد ", " بیلیون ", " بیلیارد ", " تریلیون ", " تریلیارد ",
            " کوآدریلیون ", " کادریلیارد ", " کوینتیلیون ", " کوانتینیارد ", " سکستیلیون ", " سکستیلیارد ", " سپتیلیون ",
            " سپتیلیارد ", " اکتیلیون ", " اکتیلیارد ", " نانیلیون ", " نانیلیارد ", " دسیلیون ", " دسیلیارد "]
    ];

    /**
     * Clear number and split to 3th sections
     * @param {*} num 
     */
    function PrepareNumber(num) {
        if (typeof num === "number") {
            num = num.toString();
        }
        NumberLength = num.length % 3;
        if (NumberLength === 1) {
            num = "00" + num;
        } else if (NumberLength === 2) {
            num = "0" + num;
        }
        //Explode to array
        return num.replace(/\d{3}(?=\d)/g, "$&*").split('*');
    }

    /**
     * Convert 3 numbers into letter
     * @param {*} num 
     */
    function ThreeNumbersToLetter(num) {
        //return zero
        if (parseInt(num) === 0) {
            return "";
        }
        parsedInt = parseInt(num);
        if (parsedInt < 10) {
            return Letters[0][parsedInt];
        }
        if (parsedInt <= 20) {
            return Letters[1][parsedInt - 10];
        }
        if (parsedInt < 100) {
            one = parsedInt % 10;
            ten = (parsedInt - one) / 10;
            if (one > 0) {
                return Letters[2][ten] + spliter + Letters[0][one];
            }
            return Letters[2][ten];
        }
        one = parsedInt % 10;
        hundreds = (parsedInt - parsedInt % 100) / 100;
        ten = (parsedInt - ((hundreds * 100) + one)) / 10;
        out = [Letters[3][hundreds]];
        SecendPart = ((ten * 10) + one);
        if (SecendPart > 0) {
            if (SecendPart < 10) {
                out.push(Letters[0][SecendPart]);
            } else if (SecendPart <= 20) {
                out.push(Letters[1][SecendPart - 10]);
            } else {
                out.push(Letters[2][ten]);
                if (one > 0) {
                    out.push(Letters[0][one]);
                }
            }
        }
        return out.join(spliter);
    }

    /**
     * Main function
     */
    return function (num) {
        //return zero
        if (parseInt(num) === 0) {
            return zero;
        }
        if (num.length > 66) {
            return "خارج از محدوده";
        }
        //Split to sections
        SplitedNumber = PrepareNumber(num);

        //Fetch Sections and convert
        funcout = [];
        SplitLength = SplitedNumber.length;
        for (i = 0; i < SplitLength; i++) {
            SectionTitle = Letters[4][SplitLength - (i + 1)];
            converted = ThreeNumbersToLetter(SplitedNumber[i]);
            if (converted !== "") {
                funcout.push(converted + SectionTitle);
            }
        }
        return funcout.join(spliter);
    };
})();
String.prototype.toPersian = function () {
    return NumToPersian(this);
};
Number.prototype.toPersian = function () {
    return NumToPersian(parseInt(this).toString());
};


$(document).ready(function () {
    $(".price-text").each(function () {
        var obj = $(this);
        var element = obj.attr("data-bind");

        $("#" + element).on("change", function () {
            var num = parseInt(removeSeparator($(this).val()));
            var stringNumber = NumToPersian(num);
            $(obj).html(stringNumber + " ریال");
        });

        $("#" + element).on("keyup", function () {
            var num = parseInt(removeSeparator($(this).val()));
            var stringNumber = NumToPersian(num);
            $(obj).html(stringNumber + " ریال");
        });

        var num = parseInt(removeSeparator($("#" + element).val()));
        var stringNumber = NumToPersian(num);
        $(obj).html(stringNumber + " ریال");
    });

    $(".number-text").each(function () {
        var obj = $(this);
        var element = obj.attr("data-bind");

        $("#" + element).on("change", function () {
            var num = parseInt(removeSeparator($(this).val()));
            var stringNumber = NumToPersian(num);
            $(obj).html(stringNumber);
        });

        $("#" + element).on("keyup", function () {
            var num = parseInt(removeSeparator($(this).val()));
            var stringNumber = NumToPersian(num);
            $(obj).html(stringNumber);
        });

        var num = parseInt(removeSeparator($("#" + element).val()));
        var stringNumber = NumToPersian(num);
        $(obj).html(stringNumber);
    });

});