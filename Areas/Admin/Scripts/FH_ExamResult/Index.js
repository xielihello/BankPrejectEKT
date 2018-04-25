/***************************************************************
  FileName:复核报表成绩管理 javascript
  Copyright（c）2017-金融教育在线技术开发部
  Author:袁学
  Create Date:2017-4-20
 ******************************************************************/

//分数排序
function ResultDesc() {
    if (descby == "ER_Score desc") {
        descby = "ER_Score asc";
    } else {
        descby = "ER_Score desc";
    }
    bindIngfo();
}

$(function () {
    bindSchool();
    bindTeam();
    bindIngfo();
});

//搜索
function searchinfo() {

    bindIngfo();

}
var descby = "ER_Score desc";
//列表数据加载
function bindIngfo(page) {
    var E_Name = $("#E_Name").val();//考试名称
    var E_Type = $("#E_Type").val();//考试模式

    var SchooId = $("#SchooId").val();//学院
    var TeamId = $("#TeamId").val();//班级

    var P_Kind = $("#P_Kind").val();//试卷属性
    var P_Custom2 = $("#P_Custom2").val();//试卷来源

    var StudentInfo = $("#StudentInfo").val();//学生信息
    var ScoreQJ = $("#ScoreQJ").val();//分数区间
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
        url: '/Admin/FH_ExamResult/GetList',
        data: { "descby": descby, "E_Name": E_Name, "E_Type": E_Type, "SchooId": SchooId, "TeamId": TeamId, "P_Kind": P_Kind, "P_Custom2": P_Custom2, "StudentInfo": StudentInfo, "ScoreQJ": ScoreQJ, "page": page, "PageSize": PageSize },
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


                html += '<td><span class="pie" >' + data[i]["E_Name"] + '</span></td>';
                //管理员
                if (parseInt(UserType) == 1) {
                    //试卷属性
                    html += '<td><span class="pie">' + data[i]["P_Kind"] + '</span></td>';

                    //试卷来源
                    if (data[i]["AUserName"] == "null" || data[i]["AUserName"] == null || data[i]["AUserName"].length == 0) {
                        html += '<td><span class="pie">' + data[i]["UR_Custom2"] + '</span></td>';
                    } else {
                        html += '<td><span class="pie">' + data[i]["AUserName"] + '</span></td>';
                    }

                }
                //考试类型
                html += '<td><span class="pie">' + data[i]["E_Type"] + '</span></td>';

                //学生账号
                html += '<td><span class="pie" >' + data[i]["UserNo"] + '</span></td>';
                if (data[i]["UserName"] == null) {
                    html += '<td><span class="pie" >---</span></td>';
                } else {
                    //学生姓名
                    html += '<td><span class="pie" >' + data[i]["UserName"] + '</span></td>';
                }

                //管理员
                if (parseInt(UserType) == 1) {
                    //院校
                    html += '<td><span class="pie" >' + data[i]["Name"] + '</span></td>';

                }
                //班级
                html += '<td><span class="pie" >' + data[i]["TeamName"] + '</span></td>';

                //管理员
                if (parseInt(UserType) == 1) {
                    //班级教师
                    var txtCustom2 = data[i]["Custom2"] + "";
                    if (txtCustom2.length > 10) {
                        txtCustom2 = txtCustom2.substr(0, 7) + "...";
                    }
                    html += '<td><span class="pie">' + txtCustom2 + '</span></td>';
                }
                //试卷总分
                html += '<td><span class="pie" >' + data[i]["Score"] + '</span></td>';
                //试卷得分
                html += '<td><span class="pie" >' + data[i]["ER_Score"] + '</span></td>';
                //提交时间
                var tjtime = data[i]["ER_AddTime"] + "";
                html += '<td><span class="pie" >' + tjtime.replace("T", " ").substr(0, 19) + '</span></td>';
                //操作查看明细
                html += '<td><a href="javascript:void(0);" onclick="See(' + data[i]["ER_EId"] + ',' + data[i]["ER_PId"] + ',' + data[i]["ER_MId"] + ')" class=" btn-info btn-sm"><i class="fa fa-lightbulb-o m-r-xxs"></i>查看明细 </a></td>';
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
        url: '/Admin/FH_ExamResult/GetSchool',
        async: false,
        success: function (data) {
            var html = '<option value="0">分行名称</option>';
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
        url: '/Admin/FH_ExamResult/GetTeam' + wheres,
        async: false,
        success: function (data) {
            var html = '<option value="0">支行名称</option>';
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

//查看明细
function See(Eid, Pid, Mid) {
    window.open("/FH_Management/Preview?Eid=" + Eid + "&Pid=" + Pid + "&UIds=" + Mid + "&Type=1");
 
}

//成绩导出
function ExportExamResult() {
    var E_Name = $("#E_Name").val();//考试名称
    var E_Type = $("#E_Type").val();//考试模式

    var SchooId = $("#SchooId").val();//学院
    var TeamId = $("#TeamId").val();//班级

    var P_Kind = $("#P_Kind").val();//试卷属性
    var P_Custom2 = $("#P_Custom2").val();//试卷来源

    var StudentInfo = $("#StudentInfo").val();//学生信息
    var ScoreQJ = $("#ScoreQJ").val();//分数区间
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
        url: '/Admin/FH_ExamResult/ExportExamResult',
        data: { "descby": descby, "E_Name": E_Name, "E_Type": E_Type, "SchooId": SchooId, "TeamId": TeamId, "P_Kind": P_Kind, "P_Custom2": P_Custom2, "StudentInfo": StudentInfo, "ScoreQJ": ScoreQJ },
        async: false,
        success: function (data) {

            $("#downFile").attr("href", "/ashx/download.ashx?downurl=" + data[0]["filename"]);
            $("#tf").click();
        }
    });
}
