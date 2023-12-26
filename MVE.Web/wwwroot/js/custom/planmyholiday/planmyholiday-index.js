(function ($) {
    function HolidayIndex() {
        var $this = this;

        function initilizeForm() {

            //---------- plan my holiday saving -----------------

            const form = document.getElementById('form-planymyholiday');
            form.addEventListener('keypress', function (e) {
                if (e.keyCode === 13) {
                    e.preventDefault();
                }
            });


            $(document).on("submit", "#form-planymyholiday", function (event) {
                event.preventDefault();
                event.stopImmediatePropagation();
                $("#front_loader").show();

                var formData = new FormData(this);
                var url = this.action;
                $.ajax({
                    url: url,
                    type: 'POST',
                    data: formData,
                    success: function (result) {

                        $("#front_loader").hide();
                        if (result.isSuccess == true) {
                            // Global.ShowMessage(result.message, Global.MessageType.Success);
                            $(".plan-my-holiday-right").html("<div style='padding: 80px;'>" + result.message + "</div>");
                            //setTimeout(function () {
                            //    location.reload(true);
                            //}, 8000);
                        } else {
                            Global.ShowMessage(result.errorMessage, Global.MessageType.Error);
                        }
                    },
                    error: function (result) {
                        $("#front_loader").hide();
                        Global.ShowMessage(result.errorMessage == undefined ? "Please Check all details" : result.errorMessage, Global.MessageType.Error);
                    },
                    cache: false,
                    contentType: false,
                    processData: false
                });
                //$('#startloader').hide();
                return false;

            });


            //---------- plan my holiday saving -----------------






        }

        function InitilizeDatePicker() {
            //$('#DepartureDate').daterangepicker({
            //    singleDatePicker: true,
            //    timePicker: false,
            //    //autoUpdateInput: false,
            //    //showDropdowns: true,
            //    //locale: {
            //    //    format: "dd-mm-yyyy"
            //    //}
            //    autoUpdateInput: true,
            //    showDropdowns: true,
            //    yearRange: "-100:+0",
            //    minDate: moment(),
            //    locale: {
            //        format: dateFormatCalenderWithoutTime //"dd-mm-yyyy"
            //    }
            //}).on('apply.daterangepicker', function (ev, picker) {

            //    var selectedDate = picker.startDate;

            //    // Get the current date
            //    var currentDate = moment();

            //    // Check if the selected date is in the past
            //    if (selectedDate.isBefore(currentDate, 'day')) {
            //        // If the selected date is in the past, prevent it from being selected
            //        $('#DepartureDate').val(''); // Clear the input field
            //    }

            //    $(this).val(picker.endDate.format(dateFormatCalenderWithoutTime)).change();

            //    $('#' + ev.delegateTarget.name).removeClass('error').next('span').remove();
            //}).on('keypress paste', function (e) {
            //    e.preventDefault();
            //    return false;
            //}).attr("autocomplete", "off");
            //$(document).off('change', ".datepicker").on('change', ".datepicker", function () {

            //});

            $("#DepartureDate").datepicker({
                dateFormat: "dd/m/yy", // Set the date format
                minDate: 0,            // Restrict dates to today and beyond
                maxDate: "+3Y",         // Restrict dates up to 1 year in the future

            }).datepicker("setDate", 'now');

            $("#DepartureMonth").datepicker({
                dateFormat: "MM/yy", // Set the date format
                minDate: 0,            // Restrict dates to today and beyond
                maxDate: "+1Y",         // Restrict dates up to 1 year in the future

            }).datepicker("setDate", 'now');

        }
        $this.init = function () {
            initilizeForm();

            $("#step2contact").hide();
            $("#step3tellus").hide();
            $("#step4almost").hide();

            $(".fixeddiv").hide();
            $(".flexiblediv").hide();
            $(".anytimediv").hide();

            //Accommodations
            var $radios = $('input:radio[name=Accommodations]');
            if ($radios.is(':checked') === false) {
                $radios.filter('[value=Homestays]').prop('checked', true);
            }

            //BookWhen
            var $radiosBookWhen = $('input:radio[name=BookWhen]');
            if ($radiosBookWhen.is(':checked') === false) {
                $radiosBookWhen.filter('[Id=optionbook_1]').prop('checked', true);
            }
            //TypeOfTrip
            var $radiosTypeOfTrip = $('input:radio[name=TypeOfTrip]');
            if ($radiosTypeOfTrip.is(':checked') === false) {
                $radiosTypeOfTrip.filter('[value=Adventurous]').prop('checked', true);
            }
            //PreferredTimeToCallYou
            //var $radiosPreferredTimeToCallYou = $('input:radio[name=PreferredTimeToCallYou]');
            //if ($radiosPreferredTimeToCallYou.is(':checked') === false) {
            //    $radiosPreferredTimeToCallYou.filter('[Id=Anytime_1]').prop('checked', true);
            //}

            //RequiredInYourLandPackage
            var $radiosRequiredInYourLandPackage = $('input:radio[name=RequiredInYourLandPackage]');
            if ($radiosRequiredInYourLandPackage.is(':checked') === false) {
                $radiosRequiredInYourLandPackage.filter('[Id=Honeymoon_1]').prop('checked', true);
            }
            InitilizeDatePicker();

        };
    }
    $(function () {
        var self = new HolidayIndex();
        self.init();
        $(document).on('click', '.resendMailClass', function (e) {

            $("#front_loader").show();
            var linkUrl = $(this).attr('data-href');

            $.ajax({
                url: linkUrl,
                type: 'GET',
                success: function (data) {
                    $("#front_loader").hide();
                    if (data.isSuccess == true) {

                        $(".plan-my-holiday-right").html("<div style='padding: 80px;'>" + data.message + "</div>");

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
        $(document).on('click', '.resendagainMailClass', function (e) {

            $("#front_loader").show();
            var linkUrl = $(this).attr('href');
            var msg = "<b>Thank you for submitting your request.</b> A confirmation link has been resent to your email { usermodel.Email }.Please click on the link to activate your request. Did't receive email? <a href='/CustomQuote/ResendCustomPlanConfirmationLink?userid={userid}&reqId={reqId}'> Resend Confirmation Link</a>";
            ////    $("#customquote-div-formblock").html("<div style='padding: 80px;'>" + msg + "</div>");

            $(".plan-my-holiday-right").html("<div style='padding: 80px;'>" + linkUrl + "</div>");
        });

        document.querySelectorAll('#datetype-div input[type="radio"][name="DepartureDateType"]').forEach(radio => radio.checked = false);

    })
})(jQuery)


function NextManageWizard(divid) {
    
    var from_err = $("#From-error");
    var to_err = $("#To-error");
    var datetype_err = $("#DepartureDateType-error");
    var err_msg = $("#err_msg");
    var email_err = $("#email_error");
    var phone_err = $("#phone_error");
    var rate_err = $("#rate_error");

    if (divid == "step2contact") {

        if (from_err.length > 0 && Global.IsNullOrEmptyString($("#From").val())) {
            from_err.html("Please select city name");
            return false;
        }
        else {
            from_err.html('');
        }
        if (to_err.length > 0 && Global.IsNullOrEmptyString($("#To").val())) {
            to_err.html("Please select city name");
            return false;
        }
        else {
            to_err.html('');
        }
        if (datetype_err.length > 0 && $('#datetype-div input[type="radio"][name="DepartureDateType"]:checked').length == "0") {
            datetype_err.html("Select any one date-type");
            return false;
        } else {
            datetype_err.html('');
            if (err_msg.length > 0) {
                if ($('#datetype-div input[type="radio"][name="DepartureDateType"]:checked').val() == "Fixed") {
                    err_msg.html('');
                    if (Global.IsNullOrEmptyString($("#DepartureDate").val())) {
                        err_msg.html("Select any One date");
                        return false;
                    }
                } else if ($('#datetype-div input[type="radio"][name="DepartureDateType"]:checked').val() == "Flexible") {
                    err_msg.html('');
                    if (Global.IsNullOrEmptyString($("#DepartureMonth").val()) || Global.IsNullOrEmptyString($("#DepartureWeek").val())) {
                        err_msg.html("Select Month, Enter Week and Enter Minimum 2 Days");
                        return false;
                    }
                } else if ($('#datetype-div input[type="radio"][name="DepartureDateType"]:checked').val() == "Anytime") {
                    err_msg.html('');

                    var depturevalue = $("#DepartureDays").val();

                    if (Global.IsNullOrEmptyString(depturevalue)) {
                        err_msg.html("Enter Minimum 2 Days");
                        return false;
                    }
                    var depturevalue1 = parseInt(depturevalue);
                    if (depturevalue1 < "2") {
                        err_msg.html("Enter Minimum 2 Days");
                        return false;
                    }
                } else {
                    err_msg.html('');
                }
            }
        }

        $("#step2contact").show();
        $("#step1where").hide();
        $("#step3tellus").hide()
        $("#step4almost").hide();
    }
    if (divid == "step3tellus") {
        var regex = /^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$/;
        if (email_err.length > 0 && Global.IsNullOrEmptyString($("#step2contact").find("#Email").val()) || !regex.test($("#step2contact").find("#Email").val())) {
            email_err.html("Enter valid email id");
            return false;
        }
        else {
            email_err.html('');
        }

        if (phone_err.length > 0 && Global.IsNullOrEmptyString($("#step2contact").find("#PhoneNumber").val()) || $("#step2contact").find("#PhoneNumber").val().length < 10 || $("#step2contact").find("#PhoneNumber").val().length > 16) {
            phone_err.html("Enter valid phone number the length should between 10 to 16 characters");
            return false;
        }
        else {
            phone_err.html('');
        }
        $("#step2contact").hide()
        $("#step3tellus").show()
        $("#step1where").hide();
        $("#step4almost").hide();
    }
    if (divid == "step4almost") {

        if (rate_err.length > 0 && $('#hotelrating-div input[type="radio"][name="HotelCategoryRating"]:checked').length == "0") {
            rate_err.html("Hotel Category(Rating) must be selected");
            return false;
        }
        else {
            rate_err.html('');
        }
        $("#step2contact").hide()
        $("#step3tellus").hide()
        $("#step1where").hide();
        $("#step4almost").show();
    }
}


function BackManageWizard(divid) {
    if (divid == "step2contact") {
        $("#step2contact").show();
        $("#step1where").hide();
        $("#step3tellus").hide()
        $("#step4almost").hide();
    }
    if (divid == "step3tellus") {

        $("#step2contact").hide()
        $("#step3tellus").show()
        $("#step1where").hide();
        $("#step4almost").hide();
    }
    if (divid == "step1where") {
        $("#step2contact").hide()
        $("#step3tellus").hide()
        $("#step1where").show();
        $("#step4almost").hide();
    }

}


function DepartureTypeShowHide(divid) {
    if (divid == "Fixed") {
        $("#err_msg").html('');
        $(".fixeddiv").show();
        $(".flexiblediv").hide();
        $(".anytimediv").hide();
    }
    if (divid == "Flexible") {
        $("#err_msg").html('');
        $(".fixeddiv").hide();
        $(".flexiblediv").show();
        $(".anytimediv").hide();
    }
    if (divid == "Anytime") {
        $("#err_msg").html('');
        $(".fixeddiv").hide();
        $(".flexiblediv").hide();
        $(".anytimediv").show();

    }
    $("#" + divid).click(); // for radio-btn checked case

}

