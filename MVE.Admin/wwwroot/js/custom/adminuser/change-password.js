(function ($) {
    function ChangePasswordIndex() {
        var $this = this;

        function initilizeForm() {
            var formChangePassword = new Global.FormHelper($("#frm-change-password"), {
                updateTargetId: "validation-summary",
                validateSettings: {
                    ignore: []
                }
            }, function onSuccess(result) {
                    if (result) {
                        $("#frm-change-password").trigger("reset");
                        var $changePasswordMessageDiv = $('#changePasswordMessageDiv');
                        if (Global.IsNotNullOrEmptyString(result.message)) {
                            $changePasswordMessageDiv.addClass('alert-success').removeClass('alert-danger');
                            $changePasswordMessageDiv.empty().html(result.message);
                            $changePasswordMessageDiv.show();
                            window.location.href = "/Admin/Dashboard/Index";
                        }

                        window.setTimeout(function () {
                            $changePasswordMessageDiv.html('');
                            $changePasswordMessageDiv.hide();
                        }, 5000);
                    }
            });
        }

        $this.init = function () {
            initilizeForm();
        };

    }
    $(function () {
        var self = new ChangePasswordIndex();
        self.init();
    });
})(jQuery);