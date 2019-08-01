$(document).ready(function() {

    //File Label
    $('.custom-file input').change(function (e) {
        var files = [];
        for (var i = 0; i < $(this)[0].files.length; i++) {
            files.push('[' + $(this)[0].files[i].name + ']' + '(' + $(this)[0].files[i].name+ ')');
        }
        $('#Content').append(files.join(' '));
    });

});
