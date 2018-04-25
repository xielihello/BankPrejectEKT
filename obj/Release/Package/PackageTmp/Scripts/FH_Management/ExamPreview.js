

function getQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) return unescape(r[2]); return null;
}
var timeExam;
var currentTime;
var startTestTime;
var AddStartDateTime;
var E_StartTime;
var E_EndTime;

var Pid = "";
var Eid = "";

//var testType = -1;
$(function () {
    Eid = getQueryString('Eid');
    Pid = getQueryString('Pid');

    currentTime = stringToDate($("#CurrentTime").val());//当前时间
    AddStartDateTime = stringToDate($("#AddStartDateTime").val());//当前进入时间 算开始 开始时间
    startTestTime = stringToDate($("#TestStartDateTime").val()); //考试结束时间结束时间
    E_StartTime = stringToDate($("#E_StartTime").val());//有效开始时间
    E_EndTime = stringToDate($("#E_EndTime").val());//有效结束时间

    $.ajax({
        url: '/ashx/TimeHandler.ashx',

        success: function (data) {

            if (data.length > 16) {
                $("#CurrentTime").val(data);
                currentTime = stringToDate(data);

            }
        }
    });

    // currentTime当前时间 
    if (E_StartTime > currentTime) {//如果有效开始时间大于当前时间 那么 还是倒计时页面
        $("#MinDiv").css('display', 'none');
        var bodyWidth = document.documentElement.clientWidth;
        var bodyHeight = Math.max(document.documentElement.clientHeight, document.body.scrollHeight);

        $("<div class='wrapTime' style='width:100%;height:100%' ><iframe src='/FH_Management/TimeIndex?Eid=" + Eid + "&Pid=" + Pid + "' id='ifmhtml' style='width:" + bodyWidth + "px; height:" + bodyHeight + "px' scrolling='no'></iframe></div>").appendTo("body");
        //$(".wrapTime").width(bodyWidth);
        //$(".wrapTime").height(bodyHeight);

    } else {
        $("#MinDiv").css('display', 'blcok');
    }

    //如果没有提交 当前时间大于学生实际剩余做题时间
    if (currentTime > startTestTime) {
        //数据提交
        Add();
    }



    timeExam = setInterval("Time()", 1000);

    yidicijiaz()//第一次加载
});
var timeEnd = 1;//全局倒计时是否结束
var Mtime = 1;

//倒计时
function Time() {


    //当前时间应该大于有效开始时间 去除阴影
    if (currentTime >= E_StartTime && Mtime == 1) {
        $(".wrapTime").remove();
        this.document.body.scrollTop = 0;
        $("#MinDiv").css('display', 'block');

        $.ajax({
            Type: "post",
            dataType: "json",
            url: '/HB_kaoshi/UpDateTime?Eid=' + Eid + '&Pid=' + Pid + '&Type=3',
            async: false,
            success: function (data) {

            }
        })

        Mtime = 0;
    }
    var leaveSecond = (startTestTime - currentTime) / 1000;//秒

    if (leaveSecond <= 0) {
        timeEnd = 0;
        $("#daojishi").html('00:00:00');
        clearInterval(timeExam);
        btnOk();

    }
    else {

        var theTime = parseInt(leaveSecond);// 秒
        var theTime1 = 0;// 分
        var theTime2 = 0;// 小时
        if (theTime > 60) {
            theTime1 = parseInt(theTime / 60);
            theTime = parseInt(theTime % 60);
            if (theTime1 > 60) {
                theTime2 = parseInt(theTime1 / 60);
                theTime1 = parseInt(theTime1 % 60);
            }
        }
        var result = "" + parseInt(theTime);//秒
        if (theTime1 > 0) {
            result = "" + parseInt(theTime1) + "分" + result;
        }
        if (theTime2 > 0) {
            result = "" + parseInt(theTime2) + "小时" + result;
        }

        var ss = theTime;//秒
        if (ss < 10) {
            ss = "0" + ss;
        }
        var mm = theTime1;//分钟
        if (mm < 10) {
            mm = "0" + mm;
        }
        var hh = theTime2;
        if (hh < 10) {
            hh = "0" + hh;
        }

        document.getElementById("daojishi").innerText = hh + ":" + mm + ":" + ss;

        if (leaveSecond == 1200) {
            layer.msg('本次考试剩余时间为20分钟，请考生把握好做题进度！', { time: 2500 });
        }

    }
    currentTime.setSeconds(currentTime.getSeconds() + 1);

    var leaveYu = leaveSecond % 10;
    if (leaveYu == 0) {
        // ----------------异步获取时间-------------------------
        $.ajax({
            url: '/ashx/TimeHandler.ashx',

            success: function (data) {

                if (data.length > 16) {
                    $("#CurrentTime").val(data);
                    currentTime = stringToDate(data);

                }
            }
        });
        // ----------------异步获取时间--------------------------
    }

}

var data_id = 0;
var topicid = 0;
var testpaperid = 0;
var Title = "";
var erial = 1;//默认页
function dianjishij(type) {
    var models = null;
    var len = $("#thdiv").find('.num_sty').length;//总页数

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
    $("#thdiv").find('.num_sty').each(function (i) {
        
        if (type == 1 && i == 0) {
            models = $(this);
            erial = 1;
            return false;
        }
        if (type == 2) {//上一页
            var t = $(this).attr("data-erial")
            if (erial > 1 && (parseInt(erial) - 1) == t) {
                models = $(this);
                erial = parseInt(erial) - 1
                return false;
            }
        }
        if (type == 3) {//下一页
            var t = $(this).attr("data-erial")
            if (erial < len && (parseInt(erial) + 1) == t) {
                models = $(this);
                erial = parseInt(erial) + 1
                return false;
            }
        }
        if (type == 4 && (i + 1) == len) {
            erial = len;
            models = $(this);
            return false;
        }
    });
   
    color_sure(models, 1,0);
}
function yidicijiaz() {
    var model = null;
    var models = null;
    $("#thdiv").find('.num_sty').each(function (i) {
        var t = $(this).attr("data-type")
        if (t == "0") {
            model = $(this);
            return false;
        }
        if(i==0){
            models = $(this);
        }
    });
    if (model == null) {
        model = models;
    }
    //$(model).find("span").removeClass("color_no")//未
    //$(model).find("span").addClass("color_now")  //当前            color_sure//已
    
    color_sure(model, 0, 0);

}
var objli = null;
var btnok = false;
//选择题
function color_sure(obj, ts,ists) {
    if (ts != 0) {

        var boo = false;
        var list = [];
        var inputisnull = true;
        $("#tb_tbodys").find("tr").each(function () {
            var Istrue = $(this).find("td").eq(0).attr("class");
            if (Istrue == "true") {
                var m = {};
                m.Line = $(this).find("td").eq(0).attr("data-lind")//行
                if (jsIsture($(this).find("#" + m.Line + "qm")) || jsIsture($(this).find("#" + m.Line + "qc"))) {
                    boo = true;
                    return;
                }
                m.Final = jsvalues($(this).find("#" + m.Line + "qm"));//期末
                m.Beginning = jsvalues($(this).find("#" + m.Line + "qc"));//期初  
                if (m.Final > 0 || m.Beginning > 0) {
                    inputisnull = false;
                }
                m.Type = 0;
                list.push(m);
                 
            }
            Istrue = $(this).find("td").eq(4).attr("class");
            if (Istrue == "true") {
                var m = {};
                m.Line = $(this).find("td").eq(4).attr("data-lind")//行
                if (jsIsture($(this).find("#" + m.Line + "qm")) || jsIsture($(this).find("#" + m.Line + "qc"))) {
                    boo = true;
                    return;
                }
                m.Final = jsvalues($(this).find("#" + m.Line + "qm"));//期末
                m.Beginning = jsvalues($(this).find("#" + m.Line + "qc"));//期初  
                if (m.Final > 0 || m.Beginning > 0) {
                inputisnull = false;
                }
                m.Type = 1;
                list.push(m);
            }

        });
        if (boo) {
            layer.msg('请输入正确的值！', function () { });
            return;
        }
        if (ists == 0) {
            btnok = false;
        }
        if (!inputisnull) {
            
            if (!btnok) {
                $(objli).find("span").removeClass("color_no")
                $(objli).find("span").removeClass("color_now")
                $(objli).find("span").addClass("color_sure")
            }
            BtnSubim(Eid, Pid, topicid, list);
        } else {
            if (!btnok) {
                $(objli).find("span").removeClass("color_no")
                $(objli).find("span").removeClass("color_now")
                $(objli).find("span").addClass("no_bt")
            }

        }
    }
    if (ists == 0) {
        inputnull();//清除

        objli = obj;
        $(obj).attr("data-type", "3");

        $(obj).find("span").removeClass("color_no")
        $(obj).find("span").addClass("color_now")

        topicid = $(obj).attr("data-topicid");
        erial = $(obj).attr("data-erial");
        testpaperid = $(obj).attr("data-testpaperid");
        data_id = $(obj).attr("data-id");
        var testsfraction = $(obj).attr("data-testsfraction");
        var type = $(obj).attr("data-type");
        Title = $(obj).attr("data-title");
        $("#title").html(erial + "." + Title);
        $("#score").html(testsfraction)
        GetModelTestsId();//加载题目
        GetModelById();
    }
}


//保存当前答案
function BtnSubim(Eid, Pid, topicid,list) {
     
   
    var tb = list;


    //表单提交
    $.ajax({
        type: "post",
        url: '/FH_Management/AddExamination',
        data: { entityList: JSON.stringify(list), Eid: Eid, Pid: Pid, QBId: topicid },
        success: function (data) {
            if (data > 0) { 
                $(objli).attr("data-type", "1");
            } else {
                layer.msg('保存答案失败', { icon: 2 });
                return;
            }
        }
    });
}
 
//读取当前题目
function GetModelTestsId() {
    $.ajax({
        Type: "post",
        url: '/FH_Management/GetModelTestsId',
        dataType: "json", cache: false,
        contentType: "application/json; charset=utf-8",
        data: { Id: topicid },
        success: function (data) {
            //debugger
            if (!IsNullOrEmpty(data)) {
                var obj = data;//JSON.parse(data)
                if (obj.length > 0) {
                    var html = '<div class="text_red">题目信息：</div>'
                    //html += Title + "<br />";
                    var sis = 1;
                    for (var i = 0; i < obj.length; i++) {
                        var Assets = obj[i]["Assets"];
                        var Line = obj[i]["Line"];//类型
                        var FinalBalance = obj[i]["FinalBalance"];//答案
                        var BeginningBalance = obj[i]["BeginningBalance"];//答案

                        if (Line != 15 && Line != 29 && Line != 30 && Line != 41 && Line != 46 && Line != 47 && Line != 52 && Line != 53) {
                            if ((MoneyVerification(FinalBalance) + MoneyVerification(BeginningBalance)) > 0) {
                                html += '<span style="width:150px;display: -webkit-inline-box;">' + parseInt(sis) + '.' + obj[i]["Assets"] + "</span> ";
                                html += '期初余额：<span style="width:150px;display: -webkit-inline-box;">' + (BeginningBalance == null ? "" : BeginningBalance) + ";</span> ";
                                html += ' 期末余额：<span style="width:150px;display: -webkit-inline-box;">' + (FinalBalance == null ? "" : FinalBalance) + "; </span> <br />";
                                sis++;
                            }

                        }
                    }
                    $("#divtim").html(html);
                }
            }
        }
    });
}
//读取当前答案
function GetModelById() {
    //return;
    $.ajax({
        type: "post",
        url: '/FH_Management/GetModelById',
        data: { Eid: Eid, Pid: Pid, QBId: topicid },
        success: function (data) { 
            if (!IsNullOrEmpty(data)) {
                var obj = JSON.parse(data)
                if (obj.length > 0) {
                    var html = '<div class="text_red">题目信息：</div>'
                    html += Title + "<br />"; 
                    for (var i = 0; i < obj.length; i++) {
                        var Line = obj[i]["Spare1"];
                        var Spare2 = obj[i]["Spare2"];//类型
                        var RightAnswer = obj[i]["OperationAnswer"];//答案
                        if (Spare2 == "0") {
                            $("#tb_tbodys").find("#" + Line + "qc").val(RightAnswer);
                            $("#tb_tbodys").find("#" + Line + "qc").attr("title", RightAnswer);
                        } else {
                            $("#tb_tbodys").find("#" + Line + "qm").val(RightAnswer);
                            $("#tb_tbodys").find("#" + Line + "qm").attr("title", RightAnswer);
                        } 
                    } 
                }
                Calculation();
            }
        }
    });
}
//清空input
function inputnull() {
    $("#tb_tbodys").find("input").each(function () {
        var tr = $(this).attr("readonly")
        if (tr != "readonly") {
            $(this).val("");
        } else {
            $(this).val("0.00");
        }
    });
}

//总提交按钮
function btnOk() {


    //一种时间到了自动提交
    btnok = true;
    color_sure(objli, 1, 1);
    if (timeEnd == 0) {

        Add();

    } else {
        //一种手动点击提交
        layer.confirm('您的成绩将被记录，是否确认提交试卷？', {
            title: '系统提示',
            btn: ['确定', '取消'],
            shadeClose: true, //开启遮罩关闭
            skin: 'layui-layer-lan'
            //按钮
        }, function () {
            Add();
        });
    }
}

function Add() {
    //不自动关闭 time:-1  亮度页面不可点击 shade:0.01
    layer.msg('试卷正在提交，请稍等......', {
        icon: 16, shade: 0.1, time: -1
    });
    //数据提交
    var dctijiao = $("#E_Type").val();//竞赛模式
    var E_IsTimeBonus = $("#E_IsTimeBonus").val();//是否时间加分
    var P_Score = $("#P_Score").val();//试卷分值
    var E_Whenlong = $("#E_Whenlong").val();//时长
    $.ajax({
        url: "/FH_Management/AddExaminationResult",
        data: { "dctijiao": dctijiao, "Eid": Eid, "Pid": Pid, "E_IsTimeBonus": E_IsTimeBonus, "P_Score": P_Score, "E_Whenlong": E_Whenlong },
        type: 'POST',
        dataType: 'json',
        // async: false,
        success: function (d) {
            //var d = "1";
            if (d == "1") {//交卷成功 

                layer.closeAll();
                //定时器停止
                clearInterval(timeExam);
                $("#daojishi").html('00:00:00');
                if (dctijiao == "2") {//如果是考试模式
                    window.location.href = "/HB_kaoshi/GetScoresIndex?Type=3&Pid=" + Pid + "&Eid=" + Eid;
                    return;
                }
                //练习模式 弹框
                //查看本次练习分数
                showScore();
                //setTimeout('showScore()', 2000);

            }
            if (d == "88") {
                //考试模式下已经提交过 返回到成绩页面
                window.location.href = "/HB_kaoshi/GetScoresIndex?Type=3&Pid=" + Pid + "&Eid=" + Eid;
                return;
            }
            if (d == "99") {
                layer.alert('交卷失败，请刷新或重新登录再试！', { skin: 'layui-layer-lan' }, function () {

                    layer.closeAll();//关闭所有弹出框
                });
            }
        }
    });
}

//查案试卷事件
function ckjx() {
    window.open('/FH_Management/Preview?Eid=' + Eid + '&Pid=' + Pid + '&UIds=0&Type=0');
}

//退出按钮
function tuichukaoshi() {
    layer.confirm('您的成绩将不会被记录，是否确认退出？', {
        title: '系统提示',
        btn: ['确定', '取消'],
        shadeClose: true, //开启遮罩关闭
        skin: 'layui-layer-lan'
        //按钮
    },
   function () {
       //1清掉成绩记录
       $.ajax({
           url: "/FH_Management/DelExamResult",
           data: { "Pid": Pid, "Eid": Eid },
           type: 'POST',
           async: false,
           dataType: 'json',
           success: function (data) {
               if (data == "1") {
                   window.close();
               }
           }
       })

   }
  );
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
        url: "/FH_Management/GetExaminationResultBys",
        data: { "Pid": Pid, "Eid": Eid },
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

//返回值
function jsvalues(obj) {
    var v;
    var value = $(obj).val()
    if (!IsNullOrEmpty(value)) {
        v = value;
    }
    return v;
}
//判断是否金额类型
function jsIsture(obj) {
    var v;
    var value = $(obj).val()
    if (!IsNullOrEmpty(value)) {
        if (!isMoney(value, 0)) {
            $(obj).focus();
            return true;
        }
    }
    return false;
}

function MoneyVerification(money) {
    var velue = 0;
    if (!IsNullOrEmpty(money)) {
        if (isMoney(money, 0)) {
            velue = money;
        }
    }
    return velue;
}
//字符串转时间
function stringToDate(str) {
    var arr = str.split(/\s|-|:/);
    var year = parseInt(arr[0]);
    var month = parseInt(arr[1].charAt(0)) * 10 + parseInt(arr[1].charAt(1));
    var date = parseInt(arr[2].charAt(0)) * 10 + parseInt(arr[2].charAt(1));

    var hour = parseInt(arr[3].charAt(0)) * 10 + parseInt(arr[3].charAt(1));
    var minute = parseInt(arr[4].charAt(0)) * 10 + parseInt(arr[4].charAt(1));
    var second = parseInt(arr[5].charAt(0)) * 10 + parseInt(arr[5].charAt(1));

    return new Date(year, month - 1, date, hour, minute, second);
}


function jsVerification(obj) {
    var money = $(obj).val();
    $(obj).attr("title", money);
    if (!IsNullOrEmpty(money)) {
        if (!isMoney(money, 0)) {
            layer.msg('请输入正确的值！', function () { });
            $(obj).focus();
            return;
        }
    }
    Calculation();
}

function Calculation() {
    /*-------------行15=行1+行2+行3+行4+行5+行6+行7+行8+行9+行14；-------------*/
    var qm1 = MoneyVerification($("#1qm").val());
    var qm2 = MoneyVerification($("#2qm").val());
    var qm3 = MoneyVerification($("#3qm").val());
    var qm4 = MoneyVerification($("#4qm").val());
    var qm5 = MoneyVerification($("#5qm").val());
    var qm6 = MoneyVerification($("#6qm").val());
    var qm7 = MoneyVerification($("#7qm").val());
    var qm8 = MoneyVerification($("#8qm").val());
    var qm9 = MoneyVerification($("#9qm").val());
    var qm14 = MoneyVerification($("#14qm").val());
    var qm15 = (parseFloat(qm1) + parseFloat(qm2) + parseFloat(qm3) + parseFloat(qm4) + parseFloat(qm5) + parseFloat(qm6) + parseFloat(qm7) + parseFloat(qm8) + parseFloat(qm9) + parseFloat(qm14)).toFixed(2);
    //alert(qm15)
    $("#15qm").val(parseFloat(qm15).toFixed(2));
    $("#15qm").attr("title", parseFloat(qm15).toFixed(2))

    var qc1 = MoneyVerification($("#1qc").val());
    var qc2 = MoneyVerification($("#2qc").val());
    var qc3 = MoneyVerification($("#3qc").val());
    var qc4 = MoneyVerification($("#4qc").val());
    var qc5 = MoneyVerification($("#5qc").val());
    var qc6 = MoneyVerification($("#6qc").val());
    var qc7 = MoneyVerification($("#7qc").val());
    var qc8 = MoneyVerification($("#8qc").val());
    var qc9 = MoneyVerification($("#9qc").val());
    var qc14 = MoneyVerification($("#14qc").val());
    var qc15 = parseFloat(qc1) + parseFloat(qc2) + parseFloat(qc3) + parseFloat(qc4) + parseFloat(qc5) + parseFloat(qc6) + parseFloat(qc7) + parseFloat(qc8) + parseFloat(qc9) + parseFloat(qc14);
    $("#15qc").val(qc15.toFixed(2));
    $("#15qc").attr("title", parseFloat(qc15).toFixed(2))


    /*--------------行29=行16+行17+行20+行21+行22+行23+行24+行25+行26 +行27+行28；-------------*/
    var qm16 = MoneyVerification($("#16qm").val());
    var qm17 = MoneyVerification($("#17qm").val());
    var qm20 = MoneyVerification($("#20qm").val());
    var qm21 = MoneyVerification($("#21qm").val());
    var qm22 = MoneyVerification($("#22qm").val());
    var qm23 = MoneyVerification($("#23qm").val());
    var qm24 = MoneyVerification($("#24qm").val());
    var qm25 = MoneyVerification($("#25qm").val());
    var qm26 = MoneyVerification($("#26qm").val());
    var qm27 = MoneyVerification($("#27qm").val());
    var qm28 = MoneyVerification($("#28qm").val());
    var qm29 = parseFloat(qm16) + parseFloat(qm17) + parseFloat(qm20) + parseFloat(qm21) + parseFloat(qm22) + parseFloat(qm23) + parseFloat(qm24) + parseFloat(qm25) + parseFloat(qm26) + parseFloat(qm27) + parseFloat(qm28);
    $("#29qm").val(qm29.toFixed(2));
    $("#29qm").attr("title", parseFloat(qm29).toFixed(2))

    var qc16 = MoneyVerification($("#16qc").val());
    var qc17 = MoneyVerification($("#17qc").val());
    var qc20 = MoneyVerification($("#20qc").val());
    var qc21 = MoneyVerification($("#21qc").val());
    var qc22 = MoneyVerification($("#22qc").val());
    var qc23 = MoneyVerification($("#23qc").val());
    var qc24 = MoneyVerification($("#24qc").val());
    var qc25 = MoneyVerification($("#25qc").val());
    var qc26 = MoneyVerification($("#26qc").val());
    var qc27 = MoneyVerification($("#27qc").val());
    var qc28 = MoneyVerification($("#28qc").val());
    var qc29 = parseFloat(qc16) + parseFloat(qc17) + parseFloat(qc20) + parseFloat(qc21) + parseFloat(qc22) + parseFloat(qc23) + parseFloat(qc24) + parseFloat(qc25) + parseFloat(qc26) + parseFloat(qc27) + parseFloat(qc28);
    $("#29qc").val(qc29.toFixed(2));
    $("#29qc").attr("title", parseFloat(qc29).toFixed(2))

    /*--------------行30=行15+行29；-------------*/
    var qm15 = MoneyVerification($("#15qm").val());
    var qm29 = MoneyVerification($("#29qm").val());
    var qm30 = parseFloat(qm15) + parseFloat(qm29);
    $("#30qm").val(qm30.toFixed(2));
    $("#30qm").attr("title", parseFloat(qm30).toFixed(2))

    var qc15 = MoneyVerification($("#15qc").val());
    var qc29 = MoneyVerification($("#29qc").val());
    var qc30 = parseFloat(qc15) + parseFloat(qc29);
    $("#30qc").val(qc30.toFixed(2));
    $("#30qc").attr("title", parseFloat(qc30).toFixed(2))


    /*--------------行41=行31+行32+行33+行34+行35+行36+行37+行38 +行39+行40；-------------*/
    var qm31 = MoneyVerification($("#31qm").val());
    var qm32 = MoneyVerification($("#32qm").val());
    var qm33 = MoneyVerification($("#33qm").val());
    var qm34 = MoneyVerification($("#34qm").val());
    var qm35 = MoneyVerification($("#35qm").val());
    var qm36 = MoneyVerification($("#36qm").val());
    var qm37 = MoneyVerification($("#37qm").val());
    var qm38 = MoneyVerification($("#38qm").val());
    var qm39 = MoneyVerification($("#39qm").val());
    var qm40 = MoneyVerification($("#40qm").val());
    var qm41 = parseFloat(qm31) + parseFloat(qm32) + parseFloat(qm33) + parseFloat(qm34) + parseFloat(qm35) + parseFloat(qm36) + parseFloat(qm37) + parseFloat(qm38) + parseFloat(qm39) + parseFloat(qm40);
    $("#41qm").val(qm41.toFixed(2));
    $("#41qm").attr("title", parseFloat(qm41).toFixed(2))

    var qc31 = MoneyVerification($("#31qc").val());
    var qc32 = MoneyVerification($("#32qc").val());
    var qc33 = MoneyVerification($("#33qc").val());
    var qc34 = MoneyVerification($("#34qc").val());
    var qc35 = MoneyVerification($("#35qc").val());
    var qc36 = MoneyVerification($("#36qc").val());
    var qc37 = MoneyVerification($("#37qc").val());
    var qc38 = MoneyVerification($("#38qc").val());
    var qc39 = MoneyVerification($("#39qc").val());
    var qc40 = MoneyVerification($("#40qc").val());
    var qc41 = parseFloat(qc31) + parseFloat(qc32) + parseFloat(qc33) + parseFloat(qc34) + parseFloat(qc35) + parseFloat(qc36) + parseFloat(qc37) + parseFloat(qc38) + parseFloat(qc39) + parseFloat(qc40);
    $("#41qc").val(qc41.toFixed(2));
    $("#41qc").attr("title", parseFloat(qc41).toFixed(2))

    /*--------------行46=行42+行43+行44+行45；-------------*/
    var qm42 = MoneyVerification($("#42qm").val());
    var qm43 = MoneyVerification($("#43qm").val());
    var qm44 = MoneyVerification($("#44qm").val());
    var qm45 = MoneyVerification($("#45qm").val());
    var qm46 = parseFloat(qm42) + parseFloat(qm43) + parseFloat(qm44) + parseFloat(qm45);
    $("#46qm").val(qm46.toFixed(2));
    $("#46qm").attr("title", parseFloat(qm46).toFixed(2))

    var qc42 = MoneyVerification($("#42qc").val());
    var qc43 = MoneyVerification($("#43qc").val());
    var qc44 = MoneyVerification($("#44qc").val());
    var qc45 = MoneyVerification($("#45qc").val());
    var qc46 = parseFloat(qc42) + parseFloat(qc43) + parseFloat(qc44) + parseFloat(qc45);
    $("#46qc").val(qc46.toFixed(2));
    $("#46qc").attr("title", parseFloat(qc46).toFixed(2))


    /*--------------行47=行41+行46；-------------*/
    var qm41 = MoneyVerification($("#41qm").val());
    var qm46 = MoneyVerification($("#46qm").val());
    var qm47 = parseFloat(qm41) + parseFloat(qm46);
    $("#47qm").val(qm47.toFixed(2));
    $("#47qm").attr("title", parseFloat(qm47).toFixed(2))


    var qc41 = MoneyVerification($("#41qc").val());
    var qc46 = MoneyVerification($("#46qc").val());
    var qc47 = parseFloat(qc41) + parseFloat(qc46);
    $("#47qc").val(qc47.toFixed(2));
    $("#47qc").attr("title", parseFloat(qc47).toFixed(2))

    /*--------------行52=行48+行49+行50+行51；-------------*/
    var qm48 = MoneyVerification($("#48qm").val());
    var qm49 = MoneyVerification($("#49qm").val());
    var qm50 = MoneyVerification($("#50qm").val());
    var qm51 = MoneyVerification($("#51qm").val());
    var qm52 = parseFloat(qm48) + parseFloat(qm49) + parseFloat(qm50) + parseFloat(qm51);
    $("#52qm").val(qm52.toFixed(2));
    $("#52qm").attr("title", parseFloat(qm52).toFixed(2))

    var qc48 = MoneyVerification($("#48qc").val());
    var qc49 = MoneyVerification($("#49qc").val());
    var qc50 = MoneyVerification($("#50qc").val());
    var qc51 = MoneyVerification($("#51qc").val());
    var qc52 = parseFloat(qc48) + parseFloat(qc49) + parseFloat(qc50) + parseFloat(qc51);
    $("#52qc").val(qc52.toFixed(2));
    $("#52qc").attr("title", parseFloat(qc52).toFixed(2))

    /*--------------行53=行47+行52=行30；-------------*/
    var qm47 = MoneyVerification($("#47qm").val());
    var qm52 = MoneyVerification($("#52qm").val());
    var qm53 = parseFloat(qm47) + parseFloat(qm52);
    $("#53qm").val(qm53.toFixed(2));
    $("#53qm").attr("title", parseFloat(qm53).toFixed(2))


    var qc47 = MoneyVerification($("#47qc").val());
    var qc52 = MoneyVerification($("#52qc").val());
    var qc53 = parseFloat(qc47) + parseFloat(qc52);
    $("#53qc").val(qc53.toFixed(2));
    $("#53qc").attr("title", parseFloat(qc53).toFixed(2))
    
}