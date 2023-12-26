(function ($) {
    function HolidayIndex() {
        var $this = this;

        function initilizeForm() {

            //---------- Book Now saving -----------------
            $(document).on("submit", "#form-BookNow", function (event) {
                event.preventDefault();
                event.stopImmediatePropagation();
                var formData = new FormData(this);
                var url = this.action; 
                $.ajax({
                    url: url,
                    type: 'POST',
                    data: formData,
                    success: function (result) {
                        
                        if (result.isSuccess == true) {
                            // Global.ShowMessage(result.message, Global.MessageType.Success);
                            $("#booknow-form-block").html("<div style='padding: 80px;'>" + result.message + "</div>");
                            setTimeout(function () {
                                location.reload(true);
                            }, 8000);
                        } else {
                            Global.ShowMessage(result.errorMessage, Global.MessageType.Error);
                        }
                    },
                    error: function (result) {
                        Global.ShowMessage(result.errorMessage == undefined ? "Please Check all details" : result.errorMessage, Global.MessageType.Error);
                    },
                    cache: false,
                    contentType: false,
                    processData: false
                });
                return false;
            });


            //---------- Book Now saving -----------------



        }


        $(document).ready(function () {
            
            // Handle the click event for individual duration checkboxes
            $(".duration-checkbox").click(function () {
                // Check or uncheck all individual duration checkboxes based on the state of the clicked checkbox
                var isChecked = $(this).prop("checked");
                if ($(this).hasId("listing-duration-0")) {
                    // Handle the "All Duration" checkbox
                    $(".duration-checkbox").prop("checked", isChecked);
                } else {
                    // Handle individual duration checkboxes
                    var allDurationChecked = $(".duration-checkbox:checked").length === $(".duration-checkbox").length;
                    $(".all-duration-checkbox").prop("checked", allDurationChecked);
                }
            });
        });



        $this.init = function () {
            initilizeForm();

        };
    }
    $(function () {
        var self = new HolidayIndex();
        self.init();
    })
})(jQuery)

