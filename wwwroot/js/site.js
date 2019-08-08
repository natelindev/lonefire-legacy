// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

(function($) {
  "use strict"; // Start of use strict

  // Scroll to top button appear
  $(document).on('scroll', function() {
      var scrollDistance = $(this).scrollTop();
      if (scrollDistance > 100) {
          $('.scroll-to-top').fadeIn();
      } else {
          $('.scroll-to-top').fadeOut();
      } 
  });
    // Toggle the side navigation
    $(".sidebar-toggler").on('click', function(e) {
        $(".sidebar").toggleClass("toggled");
        $(".sidebar-toggler").toggleClass("toggled");
        if ($(".sidebar").hasClass("toggled")) {
          $('.sidebar .collapse').collapse('hide');
        };
  });

  // Smooth scrolling using jQuery easing
  $(document).on('click', 'a.nav-link', function(e) {
    var $anchor = $(this);
    $('html, body').stop().animate({
      scrollTop: ($($anchor.attr('href')).offset().top)
    }, 600, 'swing');
    e.preventDefault();
  });

    $(document).on('click', 'a.scroll-to-top', function(e) {
    var $anchor = $(this);
    $('html, body').stop().animate({
      scrollTop: ($($anchor.attr('href')).offset().top)
    }, 1000, 'easeInOutExpo');
    e.preventDefault();
  });

})(jQuery); // End of use strict

$(document).ready(function() {

    
    //smooth hover class modifcation
    $('.border-draw, .border-draw-within').hoverIntent(function() {
        $(this).removeClass('active');
            setTimeout(function(){
                $(this).addClass('temp');
            },500);
        },
        function(){
            $(this).addClass('active');
            $(this).removeClass('temp');
        }
    );

    //File Label
    $('.custom-file input').change(function (e) {
        var files = [];
        var names = [];

        for (var i = 0; i < $(this)[0].files.length; i++) {
            files.push($(this)[0].files[i].name);
        }

        for (var i = 0; i < $(this)[0].files.length; i++) {
            names.push('![' + $(this)[0].files[i].name + ']' + '(' + $(this)[0].files[i].name+ ')');
        }

        $(this).next('.custom-file-label').html(files.join(' '));
        $('#Content').append(names.join(' '));
    });

    $('.toast').toast('show');

    function headerImgLoaded() {
        //get rgb array from img
        var colorThief = new ColorThief();
        var sourceImg = document.getElementById('header-img');
        var rgbs = colorThief.getPalette(sourceImg, 8);
        var hsls = [];
        //rgb to hsl
        rgbs.forEach(function (rgb) {
            //console.log('rbg: ' + rgb);
            hsls.push(rgbToHsl(rgb[0], rgb[1], rgb[2]));
        });

        var generator = new ColorfulBackgroundGenerator();

        // This adds 5 layers to the generator
        // The parameters are: degree[0-360],
        //                     h[0-360], 
        //                     s[0-1], 
        //                     l[0-1],
        //                     posColor[0-100], 
        //                     posTransparency[0-100]
        // The lowest layer (at the bottom) in the css is the first added layer.
        hsls.forEach(function (hsl) {
            //console.log('hsl: '+ hsl);
            generator.addLayer(new ColorfulBackgroundLayer({ degree: getRandomInt(20, 300), h: hsl[0], s: hsl[1], l: hsl[2], posColor: getRandomInt(0, 50), posTransparency: getRandomInt(50, 80) }));
        });

        // Assign generated style to the element identified by it's id
        generator.assignStyleToElementId("page-top");
    }

    var img = document.getElementById('header-img');
    if (img.complete) {
        headerImgLoaded();
    }else {
        img.addEventListener('load', headerImgLoaded);
    }
    
});

function toast(message,option) {
    var new_toast = 
            '<div class="toast mx-auto text-white border-0 shadow-lg bg-'+option+'" role="alert" aria-live="assertive" aria-atomic="true" data-delay="5000">' +
            '<div class="toast-body">'+
            message+
            '</div>'+
            '</div>';
        $(new_toast).appendTo(".toast-wrapper");
        $('.toast').toast('show');
        setTimeout(function() {
            $('.toast-wrapper').empty();
        },6000);
}

function rgbToHsl(r, g, b){
    r /= 255, g /= 255, b /= 255;
    var max = Math.max(r, g, b), min = Math.min(r, g, b);
    var h, s, l = (max + min) / 2;

    if(max == min){
        h = s = 0; // achromatic
    }else{
        var d = (max - min);
        s = l >= 0.5 ? d / (2 - (max + min)) : d / (max + min);
        switch(max){
            case r: h = ((g - b) / d + 0)*60; break;
            case g: h = ((b - r) / d + 2)*60; break;
            case b: h = ((r - g) / d + 4)*60; break;
        }
    }

    return [h, s, l];
}

function getRandomInt(min, max) {
    min = Math.ceil(min);
    max = Math.floor(max);
    return Math.floor(Math.random() * (max - min + 1)) + min;
}
