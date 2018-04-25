
var UId = "0";
var Title = "";
var Type = 0
$(function () {
    UId = $("#Id").val()
    Title = $("#Title").val()
    Type = $("#Type").val()
    if (UId != "0") {
        GetModelById();
    }
})

function jsVerification(obj) {
    var money = $(obj).val(); 
    if (!IsNullOrEmpty(money)) {
        if (!isMoney(money, 0)) {
            layer.msg('请输入正确的值！', function () { });
            $(obj).focus();
            return;
        }
    }
    Calculation();
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
function Calculation() {
    /*-------------行15=行1+行2+行3+行4+行5+行6+行7+行8+行9+行14；-------------*/
    var qm1 = MoneyVerification($("#1qm").val());
    var qm2 = MoneyVerification($("#2qm").val());
    var qm3 = MoneyVerification($("#3qm").val());
    var qm4 = MoneyVerification($("#4qm").val());
    var qm5 = MoneyVerification($("#5qm").val());
    var qm6 = MoneyVerification($("#6qm").val());
    var qm7 = MoneyVerification($("#7qm").val());
    var qm8 = MoneyVerification($("#8qm").val());
    var qm9 = MoneyVerification($("#9qm").val());
    var qm14 = MoneyVerification($("#14qm").val());
    var qm15 = (parseFloat(qm1) + parseFloat(qm2) + parseFloat(qm3) + parseFloat(qm4) + parseFloat(qm5) + parseFloat(qm6) + parseFloat(qm7) + parseFloat(qm8) + parseFloat(qm9) + parseFloat(qm14)).toFixed(2);
    //alert(qm15)
    $("#15qm").val(parseFloat(qm15).toFixed(2));

    var qc1 = MoneyVerification($("#1qc").val());
    var qc2 = MoneyVerification($("#2qc").val());
    var qc3 = MoneyVerification($("#3qc").val());
    var qc4 = MoneyVerification($("#4qc").val());
    var qc5 = MoneyVerification($("#5qc").val());
    var qc6 = MoneyVerification($("#6qc").val());
    var qc7 = MoneyVerification($("#7qc").val());
    var qc8 = MoneyVerification($("#8qc").val());
    var qc9 = MoneyVerification($("#9qc").val());
    var qc14 = MoneyVerification($("#14qc").val());
    var qc15 = parseFloat(qc1) + parseFloat(qc2) + parseFloat(qc3) + parseFloat(qc4) + parseFloat(qc5) + parseFloat(qc6) + parseFloat(qc7) + parseFloat(qc8) + parseFloat(qc9) + parseFloat(qc14);
    $("#15qc").val(qc15.toFixed(2));


    /*--------------行29=行16+行17+行20+行21+行22+行23+行24+行25+行26 +行27+行28；-------------*/
    var qm16 = MoneyVerification($("#16qm").val());
    var qm17 = MoneyVerification($("#17qm").val());
    var qm20 = MoneyVerification($("#20qm").val());
    var qm21 = MoneyVerification($("#21qm").val());
    var qm22 = MoneyVerification($("#22qm").val());
    var qm23 = MoneyVerification($("#23qm").val());
    var qm24 = MoneyVerification($("#24qm").val());
    var qm25 = MoneyVerification($("#25qm").val());
    var qm26 = MoneyVerification($("#26qm").val());
    var qm27 = MoneyVerification($("#27qm").val());
    var qm28 = MoneyVerification($("#28qm").val());
    var qm29 = parseFloat(qm16) + parseFloat(qm17) + parseFloat(qm20) + parseFloat(qm21) + parseFloat(qm22) + parseFloat(qm23) + parseFloat(qm24) + parseFloat(qm25) + parseFloat(qm26) + parseFloat(qm27) + parseFloat(qm28);
    $("#29qm").val(qm29.toFixed(2));

    var qc16 = MoneyVerification($("#16qc").val());
    var qc17 = MoneyVerification($("#17qc").val());
    var qc20 = MoneyVerification($("#20qc").val());
    var qc21 = MoneyVerification($("#21qc").val());
    var qc22 = MoneyVerification($("#22qc").val());
    var qc23 = MoneyVerification($("#23qc").val());
    var qc24 = MoneyVerification($("#24qc").val());
    var qc25 = MoneyVerification($("#25qc").val());
    var qc26 = MoneyVerification($("#26qc").val());
    var qc27 = MoneyVerification($("#27qc").val());
    var qc28 = MoneyVerification($("#28qc").val());
    var qc29 = parseFloat(qc16) + parseFloat(qc17) + parseFloat(qc20) + parseFloat(qc21) + parseFloat(qc22) + parseFloat(qc23) + parseFloat(qc24) + parseFloat(qc25) + parseFloat(qc26) + parseFloat(qc27) + parseFloat(qc28);
    $("#29qc").val(qc29.toFixed(2));

    /*--------------行30=行15+行29；-------------*/
    var qm15 = MoneyVerification($("#15qm").val());
    var qm29 = MoneyVerification($("#29qm").val());
    var qm30 = parseFloat(qm15) + parseFloat(qm29);
    $("#30qm").val(qm30.toFixed(2));

    var qc15 = MoneyVerification($("#15qc").val());
    var qc29 = MoneyVerification($("#29qc").val());
    var qc30 = parseFloat(qc15) + parseFloat(qc29);
    $("#30qc").val(qc30.toFixed(2));


    /*--------------行41=行31+行32+行33+行34+行35+行36+行37+行38 +行39+行40；-------------*/
    var qm31 = MoneyVerification($("#31qm").val());
    var qm32 = MoneyVerification($("#32qm").val());
    var qm33 = MoneyVerification($("#33qm").val());
    var qm34 = MoneyVerification($("#34qm").val());
    var qm35 = MoneyVerification($("#35qm").val());
    var qm36 = MoneyVerification($("#36qm").val());
    var qm37 = MoneyVerification($("#37qm").val());
    var qm38 = MoneyVerification($("#38qm").val());
    var qm39 = MoneyVerification($("#39qm").val());
    var qm40 = MoneyVerification($("#40qm").val());
    var qm41 = parseFloat(qm31) + parseFloat(qm32) + parseFloat(qm33) + parseFloat(qm34) + parseFloat(qm35) + parseFloat(qm36) + parseFloat(qm37) + parseFloat(qm38) + parseFloat(qm39) + parseFloat(qm40);
    $("#41qm").val(qm41.toFixed(2));

    var qc31 = MoneyVerification($("#31qc").val());
    var qc32 = MoneyVerification($("#32qc").val());
    var qc33 = MoneyVerification($("#33qc").val());
    var qc34 = MoneyVerification($("#34qc").val());
    var qc35 = MoneyVerification($("#35qc").val());
    var qc36 = MoneyVerification($("#36qc").val());
    var qc37 = MoneyVerification($("#37qc").val());
    var qc38 = MoneyVerification($("#38qc").val());
    var qc39 = MoneyVerification($("#39qc").val());
    var qc40 = MoneyVerification($("#40qc").val());
    var qc41 = parseFloat(qc31) + parseFloat(qc32) + parseFloat(qc33) + parseFloat(qc34) + parseFloat(qc35) + parseFloat(qc36) + parseFloat(qc37) + parseFloat(qc38) + parseFloat(qc39) + parseFloat(qc40);
    $("#41qc").val(qc41.toFixed(2));

    /*--------------行46=行42+行43+行44+行45；-------------*/
    var qm42 = MoneyVerification($("#42qm").val());
    var qm43 = MoneyVerification($("#43qm").val());
    var qm44 = MoneyVerification($("#44qm").val());
    var qm45 = MoneyVerification($("#45qm").val());
    var qm46 = parseFloat(qm42) + parseFloat(qm43) + parseFloat(qm44) + parseFloat(qm45);
    $("#46qm").val(qm46.toFixed(2));

    var qc42 = MoneyVerification($("#42qc").val());
    var qc43 = MoneyVerification($("#43qc").val());
    var qc44 = MoneyVerification($("#44qc").val());
    var qc45 = MoneyVerification($("#45qc").val());
    var qc46 = parseFloat(qc42) + parseFloat(qc43) + parseFloat(qc44) + parseFloat(qc45);
    $("#46qc").val(qc46.toFixed(2));


    /*--------------行47=行41+行46；-------------*/
    var qm41 = MoneyVerification($("#41qm").val());
    var qm46 = MoneyVerification($("#46qm").val());
    var qm47 = parseFloat(qm41) + parseFloat(qm46);
    $("#47qm").val(qm47.toFixed(2));


    var qc41 = MoneyVerification($("#41qc").val());
    var qc46 = MoneyVerification($("#46qc").val());
    var qc47 = parseFloat(qc41) + parseFloat(qc46);
    $("#47qc").val(qc47.toFixed(2));

    /*--------------行52=行48+行49+行50+行51；-------------*/
    var qm48 = MoneyVerification($("#48qm").val());
    var qm49 = MoneyVerification($("#49qm").val());
    var qm50 = MoneyVerification($("#50qm").val());
    var qm51 = MoneyVerification($("#51qm").val());
    var qm52 = parseFloat(qm48) + parseFloat(qm49) + parseFloat(qm50) + parseFloat(qm51);
    $("#52qm").val(qm52.toFixed(2));

    var qc48 = MoneyVerification($("#48qc").val());
    var qc49 = MoneyVerification($("#49qc").val());
    var qc50 = MoneyVerification($("#50qc").val());
    var qc51 = MoneyVerification($("#51qc").val());
    var qc52 = parseFloat(qc48) + parseFloat(qc49) + parseFloat(qc50) + parseFloat(qc51);
    $("#52qc").val(qc52.toFixed(2));

    /*--------------行53=行47+行52=行30；-------------*/
    var qm47 = MoneyVerification($("#47qm").val());
    var qm52 = MoneyVerification($("#52qm").val());
    var qm53 = parseFloat(qm47) + parseFloat(qm52);
    $("#53qm").val(qm53.toFixed(2));


    var qc47 = MoneyVerification($("#47qc").val());
    var qc52 = MoneyVerification($("#52qc").val());
    var qc53 = parseFloat(qc47) + parseFloat(qc52);
    $("#53qc").val(qc53.toFixed(2));
}


//返回值
function jsvalues(obj) {
    var v;
    var value = $(obj).val()
    if (!IsNullOrEmpty(value)) {
        v = value;
    }
    return v;
}
//判断是否金额类型
function jsIsture(obj) {
    var v;
    var value = $(obj).val()
    if (!IsNullOrEmpty(value)) {
        if (!isMoney(value, 0)) {
            $(obj).focus();
            return true;
        }
    }
    return false;
}


function jsReturn() {
    //window.location.href = "/Admin/FH_ReportForm/Topic";
    var IsType = $("#IsType").val();
    if (IsType == "0") {
        window.history.go(-1);
    } else {
        window.close();
    }
}
//新增保存
function BtnSubim() {
    var Title = $("#EditTitle").val();

    if (IsNullOrEmpty(Title)) {
        layer.msg('请输入题目标题！', function () { });
        $("#EditTitle").focus();
        return;
    }
    var boo = false;
    var list = [];
    var line30_Final = 0;
    var line30_Beginning = 0;

    var line53_Final = 0;
    var line53_Beginning = 0;


    var line9_Final = 0;
    var line9_Beginning = 0;

    var line99_Final = 0;
    var line99_Beginning = 0;
    var isnull = true;
    $("#tb_tbody").find("tr").each(function () {
        var Istrue = $(this).find("td").eq(0).attr("class");
        if (!IsNullOrEmpty(Istrue) && Istrue.indexOf("true") >= 0) {
            var m = {};
            m.Line = $(this).find("td").eq(0).attr("data-lind")//行
            if (jsIsture($(this).find("#" + m.Line + "qm")) || jsIsture($(this).find("#" + m.Line + "qc"))) {
                boo = true;
                return;
            }
            m.Final = jsvalues($(this).find("#" + m.Line + "qm"));//期末
            m.Beginning = jsvalues($(this).find("#" + m.Line + "qc"));//期初
            
            if (m.Line != 15 && m.Line != 29 && m.Line != 30) {
                if (!IsNullOrEmpty(m.Final) || !IsNullOrEmpty(m.Beginning)) {
                    isnull = false;
                }
            }
            if (m.Line == 10 || m.Line == 11 || m.Line == 12 || m.Line == 13) {
                line99_Final += parseFloat(MoneyVerification(m.Final));
                line99_Beginning += parseFloat(MoneyVerification(m.Beginning));
            }
            if (m.Line == 9) {
                line9_Final = MoneyVerification(m.Final);
                line9_Beginning = MoneyVerification(m.Beginning);
            }
            if (m.Line == 30) {
                line30_Final = MoneyVerification(m.Final);
                line30_Beginning = MoneyVerification(m.Beginning);
            }
            m.Assets = $(this).find("td").eq(0).find("label").html();
            m.Type = 0;
            list.push(m);


        }
        Istrue = $(this).find("td").eq(4).attr("class");
        if (!IsNullOrEmpty(Istrue) && Istrue.indexOf("true") >= 0) {
            var m = {};
            m.Line = $(this).find("td").eq(4).attr("data-lind")//行
            if (jsIsture($(this).find("#" + m.Line + "qm")) || jsIsture($(this).find("#" + m.Line + "qc"))) {
                boo = true;
                return;
            }
            m.Final = jsvalues($(this).find("#" + m.Line + "qm"));//期末
            m.Beginning = jsvalues($(this).find("#" + m.Line + "qc"));//期初
            if (m.Line != 41 && m.Line != 46 && m.Line != 47 && m.Line != 52 && m.Line != 53) {
                if (!IsNullOrEmpty(m.Final) || !IsNullOrEmpty(m.Beginning)) {
                    isnull = false;
                }
            }
            if (m.Line == 53) {
                line53_Final = MoneyVerification(m.Final);

                line53_Beginning = MoneyVerification(m.Beginning);
            }
            m.Assets = $(this).find("td").eq(4).find("label").html();
            m.Type = 1;
            list.push(m);


        }
    });
   
    if (isnull) {
        layer.msg('请填写题目内容！', function () { });
        return;
    }
    line9_Final = parseFloat(line9_Final).toFixed(2)
    line99_Final = parseFloat(line99_Final).toFixed(2)
    line9_Beginning = parseFloat(line9_Beginning).toFixed(2)
    line99_Beginning = parseFloat(line99_Beginning).toFixed(2)
    if (parseFloat(line9_Final) < parseFloat(line99_Final)) {
        layer.msg('行9必须大于等于（行10 + 行11 + 行12 + 行13）！', function () { });
        return;
    }
    if (parseFloat(line9_Beginning) < parseFloat(line99_Beginning)) {
        layer.msg('行9必须大于等于（行10 + 行11 + 行12 + 行13）！', function () { });
        return;

    }
    //return;
    if (line30_Final != line53_Final || line30_Beginning != line53_Beginning) {
        layer.msg('资产总计必须等于负债和所有者权益(或股东权益)总计！', function () { });
        return;
    }
    if (boo) {
        layer.msg('请填写正确的值！', function () { });
        return;
    }
    var tb = list;


    //表单提交
    $.ajax({
        type: "post",
        url: '/Admin/FH_ReportForm/Edit',
        data: { entityList: JSON.stringify(list), Title: Title, Id: UId },
        success: function (data) {
            if (data == 0) {
                jsReturn();
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


//读取当前
function GetModelById() {
    //return;
    $.ajax({
        type: "post",
        url: '/Admin/FH_ReportForm/GetModelById',
        data: { Id: UId },
        success: function (data) {
            if (Type == 0) {
                $("#EditTitle").val(Title);
                if (!IsNullOrEmpty(data)) {
                    var obj = JSON.parse(data)
                    if (obj.length > 0) {
                        for (var i = 0; i < obj.length; i++) {
                            var Line = obj[i]["Line"];
                            var FinalBalance = obj[i]["FinalBalance"];//期末余额
                            var BeginningBalance = obj[i]["BeginningBalance"];//期初余额
                            $("#tb_tbody").find("#" + Line + "qm").val(FinalBalance);
                            $("#tb_tbody").find("#" + Line + "qc").val(BeginningBalance);
                        }
                    }

                }
            } else {
                if (!IsNullOrEmpty(data)) {
                    var obj = JSON.parse(data)
                    if (obj.length > 0) {
                        var html = '<tr><td><label class="control-label ">题目标题：</label></td></tr>'
                        html += '<tr><td><label class="col-sm-22">' + Title + '</label></td></tr>';
                        html += '<tr><td><label class="control-label">题目信息：</label></td></tr>';
                        var sis = 1;
                        for (var i = 0; i < obj.length; i++) {
                            var Line = obj[i]["Line"];
                            var FinalBalance = obj[i]["FinalBalance"];//期末余额
                            var BeginningBalance = obj[i]["BeginningBalance"];//期初余额
                            $("#tb_tbodys").find("#" + Line + "qm").val(FinalBalance);
                            $("#tb_tbodys").find("#" + Line + "qc").val(BeginningBalance);
                            if (Line != 15 && Line != 29 && Line != 30 && Line != 41 && Line != 46 && Line != 47 && Line != 52 && Line != 53) {
                                if ((MoneyVerification(FinalBalance) + MoneyVerification(BeginningBalance)) > 0) {
                                    html += '<tr>';
                                    html += '<td>';
                                    html += '<div><label class="col-sm-22">' + (parseInt(sis) + 1) + '.' + obj[i]["Assets"] + '</label></div>';
                                    html += '<div><label class="col-sm-22">期初余额：' + (BeginningBalance == null ? "" : BeginningBalance) + ';</label></div>';
                                    html += '<div><label class="col-sm-22">期末余额：' + (FinalBalance == null ? "" : FinalBalance) + ';</label></div>';
                                    html += '</td> ';
                                    html += '</tr>';
                                    sis++;
                                }
                            }

                        }
                        $("#tbTitle").html(html);
                    }

                }
            }
        }
    });
}
