

function Forgotpassowrd() {
    var forgetpassword = [];
    var email = $("#email").val();
    forgetpassword.push(email);
    $.ajax
        ({
            type: "POST",
            contentType: "application/json",
            dataType: "JSON",
            data: JSON.stringify({ email: email }),
            url: "/Login/ForgetPassword",
            success: function (response) {
                if (response.success) {
                    ShowSuccess();
                } else {
                    console.log(response);
                    alert(response.message);
                }
            },
            error: function (errormessage) {
                alert(errormessgae);
            }
        });
}

