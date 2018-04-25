
$(function () {
    timer($("#Time").val())
});

var intDiff;//倒计时总秒数量
var TimeOut;
//倒计时
function timer(intDiff) {
    TimeOut = window.setInterval(function () {
        var day = 0,
            hour = 0,
            minute = 0,
            second = 0;//时间默认值		
        if (intDiff > 0) {
            day = Math.floor(intDiff / (60 * 60 * 24));
            hour = Math.floor(intDiff / (60 * 60)) - (day * 24);
            minute = Math.floor(intDiff / 60) - (day * 24 * 60) - (hour * 60);
            second = Math.floor(intDiff) - (day * 24 * 60 * 60) - (hour * 60 * 60) - (minute * 60);
            if (hour <= 9) hour = '0' + hour;
            if (minute <= 9) minute = '0' + minute;
            if (second <= 9) second = '0' + second;
            //$('#day_show').html(day + "天");
            //$('#hour_show').html('<s id="h"></s>' + hour + '时');
            //$('#minute_show').html('<s></s>' + minute + '分');
            //$('#second_show').html('<s></s>' + second + '秒');
            //$("#OpenId").hide();
            //$("#Submit").show();
            //$("#hall_manager").removeClass("part_one part_col_gray");
            //$("#hall_manager").addClass("part_one part_col_bule");
            //$("#imgid").hide();
            //$("#iframeId").show();
            //$('#iframeId').attr('src', 'index_detailed.aspx');//

            $("#time_num").html(hour + ":" + minute + ":" + second + "");

        } else {
            clearInterval(TimeOut);
            AddTime();
            window.location.href = "/Bill_Competition/Examination?ExamId=" + $("#ExamId").val() + "&PaperId=" + $("#PaperId").val();
        }
        intDiff--;
    }, 1000);
}

function AddTime() {
    $.ajax({
        cache: true,
        type: "POST",
        url: '/Bill_Competition/UpdaTime',
        data: { ExamId: $("#ExamId").val(), PaperId: $("#PaperId").val() },// 你的formid
        async: false,
        success: function (data) {
        }
    });
}