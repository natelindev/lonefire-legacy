function setLike(articleID) {
    document.cookie=articleID+'=y';
}

function hasLiked(articleID) {
    var cookies=document.cookie.split(';');
    for (var i=0;i<cookies.length;i++) {
        var cookie=cookies[i].split('=');
        if (cookie[0].trim()==articleID) return true;
    }
    return false;
}

var button=document.getElementById('like');

button.onclick=function() {
    var articleID=this.getAttribute('articleID');
    var count = parseInt(this.getAttribute('count'), 10);
    var likeLabel = document.getElementById('like-label');
    if (!hasLiked(articleID)) {

        $.ajax({
            url: '/Article/AjaxLikeArticle',
            type: 'post',
            data: 
            {
                articleID : articleID
            },
            success: function(data) {
                likeLabel.innerHTML=count+1;
            }
        });

        setLike(articleID);
    } else {
        toast('只能点一次赞哦','danger');
    }
}

hljs.initHighlightingOnLoad();



$(document).ready(function () {
    var scrollDistance = $(this).scrollTop();
    var main_distance = $('.card-main').offset().top;

    if (scrollDistance > main_distance) {
        $('.sidebar').css('top', '0px');
        $('.sidebar').addClass('fixed');
    } else {
        $('.sidebar').css('top', main_distance + 'px');
        $('.sidebar').removeClass('fixed');
    }

    $('.sidebar').css('top', main_distance + 'px');
});