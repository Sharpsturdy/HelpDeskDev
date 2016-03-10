require([
                // Require the basic chart class
                "dojox/charting/Chart",
                "dojox/charting/widget/Chart2D",

                // Require the theme of our choosing
                "dojox/charting/themes/PlotKit/blue",

                // Charting plugins:

                // Load the Legend widget class
                "dojox/charting/widget/Legend",

                // 	We want to plot Columns
                "dojox/charting/plot2d/Columns",

                //	We want to use Markers
                "dojox/charting/plot2d/Markers",

                //	We'll use default x/y axes
                "dojox/charting/axis2d/Default",

                // Wait until the DOM is ready
                "dojo/domReady!"
], function (Chart, Chart2D, theme, Legend) {
       
    // Create the chart within it's "holding" node
    var chart = new Chart("chart1");

    
    chart.setTheme(theme) // Set the theme
    .addPlot("default", { // Add the only/default plot
        type: "Columns",
        markers: true,
        tension: 3,
        gap: 5,
    })
    .addSeries("Monthly Tickets By Status", chartData[0]) // Add the series of data
    .addAxis("y", { vertical: true, fixLower: "minor", fixUpper: "major" });

    // Add axes
    //chart.addAxis("x");
    chart.addAxis("x", {max:7,  labels:
        [{value:1, text:"Submitted"}, {value:2, text:"Accepted"}, {value:3, text:"Assigned"},
        {value:4, text:"Returned"},{value:5, text:"Completed"}, {value:6, text:"Rejected"}, {value:7, text:"Deleted"}]
    });

    //Highlight!
    new dojox.charting.action2d.Highlight(chart, "default");

    /*,  {
text : function(o) {
return "<nobr><strong>" + o.run.name + " [" + o.y + "]</strong></nobr>";
    //+ tooltips[o.run.name][o.x-1];
        //dojo.currency.format(o.y, {currency : "USD", places : 0}));
}
} );*/
    // Render the chart!
    chart.render();

    //Add legend
    var legend = new Legend({ chart: chart, horizontal: true }, "legend1");

    //Tooltip
    var tip = new dojox.charting.action2d.Tooltip(chart, "default");

    var chart2 = new Chart("chart2");
    chart2.setTheme(theme)
    .addPlot("default", {
        type: "Columns",
        markers: true,
        tension: 3,
        gap: 5,
    })
    .addAxis("x", { labels: chart2Labels })
    .addSeries("Monthly Tickets By Originator", chart2Data[0]) // Add the series of data
    .addAxis("y", { vertical: true, fixUpper: "major" });
    //Highlight!
    new dojox.charting.action2d.Highlight(chart2, "default");

    // Render the chart!
    chart2.render();

    //Add legend
    var legend = new Legend({ chart: chart2, horizontal: true }, "legend2");

    //Tooltip
    var tip = new dojox.charting.action2d.Tooltip(chart2, "default");
});
