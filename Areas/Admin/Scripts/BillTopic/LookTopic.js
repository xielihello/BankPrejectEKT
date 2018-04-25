$(function () {
    var Id = getQueryString('ID');
    var PaperId = getQueryString('PaperId');
    var type = getQueryString('type');
    if (type == "1") {
        $("#Paper").show();
        $("#Topic").hide();
        $("#EditPaper").hide();
    }else if (type == "2") {
        $("#Paper").hide();
        $("#Topic").hide();
        $("#EditPaper").show();
        $("#EditPaper").attr("href", "/Admin/BillTestPaper/EditTestPaper?ID=" + PaperId + "");
    }
    else {
        $("#Paper").hide();
        $("#Topic").show();
        $("#EditPaper").hide();
    }
    var FormId = getQueryString('FormId');
    DataFor(Id, FormId);
})

function DataFor(TopicId, FormId) {
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
            Data(TopicId, FormId);
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

//去掉所有空格
function Trim(str, is_global) {
    var result;
    result = str.replace(/(^\s+)|(\s+$)/g, "");
    if (is_global.toLowerCase() == "g") {
        result = result.replace(/\s/g, "");
    }
    return result;
}

function getQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) return unescape(r[2]); return null;
}