/***************************************************************
  FileName:货币知识试卷管理  用户权限 javascript
  Copyright（c）2017-金融教育在线技术开发部
  Author:袁学
  Create Date:2017-4-1
 ******************************************************************/

// JS 获取URL参数方法
function getQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) return unescape(r[2]); return null;
}


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


    var $checkboxAllTo = $(".checkbox-allTo"),
                     $checkboxTo = $(".new_tableTo").find("[type='checkbox']").not("[disabled]"),
                     lengthTo = $checkboxTo.length,
                     iTo = 0;
    $checkboxAllTo.on("ifClicked", function (event) {
        if (event.target.checked) {
            $checkboxTo.iCheck('uncheck');
            iTo = 0;
        } else {
            $checkboxTo.iCheck('check');
            iTo = lengthTo;
        }
    });
}

//列表数据加载
function bindIngfo(page) {
    var SchoolName = $("#SchoolName").val();//裁判信息
    var PageSize = 10;
    var CompetitionType = getQueryString('CompetitionType');//赛项类型
    var CompetitionId = getQueryString('CompetitionId');//试卷Id
    $.ajax({
        Type: "post",
        dataType: "json",
        cache: false,
        contentType: "application/json; charset=utf-8",
        url: '/Admin/AuthorizationDetailsPush/GetList',
        data: { "SchoolName": SchoolName, "page": page, "PageSize": PageSize, "CompetitionType": CompetitionType, "CompetitionId": CompetitionId },
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
                    html += '<td><span class="pie">' + data[i]["Name"] + '</span></td>';
                    html += '<td><a href="javascript:void(0);" onclick="Edit(' + data[i]["URId"] + ')" class=" btn-primary btn-sm"><i class="fa fa-pencil m-r-xxs"></i>取消 </a></td> ';
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

//取消
function Edit(Id) {

    layer.confirm('您确定要取消对该分行的权限分配吗？', {
        title: '提示',
        btn: ['确定', '取消'],
        shadeClose: true, //开启遮罩关闭
        skin: 'layui-layer-lan'
        //按钮
    },
        function () {
            $.ajax({
                type: "POST",
                dataType: "text",
                url: '/Admin/AuthorizationDetailsPush/Del',
                data: { "Id": Id },
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

//新增
function AddInfo() {
    FormRest();
    layer.open({
        type: 1,
        title: '新增',
        skin: 'layui-layer-lan', //样式类名
        anim: 2,
        shadeClose: false, //开启遮罩关闭
        area: ['600px', '470px'], //宽高
        content: $("#Add")
    });
    ListInfo();
}

function searchinfoTo() {
    ListInfo();
}

//弹框数据读取
function ListInfo(page) {
    var SchoolNameTo = $("#SchoolNameTo").val();//裁判信息
    var PageSize = 5;
    var CompetitionType = getQueryString('CompetitionType');//赛项类型
    var CompetitionId = getQueryString('CompetitionId');//试卷Id
    $.ajax({
        Type: "post",
        dataType: "json",
        cache: false,
        contentType: "application/json; charset=utf-8",
        url: '/Admin/AuthorizationDetailsPush/GetList_To',
        data: { "SchoolNameTo": SchoolNameTo, "page": page, "PageSize": PageSize, "CompetitionType": CompetitionType, "CompetitionId": CompetitionId },
        success: function (tb) {
            var html = '';
            var data = tb.Tb;//转换table
            if (tb != null && tb.Total != 0) {//table数据不为空
                for (var i = 0; i < data.length; i++) {
                    html += '<tr><td>';
                    html += ' <input type="checkbox" class="i-checks" name="inputTo[]" value="' + data[i]["Id"] + '"></td>';
                    html += '<td><span class="pie">' + data[i]["Name"] + '</span></td>';

                    html += '</tr>';
                }
            }
            $("#tablelistTo").html(html);
            //分页控件加载
            bootstrapPaginator("#PaginatorLibrary2", tb, ListInfo);//分页

            //样式重新加载
            redload();
        }
    });

}

function FormRest() {
    layer.closeAll();//关闭
    $('#Addform')[0].reset();//清空表单数据
}

//添加
function BtnSubim() {
    var chks = document.getElementsByName('inputTo[]');//name
    var chkstr = "";
    for (var i = 0; i < chks.length; i++) {
        if (chks[i].checked == true) {
            chkstr += chks[i].value + ",";
        }
    }

    if (chkstr.length == 0) {
        layer.msg('请选择分行！', function () { });
        return;
    }

    chkstr = chkstr.substr(0, chkstr.length - 1);
    var CompetitionType = getQueryString('CompetitionType');//赛项类型
    var CompetitionId = getQueryString('CompetitionId');//试卷Id
    $.ajax({
        type: "POST",
        dataType: "text",
        url: '/Admin/AuthorizationDetailsPush/Add',
        data: { "Ids": chkstr, "CompetitionType": CompetitionType, "CompetitionId": CompetitionId },//多个Id
        success: function (data) {
            if (data == "1") {
                layer.closeAll();
                layer.msg('操作成功', { icon: 1 });
                bindIngfo();
            }
            if (data == "99") {
                layer.msg('操作失败', { icon: 2 });
                return;
            }

        }
    })
}


function FanHui() {
    var Type = getQueryString('CompetitionType');//赛项类型
    if (Type == "1") {//返回货币知识 试卷管理
        window.location.href = '/Admin/HB_Paper/Index';
    }
    if (Type == "2") {//返回 手工
        window.location.href = '/Admin/MbcTaskManagement/MbcTaskManagement';
    }
    if (Type == "3") {//复核
        window.location.href = '/Admin/FH_ReportForm/TestPaper';
    }
    if (Type == "4") {//单据
        window.location.href = '/Admin/BillTestPaper/TestPaperList';
    }
}


