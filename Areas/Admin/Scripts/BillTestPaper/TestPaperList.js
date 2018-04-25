var UserId;

$(function () {
    UserId = $("#UserId").val();
    DataBind();
    box();
})

//加载列表
function DataBind(page) {
    var TestAttribute = $("#TestAttribute").val();
    var PaperSource = $("#PaperSource").val();
    var PaperName = $("#PaperName").val();
    var PageSize = 10;
    $.ajax({
        type: "POST",
        dataType: "json",
        async: false,
        url: '/Admin/BillTestPaper/GetTestPaperList',
        data: { TestAttribute: TestAttribute, PaperSource: PaperSource, PaperName: PaperName, page: page, PageSize: PageSize },
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
                var dttype = 0;
                if ($("#UserType").val() == "1") {
                    if (data[i]["Kind"] == "1") {
                        dttype = 1;
                    }
                } else {
                    if (data[i]["AddOperator"] == UserId) {
                        dttype = 1;
                    }
                }
                html += " <td> <input type='checkbox' data-type='" + dttype + "' value='" + data[i]["ID"] + "' class='i-checks' name='input[]'></td>";
                html += " <td><span class='pie'>" + data[i]["PaperName"] + "</span> </td>";
                html += "  <td><span class='pie'>" + data[i]["Score"] + "</span></td>";
                if (data[i]["Kind"] == "1") {
                    html += "  <td><span class='pie'>系统试卷</span></td>";
                } else {
                    html += "  <td><span class='pie'>教师试卷</span></td>";
                }
                if (data[i]["UserName"] != "") {
                    html += "  <td><span class='pie'>" + data[i]["UserName"] + "</span></td>";
                } else {
                    html += "  <td><span class='pie'>" + data[i]["UserNo"] + "</span></td>";
                }
                if (data[i]["Kind"] == "1") {
                    if ($("#UserType").val() == "2") {
                        html += "<td><a style='margin-left: 5px;' href='javascript:void(0);' onclick=\"window.location.href = '/Admin/BillTestPaper/LookTestPaper/" + data[i]["ID"] + "'\" class=' btn-info btn-sm'><i class='fa fa-lightbulb-o m-r-xxs '></i>预览 </a></td>";
                    } else {
                        html += "<td><a href='javascript:void(0);' onclick=\"window.location.href = '/Admin/BillTestPaper/EditTestPaper?ID=" + data[i]["ID"] + "'\" class=' btn-primary btn-sm'><i class='fa fa-pencil'></i>编辑 </a>";
                        html += "<a style='margin-left: 5px;' href='javascript:void(0);' onclick=\"window.location.href = '/Admin/BillTestPaper/LookTestPaper/" + data[i]["ID"] + "'\" class=' btn-info btn-sm'><i class='fa fa-lightbulb-o'></i>预览 </a>"
                        html += "<a style='margin-left: 5px;' href='javascript:void(0);' onclick=\"window.location.href = '/Admin/AuthorizationDetailsPush/Index?CompetitionId=" + data[i]["ID"] + "&CompetitionType=4';\" class='  btn-danger btn-sm'><i lass='fa fa-pencil m-r-xxs'></i>权限用户明细 </a></td>"
                    }

                } else {
                    if (data[i]["Kind"] == "2") {
                        if ($("#UserType").val() == "2") {
                            html += "<td><a href='javascript:void(0);' onclick=\"window.location.href = '/Admin/BillTestPaper/EditTestPaper?ID=" + data[i]["ID"] + "'\" class=' btn-primary btn-sm m-r-sm'><i class='fa fa-pencil m-r-xxs'></i>编辑 </a>"
                            html += "<a style='margin-left: 5px;' href='javascript:void(0);' onclick=\"window.location.href = '/Admin/BillTestPaper/LookTestPaper/" + data[i]["ID"] + "'\" class=' btn-info btn-sm'><i class='fa fa-lightbulb-o m-r-xxs '></i>预览 </a></td>";
                        } else {
                            html += "<td><a style='margin-left: 5px;' href='javascript:void(0);' onclick=\"window.location.href = '/Admin/BillTestPaper/LookTestPaper/" + data[i]["ID"] + "'\" class=' btn-info btn-sm'><i class='fa fa-lightbulb-o m-r-xxs '></i>预览 </a></td>";
                        }
                    } else {
                        html += "<td><a style='margin-left: 5px;' href='javascript:void(0);' onclick=\"window.location.href = '/Admin/BillTestPaper/LookTestPaper/" + data[i]["ID"] + "'\" class=' btn-info btn-sm'><i class='fa fa-lightbulb-o m-r-xxs '></i>预览 </a></td>";
                    }
                }
            }
            $("#table").html(html);
            bootstrapPaginator("#PaginatorLibrary", tb, DataBind);//分页
            box();
        }
    });
}

$("#query").click(function () {
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

//删除
function Del() {
    var strchks = "";
    var isboo = 0;
    var chks = document.getElementsByName('input[]');//name
    for (var i = 0; i < chks.length; i++) {
        if (chks[i].checked == true) {
            if ($(chks[i]).attr("data-type") == "1") {
                strchks += "" + chks[i].value + ",";
            } else {
                isboo = 1;
            }
        }
    }
    if (isboo == 1 && strchks.length == 0) {
        var txt = "教师试卷！";
        if ($("#UserType").val() == "2") {
            txt = "系统试卷！";
        }
        layer.msg('你无权删除' + txt, function () { });
        return;
    }
    txt = "";
    if (isboo == 1) {
        txt = "（不含教师试卷！）";
        if ($("#UserType").val() == "2") {
            txt = "（不含系统试卷！）";
        }
    }
    strchks += "0";
    if (strchks == "0") {
        layer.msg("请先选中需要删除的数据！", function () { });
        return;
    }
    layer.confirm("您确定要删除所选试卷吗" + txt + "？", {
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
              url: '/Admin/BillTestPaper/DeleTestPaper',
              data: { TopicId: strchks },
              success: function (data) {
                  if (data > 0) {
                      layer.msg('操作成功！', { icon: 1 });
                  } else {
                      layer.msg('操作失败！', { icon: 2 });
                  }
                  DataBind();
              }
          });
      });
}

//导出试卷脚本
function Export() {
    var strchks = "";
    var chks = document.getElementsByName('input[]');//name
    for (var i = 0; i < chks.length; i++) {
        if (chks[i].checked == true) {
            strchks += "" + chks[i].value + ",";
        }
    }
    strchks += "0";
    if (strchks == "0") {
        layer.msg('请先选择一行数据', function () { });
        return;
    }
    if (strchks.split(',').length >= 3) {
        layer.msg('只能选择一行数据', function () { });
        return;
    }
    $.ajax({
        type: "POST",
        dataType: "text",
        //async: false,
        url: '/Admin/BillTestPaper/BillExport',
        data: { TopicId: strchks },
        success: function (data) {
            $("#downFile").attr("href", "/ashx/download.ashx?downurl=" + '/Export/单据试卷脚本.sql');
            $("#tf").click()
        }
    });
}