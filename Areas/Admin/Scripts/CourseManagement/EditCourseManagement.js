
$(function () {
    redload();
});

function btn_Update(Id) {
    if (Id == 1) {
        $("#upfile").click();
        $("#upfile").change(function (e) {
            $("#UpName").html($(this).val().split('\\')[$(this).val().split('\\').length - 1]);
            $("#WaitingUp").html("(等待上传)")
        })
    } else {
        $("#upimg").click();
        $("#upimg").change(function (e) {
            $("#UpName_img").html($(this).val().split('\\')[$(this).val().split('\\').length - 1]);
            setImagePreview();
            $("#WaitingUp_img").html("(等待上传)");
        })
    }
}


//function clickaddimg() {
//    $("#upimg").click();
//}
//隐藏图片
function Close() {
    $(".img_close").hide();
}
//本地图片
function setImagePreview() {
    $("#addimg").html('  <img class="img_close" src="/img/iconclose.png" onclick="Close()" style="position: absolute; left: 165px; top: -5px;"><img class="img_close" src="/img/student/icoppt.png" onclick="clickaddimg()" style="width: 170px; height: 160px; cursor: pointer;" id="preview" />');
    //选择图片后直接显示
    var docObj = document.getElementById("upimg");
    var imgObjPreview = document.getElementById("preview");
    if (docObj.files && docObj.files[0]) {
        var f = docObj.files;
        var imgtype = f[0].name;//获取文件名
        //判断只能设置图片
        var exp = /.jpg$|.gif$|.jpeg$|.png$|.bmp$/;
        if (exp.exec(imgtype) == null) {
            layer.alert('图片格式不正确！', {
                skin: 'layui-layer-red', closeBtn: 0
            });
            return false;
        }
        imgObjPreview.src = window.URL.createObjectURL(docObj.files[0]);
    }
    return true;
}

function Save() {
    if ($("#CourseName").val() == "") {
        layer.msg('课程名称不能为空', function () { });
        return;
    }
    $('#form_Course').ajaxSubmit({
        url: "/Admin/CourseManagement/Upload",
        type: "POST",
        dataType: "json",
        data: $('#form_Course').serialize(),
        beforeSend: function (xhr, self) {
            var index = layer.load(0, { shade: false }); //0代表加载的风格，支持0-2
            if ($("#WaitingUp").html() != "") { $("#WaitingUp").html("(正在上传)"); }
            if ($("#WaitingUp_img").html() != "") { $("#WaitingUp_img").html("(正在上传)"); }
        },
        success: function (data) {
            layer.closeAll();
            if (data == "-1") {
                layer.msg('上传文件格式错误', function () { });
                return;
            } if (data > 0) {
                if (data == "888") {
                    layer.msg('PPT文件异常，请检查文件！', { icon: 2 });
                    return;
                }
                if (data == "999") {
                    layer.msg('课程名称已存在', function () { });
                } else {
                    layer.msg('操作成功！', { icon: 1 });
                    if ($("#WaitingUp").html() != "") { $("#WaitingUp").html("(上传成功)"); }
                    if ($("#WaitingUp_img").html() != "") { $("#WaitingUp_img").html("(上传成功)"); }
                    setTimeout("window.location.href = '/Admin/CourseManagement/Courselist'", 1000);

                }
                return;
            }
            else {
                layer.msg('操作失败', { icon: 2 });
                return;
            }
        },
        error: function (data) {
            layer.closeAll();
            layer.msg('上传文件异常！', { icon: 2 });
        }
    });
}

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