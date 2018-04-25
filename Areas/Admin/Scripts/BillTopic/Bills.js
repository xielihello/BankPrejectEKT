//单据位置控制js
$(function () {
    $("#TopicId").val($('#TopicId', parent.document).val());
    $("#Title").val($('#Title', parent.document).val())
    var Summernote = HTMLEncode($('#Summernote', parent.document).val());
    $("#Summernote").val(Summernote);
    DataFor($('#TopicId', parent.document).val());
    Data($('#TopicId', parent.document).val(), $("#FormId").val());
})

function DataFor(TopicId) {
    $.ajax({
        type: "POST",
        dataType: "text",
        async: false,
        url: '/Admin/BillTopic/Bills',
        data: { FormId: $("#FormId").val() },
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


//htmlb标签 转义
function HTMLEncode(html) {
    var temp = document.createElement("div");
    (temp.textContent != null) ? (temp.textContent = html) : (temp.innerText = html);
    var output = temp.innerHTML;
    temp = null;
    return output;
}

