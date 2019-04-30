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
            for (var i = 0; i < $(this)[0].files.length; i++) {
                files.push($(this)[0].files[i].name);
            }
            $(this).next('.custom-file-label').html(files.join(' '));
    });

    $('.toast').toast('show');

});