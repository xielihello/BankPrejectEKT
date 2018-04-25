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
$(document).ready(function () {
    redload();
    var v = "";
    $.ajax({
        type: "POST",
        async: false,
        url: '/Admin/BillExam/SelectTask',
        data: { id: 0 },
        success: function (data) {
            v = data
        }
    });
    $("#selectId").html(v);
    var T = "";
    $.ajax({
        type: "POST",
        async: false,
        url: '/Admin/BillExam/GetListTeam',
        data: { id: 0 },
        success: function (data) {
            T = data
        }
    });
    $("#selectId2").html(T);
    var config = {
        '.chosen-select': {},
        '.chosen-select-deselect': {
            allow_single_deselect: true
        },
        '.chosen-select-no-single': {
            disable_search_threshold: 10
        },
        '.chosen-select-no-results': {
            no_results_text: 'Oops, nothing found!'
        },
        '.chosen-select-width': {
            width: "95%"
        }
    }
    for (var selector in config) {
        $(selector).chosen(config[selector]);
    }
});

function Return() {
    window.location.href = "/Admin/BillExam/ExamList";
}

function Save() {
    debugger;
    var Taskid = $("#selectId").val();
    var EamxName = $("#EamxName").val();
    var Classid = $("#selectId2").val();
    var StartTime = $("#StartTime").val();
    var EndTime = $("#EndTime").val();
    var Pattern = $('input[name="Pattern"]:checked').val();
    var minId = $("#minId").val();
    var Plus = $('input[name="Plus"]:checked').val();

    if (Taskid == 0) {
        layer.msg('请选择试卷！', function () { });
        return false;
    }
    if (EamxName.length == 0) {
        layer.msg('请填写考试名称！', function () { });
        return false;
    }
    var usertpe = $("#usertypehidd").val();//当前登录角色
    if (usertpe == "2") {//只有教师端 才校验班级是否选中
        if (Classid.length == 0) {
            layer.msg('请选择分配班级！', function () { });
            return false;
        }
    }

    if (StartTime.length == 0) {
        layer.msg('请设置考试开始时间！', function () { }); return false;

    }
    if (EndTime.length == 0) {
        layer.msg('请设置考试结束时间！', function () { }); return false;

    }
    var t1 = new Date(StartTime);
    var t2 = new Date(EndTime);
    if (t1 > t2) {
        layer.msg('考试开始时间不能大于结束时间！', function () { });
        return false;
    }
    if (minId.length == 0) {
        layer.msg('请填写考试时长！', function () { });
        return false;
    }

    var date3 = (t2.getTime() - t1.getTime()) / 1000;// //时间差的秒数
    //计算分钟
    var leave1 = date3 / (60);

    if (parseInt(minId) > parseInt(leave1)) {
        layer.msg('考试时长不能大于考试有效时间段！', function () { });
        return false;
    }

    var TeamId = "," + Classid + ",";

    $.ajax({
        type: "POST",
        dataType: "text",
        url: '/Admin/BillExam/AddSave',
        data: {
            "Taskid": Taskid, "EamxName": EamxName, "Classid": TeamId,
            "StartTime": StartTime, "EndTime": EndTime, "Pattern": Pattern,
            "minId": minId, "Plus": Plus
        },
        success: function (data) {
            if (data == "1") {
                layer.closeAll();//关闭所有弹出框
                layer.msg('操作成功', { icon: 1, time: 800 }, function () {
                    window.location.href = '/Admin/BillExam/ExamList/';
                });


            }
            if (data == "88") {
                layer.msg('考试名称已经存在！', { icon: 2 });
                return;
            }
            if (data == "99") {
                layer.msg('操作失败', { icon: 2 });
                return;
            }

        }
    })

}
function gradeChange() {
    var text = $('#selectId option:selected').text();
    $("#EamxName").val(text);
}