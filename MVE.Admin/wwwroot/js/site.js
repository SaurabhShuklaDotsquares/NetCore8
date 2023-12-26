(function ($) {
    'use strict';

    function SiteBarIndex() {
        var $this = this;
        function setNavigation() {
            if ($('.nav-link').hasClass('active')) {
                $('.nav-link.active').parents('ul>li.nav-item.has-treeview').addClass('menu-open');
            }
            else {
                var url = window.location;
                $('ul.nav-sidebar a').filter(function () {
                    return this.href == url;
                }).addClass('active');
                $('ul.nav-treeview a').filter(function () {
                    return this.href == url;
                }).parentsUntil(".nav-sidebar > .nav-treeview").addClass('menu-open').prev('a').addClass('active');
            }
           
        }

        $this.init = function () {
            setNavigation();
        };
    }

    $(function () {
        var self = new SiteBarIndex();
        self.init();
    });

}(jQuery));