$().ready(function(){

    // you can use own color converting function if you want
    var my_color = d3.scaleOrdinal(d3.schemeCategory10);
    var href_func = function(d){ return "/Tag/List?name=" + d.text }

    // maketextCloud(data, css selector that you wanna insert in, scale of svg, class name of svg, font-family, rotate or not, your color converting function)
    var width = document.getElementById('tag-card').offsetWidth;
    makeWordCloud(data, href_func, ".card-body", width, 300, "tag-cloud-img", "Impact", false, my_color)
    
    // [ svg class, font-family, rotate texts or not, color function ] are optional.
    // the simplest way => window.makeWordCloud(data, "body", 500)

})


