/***************************************************************
  FileName:手工点钞查看成绩 javascript
  Copyright（c）2017-金融教育在线技术开发部
  Author:邵世铨
  Create Date:2017-4-14
 ******************************************************************/
$(document).ready(function () {
    GetList();
});
// JS 获取URL参数方法
function getQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) return unescape(r[2]); return null;
}
//获取列表数据
function GetList()
{
    //做题ID
    var AchieID = getQueryString("ResponseState");
    //考试ID
    var ExaId = getQueryString("ExaId");

    $.ajax({
        type: "POST",
        dataType: "text",
        async: false,
        url: '/SG_Competition/SG_AchievementList',
        data: { AchieID:AchieID,ExaId:ExaId },
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
            var AchieScore = $("#AchieScoreid").val()+"分";
            $("#AchieScoreid2").html(AchieScore);
        }
    });
}

//返回
function Return()
{
    window.close();
}