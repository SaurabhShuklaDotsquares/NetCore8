(function ($) {
    function SettingIndex() {
        var $this = this;

        function initilizeForm() {
            var formSetting = new Global.FormHelperWithFiles($("#frm-setting"), {
                updateTargetId: "validation-summary",
                validateSettings: {
                    ignore: []
                }, beforeSubmit: function () {
                    var phone_err = $("#mobilenumber");
                    var phonenumber = $('#SupportMobile').val();

                    if (phonenumber != "" && (phonenumber.length < 10 || phonenumber.length > 16)) {
                        phone_err.html("Enter valid phone number and length should be 10 to 16 character");
                        return false;
                    }
                    return true;
                }
            }, function onSuccess(result) {
                if (result) {

                    //$("#frm-setting").trigger("reset");
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
            });
        }

        $this.init = function () {
            initilizeForm();
            $(document).on('change', '#theme_file', function (e) {

                var reader = new FileReader();
                var files = event.target.files;
                if (files.length > 0) {
                    var extension = files[0].name.substr((files[0].name.lastIndexOf('.') + 1));
                    if (extension != "jpeg" && extension != "jpg" && extension != "png" && extension != "JPEG" && extension != "JPG" && extension != "PNG") {
                        Global.ShowMessage('Please choose jpeg/jpg/ png.', Global.MessageType.Error);
                        $('#imgCategory').val('');
                        return;
                    }
                    reader.onload = function () {
                        var output = document.getElementById('imgCategory');
                        output.src = reader.result;
                    };
                    reader.readAsDataURL(event.target.files[0]);
                }
            });

            $(document).on('change', '#theme_file_dark', function (e) {

                var reader = new FileReader();
                var files = event.target.files;
                if (files.length > 0) {
                    var extension = files[0].name.substr((files[0].name.lastIndexOf('.') + 1));
                    if (extension != "jpeg" && extension != "jpg" && extension != "png" && extension != "JPEG" && extension != "JPG" && extension != "PNG") {
                        Global.ShowMessage('Please choose jpeg/jpg/ png.', Global.MessageType.Error);
                        $('#imgCategoryDark').val('');
                        return;
                    }
                    reader.onload = function () {
                        var output = document.getElementById('imgCategoryDark');
                        output.src = reader.result;
                    };
                    reader.readAsDataURL(event.target.files[0]);
                }
            });
        };

    }
    $(function () {
        var self = new SettingIndex();
        self.init();
    });
})(jQuery);