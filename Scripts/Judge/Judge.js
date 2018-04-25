/***************************************************************
  FileName:打裁判-裁判扣分javascript
  Copyright（c）2017-金融教育在线技术开发部
  Author:邵世铨
  Create Date:2017-4-13
 ******************************************************************/

function Save()
{
    var Points = $("#Pointsid").val();
    $.ajax({
        type: "POST",
        dataType: "text",
        url: '/Judge/Save',
        data: { "Points": Points},
        success: function (data) {
            layer.msg('' + data + '', function () {
            });
            window.location.href = '/SG_Competition';
        }
    });
}

function CloseIndex() {
    //parent.layerConfirm('确认要退出后台?', function (num) { $("form[name='myUserExit']").submit() });
    //layer.msg('确认要退出后台！', function (num) { $("form[name='myUserExit']").submit() });
    layer.confirm('确认要退出后台？', {
        title: '退出',
        btn: ['确定', '取消'],
        shadeClose: true, //开启遮罩关闭
        skin: 'layui-layer-lan'
        //按钮
    },
function () {
    $.ajax({
        type: "POST",
        dataType: "text",
        url: '/Judge/Out',
        data: { "Points": 0 },
        success: function (data) {
            window.location.href = '/login';
        }
    });
});

}