/***************************************************************
  FileName:货币知识题库管理编辑 javascript
  Copyright（c）2017-金融教育在线技术开发部
  Author:袁学
  Create Date:2017-4-1
 ******************************************************************/

// JS 获取URL参数方法
function getQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) return unescape(r[2]); return null;
}

$(document).ready(function () { //通用的复选及单选框样式
    $('.i-checks').iCheck({
        checkboxClass: 'icheckbox_square-green',
        radioClass: 'iradio_square-green',
    });

});


//单行信息读取
function bindInfoById() {
    $.ajax({
        url: '/Admin/HB_QuestionBank/GetListById?QuestionBId=' + getQueryString('QuestionBId'),
        type: 'POST',
        dataType: 'json',
        async: false,
        success: function (data) {
            if (data.length > 0) {
                var QB_Type = parseInt(data[0]["QB_Type"]);
                $("#Type").val(QB_Type);
                TypeSelct(QB_Type);
                //描述html 反转义

                $('.summernote').code(HTMLDecode(data[0]["QB_Description"]));

                if (QB_Type == 1) {//单选题
                    $("#danxAtxt").val(data[0]["QB_A"]); $("#danxBtxt").val(data[0]["QB_B"]);
                    $("#danxCtxt").val(data[0]["QB_C"]); $("#danxDtxt").val(data[0]["QB_D"]);

                    document.getElementById("danxuan" + data[0]["QB_Answer"]).checked = true;
                    $("#danxuan" + data[0]["QB_Answer"]).parent().addClass('checked');
                }

                if (QB_Type == 2) {//多选题
                    $("#duoxtcbxAtxt").val(data[0]["QB_A"]); $("#duoxtcbxBtxt").val(data[0]["QB_B"]);
                    $("#duoxtcbxCtxt").val(data[0]["QB_C"]); $("#duoxtcbxDtxt").val(data[0]["QB_D"]);
                    $("#duoxtcbxEtxt").val(data[0]["QB_E"]);
                    var QB_Answer = data[0]["QB_Answer"] + "";
                    var ListAnswer = QB_Answer.split(',');
                    for (var i = 0; i < ListAnswer.length; i++) {
                        document.getElementById("duoxtcbx" + ListAnswer[i]).checked = true;
                        $("#duoxtcbx" + ListAnswer[i]).parent().addClass('checked');
                    }

                }

                if (QB_Type == 3) {//判断题
                    var QB_Answer = data[0]["QB_Answer"];
                    if (QB_Answer == "对") {
                        document.getElementById("danxuantiA").checked = true;
                        $("#danxuantiA").parent().addClass('checked');
                    }
                    if (QB_Answer == "错") {
                        document.getElementById("danxuantiB").checked = true;
                        $("#danxuantiB").parent().addClass('checked');
                    }
                }

                if (QB_Type == 4) {//填空题
                    $("#tiankongtiAnswer").val(data[0]["QB_Answer"]);
                }
                //其它
                if (QB_Type == 5 || QB_Type == 6 || QB_Type == 7 || QB_Type == 8) {
                    $("#QTAnswer").val(data[0]["QB_Answer"]);//标准答案
                    $("#QTKeyword").val(data[0]["QB_Keyword"]);//关键字

                }

            }

        }
    });
}

//下拉框控制题型			
var map = {
    "0": "MR", //默认题目
    "1": "DanX", //单选
    "2": "DuoX", //多选
    "3": "PD", //判断题
    "4": "TK", //填空题
    "5": "QT", //简答题
    "6": "QT", //名词解析
    "7": "QT", //案例分析
    "8": "QT", //论述题
};

//试题类型下拉
function TypeSelct(value) {
    $('#Editform')[0].reset();//数据清理
    $("#Type").val(value);
    var divId = map[value];
    $("#" + divId).show().siblings().hide(); //显示当前所选的slect值，隐藏其他的slect值
}

//富文本编辑器API
$(document).ready(function () {

    $('.summernote').summernote({
        lang: 'zh-CN',
        height: 150,
        toolbar: [
              ['style', ['bold', 'italic', 'underline', 'clear']]
        ]
    });
    //数据读取绑定
    bindInfoById();

});

//富文本编辑器去空格
function checkBlankSpace(str) {

    while (str.lastIndexOf("&nbsp;") >= 0) {
        str = str.replace("&nbsp;", "");
    }

    str = str.replace(/(^\s*)|(\s*$)/g, "");//去掉前后空格

    if (str.length == 0) {
        return false;
    }

    return true;
}

//保存
function BtnSubim() {
    if (checkFormJs()) {//校验
        //取值
        var QB_Type = parseInt($("#Type").val());//题型
        var QB_Description = $('.summernote').code();//试题描述
        var QB_A = ""; var QB_B = ""; var QB_C = "";
        var QB_D = ""; var QB_E = "";
        var QB_Answer = "";
        var QB_Keyword = "";

        if (QB_Type == 1) {//单选题
            QB_A = $("#danxAtxt").val(); QB_B = $("#danxBtxt").val();
            QB_C = $("#danxCtxt").val(); QB_D = $("#danxDtxt").val();
            QB_Answer = $('input[name="danxuan"]:checked').val();
        }
        if (QB_Type == 2) {//多选题
            QB_A = $("#duoxtcbxAtxt").val(); QB_B = $("#duoxtcbxBtxt").val();
            QB_C = $("#duoxtcbxCtxt").val(); QB_D = $("#duoxtcbxDtxt").val();
            QB_E = $("#duoxtcbxEtxt").val();
            //获取复选框所有选择的值
            var chk_value = [];
            $('input[name="duoxuan"]:checked').each(function () {
                chk_value.push($(this).val());
            });
            QB_Answer = chk_value + "";

        }
        if (QB_Type == 3) {//判断题
            QB_A = '对'; QB_B = '错';
            QB_Answer = $('input[name="panduan"]:checked').val();
        }
        if (QB_Type == 4) {//填空题
            QB_Answer = $("#tiankongtiAnswer").val();//标准答案
        }
        //其它
        if (QB_Type == 5 || QB_Type == 6 || QB_Type == 7 || QB_Type == 8) {
            QB_Answer = $("#QTAnswer").val();//标准答案
            QB_Keyword = $("#QTKeyword").val();//关键字

        }
        //转义
        QB_Description = HTMLEncode(QB_Description);

        $.ajax({
            type: "POST",
            dataType: "text",
            url: '/Admin/HB_QuestionBank/Edit',
            data: {
                "QB_Type": QB_Type, "QB_Description": QB_Description,
                "QB_A": QB_A, "QB_B": QB_B, "QB_C": QB_C, "QB_D": QB_D,
                "QB_E": QB_E, "QB_Answer": QB_Answer, "QB_Keyword": QB_Keyword,
                "QuestionBId": getQueryString('QuestionBId')
            },
            success: function (data) {
                if (data == "1") {
                    layer.closeAll();//关闭所有弹出框
                    layer.msg('操作成功', { icon: 1, time: 800 }, function () {
                        window.location.href = '/Admin/HB_QuestionBank/Index';
                    });


                }
                if (data == "77") {
                    layer.msg('无权限修改本题，编辑失败！', { icon: 2 });
                    return;
                }
                if (data == "88") {
                    layer.msg('对不起，系统已存在相同题目，编辑失败！', { icon: 2 });
                    return;
                }
                if (data == "99") {
                    layer.msg('操作失败', { icon: 2 });
                    return;
                }

            }
        })

    }

}

//编辑校验
function checkFormJs() {
    var Type = parseInt($("#Type").val());

    //验证选中试题类型
    if (Type == 0) {
        layer.msg('请选择试题类型！', function () { });
        return false;
    }
    //验证输入了试题描述
    var Description = $('.summernote').code();
    if (checkBlankSpace(Description) == false) {
        layer.msg('请输入试题描述！', function () { });
        return false;
    }

    //如果是多选题
    if (Type == 2) {
        var duoxuan = document.getElementsByName('duoxuan');
        var dxcount = 0;
        for (var i = 0; i < duoxuan.length; i++) {
            if (duoxuan[i].checked) {
                dxcount++;
            }
        }

        if (dxcount == 0) {
            layer.msg('请勾选选项，进行标准答案设置！！', function () { });
            return false;
        }
    }
    //填空题
    if (Type == 4) {
        var tiankongtiAnswer = $("#tiankongtiAnswer").val();
        tiankongtiAnswer = tiankongtiAnswer.replace(/(^\s*)|(\s*$)/g, "");
        if (tiankongtiAnswer.length == 0) {
            layer.msg('请输入标准答案！', function () { });
            return false;
        }
    }
    //其他
    if (Type == 5 || Type == 6 || Type == 7 || Type == 8) {
        var QTAnswer = $("#QTAnswer").val();
        QTAnswer = QTAnswer.replace(/(^\s*)|(\s*$)/g, "");
        if (QTAnswer.length == 0) {
            layer.msg('请输入标准答案！', function () { });
            return false;
        }

    }

    return true;
}
