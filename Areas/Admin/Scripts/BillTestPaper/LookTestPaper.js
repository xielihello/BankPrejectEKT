
var erial = 1;
var len = 0;
$(function () {
    len = $("#timbt").find("button").length;
    if (len <= 0) {
        $("#fy").hide();
    }
    Pagediv(1);
})

function AjaxGetModel(obj) {
    var EvenId = 0;
    var Title = "";
    var Fraction = 0;
    var FormId = "";
    var BillStyle = "";
    var TopicId = "";

    $("#timbt").find("button").each(function (i) {
        if (obj == (i + 1)) {
            EvenId = $(this).attr("EvenId");
            Title = $(this).attr("Title");
            Fraction = $(this).attr("Fraction");
            FormId = $(this).attr("FormId");
            BillStyle = $(this).attr("BillStyle");
            TopicId = $(this).attr("TopicId");
            $(this).removeClass("btn-outline")
        } else {
            $(this).addClass("btn-outline")
        }
    });
    DataFor(EvenId, Title, Fraction, FormId, BillStyle, TopicId)
}

function Pagediv(type) {
    var EvenId = 0;
    var Title = "";
    var Fraction = 0;
    var FormId = "";
    var BillStyle = "";
    var TopicId = "";
    if (type == 2) { //上一页
        if (erial > 1) {
            erial = erial - 1;
        }

    } else if (type == 3) { //下一页
        if (erial < len) {
            erial = erial + 1;
        }

    } else if (type == 4) {//尾一页
        erial = len;
    } else {
        erial = 1;
    }
    //$(this).parent().next().show();
    $("#timbt").find("button").each(function (i) {
        if (erial == (i + 1)) {
            EvenId = $(this).attr("EvenId");
            Title = $(this).attr("Title");
            Fraction = $(this).attr("Fraction");
            FormId = $(this).attr("FormId");
            BillStyle = $(this).attr("BillStyle");
            TopicId = $(this).attr("TopicId");
            $(this).removeClass("btn-outline")
        } else {
            $(this).addClass("btn-outline")
        }
    });
    DataFor(EvenId, Title, Fraction, FormId, BillStyle, TopicId)
}

function DataFor(EvenId, Title, Fraction, FormId, BillStyle, TopicId) {
    $("#tim").html(Title + " (" + Fraction + ")");
    $("#fombody").removeClass();
    $("#fombody").find("div").remove();
    $("#fombody").addClass(BillStyle);
    TaskPlan(EvenId);
    $.ajax({
        type: "POST",
        dataType: "text",
        async: false,
        url: '/Admin/BillTopic/Bills',
        data: { FormId: FormId },
        success: function (data) {
            var jsond = eval("(" + Trim(data, "g") + ")"); //转义json
            for (var i = 0; i < jsond.length; i++) { //循环添加div
                var divObj = $('<div> <input class="bills_input" type="text" id="Bills' + i + '" name="Bills' + i + '"  /></div>'); //定义一个div
                divObj.attr({
                    "id": i,
                    "rel": jsond[i].fwTool,
                }).css({ //改变div样式
                    "left": jsond[i].x,
                    "top": jsond[i].y,
                    'position': 'absolute',
                    'width': jsond[i].w,
                    'height': jsond[i].h,
                    'text-align': jsond[i].t,
                    'color': jsond[i].color
                }).appendTo($('#fombody')); //添加一个div
            }
            Data(TopicId, FormId)
        }
    });
}

function Data(TopicId, FormId) {
    if (TopicId != "" && TopicId != undefined) {
        $.ajax({
            type: "POST",
            dataType: "json",
            async: false,
            url: '/Admin/BillTopic/Bill_Data',
            data: { TopicId: TopicId, FormId: FormId },
            success: function (data) {
                if (data.length > 0) {
                    for (var i = 0; i < data.length; i++) {
                        $("#" + data[i]["EnglishName"] + "").val(data[i]["Answer"]);
                    }
                }
            }
        });
    }
}

function TaskPlan(EvenId) {
    $.ajax({
        type: "POST",
        dataType: "json",
        async: false,
        url: '/Admin/BillTestPaper/GetTaskPlan',
        data: { EvenId: EvenId },
        success: function (data) {
            $("#TaskExplan").html(HTMLDecode(data))
            
        }
    });
}

//去掉所有空格
function Trim(str, is_global) {
    var result;
    result = str.replace(/(^\s+)|(\s+$)/g, "");
    if (is_global.toLowerCase() == "g") {
        result = result.replace(/\s/g, "");
    }
    return result;
}