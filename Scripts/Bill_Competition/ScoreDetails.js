var erial = 1;
var len = 0;
$(function () {
    len = $("#timli").find("li").length;
    if (len <= 0) {
        $("#paging").hide();
    }
    Pagediv(1)
    // DataFor();
    // bootstrapPaginator()
});

function Pagediv(type) {
    var TopicId = "";
    var FormId = "";
    var ExamId = "";
    var PaperId = "";
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
    $("#timli").find("li").each(function (i) {
        if (erial == (i + 1)) {
            FormId = $(this).attr("FormId");
            TopicId = $(this).attr("TopicId");
            ExamId = $(this).attr("ExamId");
            PaperId = $(this).attr("PaperId");
            UId = $(this).attr("UId");
            var classname = $(this).find("span").attr('class');
            $(this).find("span").removeClass()
            $(this).find("span").addClass(classname + " active");
        } else {
            $(this).find("span").removeClass('active')
        }
    });
    DataFor(TopicId, FormId, ExamId, PaperId, UId)
}

function Topic(obj) {
    $("#timli").find("li").each(function (i) {
        if (obj == (i + 1)) {
            FormId = $(this).attr("FormId");
            TopicId = $(this).attr("TopicId");
            ExamId = $(this).attr("ExamId");
            PaperId = $(this).attr("PaperId");
            UId = $(this).attr("UId");
            var classname = $(this).find("span").attr('class');
            $(this).find("span").removeClass()
            $(this).find("span").addClass(classname + " active");
        } else {
            $(this).find("span").removeClass('active')
        }
    });
    DataFor(TopicId, FormId, ExamId, PaperId, UId)
}

function DataFor(TopicId, FormId, ExamId, PaperId, UId) {
    TopicData(TopicId, FormId, ExamId, PaperId);
    $.ajax({
        type: "POST",
        dataType: "text",
        async: false,
        url: '/Bill_Competition/Bills',
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
            Data(TopicId, FormId, ExamId, PaperId, UId);
        }
    });
}

function Data(TopicId, FormId, ExamId, PaperId, UId) {
    var Url = "/Bill_Competition/Bill_DataTwo";
    if (getQueryString('UIds') != "0") {
        Url = "/Admin/BillTopic/Bill_DataTwo";
    }
    $.ajax({
        type: "POST",
        dataType: "json",
        async: false,
        url: Url,
        data: { TopicId: TopicId, FormId: FormId, ExamId: ExamId, PaperId: PaperId, UIds: UId },
        success: function (data) {
            if (data.length > 0) {
                for (var i = 0; i < data.length; i++) {
                    $("#" + data[i]["EnglishName"] + "").val(data[i]["OperationAnswer"]);
                }
            }
        }
    });
}

function TopicData(TopicId, FormId, ExamId, PaperId) {
    $.ajax({
        type: "POST",
        dataType: "json",
        url: '/Bill_Competition/ScoreDetailsTwo',
        async: false,
        data: { TopicId: TopicId, FormId: FormId, ExamId: ExamId, PaperId: PaperId },
        success: function (data) {
            if (data.length > 0) {
                $("#fombody").find("div").remove();
                $("#fombody").removeClass();
                $("#fombody").addClass(data[0]["BillStyle"])
                $("#SingleScore").html(data[0]["SingleScore"]);
                $("#TaskExplan").html(HTMLDecode(data[0]["TaskExplan"]));
                $("#TopicTitle").html(erial + "." +data[0]["TopicTitle"]);
                $("#TopicId").val(TopicId);
            }
        }
    });
}

function OperationDetails() {
    var Uids = getQueryString('Uids');
    $("#Add").show();
    $.ajax({
        type: "POST",
        dataType: "json",
        async: false,
        url: '/Bill_Competition/OperationDetails',
        data: { TopicId: $("#TopicId").val(), PaperId: $("#PaperId").val(), ExamId: $("#ExamId").val(), Uids: Uids },
        success: function (data) {
            $("#table").html(data);
            layer.open({
                title: '操作明细',
                type: 1,
                skin: 'layui-layer-lan', //样式类名
                anim: 2,
                shadeClose: true, //开启遮罩关闭
                area: ['720px', '450px'], //宽高
                content: $("#Add")
            });
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
