/***************************************************************
  FileName:个人总成绩 javascript
  Copyright（c）2017-金融教育在线技术开发部
  Author:袁学
  Create Date:2017-4-18
 ******************************************************************/

//分数排序
function ResultDesc() {
    if (descby == "Toscores desc") {
        descby = "Toscores asc";
    } else {
        descby = "Toscores desc";
    }
    bindIngfo();
}

$(function () {
    bindSchool();
    bindTeam();
    BindAllSelect();
    bindIngfo();
})
var descby = "Toscores desc";

function bindIngfo(page) {

    var SchooId = $("#SchooId").val();//学院
    var TeamId = $("#TeamId").val();//班级


    var StudentInfo = $("#StudentInfo").val();//学生信息
    var ScoreQJ = $("#ScoreQJ").val();//分数区间

    var sjly = $("#sjly").val();//试卷来源

    //四个赛项的选择
    var hbzs_selectId = $("#hbzs_selectId").val();
    var sgdc_selectId = $("#sgdc_selectId").val();
    var fhbb_selectId = $("#fhbb_selectId").val();
    var djlr_selectId = $("#djlr_selectId").val();

    //校验分数区间
    if (ScoreQJ.length > 0) {
        var ScoreQJtxt = ScoreQJ + "";

        var sqjtxtlist = ScoreQJtxt.split('-');
        if (sqjtxtlist.length == 1 || sqjtxtlist.length > 2) {//格式错误
            layer.msg('搜索条件分数区间格式错误，正确格式如：60-70', function () { });
            return;
        }

        //再校验前后
        var to1 = sqjtxtlist[0];
        var to2 = sqjtxtlist[1];
        if (isNaN(to1)) {
            layer.msg('分数区间格式错误，不能包含英文字符！', function () { });
            return;
        }

        if (isNaN(to2)) {
            layer.msg('分数区间格式错误，不能包含英文字符！', function () { });
            return;
        }

        //必须-前的小于-后的值
        if (parseFloat(to1) > parseFloat(to2)) {
            layer.msg('分数区间格式错误，区间开始值不能大于结束值！', function () { });
            return;
        }
    }

    var PageSize = 10;

    $.ajax({
        Type: "post",
        dataType: "json",
        cache: false,
        contentType: "application/json; charset=utf-8",
        url: '/Admin/PersonalTotalScore/GetList',
        data: {
            "descby": descby, "SchooId": SchooId, "TeamId": TeamId, "StudentInfo": StudentInfo, "ScoreQJ": ScoreQJ,
            "sjly": sjly, "hbzs_selectId": hbzs_selectId, "sgdc_selectId": sgdc_selectId, "fhbb_selectId": fhbb_selectId, "djlr_selectId": djlr_selectId,
            "page": page, "PageSize": PageSize
        },
        success: function (tb) {
            var html = '';
            var data = tb.Tb;//转换table
            var UserType = $("#UserTypeID").val();
            for (var i = 0; i < data.length; i++) {
                html += '<tr>';
                //当前页面
                var idx = 0;
                if (page != "undefined" && page != null) {
                    idx = page;
                    idx = idx - 1;
                }
                html += '<td><span class="pie">' + ((idx * PageSize) + i + 1) + '</span></td>';



                //管理员
                if (parseInt(UserType) == 1) {

                    //院校
                    html += '<td><span class="pie" >' + data[i]["Name"] + '</span></td>';
                }
                //班级
                html += '<td><span class="pie">' + data[i]["TeamName"] + '</span></td>';

                //管理员
                if (parseInt(UserType) == 1) {
                    //班级教师
                    var txtCustom2 = data[i]["Custom2"] + "";
                    if (txtCustom2.length > 10) {
                        txtCustom2 = txtCustom2.substr(0, 7) + "...";
                    }
                    html += '<td><span class="pie">' + txtCustom2 + '</span></td>';
                }

                //学生账号
                html += '<td><span class="pie" >' + data[i]["UserNo"] + '</span></td>';

                if (data[i]["UserName"] == null) {
                    html += '<td><span class="pie" >---</span></td>';
                } else {
                    //学生姓名
                    html += '<td><span class="pie" >' + data[i]["UserName"] + '</span></td>';
                }

                //四个赛项分值
                html += '<td><span class="pie" >' + data[i]["hbzs"].toFixed(2) + '</span></td>';
                html += '<td><span class="pie" >' + data[i]["sgdc"].toFixed(2) + '</span></td>';
                html += '<td><span class="pie" >' + data[i]["fhbb"].toFixed(2) + '</span></td>';
                html += '<td><span class="pie" >' + data[i]["djlr"].toFixed(2) + '</span></td>';
                //总分
                html += '<td><span class="pie" >' + data[i]["Toscores"].toFixed(2) + '</span></td>';
                html += '</tr>';

            }
            $("#tablelist").html(html);

            bootstrapPaginator("#PaginatorLibrary", tb, bindIngfo);//分页
            //样式重新加载
            redload();


        }
    });
}

//复选框 全选样式 控制
function redload() {
    $('.i-checks').iCheck({
        checkboxClass: 'icheckbox_square-green',
        radioClass: 'iradio_square-green',
    });
    //全选checkbox
    var $checkboxAll = $(".checkbox-all"),
                       $checkbox = $(".new_table").find("[type='checkbox']").not("[disabled]"),
                       length = $checkbox.length,
                       i = 0;
    $checkboxAll.on("ifClicked", function (event) {
        if (event.target.checked) {
            $checkbox.iCheck('uncheck');
            i = 0;
        } else {
            $checkbox.iCheck('check');
            i = length;
        }
    });
}

//绑定院校下拉框
function bindSchool() {
    $.ajax({
        Type: "post",
        dataType: "json",
        url: '/Admin/PersonalTotalScore/GetSchool',
        async: false,
        success: function (data) {
            var html = '<option value="0">院校名称</option>';
            for (var i = 0; i < data.length; i++) {
                html += '<option value="' + data[i]["Id"] + '">' + data[i]["Name"] + '</option>';
            }
            $("#SchooId").html(html);
        }
    });
}

//绑定的班级下拉框
function bindTeam() {

    var wheres = '';
    if ($("#UserTypeID").val() == "1") {
        wheres = '?SchooId=' + $("#SchooId").val();
    }
    $.ajax({
        Type: "post",
        dataType: "json",
        url: '/Admin/PersonalTotalScore/GetTeam' + wheres,
        async: false,
        success: function (data) {
            var html = '<option value="0">班级名称</option>';
            for (var i = 0; i < data.length; i++) {
                html += '<option value="' + data[i]["Id"] + '">' + data[i]["TeamName"] + '</option>';
            }
            $("#TeamId").html(html);
        }
    });
}

//联动
function SelctSchool() {
    bindTeam();
}

//搜索
function searchinfo() {

    bindIngfo();

}

//绑定下拉框四个赛项的考试
function BindAllSelect() {
    var wheres = '';
    if ($("#UserTypeID").val() == "1") {//管理员端 试卷来源
        wheres = '?sjly=' + $("#sjly").val();
    }
    //货币知识
    $.ajax({
        Type: "post",
        dataType: "json",
        url: '/Admin/PersonalTotalScore/GetHBZS' + wheres,
        async: false,
        success: function (data) {
            var html = '<option value="0">货币知识</option>';
            for (var i = 0; i < data.length; i++) {
                html += '<option value="' + data[i]["EId"] + '">' + data[i]["E_Name"] + '</option>';
            }
            $("#hbzs_selectId").html(html);
        }
    });

    //手工点钞
    $.ajax({
        Type: "post",
        dataType: "json",
        url: '/Admin/PersonalTotalScore/GetSGDC' + wheres,
        async: false,
        success: function (data) {
            var html = '<option value="0">手工点钞</option>';
            for (var i = 0; i < data.length; i++) {
                html += '<option value="' + data[i]["ID"] + '">' + data[i]["ExaminationName"] + '</option>';
            }
            $("#sgdc_selectId").html(html);
        }
    });

    //复核报表

    $.ajax({
        Type: "post",
        dataType: "json",
        url: '/Admin/PersonalTotalScore/GetFHBB' + wheres,
        async: false,
        success: function (data) {
            var html = '<option value="0">复核报表</option>';
            for (var i = 0; i < data.length; i++) {
                html += '<option value="' + data[i]["Id"] + '">' + data[i]["ExaminationName"] + '</option>';
            }
            $("#fhbb_selectId").html(html);
        }
    });

    //单据录入

    $.ajax({
        Type: "post",
        dataType: "json",
        url: '/Admin/PersonalTotalScore/GetDJlR' + wheres,
        async: false,
        success: function (data) {
            var html = '<option value="0">单据录入</option>';
            for (var i = 0; i < data.length; i++) {
                html += '<option value="' + data[i]["ID"] + '">' + data[i]["ExamName"] + '</option>';
            }
            $("#djlr_selectId").html(html);
        }
    });


}

//个人总成绩 导出
function ExportTotalScore() {
    var SchooId = $("#SchooId").val();//学院
    var TeamId = $("#TeamId").val();//班级


    var StudentInfo = $("#StudentInfo").val();//学生信息
    var ScoreQJ = $("#ScoreQJ").val();//分数区间

    var sjly = $("#sjly").val();//试卷来源

    //四个赛项的选择
    var hbzs_selectId = $("#hbzs_selectId").val();
    var sgdc_selectId = $("#sgdc_selectId").val();
    var fhbb_selectId = $("#fhbb_selectId").val();
    var djlr_selectId = $("#djlr_selectId").val();

    //校验分数区间
    if (ScoreQJ.length > 0) {
        var ScoreQJtxt = ScoreQJ + "";

        var sqjtxtlist = ScoreQJtxt.split('-');
        if (sqjtxtlist.length == 1 || sqjtxtlist.length > 2) {//格式错误
            layer.msg('搜索条件分数区间格式错误，正确格式如：60-70', function () { });
            return;
        }

        //再校验前后
        var to1 = sqjtxtlist[0];
        var to2 = sqjtxtlist[1];
        if (isNaN(to1)) {
            layer.msg('分数区间格式错误，不能包含英文字符！', function () { });
            return;
        }

        if (isNaN(to2)) {
            layer.msg('分数区间格式错误，不能包含英文字符！', function () { });
            return;
        }

        //必须-前的小于-后的值
        if (parseFloat(to1) > parseFloat(to2)) {
            layer.msg('分数区间格式错误，区间开始值不能大于结束值！', function () { });
            return;
        }
    }


    $.ajax({
        Type: "post",
        dataType: "json",
        url: '/Admin/PersonalTotalScore/ExportTotalScore',
        data: {
            "descby": descby, "SchooId": SchooId, "TeamId": TeamId, "StudentInfo": StudentInfo, "ScoreQJ": ScoreQJ,
            "sjly": sjly, "hbzs_selectId": hbzs_selectId, "sgdc_selectId": sgdc_selectId,
            "fhbb_selectId": fhbb_selectId, "djlr_selectId": djlr_selectId
        },
        async: false,
        success: function (data) {

            $("#downFile").attr("href", "/ashx/download.ashx?downurl=" + data[0]["filename"]);
            $("#tf").click();
        }
    });
}
