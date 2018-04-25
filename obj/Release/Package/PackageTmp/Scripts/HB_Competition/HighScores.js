/***************************************************************
  FileName:总排行榜 javascript
  Copyright（c）2017-金融教育在线技术开发部
  Author:袁学
  Create Date:2017-4-24
 ******************************************************************/


$(function () {
    setTab('one', 1)
})
//切换标签
function setTab(name, cursel) {
    for (var i = 0; i < 4; i++) {
        if (cursel == (i + 1)) {
            $("#one" + (i + 1)).addClass("zzjs");
            $("#con_one_" + (i + 1)).show();
        } else {
            $("#con_one_" + (i + 1)).hide();
            $("#one" + (i + 1)).removeClass();
        }
    }


    $.ajax({
        url: "/HighScores/GetPH?Type=" + cursel,
        type: 'POST',
        async: false,
        dataType: 'text',
        success: function (data) {
            $("#con_one_" + cursel).html(data);
        }
    });

}


