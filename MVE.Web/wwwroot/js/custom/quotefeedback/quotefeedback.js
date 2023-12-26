(function ($) {
    function QuoteFeedbackIndex() {
        var $this = this;


        $(document).on("submit", "#form-QuoteFeedbackSubmit", function (event) {
            
            event.preventDefault();
            event.stopImmediatePropagation();
            $("#front_loader").show();

            var formData = new FormData(this);
            //formData = formData.serializeObject();
            //var form=$.extend(formData, { 'IsQuoteFullfillRequirement': $("#IsQuoteFullfillRequirement").is(":checked"), 'IsItRequiredMoreChanges': $("#IsItRequiredMoreChanges").is(":checked") }); //Send Additional data
            if ($("#IsItRequiredMoreChanges").is(":checked") == true) {
                if ($("#EmailContentForQuote").val() == "") {
                    $("#front_loader").hide();
                    $(".clsEmailContentForQuote").text("Email Content (Feedback message) required");
                    return false;
                }
            }
            else {
                $(".clsEmailContentForQuote").text("");
            }

            var url = this.action;
            $.ajax({
                url: url,
                type: 'POST',
                data: formData,
                success: function (result) {
                    
                    $("#front_loader").hide();
                    if (result.isSuccess == true) {
                       
                        $("#EmailContentForQuote").val('');
                        Global.ShowMessage(result.message +". Redirecting on home page..", Global.MessageType.Success);
                         setTimeout(function () {
                             window.location.href = $("#hdnHomeUrl").val();
                        }, 4000);

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

        $(document).on('click', '.confirmationstatusok', function (e) {

            if ($(this).is(':checked') == true) {
                $("#EmailContentForQuote").val("Its fine now, Let's close the deal.")
                $("#EmailContentForQuote").attr("readonly", "readonly");
                $("#EmailContentForQuote").attr("placeholder", "Its fine now, Let's close the deal");
                $("#EmailContentForQuote").attr("readonly", "readonly");

                $("#IsQuoteFullfillRequirementVal").val("1");
                $("#IsItRequiredMoreChangesVal").val("0");
                $("#IsQuoteFullfillRequirementVal").prop('checked', true);
                $("#IsItRequiredMoreChanges").prop('checked', false);
            } else {
                $("#EmailContentForQuote").val("")
                $("#EmailContentForQuote").removeAttr("readonly");
                $("#EmailContentForQuote").attr("placeholder", "Its required more changes");

                $("#IsQuoteFullfillRequirementVal").val("0");
                $("#IsItRequiredMoreChangesVal").val("1");
                $("#IsItRequiredMoreChanges").prop('checked', true);
                $("#IsQuoteFullfillRequirementVal").prop('checked', false);
            }
        });

        $(document).on('click', '.confirmationstatusnotok', function (e) {
            
            if ($(this).is(':checked') == true) {
                $("#EmailContentForQuote").val("")
                $("#EmailContentForQuote").removeAttr("readonly");
                $("#EmailContentForQuote").attr("placeholder", "Its required more changes");

                $("#IsQuoteFullfillRequirementVal").val("0");
                $("#IsItRequiredMoreChangesVal").val("1");
                $("#IsItRequiredMoreChangesVal").prop('checked', true);
                $("#IsQuoteFullfillRequirementVal").prop('checked', false);

            } else {
                $("#EmailContentForQuote").val("Its fine now, Let's close the deal.")
                $("#EmailContentForQuote").attr("readonly", "readonly");
                $("#EmailContentForQuote").attr("placeholder", "Its fine now, Let's close the deal");

                $("#IsQuoteFullfillRequirementVal").val("1");
                $("#IsItRequiredMoreChangesVal").val("0");
                $("#IsItRequiredMoreChangesVal").prop('checked', false);
                $("#IsQuoteFullfillRequirementVal").prop('checked', true);
            }
        });

        function initilizeForm() {
            const form = document.getElementById('form-QuoteFeedbackSubmit');
            form.addEventListener('keypress', function (e) {
                if (e.keyCode === 13) {
                    e.preventDefault();
                }
            });
            

            


        }


        $this.init = function () {
            initilizeForm();
            $("#header_main").addClass("inner-header");
        };
    }

    $(function () {
        var self = new QuoteFeedbackIndex();
        self.init();
    })

})(jQuery)

