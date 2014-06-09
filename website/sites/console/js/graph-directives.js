// AngularJS+D3 graph directives


// An angular module that injects D3.

angular.module('d3', [])
    .factory('d3Service', [function() {
        return d3;
    }]);

// An angular module that provides graph directives

var d3Graph = angular.module("d3Graph", ["d3"]);

d3Graph.directive("qsD3Graph", ["d3Service", function(d3Service) {

    var d3 = d3Service;

    var link = function(scope, element, attrs) {

        var yLabel = attrs.ylabel || "";
        var warnLevel = parseInt(attrs.warnlevel) || 0;
        var warnLabel = attrs.warnlabel;

        var warnLine = null;
        if(warnLabel) {
            warnLine = {
                lineValue: warnLevel,
                label: warnLabel
            };
        }

        scope.$watch('data', function(newData, oldData) {
            if(!newData) {
                return;
            }
            if(newData.length == 0) {
                return;
            }
            drawLineGraph(element[0], newData, yLabel, warnLine);
        }, false);
    }
    
    var drawLineGraph = function(element, data, yLabel, warnLine) {

        d3.select(element).select('*').remove();

        var svg = d3.select(element).append("svg")
            .attr("class", "graph queue");

        var margin = { top: 50, left: 50, right: 50, bottom: 50 };
        
        var height = +svg.style("height").replace("px", "") - margin.top - margin.bottom;
        var width = +svg.style("width").replace("px", "") - margin.left - margin.right;

        var xDomain = d3.extent(data, function(d) { return d[0]; })
        var yDomain = d3.extent(data, function(d) { return d[1]; });

        var xScale = d3.time.scale().range([0, width]).domain(xDomain);
        var yScale = d3.scale.linear().range([height, 0]).domain(yDomain);

        var xAxis = d3.svg.axis().scale(xScale).orient('bottom');
        var yAxis = d3.svg.axis().scale(yScale).orient('left');

        var line = d3.svg.line()
            .x(function(d) { return xScale(d[0]); })
            .y(function(d) { return yScale(d[1]); });

        var area = d3.svg.area()
            .x(function(d) { return xScale(d[0]); })
            .y0(function(d) { return yScale(d[1]); })
            .y1(height);

        var g = svg.append('g').attr('transform', 'translate(' + margin.left + ', ' + margin.top + ')');

        g.append('path')
            .datum(data)
            .attr('class', 'area')
            .attr('d', area);

        g.append('g')
            .attr('class', 'x axis')
            .attr('transform', 'translate(0, ' + height + ')')
            .call(xAxis);

        g.append('g')
            .attr('class', 'y axis')
            .call(yAxis)
            .append('text')
                .attr('class', 'axis')
                .attr('transform', 'rotate(-90)')
                .attr('y', 6)
                .attr('dy', '.71em')
                .attr('text-anchor', 'end')
                .text(yLabel);

        g.append('path')
            .datum(data)
            .attr('class', 'line')
            .attr('d', line);

        g.selectAll('circle').data(data).enter().append('circle')
            .attr('cx', function(d) { return xScale(d[0]); })
            .attr('cy', function(d) { return yScale(d[1]); })
            .attr('r', 5)
            .attr('class', 'circle');

        // focus tracking

        var focus = g.append('g').style('display', 'none');
            
        focus.append('line')
            .attr('id', 'focusLineX')
            .attr('class', 'focusLine');
        focus.append('line')
            .attr('id', 'focusLineY')
            .attr('class', 'focusLine');
        focus.append('circle')
            .attr('id', 'focusCircle')
            .attr('r', 5)
            .attr('class', 'circle focusCircle');

        var bisectDate = d3.bisector(function(d) { return d[0]; }).left;

        g.append('rect')
            .attr('class', 'overlay')
            .attr('width', width)
            .attr('height', height)
            .on('mouseover', function() { focus.style('display', null); })
            .on('mouseout', function() { focus.style('display', 'none'); })
            .on('mousemove', function() { 
                if(data.length == 0) {
                    return;
                }

                var mouse = d3.mouse(this);
                var mouseDate = xScale.invert(mouse[0]);
                var i = bisectDate(data, mouseDate); // returns the index to the current data item

                var d0 = data[i - 1]
                var d1 = data[i];
                var d = 0;
                // work out which date value is closest to the mouse
                if(d0 && d1) {
                    d = mouseDate - d0[0] > d1[0] - mouseDate ? d1 : d0;
                } else {
                    d = d0 ? d0 : d1;
                }

                var x = xScale(d[0]);
                var y = yScale(d[1]);

                focus.select('#focusCircle')
                    .attr('cx', x)
                    .attr('cy', y);
                focus.select('#focusLineX')
                    .attr('x1', x).attr('y1', yScale(yDomain[0]))
                    .attr('x2', x).attr('y2', yScale(yDomain[1]));
                focus.select('#focusLineY')
                    .attr('x1', xScale(xDomain[0])).attr('y1', y)
                    .attr('x2', xScale(xDomain[1])).attr('y2', y);
            });

        // warn line

        if(warnLine && yDomain[0] < warnLine.lineValue && yDomain[1] > warnLine.lineValue) {
            g.append('line')
                .attr('x1', xScale(xDomain[0]))
                .attr('y1', yScale(warnLine.lineValue))
                .attr('x2', xScale(xDomain[1]))
                .attr('y2', yScale(warnLine.lineValue))
                .attr('class', 'zeroline');
            g.append('text')
                .attr('x', xScale(xDomain[1]))
                .attr('y', yScale(warnLine.lineValue))
                .attr('dy', '1em')
                .attr('text-anchor', 'end')
                .text(warnLine.label)
                .attr('class', 'axis zerolinetext');
        }
    };


    return {
        restrict: 'AE',
        scope: {
            data: '='
        },
        link: link
    };
}]);
