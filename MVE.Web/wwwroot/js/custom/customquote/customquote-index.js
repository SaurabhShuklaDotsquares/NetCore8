(function ($) {
    function CustomQuoteIndex() {
        var $this = this;

        function initilizeForm() {

            //---------- CustomQuote saving -----------------
            $(document).on("submit", "#form-customquote", function (event) {
                event.preventDefault();
                event.stopImmediatePropagation();
                var formData = new FormData(this);
                var url = this.action;                 
                if (!isValidURL($("#packageUrl").val())) {
                    Global.ShowMessage("Please enter a valid URL.", Global.MessageType.Error);
                    return false;
                }
                $("#front_loader").show();

                $.ajax({
                    url: url,
                    type: 'POST',
                    data: formData,
                    success: function (result) {
                        $("#front_loader").hide();
                        if (result.isSuccess == true) {
                            //Global.ShowMessage(result.message, Global.MessageType.Success); 
                            
                            //$(".no-gutters").html("<div style='padding: 80px;'>" + result.message + "</div>");
                            $("#customquote-div-formblock").html("<div style='padding: 80px;'>" + result.message + "</div>");
                            
                        } else {
                            Global.ShowMessage(result.errorMessage, Global.MessageType.Error);
                        }
                    },
                    error: function (result) {
                        Global.ShowMessage(result.errorMessage == undefined ? "something went wrong on request" : result.errorMessage, Global.MessageType.Error);
                    },
                    cache: false,
                    contentType: false,
                    processData: false
                });
                return false;
            });


            //---------- CustomQuote saving end-----------------


        }

        $this.init = function () {
            initilizeForm();
           // $("#packageUrl").val(window.location.href);
   
            $(document).on('click', '.resendmailcls', function (e) {

                $("#front_loader").show();
                var linkUrl = $(this).attr('data-href');

                $.ajax({
                    url: linkUrl,
                    type: 'GET',
                    success: function (data) {
                        $("#front_loader").hide();
                        
                        if (data.isSuccess == true) {
                            $("#customquote-div-formblock").html("<div style='padding: 80px;'>" + data.message + "</div>");
                        } else {
                            Global.ShowMessage(data.errorMessage, Global.MessageType.Error);
                        }
                    },
                    error: function (xhr, status, error) {
                        // Handle any errors here.
                        console.log('Error:', error);
                    }
                });



            });








        };
    }
    $(function () {
        var self = new CustomQuoteIndex();
        self.init();
    })
})(jQuery)


function isValidURL(url) {
    var urlPattern = /^(https?|ftp):\/\/[^\s/$.?#].[^\s]*$|^www\.[^\s/$.?#].[^\s]*$/;
    return urlPattern.test(url);
}