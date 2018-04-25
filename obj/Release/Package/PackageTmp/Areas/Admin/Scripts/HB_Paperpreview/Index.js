/***************************************************************
  FileName:货币知识试卷微调预览 javascript
  Copyright（c）2017-金融教育在线技术开发部
  Author:袁学
  Create Date:2017-4-7
 ******************************************************************/

// JS 获取URL参数方法
function getQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) return unescape(r[2]); return null;
}


$(function () {
    PId = getQueryString('Pid');
    MType = getQueryString('Type');
    if (MType != "1") {//不可编辑
        //隐藏加点题按钮
        $("#Addtid").css("display", "none")
        for (var i = 1; i < 9; i++) {
            //隐藏每个类型分值批量设定
            $("#SetFid" + i).css("display", "none")
        }
    }
    BindPreview();

})
var PId = "0";//试卷Id
var MType = "";//区分

var tjdxtnum = 0, tjdxtscore = 0.00;
var tjdoxtnum = 0, tjdoxtscore = 0.00;
var tjpdtnum = 0, tjpdtscore = 0.00;

var tjtktnum = 0, tjtktscore = 0.00;
var jiandatnum = 0, jiandatscore = 0.00;
var mingctinum = 0, mingcitscore = 0.00;
var anlitktnum = 0, anlittscore = 0.00;
var lunshutnum = 0, lunshutscore = 0.00;

//页面数据加载
function BindPreview() {
    $.ajax({
        url: '/Admin/HB_Paperpreview/GetPreview?PId=' + PId,
        type: 'POST',
        dataType: 'json',
        success: function (data) {
            if (data != null) {
                var danxuantihtml = "";
                var duoxuantihtml = "";
                var panduantihtml = "";

                var tiankongtihtml = "";
                var jiandatihtml = "";
                var mingcitihtml = "";
                var anlitihtml = "";
                var lunshutihtml = "";

                tjdxtnum = 0, tjdxtscore = 0.00;
                tjdoxtnum = 0, tjdoxtscore = 0.00;
                tjpdtnum = 0, tjpdtscore = 0.00;

                tjtktnum = 0, tjtktscore = 0.00;
                jiandatnum = 0, jiandatscore = 0.00;
                mingctinum = 0, mingcitscore = 0.00;
                anlitktnum = 0, anlittscore = 0.00;
                lunshutnum = 0, lunshutscore = 0.00;

                var AllScores = 0.00;//总分
                $("#span_PName").html(data[0]["P_Name"]);
                for (var i = 0; i < data.length; i++) {
                    AllScores += parseFloat(data[i]["EP_Score"]); //获取分值

                    var QBtype = parseInt(data[i]["QB_Type"]);

                    if (QBtype == 1) {//单选题
                        tjdxtnum += 1;
                        tjdxtscore += parseFloat(data[i]["EP_Score"]);
                        danxuantihtml += "<div style=\"margin: 8px;\" id=\"divmouse" + i + "\" onmousemove=\"movedivview(" + i + ")\" onmouseout=\"outdiview(" + i + ")\">";
                        danxuantihtml += "    <div style=\"\">" + (i + 1) + "、<span style='color:rgba(14, 151, 250, 0.78)'>（该小题 " + data[i]["EP_Score"] + " 分）</span></div>";
                        danxuantihtml += "      <div style=\"float: right; display: none;\" id=\"setscoredeldiv" + i + "\">";
                        danxuantihtml += "          <a href=\"javascript:void(0)\" onclick=\"clickopeneditscore(" + data[i]["QuestionBId"] + "," + QBtype + "," + (i + 1) + ")\">【设定分值】</a> <a href=\"javascript:void(0)\" onclick=\"clickdelsjst(" + data[i]["QuestionBId"] + ")\">【删除】</a>";
                        danxuantihtml += "      </div>";
                        danxuantihtml += "      <div style=\"padding-left: 10px;\">";
                        danxuantihtml += "          " + HTMLDecode(data[i]["QB_Description"]) + "";
                        danxuantihtml += "      </div>";
                        danxuantihtml += "      <div style=\"margin-left: 20px; margin-top: 5px; clear: both;\">";
                        if (data[i]["QB_A"] != "" && data[i]["QB_A"].length > 0) {
                            danxuantihtml += "          <div style=\"padding-top: 3px;\">A." + data[i]["QB_A"] + "</div>";
                        }
                        if (data[i]["QB_B"] != "" && data[i]["QB_B"].length > 0) {
                            danxuantihtml += "          <div style=\"padding-top: 3px;\">B." + data[i]["QB_B"] + "</div>";
                        }
                        if (data[i]["QB_C"] != "" && data[i]["QB_C"].length > 0) {
                            danxuantihtml += "          <div style=\"padding-top: 3px;\">C." + data[i]["QB_C"] + "</div>";
                        }
                        if (data[i]["QB_D"] != "" && data[i]["QB_D"].length > 0) {
                            danxuantihtml += "          <div style=\"padding-top: 3px;\">D." + data[i]["QB_D"] + "</div>";
                        }
                        danxuantihtml += "       </div> <div>&nbsp;</div>";
                        danxuantihtml += "</div>";
                    }
                    if (QBtype == 2) {//多选题
                        tjdoxtnum += 1;
                        tjdoxtscore += parseFloat(data[i]["EP_Score"]);

                        duoxuantihtml += "<div style=\"margin: 8px;\" id=\"divmouse" + i + "\" onmousemove=\"movedivview(" + i + ")\" onmouseout=\"outdiview(" + i + ")\">";
                        duoxuantihtml += "     <div style=\"\">" + (i + 1) + "、<span style='color:rgba(14, 151, 250, 0.78)'>（该小题 " + data[i]["EP_Score"] + " 分）</span></div>";
                        duoxuantihtml += "      <div style=\"float: right; display: none;\" id=\"setscoredeldiv" + i + "\">";
                        duoxuantihtml += "           <a href=\"javascript:void(0)\" onclick=\"clickopeneditscore(" + data[i]["QuestionBId"] + "," + QBtype + "," + (i + 1) + ")\">【设定分值】</a> <a href=\"javascript:void(0)\" onclick=\"clickdelsjst(" + data[i]["QuestionBId"] + ")\">【删除】</a>";
                        duoxuantihtml += "      </div>";
                        duoxuantihtml += "      <div style=\"padding-left: 10px;\">";
                        duoxuantihtml += "          " + HTMLDecode(data[i]["QB_Description"]) + "";
                        duoxuantihtml += "      </div>";
                        duoxuantihtml += "      <div style=\"margin-left: 20px; margin-top: 5px; clear: both;\">";
               
                        if (data[i]["QB_A"] != "" && data[i]["QB_A"].length > 0) {
                            duoxuantihtml += "          <div style=\"padding-top: 3px;\">A." + data[i]["QB_A"] + "</div>";
                        }
                        if (data[i]["QB_B"] != "" && data[i]["QB_B"].length > 0) {
                            duoxuantihtml += "          <div style=\"padding-top: 3px;\">B." + data[i]["QB_B"] + "</div>";
                        }
                        if (data[i]["QB_C"] != "" && data[i]["QB_C"].length > 0) {
                            duoxuantihtml += "          <div style=\"padding-top: 3px;\">C." + data[i]["QB_C"] + "</div>";
                        }
                        if (data[i]["QB_D"] != "" && data[i]["QB_D"].length > 0) {
                            duoxuantihtml += "          <div style=\"padding-top: 3px;\">D." + data[i]["QB_D"] + "</div>";
                        }
                        if (data[i]["QB_E"] != null && data[i]["QB_E"].length > 0) {
                            duoxuantihtml += "      <div style=\"padding-top: 3px;\">E." + data[i]["QB_E"] + "</div>";
                        }
                        duoxuantihtml += "       </div> <div>&nbsp;</div>";
                        duoxuantihtml += "</div>";
                    }
                    if (QBtype == 3) {//判断题
                        tjpdtnum += 1;
                        tjpdtscore += parseFloat(data[i]["EP_Score"]);

                        panduantihtml += "<div style=\"margin: 8px;\" id=\"divmouse" + i + "\" onmousemove=\"movedivview(" + i + ")\" onmouseout=\"outdiview(" + i + ")\">";
                        panduantihtml += "     <div style=\"\">" + (i + 1) + "、<span style='color:rgba(14, 151, 250, 0.78)'>（该小题 " + data[i]["EP_Score"] + " 分）</span></div>";
                        panduantihtml += "      <div style=\"float: right; display: none;\" id=\"setscoredeldiv" + i + "\">";
                        panduantihtml += "          <a href=\"javascript:void(0)\" onclick=\"clickopeneditscore(" + data[i]["QuestionBId"] + "," + QBtype + "," + (i + 1) + ")\">【设定分值】</a> <a href=\"javascript:void(0)\" onclick=\"clickdelsjst(" + data[i]["QuestionBId"] + ")\">【删除】</a>";
                        panduantihtml += "      </div>";
                        panduantihtml += "      <div style=\"padding-left: 10px;\">";
                        panduantihtml += "          " + HTMLDecode(data[i]["QB_Description"]) + "";
                        panduantihtml += "      </div>";
                        panduantihtml += "      <div style=\"margin-left: 20px; margin-top: 5px; clear: both;\">";
                        panduantihtml += "          <div style=\"padding-top: 3px;\">A." + data[i]["QB_A"] + "</div>";
                        panduantihtml += "          <div style=\"padding-top: 3px;\">B." + data[i]["QB_B"] + "</div>";
                        panduantihtml += "       </div> <div>&nbsp;</div>";
                        panduantihtml += "</div>";
                    }

                    if (QBtype == 4) {//填空题
                        tjtktnum += 1;
                        tjtktscore += parseFloat(data[i]["EP_Score"]);

                        tiankongtihtml += "<div style=\"margin: 8px;\" id=\"divmouse" + i + "\" onmousemove=\"movedivview(" + i + ")\" onmouseout=\"outdiview(" + i + ")\">";
                        tiankongtihtml += "    <div style=\"\">" + (i + 1) + "、<span style='color:rgba(14, 151, 250, 0.78)'>（该小题 " + data[i]["EP_Score"] + " 分）</span></div>";
                        tiankongtihtml += "      <div style=\"float: right; display: none;\" id=\"setscoredeldiv" + i + "\">";
                        tiankongtihtml += "          <a href=\"javascript:void(0)\" onclick=\"clickopeneditscore(" + data[i]["QuestionBId"] + "," + QBtype + "," + (i + 1) + ")\">【设定分值】</a> <a href=\"javascript:void(0)\" onclick=\"clickdelsjst(" + data[i]["QuestionBId"] + ")\">【删除】</a>";
                        tiankongtihtml += "      </div>";
                        tiankongtihtml += "      <div style=\"padding-left: 10px;\">";
                        tiankongtihtml += "          " + HTMLDecode(data[i]["QB_Description"]) + "";
                        tiankongtihtml += "      </div>";
                        tiankongtihtml += "      <div style=\"margin-left: 20px; margin-top: 5px; clear: both;\">";
                        tiankongtihtml += "       </div>";
                        tiankongtihtml += "</div>";
                    }
                    if (QBtype == 5) {//简答题
                        jiandatnum += 1;
                        jiandatscore += parseFloat(data[i]["EP_Score"]);

                        jiandatihtml += "<div style=\"margin: 8px;\" id=\"divmouse" + i + "\" onmousemove=\"movedivview(" + i + ")\" onmouseout=\"outdiview(" + i + ")\">";
                        jiandatihtml += "     <div style=\"\">" + (i + 1) + "、<span style='color:rgba(14, 151, 250, 0.78)'>（该小题 " + data[i]["EP_Score"] + " 分）</span></div>";
                        jiandatihtml += "      <div style=\"float: right; display: none;\" id=\"setscoredeldiv" + i + "\">";
                        jiandatihtml += "          <a href=\"javascript:void(0)\" onclick=\"clickopeneditscore(" + data[i]["QuestionBId"] + "," + QBtype + "," + (i + 1) + ")\">【设定分值】</a> <a href=\"javascript:void(0)\" onclick=\"clickdelsjst(" + data[i]["QuestionBId"] + ")\">【删除】</a>";
                        jiandatihtml += "      </div>";
                        jiandatihtml += "      <div style=\"padding-left: 10px;\">";
                        jiandatihtml += "          " + HTMLDecode(data[i]["QB_Description"]) + "";
                        jiandatihtml += "      </div>";
                        jiandatihtml += "      <div style=\"margin-left: 20px; margin-top: 5px; clear: both;\">";
                        jiandatihtml += "       </div>";
                        jiandatihtml += "</div>";
                    }
                    if (QBtype == 6) {//名词
                        mingctinum += 1;
                        mingcitscore += parseFloat(data[i]["EP_Score"]);

                        mingcitihtml += "<div style=\"margin: 8px;\" id=\"divmouse" + i + "\" onmousemove=\"movedivview(" + i + ")\" onmouseout=\"outdiview(" + i + ")\">";
                        mingcitihtml += "   <div style=\"\">" + (i + 1) + "、<span style='color:rgba(14, 151, 250, 0.78)'>（该小题 " + data[i]["EP_Score"] + " 分）</span></div>";
                        mingcitihtml += "      <div style=\"float: right; display: none;\" id=\"setscoredeldiv" + i + "\">";
                        mingcitihtml += "          <a href=\"javascript:void(0)\" onclick=\"clickopeneditscore(" + data[i]["QuestionBId"] + "," + QBtype + "," + (i + 1) + ")\">【设定分值】</a> <a href=\"javascript:void(0)\" onclick=\"clickdelsjst(" + data[i]["QuestionBId"] + ")\">【删除】</a>";
                        mingcitihtml += "      </div>";
                        mingcitihtml += "      <div style=\"padding-left: 10px;\">";
                        mingcitihtml += "          " + HTMLDecode(data[i]["QB_Description"]) + "";
                        mingcitihtml += "      </div>";
                        mingcitihtml += "      <div style=\"margin-left: 20px; margin-top: 5px; clear: both;\">";
                        mingcitihtml += "       </div>";
                        mingcitihtml += "</div>";
                    }
                    if (QBtype == 7) {//案例
                        anlitktnum += 1;
                        anlittscore += parseFloat(data[i]["EP_Score"]);

                        anlitihtml += "<div style=\"margin: 8px;\" id=\"divmouse" + i + "\" onmousemove=\"movedivview(" + i + ")\" onmouseout=\"outdiview(" + i + ")\">";
                        anlitihtml += "     <div style=\"\">" + (i + 1) + "、<span style='color:rgba(14, 151, 250, 0.78)'>（该小题 " + data[i]["EP_Score"] + " 分）</span></div>";
                        anlitihtml += "      <div style=\"float: right; display: none;\" id=\"setscoredeldiv" + i + "\">";
                        anlitihtml += "          <a href=\"javascript:void(0)\" onclick=\"clickopeneditscore(" + data[i]["QuestionBId"] + "," + QBtype + "," + (i + 1) + ")\">【设定分值】</a> <a href=\"javascript:void(0)\" onclick=\"clickdelsjst(" + data[i]["QuestionBId"] + ")\">【删除】</a>";
                        anlitihtml += "      </div>";
                        anlitihtml += "      <div style=\"padding-left: 10px;\">";
                        anlitihtml += "          " + HTMLDecode(data[i]["QB_Description"]) + "";
                        anlitihtml += "      </div>";
                        anlitihtml += "      <div style=\"margin-left: 20px; margin-top: 5px; clear: both;\">";
                        anlitihtml += "       </div>";

                        anlitihtml += "</div>";
                    }
                    if (QBtype == 8) {//论述
                        lunshutnum += 1;
                        lunshutscore += parseFloat(data[i]["EP_Score"]);

                        lunshutihtml += "<div style=\"margin: 8px;\" id=\"divmouse" + i + "\" onmousemove=\"movedivview(" + i + ")\" onmouseout=\"outdiview(" + i + ")\">";
                        lunshutihtml += "     <div style=\"\">" + (i + 1) + "、<span style='color:rgba(14, 151, 250, 0.78)'>（该小题 " + data[i]["EP_Score"] + " 分）</span></div>";
                        lunshutihtml += "      <div style=\"float: right; display: none;\" id=\"setscoredeldiv" + i + "\">";
                        lunshutihtml += "          <a href=\"javascript:void(0)\" onclick=\"clickopeneditscore(" + data[i]["QuestionBId"] + "," + QBtype + "," + (i + 1) + ")\">【设定分值】</a> <a href=\"javascript:void(0)\" onclick=\"clickdelsjst(" + data[i]["QuestionBId"] + ")\">【删除】</a>";
                        lunshutihtml += "      </div>";
                        lunshutihtml += "      <div style=\"padding-left: 10px;\">";
                        lunshutihtml += "          " + HTMLDecode(data[i]["QB_Description"]) + "";
                        lunshutihtml += "      </div>";
                        lunshutihtml += "      <div style=\"margin-left: 20px; margin-top: 5px; clear: both;\">";
                        lunshutihtml += "       </div>";

                        lunshutihtml += "</div>";
                    }


                }
                //
                $("#danxuantiMian").html(''); $("#danxuantiDIVSHOW").css("display", "none");
                $("#duoxuantiMian").html(''); $("#duoxuantiDIVSHOW").css("display", "none");
                $("#panduantiMian").html(''); $("#panduantiDIVSHOW").css("display", "none");


                $("#tiankongtiMian").html(''); $("#tiankongtDIVSHOW").css("display", "none");
                $("#jiandatiMian").html(''); $("#jiandatDIVSHOW").css("display", "none");
                $("#mingcitiMian").html(''); $("#mingcitDIVSHOW").css("display", "none");
                $("#anlitiMian").html(''); $("#anlitDIVSHOW ").css("display", "none");
                $("#lunshutiMian").html(''); $("#lunshutDIVSHOW").css("display", "none");

                var zxcstr = "一,二,三,四,五,六,七,八";
                var zxsctrsp = zxcstr.split(',');
                var zxcflage1 = false; var zxcflage2 = false; var zxcflage3 = false; var zxcflage4 = false;
                var zxcflage5 = false; var zxcflage6 = false; var zxcflage7 = false; var zxcflage8 = false;
                for (var i = 0; i < zxsctrsp.length; i++) {
                    for (var m = 0; m < zxsctrsp.length; m++) {

                        if (zxcflage1 == false) {
                            if (danxuantihtml.length > 0) {
                                $("#zxc1").html(zxsctrsp[i]);
                                zxcflage1 = true;
                                break;
                            }
                        }
                        if (zxcflage2 == false) {
                            if (duoxuantihtml.length > 0) {
                                $("#zxc2").html(zxsctrsp[i]);
                                zxcflage2 = true;
                                break;
                            }
                        }
                        if (zxcflage3 == false) {
                            if (panduantihtml.length > 0) {
                                $("#zxc3").html(zxsctrsp[i]);
                                zxcflage3 = true;
                                break;
                            }
                        }
                        if (zxcflage4 == false) {
                            if (tiankongtihtml.length > 0) {
                                $("#zxc4").html(zxsctrsp[i]);
                                zxcflage4 = true;
                                break;
                            }
                        }

                        if (zxcflage5 == false) {
                            if (jiandatihtml.length > 0) {
                                $("#zxc5").html(zxsctrsp[i]);
                                zxcflage5 = true;
                                break;
                            }
                        }

                        if (zxcflage6 == false) {
                            if (mingcitihtml.length > 0) {
                                $("#zxc6").html(zxsctrsp[i]);
                                zxcflage6 = true;
                                break;
                            }
                        }


                        if (zxcflage7 == false) {
                            if (anlitihtml.length > 0) {
                                $("#zxc7").html(zxsctrsp[i]);
                                zxcflage7 = true;
                                break;
                            }
                        }

                        if (zxcflage8 == false) {
                            if (lunshutihtml.length > 0) {
                                $("#zxc8").html(zxsctrsp[i]);
                                zxcflage8 = true;
                                break;
                            }
                        }
                    }

                }
                //总分
                $("#span_AllScore").html('总分：' + AllScores.toFixed(2));
                if (danxuantihtml.length > 0) { //单选题显示
                    $("#danxuantiDIVSHOW").css("display", "block");
                    $("#danxuantiMian").html(danxuantihtml);

                    $("#TJ_danxuantinum").html('共 ' + tjdxtnum + ' 道，共 ' + tjdxtscore.toFixed(2) + ' 分');
                }
                if (duoxuantihtml.length > 0) {//多选题显示
                    $("#duoxuantiDIVSHOW").css("display", "block");
                    $("#duoxuantiMian").html(duoxuantihtml);

                    $("#TJ_duoxuantinum").html('共 ' + tjdoxtnum + ' 道，共 ' + tjdoxtscore.toFixed(2) + ' 分');
                }
                if (panduantihtml.length > 0) {//判断题显示
                    $("#panduantiDIVSHOW").css("display", "block");
                    $("#panduantiMian").html(panduantihtml);

                    $("#TJ_panduantinum").html('共 ' + tjpdtnum + ' 道，共 ' + tjpdtscore.toFixed(2) + ' 分');
                }

                if (tiankongtihtml.length > 0) {//填空题显示
                    $("#tiankongtDIVSHOW").css("display", "block");
                    $("#tiankongtiMian").html(tiankongtihtml);

                    $("#TJ_tiankongtinum").html('共 ' + tjtktnum + ' 道，共 ' + tjtktscore.toFixed(2) + ' 分');
                }
                if (jiandatihtml.length > 0) {//简答题显示
                    $("#jiandatDIVSHOW").css("display", "block");
                    $("#jiandatiMian").html(jiandatihtml);

                    $("#TJ_jiandatinum").html('共 ' + jiandatnum + ' 道，共 ' + jiandatscore.toFixed(2) + ' 分');
                }
                if (mingcitihtml.length > 0) {//名词题显示
                    $("#mingcitDIVSHOW").css("display", "block");
                    $("#mingcitiMian").html(mingcitihtml);

                    $("#TJ_mingcitinum").html('共 ' + mingctinum + ' 道，共 ' + mingcitscore.toFixed(2) + ' 分');
                }

                if (anlitihtml.length > 0) {//案例题显示
                    $("#anlitDIVSHOW").css("display", "block");
                    $("#anlitiMian").html(anlitihtml);

                    $("#TJ_anlitinum").html('共 ' + anlitktnum + ' 道，共 ' + anlittscore.toFixed(2) + ' 分');
                }

                if (lunshutihtml.length > 0) {//论述题显示
                    $("#lunshutDIVSHOW").css("display", "block");
                    $("#lunshutiMian").html(lunshutihtml);

                    $("#TJ_lunshutinum").html('共 ' + lunshutnum + ' 道，共 ' + lunshutscore.toFixed(2) + ' 分');
                }

            }
        }
    })
}

//鼠标移动上去 添加样式
function movedivview(n) {
    if (MType == "1") {//允许的才执行
        $("#divmouse" + n).addClass("addborder");
        $("#setscoredeldiv" + n).css("display", "block");
    }
}

//鼠标移开取消样式
function outdiview(n) {
    if (MType == "1") {//允许的才执行
        $("#divmouse" + n).removeClass("addborder");
        $("#setscoredeldiv" + n).css("display", "none");
    }
}

//单个设定分值
function clickopeneditscore(QuestionBId, type, num) {
    scoresQBId = QuestionBId;
    $("#Span1").html(GetTiType(type) + '（第' + num + '小题）');
    $("#Dtxtscore").val('');
    jQuery('#SetopenScore').modal('show', { backdrop: 'static' });
}

var scorestype;
var scoresQBId;
//批量设定分值
function SetBatchScores(type) {
    scorestype = type;
    if (type == 1) {
        $("#yx_type").html(GetTiType(type) + '（共 ' + tjdxtnum + ' 道）');
    }
    if (type == 2) {
        $("#yx_type").html(GetTiType(type) + '（共 ' + tjdoxtnum + ' 道）');
    }
    if (type == 3) {
        $("#yx_type").html(GetTiType(type) + '（共 ' + tjpdtnum + ' 道）');
    }
    if (type == 4) {
        $("#yx_type").html(GetTiType(type) + '（共 ' + tjtktnum + ' 道）');
    }

    if (type == 5) {
        $("#yx_type").html(GetTiType(type) + '（共 ' + jiandatnum + ' 道）');
    }
    if (type == 6) {
        $("#yx_type").html(GetTiType(type) + '（共 ' + mingctinum + ' 道）');
    }
    if (type == 7) {
        $("#yx_type").html(GetTiType(type) + '（共 ' + anlitktnum + ' 道）');
    }
    if (type == 8) {
        $("#yx_type").html(GetTiType(type) + '（共 ' + lunshutnum + ' 道）');
    }
    $("#txtscores").val('');
    jQuery('#BatchSetopenScore').modal('show', { backdrop: 'static' });

}

//批量设置分值保存
function BatchBtnOk() {
    var scores = $("#txtscores").val();
    if (scores.length == 0) {
        layer.msg('请输入分值！', function () { });
        return false;
    }
    ///单题分值校验
    if (parseFloat(scores) > 100) {
        layer.msg('单题分值不能大于100！', function () { });
        return false;
    }

    $.ajax({
        url: '/Admin/HB_Paperpreview/SetBatchScores',
        data: { "EP_Score": scores, "Type": scorestype, "EP_PId": PId },
        type: 'POST',
        success: function (data) {
            if (data == "1") {
                layer.msg('操作成功', { icon: 1 });
                jQuery('#BatchSetopenScore').modal('hide');
                BindPreview();
                $("#txtscores").val('');

            } else {
                layer.msg('操作失败', { icon: 2 });
            }
        }
    })
}

//单个设定分值保存
function BtnOk() {
    var scores = $("#Dtxtscore").val();
    if (scores.length == 0) {
        layer.msg('请输入分值！', function () { });
        return false;
    }
    ///单题分值校验
    if (parseFloat(scores) > 100) {
        layer.msg('单题分值不能大于100！', function () { });
        return false;
    }
    $.ajax({
        url: '/Admin/HB_Paperpreview/IndividualSetScore',
        data: { "EP_Score": scores, "EP_QBId": scoresQBId, "EP_PId": PId },
        type: 'POST',
        success: function (data) {
            if (data == "1") {
                layer.msg('操作成功', { icon: 1 });
                jQuery('#SetopenScore').modal('hide');

                BindPreview();
                $("#Dtxtscore").val('');
            } else {
                layer.msg('操作失败', { icon: 2 });
            }
        }
    })
}

//删除 移除试题
function clickdelsjst(QuestionBId) {
    layer.confirm('确认将该题移除试卷吗？', {
        title: '删除',
        btn: ['确定', '取消'],
        shadeClose: true, //开启遮罩关闭
        skin: 'layui-layer-lan'
        //按钮
    },
       function () {
           $.ajax({
               url: '/Admin/HB_Paperpreview/delExaminationPapers',
               data: { "EP_QBId": QuestionBId, "EP_PId": PId },
               type: 'POST',
               dataType: 'json',
               success: function (data) {
                   if (data == "1") {
                       layer.msg('操作成功', { icon: 1 });
                       BindPreview();
                   } else {
                       layer.msg('操作失败', { icon: 2 });
                   }
               }

           })
       });
}

//题型
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

//加点题
function AddT() {
    window.location.href = '/Admin/HB_Paperpreview/AddTIndex?PId=' + PId + '&Type=' + getQueryString('Type');
}