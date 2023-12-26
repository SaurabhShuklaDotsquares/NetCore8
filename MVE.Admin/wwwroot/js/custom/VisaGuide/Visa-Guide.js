(function ($) {
    function ManageVisaGuideIndex() {
        var $this = this;
        function initilization() {
            $(document).on("submit", "#visaGuid-Form", function (event) {
                event.preventDefault();
                event.stopImmediatePropagation();
                var id = 'CountryId';
                var msg = "Please select country";
                var IdValue = $('#' + id).val();
                if (IdValue > 0) {
                    $('[data-valmsg-for=' + id + ']').text('');
                } else {
                    $('[data-valmsg-for=' + id + ']').text(msg);
                    return false;
                }

                var title = $("#PageTitle").val().trim();
               

                // Check if any editor content is null or empty
                var editor = CKEDITOR.instances.ContentData;

                if (
                    !title ||

                    !editor.getData().trim()
                ) {
                    Global.ShowMessage('Please check and ensure that all title and description content should not be empty', Global.MessageType.Error);
                    return false;
                }
                loader();

                var formData = new FormData(this);
                var url = this.action;
                
                $.ajax({
                    url: url,
                    type: 'POST',
                    data: formData,
                    success: function (result) {
                        if (result != null) {
                            Global.ShowMessage(result.message, Global.MessageType.Success);
                            window.setTimeout(function () {
                                window.location.href = "/Admin/Country/Index";
                            }, 2000);
                            
                        } else {
                            Global.ShowMessage(result.message, Global.MessageType.Error);
                        }
                        $(".loader").hide();
                        window.setTimeout(function () {
                            window.history.back();
                        }, 3000);
                    },
                    error: function (result) {
                        Global.ShowMessage(result.message == undefined ? "Server not responding" : result.message, Global.MessageType.Error);
                    },
                    cache: false,
                    contentType: false,
                    processData: false
                });
                return false;
            });
                     
        }
        function GetVisaGuideSection() {
            $.get("/Admin/VisaGuide/VisaGuideSection", { id: $('#CountryId').val() }, function (res) {
                $("#visa-body").html('');
                $("#visa-body").html(res);
                CKEDITOR.replace('ContentData');
            })
        }
        $this.init = function () {
            GetVisaGuideSection();

            //$("#CountryId").select2();
            initilization();
           
        };
    }

    $(function () {
        var self = new ManageVisaGuideIndex();
        self.init();
    })
})(jQuery)



