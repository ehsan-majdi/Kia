$(document).ready(function () {
    setUpStepWizard();
    loadRemoteSelect(".cmbWorkshop", "/workshop/getAll", null, "id", "name", "...", function () {
        loadRemoteSelect("#cmbSize", "/size/getAll", null, "id", "title", "...", function () {
            load();
        });
    });

    $("#cmbSize").on("change", function () {
        var params = { id: $(this).val() };
        if ($(this).val() != null && $(this).val() != "")
            loadRemoteSelect("#cmbNormalSizeValue", "/size/getSizeValueList", params, "id", "value", "...");
        else {
            $("#cmbNormalSizeValue").empty();
            $("#cmbNormalSizeValue").append("<option value=\"\">...</option>");
        }
    });

    $("#cmbProductType").on("change", function () {
        var productType = $(this).val();
        if (productType == 10)
            $("#formOuterWerkType").slideDown("slow");
        else
            $("#formOuterWerkType").slideUp("slow");
        if (productType == 3)
            $("#cmbRingSizeType").show()
        else
            $("#cmbRingSizeType").hide()
        if (productType == 2)
            $(".earringBack").show()
        else
            $(".earringBack").hide()
    });

    $("#btnSave").on("click", function (event) {
        event.preventDefault();

        if (checkForm("#form")) {
            var entity = getEntity("#form");
            var stoneList = new Array();
            $(".stone-list").each(function (index, element) {
                var order = $(this).attr("data-order");
                var shape = $(this).find(".shape").val();
                var shapeSizeId = $(this).find(".shape-size").val();
                var defaultStoneId = $(this).find(".default-stone:checked").val();
                $(this).find(".stone-item.checked").each(function (i, e) {
                    stoneList.push({
                        order: order,
                        defaultStoneId: defaultStoneId,
                        stoneId: $(this).attr("data-stone-id"),
                        shapeSizeId: shapeSizeId,
                        stoneShape: shape
                    });
                });
            });
            entity.stoneList = stoneList;

            var leatherList = new Array();
            $(".leather-list").each(function (index, element) {
                var order = $(this).attr("data-order");
                $(this).find(".leather-item.checked").each(function (i, e) {
                    leatherList.push({
                        order: order,
                        leatherId: $(this).attr("data-leather-id")
                    });
                });
            });
            entity.leatherList = leatherList;

            var fileList = new Array();
            $(".file-list").each(function (index, element) {
                fileList.push({
                    fileName: $(this).attr("data-file-name"),
                    fileType: $(this).attr("data-file-type"),
                    paddingImg: $(this).find("#cmbPaddingImg").val()
                });
            });
            entity.fileList = fileList;
            //if (entity.fileList.length < 1) {
            //    alert("عکس انتخاب نشده است.");
            //}
            if (leatherList.length < entity.leatherCount) {
                alert("تعداد چرم انتخاب شده کمتر تعداد چرم مورد نیاز می باشد.");
            }
            else if (stoneList.length < entity.stoneCount) {
                alert("تعداد سنگ انتخاب شده کمتر تعداد سنگ مورد نیاز می باشد.");
            }
            else {
                loader(true);
                $.post("/product/save", entity, function (response) {
                    loader(false);
                    if (response.status == 200) {
                        callbackAlert(response.message, function () {
                            document.location = "/product";
                        });
                    }
                    else {
                        alert(response.message);
                    }
                });
            }
        }
    });

    $("#btnNextStep1").on("click", function (event) {
        event.preventDefault();
        if (checkForm("#form")) {
            checkSteps();

            if ($("#txtStoneCount").val() > 0) {
                showStep(2);
            }
            else if ($("#txtLeatherCount").val() > 0) {
                showStep(3);
            }
            else {
                showStep(4);
            }
        }
    });

    $("#btnPrevStep2").on("click", function (event) {
        event.preventDefault();
        showStep(1);
    });

    $("#btnNextStep2").on("click", function (event) {
        event.preventDefault();
        if ($("#txtLeatherCount").val() > 0) {
            showStep(3);
        }
        else {
            showStep(4);
        }
    });

    $("#btnPrevStep3").on("click", function (event) {
        event.preventDefault();
        if ($("#txtStoneCount").val() > 0) {
            showStep(2);
        }
        else {
            showStep(1);
        }
    });
    $("#btnNextStep3").on("click", function (event) {
        event.preventDefault();
        showStep(4);
    });

    $("#btnPrevStep4").on("click", function (event) {
        event.preventDefault();
        if ($("#txtLeatherCount").val() > 0) {
            showStep(3);
        }
        else if ($("#txtStoneCount").val() > 0) {
            showStep(2);
        }
        else {
            showStep(1);
        }
    });

    $("#btnUploadNewImage").on("click", function (event) {
        event.preventDefault();

        $("#cmbFileType").val("");
        $('.progress .progress-bar').css('width', '0%');

        $("#modalUploadFile").modal('show');
    });

    $("#openFile").on("click", function () {


        if ($("#cmbFileType").val() != "") {
            if ($("#cmbFileType").val() == 5) {

            }
            $("#file").click();
        }
        else {
            alert("ابتدا نوع فایل را مشخص کنید.");
        }
    });

    $("#file").on("change", function () {
        save()
    });

    //$('#file').fileupload({

    //    dataType: 'json',
    //    url: "/product/uploadFile?id=" + id + "&type=" + fileTypeTitle,
    //    autoUpload: true,
    //    done: function (e, data) {
    //        var response = data.result;
    //        if (response.status == 200) {
    //            addFile($("#cmbFileType").val(), response.data.name);
    //            $("#modalUploadFile").modal('hide');
    //        }
    //        alert(response.message);
    //        loader(false);
    //    }
    //}).on('fileuploadprogressall', function (e, data) {
    //    var progress = parseInt(data.loaded / data.total * 100, 10);
    //    $('.progress .progress-bar').css('width', progress + '%');
    //});
    function save() {

        var params = { id: id, type: $("#hiddenTypeTitle").val() };

        $("#fileModal").find("button").prop("disabled", true);
        $("#fileModal").find("input").prop("disabled", true);
        $("#fileModal").find("textarea").prop("disabled", true);
        $("#fileModal").find("select").prop("disabled", true);

        var filesList = $("#file").prop("files");
        $("#file").fileupload({
            dataType: 'json',
            url: "/product/uploadFile",
            formData: params,
            autoUpload: false,
            start: function () {
                $("#progressBar").closest(".progress").removeClass("hidden");
            },
            success: function (response, textStatus, jqXHR) {
                $("#fileModal").find("button").prop("disabled", false);
                $("#fileModal").find("input").prop("disabled", false);
                $("#fileModal").find("textarea").prop("disabled", false);
                $("#fileModal").find("select").prop("disabled", false);
                $("#file").fileupload("destroy");

                if (response.status === 403 || response.status === 401) {
                    document.location.href = "/login?redirect=" + document.location.pathname;
                }
                else if (response.status === 200) {
                    addFile($("#cmbFileType").val(), response.data.name);
                    $("#modalUploadFile").modal('hide');
                    alert(response.message);
                }
                else {
                    alert(response.message);
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                $("#file").fileupload("destroy");
                $("#progressBar").closest(".progress").removeClass("hidden");
            },
            complete: function (result, textStatus, jqXHR) {
                $("#file").fileupload("destroy");
                $("#progressBar").closest(".progress").addClass("hidden");
            },
            progressall: function (e, data) {
                var progress = parseInt(data.loaded / data.total * 100, 10);
                console.log(progress)
                $('#progressBar').css({ 'width': progress + "%" });
                $('#progressBar').find(".sr-only").html(progress + "%");
            }
        });

        $("#file").fileupload('send', { files: filesList });
    }
    $(document).on("click", ".delete-file", function (event) {
        $(this).closest(".file-list").remove();

        if ($(".file-list").length == 0) {
            $(".no-image").show();
            $(".table-image").addClass("hidden");
        }

    });

    $(document).on("click", ".stone-type", function (event) {
        event.preventDefault();
        var order = $(this).attr("data-order");
        var type = $(this).attr("data-type");

        //$(".stone-item[data-order=" + order + "]").removeClass("checked");
        if ($(".stone-item[data-order=" + order + "][data-stone-type=" + type + "]").not(".checked").length > 0) {
            $(".stone-item[data-order=" + order + "][data-stone-type=" + type + "]").addClass("checked").change();
        }
        else {
            $(".stone-item[data-order=" + order + "][data-stone-type=" + type + "]").removeClass("checked").change();
        }
    });

    $(document).on("click", ".leather-type", function (event) {
        event.preventDefault();
        var order = $(this).attr("data-order");
        var type = $(this).attr("data-type");

        //$(".leather-item[data-order=" + order + "]").removeClass("checked");
        if ($(".leather-item[data-order=" + order + "][data-leather-type=" + type + "]").not(".checked").length > 0) {
            $(".leather-item[data-order=" + order + "][data-leather-type=" + type + "]").addClass("checked");
        }
        else {
            $(".leather-item[data-order=" + order + "][data-leather-type=" + type + "]").removeClass("checked");
        }
    });

    $(document).on("click", ".stone-item", function () {
        $(this).toggleClass("checked");

        if ($(this).hasClass("checked")) {
            $(this).closest(".stone").find(".default-stone").prop("disabled", false);
        }
        else {
            $(this).closest(".stone").find(".default-stone").prop("disabled", true);
            $(this).closest(".stone").find(".default-stone").prop("checked", false);
        }
    });

    $(document).on("change", ".stone-item", function () {
        if ($(this).hasClass("checked")) {
            $(this).closest(".stone").find(".default-stone").prop("disabled", false);
        }
        else {
            $(this).closest(".stone").find(".default-stone").prop("disabled", true);
            $(this).closest(".stone").find(".default-stone").prop("checked", false);
        }
    });
    $(document).on("click", ".leather-item", function () {
        $(this).toggleClass("checked");
    });

    $(document).on("change", ".shape", function () {
        var item = $(this);
        var target = item.closest(".stone-list").find(".shape-size:first");
        var params = { shape: item.val() };
        loader(true);
        $.get("/shapeSize/getAllSize", params, function (response) {
            loader(false);
            $(target).empty();
            if (response.status == 200) {
                var data = response.data;
                $(target).append("<option value=\"\">...</option>");
                for (var i = 0; i < data.list.length; i++) {
                    var item = data.list[i];
                    $(target).append("<option value=\"" + item["id"] + "\">" + item["size"] + "</option>")
                }
            }
        });
    });

});

function checkSteps() {
    if ($("#txtStoneCount").val() > 0) {
        var stoneCount = parseInt($("#txtStoneCount").val());
        for (var i = 0; i < stoneCount; i++) {
            if ($(".stone-list[data-order=" + (i + 1) + "]").length == 0) {
                var html = $("#stoneTemplate").html();
                html = html.replaceAll("{{order}}", i + 1);
                $("#stoneContent").append(html);
            }
        }

        $(".stone-list").each(function (index, element) {
            if ($(this).attr("data-order") > stoneCount) {
                $(this).remove();
            }
        });

        $("#headerStep2").removeClass("disabled")
    }
    else {
        $("#stoneContent").empty();
        $("#headerStep2").addClass("disabled")
    }

    if ($("#txtLeatherCount").val() > 0) {
        var leatherCount = parseInt($("#txtLeatherCount").val());
        for (var i = 0; i < leatherCount; i++) {
            if ($(".leather-list[data-order=" + (i + 1) + "]").length == 0) {
                var html = $("#leatherTemplate").html();
                html = html.replaceAll("{{order}}", i + 1);
                $("#leatherContent").append(html);
            }
        }

        $(".leather-list").each(function (index, element) {
            if ($(this).attr("data-order") > leatherCount) {
                $(this).remove();
            }
        });

        $("#headerStep3").removeClass("disabled")
    }
    else {
        $("#leatherContent").empty();
        $("#headerStep3").addClass("disabled")
    }

    $("#headerStep4").removeClass("disabled");

    $('[data-toggle="tooltip"]').tooltip();
}

function addFile(type, fileName) {
    $(".no-image").hide();
    $(".table-image").removeClass("hidden");

    var fileTypeTitle = "";
    switch (parseInt(type)) {
        case 0:
            fileTypeTitle = "زمینه سفید";
            break;
        case 1:
            fileTypeTitle = "سفارش";
            break;
        case 2:
            fileTypeTitle = "ربات و سایت";
            break;
        case 3:
            fileTypeTitle = "سایر";
            break;
        case 4:
            fileTypeTitle = "مدل";
            break;
        case 5:
            fileTypeTitle = "دوربین";
            break;
    }

    var row =
        "<tr class=\"file-list\" data-file-name=\"{{fileName}}\" data-file-type=\"{{fileType}}\">" +
        "<td class=\"hidden-xs\">{{fileTypeTitle}}</td>" +
        "<td> <img src=\"/image/product/500x500/{{fileName}}\" style=\"max-width:100px;height:100px;\" \></td>" +
        "<td class=\"txt-al-c\">" +
        "<select id=\"cmbPaddingImg\" class=\"form-control\"><option value=\"\">...</option><option value=\"0\">0</option><option value=\"12.5\">12.5</option><option value=\"25\">25</option><option value=\"37.5\">37.5</option><option value=\"50\">50</option><option value=\"62.5\">62.5</option><option value=\"75\">75</option><option value=\"87.5\">87.5</option><option value=\"100\">100</option></select>" +
        "</td>" +
        "<td class=\"txt-al-c\">" +
        "<button class=\"delete-file btn btn-danger btn-xs\" data-title=\"حذف\"><i class=\"fa fa-times\" aria-hidden=\"true\"></i></button>" +
        "</td>" +
        "</tr>";

    row = row.replaceAll("{{fileName}}", fileName).replaceAll("{{fileType}}", type).replaceAll("{{fileTypeTitle}}", fileTypeTitle)

    if (type == 1) {
        $("#orderFileName").attr("src", "/upload/product/" + fileName).show();
    }

    $(".table-body-image").append(row);
}

function load() {
    if (id > 0) {
        loader(true);
        $.get("/product/load/" + id, function (response) {
            loader(false);
            if (response.status == 200) {
                setEntity(response.data, "#form");
                if (response.data.productType == 10)
                    $("#formOuterWerkType").slideDown("slow");
                else
                    $("#formOuterWerkType").slideUp("slow");
                if (response.data.productType == 3)
                    $("#cmbRingSizeType").show()
                else
                    $("#cmbRingSizeType").hide()
                if (response.data.productType == 2)
                    $(".earringBack").show()
                else
                    $(".earringBack").hide()
                checkForm("#form");

                if (response.data.workshopId) {
                    var id = response.data.workshopId;
                    console.log(id)
                    loadRemoteSelect("#cmbWorkshopTag", "/product/getWorkshopTag", { id: id }, "id", "value", "...", function () {
                       

                        $("#cmbWorkshopTag").val(response.data.workshopTagId);
                     
                    });
                }
                if (response.data.sizeId) {
                    var params = { id: response.data.sizeId };
                    loadRemoteSelect("#cmbNormalSizeValue", "/size/getSizeValueList", params, "id", "value", "...", function () {
                        $("#cmbNormalSizeValue").val(response.data.normalSizeValueId);
                    });
                }
                if (response.data.goldColor == true) {
                    $("#chkGold").prop("checked", true)
                }
                else {
                    $("#chkGold").prop("checked", false)
                }
                if (response.data.rosegoldColor == true) {
                    $("#chkRosegold").prop("checked", true)
                }
                else {
                    $("#chkRosegold").prop("checked", false)
                }
                if (response.data.whiteColor == true) {
                    $("#chkWhite").prop("checked", true)
                }
                else {
                    $("#chkWhite").prop("checked", false)
                }

                if (response.data.orderableBranchType == true) {
                    $("#chkOrderableBranchType").prop("checked", true)
                }
                else {
                    $("#chkOrderableBranchType").prop("checked", false)
                }
                if (response.data.orderableCoWorkerType == true) {
                    $("#chkOrderableCoWorkerType").prop("checked", true)
                }
                else {
                    $("#chkOrderableCoWorkerType").prop("checked", false)
                }
                if (response.data.orderableSolicitorshipType == true) {
                    $("#chkOrderableSolicitorshipType").prop("checked", true)
                }
                else {
                    $("#chkOrderableSolicitorshipType").prop("checked", false)
                }
                if (response.data.orderableOtherType == true) {
                    $("#chkOrderableOtherType").prop("checked", true)
                }
                else {
                    $("#chkOrderableOtherType").prop("checked", false)
                }
                checkSteps();

                var getList = new Array();
                for (var i = 0; i < response.data.stoneList.length; i++) {
                    var item = response.data.stoneList[i];
                    $("[data-order=" + item.order + "][data-stone-id=" + item.stoneId + "]").addClass("checked");
                    $("[data-order=" + item.order + "][data-stone-id=" + item.stoneId + "]").closest(".stone").find(".default-stone").prop("disabled", false);

                    if (item.defaultStoneId) {
                        $("input[type=radio][data-order=" + item.order + "][value=" + item.defaultStoneId + "]").prop("checked", true);
                    }

                    $(".shape[data-order=" + item.order + "]").val(item.stoneShape);

                    if (getList.indexOf(item.order) < 0) {
                        getList.push(item.order);
                        var params = { shape: item.stoneShape, order: item.order, selected: item.shapeSizeId };
                        loader(true);
                        $.get("/shapeSize/getAllSize", params, function (response) {
                            loader(false);
                            var target = $(".shape-size[data-order=" + response.data.order + "]");
                            $(target).empty();
                            if (response.status == 200) {
                                var data = response.data;
                                $(target).append("<option value=\"\">...</option>");
                                for (var i = 0; i < data.list.length; i++) {
                                    var item = data.list[i];
                                    $(target).append("<option value=\"" + item["id"] + "\">" + item["size"] + "</option>");
                                }

                                if (response.data.selected) {
                                    $(target).val(response.data.selected);
                                }
                            }
                        });
                    }
                }

                for (var i = 0; i < response.data.leatherList.length; i++) {
                    var item = response.data.leatherList[i];
                    $("[data-order=" + item.order + "][data-leather-id=" + item.leatherId + "]").addClass("checked");
                }

                for (var i = 0; i < response.data.fileList.length; i++) {
                    var item = response.data.fileList[i];
                    addFile(item.fileType, item.fileName);
                    $(".table-body-image tr:last-child #cmbPaddingImg").val(item.paddingImg);
                }
            }
            else {
                callbackAlert(response.message, function () {
                    document.location = "/product";
                });
            }
        });
    }
}
