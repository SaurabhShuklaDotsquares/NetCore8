function setIPhoneMenu() {

    jQuery('.subItem').remove();
    jQuery(".iphonNav").css("display", "none");
    jQuery(".iphonNav").addClass("iphon_navigation");
    jQuery(".iphonNav").removeClass("navigation");
    jQuery(".iphonNav > ul").attr("id", "");
    jQuery(".menuImage").css("display", "block");
    jQuery(".iphonNav ul li").each(function() {
        if (jQuery(this).children("ul").length > 0) {
            jQuery(this).addClass("parentMenu");
            jQuery(this).prepend("<div class='subItem'></div>");
            jQuery(this).children("ul").addClass("subMenu");
        }
    });
    //jQuery(".subMenu").css("display", "none");
    jQuery(".menuImage").unbind("click");
   jQuery(".menuImage").click(function() {
        jQuery(".iphonNav").slideToggle();
    });
    jQuery(".subItem").unbind("click");
    jQuery(".subItem").click(function(e) {
        jQuery(this).parent().children(".subMenu").slideToggle(300, function() { 
            if (jQuery(this).is(":visible")) {
                jQuery(this).parent().parent().children(".parentMenuActive").find(".subMenu").each(function() {
                    jQuery(this).slideUp(300);
                    jQuery(this).css("display", "none");
                    jQuery(this).parent().removeClass("parentMenuActive");
                });
                jQuery(this).parent().parent().children(".parentMenuActive").removeClass("parentMenuActive");
                jQuery(this).parent().addClass("parentMenuActive");

            } else {
                jQuery(this).parent().removeClass("parentMenuActive");
                jQuery(this).find(".subMenu").each(function() {
                    jQuery(this).css("display", "none");
                    jQuery(this).parent().removeClass("parentMenuActive");

                });
            }
        });

        e.stopPropagation();
    });
}

function setDesktopMenu() {
    jQuery(".iphonNav").addClass("navigation");
    jQuery(".iphonNav").css("display", "block");
    jQuery(".iphonNav").removeClass("iphon_navigation");
    jQuery(".iphonNav>ul").attr("id", "nav");
    jQuery(".menuImage").css("display", "none");
}
jQuery(document).ready(function() {
    if (jQuery(window).width() < 992) {
        setIPhoneMenu();
    } else {
        setDesktopMenu();
    }
});
jQuery(window).resize(function() {
    if (jQuery(window).width() < 992) {
        setIPhoneMenu();
    } else {
        setDesktopMenu();
    }
});

jQuery(document).ready(function() {
	jQuery(document).on('click keyup touchstart',function(e) {
		if(e.which==27){
			jQuery('.iphonNav').removeClass('nav-open');
			jQuery('body').removeClass('scroll-hidden');			
		}else if($(e.target).closest('.iphonNav').length==0 && $(e.target).closest('.container2').length==0){
			jQuery('.iphonNav').removeClass('nav-open');
			jQuery('body').removeClass('scroll-hidden');			
		}
    });
    jQuery(document).on('click','.menuImage',function() {
        jQuery('.iphonNav').toggleClass('nav-open');
        jQuery('body').toggleClass('scroll-hidden');
    });
	$(document).on('scroll',window,function(){
		$(window).scrollTop()>=80 ? $("body").addClass("navnewclass") : $("body").removeClass("navnewclass") 
	});
});
