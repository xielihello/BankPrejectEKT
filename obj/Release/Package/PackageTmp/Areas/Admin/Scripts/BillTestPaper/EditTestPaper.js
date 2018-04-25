$(function () {
    $("#ID").val(getQueryString('ID'))
    DataBind();
    box();
})

//加载列表
function DataBind(page) {
    var BillType = $("#BillType").val();
    var BillName = $("#BillName").val();
    var CoilState = $("#CoilState").val();
    var UsingState = $("#UsingState").val();
    var Title = $("#Title").val();
    var PageSize = 10;
    $.ajax({
        type: "POST",
        dataType: "json",
        async: false,
        url: '/Admin/BillTestPaper/GetEditTestPaperlist',
        data: { Id: $("#PaperId").val(), BillType: BillType, BillName: BillName, CoilState: CoilState, UsingState: UsingState, Title: Title, page: page, PageSize: PageSize },
        success: function (tb) {
            var html = '';
            var data = tb.Tb;//转换table
            for (var i = 0; i < data.length; i++) {
                //当前页面
                var idx = 0;
                if (page != "undefined" && page != null) {
                    idx = page;
                    idx = idx - 1;
                }
                html += " <tr><td>" + ((idx * PageSize) + i + 1) + "</td>";
                html += " <td> <input type='checkbox' value='" + data[i]["ID"] + "' class='i-checks' name='inp_box'></td>";

                html += " <td><span class='pie'>" + data[i]["TopicTitle"] + "</span></td>";
                html += " <td><span class='pie'>" + data[i]["Bill_Spare"] + "</span> </td>";
                html += "  <td><span class='pie'>" + data[i]["BillName"] + "</span></td>";
                if (parseInt(data[i]["CoilState"]) > 0) {
                    html += " <td><span class='m-l-md' id='Paper1_" + data[i]["ID"] + "'>已加入当前试卷</span></td>"
                } else {
                    html += " <td><span class='m-l-md' id='Paper_" + data[i]["ID"] + "'>未加入当前试卷</span></td>"
                }
                if (parseInt(data[i]["UsingState"]) > 0) {
                    html += " <td><span class='m-l-md' id='Use1_" + data[i]["ID"] + "' style='color: red'>已使用</span></td>"
                } else {
                    html += " <td><span class='m-l-md' id='Use_" + data[i]["ID"] + "'>未使用</span></td>"
                }
                html += "<td><a href='javascript:void(0);' onclick=\"window.location.href='/Admin/BillTopic/LookTopic?ID=" + data[i]["ID"] + "&FormId=" + data[i]["BillFormId"] + "&PaperId=" + getQueryString('ID') + "&type=2'\" class=' btn-info btn-sm'><i class='fa fa-lightbulb-o m-r-xxs'></i>查看 </a></td>";
                var Score = data[i]["Score"];
                if (Score == null) {
                    Score = "";
                }
                html += "<td><input type=\"text\" placeholder=\"请输入分值\"  class=\"input_text\" id=\"Score_" + data[i]["ID"] + "\" value='" + Score + "'  name=\"Score\" autocomplete=\"off\" onkeyup='\"value=value.replace(/[^\d.]/g,'')\"' onafterpaste='\"this.value=this.value.replace(/[^\d]/g,'') \"'></td></tr>";
            }
            $("#table").html(html);
            bootstrapPaginator("#PaginatorLibrary", tb, DataBind);//分页
            box();
        }
    });
}

//查询
$("#query").click(function () {
    DataBind();
});

function box() {
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

function AddTestPaper() {
    var SumScore = 0;
    var Score = "";
    var strchks = "";
    var rad_strchks = "";
    var Number = 0;
    if ($("#PaperName").val() == "") {
        layer.msg('试卷名称为必填项！', function () { });
        return;
    }
    var chks = document.getElementsByName('inp_box');//name
    for (var i = 0; i < chks.length; i++) {
        if (chks[i].checked == true) {
            Number = parseInt(i) + 1;
            strchks += "" + chks[i].value + ",";
            if ($("#Score_" + chks[i].value + "").val() != "") {
                SumScore += parseInt($("#Score_" + chks[i].value + "").val());
                Score += $("#Score_" + chks[i].value + "").val() + ",";
            } else {
                layer.msg('请先设置分值！', function () { });
                return;
            }
        }
    }
    var rad_chks = document.getElementsByName('rad');//name
    for (var i = 0; i < rad_chks.length; i++) {
        if (rad_chks[i].checked == true) {
            rad_strchks = "" + rad_chks[i].value + "";
        }
    }
    var toppd = Paper_Pd($("#PaperName").val())
    if (toppd > 0) {
        layer.msg('试卷名称不能重复！', function () { });
        return false;
    }
    if (strchks == "") {
        layer.msg('请先勾选题目！', function () { });
        return;
    }
    if (Score == 0) {
        layer.msg('请先设置分值！', function () { });
        return;
    }
    $.ajax({
        type: "POST",
        dataType: "json",
        async: false,
        url: '/Admin/BillTestPaper/AddTestPaper_Edit',
        data: { TopicId: strchks, PaperName: $("#PaperName").val(), Sequence: rad_strchks, Score: Score, PaperId: $("#PaperId").val() },
        success: function (data) {
            if (data > 0) {
                $("#Sum_Score").html(SumScore);
                $("#Quantity").html(Number);
                for (var i = 0; i < strchks.split(',').length - 1; i++) {
                    $("#Paper_" + strchks.split(',')[i] + "").html("已加入当前试卷");
                }
                layer.msg('操作成功！', { icon: 1 });
            }
        }
    });
}

function DelTestPaper() {
    if ($("#PaperId").val() == "") {
        layer.msg('当前试卷无题目可供移除！', function () { });
        return;
    }
    var strchks = "";
    var chks = document.getElementsByName('inp_box');//name
    for (var i = 0; i < chks.length; i++) {
        if (chks[i].checked == true) {
            strchks += "" + chks[i].value + ",";
        }
    }
    $("#GLId").val($("#GLId").val() + strchks);
    for (var i = 0; i < $("#GLId").val().split(',').length - 1; i++) {
        $("#Paper1_" + $("#GLId").val().split(',')[i] + "").html("未加入当前试卷");
    }
}

function getQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) return unescape(r[2]); return null;
}


function check() {
    if ($("#PaperName").val() == "") {
        layer.msg('试卷名称为必填项！', function () { });
        return false;
    }
    var toppd = Paper_Pd($("#PaperName").val())
    if (toppd > 0) {
        layer.msg('试卷名称不能重复！', function () { });
        return false;
    }
    return true;
}

//重复判断
function Paper_Pd(PaperName) {
    var data1;
    $.ajax({
        type: "POST",
        dataType: "text",
        async: false,
        url: '/Admin/BillTestPaper/Paper_Pd',
        data: { PaperName: PaperName, Type: "1" },
        success: function (data) {
            data1 = data;
        }
    });
    return data1;
}
