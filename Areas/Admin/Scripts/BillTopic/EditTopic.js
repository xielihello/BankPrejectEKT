$(function () {
    $('.summernote').summernote({
        lang: 'zh-CN'
    });
    DataBind();
});

//加载列表
function DataBind(page) {
    var FormId = getQueryString('FormId');
    var BillType = $("#BillType").val();
    var BillName = $("#BillName").val();
    var PageSize = 10;
    $.ajax({
        type: "POST",
        dataType: "json",
        async: false,
        url: '/Admin/BillTopic/AddTopicGetList',
        data: { FormId: FormId, BillType: BillType, BillName: BillName, page: page, PageSize: PageSize },
        success: function (tb) {
            var html = '';
            var data = tb.Tb;//转换table
            for (var i = 0; i < data.length; i++) {
                html += " <tr><td><span class='pie'>" + data[i]["BillFormId"] + "</span></td>";
                html += " <td><span class='pie'>" + data[i]["Bill_Spare2"] + "</span> </td>";
                html += "  <td><span class='m-l-md'>" + data[i]["BillName"] + "</span></td>";
                html += "  <td> <a href='javascript:void(0);' onclick=\"Query('" + data[i]["BillFormId"] + "','" + data[i]["BillName"] + "')\" class='btn-primary btn-sm'><i class='fa fa-pencil m-r-xxs'></i>答案设置 </a>";
                if (data[i]["BillFormId"] == $("#FormId").val()) {
                    html += " <span style='color: red;' id='Id_" + data[i]["BillFormId"] + "'>&nbsp;&nbsp;&nbsp;&nbsp;已设置</span></td>"
                } else {
                    html += " <span style='color: red;' id='Id_" + data[i]["BillFormId"] + "'></span></td>"
                }
            }
            $("#table").html(html);
            bootstrapPaginator("#PaginatorLibrary", tb, DataBind);//分页
        }
    });
}

//查询
$("#query").click(function () {
    DataBind();
})

//单据类型
$("#BillType").change(function () {
    var BillType = $(this).val();
    $.ajax({
        type: "POST",
        // dataType: "text",
        async: false,
        url: '/Admin/BillTopic/BillNameSelect',
        data: { BillType: BillType },
        success: function (data) {
            $("#BillName").html(data);
        }
    });
});

//验证
function checkform() {
    var Summernote = $('.summernote').code();//试题描述
    $("#Summernote").val(HTMLEncode(Summernote));
    var Title = $("#Title").val();
    if (Title == "") {
        layer.msg('题目标题是必填项！', function () { });
        return false;
    }
    if (Summernote == "") {
        layer.msg('试题描述是必填项！', function () { });
        return false;
    }
    var toppd = Topic_Pd($("#Title").val())
    if (toppd > 0) {
        layer.msg('题目标题不能重复！', function () { });
        return false;
    }
    return true;
}

//答案设置
function Query(FormId, BillName) {
    var TopicId = $("#TopicId").val();
    if (checkform()) {
        var pd = Answer_Pd(TopicId);
        var FormId1 = getQueryString('FormId');
        if (FormId1 != FormId) {
            if (pd > 0) {
                layer.confirm('您已进行过表单设置，是否确认修改表单，是则清空数据可以继续设置，否不清空不能设置？', {
                    //title: '删除',
                    btn: ['是', '否'],
                    shadeClose: true, //开启遮罩关闭
                    skin: 'layui-layer-lan'
                    //按钮
                }, function () {
                    $("#FormId").val(FormId);
                    layer.open({
                        type: 2,
                        title: BillName,
                        skin: 'layui-layer-lan', //样式类名
                        shadeClose: true,
                        shade: false,
                        scrolling: 'no',
                        maxmin: true, //开启最大化最小化按钮
                        area: ['800px', '520px'],
                        content: "/Admin/BillTopic/Bills" + FormId + ""
                    });
                });
            }
        } else {
            layer.open({
                type: 2,
                title: BillName,
                skin: 'layui-layer-lan', //样式类名
                shadeClose: true,
                shade: false,
                scrolling: 'no',
                maxmin: true, //开启最大化最小化按钮
                area: ['800px', '520px'],
                content: "/Admin/BillTopic/Bills" + FormId + ""
            });
        }
    }
}

//答案判断
function Answer_Pd(TopicId) {
    var pd
    ;
    $.ajax({
        type: "POST",
        dataType: "text",
        async: false,
        url: '/Admin/BillTopic/Answer_Pd',
        data: { TopicId: TopicId },
        success: function (data) {
            pd = data;
        }
    });
    return pd;
}

//题目重复判断
function Topic_Pd(Title) {
    var data1;
    $.ajax({
        type: "POST",
        dataType: "text",
        async: false,
        url: '/Admin/BillTopic/Topic_Pd',
        data: { Title: Title, Type: "1" },
        success: function (data) {
            data1 = data;
        }
    });
    return data1;
}

function getQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) return unescape(r[2]); return null;
}