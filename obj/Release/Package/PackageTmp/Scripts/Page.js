function bootstrapPaginator(obj, data, clicks) {
    
    if (data == null || data.Total == 0) { 
        $(obj).hide();
    } else {
        $(obj).show();
        var Total = data.Total //总条数
        var PageIndex = data.PageIndex//页数
        var PageTotal = data.PageTotal//总页数
        var PageSize = data.PageSize  //每页记录 

        var html = '';
        html += '<a class="Previous_page">◀</a>'; 

        if (PageIndex > 1) {
            html += '<a class="PageIndex  Index_' + parseInt(PageIndex - 1) + '">' + parseInt(PageIndex - 1) + '</a>';
        }
        if (PageTotal > PageIndex) {
            var pin = PageTotal - PageIndex;
            html += '<a class="PageIndex Index_' + PageIndex + '">' + PageIndex + '</a>';
            if (pin == 1) {
                html += '<a class="PageIndex  Index_' + PageTotal + '">' + PageTotal + '</a>';
            } else if (pin == 2) {
                html += '<a class="PageIndex  Index_' + parseInt(PageIndex + 1) + '">' + parseInt(PageIndex + 1) + '</a>';
                html += '<a class="PageIndex  Index_' + PageTotal + '">' + PageTotal + '</a>';
            } else {
                html += '<a class="PageIndex  Index_' + parseInt(PageIndex + 1) + '">' + parseInt(PageIndex + 1) + '</a>';
                html += '<a class="PageIndex  Index_' + parseInt(PageIndex + 2) + '">' + parseInt(PageIndex + 2) + '</a>';
                html += '<span>......</span>';
                html += '<a class="PageIndex  Index_' + PageTotal + '">' + PageTotal + '</a>';

            } 
        } else {
            html += '<a class="PageIndex   Index_' + PageTotal + '">' + PageTotal + '</a>';
        } 
        html += '<a class="next_page">▶</a>';
        html += '<span>跳转到</span>';
        html += '<span>';
        html += '<input type="text" class="number" name="" id="" value="" /></span>';
        html += '<a class="clik_GO">GO</a> ';
        html += '';
        $(obj).html(html);
        $(obj).find(".Index_" + PageIndex).addClass("xz");
        if (PageIndex == 1) {
            $(obj).find(".Previous_page").addClass("disabled");
        }
        if (PageIndex >= PageTotal) {
            $(obj).find(".next_page").addClass("disabled");
        }
         
        //$(obj).find(".Previous_page").bind('click', clicks)
        $(obj).find(".Previous_page").click(function () {//上一页
            var index = parseInt(PageIndex) - 1;
            if (index > 0) {
                page = index;
                clicks(index);
            }
        });
        $(obj).find(".PageIndex").click(function () {//页数
            var index = $(this).html()
            if (isInteger(index)) {
                page = index;
                clicks(index);
            } else {
                layer.msg('分页错误！', function () { });
                return;
            }
        })
        $(obj).find(".next_page").click(function () {//下一页
            var index = parseInt(PageIndex) + 1;
            if (index < (PageTotal + 1)) {
                page = index;
                clicks(index);
            }
        }); 
        $(obj).find(".clik_GO").click(function () {//GO
            var index = $(obj).find(".number").val(); 
            if (isInteger(index) && index > 0 && index < (PageTotal + 1)) {
                page = index;
                clicks(index);
            } else {
                layer.msg('请输入正确的页数！', function () { });
                $(obj).find(".number").focus();
            }
        });

    }



}
function isInteger(x) {
    return x % 1 === 0;
}
