/***************************************************************
  FileName:货币知识 考试明细 javascript
  Copyright（c）2017-金融教育在线技术开发部
  Author:袁学
  Create Date:2017-4-14
 ******************************************************************/

function getQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) return unescape(r[2]); return null;
}

var Pid = "";
var Eid = "";
var MType = "";//1是学生 2是老师
var Mid = "";//学生id

$(function () {
    Eid = getQueryString('Eid');
    Pid = getQueryString('Pid');
    MType = getQueryString('Type');
    Mid = getQueryString('Mid');
    if (Pid == null || Eid == null || Pid == "" || Eid == "") {
        return;
    }

    selectType(1);//题型选择

    //成绩统计
    GetResult();
});
var aixi = 0;
//头部8种题型选择
function selectType(type) {

    //头部题型选择样式控制
    for (var i = 1; i < 9; i++) {
        $("#Typeli" + i).css("display", "none");//所有的先隐藏
        $("#Typeli" + i).removeClass('active');
    }

    var alltype = 0;
    //控制当前是什么题型
    $.ajax({
        url: "/HB_ExamPreview/GetTypeNum",
        data: { "Pid": Pid },
        type: 'POST',
        async: false,
        dataType: 'json',
        success: function (data) {
            if (data.length > 0) {
                //获取本次考试第一个类型题目
                if (parseInt(data[0]["QB_Type"]) != 1 && aixi == 0) {
                    alltype = parseInt(data[0]["QB_Type"]);

                }


                for (var i = 0; i < data.length; i++) {
                    $("#Typeli" + data[i]["QB_Type"]).css("display", "block");
                    //与题型当前相等的时候
                    if (type == parseInt(data[i]["QB_Type"])) {
                        $("#titleScore").html(GetTiType(type) + '（共' + data[i]["Tnum"] + '题，总计' + data[i]["Tscoers"] + '分，当前题<span id="newQBId"></span>分）');
                    }
                }
            }
        }
    });
    //如果类型题目不是单选题 那就冲洗为类型赋值
    if (alltype != 0) {
        type = alltype;
    }
   
    //题型选择样式
    $("#Typeli" + type).addClass('active');

    //数据清理
    xTid = 0;
    xT_type = 0;
    yx_bianliang = ",";
    quanjuY = 0;
    quanjuQBIdALL = "";

    BindQuestionNo(type);//题号加载
    Bindinfo(quanjuQBIdALL[0]);//做题内容加载
    //题型默认第一个
    //$("#YXthxz" + xTid).removeClass('color_no'); //移除未回答样式
    //$("#YXthxz" + xTid).removeClass('color_sure'); //移除已回答样式
    //$("#YXthxz" + xTid).removeClass('color_now'); //移除选中
    ////当前题添加选中样式
    $("#YXthxz" + xTid).addClass('active');
    //GOtiColor(type);//样式加载

}


//成绩统计
function GetResult() {
    $.ajax({
        url: "/HB_ExamPreview/GetResult",
        data: { "Pid": Pid, "Mid": Mid, "Type": MType, "Eid": Eid },
        type: 'POST',
        async: false,
        dataType: 'json',
        success: function (data) {
            if (data != null && data.length > 0) {
                var html = '';
                var reuslts = 0;//总分
                var dereulsts = 0;//得分
                for (var i = 0; i < data.length; i++) {
                    reuslts += parseFloat(data[i]["Tscoers"]);
                    dereulsts += parseFloat(data[i]["Goal"]);
                    html += '<tr>';
                    html += '<td>' + GetTiType(data[i]["QB_Type"]) + '</td>';
                    html += '<td class="text_blue">' + data[i]["Tscoers"] + '</td>';
                    html += ' <td>' + data[i]["Goal"] + '</td>';
                    var bili = parseFloat(data[i]["Goal"]) / parseFloat(data[i]["Tscoers"]);
                    html += '<td class="text_red">' + (bili * 100).toFixed(2) + '%</td>';
                    html += ' </tr>';
                }
                html += ' <tr>';
                html += ' <td>总分</td>';
                html += '<td class="text_blue">' + reuslts + '</td>';
                html += '<td>' + dereulsts + '</td>';
                html += '<td class="text_red">' + ((dereulsts / reuslts) * 100).toFixed(2) + '%</td>';
                html += '</tr>';
                $("#tablelist").html(html);
            }
        }
    });
}


//返回题型中文名
function GetTiType(type) {
    if (type == 1) {
        return "单选题";
    }
    if (type == 2) {
        return "多选题";
    }
    if (type == 3) {
        return "判断题";
    }
    if (type == 4) {
        return "填空题";
    }
    if (type == 5) {
        return "简答题";
    }
    if (type == 6) {
        return "名词解释题";
    }
    if (type == 7) {
        return "案例分析题";
    }
    if (type == 8) {
        return "论述题";
    }
    return "";
}


//绑定加载左边题号 type=试题类型
function BindQuestionNo(type) {
    $.ajax({
        url: "/HB_ExamPreview/KaoShiByPId_ZQList",
        data: { "Pid": Pid, "isto": "1", "Eid": Eid, "QB_Type": type },
        type: 'POST',
        async: false,
        dataType: 'json',
        success: function (data) {
            var html = "";
            if (data != null && data.length > 0) {
                var ALLQBId = "";//题id拼接
                //总长度
                zongcounnum = data.length;

                for (var y = 0; y < data.length; y++) {
                    var T_Type = parseInt(data[y]["QB_Type"]);//题型
                    var QBId = data[y]["QuestionBId"];//题id

                    ALLQBId += QBId + ","
                    //题号
                    var thNum = (y + 1) + "";
                    if (y < 9) {
                        thNum = "0" + (y + 1);
                    }

                    html += '<li class="num_sty "  onclick="djth(' + QBId + ',' + y + ')"><span class="num_bj color_no" id="YXthxz' + QBId + '">' + thNum + '</span></li>';
                }
                quanjuQBIdALL = ALLQBId.split(',');

                $("#thdiv").html(html);
                ////////题号样式///////////////////////
                $.ajax({
                    url: "/HB_ExamPreview/GetExaminationDetailsbys",
                    data: { "Pid": Pid, "Eid": Eid, "Type": MType, "Mid": Mid, "QB_Type": type },
                    type: 'POST',
                    async: false,
                    dataType: 'json',
                    success: function (d) {
                        if (d != null && d.length > 0) {
                          
                            for (var y = 0; y < d.length; y++) {
                                var QBId = d[y]["QuestionBId"];

                                var ED_OkNo = d[y]["ED_OkNo"];
                                var ED_Content = d[y]["ED_Content"];
                                $("#YXthxz" + QBId).removeClass('color_no');
                                if (ED_Content.length == 0 || ED_Content == "") {//没有填写 就是未做
                                    //添加未做题样式
                                    $("#YXthxz" + QBId).addClass('color_no');
                                }
                                else {
                                    if (ED_OkNo == "正确") {
                                        $("#YXthxz" + QBId).addClass('color_look_sure');
                                    
                                    }
                                    else {
                                        $("#YXthxz" + QBId).addClass('color_now');
                                    }
                                }
                            }
                        }
                    }
                });
            }
            //
        }
    });
}

var MtxtId;
var xTid; //当前qbid
var xT_type;
var yx_bianliang = ",";
var quanjuY = 0;//全局游标
var zongcounnum = 0;//总长度
//全局 存了当前题型试题ID 
var quanjuQBIdALL; //


function BindDaan() {

    $.ajax({
        url: "/HB_ExamPreview/GetExaminationDetailsbysTo",
        data: { "Pid": Pid, "Eid": Eid, "QBId": xTid, "Type": MType, "Mid": Mid },
        type: 'POST',
        dataType: 'json',
        async: false,
        success: function (d) {

            if (d != null && d.length > 0) {
                var QBType = parseInt(d[0]["QB_Type"]);

                if (QBType == 1) {//单选题
                    if (d[0]["ED_Content"] != "" && d[0]["ED_Content"].length > 0) {
                        $("#danx" + d[0]["ED_Content"] + xTid).addClass('active');
                    }
                }

                if (QBType == 2) {//多选题
                    if (d[0]["ED_Content"] != "" && d[0]["ED_Content"].length > 0) {
                        var str = d[0]["ED_Content"] + "";
                        var spstr = str.split(',');
                        for (var i = 0; i < spstr.length; i++) {
                            $("#duox" + spstr[i] + xTid).addClass('active');
                        }
                    }
                }

                if (QBType == 3) {//判断题
                    if (d[0]["ED_Content"] == "对") {
                        $("#pduanA" + xTid).addClass('active');

                    }
                    if (d[0]["ED_Content"] == "错") {
                        $("#pduanB" + xTid).addClass('active');

                    }
                }



                var T_type = "";
                var falg = false;
                if (QBType == 4) { T_type = "tiankT"; falg = true; }
                if (QBType == 5) { T_type = "jiandT"; falg = true; }
                if (QBType == 6) { T_type = "mingcjs"; falg = true; }
                if (QBType == 7) { T_type = "anlifxT"; falg = true; }
                if (QBType == 8) { T_type = "lunsT"; falg = true; }
                if (falg == true) {
                    $("#" + T_type + xTid).val(d[0]["ED_Content"]);
                }


                var txtOkNo = '<span class="text_red">回答错误!</span>';
                if (d[0]["ED_OkNo"] == "正确") {
                    txtOkNo = '<span class="text_green">回答正确!</span>';
                }

                //  您选择的答案为<span class="text_red">C</span>,本题的正确答案为 <span class="text_green">B</span>,<span class="text_red">回答错误!</span>
                if (QBType == 1 || QBType == 2 || QBType == 3) {

                    $("#dananshowdiv").html('您选择的答案为：<span class="text_red">' + d[0]["ED_Content"] + '</span> ,本题的正确答案为：<span class="text_green">' + d[0]["QB_Answer"] + '</span> ,' + txtOkNo);
                }

                if (QBType == 4) {//填空题

                    $("#dananshowdiv").html('本题正确答案为：<span class="text_green">' + d[0]["QB_Answer"] + '</span> ,' + txtOkNo);
                }

                if (QBType == 5 || QBType == 6 || QBType == 7 || QBType == 8) {
                    $("#dananshowdiv").html('本题正确答案为：<span class="text_green">' + d[0]["QB_Answer"] + '</span> ,得分关键字为：<span class="text_green">' + d[0]["QB_Keyword"] + '</span> ,' + txtOkNo);

                }
            }
        }
    })
}

//绑定右边试题数据
function Bindinfo(QBId) {
    $.ajax({
        url: "/HB_ExamPreview/GetQuestionBankByIdAndPid",
        data: { "QuestionBId": QBId, "Pid": Pid },
        type: 'POST',
        async: false,
        dataType: 'json',
        success: function (data) {
            $("#YXthxz" + MtxtId).removeClass('active');
            if (data != null && data.length > 0) {
                var T_Type = parseInt(data[0]["QB_Type"]); //题型
                var xuhao = 0;//序号
                if (quanjuY == 0) {
                    xuhao = 1;
                } else {
                    xuhao = quanjuY + 1;
                }
                //当前题多少分
                $("#newQBId").html(data[0]["EP_Score"]);
                $("#titleDescription").html(xuhao + "、" + HTMLDecode(data[0]["QB_Description"]));//题目描述
                var daanHtml = "";

                xTid = data[0]["QuestionBId"];
                xT_type = parseInt(data[0]["QB_Type"]);

                if (T_Type == 1) { //单选题
                    if (data[0]["QB_A"] != "" && data[0]["QB_A"].length > 0) {
                        daanHtml += "<div class=\"state_xz\"><span class=\"bt_xz\" id=\"danxA" + xTid + "\"  onclick=\"BindTNum('" + xTid + "','1','A')\">A</span><span>" + data[0]["QB_A"] + "</span></div>";
                    }
                    if (data[0]["QB_B"] != "" && data[0]["QB_B"].length > 0) {
                        daanHtml += "<div class=\"state_xz\"><span class=\"bt_xz\" id=\"danxB" + xTid + "\"  onclick=\"BindTNum('" + xTid + "','1','B')\">B</span><span>" + data[0]["QB_B"] + "</span></div>";
                    }
                    if (data[0]["QB_C"] != "" && data[0]["QB_C"].length > 0) {
                        daanHtml += "<div class=\"state_xz\"><span class=\"bt_xz\" id=\"danxC" + xTid + "\"  onclick=\"BindTNum('" + xTid + "','1','C')\">C</span><span>" + data[0]["QB_C"] + "</span></div>";
                    }
                    if (data[0]["QB_D"] != "" && data[0]["QB_D"].length > 0) {
                        daanHtml += "<div class=\"state_xz\"><span class=\"bt_xz\" id=\"danxD" + xTid + "\"  onclick=\"BindTNum('" + xTid + "','1','D')\">D</span><span>" + data[0]["QB_D"] + "</span></div>";
                    }
                }
                if (T_Type == 2) { //多选题
                    if (data[0]["QB_A"] != "" && data[0]["QB_A"].length > 0) {
                        daanHtml += "<div class=\"state_xz\"><span class=\"bt_xz\" id=\"duoxA" + xTid + "\"  onclick=\"BindTNum('" + xTid + "','2','A')\">A</span><span>" + data[0]["QB_A"] + "</span></div>";
                    }
                    if (data[0]["QB_B"] != "" && data[0]["QB_B"].length > 0) {
                        daanHtml += "<div class=\"state_xz\"><span class=\"bt_xz\" id=\"duoxB" + xTid + "\"  onclick=\"BindTNum('" + xTid + "','2','B')\">B</span><span>" + data[0]["QB_B"] + "</span></div>";
                    }
                    if (data[0]["QB_C"] != "" && data[0]["QB_C"].length > 0) {
                        daanHtml += "<div class=\"state_xz\"><span class=\"bt_xz\" id=\"duoxC" + xTid + "\"  onclick=\"BindTNum('" + xTid + "','2','C')\">C</span><span>" + data[0]["QB_C"] + "</span></div>";
                    }
                    if (data[0]["QB_D"] != "" && data[0]["QB_D"].length > 0) {
                        daanHtml += "<div class=\"state_xz\"><span class=\"bt_xz\" id=\"duoxD" + xTid + "\"  onclick=\"BindTNum('" + xTid + "','2','D')\">D</span><span>" + data[0]["QB_D"] + "</span></div>";
                    }
                    if (data[0]["QB_E"] != null && data[0]["QB_E"].length > 0) {
                        daanHtml += "<div class=\"state_xz\"><span class=\"bt_xz\" id=\"duoxE" + xTid + "\"  onclick=\"BindTNum('" + xTid + "','2','E')\">E</span><span>" + data[0]["QB_E"] + "</span></div>";
                    }
                }
                if (T_Type == 3) { //判断题
                    daanHtml += "<div class=\"state_xz\"><span class=\"bt_xz\" id=\"pduanA" + xTid + "\"  onclick=\"BindTNum('" + xTid + "','3','A')\">A</span><span>对</span></div>";
                    daanHtml += "<div class=\"state_xz\"><span class=\"bt_xz\" id=\"pduanB" + xTid + "\"  onclick=\"BindTNum('" + xTid + "','3','B')\">B</span><span>错</span></div>";

                }

                var T_typeName = "";

                var falg = false;
                if (T_Type == 4) { falg = true; T_typeName = "tiankT"; }
                if (T_Type == 5) { falg = true; T_typeName = "jiandT"; }
                if (T_Type == 6) { falg = true; T_typeName = "mingcjs"; }
                if (T_Type == 7) { falg = true; T_typeName = "anlifxT"; }
                if (T_Type == 8) { falg = true; T_typeName = "lunsT"; }


                if (falg == true) {
                    daanHtml += "<div style=\"margin:10px;\"><textarea style=\"width:90%; height:140px;\" id=\"" + T_typeName + xTid + "\" onkeyup=\"BindTNum('" + xTid + "','" + T_Type + "','" + T_typeName + "')\" placeholder=\"请输入标准答案....\" ></textarea></div>";
                }
                //显示 绑定答案ABCD
                $("#daanContent").html(daanHtml);

                BindDaan();
                $("#YXthxz" + xTid).addClass('active');
                MtxtId = xTid;
            }
        }
    });
}

//上一题
function clickshangyiti() {
    if (quanjuY != 0) {
        //获取上一题的下标
        var yxidx = 0;
        for (var m = 0; m < quanjuQBIdALL.length; m++) {
            if (quanjuQBIdALL[m] == xTid) {
                yxidx = m - 1;
                break;
            }

        }
        quanjuY = yxidx;
        //加载上一题
        Bindinfo(quanjuQBIdALL[yxidx]);
    }

}

//下一题
function clickxiayiti() {
    if (zongcounnum - 1 != quanjuY) {//最后一题
        //获取下一题的下标
        var yxidx = 0;
        for (var m = 0; m < quanjuQBIdALL.length; m++) {
            if (quanjuQBIdALL[m] == xTid) {
                yxidx = m + 1;
                break;
            }

        }
        quanjuY = yxidx;
        //加载下一题
        Bindinfo(quanjuQBIdALL[yxidx]);
    }
}

//首题
function clickshouti() {
    var yxidx = 0;
    quanjuY = yxidx;
    //加载上一题
    Bindinfo(quanjuQBIdALL[yxidx]);

}

//尾题
function clickweiti() {

    var yxidx = 0;
    yxidx = zongcounnum - 1
    quanjuY = yxidx;
    //加载上一题
    Bindinfo(quanjuQBIdALL[yxidx]);

}

//序号点击
function djth(QBId, y) {
    quanjuY = y;
    Bindinfo(QBId);//qbid 为当前选中的题号
}

//退出
function tuichu() {
    var Type = getQueryString('Type');
    if (Type != null && Type == "1") {
        window.close();
        window.opener.opener = null; window.opener.close();
    } else {
        window.close();
    }
}
