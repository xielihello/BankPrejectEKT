/***************************************************************
  FileName:手工点钞-裁判扣分javascript
  Copyright（c）2017-金融教育在线技术开发部
  Author:邵世铨
  Create Date:2017-4-13
 ******************************************************************/

//裁判扣分
function Points()
{
    layer.open({
        title: '裁判员操作',
        type: 1,
        skin: 'layui-layer-lan', //样式类名
        anim: 2,
        //shadeClose: true, //开启遮罩关闭
        area: ['370px', '200px'], //宽高
        content: $("#classAdd")
    });
}

// JS 获取URL参数方法
function getQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) return unescape(r[2]); return null;
}

function Save()
{
    var AnswerId = getQueryString("ResponseState");
    var UserNo = $("#UserNo").val();
    if(UserNo==""||UserNo==null)
    {
        layer.msg('用户名不能为空', function () {
        });
        return false;
    }
    var Pwdid = $("#Pwdid").val();
    if (Pwdid == "" || Pwdid == null) {
        layer.msg('密码不能为空', function () {
        });
        return false;
    }
    var Score = $("#Scoreid").val();
    if (Score == "" || Score == null) {
        layer.msg('扣分值不能为空', function () {
        });
        return false;
    }
    $.ajax({
        type: "POST",
        dataType: "text",
        url: '/SG_GradLook/Save',
        data: { "AnswerId": AnswerId, "UserNo": UserNo, "Pwdid": Pwdid, "Score": Score },
        success: function (data) {
            layer.msg('' + data + '', function () {
            });
            window.location.href = '/SG_GradLook/GradLookShow?AnswerId=' + AnswerId + '';
        }
    });
}

function TYcheckeNum(Id) {
    var dqnum = $("#" + Id).val();
    if (isNaN(dqnum)) {
        $("#" + Id).val('');

    }
    var patrn = /^([1-9]\d{0,9}|0)([.]?|(\.\d{1,2})?)$/;
    if (!patrn.test(dqnum)) {
        $("#" + Id).val('');

    }
}