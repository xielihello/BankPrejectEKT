/***************************************************************
  FileName:货币知识考试编辑 javascript
  Copyright（c）2017-金融教育在线技术开发部
  Author:袁学
  Create Date:2017-4-10
 ******************************************************************/

// JS 获取URL参数方法
function getQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) return unescape(r[2]); return null;
}

$(document).ready(function () {

    redload();
    BindDll();
    GetListById();
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

//编辑时信息读取
function GetListById() {
    $.ajax({
        Type: "post",
        dataType: "json",
        url: '/Admin/HB_Examination/GetListById?EId=' + getQueryString('EId'),
        async: false,
        success: function (data) {
            if (data.length > 0) {
                if (parseInt(data[0]["num"])>0) {
                    $("#EditE_PId").val(data[0]["E_PId"]);
                }
                
                $("#EditP_Name").val(data[0]["E_Name"]);
  
                //班级
                var usertpe = $("#usertypehidd").val();//当前登录角色
                if (usertpe == "2") {//只有教师端 才班级赋值
                    var E_TeamId = data[0]["E_TeamId"] + "";
                    var teamlist = E_TeamId.substring(1, E_TeamId.length - 1);//去掉前后 ，
                    var teamarr = teamlist.split(',');
                    $("#EditTeamId").val(teamarr);
                }
                var E_StartTime = data[0]["E_StartTime"] + "";
                var E_EndTime = data[0]["E_EndTime"] + "";


                $("#EditE_StartTime").val(E_StartTime.replace('T', ' '));

                $("#EditE_EndTime").val(E_EndTime.replace('T', ' '));

                //竞赛模式
                var E_Type = data[0]["E_Type"];
                document.getElementById("E_TypeA").checked = false;
                $("#E_TypeA").parent().removeClass('checked');
                document.getElementById("E_TypeB").checked = false;
                $("#E_TypeB").parent().removeClass('checked');

                if (E_Type == "1") {
                    document.getElementById("E_TypeA").checked = true;
                    $("#E_TypeA").parent().addClass('checked');
                }
                if (E_Type == "2") {
                    document.getElementById("E_TypeB").checked = true;
                    $("#E_TypeB").parent().addClass('checked');
                }

                //考试时长
                $("#EditE_Whenlong").val(data[0]["E_Whenlong"]);

                //是否时间加分
                var E_IsTimeBonus = data[0]["E_IsTimeBonus"];
                document.getElementById("E_IsTimeBonusA").checked = false;
                $("#E_IsTimeBonusA").parent().removeClass('checked');
                document.getElementById("E_IsTimeBonusB").checked = false;
                $("#E_IsTimeBonusB").parent().removeClass('checked');

                if (E_IsTimeBonus == "1") {
                    document.getElementById("E_IsTimeBonusA").checked = true;
                    $("#E_IsTimeBonusA").parent().addClass('checked');
                }
                if (E_IsTimeBonus == "0") {
                    document.getElementById("E_IsTimeBonusB").checked = true;
                    $("#E_IsTimeBonusB").parent().addClass('checked');
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
            $("#EditTeamId").html(html);
        }
    })

}


//下拉框事件
function SelectPId() {
    var txtSelect = $("#EditE_PId").find("option:selected").text();
    $("#EditP_Name").val(txtSelect);
}

function EditBtnSubim() {
    var EditE_PId = $("#EditE_PId").val();
    var EditP_Name = $("#EditP_Name").val();
    var EditTeamId = $("#EditTeamId").val();
    var EditE_StartTime = $("#EditE_StartTime").val();
    var EditE_EndTime = $("#EditE_EndTime").val();
    var EditE_Type = $('input[name="E_Type"]:checked').val();
    var EditE_Whenlong = $("#EditE_Whenlong").val();
    var EditE_IsTimeBonus = $('input[name="E_IsTimeBonus"]:checked').val();
    
    if (EditE_PId==null) {
        layer.msg('请选择试卷！', function () { });
        return false;
    }
    if (EditE_PId.length == 0) {
        layer.msg('请选择试卷！', function () { });
        return false;
    }
    if (EditP_Name.length == 0) {
        layer.msg('请填写考试名称！', function () { });
        return false;
    }
    if (EditP_Name.length > 15) {
        layer.msg('考试名称长度不能超过15个汉字！', function () { });
        return false;
    }
    var usertpe = $("#usertypehidd").val();//当前登录角色
    if (usertpe == "2") {//只有教师端 才校验班级是否选中
        if (EditTeamId.length == 0) {
            layer.msg('请选择分配支行！', function () { });
            return false;
        }
        EditTeamId = "," + EditTeamId + ",";

    }

    if (EditE_StartTime.length == 0) {
        layer.msg('请设置考试开始时间！', function () { }); return false;

    }
    if (EditE_EndTime.length == 0) {
        layer.msg('请设置考试结束时间！', function () { }); return false;

    }
    var t1 = new Date(EditE_StartTime);
    var t2 = new Date(EditE_EndTime);
    if (t1 > t2) {
        layer.msg('考试开始时间不能大于结束时间！', function () { });
        return false;
    }
    if (EditE_Whenlong.length == 0) {
        layer.msg('请填写考试时长！', function () { });
        return false;
    }

    var date3 = (t2.getTime() - t1.getTime()) / 1000;// //时间差的秒数
    //计算分钟
    var leave1 = date3 / (60);

    if (parseInt(EditE_Whenlong) > parseInt(leave1)) {
        layer.msg('考试时长不能大于考试有效时间段！', function () { });
        return false;
    }

    var TeamId = EditTeamId;
  
   
    $.ajax({
        type: "POST",
        dataType: "text",
        url: '/Admin/HB_Examination/Edit',
        data: {
            "EditE_PId": EditE_PId, "EditP_Name": EditP_Name, "EditTeamId": TeamId,
            "EditE_StartTime": EditE_StartTime, "EditE_EndTime": EditE_EndTime, "EditE_Type": EditE_Type,
            "EditE_Whenlong": EditE_Whenlong, "EditE_IsTimeBonus": EditE_IsTimeBonus, "EId": getQueryString('EId')
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
function EditFormRest() {
    window.location.href = '/Admin/HB_Examination/Index';
}
