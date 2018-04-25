/***************************************************************
  FileName:学生管理 javascript
  Copyright（c）2017-金融教育在线技术开发部
  Author:袁学
  Create Date:2017-3-30
 ******************************************************************/

$(function () {

    bindSchool();
    bindTeam();
    bindIngfo();
});

//搜索
function searchinfo() {
    bindIngfo();

}



//列表数据加载
function bindIngfo(page) {
    var SchooId = $("#SchooId").val();//学院
    var TeamId = $("#TeamId").val();//班级
    var TeacherName = $("#TeacherName").val();//教师名称
    var StudentInfo = $("#StudentInfo").val();//学生信息
    var PageSize = 10;

    $.ajax({
        Type: "post",
        dataType: "json",
        cache: false,
        contentType: "application/json; charset=utf-8",
        url: '/Admin/StudentManagement/GetList',
        data: { "SchooId": SchooId, "TeamId": TeamId, "TeacherName": TeacherName, "StudentInfo": StudentInfo, "page": page, "PageSize": PageSize },
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
//学院下拉框
function bindSchool() {
    $.ajax({
        Type: "post",
        dataType: "json",
        url: '/Admin/StudentManagement/GetSchool',
        async: false,
        success: function (data) {
            var html = '<option value="0">请选择院校名称</option>';
            for (var i = 0; i < data.length; i++) {
                html += '<option value="' + data[i]["Id"] + '">' + data[i]["Name"] + '</option>';
            }
            $("#SchooId").html(html);
        }
    });
}

//班级下拉框
function bindTeam() {
    $.ajax({
        Type: "post",
        dataType: "json",
        url: '/Admin/StudentManagement/GetTeam?SchooId=' + $("#SchooId").val(),
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

//联动
function SelctSchool() {
    bindTeam();
}

//批量删除
function del_all() {
    var chks = document.getElementsByName('input[]');//name
    var chkstr = "";
    for (var i = 0; i < chks.length; i++) {
        if (chks[i].checked == true) {
            chkstr += chks[i].value + ",";
        }
    }

    if (chkstr.length == 0) {
        layer.msg('请选择要删除的数据！', function () { });
        return;
    }

    chkstr = chkstr.substr(0, chkstr.length - 1);

    layer.confirm('您确定要删除所选学生吗？', {
        title: '删除',
        btn: ['确定', '取消'],
        shadeClose: true, //开启遮罩关闭
        skin: 'layui-layer-lan'
        //按钮
    },
        function () {
            $.ajax({
                type: "POST",
                dataType: "text",
                url: '/Admin/StudentManagement/DelStudent',
                data: { "Ids": chkstr },//多个Id
                success: function (data) {
                    if (data == "1") {
                        layer.closeAll();//关闭所有弹出框
                        bindIngfo();

                        layer.msg('操作成功', { icon: 1 });

                    }
                    if (data == "99") {
                        layer.msg('操作失败', { icon: 2 });
                        return;
                    }

                }
            })
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
        url: '/Admin/StudentManagement/InitializationStudentPwd',
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

//新增弹框
function AddInfo() {
    FormRest();
    layer.open({
        type: 1,
        title: '新增',
        skin: 'layui-layer-lan', //样式类名
        anim: 2,
        shadeClose: false, //开启遮罩关闭
        area: ['500px', '320px'], //宽高
        content: $("#Add")
    });
    AddbindSchool();
}

//新增学院下拉框
function AddbindSchool() {
    $.ajax({
        Type: "post",
        dataType: "json",
        url: '/Admin/StudentManagement/GetSchool',
        async: false,
        success: function (data) {
            var html = '<option value="0">请选择院校名称</option>';
            for (var i = 0; i < data.length; i++) {
                html += '<option value="' + data[i]["Id"] + '">' + data[i]["Name"] + '</option>';
            }
            $("#AddSchoolId").html(html);
        }
    });
}

//新增班级数据联动
function AddTeamSelect() {
    $.ajax({
        Type: "post",
        dataType: "json",
        url: '/Admin/StudentManagement/GetTeam?SchooId=' + $("#AddSchoolId").val(),
        async: false,
        success: function (data) {
            var html = '<option value="0">请选择班级名称</option>';
            for (var i = 0; i < data.length; i++) {
                html += '<option value="' + data[i]["Id"] + '">' + data[i]["TeamName"] + '</option>';
            }
            $("#AddTeamId").html(html);
        }
    });
}

//新增保存
function BtnSubim() {
    var AddSchoolId = $("#AddSchoolId").val();
    var AddTeamId = $("#AddTeamId").val();
    var AddStudentNo = $("#AddStudentNo").val();
    var AddStudentName = $("#AddStudentName").val();
    if (AddSchoolId == "0") {
        layer.msg('请选择院校！', function () { });
        return;
    }
    if (AddTeamId == "0") {
        layer.msg('请选择班级！', function () { });
        return;
    }
    if (AddStudentNo.length == 0) {
        layer.msg('请输入学生编号！', function () { });
        return;
    }
    if (AddStudentNo.length > 15) {
        layer.msg('学生编号长度不能大于15位！', function () { });
        return;
    }

    if (AddStudentName.length == 0) {
        layer.msg('请输入学生姓名！', function () { });
        return;
    }

    //表单提交
    $("#Addform").ajaxSubmit({
        type: "post",
        url: '/Admin/StudentManagement/Add',
        data: $("#Addform").serialize(),
        success: function (data) {
            if (data == "1") {
                layer.closeAll();//关闭所有弹出框
                bindIngfo();

                layer.msg('操作成功', { icon: 1 });
            }
            if (data == "88") {
                layer.msg('学生编号已经存在！', function () { });
                return;
            }
            if (data == "99") {
                layer.msg('操作失败', { icon: 2 });
                return;
            }
        }
    });
}

//新增取消
function FormRest() {
    layer.closeAll();//关闭
    $('#Addform')[0].reset();//清空表单数据
}

//批量新增
function BatchAddInfo() {
    BatchFormRest();
    layer.open({
        type: 1,
        title: '批量新增',
        skin: 'layui-layer-lan', //样式类名
        anim: 2,
        shadeClose: false, //开启遮罩关闭
        area: ['500px', '320px'], //宽高
        content: $("#BatchAdd")
    });
    BatchAddbindSchool();
}

//整数校验通用规则
function TcheckeNum(Id) {
    var dqnum = $("#" + Id).val();
    $("#" + Id).val(dqnum.replace(/\D/g, ''));
    dqnum = $("#" + Id).val();
    if (isNaN(dqnum)) {
        $("#" + Id).val('');

    }
    var patrn = /^[0-9]*[1-9][0-9]*$/;
    if (!patrn.test(dqnum)) {
        $("#" + Id).val('');

    }

    automaticStuNo();
}
function checkeNum(Id) {
    var dqnum = $("#" + Id).val();
    $("#" + Id).val(dqnum.replace(/\D/g, ''));
    dqnum = $("#" + Id).val();
    if (isNaN(dqnum)) {
        $("#" + Id).val('');

    }
    var patrn = /^[0-9]*[1-9][0-9]*$/;
    if (!patrn.test(dqnum)) {
        $("#" + Id).val('');

    }

    
}
//批量新增院校下拉框
function BatchAddbindSchool() {
    $.ajax({
        Type: "post",
        dataType: "json",
        url: '/Admin/StudentManagement/GetSchool',
        async: false,
        success: function (data) {
            var html = '<option value="0">请选择院校名称</option>';
            for (var i = 0; i < data.length; i++) {
                html += '<option value="' + data[i]["Id"] + '">' + data[i]["Name"] + '</option>';
            }
            $("#BatchAddSchoolId").html(html);
        }
    });
}

//批量新增 联动班级
function BatchAddTeamSelect() {
    $.ajax({
        Type: "post",
        dataType: "json",
        url: '/Admin/StudentManagement/GetTeam?SchooId=' + $("#BatchAddSchoolId").val(),
        async: false,
        success: function (data) {
            var html = '<option value="0">请选择班级名称</option>';
            for (var i = 0; i < data.length; i++) {
                html += '<option value="' + data[i]["Id"] + '">' + data[i]["TeamName"] + '</option>';
            }
            $("#BatchAddTeamId").html(html);
        }
    });
}

//批量新增 取消
function BatchFormRest() {
    layer.closeAll();//关闭
    $('#BatchAddform')[0].reset();//清空表单数据
}

//批量新增 学生数量自动计算
function automaticStuNo() {
    var BatchS = $("#BatchS").val();
    var BatchE = $("#BatchE").val();

    if (BatchS.length > 0 && BatchE.length > 0) {
        var BianHao = parseInt(BatchE) - parseInt(BatchS);
        if (parseInt(BatchE) >= parseInt(BatchS)) {
            $("#BatchAddStudentNum").val((BianHao + 1));
        } else {
            $("#BatchAddStudentNum").val('');
        }

    } else {
        $("#BatchAddStudentNum").val('');
    }
}

//批量新增 保存
function BatchBtnSubim() {
    var BatchAddSchoolId = $("#BatchAddSchoolId").val();
    var BatchAddTeamId = $("#BatchAddTeamId").val();
    var BatchS = $("#BatchS").val();
    var BatchE = $("#BatchE").val();
    var BatchAddStudentNum = $("#BatchAddStudentNum").val();
    if (BatchAddSchoolId == "0") {
        layer.msg('请选择院校！', function () { });
        return;
    }
    if (BatchAddTeamId == "0") {
        layer.msg('请选择班级！', function () { });
        return;
    }
    if (BatchS.length == 0) {
        layer.msg('请输入学生开始编号！', function () { });
        return;
    }
    if (BatchE.length == 0) {
        layer.msg('请输入学生结束编号！', function () { });
        return;
    }
    if (BatchS.length > 15) {
        layer.msg('学生开始编号长度不能大于15位！', function () { });
        return;
    }
    if (BatchE.length > 15) {
        layer.msg('学生结束编号长度不能大于15位！', function () { });
        return;
    }
    if (parseInt(BatchS) > parseInt(BatchE)) {
        layer.msg('学生开始编号不能大于结束编号！', function () { });
        return;
    }
    if (parseInt(BatchAddStudentNum) > 100) {
        layer.msg('学生编号区间不能大于100！', function () { });
        return;
    }

    //不自动关闭 time:-1  亮度页面不可点击 shade:0.01
    layer.msg('正在努力生成学员信息，请稍等', {
        icon: 16, shade: 0.01, time: -1
    });
    //表单提交
    $("#BatchAddform").ajaxSubmit({
        type: "post",
        url: '/Admin/StudentManagement/BatchAdd',
        data: $("#BatchAddform").serialize(),
        success: function (data) {

            if (data == "0") {
                layer.closeAll();//关闭所有弹出框
                layer.msg('操作失败', { icon: 2 });
                return;
            } else {
                layer.closeAll();//关闭所有弹出框
                bindIngfo();

                layer.alert('操作成功,批量新增了' + data + '个学生！');
            }
        }
    });
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
        url: '/Admin/StudentManagement/Export',
        data: { "SchooId": SchooId, "TeamId": TeamId, "TeacherName": TeacherName, "StudentInfo": StudentInfo },
        async: false,
        success: function (data) {

            $("#downFile").attr("href", "/ashx/download.ashx?downurl=" + data[0]["filename"]);
            $("#tf").click();
        }
    });


}

var MUId = 0;
//编辑
function EditStudent(UId) {
    EditFormRest();
    layer.open({
        type: 1,
        title: '编辑',
        skin: 'layui-layer-lan', //样式类名
        anim: 2,
        shadeClose: false, //开启遮罩关闭
        area: ['500px', '380px'], //宽高
        content: $("#Edit")
    });
    EditBindScholl();
    GetStudentById(UId);
    MUId = UId;
}

//编辑 绑定院校
function EditBindScholl() {
    $.ajax({
        Type: "post",
        dataType: "json",
        url: '/Admin/StudentManagement/GetSchool',
        async: false,
        success: function (data) {
            var html = '<option value="0">请选择院校名称</option>';
            for (var i = 0; i < data.length; i++) {
                html += '<option value="' + data[i]["Id"] + '">' + data[i]["Name"] + '</option>';
            }
            $("#EditSchoolId").html(html);
        }
    });
}

//编辑 联动班级
function EditTeamSelect() {
    $.ajax({
        Type: "post",
        dataType: "json",
        url: '/Admin/StudentManagement/GetTeam?SchooId=' + $("#EditSchoolId").val(),
        async: false,
        success: function (data) {
            var html = '<option value="0">请选择班级名称</option>';
            for (var i = 0; i < data.length; i++) {
                html += '<option value="' + data[i]["Id"] + '">' + data[i]["TeamName"] + '</option>';
            }
            $("#EditTeamId").html(html);
        }
    });
}

//读取当前选择行数据
function GetStudentById(UId) {
    $.ajax({
        Type: "post",
        dataType: "json",
        url: '/Admin/StudentManagement/GetStudentById?UId=' + UId,
        async: false,
        success: function (data) {
            if (data.length > 0) {
                $("#EditSchoolId").val(data[0]["SchoolId"]);

                EditTeamSelect();

                $("#EditTeamId").val(data[0]["UserClassId"]);
                $("#EditStudentName").val(data[0]["UserName"]);

                $("#EditStudentNo").html(data[0]["StudentNo"]);
                $("#EditUserNo").html(data[0]["UserNo"]);
                $("#EditStudenPwd").val(data[0]["UserPwd"]);
            }
        }
    });
}

//编辑 保存按钮
function EditBtnSubim() {
    var EditSchoolId = $("#EditSchoolId").val();
    var EditTeamId = $("#EditTeamId").val();
    var EditStudentName = $("#EditStudentName").val();
    var EditStudenPwd = $("#EditStudenPwd").val();
    if (EditSchoolId == "0") {
        layer.msg('请选择院校！', function () { });
        return;
    }
    if (EditTeamId == "0") {
        layer.msg('请选择班级！', function () { });
        return;
    }
    if (EditStudentName.length == 0) {
        layer.msg('请输入学生姓名！', function () { });
        return;
    }
    if (EditStudenPwd.length == 0) {
        layer.msg('请输入登录密码！', function () { });
        return;
    }
    var reg = /^[a-zA-Z0-9]+$/;
    if (!reg.test(EditStudenPwd)) {
        layer.msg('密码只能是字母或数字组成！', function () { });
        return;
    }
    //表单提交
    $("#Editform").ajaxSubmit({
        type: "post",
        url: '/Admin/StudentManagement/Edit?UId=' + MUId,
        data: $("#Editform").serialize(),
        success: function (data) {
            if (data == "1") {
                layer.closeAll();//关闭所有弹出框
                bindIngfo();

                layer.msg('操作成功', { icon: 1 });
            }

            if (data == "99") {
                layer.msg('操作失败', { icon: 2 });
                return;
            }
        }
    });
}

//编辑关闭
function EditFormRest() {
    layer.closeAll();//关闭
    $('#Editform')[0].reset();//清空表单数据
}

//账户状态
function EdtiState(UId, value) {

    $.ajax({
        url: '/Admin/StudentManagement/EdtiState?UId=' + UId + "&value=" + value,
        type: 'POST',
        dataType: 'text',
        async: false,
        success: function (data) {

            if (data == "1") {
                layer.closeAll();//关闭所有弹出框

                layer.msg('状态修改成功', { icon: 6 });

            }
            if (data == "99") {
                layer.msg('操作失败', { icon: 2 });
                return;
            }


        }
    });
}