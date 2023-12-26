(function ($) {
    function StaticPageJs() {
        var $this = this;
        $this.init = function () {

            $('.dis-inline').on('click', function () {
                var staticpage = $(this).attr("data-attr");
                window.location = domain + staticpage;
            });
        };
    }
    $(function () {
        var self = new StaticPageJs();
        self.init();


    });
}(jQuery));