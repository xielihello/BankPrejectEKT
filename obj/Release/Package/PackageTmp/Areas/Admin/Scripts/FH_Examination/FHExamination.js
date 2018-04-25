/***************************************************************
  FileName:复核考试管理 javascript
  Copyright（c）2017-金融教育在线技术开发部
  Author:邵世铨
  Create Date:2017-4-11
 ******************************************************************/
$(document).ready(function () {
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
    bindIngfo();
});
//删除
function Dele() {
    var chks = document.getElementsByName('input[]');//name
    var chkstr = "";
    for (var i = 0; i < chks.length; i++) {
        if (chks[i].checked == true) {
            chkstr += chks[i].value + ",";
        }
    }

    if (chkstr.length == 0) {
        layer.msg('请选择要删除的数据！', function () { });
        return;
    }



    chkstr = chkstr.substr(0, chkstr.length - 1);
    var deletitle = "";
    if (chkstr.split(',').length > 1) {//单选还是多选
        deletitle = "您确定要删除所选考试吗(不含已激活状态下考试)？";
    }
    else {
        //去查询下校验下
        //只勾选了一行
        var IsStateId = $("#IsStateId" + chkstr).val();
        if (IsStateId == "1") {//激活
            layer.msg('请先关闭激活后删除！', function () { });
            return;
        } else {
            deletitle = "您确定要删除所选考试吗？";
        }
    }

    layer.confirm(deletitle, {
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
                url: '/Admin/FH_Examination/Delete',
                data: { Id: chkstr },
                success: function (data) {
                    if (data > 0) {
                        layer.closeAll();//关闭所有弹出框
                        bindIngfo();
                        layer.msg('操作成功！', { icon: 1 });
                    } else {
                        layer.msg('操作失败！', { icon: 2 });
                    }
                }
            });
        });
}
function Activation() {
    var chks = document.getElementsByName('input[]');//name
    var chkstr = "";
    for (var i = 0; i < chks.length; i++) {
        if (chks[i].checked == true) {
            chkstr += chks[i].value + ",";
        }
    }
    chkstr += "0";
    if (chkstr == "" || chkstr == "0") {
        layer.msg('请先选中一项', function () { });
        return false;
    }
    layer.confirm('您确定要激活所选考试吗？', {
        title: '系统提示',
        btn: ['确定', '取消'],
        shadeClose: true, //开启遮罩关闭
        skin: 'layui-layer-lan'
        //按钮
    },
        function () {
            $.ajax({
                type: "POST",
                // dataType: "text",
                async: false,
                url: '/Admin/FH_Examination/Activation',
                data: { Id: chkstr },
                success: function (data) {
                    if (data > 0) {
                        layer.closeAll();//关闭所有弹出框
                        bindIngfo();
                        layer.msg('操作成功！', { icon: 1 });
                    } else {
                        layer.msg('操作失败！', { icon: 2 });
                    }

                }
            });
        });
}
function Close() {
    var chks = document.getElementsByName('input[]');//name
    var chkstr = "";
    for (var i = 0; i < chks.length; i++) {
        if (chks[i].checked == true) {
            chkstr += chks[i].value + ",";
        }
    }
    chkstr += "0";
    if (chkstr == "" || chkstr == "0") {
        layer.msg('请先选中一项', function () { });
        return false;
    }
    layer.confirm('您确定要关闭所选考试吗？', {
        title: '系统提示',
        btn: ['确定', '取消'],
        shadeClose: true, //开启遮罩关闭
        skin: 'layui-layer-lan'
        //按钮
    },
        function () {
            $.ajax({
                type: "POST",
                // dataType: "text",
                async: false,
                url: '/Admin/FH_Examination/Close',
                data: { Id: chkstr },
                success: function (data) {
                    if (data > 0) {
                        layer.closeAll();//关闭所有弹出框
                        bindIngfo();
                        layer.msg('操作成功！', { icon: 1 });
                    } else {
                        layer.msg('操作失败！', { icon: 2 });
                    }
                }
            });
        });
}
//新增
function Added() {
    window.location.href = "/Admin/FH_Examination/Add";
}
//编辑
function Edit(EId, Isactivation) {
    if (Isactivation == "1") {//
        layer.msg('请先关闭激活！', function () { });
        return;

    }
    window.location.href = '/Admin/FH_Examination/Edit?EId=' + EId;
}
//查看 E_Kind=1 管理员端 直接弹出框查看， E_Kind=2 教师端跳转页面查看分配班级等信息
function Show(EId, E_Kind) {

    jQuery('#searchDIV').modal('show', { backdrop: 'static' });
    $.ajax({
        url: '/Admin/FH_Examination/GetListById?EId=' + EId,
        type: 'POST',
        dataType: 'json',
        async: false,
        success: function (data) {
            if (data.length > 0) {
                $("#Span1").html(data[0]["ExaminationName"]);

                //考试有效时间
                var E_StartTime = data[0]["ExaminationStartTime"] + "";
                var E_EndTime = data[0]["ExaminationEndTime"] + "";

                $("#Span2").html(E_StartTime.substr(0, 19).replace('T', ' ') + ' -- ' + E_EndTime.substr(0, 19).replace('T', ' '));
                var txtE_Type = '考试模式';
                if (parseInt(data[0]["ExaminationType"]) == 1) {
                    txtE_Type = '练习模式';
                }

                $("#Span3").html(txtE_Type);//考试模式
                $("#Span4").html(data[0]["ExaminationLength"]);//考试时长

                var txtE_IsTimeBonus = "是";
                if (parseInt(data[0]["IsPlus"]) == 2) {
                    txtE_IsTimeBonus = '否';
                }
                $("#Span5").html(txtE_IsTimeBonus);//是否时间加分

                if (E_Kind == "2") {
                    var E_TeamId = data[0]["Spare2"] + "";
                    var teamlist = E_TeamId.substring(1, E_TeamId.length - 1);//去掉前后 ，
                    $.ajax({
                        url: '/Admin/FH_Examination/GetListTeamByInId?Id=' + teamlist,
                        type: 'POST',
                        dataType: 'json',
                        async: false,
                        success: function (d) {
                            var teamhtml = "";

                            for (var i = 0; i < d.length; i++) {
                                teamhtml += d[i]["TeamName"] + ",";
                            }
                            if (teamhtml.length > 0) {
                                teamhtml = teamhtml.substr(0, teamhtml.length - 1);
                            }
                            $("#Span6").html(teamhtml);
                        }
                    });
                    $("#serachTadm").css("display", "block");
                }
            }
        }
    });
}


//列表数据加载
function bindIngfo(page) {

    var E_Name = $("#TeacherName").val();//考试名称
    var E_Type = $("#PatternId").val();//竞赛模式
    var E_IsState = $("#StateId").val();//激活状态
    var PageSize = 10;

    $.ajax({
        url: '/Admin/FH_Examination/GetList',
        Type: "post",
        dataType: "json", cache: false,
        contentType: "application/json; charset=utf-8",
        data: { "E_Name": E_Name, "E_Type": E_Type, "E_IsState": E_IsState, "page": page, "PageSize": PageSize },
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

                html += '<td><input type="checkbox" class="i-checks" name="input[]" value="' + data[i]["Id"] + '"></td>';

                html += '<td><span class="pie" >' + data[i]["ExaminationName"] + '</span></td>';

                var txtE_Type = '考试模式';
                if (parseInt(data[i]["ExaminationType"]) == 1) {
                    txtE_Type = '练习模式';
                }
                html += '<td><span class="pie">' + txtE_Type + '</span></td>';

                var E_StartTime = data[i]["ExaminationStartTime"] + "";
                var E_EndTime = data[i]["ExaminationEndTime"] + "";

                html += '<td><span class="pie">' + E_StartTime.substr(0, 19).replace('T', ' ') + ' -- ' + E_EndTime.substr(0, 19).replace('T', ' ') + '</span></td>';
                var txtE_IsState = '';
                if (parseInt(data[i]["Isactivation"]) == 1) {
                    txtE_IsState = '已激活';
                }

                if (parseInt(data[i]["Isactivation"]) == 2) {
                    txtE_IsState = '未激活';
                }
                html += '<td><span class="pie">' + txtE_IsState + '</span><input type="hidden"  id="IsStateId' + data[i]["Id"] + '" value="' + data[i]["Isactivation"] + '"/></td>';


                html += '<td>';
                //查看 中看种类区分E_Kind
                html += '<a href="javascript:void(0);" onclick="Show(' + data[i]["Id"] + ')" class=" btn-info btn-sm"><i class="fa fa-lightbulb-o m-r-xxs"></i>查看 </a>';
                html += '<a style="margin-left: 5px;"  href="javascript:void(0);" onclick="Edit(' + data[i]["Id"] + ',' + data[i]["Isactivation"] + ')" class=" btn-primary btn-sm"><i class="fa fa-pencil m-r-xxs"></i>编辑 </a> ';
                html += '</td>';

                html += '</tr>';
            }

            $("#tablelist").html(html);

            bootstrapPaginator("#PaginatorLibrary", tb, bindIngfo);//分页
            //样式重新加载
            redload();
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