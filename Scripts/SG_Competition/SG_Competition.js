/***************************************************************
  FileName:手工点钞竞赛列表 javascript
  Copyright（c）2017-金融教育在线技术开发部
  Author:邵世铨
  Create Date:2017-4-12
 ******************************************************************/

//竞赛列表切换
$(function () {
    setTab('one', 1)
    //$("li:first").removeClass("active");//移除掉所有的选中样式
    //$(".sgdc").addClass("active");
    bindIngfo();
    bindIngfoTo();
    setInterval("bindIngfo()", 10000);
    setInterval("bindIngfoTo()", 10000);
    //RemainingTime();
})
//切换标签
function setTab(name, cursel) {
    //竞赛任务
    if (cursel == 1) {
        $("#" + name + "2").removeClass("off");
        $("#" + name + "1").addClass("off");
        $("#con_one_1").show();
        $("#con_one_2").hide();
        $("#ChartDiv").hide();
        setInterval("bindIngfo()", 10000);
    } else {
        $("#" + name + "1").removeClass("off");
        $("#" + name + "2").addClass("off");
        $("#con_one_2").show();
        $("#con_one_1").hide();
       // $("#ChartDiv").show();
    }
}


//进行竞赛搜索
function searchinfo() {
    bindIngfo();
}

//已完成进行竞赛搜索
function searchinfoTo() {
    bindIngfoTo();
}

//未进行竞赛列表数据加载
function bindIngfo(page) {
    var E_Type = $("#E_Type").val();//类型
    var E_Name = $("#E_Name").val();//竞赛名称

    var PageSize = 10;

    $.ajax({
        Type: "post",
        dataType: "json",
        cache: false,
        contentType: "application/json; charset=utf-8",
        url: '/SG_Competition/GetList',
        data: { "E_Name": E_Name, "E_Type": E_Type, "page": page, "PageSize": PageSize },
        success: function (data) {
            var tb = data;
            if (tb != null && tb.TableHTML.length > 0) {
                $("#tablelist").html(tb.TableHTML);

            } else {
                $("#tablelist").html("");
            }
            bootstrapPaginator("#PaginatorLibrary1", tb, bindIngfo);//分页
        }
    });
}

//已完成竞赛列表数据加载
function bindIngfoTo(page) {
    debugger;
    var E_TypeTo = $("#E_TypeTo").val();//类型
    var E_NameTo = $("#E_NameTo").val();//竞赛名称

    var PageSize = 10;

    $.ajax({
        Type: "post",
        dataType: "json",
        cache: false,
        contentType: "application/json; charset=utf-8",
        url: '/SG_Competition/GetListTo',
        data: { "E_NameTo": E_NameTo, "E_TypeTo": E_TypeTo, "page": page, "PageSize": PageSize },
        success: function (data) {
            var tb = data;
            if (tb != null && tb.TableHTML.length > 0) {
                $("#tablelistTo").html(tb.TableHTML);

            } else {
                $("#tablelistTo").html("");
            }
            bootstrapPaginator("#PaginatorLibrary2", tb, bindIngfoTo);//分页
        }
    });
}

var datetday = 7;//最近多少个工作日
//成长轨迹
function ViewGrowthTrajectory(Eid) {
    $("#ChartDiv").show();//点击考试名称 显示成长轨迹
    var myDate = new Date(); //获取今天日期
    myDate.setDate(myDate.getDate() - datetday);
    var dateArray = [];
    var dateTemp;
    var flag = 1;
    for (var i = 0; i < datetday; i++) {
        dateTemp = myDate.getFullYear() + "-" + (myDate.getMonth() + 1) + "-" + myDate.getDate();
        dateArray.push(dateTemp);
        myDate.setDate(myDate.getDate() + flag);
    }
    //得到前7个工作日
    var datet = dateArray + "";
    //通过ajax请求 获取前7工作日的每天平均练习情况
    var listscore = "";
    var EName = "";//精神名称
    $.ajax({
        type: "POST",
        dataType: "json",
        url: '/SG_Competition/GeViewGrowthTrajectory',
        data: { "Eid": Eid, "datet": datet },
        async: false,
        success: function (data) {
            listscore = data[0]["scoresstr"];
            EName = data[0]["EName"];
        }
    });

    var scoreArray = [];//成绩分数素组

    var splitscore = listscore.split(',');
    for (var i = 0; i < splitscore.length; i++) {
        if (splitscore[i].length > 0) {
            scoreArray.push(splitscore[i]);
        }
    }

    //标准面积图，填充样式，平滑曲线
    var myChart = echarts.init(document.getElementById('chart-personPK'));

    // 标准面积图，填充样式，平滑曲线
    var option = {
        title: {
            text: '成长轨迹',
            subtext: '综合能力指数(分值)'
        },
        tooltip: {
            color: 'green',
            trigger: 'axis'
        },

        legend: {
            data: [EName]
        },
        toolbox: {
            show: true,
            feature: {
                mark: {
                    show: true
                },
                dataView: {
                    show: true,
                    readOnly: false
                },
                magicType: {
                    show: true,
                    type: ['line', 'bar', 'stack', 'tiled']
                },
                restore: {
                    show: true
                },

                saveAsImage: {
                    show: true
                }
            }
        },
        calculable: true,
        xAxis: [{
            type: 'category',
            boundaryGap: false,
            data: dateArray //;前7天 日期=dateArray ['周一', '周二', '周三', '周四', '周五', '周六', '周日']
        }],
        yAxis: [{
            type: 'value'
        }],
        series: [
            {
                name: EName,//竞赛名称
                type: 'line',

                smooth: true,
                itemStyle: {
                    normal: {
                        areaStyle: {
                            type: 'default',
                            color: 'green',
                        }
                    }
                },
                data: scoreArray //分数 [80, 82, 70, 90, 91, 95, 90]
            }
        ]
    };
    // 使用刚指定的配置项和数据显示图表。
    myChart.setOption(option);
}

function Getinto(Eid, Pid)
{
    //不存在就新增
    $.ajax({
        url: "/HB_Competition/CheckGointo",
        data: { "Eid": Eid, "Pid": Pid, "Type": "2" },
        type: 'POST',
        async: false,
        dataType: 'json',
        success: function (data) {
            if (data == "1") {
                window.open('/SG_Competition/SG_CompetitionExamination?Exaid=' + Eid + "&Pid=" + Pid);
            } else {
                layer.alert('操作失败，请刷新或重新登录再试！', { skin: 'layui-layer-lan' }, function () {
                    layer.closeAll();//关闭所有弹出框
                });
            }
        }
    });
   
}


function See(Eid)
{
    //不存在就新增
    var AnswerId = "0";
    $.ajax({
        url: "/SG_Competition/See",
        data: { "Eid": Eid },
        type: 'POST',
        async: false,
        dataType: 'json',
        success: function (data) {
            AnswerId = data;
        }
    });
    window.open('/SG_Competition/ShowAchievement?ResponseState=' + AnswerId + '&ExaId=' + Eid + '');
}

//排行榜
function PH(Eid, Pid) {
    layer.open({
        type: 1,
        title: '排行榜',
        // skin: 'layui-layer-lan', //样式类名
        anim: 2,
        shadeClose: false, //开启遮罩关闭
        area: ['800px', '440px'], //宽高
        content: $("#PHId"),
        end: function () {
            layer.closeAll();//关闭所有弹出框
        }
    });

    //获取数据
    $.ajax({
        url: "/SG_Competition/GetPH",
        data: { "Eid": Eid, "Pid": Pid },
        type: 'POST',
        async: false,
        dataType: 'text',
        success: function (data) {
            $("#PHContent").html(data);
        }
    });
}
