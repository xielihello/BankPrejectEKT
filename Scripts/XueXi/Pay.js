

var time;
var xutime;
var CurrentTime;
var RemainingTime;

var video = document.getElementById('videoPlay');

//播放结束事件
video.onended = function () {
    console.log("音频播放完成");
    clearInterval(time);
    clearInterval(xutime);
};

//播放开始事件
video.oncanplay = function () {
    console.log("音频播放开始");
    //播放总时长
    RemainingTime = video.duration.toFixed(0);
    console.log(RemainingTime);
    //是否可以续播
    var TimeInt = TimeIntervalSelect();
    //可以
    if (TimeInt > 0) {
        //播放进度加1秒后小于总时长，进入续播
        if (parseFloat(TimeInt) + 1 < parseFloat(RemainingTime)) {
            if (pd == 0) {
                console.log("之前进度:" + TimeInt);
                ContinuedBroadcasting(TimeInt);
                pd = 1;
            }
        } //不可以，插入播放进度
        else {
            TimeInterval();
        }
    }
        //不可以，插入播放进度
    else {
        TimeInterval();
    }
}


//断点续播（秒）
function ContinuedBroadcasting(Second) {
    //断点续播播放进度推后一秒
    $('video').prop('currentTime', parseFloat(Second) - 1);
    $('video').trigger('play');
    xutime = setInterval(function () {
        CurrentTime = video.currentTime.toFixed(1); //播放进度（单位：秒）
        console.log("续播进度:" + CurrentTime);
        InsetTimeInterval(CurrentTime);
    }, 1000);
}

//记录播放进度
function TimeInterval() {
    if (parseFloat(CurrentTime) > parseFloat(RemainingTime)) {
        clearInterval(time);
    }
    //轮询插入当前播放进度
    time = setInterval(function () {
        CurrentTime = video.currentTime.toFixed(1); //当前播放进度（单位：秒）
        console.log("当前进度:" + CurrentTime);
        InsetTimeInterval(CurrentTime);
    }, 1000);
}

var id = 0;
var pd = 0;
$("video").click(function () {
    if (id == 0) {
        //播放暂停
        $('video').trigger('pause');
        id = 1;
        clearInterval(time);
        clearInterval(xutime);
    } else {
        //播放继续
        $('video').trigger('play');
        id = 0;
        TimeInterval();
    }
})

//获取进度
function TimeIntervalSelect() {
    var Progress;
    $.ajax({
        url: "/XueXi/TimeIntervalSelect",
        data: { CourseId: $("#CourseId").val() },
        type: 'GET',
        async: false,
        dataType: 'json',
        success: function (data) {
            Progress = data;
        }
    });
    return Progress;
}

//插入进度
function InsetTimeInterval(CurrentTime) {
    $.ajax({
        url: "/XueXi/TimeInterval",
        data: { CourseId: $("#CourseId").val(), CurrentTime: CurrentTime },
        type: 'GET',
        async: false,
        dataType: 'json',
        success: function (data) {
        }
    });
}