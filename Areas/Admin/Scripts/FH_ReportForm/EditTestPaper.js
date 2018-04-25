var Id = null
var Type = 1;
var checkszu = [];

var list = [];
$(function () {
    Id = $("#Id").val();
    Type = $("#Type").val();
   
    if (Type == 1) {
        //手工组卷
        bindIngfo(1);
       
    } else {
        //智能组卷
        $('#Score').bind('input propertychange', function () {
            $("#Sum_Score").html(($("#SumExtract").val() * $("#Score").val()).toFixed(2))
        });
    }
    box();
})

//手工组卷列表数据加载
function bindIngfo(page) {
    var Title = $("#Title").val();//题目名称 
    var State = $("#State").find("option:selected").val();//
    var PaperIsUse = $("#PaperIsUse").find("option:selected").val();//  
    var PageSize = 10;
    $.ajax({
        Type: "post",
        url: '/Admin/FH_ReportForm/GetTestPaperByListId',
        dataType: "json", cache: false,
        contentType: "application/json; charset=utf-8",
        data: { "Title": Title, "State": State, "PaperIsUse": PaperIsUse, "Id": Id, "page": page, "PageSize": PageSize },
        success: function (data) {
            var tb = data;
            if (tb != null && tb.Total > 0) {
                var html = "";
                var obj = tb.Tb;
                var total = 0;
                var totalFr = 0;
                if (obj.length > 0) {
                    for (var i = 0; i < obj.length; i++) {//rownum
                         
                       

                        var Kind = obj[i]["Kind"];
                        var AddOperator = obj[i]["AddOperator"];
                        var State = obj[i]["State"];
                        var PaperIsUse = obj[i]["PaperIsUse"];
                        var checks = ""

                        var TestsFraction = obj[i]["TestsFraction"]

                        var bo = true;
                        if (list != null && list.length > 0) {
                            for (var s = 0; s < list.length; s++) {
                                if (parseInt(list[s].Id) == obj[i]["Id"]) {

                                    if (list[s].type != 0) {//移除
                                        State = 0;
                                        PaperIsUse = parseInt(PaperIsUse) - 1
                                        TestsFraction = 0;
                                    } else {
                                        State = 1;
                                        PaperIsUse = parseInt(PaperIsUse) + 1;
                                        TestsFraction = list[s].Score;
                                    }
                                    bo = false;
                                }
                            }
                        }
                        //if (bo) {
                        //    if (parseInt(State) > 0) {
                        //        var m = {};
                        //        m.Id = obj[i]["Id"]
                        //        m.Score = TestsFraction
                        //        m.type = 0;
                        //        list.push(m);
                        //    }
                        //}
                        //alert(PaperIsUse);
                        if (parseInt(State) > 0) {
                            checks = "checked";
                            total++;
                        } 
                        if (IsNullOrEmpty(TestsFraction)) {
                            TestsFraction = "";
                        } else {
                            totalFr += parseFloat(TestsFraction); 
                        }

                        

                        html += '<tr class="boo" Id="tr_' + obj[i]["Id"] + '">';
                        html += '<td  style="width:50px;">' + obj[i]["rownum"] + '</td>';
                        html += '<td>';
                        html += '<input type="checkbox" data-State="' + State + '" data-PaperIsUse="' + PaperIsUse + '" data-Score="' + (TestsFraction == "" ? 0 : TestsFraction) + '"  class="i-checks" value="' + obj[i]["Id"] + '" name="input[]">';
                        html += '</td>';
                        html += '<td><span class="pie">' + obj[i]["Title"] + '</span>';
                        html += '</td>';
                        html += '<td><span class="pie isstate" >' + (parseInt(State) > 0?"已加入当前试卷":"未加入当前试卷") + '</span>';
                        html += '</td>';
                        html += '<td><span class="pie ispagerisuse"  ' + (parseInt(PaperIsUse) > 0 ?"style='color: red;'":"") + '>' + (parseInt(PaperIsUse) > 0 ? "已使用" : "未使用") + '</span>';
                        html += '</td>';
                        html += '<td>'; 
                        html += "<a href=\"javascript:;\" onclick=\"jshref(" + obj[i]["Id"] + ",'" + obj[i]["Title"] + "')\" class=\"  btn-info btn-sm\"><i class=\"fa fa-lightbulb-o m-r-xxs\"></i>查看 </a>";
                        html += '</td>';
                        html += ' <td>';
                        html += "<input type=\"text\" placeholder=\"请输入分值\"  maxlength=\"11\" data-score=\"" + TestsFraction + "\" value=\"" + TestsFraction + "\" class=\"input_text\" name=\"Score\"  oninput=\"jsVerification(this)\">";
                        html += '</td>';
                        html += '</tr>';
                    }
                }
                $("#tablelist").html(html);
                //$("#Sum_Score").html(totalFr)
                //$("#Quantity").html(total)
            } else {
                $("#tablelist").html("<tr><td colspan='10'></td></tr>");
            }
            bootstrapPaginator("#PaginatorLibrary", tb, bindIngfo)//分页

            box();
            //$("input[name='input[]']").on('ifChecked', function (event) {
            //    alert($(this).val());
            //});

            //checkszu = [];
            //$(".iCheck-helper").click(function () {
            //    debugger;
            //    if ($(this).parent().find("input")[0].checked) { //勾选保存
            //        var m = {};
            //        m.Id = $(this).parent().find("input").val()
            //        m.Score = $(this).parent("tr").find("input[name='Score']").find("input").val()
            //        checkszu.push(m)
            //    } else {                                           //反勾选去除
            //        var v = $(this).parent().find("input").val()
            //        if (checkszu.length > 0) {
            //            for (var i = 0; i < checkszu.length; i++) {
            //                if (checkszu[i].Id == v)
            //                {
            //                    checkszu.splice(i, 1)
            //                    return false;
            //                }
            //            }
            //        }
            //    }
            //})

        }
    });

}

//判断是否金额类型
function jsVerification(obj) {
    var money = $(obj).val();

    if (!IsNullOrEmpty(money)) {
        if (!isMoney(money, 0)) {
            layer.msg('请输入正确的分值！', function () { });
            $(obj).focus();
            return true;
        } 
    }
}
//加入试卷、移除试卷
//type==1?移除 ： 添加
function AjaxCacheTest(type) {

    var model = [];
    var checkszu = [];
    var mi = 0;
    var boo = false;
    $("#tablelist").find("tr.boo").each(function () {
        if ($(this).find("input[name='input[]']")[0].checked) {
            var m = {};
            m.Id = $(this).find("input[name='input[]']").val()
            m.Score = $(this).find("input[name='Score']").val()
            m.type = type;
            if (type == 0) {//移除状态不判断。
                if (IsNullOrEmpty(m.Score)) {
                    layer.msg('请输入分值！', function () { });
                    $(this).find("input[name='Score']").focus();
                    boo = true;
                    return false;

                }
                if (jsVerification($(this).find("input[name='Score']"))) {
                    boo = true;
                    return false;
                }
                //$(this).find("input[name='Score']").attr("data-Score", m.Score);
            }

            list.push(m);
            checkszu.push(m);
            model[mi] = this;
            mi++;
        }
    });
    if (boo) {
        return;
    }
    if (checkszu.length <= 0) {
        layer.msg('请勾选你要加入的题目！', function () { });
        return;
    }

    var toScore = 0;
    var len = 0;
    for (var i = 0; i < model.length; i++) {
        var Score = 0;
        var State = $(model[i]).find("input[type='checkbox']").attr("data-State")//>0已加入当前试卷
        var PaperIsUse = $(model[i]).find("input[type='checkbox']").attr("data-PaperIsUse")//>0已使用
        if (type == 0) {
            $(model[i]).find("td").eq(3).find("span").html("已加入当前试卷")
            $(model[i]).find("td").eq(4).find("span").html("已使用")
            $(model[i]).find("td").eq(4).find("span").css("color", "red");

           var Scorels = $(model[i]).find("td").eq(6).find("input").val() 
           var data_sc = $(model[i]).find("td").eq(6).find("input").attr("data-Score");
            $(model[i]).find("input[type='checkbox']").attr("data-State", "1");
            if (parseInt(State) > 0) {
                Score = parseFloat(Scorels) - parseFloat(data_sc); 
            } else {
                Score = Scorels;
                len++; 
            }
            PaperIsUse = parseInt(PaperIsUse) + 1;
            $(model[i]).find("td").eq(6).find("input").attr("data-Score", Scorels)
        } else {
            $(model[i]).find("td").eq(3).find("span").html("未加入当前试卷")
            PaperIsUse = parseInt(PaperIsUse) - 1;
            if (parseInt(PaperIsUse) < 1) {
                $(model[i]).find("td").eq(4).find("span").html("未使用")
            }
            $(model[i]).find("td").eq(4).find("span").css("color", "#676a6c");
            Score = $(model[i]).find("td").eq(6).find("input").attr("data-Score");
            $(model[i]).find("td").eq(6).find("input").val("");
            $(model[i]).find("input[type='checkbox']").attr("data-State", "0")
            if (parseInt(State) > 0) {
                len++;

            } else {
                Score = 0;
            }
            
        }
        $(model[i]).find("input[type='checkbox']")[0].checked = false;
        $(model[i]).find("input[type='checkbox']").parent().removeClass("checked");
        toScore += parseFloat(Score);

        //alert(list.length);

    }

    var totalSo = $("#Sum_Score").html();
    var mleng = $("#Quantity").html();
    if (type == 0) {
        $("#Sum_Score").html(parseFloat(parseFloat(totalSo) + toScore).toFixed(2))
        $("#Quantity").html(parseInt(mleng) + len)
    } else {
        $("#Sum_Score").html(parseFloat(parseFloat(totalSo) - toScore).toFixed(2))

        $("#Quantity").html(parseInt(mleng) - len)
    }
    layer.msg('操作成功', { icon: 1 });

}

function MoneyVerification(money) {
    var velue = 0;
    if (!IsNullOrEmpty(money)) {
        if (isMoney(money, 0)) {
            velue = money;
        }
    }
    return velue;
}
function AjaxAddTestPager() {
    var TestName = $("#TestName").val();
    var IsOrder = $("#radio").find('input[name="IsOrder"]:checked').val();
    if (IsNullOrEmpty(TestName)) {
        layer.msg('请输入试卷名称！', function () { });
        $("#TestName").focus();
        return
    }

   
    var Quantity = $("#Quantity").html()
    if (MoneyVerification(Quantity) <= 0) {
        layer.msg('请先加入题目！', function () { }); 
        return
    }
    $.ajax({
        type: "post",
        url: '/Admin/FH_ReportForm/AjaxAddTestPager',
        data: { entityList: JSON.stringify(list), TestName: TestName, IsOrder: IsOrder, Id: Id },
        success: function (data) {
            if (data == 1) {
                jsReturn();
            } else if (data == -2) {
                layer.msg('已存在相同试卷名称！', { icon: 2 });
                return;
            } else {
                layer.msg('操作失败', { icon: 2 });
                return;
            }

        }
    });

}

//手工组卷列表加入试卷
function GetUpdateTestPager(type,State) {

    var TestName = $("#TestName").val();
    var IsOrder = $('table input[name="input[]"]:checked').val();
    if (IsNullOrEmpty(TestName)) {
        layer.msg('请输入试卷名称！', function () { });
        $("#TestName").focus();
        return
    }
    checkszu = [];
    var model = [];
    var mi=0;
    if (State == "A") {
        var boo = false;
        $("#tablelist").find("tr.boo").each(function () {
            if ($(this).find("input[name='input[]']")[0].checked) {
                var m = {};
                m.Id = $(this).find("input[name='input[]']").val()
                m.Score = $(this).find("input[name='Score']").val()
                if (type == 1) {//移除状态不判断。
                    if (IsNullOrEmpty(m.Score)) {
                        layer.msg('请输入分值！', function () { });
                        $(this).find("input[name='Score']").focus();
                        boo = true;
                        return false;

                    }
                    if (jsVerification($(this).find("input[name='Score']"))) {
                        boo = true;
                        return false;
                    }
                }
                checkszu.push(m);
                model[mi] = this;
                mi++;
            }
        })
        if (boo) {
            return;
        }
        if (checkszu.length <= 0) {
            layer.msg('请勾选你要加入的题目！', function () { });
            return;
        }
    }
    else { 
        if (IsNullOrEmpty(Id)) {
            layer.msg('请先选择题目加入试题！', function () { });
            return;
        }
    }
    if (IsOrder == "" || IsOrder == undefined) {
        layer.msg('请勾选你要加入的题目！', function () { });
        return;
    }
    
    
    //表单提交
    $.ajax({
        type: "post",
        url: '/Admin/FH_ReportForm/UpdateTestPager',
        data: { entityList: JSON.stringify(checkszu), TestName: TestName, IsOrder: IsOrder, type: type, State: State, Id: Id },
        success: function (data) {
          //  debugger;
            if (parseInt(data) > 0) {
                if (State == "B") {
                    jsReturn();
                } else {
                    Id = data;
                    if (model != null && model.length > 0) {
                        var toScore = 0;
                        for (var i = 0; i < model.length; i++) {
                            $(model[i]).find("td").eq(3).find("span").html("已加入当前试卷")
                            $(model[i]).find("td").eq(4).find("span").html("已使用")
                            $(model[i]).find("td").eq(4).find("span").css("color", "red");
                            var Score = $(model[i]).find("td").eq(6).find("input").val();
                            toScore += parseFloat(Score);
                        }
                        $("#Sum_Score").html(toScore)
                        $("#Quantity").html(model.length)
                    }
                      //bindIngfo(1);

                }
                layer.msg('操作成功', { icon: 1 });
            }
            else if (data == -2) {
                layer.msg('题目名称存在相同的记录', { icon: 2 });
                $("#EditTitle").focus();
                return;

            } else {
                layer.msg('操作失败', { icon: 2 });
                return;
            }
        }
    });
    
}

 
//智能组卷保存
function Save() {
    var Sum_Number = $("#Sum_Number").html();
    var Unused = $("#Unused").html();
    var PaperName = $("#PaperName").val();
    var SumExtract = $("#SumExtract").val();//随机抽取总数
    var ExtractUnused = $("#ExtractUnused").val();//随机抽取未使用
    var Score = $("#Score").val();
    if (PaperName == "") {
        layer.msg('试卷名称为必填项！', function () { });
        $("#PaperName").focus();
        return;
    }
    if (SumExtract == "") {
        layer.msg('共抽取题目数量为必填项！', function () { });
        $("#SumExtract").focus();
        return;
    }

    if (Score == "") {
        layer.msg('分数为必填项！', function () { });
        $("#Score").focus();
        return;
    }
    if (parseInt(SumExtract) > parseInt(Sum_Number)) {
        layer.msg('抽取总数不得大于可用总数！', function () { });
        $("#SumExtract").focus();
        return;
    }
    if (ExtractUnused == "") {
        ExtractUnused = 0;
    }
    if (parseInt(ExtractUnused) > parseInt(Sum_Number)) {
        layer.msg('抽取未被使用数不得大于可用总数！', function () { });
        $("#ExtractUnused").focus();
        return;
    }
    if (parseInt(ExtractUnused) > parseInt(Unused)) {
        layer.msg('抽取未被使用数不得大于当前未被使用数！', function () { });
        $("#ExtractUnused").focus();
        return;
    }
    if (parseInt(ExtractUnused) > parseInt(SumExtract)) {
        layer.msg('抽取未被使用数不得大于抽取总数！', function () { });
        $("#ExtractUnused").focus();
        return;
    }
    var rad_strchks = "";
    var rad_chks = document.getElementsByName('rad');//name
    for (var i = 0; i < rad_chks.length; i++) {
        if (rad_chks[i].checked == true) {
            rad_strchks = "" + rad_chks[i].value + "";
        }
    }

    $.ajax({
        type: "POST",
        dataType: "json",
        async: false,
        url: '/Admin/FH_ReportForm/SaveTestPaper',
        data: { PaperName: PaperName, SumExtract: SumExtract, ExtractUnused: ExtractUnused, Score: Score, Sequence: rad_strchks },
        success: function (data) {
            if (data == -1) {
                layer.msg('操作失败，随机抽取设置错误！', { icon: 2 });
                return;
            } if (data == -2) {
                layer.msg('已存在相同的试卷名称！', { icon: 2 });
                return;
            }
            if (data != 1) {
                window.location.href = "/Admin/FH_ReportForm/TestPaper";
            } else {
                layer.msg('操作失败！', { icon: 2 });
            }
        }
    });
}





function jsReturn() {
    //window.location.href = "/Admin/FH_ReportForm/TestPaper";
    window.history.go(-1);
}

function jshref(id, title) {
    window.open("/Admin/FH_ReportForm/EditsTopic?Type=1&Id=" + id + "&Title=" + title);
}








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