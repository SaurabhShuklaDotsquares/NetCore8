(function ($) {
    function ContactUsIndex() {
        var $this = this;

        function initilizeForm() {

            $(document).on("submit", "#frm-user-personal", function (event) {
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
                            $("#personal-tab").click();

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

            $(document).on("submit", "#frm-user-billing", function (event) {
                event.preventDefault();
                event.stopImmediatePropagation();
                var formData = new FormData(this);
                var url = this.action;
                $("#CountryId").closest(".form-group").find('span').text("");
                $("#StateId").closest(".form-group").find('span').text("");
                if ($("#CountryId").val() == 0) {
                    $("#CountryId").closest(".form-group").find('span').text("Please select country");
                    return false;
                }
                if ($("#CountryId").val() != 0) {
                    if ($("#StateId").val() == 0) {
                        $("#StateId").closest(".form-group").find('span').text("Please select state");
                        return false;
                    }
                }
                $("#front_loader").show();
                $.ajax({
                    url: url,
                    type: 'POST',
                    data: formData,
                    success: function (result) {
                        $("#front_loader").hide();
                        if (result.isSuccess == true) {
                            Global.ShowMessage(result.message, Global.MessageType.Success);
                            $("#billing-tab").click();

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


            //---------- get country based state -----------------
            $("#CountryId").on("change", function () {
                
                var selectedCountry = $(this).val();
                $.ajax({
                    url: "/Checkout/GetStates",
                    type: "GET",
                    data: { country: selectedCountry },
                    success: function (result) {
                        var stateDropdown = $("#StateId");
                        stateDropdown.empty();
                        if (result == "" || result.length == 0) {
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
        function InitilizeDatePicker() {
            $("#DateofBirth").datepicker({
                dateFormat: "dd/mm/yy", // Set the date format
               // minDate: 1,            // Restrict dates to today and beyond
                maxDate: 0,         // Restrict dates up to 1 year in the future

            }).datepicker("setDate", 'now');
            $("#StateId").attr('disabled', 'disabled');
        }

        $this.init = function () {
            initilizeForm();
            InitilizeDatePicker();
        };

    }
    $(function () {
        var self = new ContactUsIndex();
        self.init();
    });
})(jQuery);



