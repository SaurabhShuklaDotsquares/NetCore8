(function ($) {
    function LoginIndex() {
        var $this = this;

        function initilizeForm() {

            var signInForm = new Global.FormHelper($("#form-signIn"), {
                updateTargetId: "validation-summary",
                validateSettings: {
                    ignore: []
                }
            }, function onSuccess(result) {
                $loginMessageDiv = $('#loginMessageDiv');
                if (Global.IsNotNullOrEmptyString(result.errorMessage) && result.errorMessage !== "Success") {
                    $loginMessageDiv.empty().html(result.errorMessage);
                    $loginMessageDiv.show();
                }
                else {                    
                    var dataUrl = result.redirectUrl;
                    window.location.href = dataUrl;
                }

                window.setTimeout(function () {
                    $loginMessageDiv.html('');
                    $loginMessageDiv.hide();
                }, 5000);
            });
        }
        function showPassword() {
            $("#toggle_pwd").click(function () {
                $(this).toggleClass("fa-eye fa-eye-slash");
                var type = $(this).hasClass("fa-eye-slash") ? "password" : "text";
                $("#Password").attr("type", type);
            });
           
        }       
        function disableBackBtn() {           
           setTimeout(preventBack(), 0);
            window.onunload = function () { null };
        }

        $this.init = function () {
            disableBackBtn();
            showPassword();
            initilizeForm();
            
        };
    }

    function preventBack() {
        window.history.forward();
    }
    $(function () {
        var self = new LoginIndex();
        self.init();
    })
})(jQuery)