
$(document).ready(function() {

    if($('#Avatar').val() != null)
    {
        var num_str = $('#Avatar').val();
        var num = num_str.substr(num_str.length - 5)[0];
        var num_btn = '#avatar-btn-'+num;
        $(num_btn).addClass('active');
    }

    //Avatar select
    $('.avatar-btn').on('click', function () {
        $('.avatar-container .active').removeClass('active');
        $(this).addClass('active');
        $('#Avatar').val("default-"+$(this).data('id')+'.png');
    });

});