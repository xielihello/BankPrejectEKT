
var erial = 1;
var len = 0;
$(function () {
    $("#ExamId").val(getQueryString('ExamId'));
    $("#PaperId").val(getQueryString('PaperId'));
    RemainingTime();
    DataBind();
    len = $("#topic_number").find("li").length;
    if (len <= 0) {
        $("#fy").hide();
    }
    yidicijiaz()
});

function yidicijiaz() {
    var ExamId = getQueryString('ExamId');
    var PaperId = getQueryString('PaperId');
    var model = null;
    var models = null;
    $("#topic_number").find('li').each(function (i) {
        var t = $(this).attr("data-type")
        if (t == "0") {
            TopicId = $(this).attr("topicid");
            FormId = $(this).attr("formid");
            model = $(this);
            return false;
        }
        if (i == 0) {
            TopicId = $(this).attr("topicid");
            FormId = $(this).attr("formid");
            models = $(this);
        }
    });
    if (model == null) {
        model = models;
    }
    color_sure(model, 0, 0, false);
    SingleTopic(ExamId, PaperId, TopicId, FormId)
}

function Pagediv(type) {
    var ExamId = getQueryString('ExamId');
    var PaperId = getQueryString('PaperId');
    var FormId = "";
    var TopicId = "";
    if (type == 2) {
        if ((parseInt(erial) - 1) < 1) {
            layer.msg('没有上一题了！', function () { });
            return
        }
    }
    if (type == 3) {
        if ((parseInt(erial) + 1) > len) {
            layer.msg('没有下一题了！', function () { });
            return
        }
    }
    var models = null;
    var in_sum = 0;
    var lenT = $("#fombody").find("div").length;
    $("#fombody").find("div").each(function (i) {
        if ($("#Bills" + i).val() == "" || $("#Bills" + i).val() == undefined) {
            in_sum = parseInt(in_sum) + 1;
        } else {
            in_sum = parseInt(in_sum) - 1;
        }
    });
    $("#topic_number").find("li").each(function (i) {
        TopicId = $(this).attr("topicid");
        FormId = $(this).attr("formid");
        if (type == 1) {
            var obj = $("#topic_number").find("li").eq(erial-1);
            $("#TopicId").val($(obj).attr('topicid'));
            $("#FormId").val($(obj).attr('formid'));
            erial = 1;
            models = $(this);
            return false;
        } else if (type == 2) { //上一页
            var t = $(this).attr("data-erial")
            if (erial > 1 && (parseInt(erial) - 1) == t) {
                var obj = $("#topic_number").find("li").eq(erial - 1);
                $("#TopicId").val($(obj).attr('topicid'));
                $("#FormId").val($(obj).attr('formid'));
                erial = erial - 1;
                models = $(this);
                return false;
            }

        } else if (type == 3) { //下一页
            var t = $(this).attr("data-erial")
            if (erial < len && (parseInt(erial) + 1) == t) {
                var obj = $("#topic_number").find("li").eq(erial - 1);
                $("#TopicId").val($(obj).attr('topicid'));
                $("#FormId").val($(obj).attr('formid'));
                erial = erial + 1;
                models = $(this);
                return false;
            }

        } else if (type == 4) {//尾一页
            var t = $(this).attr("data-erial");
            if (len == t) {
                var obj = $("#topic_number").find("li").eq(erial - 1);
                $("#TopicId").val($(obj).attr('topicid'));
                $("#FormId").val($(obj).attr('formid'));
                erial = len;
                models = $(this);
                return false;
            }
        }
    });
    if (lenT == in_sum) {
        color_sure(models, 1, 0, true);
    } else {
        color_sure(models, 1, 0, false);
    }
    SingleTopic(ExamId, PaperId, TopicId, FormId);
}

var objli = null;
var FormIdtwo = null;
var TopicIdtwo = null;
function color_sure(obj, ts, ists, pd) {
    if (ts != 0) {
        $(objli).find("span").removeClass("color_no")
        $(objli).find("span").removeClass("color_now")
        $(objli).find("span").addClass("color_sure")
        if (pd == true) {
            $(objli).find("span").addClass("color_no")
        }
        if (pd == false) {
            //保存答案
            Save();
        }
    }
    if (ists == 0) {
        objli = obj;
        $(obj).find("span").removeClass("color_no")
        $(obj).find("span").addClass("color_now")
        erial = $(obj).attr("data-erial");
    }
}

function RemainingTime() {
    var ExamId = $("#ExamId").val();
    var PaperId = $("#PaperId").val();
    $.ajax({
        type: "POST",
        dataType: "json",
        url: '/Bill_Competition/ExamRemainingTime',
        data: { ExamId: ExamId, PaperId: PaperId },
        async: false,
        success: function (data) {
            if (data == "1") {
                window.location.href = "/Bill_Competition/RemainingTime?ExamId=" + ExamId + "&PaperId=" + PaperId;
                return false;
            }
        }
    });
}

//题目数量
function DataBind() {
    var ExamId = $("#ExamId").val();
    var PaperId = $("#PaperId").val();
    $.ajax({
        type: "POST",
        dataType: "json",
        url: '/Bill_Competition/DataBind',
        data: { ExamId: ExamId, PaperId: PaperId },
        async: false,
        success: function (data) {
            $("#topic_number").html(data[0]["str"]);
            SumTime(data[0]["Second"]);
            timer(data[0]["Second"]);

        }
    });
}

//具体内容
function SingleTopic(ExamId, PaperId, TopicId, FormId) {
    $.ajax({
        type: "POST",
        dataType: "json",
        url: '/Bill_Competition/SingleTopic',
        data: { ExamId: ExamId, PaperId: PaperId, TopicId: TopicId },
        async: false,
        success: function (data) {
            $("#SingleScore").html(data[0]["SingleScore"]);
            $("#Topic_info").html(HTMLDecode(data[0]["TaskExplan"]));
            $("#fombody").find("div").remove();
            $("#fombody").removeClass();
            $("#fombody").addClass(data[0]["BillStyle"]);
            $("#TopicTitle").html(erial + "." + data[0]["TopicTitle"]);
            DataFor(ExamId, PaperId, TopicId, FormId);
        }
    });
}

function DataFor(ExamId, PaperId, TopicId, FormId) {
    $.ajax({
        type: "POST",
        dataType: "text",
        async: false,
        url: '/Bill_Competition/Bills',
        data: { FormId: FormId },
        success: function (data) {
            var jsond = eval("(" + Trim(data, "g") + ")"); //转义json
            for (var i = 0; i < jsond.length; i++) { //循环添加div  onpaste="return false"
                var divObj = $('<div> <input class="bills_input" autocomplete="off" type="text" id="Bills' + i + '" name="Bills' + i + '" value=\'\'  /></div>'); //定义一个div
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
            Data(ExamId, PaperId, TopicId, FormId)
        }
    });
}

function Data(ExamId, PaperId, TopicId, FormId) {
    $.ajax({
        type: "POST",
        dataType: "json",
        async: false,
        url: '/Bill_Competition/Bill_Data',
        data: { TopicId: TopicId, FormId: FormId, ExamId: ExamId, PaperId: PaperId },
        success: function (data) {
            if (data.length > 0) {
                for (var i = 0; i < data.length; i++) {
                    $("#" + data[i]["EnglishName"] + "").val(data[i]["OperationAnswer"]);
                }
            }
        }
    });
}

//保存
function Save() {
    $.ajax({
        cache: true,
        type: "POST",
        url: '/Bill_Competition/SingleTopicSave',
        data: $('#Examination').serialize(),// 你的formid
        async: false,
        success: function (data) {
            if (data > 0) {
            } else {
                layer.msg('操作失败！', { icon: 2 });
            }
        }
    });
}

//退出
function SignOut() {
    layer.confirm('您的成绩将不会被记录，是否确认退出？', {
        title: '系统提示',
        btn: ['确定', '取消'],
        shadeClose: true, //开启遮罩关闭
        skin: 'layui-layer-lan'
        //按钮
    },
    function () {
        $.ajax({
            type: "POST",
            dataType: "json",
            async: false,
            url: '/Bill_Competition/SignOut',
            data: { ExamId: $("#ExamId").val(), PaperId: $("#PaperId").val() },
            success: function (data) {
                window.close();
            }
        });
    });
}

//交卷
function Assignment() {
    var i = 1
    var html = $(".color_now").parent();
    $("#topic_number").find('li').each(function (i) {
        i = parseInt(i + 1);
        var erial = $(html).attr('data-erial');
        if (erial == i) {
            models = $(this);
            return false;
        }
    });
    $("#TopicId").val($(html).attr('topicid'));
    $("#FormId").val($(html).attr('formid'));
    layer.confirm('您的成绩将被记录，是否确认提交试卷？', {
        title: '系统提示',
        btn: ['确定', '取消'],
        shadeClose: true, //开启遮罩关闭
        skin: 'layui-layer-lan'
        //按钮
    },
   function () {
       var in_sum = 0;
       var len = $("#fombody").find("div").length;
       $("#fombody").find("div").each(function (i) {
           if ($("#Bills" + i).val() == "" || $("#Bills" + i).val() == undefined) {
               in_sum = parseInt(in_sum) + 1;
           } else {
               in_sum = parseInt(in_sum) - 1;
           }
       });
       if (len == parseInt(in_sum)) {
           color_sure(models, 0, 0, true);
           Jump();
       }
       if (len != parseInt(in_sum)) {
           color_sure(models, 0, 0, false);
           SingleTopicSave()
       }
       
   });
}

//倒计时到了提交
function SingleTopicSave() {
    $.ajax({
        cache: true,
        type: "POST",
        url: '/Bill_Competition/SingleTopicSave',
        data: $('#Examination').serialize(),
        async: false,
        success: function (data) {
            if (data > 0) {
                Jump();
            } else {
                layer.msg('操作失败！', { icon: 2 });
            }
        }
    });
}

//成绩显示页面
function Jump() {
    $.ajax({
        cache: true,
        type: "POST",
        url: '/Bill_Competition/Jump',
        data: { ExamId: $("#ExamId").val(), PaperId: $("#PaperId").val(), TopicId: $("#TopicId").val(), Time: $("#Time").html(), SumTime: $("#SumTime").val() },// 你的formid
        async: false,
        success: function (data) {
            //练习
            if (data == "1") {
                showScore();// window.location.href = "/Bill_Competition/ScoreDetails?ExamId=" + $("#ExamId").val() + "&PaperId=" + $("#PaperId").val() + "&UIds=0&Type=4";
            }
                //考试
            else {
                window.location.href = "/HB_kaoshi/GetScoresIndex?Pid=" + $("#PaperId").val() + "&Eid=" + $("#ExamId").val() + "&Type=4";
            }
        }
    });
}

//查案试卷事件
function ckjx() {
    window.open("/Bill_Competition/ScoreDetails?ExamId=" + $("#ExamId").val() + "&PaperId=" + $("#PaperId").val() + "&UIds=0&Type=4");
}

//关闭
function closegb() {
    window.close();
}
//查看分数
function showScore() {
    layer.open({
        type: 1,
        title: '考试详情',
        skin: 'layui-layer-lan', //样式类名
        anim: 2,
        shadeClose: false, //开启遮罩关闭
        area: ['420px', '250px'], //宽高
        content: $("#Edit"),
        end: function () {
            closegb();//关闭
        }
    });
    //获取本次做题成绩分数
    $.ajax({
        url: "/Bill_Competition/GetExaminationResultBys",
        data: { "Pid": $("#PaperId").val(), "Eid": $("#ExamId").val() },
        type: 'POST',
        async: false,
        dataType: 'json',
        success: function (data) {
            var ER_Score = 0;
            if (data != null && data.length > 0) {
                if (!IsNullOrEmpty(data[0]["ER_Score"])) {
                    if (isMoney(data[0]["ER_Score"], 0)) {
                        ER_Score = parseFloat(data[0]["ER_Score"]);
                    }
                }
            }
            $("#bcreuslt").html(parseFloat(ER_Score).toFixed(2));

        }
    });


}

//单个
function Topic(obj) {
    var ExamId = getQueryString('ExamId');
    var PaperId = getQueryString('PaperId');
    var in_sum = 0;
    var len = $("#fombody").find("div").length;
    $("#fombody").find("div").each(function (i) {
        if ($("#Bills" + i).val() == "" || $("#Bills" + i).val() == undefined) {
            in_sum = parseInt(in_sum) + 1;
        } else {
            in_sum = parseInt(in_sum) - 1;
        }
    });
    $("#timli").find("li").each(function (i) {
        if (obj == (i + 1)) {
            var html = $(".color_now").parent();
            $("#TopicId").val($(html).attr('topicid'));
            $("#FormId").val($(html).attr('formid'));
            TopicId = $(this).attr("topicid");
            FormId = $(this).attr("formid");
            models = $(this);
            if (len == parseInt(in_sum)) {
                color_sure(models, 1, 0, true);
            }
            if (len != parseInt(in_sum)) {
                color_sure(models, 1, 0, false);
            }
            SingleTopic(ExamId, PaperId, TopicId, FormId)
        }
    });
}

var intDiff;//倒计时总秒数量
var TimeOut;
//倒计时
function timer(intDiff) {
    TimeOut = window.setInterval(function () {
        var day = 0,
            hour = 0,
            minute = 0,
            second = 0;//时间默认值		
        if (intDiff > 0) {
            day = Math.floor(intDiff / (60 * 60 * 24));
            hour = Math.floor(intDiff / (60 * 60)) - (day * 24);
            minute = Math.floor(intDiff / 60) - (day * 24 * 60) - (hour * 60);
            second = Math.floor(intDiff) - (day * 24 * 60 * 60) - (hour * 60 * 60) - (minute * 60);
            if (hour <= 9) hour = '0' + hour;
            if (minute <= 9) minute = '0' + minute;
            if (second <= 9) second = '0' + second;
            //$('#day_show').html(day + "天");
            //$('#hour_show').html('<s id="h"></s>' + hour + '时');
            //$('#minute_show').html('<s></s>' + minute + '分');
            //$('#second_show').html('<s></s>' + second + '秒');

            //$("#OpenId").hide();
            //$("#Submit").show();
            //$("#hall_manager").removeClass("part_one part_col_gray");
            //$("#hall_manager").addClass("part_one part_col_bule");
            //$("#imgid").hide();
            //$("#iframeId").show();
            //$('#iframeId').attr('src', 'index_detailed.aspx');//

            $("#Time").html(hour + ":" + minute + ":" + second);

        } else {
            var html = $(".color_now").parent();
            $("#TopicId").val($(html).attr('topicid'));
            $("#FormId").val($(html).attr('formid'));
            clearInterval(TimeOut);
            $("#Time").html("00:00:00");
            SingleTopicSave();
            //window.location.href = "Score.aspx?ExamId=" + getQueryString('ExamId') + "&TaskId=" + TaskId;
        }
        intDiff--;
    }, 1000);
}

function getQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) return unescape(r[2]); return null;
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

//总时间
function SumTime(intDiff) {
    var day = 0,
         hour = 0,
         minute = 0,
         second = 0;//时间默认值		
    if (intDiff > 0) {
        day = Math.floor(intDiff / (60 * 60 * 24));
        hour = Math.floor(intDiff / (60 * 60)) - (day * 24);
        minute = Math.floor(intDiff / 60) - (day * 24 * 60) - (hour * 60);
        second = Math.floor(intDiff) - (day * 24 * 60 * 60) - (hour * 60 * 60) - (minute * 60);
        if (hour <= 9) hour = '0' + hour;
        if (minute <= 9) minute = '0' + minute;
        if (second <= 9) second = '0' + second;
        //$('#day_show').html(day + "天");
        //$('#hour_show').html('<s id="h"></s>' + hour + '时');
        //$('#minute_show').html('<s></s>' + minute + '分');
        //$('#second_show').html('<s></s>' + second + '秒');

        //$("#OpenId").hide();
        //$("#Submit").show();
        //$("#hall_manager").removeClass("part_one part_col_gray");
        //$("#hall_manager").addClass("part_one part_col_bule");
        //$("#imgid").hide();
        //$("#iframeId").show();
        //$('#iframeId').attr('src', 'index_detailed.aspx');//
        $("#SumTime").val(hour + ":" + minute + ":" + second);
    }
}
