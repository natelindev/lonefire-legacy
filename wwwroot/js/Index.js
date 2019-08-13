

window.addEventListener('DOMContentLoaded',
    function () {

        $.ajax({
            url: '/Home/AjaxIndex',
            type: 'post',
            data:
            {
                page: 1
            },
            success: function (data) {
                if (data && data.length) {
                    data.forEach(function (article) {
                        var tags = '';
                        var tag_html = '';
                        article.content = article.content.replace(/"/g, '&quot;')
                        article.title = article.title.replace(/"/g, '&quot;')
                        if (article.tag) {
                            tags = article.tag.split(',');
                            tags.forEach(function (tag) {
                                tag_html += '<a class="btn btn-outline-light px-1 py-1 mt-n2 mb-3 position-relative z-2" href="/Tag/List?name=' + tag + '">' + tag + '</a>'
                            });
                        }
                        $('.grid').colcade('append', $.parseHTML('<div class="grid-item">' +
                            '                            <div class="card card-article my-3 mx-auto mx-md-3 border-0 animated--shadow-translate">' +
                            '                                <img class="card-img-top darken-20 text-economica" src="' + img_upload_path + article.title  + '/' + article.headerImg + '" alt="' + article.title + ' Header Image">' +
                            '                                <div class="card-img-overlay">' +
                            tag_html +
                            '                                </div>' +
                            '                                <div class="card-body d-flex flex-column">' +
                            '                                    <div class="d-flex">' +
                            '                                        <h4 class="mx-auto text-economica">' + article.title + '</h4>' +
                            '                                    </div>' +
                            '                                    <div class="text-titillium d-flex my-3 flex-wrap flex-grow-1 text-break">' +
                            '                                        ' + article.content + '...' +
                            '                                    </div>' +
                            '                                    <div class="d-flex align-self-center text-economica justify-content-between">' +
                            '' +
                            '                                        <div class="mt-auto mb-2">' +
                            '                                            ' + moment(article.addTime).format("YYYY-MM-DD") +
                            '                                        </div>' +
                            '                                        <a class="btn border-draw-within mx-2 z-2" href="/Article/View/' + article.articleID + '">阅读更多</a>' +
                            '                                        <div class="mt-auto mb-2">' +
                            '                                            <i class="material-icons img-h-32">remove_red_eye</i>' +
                            '                                            ' + article.viewCount +
                            '                                        </div>' +
                            '                                    </div>' +
                            '                                    <a class="full-div-link z-1" href="/Article/View/' + article.articleID + '"></a>' +
                            '                                </div>' +
                            '                            </div>' +
                            '                        </div>'));


                    });
                } else {
                    $("#load-more").text('没有更多文章了');
                    $("#load-more").removeClass('btn-outline-primary');
                    $("#load-more").addClass('btn-outline-danger');

                }

                hideSpinner();
            }
        });

        
        var current_page = 1;
        $("#load-more").on('click', function (event) {
            $("#load-more").hide();
            document.getElementById('loadmore_spinner').style.display = 'block';
            event.stopPropagation();
            event.stopImmediatePropagation();
            ++current_page;
            $.ajax({
                url: '/Home/AjaxIndex',
                type: 'post',
                data:
                {
                    page: current_page
                },
                success: function (data) {
                    if (data && data.length) {
                        data.forEach(function (article) {
                            var tags = '';
                            var tag_html = '';
                            if (article.tag) {
                                tags = article.tag.split(',');
                                tags.forEach(function (tag) {
                                    tag_html += '<a class="btn btn-outline-light px-1 py-1 mt-n2 mb-3 position-relative z-2" href="/Tag/List?name=' + tag + '">' + tag + '</a>'
                                });
                            }
                            $('.grid').colcade('append', $.parseHTML('<div class="grid-item">' +
                                '                            <div class="card card-article my-3 mx-auto mx-md-3 border-0 animated--shadow-translate">' +
                                '                                <img class="card-img-top darken-20 text-economica" src="' + img_upload_path + article.title.replace('\\"','"') + '/' + article.headerImg + '" alt="' + article.title + ' Header Image">' +
                                '                                <div class="card-img-overlay">' +
                                tag_html +
                                '                                </div>' +
                                '                                <div class="card-body d-flex flex-column">' +
                                '                                    <div class="d-flex">' +
                                '                                        <h4 class="mx-auto text-economica">' + article.title + '</h4>' +
                                '                                    </div>' +
                                '                                    <div class="text-titillium d-flex my-3 flex-wrap flex-grow-1 text-break">' +
                                '                                        ' + article.content + '...' +
                                '                                    </div>' +
                                '                                    <div class="d-flex align-self-center text-economica justify-content-between">' +
                                '' +
                                '                                        <div class="mt-auto mb-2">' +
                                '                                            ' + moment(article.addTime).format("YYYY-MM-DD") +
                                '                                        </div>' +
                                '                                        <a class="btn border-draw-within mx-2 z-2" href="/Article/View/' + article.articleID + '">阅读更多</a>' +
                                '                                        <div class="mt-auto mb-2">' +
                                '                                            <i class="material-icons img-h-32">remove_red_eye</i>' +
                                '                                            ' + article.viewCount +
                                '                                        </div>' +
                                '                                    </div>' +
                                '                                    <a class="full-div-link z-1" href="/Article/View/' + article.articleID + '"></a>' +
                                '                                </div>' +
                                '                            </div>' +
                                '                        </div>'));
                        });
                    } else {
                        $("#load-more").text('没有更多文章了');
                        $("#load-more").removeClass('btn-outline-primary');
                        $("#load-more").addClass('btn-outline-danger');
                    }
                    $("#load-more").show();
                    hideLoadMoreSpinner();
                },
                error: function (data) {
                    --current_page;
                    toast('加载更多文章失败', 'danger');
                }
            });
            
        });

    });