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
    var count = parseInt(this.getAttribute('count'),10);
    if (!hasLiked(articleID)) {
        //register the like in your system
        //...
        //

        $.ajax({
            url: '/Ajax/AjaxLikeArticle',
            type: 'post',
            data: 
            {
                articleID : articleID
            },
            success: function(data) {
                button.innerText="点赞 "+(count+1);
            }
            });

        setLike(articleID);
    } else {
        alert('只能点一次赞哦!');
    }
}

