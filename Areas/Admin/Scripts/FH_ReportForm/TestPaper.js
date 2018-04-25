
var UserType = "1";
var UserId = "1";
$(function () {
    UserType = $("#UserType").val();
    UserId = $("#UserId").val();

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
    var TestPaperName = $("#TestPaperName").val();//试卷名称 
    var Kind = $("#Kind").find("option:selected").val();//
    var AddOperator = $("#AddOperator").val();//

    var PageSize = 10; 
    $.ajax({
        Type: "post",
        url: '/Admin/FH_ReportForm/GetTestPaperList',
        dataType: "json", cache: false,
        contentType: "application/json; charset=utf-8",
        data: { "TestPaperName": TestPaperName, "Kind": Kind, "AddOperator": AddOperator, "page": page, "PageSize": PageSize },
        success: function (data) {
            var tb = data;
            if (tb != null && tb.Total > 0) {
                var html = "";
                var obj = tb.Tb;
                if (obj.length > 0) {
                     
                    for (var i = 0; i < obj.length; i++) {//rownum
                        var boo = 0;
                        var booe = 0;
                        var Kind = obj[i]["Kind"];
                        var AddOperator = obj[i]["AddOperator"];
                        if (UserType == "1") {
                            if (Kind == 1) {
                                boo = 1;
                            }
                            booe = 1;
                        } else {
                            if (AddOperator == UserId) {
                                boo = 1;
                                booe = 1;
                            }
                        }
                        html += '<tr class="boo" data-Type="' + boo + '">';
                        html += '<td  style="width:50px;">' + obj[i]["rownum"] + '</td>';
                        html += '<td>';
                        var Total = obj[i]["Total"]
                        if (IsNullOrEmpty(Total)) {
                            Total = "0";
                        }
                        
                        html += '<input type="checkbox" data-boo="' + boo + '" class="i-checks" value="' + obj[i]["Id"] + '" name="input[]">';
                        
                        html += '</td>';
                        html += '<td><span class="pie">' + obj[i]["TestPaperName"] + '</span>';
                        html += '</td>';
                        html += '<td><span class="pie">' + Total + '</span>';
                        html += '</td>';
                        if (UserType == "1") {
                            html += '<td><span class="pie">' + (obj[i]["Kind"] == 1 ? "系统题目" : "教师题目") + '</span>';
                            html += '</td>';
                            if (obj[i]["UserName"] == "null" || obj[i]["UserName"] == null || obj[i]["UserName"].length == 0) {
                                html += '<td><span class="pie">' + obj[i]["UserNo"] + '</span>';
                            } else {
                                html += '<td><span class="pie">' + obj[i]["UserName"] + '</span>';
                            }
                            html += '</td>';
                        }
                        html += '<td>';

                        if (boo == 1) {
                            html += "<a href=\"javascript:;\" onclick=\"EditStudent(1," + obj[i]["Id"] + ",'" + obj[i]["Title"] + "')\" class=\" btn-primary btn-sm\"><i class=\"fa fa-pencil m-r-xxs\"></i>编辑 </a>";
                        }

                        html += "<a style=\"margin-left: 5px;\"  onclick=\"GetStudent(" + obj[i]["Id"] + ",'" + obj[i]["TestPaperName"] + "')\" href=\"javascript:;\" class=\" btn-info btn-sm\"><i class=\"fa fa-lightbulb-o m-r-xxs\"></i>预览 </a>";
                        if (UserType == "1") {
                            if (boo == 1) {
                                html += "<a style=\"margin-left: 5px;\" href=\"javascript:;\" onclick=\"AuthorizationDetailsPush(" + obj[i]["Id"] + ")\" class=\" btn-danger btn-sm\"><i class=\"fa fa-pencil m-r-xxs\"></i>权限用户明细 </a>";
                            }
                        } 
                        html += '</td>';
                        html += '</tr>';
                    }
                }
                $("#tablelist").html(html);
            } else {
                $("#tablelist").html("<tr><td colspan='10'></td></tr>");
            }
            bootstrapPaginator("#PaginatorLibrary", tb, bindIngfo)//分页

            redload();

        }
    });


}


//批量删除弹窗
function del_all() {
    var chks = document.getElementsByName('input[]');//name
    var chkstr = "";
    var isboo = 0;
    for (var i = 0; i < chks.length; i++) {
        if (chks[i].checked == true) {
            if ($(chks[i]).attr("data-boo") == "1") {
                chkstr += chks[i].value + ",";
            } else {
                isboo = 1;
            }
        }
    }
    //$("#tablelist").find(".boo").each(function () {

    //})
    if (isboo == 1 && chkstr.length == 0) {

        var txt = "教师试卷！";
        if (UserType == "2") {
            txt = "系统试卷！";
        }
        layer.msg('你无权删除' + txt, function () { });
        return;
    }
    if (chkstr.length == 0) {
        layer.msg('请选择要删除的数据！', function () { });
        return;
    }
    chkstr = chkstr.substr(0, chkstr.length - 1);
    txt = "";
    if (isboo == 1) {
        txt = "（不含教师试卷）";
        if (UserType == "2") {
            txt = "（不含系统试卷）";
        }
    } 
    layer.confirm('您确定要删除所选试卷吗' + txt + '？', {
        title: '删除',
        btn: ['删除', '取消'],
        shadeClose: true, //开启遮罩关闭
        skin: 'layui-layer-lan'
        //按钮
    },
        function () {
            $.ajax({
                type: "POST",
                dataType: "text",
                url: '/Admin/FH_ReportForm/DelTestPaper',
                data: { "Ids": chkstr },//多个Id
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
            })
        });
}


function PaperSQL() {//ExportJob
    var chks = document.getElementsByName('input[]');//name
    var chkstr = "";
    var isboo = 0;
    for (var i = 0; i < chks.length; i++) {
        if (chks[i].checked == true) {
            if ($(chks[i]).attr("data-boo") == "1") {
                chkstr += chks[i].value + ",";
            } else {
                isboo = 1;
            }
        }
    } 
    if (isboo == 1 && chkstr.length == 0) {

        var txt = "教师试卷！";
        if (UserType == "2") {
            txt = "系统试卷！";
        }
        layer.msg('你无权导出' + txt, function () { });
        return;
    }
    if (chkstr.length == 0) {
        layer.msg('请选择要导出的数据！', function () { });
        return;
    }
    chkstr = chkstr.substr(0, chkstr.length - 1);
    txt = "";
    if (isboo == 1) {
        txt = "（不含教师试卷）";
        if (UserType == "2") {
            txt = "（不含系统试卷）";
        }
    }
    layer.confirm('您确定要导出所选试卷吗' + txt + '？', {
        title: '导出',
        btn: ['导出', '取消'],
        shadeClose: true, //开启遮罩关闭
        skin: 'layui-layer-lan'
        //按钮
    },
        function () {
            $.ajax({
                type: "POST",
                dataType: "text",
                url: '/Admin/FH_ReportForm/ExportJob',
                data: { "TestPaperId": chkstr },//多个Id
                success: function (data) {
                    layer.closeAll();//关闭所有弹出框
                    $("#downFile").attr("href", "/ashx/download.ashx?downurl=" + '/Export/复核试卷脚本.sql');
                    $("#tf").click()
                    layer.msg('导出成功', { icon: 1 });
                }
            })
        });
}
//用户权限明细
function AuthorizationDetailsPush(Id) {
    window.location.href = '/Admin/AuthorizationDetailsPush/Index?CompetitionId=' + Id + '&CompetitionType=3';
}
//手工、智能、编辑
function EditStudent(type, Id,Title) {
    if (Id == null) {
        window.location.href = "/Admin/FH_ReportForm/EditTestPaper?type=" + type;
    } else {
        window.location.href = "/Admin/FH_ReportForm/EditTestPaper?type=" + type + "&Id=" + Id;
    }
}
//预览
function GetStudent(Id, Title) {
    if (Id != null) {
        window.location.href = "/Admin/FH_ReportForm/PreviewTestPaper?Id=" + Id + "&Title=" + Title;
    }
}
//权限明细
function AuthorizationDetailsPush(Id) {
    if (Id != null) {
        window.location.href = '/Admin/AuthorizationDetailsPush/Index?CompetitionId=' + Id + '&CompetitionType=3';
    }
}

