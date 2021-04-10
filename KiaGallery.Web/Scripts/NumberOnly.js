var allowedKeys = [8, 9, 13, 35, 36, 37, 39, 1776 /*۰*/, 1777 /*۱*/, 1778 /*۲*/, 1779 /*۳*/, 1780 /*۴*/, 1781 /*۵*/, 1782 /*۶*/, 1783 /*۷*/, 1784 /*۸*/, 1785 /*۹*/];

$(document).ready(function () {
    $(document).on("keypress",".number-only", function (event) {
        if (event.which) {
            var charString = String.fromCharCode(event.which);
            var transformedChar = transformTypedChar(charString);

            var isAllowedKey = allowedKeys.join(",").match(new RegExp(event.which));

            if ((48 <= event.which && event.which <= 57) || (48 === event.which && $(this).attr("value")) || isAllowedKey) {
                if (transformedChar !== charString) {
                    var sel = getInputSelection(this), val = this.value;
                    this.value = val.slice(0, sel.start) + transformedChar + val.slice(sel.end);

                    // Move the caret
                    setInputSelection(this, sel.start + 1, sel.start + 1);
                    return false;
                }
            }
            else {
                return false;
            }
        }
    });

    $(".number-only").bind("paste", function (event) {
        var pastedText = undefined;
        if (window.clipboardData && window.clipboardData.getData) {
            pastedText = window.clipboardData.getData("Text");
        }
        else if (event.originalEvent.clipboardData && event.originalEvent.clipboardData.getData) {
            pastedText = event.originalEvent.clipboardData.getData("text/plain");
        }

        if (pastedText) {
            var convertedText = "";
            for (var i = 0; i < pastedText.length; i++) {
                var which = pastedText.charCodeAt(i);
                var isAllowedKey = allowedKeys.join(",").match(new RegExp(which));

                if ((48 <= which && which <= 57) || isAllowedKey) {
                    var charString = String.fromCharCode(which);
                    convertedText += transformTypedChar(charString);
                }
            }

            var selection = getInputSelection(this), value = this.value;
            this.value = value.slice(0, selection.start) + convertedText + value.slice(selection.end);

            // Move the caret
            setInputSelection(this, selection.start, selection.start + convertedText.length);
        }
        return false;
    });
});

function transformTypedChar(charString) {
    switch (charString) {
        case "۰":
            return "0";
        case "۱":
            return "1";
        case "۲":
            return "2";
        case "۳":
            return "3";
        case "۴":
            return "4";
        case "۵":
            return "5";
        case "۶":
            return "6";
        case "۷":
            return "7";
        case "۸":
            return "8";
        case "۹":
            return "9";
        default:
            return charString;
    }
}

function getInputSelection(element) {
    var start = 0, end = 0, normalizedValue, range,
        textInputRange, length, endRange;

    if (typeof element.selectionStart === "number" && typeof element.selectionEnd === "number") {
        start = element.selectionStart;
        end = element.selectionEnd;
    } else {
        range = document.selection.createRange();

        if (range && range.parentElement() === element) {
            length = element.value.length;
            normalizedValue = element.value.replace(/\r\n/g, "\n");

            // Create a working TextRange that lives only in the input
            textInputRange = element.createTextRange();
            textInputRange.moveToBookmark(range.getBookmark());

            // Check if the start and end of the selection are at the very end
            // of the input, since moveStart/moveEnd doesn't return what we want
            // in those cases
            endRange = element.createTextRange();
            endRange.collapse(false);

            if (textInputRange.compareEndPoints("StartToEnd", endRange) > -1) {
                start = end = length;
            } else {
                start = -textInputRange.moveStart("character", -length);
                start += normalizedValue.slice(0, start).split("\n").length - 1;

                if (textInputRange.compareEndPoints("EndToEnd", endRange) > -1) {
                    end = length;
                } else {
                    end = -textInputRange.moveEnd("character", -length);
                    end += normalizedValue.slice(0, end).split("\n").length - 1;
                }
            }
        }
    }

    return {
        start: start,
        end: end
    };
}

function offsetToRangeCharacterMove(el, offset) {
    return offset - (el.value.slice(0, offset).split("\r\n").length - 1);
}

function setInputSelection(element, startOffset, endOffset) {
    element.focus();
    if (typeof element.selectionStart === "number" && typeof element.selectionEnd === "number") {
        element.selectionStart = startOffset;
        element.selectionEnd = endOffset;
    } else {
        var range = element.createTextRange();
        var startCharMove = offsetToRangeCharacterMove(element, startOffset);
        range.collapse(true);
        if (startOffset === endOffset) {
            range.move("character", startCharMove);
        } else {
            range.moveEnd("character", offsetToRangeCharacterMove(element, endOffset));
            range.moveStart("character", startCharMove);
        }
        range.select();
    }
}