

window.addEventListener('DOMContentLoaded',
    function () {


        var current_page = 1;
        $("#load-more").on('click', function (event) {
            $("#load-more").hide();
            document.getElementById('loadmore_spinner').style.display = 'block';
            event.stopPropagation();
            event.stopImmediatePropagation();
            ++current_page;
            $.ajax({
                url: '/Home/AjaxNotes',
                type: 'post',
                data:
                {
                    page: current_page
                },
                success: function (data) {
                    if (data && data.length) {
                        data.forEach(function (note) {
                            var lock_icon = ''
                                if (note.status === 1) {
                                    lock_icon = '<i class="material-icons text-primary mr-auto mb-n1">lock</i>'
                            }
                            var title = note.title ? note.title : '';
                            $('.grid').colcade('append', $.parseHTML(
                                
                                '<div class="grid-item">' +
                                '' +
                                '                <div class="card mx-auto card-w-40 mx-sm-2 mx-md-3 border-0 my-5 shadow-lg">' +
                                '                    <div class="card-body">' +
                                '                        <div class="d-flex flex-wrap">' +
                                '                            <h6 class="mx-auto font-weight-bold text-primary">' + title + '</h6>' +
                                '                        </div>' +
                                '                        '+ note.content +
                                '                        <div class="d-flex mt-4">' +
                                                            lock_icon + 
                                '                            <small class="ml-auto text-primary mb-n1">' + moment(note.addTime).format("YYYY-MM-DD") + '</small>' +
                                '                        </div>' +
                                '                    </div>' +
                                '                </div>' +
                                '            </div>'

                            ));


                        });
                    } else {
                        $("#load-more").text('没有更多动态了');
                        $("#load-more").removeClass('btn-outline-primary');
                        $("#load-more").addClass('btn-outline-danger');
                    }
                    $("#load-more").show();
                    hideLoadMoreSpinner();
                }
            });

        });

    });