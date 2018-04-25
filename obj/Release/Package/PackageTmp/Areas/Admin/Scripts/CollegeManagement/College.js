

$(function () {
    $('#Save').bind('submit', checkform);
    box();
    DataBind();
});

//加载列表
function DataBind(page) {
    var Name = $("#Name").val();
    var PageSize = 10;
    $.ajax({
        type: "POST",
        dataType: "json",
        async: false,
        url: '/Admin/CollegeManagement/CollegeGetList',
        data: { Name: Name, page: page, PageSize: PageSize },
        success: function (tb) {
            var html = '';
            var data = tb.Tb;//转换table
            for (var i = 0; i < data.length; i++) {
                //当前页面
                var idx = 0;
                if (page != "undefined" && page != null) {
                    idx = page;
                    idx = idx - 1;
                }
                html += " <tr><td>" + ((idx * PageSize) + i + 1) + "</td>";
                html += "<td><input type='checkbox' class='i-checks' name='input[]' value='" + data[i]["Id"] + "'></td>"
                html += " <td><span class='pie'>" + data[i]["Name"] + "</span> </td>";
                html += "  <td>";
                if (data[i]["Jurisdiction"].indexOf("1") > -1) {
                    html += " <span class='m-l-md'>货币知识</span>";
                }
                if (data[i]["Jurisdiction"].indexOf("2") > -1) {
                    html += " <span class='m-l-md'>手工点钞</span>";
                }
                if (data[i]["Jurisdiction"].indexOf("3") > -1) {
                    html += " <span class='m-l-md'>复核报表</span>";
                }
                if (data[i]["Jurisdiction"].indexOf("4") > -1) {
                    html += " <span class='m-l-md'>单据录入</span>";
                }
                html += "</td>";
                html += " <td><a href='javascript:void(0);' onclick=\"Edit(" + data[i]["Id"] + ")\" class=' btn-primary btn-sm'><i class='fa fa-pencil m-r-xxs'></i>编辑 </a></td>";
            }
            $("#table").html(html);
            bootstrapPaginator("#PaginatorLibrary", tb, DataBind);//分页
            box();
        }
    });
}

$("#Query").click(function () {
    DataBind();
});

function box() {
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

//验证
function checkform() {
    var chks = document.getElementsByName('authority');//name
    var chkstr = "";
    for (var i = 0; i < chks.length; i++) {
        if (chks[i].checked == true) {
            chkstr += chks[i].value + ",";
        }
    }
    if ($("#CollegeName").val() == "") {
        layer.msg('院校名称是必填项！', function () { });
        return false;
    }
    if (chkstr == "") {
        layer.msg('用户权限是必选项！', function () { });
        return false;
    }
    $("#Authority").val(chkstr);

    var College
    $.ajax({
        type: "POST",
        // dataType: "text",
        async: false,
        url: '/Admin/CollegeManagement/CollegeCheck',
        data: { CollegeName: $("#CollegeName").val() },
        success: function (data) {
            College = data
        }
    });
    if (College >= 1) {
        layer.msg('您好,不能添加重复的院校名称！', function () { });
        return false;
    }
    return true;
}

function Added() {
    //去除全选
    $("#CollegeName").val("");
    var $Hb = $("#Hb");
    $Hb.iCheck('uncheck');
    var $Dc = $("#Dc");
    $Dc.iCheck('uncheck');
    var $Fh = $("#Fh");
    $Fh.iCheck('uncheck');
    var $Dj = $("#Dj");
    $Dj.iCheck('uncheck');
    layer.open({
        title: '新增院校',
        type: 1,
        skin: 'layui-layer-lan', //样式类名
        anim: 2,
        //shadeClose: true, //开启遮罩关闭
        area: ['500px', '280px'], //宽高
        content: $("#Add")
    });
}

//编辑
function Edit(Id) {
    $("#EId").val(Id);
    $.ajax({
        type: "POST",
        dataType: "json",
        async: false,
        url: '/Admin/CollegeManagement/CollegeEdit',
        data: { Id: Id },
        success: function (data) {
            $("#CollegeName1").val(data[0]["Name"]);
            if (data[0]["Jurisdiction"].indexOf("1") > -1) {
                $("#Hb1").prop("checked", "checked");
            }
            else {
                $("#Hb1").prop("checked", false);
            }
            if (data[0]["Jurisdiction"].indexOf("2") > -1) {
                $("#Dc1").prop("checked", "checked");
            }
            else {
                $("#Dc1").prop("checked", false);
            }
            if (data[0]["Jurisdiction"].indexOf("3") > -1) {
                $("#Fh1").prop("checked", "checked");
            }
            else {
                $("#Fh1").prop("checked", false);
            }
            if (data[0]["Jurisdiction"].indexOf("4") > -1) {
                $("#Dj1").prop("checked", "checked");
            }
            else {
                $("#Dj1").prop("checked", false);
            }
            box();
            layer.open({
                title: '编辑院校',
                type: 1,
                skin: 'layui-layer-lan', //样式类名
                anim: 2,
                //shadeClose: true, //开启遮罩关闭
                area: ['500px', '280px'], //宽高
                content: $("#edit")
            });
        }
    });
}

//编辑保存
function EditSave() {
    var CollegeName = $("#CollegeName1").val();
    var chks = document.getElementsByName('authority1');//name
    var chkstr = "";
    for (var i = 0; i < chks.length; i++) {
        if (chks[i].checked == true) {
            chkstr += chks[i].value + ",";
        }
    }
    if (CollegeName == "") {
        layer.msg('院校名称是必填项！', function () { });
        return false;
    }
    else {
        if (CollegeName.length > 25) {
            layer.msg('院校名称长度超过限制！', function () { });
            return false;
        }
    }
    if (chkstr == "") {
        layer.msg('用户权限是必选项！', function () { });
        return false;
    }
    $.ajax({
        type: "POST",
        dataType: "json",
        async: false,
        url: '/Admin/CollegeManagement/CollegeEditSave',
        data: { Id: $("#EId").val(), CollegeName: CollegeName, Jurisdiction: chkstr },
        success: function (data) {
            if (data == "-1") {
                layer.msg('您好,不能添加重复的院校名称！', { icon: 2 });
                return;
            } else if (data > 0) {
                layer.msg('操作成功！', { icon: 1 });
            } else {
                layer.msg('操作失败！', { icon: 2 });
            }
            location.reload();
        }
    });
}

//删除
function Dele() {
    var chks = document.getElementsByName('input[]');//name
    var chkstr = "";
    for (var i = 0; i < chks.length; i++) {
        if (chks[i].checked == true) {
            chkstr += chks[i].value + ",";
        }
    }
    chkstr += "0";
    if (chkstr == "" || chkstr == "0") {
        layer.msg('请先选中一项', function () { });
        return false;
    }
    layer.confirm('您确定要删除所选院校吗？', {
        title: '删除',
        btn: ['确定', '取消'],
        shadeClose: true, //开启遮罩关闭
        skin: 'layui-layer-lan'
        //按钮
    }, function () {
        $.ajax({
            type: "POST",
            // dataType: "text",
            async: false,
            url: '/Admin/CollegeManagement/CollegeDele',
            data: { Id: chkstr },
            success: function (data) {
                if (data == "-1") {
                    layer.msg('不可执行，请先删除该院校的教师用户及学生用户！', { icon: 2 });
                    return;
                } else if (data > 0) {
                    layer.msg('操作成功！', { icon: 1 });
                } else {
                    layer.msg('操作失败！', { icon: 2 });
                }
                DataBind();
            }
        });
    });
}
