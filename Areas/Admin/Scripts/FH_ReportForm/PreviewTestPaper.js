
//$(function () {
//    GetModelById(1);
//})




function MoneyVerification(money) {
    var velue = 0;
    if (!IsNullOrEmpty(money)) {
        if (isMoney(money, 0)) {
            velue = money;
        }
    }
    return velue;
}

function GetModelById(Id,Title,Fraction) {
    $("#tim").html(Title + " (" + Fraction + ")");
    $.ajax({
        type: "post",
        url: '/Admin/FH_ReportForm/GetModelById',
        data: { Id: Id },
        success: function (data) {

            if (!IsNullOrEmpty(data)) {
                var obj = JSON.parse(data)
                if (obj.length > 0) {
                    var html = ''
                    var sis = 1;
                    for (var i = 0; i < obj.length; i++) {
                        var Line = obj[i]["Line"];
                        var FinalBalance = obj[i]["FinalBalance"];//期末余额
                        var BeginningBalance = obj[i]["BeginningBalance"];//期初余额
                        $("#tb_tbodys").find("#" + Line + "qm").val(FinalBalance);
                        $("#tb_tbodys").find("#" + Line + "qc").val(BeginningBalance);
                        if (Line != 15 && Line != 29 && Line != 30 && Line != 41 && Line != 46 && Line != 47 && Line != 52 && Line != 53) {
                            if ((MoneyVerification(FinalBalance) + MoneyVerification(BeginningBalance)) > 0) {
                             
                                html += (parseInt(sis) + 1) + '.' + obj[i]["Assets"] + '<br /> ';
                                html += '期初余额：' + (BeginningBalance == null ? "" : BeginningBalance) + ';<br />';
                                html += '期末余额：' + (FinalBalance == null ? "" : FinalBalance) + ';<br />';
                                sis++;
                            }
                        }

                    }
                    $("#divtim").html(html);
                }

            }
        }
    });
}


























function jsReturn() {
    //window.location.href = "/Admin/FH_ReportForm/TestPaper";
    window.history.go(-1);
}