(function ($) {
    function AddAdminUser() {
        var $this = this;

        function initilizeForm() {
            var formSetting = new Global.FormHelperWithFiles($("#frm-adminuser"), {
                updateTargetId: "validation-summary",
                validateSettings: {
                    ignore: []
                }, beforeSubmit: function () {
                    var phone_err = $("#mobilenumber");
                    var phonenumber = $('#MobilePhone').val();

                    if (phonenumber != "" && (phonenumber.length < 10 || phonenumber.length > 16)) {
                        phone_err.html("Enter valid phone number and length should be 10 to 16 character");
                        return false;
                    }
                    return true;
                }
            }, function onSuccess(result) {
                if (result) {
                    if (result.isSuccess) {
                        //window.location.href = "/Admin/AdminUser/Index";
                        //Global.ShowMessage(result.message, Global.MessageType.Success);
                        //window.setTimeout(function () {
                        //    var $userMessageDiv = $('#userMessageDiv');
                        //    if (Global.IsNotNullOrEmptyString(result.message)) {
                        //        $userMessageDiv.addClass('alert-success').removeClass('alert-danger');
                        //        $userMessageDiv.empty().html(result.message);
                        //        $userMessageDiv.show();
                        //        //window.setTimeout(function () {
                        //        //    $userMessageDiv.html('');
                        //        //    $userMessageDiv.hide();
                        //        //    location.reload();
                        //        //}, 2000);
                        //    }
                        //}, 5000);
                        var $settingMessageDiv = $('#settingMessageDiv');
                        if (Global.IsNotNullOrEmptyString(result.message)) {
                            $settingMessageDiv.addClass('alert-success').removeClass('alert-danger');
                            $settingMessageDiv.empty().html(result.message);
                            $settingMessageDiv.show();
                        }

                        window.setTimeout(function () {
                            $settingMessageDiv.html('');
                            $settingMessageDiv.hide();
                            window.location.href = "/Admin/AdminUser/Index";
                        }, 2000);
                    }
                    else {
                        if (result.status) {
                            var $settingMessageDiv = $('#settingMessageDiv');
                            if (Global.IsNotNullOrEmptyString(result.message)) {
                                $settingMessageDiv.addClass('alert-success').removeClass('alert-danger');
                                $settingMessageDiv.empty().html(result.message);
                                $settingMessageDiv.show();
                                //window.location.href = "/Admin/Dashboard/Index";
                            }

                            window.setTimeout(function () {
                                $settingMessageDiv.html('');
                                $settingMessageDiv.hide();
                                location.reload();
                            }, 2000);
                        }
                        else {
                            var $settingMessageDiv = $('#settingMessageDiv');
                            if (Global.IsNotNullOrEmptyString(result.message)) {
                                $settingMessageDiv.addClass('alert-danger').removeClass('alert-success');
                                $settingMessageDiv.empty().html(result.message);
                                $settingMessageDiv.show();
                            } else if (Global.IsNotNullOrEmptyString(result.errorMessage)) {
                                Global.ShowMessage(result.errorMessage, Global.MessageType.Error);

                            }

                            window.setTimeout(function () {
                                $settingMessageDiv.html('');
                                $settingMessageDiv.hide();
                            }, 2000);
                        }
                    }
                }
            });


            $("#toggle_pwd").click(function () {
                $(this).toggleClass("fa-eye fa-eye-slash");
                var type = $(this).hasClass("fa-eye-slash") ? "password" : "text";
                $("#Password").attr("type", type);
            });
            //$("#toggle_Confirmpwd").click(function () {
            //    $(this).toggleClass("fa-eye fa-eye-slash");
            //    var type = $(this).hasClass("fa-eye-slash") ? "password" : "text";
            //    $("#ConfirmPassword").attr("type", type);
            //});
            if ($("#Id").val() == "0" && $("#TypeMode").val() != "edit") {
                $('.chkup').hide();
                $("#updatepass").show();
                $("#IsPassUpdate").attr('checked', 'checked');
            }
            else {
                $('.chkup').show();
                $("#updatepass").hide();
            }
            $(document).on('change', '#IsPassUpdate', function (e) {
                if (this.checked) {
                    $("#updatepass").show();
                }
                else {
                    $("#updatepass").hide();
                }
            });
        }

        $this.init = function () {
            initilizeForm();
        };

    }
    $(function () {
        var self = new AddAdminUser();
        self.init();
    });
})(jQuery);