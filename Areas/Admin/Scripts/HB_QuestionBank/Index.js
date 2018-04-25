/***************************************************************
  FileName:货币知识题库管理 javascript
  Copyright（c）2017-金融教育在线技术开发部
  Author:袁学
  Create Date:2017-4-1
 ******************************************************************/


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
}

//列表数据加载
function bindIngfo(page) {
    var QB_Type = $("#QB_Type").val();//题型
    var QB_Description = $("#QB_Description").val();//描述
    var QB_Kind = $("#QB_Kind").val();//题目属性
    var QB_Custom2 = $("#QB_Custom2").val();//来源 账号
    var PageSize = 10;
  
    $.ajax({
        url: '/Admin/HB_QuestionBank/GetList',
        Type: "post",
        dataType: "json", cache: false,
        contentType: "application/json; charset=utf-8",
        data: { "QB_Type": QB_Type, "QB_Description": QB_Description, "QB_Kind": QB_Kind, "QB_Custom2": QB_Custom2, "page": page, "PageSize": PageSize },
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



                html += '<td title="' + delHtmlTag(HTMLDecode(data[i]["QB_Description"])) + '"><span class="pie" >' + txtQB_Description + '</span></td>';
                html += '<td><span class="pie">' + GetTiType(data[i]["QB_Type"]) + '</span></td>';

                //只有管理员端有 题目属性和题目来源2字段
                if (parseInt(UserType) == 1) {

                    var txtQB_Kind = '系统题目';
                    if (parseInt(data[i]["QB_Kind"]) == 2) {
                        txtQB_Kind = '教师题目';
                    }
                    html += '<td><span class="pie">' + txtQB_Kind + '</span></td>';

                    if (data[i]["UserName"] == "null" || data[i]["UserName"] == null || data[i]["UserName"].length == 0) {
                        html += '<td><span class="pie">' + data[i]["QB_Custom2"] + '</span></td>';
                    } else {
                        html += '<td><span class="pie">' + data[i]["UserName"] + '</span></td>';
                    }



                }

                html += '<td>';


                var MQB_Kind = data[i]["QB_Kind"];//1.系统题目（所有管理员的） 2.教师的题
                if (parseInt(UserType) == 1) {  //如果是管理员

                    if (parseInt(MQB_Kind) == 1) {
                        //我是管理员面对管理员新增的题时候：  此题是管理员增加的 允许修改
                        //我是管理员面对教师增加的题时候：不让改只能查看
                        html += '<a href="javascript:void(0);" onclick="Edit(' + data[i]["QuestionBId"] + ')" class=" btn-primary btn-sm"><i class="fa fa-pencil m-r-xxs"></i>修改 </a> ';
                    }
                }
                else {
                    //如果我是教师 展示的是自己 还有可能展示的是管理员的
                    //展示的是自己的 ，来源自己新增的 可以修改 其余的只能查看
                    if (parseInt(MQB_Kind) == 2) {
                        html += '<a href="javascript:void(0);" onclick="Edit(' + data[i]["QuestionBId"] + ')" class=" btn-primary btn-sm"><i class="fa fa-pencil m-r-xxs"></i>修改 </a> ';
                    }
                }

                html += '<a style="margin-left: 5px;" href="javascript:void(0);" onclick="See(' + data[i]["QuestionBId"] + ')" class=" btn-info btn-sm"><i class="fa fa-lightbulb-o m-r-xxs"></i>查看 </a></td>';
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
    if (UserType != "0") {
        bindIngfo();
    }
}

//修改
function Edit(QuestionBId) {
    window.location.href = '/Admin/HB_QuestionBank/EditIndex?QuestionBId=' + QuestionBId;
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

//新增
function AddInfo() {
    window.location.href = '/Admin/HB_QuestionBank/AddIndex';
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
                    if (d[0]["QB_D"] != "" && d[0]["QB_D"].length>0) {
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

//删除
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

    layer.confirm('您确定要删除所选试题吗？', {
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
            url: '/Admin/HB_QuestionBank/DelQuestionBank',
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

//下载模板
function DownloadTemplates() {
    window.open('/Export/理论知识试题导入模版.xls');
}

//导入按钮
function Import() {
    $("#FileName").val('');
    jQuery('#ImportDIV').modal('show', { backdrop: 'static' });
}

//导入excel
function btnImport() {
    var docObj = document.getElementById("FileName");

    //上传文件校验只能是excel
    if (docObj.files && docObj.files[0]) {
        var f = docObj.files;
        var exltype = f[0].name;//获取文件名
        var exp = /.xls$|.xlsx$/;
        if (exp.exec(exltype) == null) {
            layer.msg('上传格式错误（仅支持.xls和.xlsx文件）', { icon: 2 });
            return;
        }
    }
    else {
        layer.msg('请选择上传文件！', function () { });
        return;
    }

    //提交
    $("#Importform").ajaxSubmit({
        type: "post",
        dataType: "json",
        url: '/ashx/hbExport.ashx?action=HBZS_QuestionBankImport',
        data: $("#Importform").serialize(),
        success: function (data) {
            if (data[0]["error"] == "") {//正常结束
                layer.closeAll();//关闭所有弹出框
                bindIngfo();


                $("#FileName").val('');
                jQuery('#ImportDIV').modal('hide');
                var success = data[0]["success"];//成功
                var repeat = data[0]["repeat"];

                if (parseInt(repeat) == 0) {
                    layer.msg('已成功导入题目' + success + '道！', { icon: 1 });
                }
                else {
                    layer.msg('成功导入题目' + success + '道,其中有' + repeat + '道重复未导入系统！', { icon: 1 });
                }

            }
            else {
                var success = data[0]["success"];//成功
                var repeat = data[0]["repeat"];
                layer.msg('已导入题目' + success + '道！出错：' + data[0]["error"], { icon: 2 });
                return;
            }
        }
    });

}
