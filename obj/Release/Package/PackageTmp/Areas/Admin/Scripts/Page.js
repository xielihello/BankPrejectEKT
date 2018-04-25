function bootstrapPaginator(obj, data,clicks) {
   
    if (data == null || data.Total == 0) {

        $(obj).hide();
    } else {
        $(obj).show();
        var Total = data.Total //总条数
        var PageIndex = data.PageIndex//页数
        var PageTotal = data.PageTotal//总页数
        var PageSize = data.PageSize  //每页记录 

        var html = '<div>';
        html += '<div class="dataTables_paginate paging_simple_numbers " id="editable_paginate">';
        html += ' <ul class="pagination">';
        html += '<li class="paginate_button previous home_page" aria-controls="editable" tabindex="0" id="">';
        html += '<a href="javascript:void(0);">首页</a>';
        html += '</li>';
        html += '<li class="paginate_button  Previous_page" aria-controls="editable" tabindex="0" id="">';
        html += '<a href="javascript:void(0);">上一页</a>';
        html += '</li>';
        html += '<li class="paginate_button  next_page" aria-controls="editable" tabindex="0" id="">';
        html += '<a href="javascript:void(0);">下一页</a>';
        html += '</li>';
        html += '<li class="paginate_button  Shadowe" aria-controls="editable" tabindex="0" id="Shadowe">';
        html += '<a class="boder_right" href="javascript:void(0);">尾页</a>';
        html += '</li>';
        html += '<li><span class="pagesize">转到</span>';
        html += '</li>';
        html += '<li class="paginate_button" aria-controls="editable" tabindex="0" id="">';
        html += '<span class="no-padding ">';
        html += "<input type=\"text\" onkeyup=\"this.value=this.value.replace(/\D/g,'')\" class=\"number\" /></span>";
        html += '</li>';
        html += '<li class="paginate_button clik_GO" aria-controls="editable" tabindex="0" id="">';
        html += '<a class="boder_right" href="javascript:void(0);">GO</a>';
        html += '</li>';
        html += '<li>';
        html += '<span class="pagesize Statistics" id="">第<span id="page">' + PageIndex + '</span>页/共<span>' + PageTotal + '</span>页</span>';
        html += '</li>';
        html += '</ul>';
        html += '</div>';
        html += '</div>';
        $(obj).html(html);

        if (PageIndex == 1) {
            $(obj).find(".Previous_page").addClass("disabled");
        }
        if (PageIndex >= PageTotal) {
            $(obj).find(".next_page").addClass("disabled");
        }

        $(obj).find(".home_page").click(function () {//首页
            clicks(1);
        });
        //$(obj).find(".Previous_page").bind('click', clicks)
        $(obj).find(".Previous_page").click(function () {//上一页
            var index = parseInt(PageIndex) - 1;
            if (index > 0) {
                clicks(index);
            }
        });
        $(obj).find(".next_page").click(function () {//下一页
            var index = parseInt(PageIndex) + 1;
            if (index < (PageTotal + 1)) {
                clicks(index);
            }
        });
        $(obj).find(".Shadowe").click(function () {//尾页
            var index = PageTotal;
            clicks(index);
        });
        $(obj).find(".clik_GO").click(function () {//GO
            var index = $(obj).find(".number").val();
            if (isInteger(index) && index > 0 && index < (PageTotal + 1)) { 
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
 