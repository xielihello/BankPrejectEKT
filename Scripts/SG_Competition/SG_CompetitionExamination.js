/***************************************************************
  FileName:手工点钞做题 javascript
  Copyright（c）2017-金融教育在线技术开发部
  Author:邵世铨
  Create Date:2017-4-13
 ******************************************************************/
var SumTime = null;
//页面加载完就执行
$(document).ready(function () {
    RemainingTime();
    var id = getQueryString("Exaid");
    var Pid = getQueryString("Pid");
    Getinfo(id, Pid);

});
// JS 获取URL参数方法
function getQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) return unescape(r[2]); return null;
}
//请求数据
function Getinfo(id, Pid) {
    $.ajax({
        type: "POST",
        dataType: "text",
        //async: false,
        url: '/SG_Competition/SG_CompetitionControllerList',
        data: { Id: id, Pid: Pid },
        success: function (data) {
            $("#tableid").html(data);
            var ExaminationNameid = $("#ExaminationNameid").val();
            var Scoreid = $("#Scoreid").val();
            var Title = "" + ExaminationNameid + "";

            Title += "(满分" + Scoreid + "分)";
            $("#titleid").html(Title);
            var UserName = $("#UserNameid").val();

            $("#studentid").html(UserName);

            var UserPicid = $("#UserPicid").val();
            document.getElementById("Studentimg").src = UserPicid;
            var SchoolName = $("#Schoolid").val();
            $("#ShoolNameid").html(SchoolName);
            var Time = $("#Timeid").val();
            timer(Time);
            ajaxControl();
        }
    });
}

function Close() {
    layer.confirm('您的成绩将不会被记录，确认退出？', {
        title: '系统提示',
        btn: ['确定', '取消'],
        shadeClose: true, //开启遮罩关闭
        skin: 'layui-layer-lan'
        //按钮
    },
          function () {
              window.location.href = '/SG_Competition/'; //刷新父页面
              window.close();

          });
}
function RemainingTime() {

    var ExamId = getQueryString("Exaid");

    var PaperId = getQueryString("Pid");;
    $.ajax({
        type: "POST",
        dataType: "text",
        url: '/SG_Competition/ExamRemainingTime',
        data: { ExamId: ExamId, PaperId: PaperId },
        async: false,
        success: function (data) {
            if (data == "1") {
                window.location.href = "/SG_Competition/RemainingTime?ExamId=" + ExamId + "&PaperId=" + PaperId;
                return;
            }
        }
    });
}

function keyPress() {
    var keyCode = event.keyCode;
    if ((keyCode >= 48 && keyCode <= 57)) {
        event.returnValue = true;
    }
    else if (keyCode == 46) {
        event.returnValue = true;
    }
    else {
        layer.msg("只能输入数字！");
        event.returnValue = false;
        //var valuer = $("#" + ID).val();
        //if (/\D/.test(valuer)) { layer.msg('只能输入数字'); this.value = ''; }
    }
}

function keyPress2(ID) {
    var valuer = $("#" + ID).val();
    if (isDot(valuer) == false) {
        if (/\D/.test(valuer)) {
            layer.msg('只能输入数字');
            var valuer = $("#" + ID).val("");;
        }
    }

}
function isDot(num) {
    var result = (num.toString()).indexOf(".");
    if (result != -1) {
        return true;
    } else {
        return false;
    }
}
function Sumbile() {
    layer.confirm('您的成绩将会被记录，确认提交？', {
        title: '系统提示',
        btn: ['确定', '取消'],
        shadeClose: true, //开启遮罩关闭
        skin: 'layui-layer-lan'
        //按钮
    },
          function () {
              //一百元数量
              var idNumber100 = $("#100idNumber").val();
              if (idNumber100 == "") {
                  layer.msg("壹佰元数量不能为空");
                  return false;
              }
              if (!IsNullOrEmpty(idNumber100)) {
                  if (/\D/.test(idNumber100)) { layer.msg('壹佰元数量只能输入数字'); this.value = ''; return false }
              }
              var idMoney100 = $("#100idMoney").val();
              //if (idMoney100 == "") {
              //    layer.msg("壹佰元金额不能为空");
              //    return false;
              //}
              if (!IsNullOrEmpty(idMoney100)) {
                  if (!(/^(?:\d+|\d+\.\d{0,2})$/.test(idMoney100))) { layer.msg('壹佰元金额只能输入数字'); this.value = ''; return false }
              }

              var idNumber50 = $("#50idNumber").val();
              if (idNumber50 == "") {
                  layer.msg("伍拾元数量不能为空");
                  return false;
              }
              if (!IsNullOrEmpty(idNumber50)) {
                  if (/\D/.test(idNumber50)) { layer.msg('伍拾元数量只能输入数字'); this.value = ''; return false }
              }
             
              var idMoney50 = $("#50idMoney").val();
              ////if (idMoney50 == "") {
              ////    layer.msg("伍拾元金额不能为空");
              ////    return false;
              ////}
              //if (/\D/.test(idMoney50)) { layer.msg('伍拾元金额只能输入数字'); this.value = ''; return false }
              if (!IsNullOrEmpty(idMoney50)) {
                  if (!(/^(?:\d+|\d+\.\d{0,2})$/.test(idMoney50))) { layer.msg('伍拾元数量只能输入数字'); this.value = ''; return false }
              }
              var idNumber20 = $("#20idNumber").val();
              if (idNumber20 == "") {
                  layer.msg("贰拾元数量不能为空");
                  return false;
              }
              if (!IsNullOrEmpty(idNumber20)) {
                  if (/\D/.test(idNumber20)) { layer.msg('贰拾元数量只能输入数字'); this.value = ''; return false }
              }
              var idMoney20 = $("#20idMoney").val();
              ////if (idMoney20 == "") {
              ////    layer.msg("贰拾元金额不能为空");
              ////    return false;
              ////}
              //if (/\D/.test(idMoney20)) { layer.msg('贰拾元金额只能输入数字'); this.value = ''; return false }
              if (!IsNullOrEmpty(idMoney20)) {
                  if (!(/^(?:\d+|\d+\.\d{0,2})$/.test(idMoney20))) { layer.msg('贰拾元金额只能输入数字'); this.value = ''; return false }
              }
              var idNumber10 = $("#10idNumber").val();
              if (idNumber10 == "") {
                  layer.msg("拾元数量不能为空");
                  return false;
              }
              if (!IsNullOrEmpty(idNumber10)) {
                  if (/\D/.test(idNumber10)) { layer.msg('拾元数量只能输入数字'); this.value = ''; return false }
              }
              var idMoney10 = $("#10idMoney").val();
              ////if (idMoney10 == "") {
              ////    layer.msg("拾元金额不能为空");
              ////    return false;
              ////}
              //if (/\D/.test(idMoney10)) { layer.msg('拾元金额只能输入数字'); this.value = ''; return false }
              if (!IsNullOrEmpty(idMoney10)) {
                  if (!(/^(?:\d+|\d+\.\d{0,2})$/.test(idMoney10))) { layer.msg('拾元金额只能输入数字'); this.value = ''; return false }
              }
              var idNumber5 = $("#5idNumber").val();
              if (idNumber5 == "") {
                  layer.msg("伍元数量不能为空");
                  return false;
              }
              if (/\D/.test(idNumber5)) { layer.msg('伍元数量只能输入数字'); this.value = ''; return false }
              var idMoney5 = $("#5idMoney").val();
              if (idMoney5 == "") {
                  layer.msg("伍元金额不能为空");
                  return false;
              }
              //if (/\D/.test(idMoney5)) { layer.msg('伍元金额只能输入数字'); this.value = ''; return false }
              if (!IsNullOrEmpty(idMoney5)) {
                  if (!(/^(?:\d+|\d+\.\d{0,2})$/.test(idMoney5))) { layer.msg('伍元金额只能输入数字'); this.value = ''; return false }
              }
              var idNumber2 = $("#2idNumber").val();

              if (idNumber2 == "") {
                  layer.msg("贰元数量不能为空");
                  return false;
              }
              if (!IsNullOrEmpty(idNumber2)) {
                  if (/\D/.test(idNumber2)) { layer.msg('贰元数量只能输入数字'); this.value = ''; return false }
              }
              var idMoney2 = $("#2idMoney").val();
              ////if (idMoney2 == "") {
              ////    layer.msg("贰元金额不能为空");
              ////    return false;
              ////}
              //if (/\D/.test(idMoney2)) { layer.msg('贰元金额只能输入数字'); this.value = ''; return false }
              if (!IsNullOrEmpty(idMoney2)) {
                  if (!(/^(?:\d+|\d+\.\d{0,2})$/.test(idMoney2))) { layer.msg('贰元金额只能输入数字'); this.value = ''; return false }
              }
              var idNumber1 = $("#1idNumber").val();
              if (idNumber1 == "") {
                  layer.msg("壹元数量不能为空");
                  return false;
              }
              var idMoney1 = $("#1idMoney").val();
              if (!IsNullOrEmpty(idNumber1)) {
                  if (/\D/.test(idNumber1)) { layer.msg('壹元数量只能输入数字'); this.value = ''; return false }
              }
              ////if (idMoney1 == "") {
              ////    layer.msg("壹元金额不能为空");
              ////    return false;
              ////}
              //if (/\D/.test(idMoney1)) { layer.msg('壹元金额只能输入数字'); this.value = ''; return false }
              if (!IsNullOrEmpty(idMoney1)) {
                  if (!(/^(?:\d+|\d+\.\d{0,2})$/.test(idMoney1))) { layer.msg('壹元金额只能输入数字'); this.value = ''; return false }
              }
              var idNumberfive = $("#fiveidNumber").val();
              if (idNumberfive == "") {
                  layer.msg("伍角数量不能为空");
                  return false;
              }
              if (!IsNullOrEmpty(idNumberfive)) {
                  if (/\D/.test(idNumberfive)) { layer.msg('伍角数量只能输入数字'); this.value = ''; return false }
              }
              var idMoneyfive = $("#fiveidMoney").val();
              ////if (idMoneyfive == "") {
              ////    layer.msg("伍角金额不能为空");
              ////    return false;
              ////}
              //if (/\D/.test(idMoneyfive)) { layer.msg('伍角金额只能输入数字'); this.value = ''; return false }
              if (!IsNullOrEmpty(idMoneyfive)) {
                  if (!(/^(?:\d+|\d+\.\d{0,2})$/.test(idMoneyfive))) { layer.msg('伍角金额只能输入数字'); this.value = ''; return false }
              }
             
              var Exaid = getQueryString("Exaid");
              layer.msg('试卷正在提交，请稍等......', {
                  icon: 16, shade: 0.1, time: -1
              });
              var Time = $("#Time").html();
              $.ajax({
                  type: "POST",
                  dataType: "text",
                  async: false,
                  url: '/SG_Competition/SG_Save',
                  data: { Id: Exaid, idNumber100: idNumber100, idMoney100: idMoney100, idNumber50: idNumber50, idMoney50: idMoney50, idNumber20: idNumber20, idMoney20: idMoney20, idNumber10: idNumber10, idMoney10: idMoney10, idNumber5: idNumber5, idMoney5: idMoney5, idNumber2: idNumber2, idMoney2: idMoney2, idNumber1: idNumber1, idMoney1: idMoney1, idNumberfive: idNumberfive, idMoney1: idMoney1, idNumberfive: idNumberfive, idMoneyfive: idMoneyfive, Time: Time, SumTime: SumTime},
                  success: function (data) {
                      var data = eval('(' + data + ')');
                      debugger;
                      if (eval.length > 0) {
                          if (data[0]["ResponseState"] == "-1") {
                              //layer.alert('考试模式下不允许重复提交！', { skin: 'layui-layer-lan' }, function () {
                              //    layer.closeAll();
                              //})
                              window.location.href = '/SG_GradLook/Index?ResponseState=' + data[0]["ResponseState"] + '&ExaId=' + data[0]["ExaId"] + '';
                          }
                          if (data[0]["ResponseState"] == "-4") {
                              layer.alert('竞赛模式下不允许重复提交！', { skin: 'layui-layer-lan' }, function () {
                                  layer.closeAll();
                              })
                          }
                          if (data[0]["ResponseState"] == "-3") {
                              layer.alert('交卷失败，请刷新或重新登录再试！', { skin: 'layui-layer-lan' }, function () {
                                  layer.closeAll();//关闭所有弹出框
                              });
                          }
                          var state = data[0]["ResponseState"];
                          var pattern = parseInt(data[0]["Pattern"]);
                          if (state > 0) {
                              layer.closeAll();//关闭所有弹出框
                              if (pattern == 2 || pattern == 3) {
                                  window.location.href = '/SG_GradLook/Index?ResponseState=' + data[0]["ResponseState"] + '&ExaId=' + data[0]["ExaId"] + '';
                              }
                              if (pattern == 1) {
                                  layer.confirm('您本次的练习成绩为' + data[0]["Score"] + '分', {
                                      btn: ['查看考卷', '关闭'] //按钮
                                  }, function () {
                                      window.location.href = '/SG_Competition/ShowAchievement?ResponseState=' + data[0]["ResponseState"] + '&ExaId=' + data[0]["ExaId"] + '';
                                  }, function () {
                                      layer.closeAll();
                                      window.close();
                                  })
                              }
                          }

                      }
                  }

              })
          })
}

function zdSumbile() {
   
    //一百元数量
    var idNumber100 = $("#100idNumber").val();

    var idMoney100 = $("#100idMoney").val();

    var idNumber50 = $("#50idNumber").val();

    var idMoney50 = $("#50idMoney").val();

    var idNumber20 = $("#20idNumber").val();

    var idMoney20 = $("#20idMoney").val();

    var idNumber10 = $("#10idNumber").val();

    var idMoney10 = $("#10idMoney").val();

    var idNumber5 = $("#5idNumber").val();

    var idMoney5 = $("#5idMoney").val();

    var idNumber2 = $("#2idNumber").val();

    var idMoney2 = $("#2idMoney").val();

    var idNumber1 = $("#1idNumber").val();

    var idMoney1 = $("#1idMoney").val();

    var idNumberfive = $("#fiveidNumber").val();

    var idMoneyfive = $("#fiveidMoney").val();


    var Exaid = getQueryString("Exaid");
    layer.msg('试卷正在提交，请稍等......', {
        icon: 16, shade: 0.1, time: -1
    });

    var Time = $("#Time").html();
    $.ajax({
        type: "POST",
        dataType: "text",
        async: false,
        url: '/SG_Competition/SG_Save',
        data: { Id: Exaid, idNumber100: idNumber100, idMoney100: idMoney100, idNumber50: idNumber50, idMoney50: idMoney50, idNumber20: idNumber20, idMoney20: idMoney20, idNumber10: idNumber10, idMoney10: idMoney10, idNumber5: idNumber5, idMoney5: idMoney5, idNumber2: idNumber2, idMoney2: idMoney2, idNumber1: idNumber1, idMoney1: idMoney1, idNumberfive: idNumberfive, idMoney1: idMoney1, idNumberfive: idNumberfive, idMoneyfive: idMoneyfive, Time: Time, SumTime: SumTime },
        success: function (data) {
           
            var data = eval('(' + data + ')');
            if (eval.length > 0) {
                if (data[0]["ResponseState"] == "-1") {
                    layer.alert('考试模式下不允许重复提交！', { skin: 'layui-layer-lan' }, function () {
                        layer.closeAll();
                    })
                }
                if (data[0]["ResponseState"] == "-4") {
                    layer.alert('竞赛模式下不允许重复提交！', { skin: 'layui-layer-lan' }, function () {
                        layer.closeAll();
                    })
                }
                if (data[0]["ResponseState"] == "-3") {
                    layer.alert('交卷失败，请刷新或重新登录再试！', { skin: 'layui-layer-lan' }, function () {
                        layer.closeAll();//关闭所有弹出框
                    });
                }
                var state = data[0]["ResponseState"];
                var pattern = parseInt(data[0]["Pattern"]);
                if (state > 0) {
                    layer.closeAll();//关闭所有弹出框
                    if (pattern == 2 || pattern == 3) {
                        window.location.href = '/SG_GradLook/Index?ResponseState=' + data[0]["ResponseState"] + '&ExaId=' + data[0]["ExaId"] + '';
                    }
                    if (pattern == 1) {
                        layer.confirm('您本次的练习成绩为' + data[0]["Score"] + '分', {
                            btn: ['查看考卷', '关闭'] //按钮
                        }, function () {
                            window.location.href = '/SG_Competition/ShowAchievement?ResponseState=' + data[0]["ResponseState"] + '&ExaId=' + data[0]["ExaId"] + '';
                        }, function () {
                            layer.closeAll();
                            window.close();
                        })
                    }
                }

            }
        }

    })
}
var intDiff;//倒计时总秒数量
var TimeOut;
var pd = 0;
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

            $("#Time").html(hour + ":" + minute + ":" + second + "");
            if (pd == 0) {
                SumTime = $("#Time").html(); //倒计时总时间
                pd = 1;
            }

        } else {

            clearInterval(TimeOut);
            zdSumbile();
        }
        intDiff--;
    }, 1000);
}
