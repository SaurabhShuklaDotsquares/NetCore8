(function ($) {
    function ChangePasswordIndex() {
        var $this = this;

        function initilizeForm() {       
          
                $(document).on("submit", "#frm-user-change-password", function (event) {
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
                            Global.ShowMessage(result.message, Global.MessageType.Success);
                            window.setTimeout(function () {
                                window.location.href = "/Dashboard/Index";
                            }, 2000);
                        } else {
                            Global.ShowMessage(result.errorMessage, Global.MessageType.Error);

                        }
                    },
                    error: function (result) {
                        Global.ShowMessage(result.errorMessage == undefined ? "Undefiend Requset" : result.errorMessage, Global.MessageType.Error);
                    },
                    cache: false,
                    contentType: false,
                    processData: false
                });
                return false;
            });


        }
        function showPassword() {
            $(".user-toggle_pwd").click(function () {
                $(this).toggleClass("fa-eye fa-eye-slash");
                var type = $(this).hasClass("fa-eye-slash") ? "password" : "text";
                $(".password_id").attr("type", type);
            });
            $(".user-toggle_oldpwd").click(function () {
                $(this).toggleClass("fa-eye fa-eye-slash");
                var type = $(this).hasClass("fa-eye-slash") ? "password" : "text";
                $(".oldpassword_id").attr("type", type);
            });

        }
        $this.init = function () {
            showPassword()
            initilizeForm();
        };

    }
    $(function () {
        var self = new ChangePasswordIndex();
        self.init();
    });
})(jQuery);