(function ($) {
    function ContactUsIndex() {
        var $this = this;

        function initilizeForm() {       
          
            $(document).on("submit", "#frm-user-contactus", function (event) {
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
       
        $this.init = function () {           
            initilizeForm();
        };

    }
    $(function () {
        var self = new ContactUsIndex();
        self.init();
    });
})(jQuery);