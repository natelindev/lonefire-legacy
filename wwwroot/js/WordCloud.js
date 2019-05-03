function makeWordCloud(data, href_func, parent_elem, svg_x,svg_y, svg_class, font, rotate_word, my_colors){

      var color_converter = null
      if(my_colors){ color_converter = my_colors }else{ color_converter = d3.scale.category20() }

      function draw(words) {

        d3.select(parent_elem).append("svg")
            .attr("width", svg_x)
            .attr("height", svg_y)
            .attr("class", svg_class)
          .append("g")
            .attr("transform", "translate(" + svg_x / 2 + "," + svg_y / 2 + ")")
          .selectAll("a")
            .data(words)
          .enter()
          .append("a")
            .attr("xlink:href", href_func)
            .text(function(d) { return d.text; })
          .append("text")
            .style("font-size", function(d) { return d.size + "px"; })
            .style("font-family", font)
            .style("fill", function(d, i) { return color_converter(i); })
            .attr("text-anchor", "middle")
            .attr("transform", function(d) {
              return "translate(" + [d.x, d.y] + ")rotate(" + d.rotate + ")";
            })
            .text(function(d) { return d.text; })
      }

      if(svg_class){ d3.select("." + svg_class).remove() }
      else{ d3.select("svg").remove() }

      var data_max =  d3.max(data, function(d){ return d.TagCount } );
      var sizeScale = d3.scaleLinear().domain([0, data_max]).range([0, 1])

      data = data.map(function(d) {
        return {text: d.TagName, size: 10 + sizeScale(d.TagCount) * 90};
      })

      var layout = d3.layout.cloud().size([svg_x, svg_y])
        .words(data)
        .padding(5)
        .fontSize(function(d) { return d.size; })
        
      if(!rotate_word){ layout.rotate(function() { return ~~(Math.random() * 2) * getRandomInt(30,60); }) }
        
      layout
        .on("end", draw)
        .start();
  }
