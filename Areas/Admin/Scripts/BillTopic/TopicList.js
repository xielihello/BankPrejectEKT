$(function () {
    box();
    DataBind();
});

//加载列表
function DataBind(page) {
    var BillType = $("#BillType").val();
    var BillName = $("#BillName").val();
    var BillTitle = $("#BillTitle").val();
    var TeamId = $("#TeamId").val();
    var Creater = $("#Creater").val();
    var PageSize = 10;
    $.ajax({
        type: "POST",
        dataType: "json",
        async: false,
        url: '/Admin/BillTopic/GetTopicList',
        data: { BillType: BillType, BillName: BillName, BillTitle: BillTitle, TeamId: TeamId, Creater: Creater, page: page, PageSize: PageSize },
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
                html += "<td><input type='checkbox' class='i-checks' value='" + data[i]["ID"] + "' name='input[]'></td>";
                html += "<td><span class='pie'>" + data[i]["TopicTitle"] + "</span></td>";
                html += "<td><span class='pie'>" + data[i]["BillType1"] + "</span></td>"
                html += "<td><span class='pie'>" + data[i]["BillName"] + "</span></td>"
                if (data[i]["Kind"] == "1") {
                    html += "<td><span class='pie'>系统题目</span></td>";
                } else {
                    html += "<td><span class='pie'>教师题目</span></td>";
                }
                if (data[i]["UserName"] == "") {
                    html += "<td><span class='pie'>" + data[i]["UserNo"] + "</span> </td>"
                } else {
                    html += "</td><td><span class='pie'>" + data[i]["UserName"] + "</span> </td>"
                }
                html += "<td><a href='javascript:void(0);' onclick=\"Edit(" + data[i]["ID"] + ",'" + data[i]["BillFormId"] + "')\"  class=' btn-primary btn-sm'><i class='fa fa-pencil m-r-xxs'></i>修改 </a>";
                html += "<a style='margin-left: 5px;' href='javascript:void(0);' onclick=\"Look(" + data[i]["ID"] + ",'" + data[i]["BillFormId"] + "')\" class=' btn-info btn-sm'><i class='fa fa-lightbulb-o m-r-xxs'></i>查看 </a>&nbsp;&nbsp;";

                html += "</td></tr>";
            }
            $("#table").html(html);
            bootstrapPaginator("#PaginatorLibrary", tb, DataBind);//分页
            box();
        }
    });
}

//查询
$("#query").click(function () {
    DataBind()
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

//单据类型
$("#BillType").change(function () {
    var BillType = $(this).val();
    $.ajax({
        type: "POST",
        // dataType: "text",
        async: false,
        url: '/Admin/BillTopic/BillNameSelect',
        data: { BillType: BillType },
        success: function (data) {
            $("#BillName").html(data);
        }
    });
});

//查看
function Look(ID, FormId) {
    window.location.href = "/Admin/BillTopic/LookTopic?ID=" + ID + "&FormId=" + FormId + "";
}

//编辑
function Edit(ID, FormId) {
    window.location.href = "/Admin/BillTopic/EditTopic?ID=" + ID + "&FormId=" + FormId + "";
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
    layer.confirm('您确定要删除所选题目吗？', {
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
                async: false,
                url: '/Admin/BillTopic/TopicDele',
                data: { TopicId: chkstr },
                success: function (data) {
                    if (data > 0) {
                        layer.msg('操作成功！', { icon: 1 });
                    } else {
                        layer.msg('操作失败！', { icon: 2 });
                    }
                    setTimeout("location.reload();", 3000);//延时3秒 
                }
            });
        });
}
