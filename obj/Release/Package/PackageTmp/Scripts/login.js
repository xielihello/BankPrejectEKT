var cookieName = "LoginName";
var cookiePwd = "pwd";
var cookieCookie = "cbCookie";
$(function () {
    var LoginName = $.cookie(cookieName);
    var pwd = $.cookie(cookiePwd);
    var cbCookie = $.cookie(cookieCookie);
    $("#username").val(LoginName);
    $("#password").data("pwd", "");
    if (pwd != null && pwd != "null") {
        $("#password").data("pwd", pwd);
        $("#password").val("******");
    }
    if (cbCookie != false && cbCookie != "false") {
        $("#cbCookie")[0].checked = cbCookie;
    }


    $("#password").change(function () {
        $(this).data("pwd", $(this).val());
    });
});


function Logon(type) {
    var LoginName = $.trim($("#username").val());
    if (LoginName == "") {
        layer.msg("用户名不能为空");
        $("#username").focus();
        return;
    }
    //var UserPwd = $.trim($("#password").val());
   var  UserPwd = $("#password").data("pwd");
    if (UserPwd == "") {
        layer.msg("密码不能为空");
        $("#password").focus();
        return;
    }

    var isSaveCookie;
    var cookies = $('#cbCookie').prop("checked");

    if (cookies == false) {
        isSaveCookie = 0;
    }
    else {
        isSaveCookie = 1;
    }

    $.ajax({
        Type: "post",
        url: '/Login/AjaxLogin',
        dataType: "text", cache: false,
        contentType: "application/json; charset=utf-8",
        data: { 'LoginName': encodeURIComponent(LoginName), 'UserPwd': encodeURIComponent(UserPwd), 'isSaveCookie': encodeURIComponent(isSaveCookie) },
        success: function (data) {
            var obj = data.split('#');
            $.cookie(cookieName, LoginName, { expires: 7 });
            $.cookie(cookieCookie, cookies, { expires: 7 });
            if (cookies) {
                $.cookie(cookiePwd, UserPwd, { expires: 7 });
            } else {
                $.cookie(cookiePwd, null);
            }
            if (obj[0] == "1") {//管理员
                location.href = "/Admin/CollegeManagement/College";
            } else if (obj[0] == "2") {//教师
                location.href = "/Admin/T_StudentManage";
            } else if (obj[0] == "3") {//学生  
                if (obj.length > 1) { 
                    if (obj[1].indexOf("1") >= 0) {
                        location.href = "/HB_Competition";
                        return;
                    }
                    if (obj[1].indexOf("2") >= 0) {
                        location.href = "/SG_Competition";
                        return;
                    }
                    if (obj[1].indexOf("3") >= 0) {
                        location.href = "/FH_Management";
                        return;
                    }
                    if (obj[1].indexOf("4") >= 0) {
                        location.href = "/Bill_Competition/GrowthProcess";
                        return;
                    }
                }

            } else if (obj[0] == "4") {//裁判
                location.href = "/Judge";
            } else if (obj[0] == "-3") {
                layer.msg("账号已被锁定或冻结！");
                return;
            } else {
                $("#password").val("");
                $("#password").focus();
                layer.msg("用户名密码不正确！");
            }

        }

    });
}

document.onkeydown = function () {
    if (event.keyCode == 13) {
        if ($("#password").val() != "******") {
            $("#password").data("pwd", $("#password").val());
        }
        Logon('1');
    }
};

function GetCookie(name) {
    var arr = document.cookie.match(new RegExp("(^| )" + name + "=([^;]*)(;|$)"));
    if (arr != null) {
        return unescape(arr[2]);
    }
    return null;
}