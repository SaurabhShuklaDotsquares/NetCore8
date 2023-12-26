$('.destinations-slider').owlCarousel({
    loop:true,
    margin:25,
    nav:true,
    dots:false,
    autoplay:true,
    items:6,
    responsive:{
        0:{
            items:1
        },
        480:{
            items:2
        },
        768:{
            items:4
        },
        992:{
            items:6
        },
        1441:{
            items:6
        }
    }
})

$('.desitnation-detail-slider').owlCarousel({
    loop:true,
    nav:true,
    dots:false,
    autoplay:true,
    items:1,
})

$('.package-price-slider').owlCarousel({
    loop:true,
    margin:0,
    nav:true,
    dots: false, 
    items:3,
    margin:30,
    autoWidth:true,
    center:true,
})



  $('#carousel').flexslider({
    animation: "slide",
    controlNav: false,
    animationLoop: false,
    slideshow: false,
    itemWidth: 110,
    itemMargin: 7,
    asNavFor: '#slider'
  });

  $('#slider').flexslider({
    animation: "slide",
    controlNav: false,
    animationLoop: false,
    slideshow: false,
    sync: "#carousel",
    start: function(slider){
      $('body').removeClass('loading');
    }
  });


//Range slider js//
$(document).ready(function() {
   // $("#ex2").slider({});
});