/***************************************************************
  FileName:货币知识手工组卷 javascript
  Copyright（c）2017-金融教育在线技术开发部
  Author:袁学
  Create Date:2017-4-5
 ******************************************************************/

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

//分值验证 当前形式下
function DQcheckeNum(Id) {
    var dqnum = $("#STScores" + Id).val();
    if (isNaN(dqnum)) {
        $("#STScores" + Id).val('');

    }
    var patrn = /^([1-9]\d{0,9}|0)([.]?|(\.\d{1,2})?)$/;
    if (!patrn.test(dqnum)) {
        $("#STScores" + Id).val('');
    }

    ///分值校验
    if (parseFloat($("#STScores" + Id).val()) > 100) {
        $("#STScores" + Id).val('100');
    }
    dqnum = $("#STScores" + Id).val();
    if (dqnum.length > 0) {
        //开始了
        //获取当前分值框的值
        var STScores = $("#STScores" + Id).val();
        //获取当前单选题多少道
        var STNUM = $("#STNUM" + Id).html();
        if (parseInt(STNUM) != 0) {
            //
            var Calculation = parseFloat(STScores) * parseFloat(STNUM);
            //共计 赋值
            $("#ZGSTS" + Id).html(Calculation);
        }

    } else {
        //当单前分值框是空的时候 ，当前共计赋值为0
        $("#ZGSTS" + Id).html('0');
    }

    var ZGScores = 0.00;
    //计算总分

    for (var i = 1; i < 9; i++) {
        var STScores = $("#STScores" + i).val();
        var STNUM = $("#STNUM" + i).html();

        if (parseInt(STNUM) != 0 && STScores.length > 0) {

            var Calculation = parseFloat(STScores) * parseFloat(STNUM);
            ZGScores += Calculation;

        }
    }
    //试卷总分赋值
    $("#PScore").html(ZGScores);

}

//搜索
function searchinfo() {
    bindIngfo();
}

var MPId = "";
//列表数据加载
function bindIngfo(page) {
    thispage = page;
    var PaperQB_Type = $("#PaperQB_Type").val();//类型
    var PaperQB_Description = $("#PaperQB_Description").val();//描述
    var PaperStateK = $("#PaperStateK").val();// 加卷状态
    var PaperIsUse = $("#PaperIsUse").val();// 是否被使用
    var PageSize = 10;

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
            display_kt_display();
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
                    $("#yxdivA").css('display', 'block');
                    $("#yxdivB").css('display', 'block');
                    $("#yxdivC").css('display', 'block');
                    $("#yxdivD").css('display', 'block');

                }
                if (d[0]["QB_Type"] == 2) {
                    QB_Typehtml = "多选题";
                    $("#yxdivA").css('display', 'block');
                    $("#yxdivB").css('display', 'block');
                    $("#yxdivC").css('display', 'block');
                    $("#yxdivD").css('display', 'block');
                    $("#yxdivE").css('display', 'block');
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
                if (d[0]["QB_E"] == null || d[0]["QB_E"].length == 0) {
                    $("#yxdivE").css('display', 'none');
                }
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
    //验证输入试卷名称
    var AddP_Name = $("#AddP_Name").val();
    var IsOrder = $('input[name="IsOrder"]:checked').val();
    if (AddP_Name.length == 0) {
        layer.msg('请输入试卷名称！', function () { });
        return;
    }
    if (AddP_Name.length > 15) {
        layer.msg('试卷名称不能超过15个汉字！', function () { });
        return false;
    }
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

    //验证全局试卷Id是否存在
    if (MPId == "") {
        //提交数据 先建试卷后 新增试卷试题表数据 返回 试卷Id
        $.ajax({
            type: "POST",
            dataType: "json",
            url: '/Admin/HB_Paper/AddManualTestPaper',
            data: { "Ids": chkstr, "IsOrder": IsOrder, "AddP_Name": AddP_Name },//多个Id
            success: function (data) {
                if (data[0]["error"] == "1") {
                    layer.closeAll();//关闭所有弹出框

                    MPId = data[0]["pid"];//赋值全局Pid
                    bindIngfo(thispage);
                    layer.msg('操作成功', { icon: 1 });
                    //去除全选
                    var $checkboxAll = $(".checkbox-all")
                    $checkboxAll.iCheck('uncheck');

                }
                if (data[0]["error"] == "99") {
                    layer.msg('操作失败', { icon: 2 });
                    return;
                }
                if (data[0]["error"] == "88") {
                    layer.msg('试卷名称已经存在,请修改试卷名称', { icon: 2 });
                    return;
                }
            }
        })

    } else {
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
}

//移除试卷
function ClearAdd() {
    if (MPId == "") {
        layer.msg('请勾选试题加入试卷！', function () { });
        return;
    } else {
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
}

var MType = "";//全局剩余选择的类型
//统计题量和每种题型多少道
function Statistics() {
    $.ajax({
        type: "POST",
        dataType: "json",
        url: '/Admin/HB_Paper/GetManualTestPaperByType?PId=' + MPId,
        success: function (data) {

            if (data.length > 0) {
                var Numt = 0;
                MType = "";
                for (var i = 0; i < data.length; i++) {
                    $("#STNUM" + data[i]["QB_Type"]).html(data[i]["num"]);
                    $("#display_kt_" + data[i]["QB_Type"]).css('display', 'block');
                    Numt += parseInt(data[i]["num"]);
                    DQcheckeNum(data[i]["QB_Type"]);//键盘按下去事件
                    MType += data[i]["QB_Type"] + ",";
                }

                $("#PTNum").html(Numt);//总题量
            } else {
                MType = "";
                $("#PScore").html('0');
                $("#PTNum").html('0');
            }

        }
    });
}

//隐藏各题类型数量
function display_kt_display() {
    for (var i = 1; i < 9; i++) {
        $("#display_kt_" + i).css('display', 'none');

        $("#STNUM" + i).html('0');
    }
}

//返回
function EditFormRest() {
    window.location.href = '/Admin/HB_Paper/Index';
}

//保存
function EditBtnSubim() {
    //修改试卷状态
    //修改每项分值
    if (MType == "" || MPId == "") {
        layer.msg('请勾选试题加入试卷！', function () { });
        return;
    }

    var str = "";
    var listtype = MType.split(',');
    //拼接现勾选设置的分值
    for (var i = 0; i < listtype.length; i++) {
        if (listtype[i].length > 0) {
            var STScores = $("#STScores" + listtype[i]).val();
            if (STScores.length == 0) {
                layer.msg('请设置试题分值！', function () { });
                return;
            }
            //试题类型-对应分值
            str += listtype[i] + "-" + STScores + ",";
        }

    }


    //验证输入试卷名称
    var AddP_Name = $("#AddP_Name").val();
    var IsOrder = $('input[name="IsOrder"]:checked').val();
    if (AddP_Name.length == 0) {
        layer.msg('试卷名称不能为空！', function () { });
        return;
    }
    if (AddP_Name.length > 15) {
        layer.msg('试卷名称不能超过15个汉字！', function () { });
        return false;
    }
    $.ajax({
        type: "POST",
        dataType: "json",
        url: '/Admin/HB_Paper/EditManualTestPaper',
        data: { "Ids": str, "IsOrder": IsOrder, "AddP_Name": AddP_Name, "NPid": MPId },//多个Id
        success: function (data) {
            if (data == "1") {
                layer.closeAll();//关闭所有弹出框
                layer.msg('操作成功', { icon: 1, time: 500 }, function () {
                    window.location.href = '/Admin/HB_Paper/Index';
                });

            }
            if (data == "99") {
                layer.msg('操作失败', { icon: 2 });
                return;
            }

            if (data == "88") {
                layer.msg('试卷名称已经存在,请修改试卷名称', { icon: 2 });
                return;
            }
        }
    })
}

