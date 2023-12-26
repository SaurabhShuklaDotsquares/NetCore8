(function ($) {
    function RatingReviewIndex() {
        var $this = this;

        function initilizeForm() {

            $(document).on("submit", "#frm-user-rating", function (event) {
                event.preventDefault();
                event.stopImmediatePropagation();
                var bookingid = $($(this).find('#BookingId')).val();
                var isAnyStarChecked = $('.star-rating span').hasClass('checked');
                if (!isAnyStarChecked) {
                    Global.ShowMessage("Please select atleast one star(*) for rating", Global.MessageType.Error);
                    return false;
                }
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
                            $("#rating-popup-" + bookingid).modal('hide');
                            setTimeout(function(){
                                localStorage.setItem("MenulinkKey", "mybooking");
                                window.location.href = "/Dashboard/Index";
                            }, 2000)
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
            $('.star-rating span').on("click", function () {
                FillRatingColor(this);
            });
        };

    }
    $(function () {
        var self = new RatingReviewIndex();
        self.init();
    });
})(jQuery);

function FillRatingColor(obj) {
    $('.star-rating span').removeClass('checked');
    // Add the 'checked' class to all stars up to and including the clicked star
    $(obj).prevAll().addBack().addClass('checked');
    $("#Rating").val($(obj).attr('star-val'))
}

