/***************************************************************
  FileName:货币知识智能组卷 javascript
  Copyright（c）2017-金融教育在线技术开发部
  Author:袁学
  Create Date:2017-4-6
 ******************************************************************/

$(function () {
    redload();
    TheAmountOfType();
});
//获取数据库现有题量
function TheAmountOfType() {
    $.ajax({
        type: "POST",
        dataType: "json",
        url: '/Admin/HB_Paper/GetIntelligentTestPaperByType',
        success: function (data) {
            if (data.length > 0) {
                for (var i = 0; i < data.length; i++) {
                    //赋值各题型 现有题量
                    $("#TNum" + data[i]["QB_Type"]).html(data[i]["num"]);
                    //尚未被使用的
                    $("#TNoUseNum" + data[i]["QB_Type"]).html(data[i]["NoUsenum"]);

                }


            }
        }
    });
}


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

//每道题分值
function DQcheckeScores(Id) {
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

    Calculation(Id);

    //总计重复题目
    if (parseInt($("#ZGExtractT").html()) >= parseInt($("#ZGTNoUseNum").html())) {
        $("#ZGTRepeatNum").html(parseInt($("#ZGExtractT").html()) - parseInt($("#ZGTNoUseNum").html()));
    }

}

//自动计算 各个题型总计分值
function Calculation(Id) {
    //获取当前分值框的值
    var STScores = $("#STScores" + Id).val();
    //获取随机抽取多少题
    var STNUM = $("#ExtractTNum" + Id).val();

    if (STScores.length > 0 && STNUM.length > 0) {
        //开始了
        var calculation = parseFloat(STScores) * parseFloat(STNUM);
        //共计 赋值
        $("#ZGSTS" + Id).html(calculation);
    } else {
        //当单前分值框是空的时候 ，当前共计赋值为0
        $("#ZGSTS" + Id).html('0');
    }

    //计算总分
    var ZGScores = 0.00;
    var ZGExtractT = 0;

    for (var i = 1; i < 9; i++) {
        var tscores = $("#STScores" + i).val();
        var tnum = $("#ExtractTNum" + i).val();

        if (tscores.length > 0 && tnum.length > 0) {
            //总分值
            var zfenzhi = parseFloat(tscores) * parseFloat(tnum);
            ZGScores += zfenzhi;
            ZGExtractT += parseInt(tnum);
        }
    }
    //试卷总分赋值
    $("#ZGScores").html(ZGScores);
    //总计抽取多少道题
    $("#ZGExtractT").html(ZGExtractT);
}

//随机抽取多少道
function DQcheckeNum(Id) {
    var dqnum = $("#ExtractTNum" + Id).val();
    if (isNaN(dqnum)) {
        $("#ExtractTNum" + Id).val('');

    }

    var patrn = /^[1-9]*[1-9][0-9]*$/;
    if (!patrn.test(dqnum)) {
        $("#ExtractTNum" + Id).val('');
    }
    //当前题型输入 不能大于单前题型总量
    dqnum = $("#ExtractTNum" + Id).val();
    if (dqnum.length > 0) {
        //合法 再断定
        //取当前题型总量
        var TNum = $("#TNum" + Id).html();
        if (parseInt(dqnum) > parseInt(TNum)) {
            layer.msg('输入值不能大于题型总题量！', { time: 1500 });
            $("#ExtractTNum" + Id).val('');
            return;
        }
    }
    //尚未被使用题型输入 不能大于单前题型尚未被使用总量 且 不能大于输入题量
    var dqnumTo = $("#NoUseExtractTNum" + Id).val();
    if (dqnumTo.length > 0) {
        //合法 再断定
        //取当前题型尚未被使用总量
        var TNoUseNum = $("#TNoUseNum" + Id).html();
        //取当前随机抽取题量
        var ExtractTNum = $("#ExtractTNum" + Id).val();

        if (parseInt(dqnumTo) > parseInt(TNoUseNum)) {
            layer.msg('未被使用输入值不能大于题型尚未被使用数量！', { time: 1500 });
            $("#NoUseExtractTNum" + Id).val('');
           
        }
        if (parseInt(dqnumTo) > parseInt(ExtractTNum)) {
            layer.msg('未被使用输入值不能大于随机抽取题目数量！', { time: 1500 });
            $("#NoUseExtractTNum" + Id).val('');
           
        }
        var ZGTNoUseNum = 0;
        for (var i = 1; i < 9; i++) {
            var NoUset = $("#NoUseExtractTNum" + i).val();
            if (NoUset.length > 0) {
                ZGTNoUseNum += parseInt(NoUset);
            }
        }
        //总计尚未被使用
        $("#ZGTNoUseNum").html(ZGTNoUseNum);


        //if (parseInt(dqnumTo) > parseInt(TNoUseNum) || parseInt(dqnumTo) > parseInt(ExtractTNum)) {
        //    layer.msg('未被使用输入值不能大于题型尚未被使用数量且不能大于随机抽取题目数量！', { time: 1500 });
        //    $("#NoUseExtractTNum" + Id).val('');

        //}
    }

    Calculation(Id);
    //总计重复题目
    if (parseInt($("#ZGExtractT").html()) >= parseInt($("#ZGTNoUseNum").html())) {
        $("#ZGTRepeatNum").html(parseInt($("#ZGExtractT").html()) - parseInt($("#ZGTNoUseNum").html()));
    }

}

//尚未被使用的题
function DQNoUsechekceNum(Id) {
    var dqnum = $("#NoUseExtractTNum" + Id).val();
    if (isNaN(dqnum)) {
        $("#NoUseExtractTNum" + Id).val('');

    }

    var patrn = /^[1-9]*[1-9][0-9]*$/;
    if (!patrn.test(dqnum)) {

        $("#NoUseExtractTNum" + Id).val('');
    }

    //尚未被使用题型输入 不能大于单前题型尚未被使用总量 且 不能大于输入题量
    dqnum = $("#NoUseExtractTNum" + Id).val();
    if (dqnum.length > 0) {
        //合法 再断定
        //取当前题型尚未被使用总量
        var TNoUseNum = $("#TNoUseNum" + Id).html();
        //取当前随机抽取题量
        var ExtractTNum = $("#ExtractTNum" + Id).val();
        if (parseInt(dqnum) > parseInt(TNoUseNum)) {
            layer.msg('输入值不能大于题型尚未被使用数量！', { time: 1500 });
            $("#NoUseExtractTNum" + Id).val('');
            return;
        }
        if (parseInt(dqnum) > parseInt(ExtractTNum)) {
            layer.msg('输入值不能大于随机抽取题目数量！', { time: 1500 });
            $("#NoUseExtractTNum" + Id).val('');
            return;
        }
        //if (parseInt(dqnum) > parseInt(TNoUseNum) || parseInt(dqnum) > parseInt(ExtractTNum)) {
        //    layer.msg('输入值不能大于题型尚未被使用数量且不能大于随机抽取题目数量！', { time: 1500 });
        //    $("#NoUseExtractTNum" + Id).val('');
        //    return;
        //}





    }
    var ZGTNoUseNum = 0;
    for (var i = 1; i < 9; i++) {
        var NoUset = $("#NoUseExtractTNum" + i).val();
        if (NoUset.length > 0) {
            ZGTNoUseNum += parseInt(NoUset);
        }
    }
    //总计尚未被使用
    $("#ZGTNoUseNum").html(ZGTNoUseNum);

    //总计重复题目
    if (parseInt($("#ZGExtractT").html()) >= parseInt($("#ZGTNoUseNum").html())) {
        $("#ZGTRepeatNum").html(parseInt($("#ZGExtractT").html()) - parseInt($("#ZGTNoUseNum").html()));
    }
}


//返回
function EditFormRest() {
    window.location.href = '/Admin/HB_Paper/Index';
}

//保存
function EditBtnSubim() {

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
    var falg = false;
    var strtxt = "";//拼接有输入的题型
    //校验
    for (var i = 1; i < 9; i++) {
        var ExtractTNum = $("#ExtractTNum" + i).val();
        var NoUseExtractTNum = $("#NoUseExtractTNum" + i).val();
        var STScores = $("#STScores" + i).val();
        //填写了随机答案 要求填写分数
        if (ExtractTNum.length > 0 && STScores.length > 0) {
            //拼接 题型-抽取题-未被使用-分值,
            strtxt += i + "-" + ExtractTNum + '-' + NoUseExtractTNum + '-' + STScores + ',';
            falg = true;
        }
    }

    if (falg == false) {
        layer.msg('组卷规则输入有误！', function () { });
        return;
    }


    //不自动关闭 time:-1  亮度页面不可点击 shade:0.01
    layer.msg('正在努力组卷，请稍等', {
        icon: 16, shade: 0.01, time: -1
    });

    $.ajax({
        type: "POST",
        dataType: "json",
        url: '/Admin/HB_Paper/AddIntelligentTestPaper',
        data: { "Ids": strtxt, "IsOrder": IsOrder, "AddP_Name": AddP_Name },//多个Id
        success: function (data) {
            if (data == "1") {
                layer.closeAll();//关闭所有弹出框

                layer.msg('操作成功', { icon: 1, time: 800 }, function () {
                    window.location.href = '/Admin/HB_Paper/Index';
                });

            }
            if (data == "99") {
                layer.closeAll();//关闭所有弹出框
                layer.msg('操作失败', { icon: 2 });
                return;
            }
            if (data == "88") {
                layer.closeAll();//关闭所有弹出框
                layer.msg('试卷名称已经存在,请修改试卷名称', { icon: 2 });
                return;
            }
        }
    });

}