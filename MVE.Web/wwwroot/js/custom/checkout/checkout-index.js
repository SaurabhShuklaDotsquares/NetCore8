(function ($) {
    function CheckoutIndex() {
        var $this = this;

        function initilizeForm() {
            $("#IsAcknowlwdge-err").hide();
            //---------- Checkout payment saving -----------------
            $(document).on("submit", "#form-proceedPay", function (event) {
                event.preventDefault();
                event.stopImmediatePropagation();
                var formData = new FormData(this);            
                var url = this.action;
                if ($("#IsAcknowlwdge").is(':checked') == false) {
                    $("#IsAcknowlwdge-err").show();
                    return false;
                }
                $("#front_loader").show();
                $.ajax({
                    url: url,
                    type: 'POST',
                    data: formData,
                    success: function (result) {                        
                        if (result.isSuccess == true) {
                            $("#front_loader").hide();                            
                            // Global.ShowMessage(result.message, Global.MessageType.Success);                           
                            setTimeout(function () {
                                window.location.href = result.redirectUrl;
                               // window.location.href = "/Dashboard/Index";
                            }, 1000);
                        } else {
                            $("#front_loader").hide();
                            Global.ShowMessage(result.errorMessage, Global.MessageType.Error);
                        }
                    },
                    error: function (result) {
                        $("#front_loader").hide();
                        Global.ShowMessage(result.errorMessage == undefined ? "undefined request" : result.errorMessage, Global.MessageType.Error);
                    },
                    cache: false,
                    contentType: false,
                    processData: false
                });
                return false;
            });


            //---------- country based state -----------------

            $("#CountryId").on("change", function () {
                var selectedCountry = $(this).val();
                $.ajax({
                    url: "/Checkout/GetStates", 
                    type: "GET",
                    data: { country: selectedCountry },
                    success: function (result) {
                        var stateDropdown = $("#StateId");
                        stateDropdown.empty();
                        if (result == "" || result.length==0) {
                            stateDropdown.append($("<option></option>").val("0").text("-- Select --"));
                            $("#StateId").attr('disabled', 'disabled');

                        } else {
                            $("#StateId").removeAttr('disabled');

                            stateDropdown.append($("<option></option>").val("0").text("-- Select --"));
                        }
                        $.each(result, function (index, item) {
                            
                            stateDropdown.append($("<option></option>").val(item.value).text(item.text));
                        });
                    },
                    error: function (error) {
                        console.log("Error fetching states: " + error);
                    }
                });               
            });

        }
        $this.init = function () {
            initilizeForm();
            $("#StateId").attr('disabled', 'disabled');
        };
    }
    $(function () {
        var self = new CheckoutIndex();
        self.init();
    })
})(jQuery)

