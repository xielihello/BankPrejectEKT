/***************************************************************
  FileName:教师端学员管理 javascript
  Copyright（c）2017-金融教育在线技术开发部
  Author:袁学
  Create Date:2017-4-11
 ******************************************************************/

$(function () {

    bindTeam();
    bindIngfo();
});

//搜索
function searchinfo() {
    bindIngfo();

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

//列表数据加载
function bindIngfo(page) {

    var TeamId = $("#TeamId").val();//班级
    var StudentInfo = $("#StudentInfo").val();//学生信息
    var PageSize = 10;

    $.ajax({
        Type: "post",
        dataType: "json",
        cache: false,
        contentType: "application/json; charset=utf-8",
        url: '/Admin/T_StudentManage/GetList',
        data: { "TeamId": TeamId, "StudentInfo": StudentInfo, "page": page, "PageSize": PageSize },
        success: function (data) {
            var tb = data;
            if (tb != null && tb.TableHTML.length > 0) {
                $("#tablelist").html(tb.TableHTML);
               
            } else {
                $("#tablelist").html("");
            }
            bootstrapPaginator("#PaginatorLibrary", tb, bindIngfo);//分页
            //样式加载
            redload();
        }
    });


}

//班级下拉框
function bindTeam() {
    $.ajax({
        Type: "post",
        dataType: "json",
        url: '/Admin/T_StudentManage/GetListTeam',
        async: false,
        success: function (data) {
            var html = '<option value="0">请选择班级名称</option>';
            for (var i = 0; i < data.length; i++) {
                html += '<option value="' + data[i]["Id"] + '">' + data[i]["TeamName"] + '</option>';
            }
            $("#TeamId").html(html);
        }
    });
}

//初始化密码
function Initialization_all_pwd() {
    var chks = document.getElementsByName('input[]');//name
    var chkstr = "";
    for (var i = 0; i < chks.length; i++) {
        if (chks[i].checked == true) {
            chkstr += chks[i].value + ",";
        }
    }

    if (chkstr.length == 0) {
        layer.msg('请选择要初始化的数据！', function () { });
        return;
    }
    chkstr = chkstr.substr(0, chkstr.length - 1);

    $.ajax({
        type: "POST",
        dataType: "text",
        url: '/Admin/T_StudentManage/InitializationStudentPwd',
        data: { "Ids": chkstr },//多个Id
        async: false,
        success: function (data) {
            if (data == "1") {
                layer.closeAll();//关闭所有弹出框
                bindIngfo();
                //去除全选
                var $checkboxAll = $(".checkbox-all")
                $checkboxAll.iCheck('uncheck');
                layer.msg('操作成功', { icon: 1 });

            }
            if (data == "99") {
                layer.msg('操作失败', { icon: 2 });
                return;
            }

        }
    })

}


//导出
function ExportStudent() {
    var SchooId = $("#SchooId").val();//学院
    var TeamId = $("#TeamId").val();//班级
    var TeacherName = $("#TeacherName").val();//教师名称
    var StudentInfo = $("#StudentInfo").val();//学生信息

    $.ajax({
        Type: "post",
        dataType: "json",
        url: '/Admin/T_StudentManage/Export',
        data: { "SchooId": SchooId, "TeamId": TeamId, "TeacherName": TeacherName, "StudentInfo": StudentInfo },
        async: false,
        success: function (data) {

            $("#downFile").attr("href", "/ashx/download.ashx?downurl=" + data[0]["filename"]);
            $("#tf").click();
        }
    });


}
