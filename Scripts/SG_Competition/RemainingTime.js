
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

            $("#time_num").html(hour + ":" + minute + ":" + second + "");

        } else {
            clearInterval(TimeOut);
            AddTime();
            window.location.href = "/SG_Competition/SG_CompetitionExamination?Exaid=" + $("#ExamId").val() + "&Pid=" + $("#PaperId").val();
        }
        intDiff--;
    }, 1000);
}

function AddTime() {
    $.ajax({
        cache: true,
        type: "POST",
        url: '/HB_kaoshi/UpDateTime',
        data: { 'Eid': $("#ExamId").val(), 'Pid': $("#PaperId").val(), 'Type': '2' },// 你的formid
        async: false,
        success: function (data) {
        }
    });
}