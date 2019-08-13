

window.addEventListener('DOMContentLoaded',
    function () {


        var current_page = 1;
        $("#load-more").on('click', function (event) {
            event.stopPropagation();
            event.stopImmediatePropagation();
            ++current_page;
            $.ajax({
                url: '/Home/AjaxImages',
                type: 'post',
                data:
                {
                    page: current_page
                },
                success: function (data) {
                    if (data && data.length) {
                        data.forEach(function (image) {
                            
                            $('.grid').colcade('append', $.parseHTML(
                                '<div class="grid-item">' +
                                '                        <div class="card card-album my-3 border-0 mx-2 animated--shadow-translate px-0">' +
                                '                            <img class="card-img-top rounded darken-20 text-economica h-100" src="' + img_upload_path + '/' + image.path + '/' + image.name +'" alt="Header Image">' +
                                '                            <div class="card-img-overlay">' +
                                '                                <div class="d-flex flex-wrap">' +
                                '                                    <h4 class="mx-auto text-economica">' + image.name +'</h4>' +
                                '                                </div>' +
                                '' +
                                '                                <div class="d-flex flex-wrap">' +
                                '                                    <h5 class="mx-auto text-economica">' + image.path + '</h5>' +
                                '                                </div>' +
                                '' +
                                '                                <div class="d-flex flex-wrap">' +
                                '                                    <h5 class="mx-auto text-economica">' + moment(image.addTime).format("YYYY-MM-DD") +'</h5>' +
                                '                                </div>' +
                                '                            </div>' +
                                '                        </div>' +
                                '                    </div>'));


                        });
                    } else {
                        $("#load-more").text('没有更多图片了');
                        $("#load-more").removeClass('btn-outline-primary');
                        $("#load-more").addClass('btn-outline-danger');

                    }
                }
            });

        });

    });