/***************************************************************
  FileName:货币知识考试管理 javascript
  Copyright（c）2017-金融教育在线技术开发部
  Author:袁学
  Create Date:2017-4-8
 ******************************************************************/

$(function () {
    bindIngfo();
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

//列表数据加载
function bindIngfo(page) {

    var E_Name = $("#E_Name").val();//考试名称
    var E_Type = $("#E_Type").val();//竞赛模式
    var E_IsState = $("#E_IsState").val();//激活状态
    var PageSize = 10;

    $.ajax({
        url: '/Admin/HB_Examination/GetList',
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

                html += '<td><input type="checkbox" class="i-checks" name="input[]" value="' + data[i]["EId"] + '"></td>';
                html += '<td><span class="pie" >' + data[i]["E_Name"] + '</span></td>';

                var txtE_Type = '考试模式';
                if (parseInt(data[i]["E_Type"]) == 2) {
                    txtE_Type = '练习模式';
                }
                html += '<td><span class="pie">' + txtE_Type + '</span></td>';

                var E_StartTime = data[i]["E_StartTime"] + "";
                var E_EndTime = data[i]["E_EndTime"] + "";

                html += '<td><span class="pie">' + E_StartTime.substr(0, 19).replace('T', ' ') + ' -- ' + E_EndTime.substr(0, 19).replace('T', ' ') + '</span></td>';

                var txtE_IsState = '已激活';
                if (parseInt(data[i]["E_IsState"]) == 2) {
                    txtE_IsState = '未激活';
                }
                html += '<td><span class="pie">' + txtE_IsState + '</span><input type="hidden"  id="IsStateId' + data[i]["EId"] + '" value="' + data[i]["E_IsState"] + '"/></td>';


                html += '<td>';
                //查看 中看种类区分E_Kind
                html += '<a href="javascript:void(0);" onclick="See(' + data[i]["EId"] + ',' + data[i]["E_Kind"] + ')" class=" btn-info btn-sm"><i class="fa fa-lightbulb-o m-r-xxs"></i>查看 </a>';
                html += '<a style="margin-left: 5px;"  href="javascript:void(0);" onclick="Edit(' + data[i]["EId"] + ',' + data[i]["E_IsState"] + ')" class=" btn-primary btn-sm"><i class="fa fa-pencil m-r-xxs"></i>编辑 </a> ';
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

//搜索
function searchinfo() {

    bindIngfo();

}


//查看 E_Kind=1 管理员端 直接弹出框查看， E_Kind=2 教师端跳转页面查看分配班级等信息
function See(EId, E_Kind) {

    jQuery('#searchDIV').modal('show', { backdrop: 'static' });
    $.ajax({
        url: '/Admin/HB_Examination/GetListById?EId=' + EId,
        type: 'POST',
        dataType: 'json',
        async: false,
        success: function (data) {
            if (data.length > 0) {
                $("#Span1").html(data[0]["E_Name"]);

                //考试有效时间
                var E_StartTime = data[0]["E_StartTime"] + "";
                var E_EndTime = data[0]["E_EndTime"] + "";

                $("#Span2").html(E_StartTime.substr(0, 19).replace('T', ' ') + ' -- ' + E_EndTime.substr(0, 19).replace('T', ' '));
                var txtE_Type = '考试模式';
                if (parseInt(data[0]["E_Type"]) == 2) {
                    txtE_Type = '练习模式';
                }

                $("#Span3").html(txtE_Type);//考试模式
                $("#Span4").html(data[0]["E_Whenlong"]);//考试时长

                var txtE_IsTimeBonus = "是";
                if (parseInt(data[0]["E_IsTimeBonus"]) == 0) {
                    txtE_IsTimeBonus = '否';
                }
                $("#Span5").html(txtE_IsTimeBonus);//是否时间加分

                if (E_Kind == "2") {
                    var E_TeamId = data[0]["E_TeamId"] + "";
                    var teamlist = E_TeamId.substring(1, E_TeamId.length - 1);//去掉前后 ，

                    $.ajax({
                        url: '/Admin/HB_Examination/GetListTeamByInId?Id=' + teamlist,
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

//编辑
function Edit(EId, E_IsState) {
    if (E_IsState == "1") {//
        layer.msg('请先关闭激活！', function () { });
        return;

    }
    window.location.href = '/Admin/HB_Examination/EditIndex?EId=' + EId;
}

//激活 状态修改 激活 1
function activation() {
    var chks = document.getElementsByName('input[]');//name
    var chkstr = "";
    for (var i = 0; i < chks.length; i++) {
        if (chks[i].checked == true) {
            chkstr += chks[i].value + ",";
        }
    }

    if (chkstr.length == 0) {
        layer.msg('请先选中一项', function () { });
        return;
    }

    chkstr = chkstr.substr(0, chkstr.length - 1);
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
               dataType: "text",
               url: '/Admin/HB_Examination/EditIsState',
               data: { "Ids": chkstr, "E_IsState": "1" },//多个Id
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

//关闭激活 2
function closeactivation() {
    var chks = document.getElementsByName('input[]');//name
    var chkstr = "";
    for (var i = 0; i < chks.length; i++) {
        if (chks[i].checked == true) {
            chkstr += chks[i].value + ",";
        }
    }

    if (chkstr.length == 0) {
        layer.msg('请先选中一项', function () { });
        return;
    }

    chkstr = chkstr.substr(0, chkstr.length - 1);

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
               dataType: "text",
               url: '/Admin/HB_Examination/EditIsState',
               data: { "Ids": chkstr, "E_IsState": "2" },//多个Id
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

//批量删除 只能删除未激活状态下的考试
function del_all() {
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
                url: '/Admin/HB_Examination/DelExamination',
                data: { "Ids": chkstr },//多个Id
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