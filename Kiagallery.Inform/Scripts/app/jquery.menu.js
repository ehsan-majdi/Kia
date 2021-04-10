$(document).ready(function () {
    var menu = { menu: menuList };

    var template = $.templates("#tmplMenu");
    var htmlOutput = template.render(menu);
    $("#menu").html(htmlOutput);
});