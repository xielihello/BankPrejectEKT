 
$(function () {

    //bindSchool();
    //bindTeam();
    bindIngfo();

    redload();

}); 
//搜索
function searchinfo() {
    bindIngfo();
    redload();
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
    var SchooName = $("#SchooName").val();//学院 
    var TeacherName = $("#TeacherName").val();//教师信息 
    var PageSize = 10;
    //alert(page)
    $.ajax({
        Type: "post",
        url: '/Admin/TeacherManagement/GetStudentList',
    dataType: "json", cache: false,
    contentType: "application/json; charset=utf-8",
    data: { "SchooName": SchooName, "TeacherName": TeacherName, "page": page, "PageSize": PageSize },
    success: function (data) {
        var tb = data;
        if (tb != null && tb.TableHTML.length > 0) {
            $("#tablelist").html(tb.TableHTML);
        } else {
            $("#tablelist").html("");
        }
        bootstrapPaginator("#PaginatorLibrary", tb, bindIngfo)//分页

        redload();

    }
});


} 


//学院下拉框
function bindSchool() {
    $.ajax({
        Type: "post",
        dataType: "json",
        url: '/Admin/TeacherManagement/GetSchool',
        async: false,
        success: function (data) {
            var html = '<option value="0">请选择分行名称</option>';
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
        url: '/Admin/TeacherManagement/GetTeam',
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

//批量删除弹窗
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

    layer.confirm('您确定要删除所选教师吗？', {
        title: '删除',
        btn: ['确定删除', '取消操作'],
        shadeClose: true, //开启遮罩关闭
        skin: 'layui-layer-lan'
        //按钮
    },
        function () {
            $.ajax({
                type: "POST",
                dataType: "text",
                url: '/Admin/TeacherManagement/DelStudent',
                data: { "Ids": chkstr },//多个Id
                success: function (data) {
                    if (data == "1") {
                        layer.closeAll();//关闭所有弹出框
                        bindIngfo();
                        redload();
                        layer.msg('操作成功', { icon: 1 });
                        //去除全选
                        var $checkboxAll = $(".checkbox-all")
                        $checkboxAll.iCheck('uncheck');
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
        url: '/Admin/TeacherManagement/InitializationStudentPwd',
        data: { "Ids": chkstr },//多个Id
        async: false,
        success: function (data) {
            if (data == "1") {
                layer.closeAll();//关闭所有弹出框
                bindIngfo();
                redload();
                layer.msg('操作成功', { icon: 1 });
                //去除全选
                var $checkboxAll = $(".checkbox-all")
                $checkboxAll.iCheck('uncheck');
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
        area: ['500px', '360px'], //宽高
        content: $("#Add")
    });
    AddbindSchool();
}

//新增学院下拉框
function AddbindSchool() {
    $.ajax({
        Type: "post",
        dataType: "json",
        url: '/Admin/TeacherManagement/GetSchool',
        async: false,
        success: function (data) {
            var html = '<option value="0">请选择分行名称</option>';
            for (var i = 0; i < data.length; i++) {
                html += '<option value="' + data[i]["Id"] + '" title="' + data[i]["Name"] + '" >' + (data[i]["Name"].length<=10?data[i]["Name"]:data[i]["Name"].substring(0,10)+".." )+ '</option>';
            }
            $("#AddSchoolId").html(html);
        }
    });
}

function getByteLen(val) {
    var len = 0;
    for (var i = 0; i < val.length; i++) {
        var a = val.charAt(i);
        if (a.match(/[^\x00-\xff]/ig) != null) {
            len += 2;
        }
        else {
            len += 1;
        }
    }
    return len;
}
//新增保存
function BtnSubim() {
    var AddSchoolId = $("#AddSchoolId").find("option:selected").val();
    var AddTeacherName = $("#AddTeacherName").val();
    var AddPhone = $("#AddPhone").val();
    var AddEmail = $("#AddEmail").val();
    var AddSex = "";
    $("AddSex").each(function () {
        if ((this)[0].cchecked) {
            AddSex = $(this).val();
        }
    })
        
    //var AddSex = $("AddSex").val();
    //var AddTeamId = $("#AddTeamId").val();
    //var AddStudentNo = $("#AddStudentNo").val(); 
    if (AddSchoolId == "0") {
        layer.msg('请选择分行！', function () { });
        $("#AddSchoolId").focus();
        return;
    }    
    if (IsNullOrEmpty(AddTeacherName)) {
        layer.msg('请输入分行管理员姓名！', function () { });
        $("#AddTeacherName").focus();
        return;
    }
    if (getByteLen(AddTeacherName) > 20) {
        layer.msg('分行管理员姓名长度不能大于20汉字！', function () { });
        return;
    }
    if (!IsNullOrEmpty(AddPhone)) { 
        if (IsPhone(AddPhone)) {
            layer.msg('请输入正确的手机号码！', function () { });
            $("#AddPhone").focus();
            return;

        }
    } if (!IsNullOrEmpty(AddEmail)) {
        if (IsEmail(AddEmail) != true) {
            layer.msg('请输入正确的邮箱格式！', function () { });
            $("#AddEmail").focus();
            return;
        }
    }

    //表单提交
    $("#Addform").ajaxSubmit({
        type: "post",
        url: '/Admin/TeacherManagement/Add',
        data: $("#Addform").serialize(),
        success: function (data) {
            if (data == "1") {
                layer.closeAll();//关闭所有弹出框
                bindIngfo();
                redload();
                layer.msg('操作成功', { icon: 1 });
            }
            if (data == "88") {
                layer.msg('手机号码已经存在', { icon: 2 });
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
        area: ['500px', '260px'], //宽高
        content: $("#BatchAdd")
    });
    BatchAddbindSchool();
}

//整数校验通用规则
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
        url: '/Admin/TeacherManagement/GetSchool',
        async: false,
        success: function (data) {
            var html = '<option value="0">请选择分行名称</option>';
            for (var i = 0; i < data.length; i++) {
                html += '<option value="' + data[i]["Id"] + '">' + data[i]["Name"] + '</option>';
            }
            $("#BatchAddSchoolId").html(html);
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
        if (parseInt(BatchE) > parseInt(BatchS)) {
            $("#BatchAddStudentNum").val((BianHao + 1));
        } else {
            $("#BatchAddStudentNum").val('');
        }

    }
}

//批量新增 保存
function BatchBtnSubim() {
    var BatchAddSchoolId = $("#BatchAddSchoolId").val();  
    var BatchAddNameNum = $("#BatchAddStudentNum").val();
    if (BatchAddSchoolId == "0") {
        layer.msg('请选择分行！', function () { });
        $("#BatchAddSchoolId").focus();
        return;
    }    
    //if (BatchAddStudentNum > 100) {
    //    layer.msg('请选择院校！', function () { });
    //    $("#BatchAddSchoolId").focus();
    //    return;
    //}
    //表单提交
    $("#BatchAddform").ajaxSubmit({
        type: "post",
        url: '/Admin/TeacherManagement/BatchAdd',
        data: $("#BatchAddform").serialize(),
        success: function (data) {
          //  alert("aaa");
            if (data == "0") {
                layer.msg('操作失败', { icon: 2 });
                return;
            } else {
                layer.closeAll();//关闭所有弹出框
                bindIngfo();
                redload();
                layer.alert('操作成功,批量新增了' + data + '个教师！');
            }
        }
    });
}
function EnterPress(e) { //传入 event 
    var e = e || window.event;
    if (e.keyCode == 13) {
        BatchBtnSubim();
       // alert("aa");
    }
}
//导出
function ExportStudent() {
    var SchooName = $("#SchooName").val();//学院 
    var TeacherName = $("#TeacherName").val();//教师信息 

    $.ajax({
        Type: "post",
        dataType: "json",
        url: '/Admin/TeacherManagement/Export',
        data: { "SchooName": SchooName, "TeacherName": TeacherName },
        async: false,
        success: function (data) {
            $("#downFile").attr("href", "/ashx/download.ashx?downurl=" + data[0]["filename"]);
              
            $("#tf").click();
        }
    });


}

//账户状态
function EdtiState(UId, value) {

    $.ajax({
        url: '/Admin/TeacherManagement/EdtiState?UId=' + UId + "&value=" + value,
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
        area: ['500px', '450px'], //宽高
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
        url: '/Admin/TeacherManagement/GetSchool',
        async: false,
        success: function (data) {
            var html = '<option value="0">请选择分行名称</option>';
            for (var i = 0; i < data.length; i++) {
                html += '<option value="' + data[i]["Id"] + '" title="' + data[i]["Name"] + '" >' + (data[i]["Name"].length <= 10 ? data[i]["Name"] : data[i]["Name"].substring(0, 10) + "..") + '</option>';
            }
            $("#EditSchoolId").html(html);
        }
    });
}

//读取当前选择行数据
function GetStudentById(UId) {
    $.ajax({
        Type: "post",
        dataType: "json",
        url: '/Admin/TeacherManagement/GetStudentById?UId=' + UId,
        async: false,
        success: function (data) {
            if (data.length > 0) {
                debugger;
                $("#EditSchoolId").val(data[0]["SchoolId"]);
                var sex = data[0]["UserSex"] 
                if (!IsNullOrEmpty(sex)) {
                    $("input[name='EditSex']").each(function () {
                        var txt = $(this).val();
                        if (txt == sex) { 
                            $(this).attr("checked", "checked");
                            $(this).parent().addClass("checked");
                        } else {
                            $(this).removeAttr("checked");
                            $(this).parent().removeClass("checked");
                        }
                    });
                }
                //EditTeamSelect(); 
                $("#EditName").val(data[0]["UserName"]);

                $("#EditNo").html(data[0]["StudentNo"]);
                $("#EditUserNo").html(data[0]["UserNo"]);
                $("#EditPwd").val(data[0]["UserPwd"]);
                $("#EditPhone").val(data[0]["UserPhone"]);
                $("#EditEmail").val(data[0]["UserEmail"]);
            }
        }
    });
}


//编辑 保存按钮
function EditBtnSubim() {
    var EditSchoolId = $("#EditSchoolId").val(); 
    var EditName = $("#EditName").val(); 
    var EditPwd = $("#EditPwd").val();
    var EditPhone = $("#EditPhone").val();
    var EditEmail = $("#EditEmail").val();

    var EditSex = "";
    $("EditSex").each(function () {
        if ((this)[0].cchecked) {
            EditSex = $(this).val();
        }
    })
    var reg = /^[a-zA-Z0-9]+$/;
    if (!reg.test(EditPwd)) {
        layer.msg('密码只能是字母或数字组成！', function () { });
        return;
    }
    if (EditSchoolId == "0") {
        layer.msg('请选择分行！', function () { });
        return;
    }
    if (IsNullOrEmpty(EditName)) {
        layer.msg('请输入分行管理员名称！', function () { });
        $("#EditName").focus();
        return;
    }
    if (getByteLen(EditName) > 20) {
        layer.msg('分行管理员姓名长度不能大于20汉字！', function () { });
        return;
    }
    if (IsNullOrEmpty(EditPwd)) {
        layer.msg('请登录密码！', function () { });
        $("#EditPwd").focus();
        return;
    }
    if (!IsNullOrEmpty(EditPhone)) {
        if (IsPhone(EditPhone)) {
            layer.msg('请输入正确的手机号码！', function () { });
            $("#EditPhone").focus();
            return;

        }
    } 
    if (!IsNullOrEmpty(EditEmail)) {
        if (IsEmail(EditEmail) != true) {
            layer.msg('请输入正确的邮箱格式！', function () { });
            $("#EditEmail").focus();
            return;
        }
    }
    //表单提交
    $("#Editform").ajaxSubmit({
        type: "post",
        url: '/Admin/TeacherManagement/Edit?UId=' + MUId,
        data: $("#Editform").serialize(),
        success: function (data) {
            if (data == "1") {
                layer.closeAll();//关闭所有弹出框
                bindIngfo();
                redload();
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