$(function () {
    box();
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

$('#Score').bind('input propertychange', function () {
    $("#Sum_Score").html(($("#SumExtract").val() * $("#Score").val()).toFixed(2))
});

$('#ExtractUnused').bind('input propertychange', function () {
    var SumExtract = $("#SumExtract").val();
    var ExtractUnused = $("#ExtractUnused").val();
    var Sum_Number = $("#Sum_Number").html();
    var Unused = $("#Unused").html();
    if (parseInt(ExtractUnused) > parseInt(SumExtract)) {
        layer.msg('输入值不能大于随机抽取题目总数！', function () { });
        $("#SumExtract").val("");
        return;
    }
    if (parseInt(ExtractUnused) > parseInt(Unused)) {
        layer.msg('抽取未被使用数不得大于当前未被使用数！', function () { });
        $("#ExtractUnused").val("");
        return;
    }
    if (parseInt(SumExtract) > parseInt(Sum_Number)) {
        layer.msg('抽取总数不得大于可用总数！', function () { });
        $("#SumExtract").val("");
        return;
    }
    if (parseInt(ExtractUnused) > parseInt(Sum_Number)) {
        layer.msg('抽取未被使用数不得大于可用总数！', function () { });
        $("#ExtractUnused").val("");
        return;
    }
});

$('#SumExtract').bind('input propertychange', function () {
    var Sum_Number = $("#Sum_Number").html();
    var Unused = $("#Unused").html();
    var SumExtract = $("#SumExtract").val();
    var ExtractUnused = $("#ExtractUnused").val();
    if (parseInt(SumExtract) > parseInt(Sum_Number)) {
        layer.msg('抽取总数不得大于可用总数！', function () { });
        $("#SumExtract").val("");
        return;
    }
    if (parseInt(ExtractUnused) > parseInt(Sum_Number)) {
        layer.msg('抽取未被使用数不得大于可用总数！', function () { });
        $("#ExtractUnused").val("");
        return;
    }
    if (parseInt(ExtractUnused) > parseInt(Unused)) {
        layer.msg('抽取未被使用数不得大于当前未被使用数！', function () { });
        $("#ExtractUnused").val("");
        return;
    }
    $("#Sum_Score").html(($("#SumExtract").val() * $("#Score").val()).toFixed(2))
});

function Save() {
    var Sum_Number = $("#Sum_Number").html();
    var Unused = $("#Unused").html();
    var PaperName = $("#PaperName").val();
    var SumExtract = $("#SumExtract").val();
    var ExtractUnused = $("#ExtractUnused").val();
    var Score = $("#Score").val();
    if (PaperName == "") {
        layer.msg('试卷名称为必填项！', function () { });
        return;
    }
    if (SumExtract == "") {
        layer.msg('共抽取题目数量为必填项！', function () { });
        return;
    }
    if (Score == "") {
        layer.msg('分数为必填项！', function () { });
        return;
    }
    var toppd = Paper_Pd($("#PaperName").val())
    if (toppd > 0) {
        layer.msg('试卷名称不能重复！', function () { });
        return;
    }
    var rad_strchks = "";
    var rad_chks = document.getElementsByName('rad');//name
    for (var i = 0; i < rad_chks.length; i++) {
        if (rad_chks[i].checked == true) {
            rad_strchks = "" + rad_chks[i].value + "";
        }
    }
    $.ajax({
        type: "POST",
        dataType: "json",
        async: false,
        url: '/Admin/BillTestPaper/SaveTestPaper',
        data: { PaperName: PaperName, SumExtract: SumExtract, ExtractUnused: ExtractUnused, Score: Score, Sequence: rad_strchks },
        success: function (data) {
            if (data > 0) {
                window.location.href = "/Admin/BillTestPaper/TestPaperList";
            } else {
                layer.msg('操作失败！', { icon: 2 });
            }
        }
    });
}

//重复判断
function Paper_Pd(PaperName) {
    var data1;
    $.ajax({
        type: "POST",
        dataType: "text",
        async: false,
        url: '/Admin/BillTestPaper/Paper_Pd',
        data: { PaperName: PaperName },
        success: function (data) {
            data1 = data;
        }
    });
    return data1;
}
