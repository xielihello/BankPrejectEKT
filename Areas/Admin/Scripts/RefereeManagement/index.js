/***************************************************************
  FileName:裁判员管理 javascript
  Copyright（c）2017-金融教育在线技术开发部
  Author:袁学
  Create Date:2017-3-30
 ******************************************************************/

$(function () {
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
    var RefereeName = $("#RefereeName").val();//裁判信息
    var PageSize = 10;

    $.ajax({
        Type: "post",
        dataType: "json",
        cache: false,
        contentType: "application/json; charset=utf-8",
        url: '/Admin/RefereeManagement/GetList',
        data: { "RefereeName": RefereeName, "page": page, "PageSize": PageSize },
        success: function (tb) {
            var html = '';
            var data = tb.Tb;//转换table
            if (tb != null && tb.Total != 0) {//table数据不为空
                for (var i = 0; i < data.length; i++) {
                    html += '<tr>';

                    //当前页面
                    var idx = 0;
                    if (page != "undefined" && page != null) {
                        idx = page;
                        idx = idx - 1;
                    }
                    html += '<td><span class="pie">' + ((idx * PageSize) + i + 1) + '</span></td>';



                    html += '<td><input type="checkbox" class="i-checks" name="input[]" value="' + data[i]["UId"] + '"></td>';
                    var UserName = data[i]["UserName"];
                    if (data[i]["UserName"] == null || data[i]["UserName"] == "null") {
                        UserName = "";
                    }
                    html += '<td><span class="pie">' + UserName + '</span></td>';
                    html += '<td><span class="pie">' + data[i]["UserNo"] + '</span></td>';
                    html += '<td><span class="pie">' + data[i]["UserPwd"] + '</span></td>';
                    html += '<td><a href="javascript:void(0);" onclick="Edit(' + data[i]["UId"] + ')" class=" btn-primary btn-sm"><i class="fa fa-pencil m-r-xxs"></i>编辑 </a></td> ';
                    html += '</tr>';
                }
            }
            $("#tablelist").html(html);
            //分页控件加载
            bootstrapPaginator("#PaginatorLibrary", tb, bindIngfo);//分页
            //样式重新加载
            redload();
        }
    });
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
        area: ['450px', '200px'], //宽高
        content: $("#Add")
    });

}

//新增保存
function BtnSubim() {

    var AddName = $("#AddName").val();

    if (AddName.length == 0) {
        layer.msg('请输入裁判员姓名！', function () { });
        return;
    }
    
    //表单提交
    $("#Addform").ajaxSubmit({
        type: "post",
        url: '/Admin/RefereeManagement/Add',
        data: $("#Addform").serialize(),
        success: function (data) {
            if (data == "1") {
                layer.closeAll();//关闭所有弹出框
                bindIngfo();
                layer.msg('操作成功', { icon: 1 });
            }
            if (data == "88") {
                layer.msg('裁判名称已经存在！', function () { });
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

    layer.confirm('您确定要删除所选裁判吗？', {
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
                url: '/Admin/RefereeManagement/DelReferee',
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


//批量新增 取消
function BatchFormRest() {
    layer.closeAll();//关闭
    $('#BatchAddform')[0].reset();//清空表单数据
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
        area: ['450px', '200px'], //宽高
        content: $("#BatchAdd")
    });
}

//批量新增 保存
function BatchBtnSubim() {

    var BatchAddNameNum = $("#BatchAddNameNum").val();

    if (BatchAddNameNum.length == 0) {
        layer.msg('请输入裁判员数量！', function () { });
        return;
    }
    if (parseInt(BatchAddNameNum) > 1000) {
        layer.msg('裁判员数量不能大于1000！', function () { });
        return;
    }


    //表单提交
    $("#BatchAddform").ajaxSubmit({
        type: "post",
        url: '/Admin/RefereeManagement/BatchAdd',
        data: $("#BatchAddform").serialize(),
        success: function (data) {

            if (data == "0") {
                layer.msg('操作失败', { icon: 2 });
                return;
            } else {
                layer.closeAll();//关闭所有弹出框
                bindIngfo();
                layer.alert('操作成功,批量新增了' + data + '个裁判！');
            }
        }
    });
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

var MUId = 0;
//编辑
function Edit(UId) {
    EditFormRest();
    layer.open({
        type: 1,
        title: '编辑',
        skin: 'layui-layer-lan', //样式类名
        anim: 2,
        shadeClose: false, //开启遮罩关闭
        area: ['450px', '280px'], //宽高
        content: $("#Edit")
    });
    GetListById(UId);
    MUId = UId;
}

//编辑时信息读取
function GetListById(UId) {
    $.ajax({
        Type: "post",
        dataType: "json",
        url: '/Admin/RefereeManagement/GetListById?UId=' + UId,
        async: false,
        success: function (data) {
            if (data.length > 0) {

                $("#EditName").val(data[0]["UserName"]);
                $("#Oldpwd").val(data[0]["UserPwd"]);
                $("#Newpwd").val(data[0]["UserPwd"]);
            }
        }
    });
}

//编辑 保存
function EditBtnSubim() {
    var EditName = $("#EditName").val();
    var Oldpwd = $("#Oldpwd").val();
    var Newpwd = $("#Newpwd").val();


    if (EditName.length == 0) {
        layer.msg('请输入裁判员姓名！', function () { });
        return;
    }
    if (Oldpwd.length == 0) {
        layer.msg('请输入新密码！', function () { });
        return;
    }
    if (Newpwd.length == 0) {
        layer.msg('请输入重复密码！', function () { });
        return;
    }
    if (Oldpwd != Newpwd) {
        layer.msg('两次输入密码不一致！', function () { });
        return;
    }

    var reg = /^[a-zA-Z0-9]+$/;
    if (!reg.test(Oldpwd)) {
        layer.msg('密码只能是字母或数字组成！', function () { });
        return;
    }

    //表单提交
    $("#Editform").ajaxSubmit({
        type: "post",
        url: '/Admin/RefereeManagement/Edit?UId=' + MUId,
        data: $("#Editform").serialize(),
        success: function (data) {
            if (data == "1") {
                layer.closeAll();//关闭所有弹出框
                bindIngfo();
                layer.msg('操作成功', { icon: 1 });
            }
            if (data == "88") {
                layer.msg('裁判名称已经存在！', function () { });
                return;
            }
            if (data == "99") {
                layer.msg('操作失败', { icon: 2 });
                return;
            }
        }
    });
}

function EditFormRest() {
    layer.closeAll();//关闭
    $('#Editform')[0].reset();//清空表单数据
}
