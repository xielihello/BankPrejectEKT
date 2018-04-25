/***************************************************************
  FileName:货币知识考试新增 javascript
  Copyright（c）2017-金融教育在线技术开发部
  Author:袁学
  Create Date:2017-4-10
 ******************************************************************/

$(document).ready(function () {

    redload();
    BindDll();
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

////绑定班级下拉框
function BindDll() {
    $.ajax({
        url: '/Admin/HB_Examination/GetListTeam',
        type: 'POST',
        dataType: 'json',
        async: false,
        success: function (data) {
            var html = '<option value="">请选择支行</option>';
            if (data != null) {
                for (var i = 0; i < data.length; i++) {
                    html += '<option value="' + data[i]["Id"] + '">' + data[i]["TeamName"] + '</option>';
                }
            }
            $("#AddTeamId").html(html);
        }
    })

}


//下拉框事件
function SelectPId() {
    var txtSelect = $("#AddE_PId").find("option:selected").text();
    $("#AddP_Name").val(txtSelect);
}

//新增保存
function AddBtnSubim() {

    var AddE_PId = $("#AddE_PId").val();
    var AddP_Name = $("#AddP_Name").val();
    var AddE_TeamId = $("#AddTeamId").val();
    var AddE_StartTime = $("#AddE_StartTime").val();
    var AddE_EndTime = $("#AddE_EndTime").val();
    var AddE_Type = $('input[name="E_Type"]:checked').val();
    var AddE_Whenlong = $("#AddE_Whenlong").val();
    var AddE_IsTimeBonus = $('input[name="E_IsTimeBonus"]:checked').val();

    if (AddE_PId.length == 0) {
        layer.msg('请选择试卷！', function () { });
        return false;
    }
    if (AddP_Name.length == 0) {
        layer.msg('请填写考试名称！', function () { });
        return false;
    }
    if (AddP_Name.length > 15) {
        layer.msg('考试名称长度不能超过15个汉字！', function () { });
        return false;
    }
    var usertpe = $("#usertypehidd").val();//当前登录角色
    if (usertpe == "2") {//只有教师端 才校验班级是否选中
        if (AddE_TeamId.length == 0) {
            layer.msg('请选择分配支行！', function () { });
            return false;
        }
        AddE_TeamId = "," + AddE_TeamId + ",";
    }

    if (AddE_StartTime.length == 0) {
        layer.msg('请设置考试开始时间！', function () { }); return false;

    }
    if (AddE_EndTime.length == 0) {
        layer.msg('请设置考试结束时间！', function () { }); return false;

    }
    var t1 = new Date(AddE_StartTime);
    var t2 = new Date(AddE_EndTime);
    if (t1 > t2) {
        layer.msg('考试开始时间不能大于结束时间！', function () { });
        return false;
    }
    if (AddE_Whenlong.length == 0) {
        layer.msg('请填写考试时长！', function () { });
        return false;
    }

    var date3 = (t2.getTime() - t1.getTime()) / 1000;// //时间差的秒数
    //计算分钟
    var leave1 = date3 / (60);

    if (parseInt(AddE_Whenlong) > parseInt(leave1)) {
        layer.msg('考试时长不能大于考试有效时间段！', function () { });
        return false;
    }

    var TeamId = AddE_TeamId;
    $.ajax({
        type: "POST",
        dataType: "text",
        url: '/Admin/HB_Examination/Add',
        data: {
            "AddE_PId": AddE_PId, "AddP_Name": AddP_Name, "AddE_TeamId": TeamId,
            "AddE_StartTime": AddE_StartTime, "AddE_EndTime": AddE_EndTime, "AddE_Type": AddE_Type,
            "AddE_Whenlong": AddE_Whenlong, "AddE_IsTimeBonus": AddE_IsTimeBonus
        },
        success: function (data) {
            if (data == "1") {
                layer.closeAll();//关闭所有弹出框
                layer.msg('操作成功', { icon: 1, time: 800 }, function () {
                    window.location.href = '/Admin/HB_Examination/Index';
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

//返回
function AddFormRest() {
    window.location.href = '/Admin/HB_Examination/Index';
}