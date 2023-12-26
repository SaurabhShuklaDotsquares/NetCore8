(function ($) {
    function LoginIndex() {
        var $this = this;
        function initilizeForm() {
            //var loginMessageDiv = $('#loginMessageDiv');
            //var signInForm = new Global.FormHelper($("#form-signIn"), {
            //    updateTargetId: "validation-summary",
            //    validateSettings: {
            //        ignore: []
            //    }
            //}, function onSuccess(result) {
            //    
            //    //if (Global.IsNotNullOrEmptyString(result.errorMessage) && result.errorMessage !== "Success") {

            //    //    // loginMessageDiv = $('#loginMessageDiv');
            //    //    loginMessageDiv.empty().html(result.errorMessage);
            //    //    loginMessageDiv.show();

            //    //}
            //    //else {
            //    //    $("#login-close-btn").click();
            //    //    $("#login-li").hide();
            //    //    window.location.href = "/Dashboard/Index";
            //    //}

            //    //window.setTimeout(function () {
            //    //    loginMessageDiv.html('');
            //    //    loginMessageDiv.hide();
            //    //}, 5000);
            //    if (result.isSuccess == true) {
            //       // Global.ShowMessage(result.message, Global.MessageType.Success);
            //        window.setTimeout(function () {
            //            window.location.href = "/Dashboard/Index";
            //        }, 2000);
            //    } else {
            //        Global.ShowMessage(result.errorMessage, Global.MessageType.Error);

            //    }
            //});

            $(document).on("submit", "#form-signIn", function (event) {
                event.preventDefault();
                event.stopImmediatePropagation();
                var formData = new FormData(this);
                var url = this.action;
                $("#front_loader").show();
                $.ajax({
                    url: url,
                    type: 'POST',
                    data: formData,
                    success: function (result) {
                        $("#front_loader").hide();

                        if (result.isSuccess == true) {
                            $("#login-close-btn").click();
                            window.location.href = "/Dashboard/Index";
                        } else {
                            Global.ShowMessage(result.errorMessage, Global.MessageType.Error);

                        }
                    },
                    error: function (result) {
                        Global.ShowMessage(result.errorMessage == undefined ? "Undefined Request" : result.errorMessage, Global.MessageType.Error);
                    },
                    cache: false,
                    contentType: false,
                    processData: false
                });
                return false;
            });


            //---------- Register -----------------
            $(document).on("submit", "#form-signUP", function (event) {                
                event.preventDefault();
                event.stopImmediatePropagation();
                var formData = new FormData(this);
                var url = this.action; // if this does not work then use '@Url.Action("Create","NewsletterSubscriptions")'
                $("#front_loader").show();
                $.ajax({
                    url: url,
                    type: 'POST',
                    data: formData,
                    success: function (result) {
                        $("#front_loader").hide();

                        if (result.isSuccess == true) {
                            Global.ShowMessage(result.message, Global.MessageType.Success);
                            $("#register-popup").modal('hide');
                            $(".modal-backdrop").remove()
                            $("#register-popup").hide();
                            //if (result.data != null && result.data != undefined) {
                            //    $.get("/Account/VerifyOTP/", { userid: result.data.id }, function (pageresult) {
                            //        $("#register-popup .modal-content").html(pageresult);
                            //        VerifyOTPTimer();
                            //    });
                            //}
                        } else {
                            Global.ShowMessage(result.errorMessage, Global.MessageType.Error);

                        }
                    },
                    error: function (result) {
                        Global.ShowMessage(result.errorMessage == undefined ? "Please Check all details" : result.errorMessage, Global.MessageType.Error);
                    },
                    cache: false,
                    contentType: false,
                    processData: false
                });
                return false;
            });


            //---------- Register -----------------

            //---------- Verify OTP -----------------
            $(document).on("submit", "#form-verifyOtp", function (event) {

                event.preventDefault();
                event.stopImmediatePropagation();
                var formData = new FormData(this);
                var url = this.action;

                if ($("#timer").text() == "" || $("#timer").text() == '0:00') {
                    Global.ShowMessage("OTP has been expired,Please resend the OTP", Global.MessageType.Error);
                    return false;
                }
                // time checking for 0.00
                $("#front_loader").show();
                $.ajax({
                    url: url,
                    type: 'POST',
                    data: formData,
                    success: function (result) {
                        $("#front_loader").hide();

                        if (result.isSuccess == true) {
                            Global.ShowMessage(result.message, Global.MessageType.Success);
                            $("#register-popup").modal('hide');
                            $(".modal-backdrop").remove()

                        } else {
                            Global.ShowMessage(result.errorMessage, Global.MessageType.Error);
                        }
                    },
                    error: function (result) {
                        Global.ShowMessage(result.errorMessage == undefined ? "Please Check all details" : result.errorMessage, Global.MessageType.Error);
                    },
                    cache: false,
                    contentType: false,
                    processData: false
                });
                return false;
            });


            //---------- Verify OTP -----------------

            //---------- Resend OTP -----------------
            $(document).on("click", "#resendOtp_btn", function (event) {
                event.preventDefault();
                event.stopImmediatePropagation();
                var userid = $("#form-verifyOtp").find("#Id").val();
                $("#EmailOtp").val('');
                $("#front_loader").show();
                $.ajax({
                    url: "/Account/ResendOTP?userid=" + userid,
                    type: 'POST',
                    success: function (result) {
                        $("#front_loader").hide();
                        if (result.isSuccess == true) {
                            Global.ShowMessage(result.message, Global.MessageType.Success);

                            VerifyOTPTimer();

                        } else {
                            Global.ShowMessage(result.errorMessage, Global.MessageType.Error);
                        }
                    },
                    error: function (result) {
                        Global.ShowMessage(result.errorMessage == undefined ? "Please Check all details" : result.errorMessage, Global.MessageType.Error);
                        $("#front_loader").hide();
                    },
                    cache: false,
                    contentType: false,
                    processData: false
                });
                return false;
            });


            //---------- Resend OTP -----------------

            //Forgotpassword

            // Forgotpassword
            $(document).on("click", "#forgotpassword_btn", function (event) {
                event.preventDefault();
                event.stopImmediatePropagation();

                // Close the "Sign In" modal
                $("#login-popup").modal('hide');

                // Show the "Forgot Password" modal
                $("#forgotpassword-popup").modal('show');
            });

            $(document).on("click", "#register_btn", function (event) {
                event.preventDefault();
                event.stopImmediatePropagation();

                // Close the "Sign In" modal
                $("#login-popup").modal('hide');

                // Show the "Forgot Password" modal
                $("#register-popup").modal('show');
            });



            $(document).on("click", "#forgotpassword-submit", function (event) {
                event.preventDefault();
                event.stopImmediatePropagation();
                var email = $("#Email").val();
                if (!Global.IsNotNullOrEmptyString(email)) {
                    $("#Email").closest(".form-group").find('span').text("Please enter email");
                    return false;
                }
                if ($("#Email").closest(".form-group").find('span').text() != "") {
                    return false;
                }
                $("#front_loader").show();

                $.ajax({
                    type: "POST",
                    url: "/Account/ForgotPassword",
                    data: { Email: email },
                    success: function (result) {
                        var loginMessageDiv = $('#loginMessageDiv');
                        if (Global.IsNotNullOrEmptyString(result.errorMessage) && result.errorMessage !== "Success") {
                            loginMessageDiv = $('#loginMessageDiv');
                            loginMessageDiv.empty().html(result.errorMessage);
                            loginMessageDiv.show();
                            if (result.isSuccess) {
                                Global.ShowMessage(result.message, Global.MessageType.Success);
                            }
                            else {
                                Global.ShowMessage(result.errorMessage, Global.MessageType.Error);
                            }
                            $("#forgotpassword-popup").modal('hide');
                            $(".modal-backdrop").remove();
                            $("#login-popup").modal('hide');
                            $(".modal-backdrop").remove();
                            $("#forgotpassword-popup").hide();
                            $("#login-popup").hide();
                        }
                        else {
                            if (result.isSuccess == true) {

                                Global.ShowMessage(result.message, Global.MessageType.Success);
                                $("#forgotpassword-popup").modal('hide');
                                $(".modal-backdrop").remove();
                                $("#login-popup").modal('hide');
                                $(".modal-backdrop").remove();
                                $("#forgotpassword-popup").hide();
                                $("#login-popup").hide();

                            } else {
                                Global.ShowMessage(result.errorMessage, Global.MessageType.Error);
                            }
                            //window.location.href = result.redirectUrl;
                        }
                        $("#front_loader").hide();

                        window.setTimeout(function () {
                            loginMessageDiv.html('');
                            loginMessageDiv.hide();
                            $("#login-popup").hide();
                            $("#forgotpassword-popup").hide();
                        }, 5000);

                    },
                    error: function () {
                        // Handle AJAX errors here.
                    }
                });
            });






            //var forgotpasswordForm = new Global.FormHelper($("#form-forgotpassword"), {
            //    updateTargetId: "validation-summary",
            //    validateSettings: {
            //        ignore: []
            //    }
            //},
            //    function onSuccess(result) {
            //    var loginMessageDiv = $('#loginMessageDiv');

            //    $("#front_loader").show();      
            //    //$("#login-popup").modal('hide');
            //    if (Global.IsNotNullOrEmptyString(result.errorMessage) && result.errorMessage !== "Success")
            //    {
            //        loginMessageDiv = $('#loginMessageDiv');
            //        loginMessageDiv.empty().html(result.errorMessage);
            //        loginMessageDiv.show();
            //        if (result.isSuccess) {
            //            Global.ShowMessage(result.message, Global.MessageType.Success);
            //        }
            //        else {
            //            Global.ShowMessage(result.errorMessage, Global.MessageType.Error);
            //        }
            //        $("#forgotpassword-popup").modal('hide');
            //        $(".modal-backdrop").remove();
            //        $("#login-popup").modal('hide');
            //        $(".modal-backdrop").remove();
            //        $("#forgotpassword-popup").hide();
            //        $("#login-popup").hide();
            //    }
            //    else {
            //        if (result.isSuccess == true) {

            //            Global.ShowMessage(result.message, Global.MessageType.Success);
            //            $("#forgotpassword-popup").modal('hide');
            //            $(".modal-backdrop").remove();
            //            $("#login-popup").modal('hide');
            //            $(".modal-backdrop").remove();
            //            $("#forgotpassword-popup").hide();
            //            $("#login-popup").hide();

            //        } else {
            //            Global.ShowMessage(result.errorMessage, Global.MessageType.Error);
            //        }
            //        //window.location.href = result.redirectUrl;
            //    }

            //    window.setTimeout(function () {     
            //        $("#front_loader").hide();
            //        loginMessageDiv.html('');
            //        loginMessageDiv.hide();
            //        $("#login-popup").hide();
            //        $("#forgotpassword-popup").hide();
            //    }, 5000);
            //});
        }
        function showPassword() {
            $(".toggle_pwd").click(function () {
                $(this).toggleClass("fa-eye fa-eye-slash");
                var type = $(this).hasClass("fa-eye-slash") ? "password" : "text";
                $(".password_id").attr("type", type);
            });

        }


        $this.init = function () {
            showPassword();
            initilizeForm();
        };
    }
    $(function () {
        var self = new LoginIndex();
        self.init();
    })
})(jQuery)

function VerifyOTPTimer() {
    const durationInSeconds = 7 * 60;

    // Get the timer element
    const timerElement = document.getElementById('timer');

    // Function to update the timer display
    function updateTimerDisplay(seconds) {
        const minutes = Math.floor(seconds / 60);
        const remainingSeconds = seconds % 60;
        const display = `${minutes}:${remainingSeconds < 10 ? '0' : ''}${remainingSeconds}`;
        timerElement.textContent = '';
        timerElement.textContent = display;
    }

    // Function to start the timer
    function startTimer() {
        let seconds = durationInSeconds;
        updateTimerDisplay(seconds);

        const timerInterval = setInterval(() => {
            seconds--;

            if (seconds < 0) {
                clearInterval(timerInterval);
                timerElement.textContent = '0:00'; // Timer reached 0
                // You can add any action to perform when the timer reaches 0 here.
            } else {
                updateTimerDisplay(seconds);
            }
        }, 1000); // Update every 1 second (1000 milliseconds)
    }

    // Start the timer when the page loads (you can trigger it with a button click or any other event)
    //window.onload = startTimer;
    startTimer();
}