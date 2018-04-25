
$(function () {
    redload();
    DataBind();
})

function redload() {
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

function DataBind(page) {
    var Name = $("#Name").val();
    var PageSize = 10;
    $.ajax({
        url: '/Admin/CourseManagement/CourseDataBind',
        Type: "post",
        dataType: "json",
        async: false,
        data: { Name: Name, page: page, PageSize: PageSize },
        success: function (data) {
            var td = data;
            $("#table").html(td.TableHTML);
            bootstrapPaginator("#PaginatorLibrary", td, DataBind);//分页
            redload();
        }
    });
}

//启用
function Enable(type, Id) {
    if (type == "0") {
        var rad_strchks = "";
        var rad_chks = document.getElementsByName('check');//name
        for (var i = 0; i < rad_chks.length; i++) {
            if (rad_chks[i].checked == true) {
                rad_strchks += "" + rad_chks[i].value + ",";
            }
        }
        Id = rad_strchks + "0";
    }
    $.ajax({
        url: '/Admin/CourseManagement/CourseEnable',
        Type: "post",
        dataType: "json",
        cache: false,
        contentType: "application/json; charset=utf-8",
        data: { Id: Id, Jurisdiction: "1" },
        success: function (data) {
            if (data > 0) {
                if (data == "999") {
                    layer.msg('不可重复启用！', { icon: 2 });
                    return;
                } else {
                    layer.msg('操作成功！', { icon: 1 });
                    DataBind();
                }
            } else {
                layer.msg('操作失败', { icon: 2 });
                return;
            }
        }
    });
}

//禁用
function Disable(type, Id) {
    if (type == "0") {
        var rad_strchks = "";
        var rad_chks = document.getElementsByName('check');//name
        for (var i = 0; i < rad_chks.length; i++) {
            if (rad_chks[i].checked == true) {
                rad_strchks += "" + rad_chks[i].value + ",";
            }
        }
        Id = rad_strchks + "0";
    }
    $.ajax({
        url: '/Admin/CourseManagement/CourseEnable',
        Type: "post",
        dataType: "json",
        cache: false,
        contentType: "application/json; charset=utf-8",
        data: { Id: Id, Jurisdiction: "2" },
        success: function (data) {
            if (data > 0) {
                if (data == "999") {
                    layer.msg('不可重复禁用！', { icon: 2 });
                    return;
                } else {
                    layer.msg('操作成功！', { icon: 1 });
                    DataBind();
                }
            } else {
                layer.msg('操作失败', { icon: 2 });
                return;
            }
        }
    });
}

//删除
function Dele() {
    var rad_strchks = "";
    var rad_chks = document.getElementsByName('check');//name
    for (var i = 0; i < rad_chks.length; i++) {
        if (rad_chks[i].checked == true) {
            rad_strchks += "" + rad_chks[i].value + ",";
        }
    }
    if (rad_strchks == "") {
        layer.msg('请先勾选一行数据！', function () { });
        return;
    }
    Id = rad_strchks + "0";
    layer.confirm("您确定要删除这个课程吗？", {
        title: '删除',
        btn: ['确定', '取消'],
        shadeClose: true, //开启遮罩关闭
        skin: 'layui-layer-lan'
        //按钮
    },
      function () {
          $.ajax({
              url: '/Admin/CourseManagement/IsDelete',
              Type: "post",
              dataType: "json",
              cache: false,
              contentType: "application/json; charset=utf-8",
              data: { Id: Id },
              success: function (data) {
                  if (data > 0) {
                      layer.msg('操作成功！', { icon: 1 });
                      DataBind();
                  } else {
                      layer.msg('操作失败', { icon: 2 });
                      return;
                  }
              }
          });
      });
}

//编辑
function Edit(Id) {
    window.location.href = "/Admin/CourseManagement/EditCourseManagement?Id=" + Id;
}

//搜索
$("#Query").click(function () {
    DataBind();
}); 