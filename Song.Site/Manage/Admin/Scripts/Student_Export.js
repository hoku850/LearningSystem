﻿$(function () {
    //全选反选取消的事件
    $(".selectBtn a").click(function () {
        var cl = $(this).attr("class");
        var checkbox = $("*[id$=cblSort] input[type=checkbox]");
        checkbox.each(function (index, element) {
            if (cl == "all") $(this).attr("checked", true);
            if (cl == "invert") $(this).attr("checked", !$(this).attr("checked"));
            if (cl == "cancel") $(this).attr("checked", false);
        });
        return false;
    });
    //	导出详情的按钮
    $("input[name$=btnExportDetails]").click(function () {
        $(this).attr("disabled", "disabled");
        var cbl = $("span[id$=cblSort]");
        //选择的学员班组（或叫组）
        var ids = "";
        cbl.find("input[type=checkbox]").each(function () {
            if ($(this).attr("checked")) {
                ids += $(this).val() + ",";
            }
        });
        var iframe = $("#iframeExportDetails");
        iframe.attr("src", iframe.attr("link") + "?sts=" + ids);
        return false;
    });
    //主管理区加载完成事件
    $("#iframeExportDetails").load(function () {
        var iframe = $(this)[0];
        var bHeight = iframe.contentWindow.document.body.scrollHeight;
        var dHeight = iframe.contentWindow.document.documentElement.scrollHeight;
        var height = Math.max(bHeight, dHeight);
        iframe.height = height;
        //
        var frameWindow = $(this)[0].contentWindow;
        frameWindow.close();
        frameWindow.focus();
        frameWindow.print();
        $("input[name$=btnExportDetails]").removeAttr("disabled");
    });
});
