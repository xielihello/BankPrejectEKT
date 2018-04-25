/***************************************************************
  FileName:货币知识试卷微调预览 加点题 javascript
  Copyright（c）2017-金融教育在线技术开发部
  Author:袁学
  Create Date:2017-4-7
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
var thispage = 0;
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

//搜索
function searchinfo() {
    MPId = getQueryString('PId');
    bindIngfo();
}
var MPId = "0";
//列表数据加载
function bindIngfo(page) {
    thispage = page;
    var PaperQB_Type = $("#PaperQB_Type").val();//类型
    var PaperQB_Description = $("#PaperQB_Description").val();//描述
    var PaperStateK = $("#PaperStateK").val();// 加卷状态
    var PaperIsUse = $("#PaperIsUse").val();// 是否被使用
    var PageSize = 10;
    MPId = getQueryString('PId');
    $.ajax({
        url: '/Admin/HB_Paper/GetQuestionBankList',
        Type: "post",
        dataType: "json", cache: false,
        contentType: "application/json; charset=utf-8",
        data: { "MPId": MPId, "PaperQB_Type": PaperQB_Type, "PaperQB_Description": PaperQB_Description, "PaperStateK": PaperStateK, "PaperIsUse": PaperIsUse, "page": page, "PageSize": PageSize },
        success: function (tb) {

            var html = '';
            var data = tb.Tb;//转换table
            for (var i = 0; i < data.length; i++) {
                html += '<tr>';
                //当前页面
                var idx = 0;
                if (page != "undefined" && page != null) {
                    idx = page;
                    idx = idx - 1;
                }
                html += '<td><span class="pie">' + ((idx * PageSize) + i + 1) + '</span></td>';


                html += '<td><input type="checkbox" class="i-checks" name="input[]" value="' + data[i]["QuestionBId"] + '"></td>';
                var txtQB_Description = HTMLDecode(data[i]["QB_Description"]) + "";//先转义
                txtQB_Description = delHtmlTag(txtQB_Description);//去除html标签

                if (txtQB_Description.length > 40) {
                    txtQB_Description = txtQB_Description.substr(0, 38) + '...';
                }



                html += '<td title="' + delHtmlTag(HTMLDecode(data[i]["QB_Description"])) + '">' + txtQB_Description + '</td>';
                html += '<td>' + GetTiType(data[i]["QB_Type"]) + '</td>';

                if (parseInt(data[i]["CountPaper"]) > 0) {
                    html += '<td><span class="text-danger">已加入当前试卷</span><input type="hidden"  id="Isjiaru' + data[i]["QuestionBId"] + '" value="1"/></td>';
                } else {
                    html += '<td>未加入当前试卷<input type="hidden"  id="Isjiaru' + data[i]["QuestionBId"] + '" value="0"/></td>';
                }


                if (parseInt(data[i]["CountNum"]) > 0) {
                    html += '<td><span class="text-danger">已使用</span></td>';
                } else {
                    html += '<td>未使用</td>';
                }

                html += '<td>';
                html += '<a href="javascript:void(0);" onclick="See(' + data[i]["QuestionBId"] + ')" class=" btn-info btn-sm"><i class="fa fa-lightbulb-o m-r-xxs"></i>查看 </a></td>';
                html += '</tr>';


            }
            $("#tablelist").html(html);
            bootstrapPaginator("#PaginatorLibrary", tb, bindIngfo);//分页
            //样式重新加载
            redload();
            Statistics();
        }
    });
}

//题型列读取
function GetTiType(type) {
    if (type == 1) {
        return "单选题";
    }
    if (type == 2) {
        return "多选题";
    }
    if (type == 3) {
        return "判断题";
    }
    if (type == 4) {
        return "填空题";
    }
    if (type == 5) {
        return "简答题";
    }
    if (type == 6) {
        return "名词解释题";
    }
    if (type == 7) {
        return "案例分析题";
    }
    if (type == 8) {
        return "论述题";
    }
    return "";
}

//查看
function See(QuestionBId) {
    jQuery('#searchDIV').modal('show', { backdrop: 'static' });
    $.ajax({
        url: '/Admin/HB_QuestionBank/GetListById?QuestionBId=' + QuestionBId,
        type: 'POST',
        dataType: 'json',
        async: false,
        success: function (d) {

            if (d.length > 0) {
                var txtQB_Kind = '系统题目';
                if (parseInt(d[0]["QB_Kind"]) == 2) {
                    txtQB_Kind = '教师题目';
                }
                $("#Span4").html(txtQB_Kind);
                $("#Span5").html(d[0]["QB_Custom2"]);
                var QB_Typehtml = "";
                $("#yxdivA").css('display', 'none');
                $("#yxdivB").css('display', 'none');
                $("#yxdivC").css('display', 'none');
                $("#yxdivD").css('display', 'none');
                $("#yxdivE").css('display', 'none');
                $("#yxdivG").css('display', 'none');
                if (d[0]["QB_Type"] == 1) {
                    QB_Typehtml = "单选题";
                    if (d[0]["QB_A"] != "" && d[0]["QB_A"].length > 0) {
                        $("#yxdivA").css('display', 'block');
                    }
                    if (d[0]["QB_B"] != "" && d[0]["QB_B"].length > 0) {
                        $("#yxdivB").css('display', 'block');
                    }
                    if (d[0]["QB_C"] != "" && d[0]["QB_C"].length > 0) {
                        $("#yxdivC").css('display', 'block');
                    }
                    if (d[0]["QB_D"] != "" && d[0]["QB_D"].length > 0) {
                        $("#yxdivD").css('display', 'block');
                    }


                }
                if (d[0]["QB_Type"] == 2) {
                    QB_Typehtml = "多选题";
                    if (d[0]["QB_A"] != "" && d[0]["QB_A"].length > 0) {
                        $("#yxdivA").css('display', 'block');
                    }
                    if (d[0]["QB_B"] != "" && d[0]["QB_B"].length > 0) {
                        $("#yxdivB").css('display', 'block');
                    }
                    if (d[0]["QB_C"] != "" && d[0]["QB_C"].length > 0) {
                        $("#yxdivC").css('display', 'block');
                    }
                    if (d[0]["QB_D"] != "" && d[0]["QB_D"].length > 0) {
                        $("#yxdivD").css('display', 'block');
                    }
                    if (d[0]["QB_E"] != "" && d[0]["QB_E"].length > 0) {
                        $("#yxdivE").css('display', 'block');
                    }
                }
                if (d[0]["QB_Type"] == 3) {
                    QB_Typehtml = "判断题";
                    $("#yxdivA").css('display', 'block');
                    $("#yxdivB").css('display', 'block');
                }
                if (d[0]["QB_Type"] == 4) { QB_Typehtml = "填空题"; }
                if (d[0]["QB_Type"] == 5) { QB_Typehtml = "简答题"; $("#yxdivG").css('display', 'block'); }
                if (d[0]["QB_Type"] == 6) { QB_Typehtml = "名词解释"; $("#yxdivG").css('display', 'block'); }
                if (d[0]["QB_Type"] == 7) { QB_Typehtml = "案例分析题"; $("#yxdivG").css('display', 'block'); }
                if (d[0]["QB_Type"] == 8) { QB_Typehtml = "论述题"; $("#yxdivG").css('display', 'block'); }


                $("#Span6").html(QB_Typehtml);
                $("#Span7").html(HTMLDecode(d[0]["QB_Description"]));
                $("#Span7").find("img").css('width', '420px');
                $("#Span8").html(d[0]["QB_A"]);
                $("#Span9").html(d[0]["QB_B"]);
                $("#Span10").html(d[0]["QB_C"]);
                $("#Span11").html(d[0]["QB_D"]);
                $("#Span12").html(d[0]["QB_E"]);
                $("#Span13").html(d[0]["QB_Answer"]);
                $("#Span14").html(d[0]["QB_Keyword"]);

            }
        }
    });

}

//加入试卷
function Add() {

    //获取复选框所有选择的值
    var chk_value = [];
    $('input[name="input[]"]:checked').each(function () {
        chk_value.push($(this).val());
    });

    var chkstr = chk_value + "";
    if (chkstr.length == 0) {
        layer.msg('请勾选组卷的试题！', function () { });
        return;
    }

    $.ajax({
        type: "POST",
        dataType: "json",
        url: '/Admin/HB_Paper/AddtoManualTestPaper',
        data: { "Ids": chkstr, "NPid": MPId },//多个Id
        success: function (data) {
            if (data == "1") {
                layer.closeAll();//关闭所有弹出框
                bindIngfo(thispage);
                layer.msg('操作成功', { icon: 1 });

            }
            if (data == "99") {
                layer.msg('操作失败', { icon: 2 });
                return;
            }

        }
    })


}

//移除试卷
function ClearAdd() {

    //不管选择的是否已加入 都执行删除
    //获取复选框所有选择的值
    var chk_value = [];
    $('input[name="input[]"]:checked').each(function () {
        chk_value.push($(this).val());
    });

    var chkstr = chk_value + "";
    if (chkstr.length == 0) {
        layer.msg('请勾选要移除的试题！', function () { });
        return;
    }
    if (chkstr.split(',').length == 1) {//单选
        //未加入的试卷提示
        var Isjiaru = $("#Isjiaru" + chkstr).val();
        if (Isjiaru == "0") {
            layer.msg('未加入当前试卷不能移除！', function () { });
            return;
        }
    }
    $.ajax({
        type: "POST",
        dataType: "json",
        url: '/Admin/HB_Paper/DeltoManualTestPaper',
        data: { "Ids": chkstr, "NPid": MPId },//多个Id
        success: function (data) {
            if (data == "1") {
                layer.closeAll();//关闭所有弹出框
                bindIngfo(thispage);
                layer.msg('操作成功', { icon: 1 });

            }
            if (data == "99") {
                layer.msg('操作失败', { icon: 2 });
                return;
            }

        }
    })

}

//计算试卷各类题型
function Statistics() {
    $.ajax({
        type: "POST",
        dataType: "json",
        url: '/Admin/HB_Paper/GetManualTestPaperByType?PId=' + MPId,
        async: false,
        success: function (data) {
            var lbltxx = "";
            if (data.length > 0) {
                for (var i = 0; i < data.length; i++) {
                    lbltxx += GetTiType(data[i]["QB_Type"]) + "有" + data[i]["num"] + "道；";
                }

                $("#txtlbl").html(lbltxx);
            }
            else {
                $("#txtlbl").html('试卷详情：无');
            }

        }
    });
}

//返回
function PaperPreview() {
    window.location.href = '/Admin/HB_Paperpreview/Index?PId=' + getQueryString('PId') + '&Type=' + getQueryString('Type');
}