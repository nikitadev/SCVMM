$(function () {

    var type = {
        cpu: { value: 0, name: "CPU", code: "c" },
        network: { value: 1, name: "Network", code: "n" },
        storage: { value: 2, name: "Storage", code: "s" }
    };

    var vmName = $("#vmName").val(),
        contentCPU = $('#contentCPU'),
        contentNetwork = $('#contentNetwork'),
        contentStorage = $('#contentStorage'),
        containerPlotCPU = $("#placeholderCPU"),
        containerPlotNetwork = $("#placeholderNetwork"),
        containerPlotStorage = $("#placeholderStorage");

    var plotCPU = $.plot(containerPlotCPU, [[0,0]]),
        plotNetwork = $.plot(containerPlotNetwork, [[0,0]]),
        plotStorage = $.plot(containerPlotStorage, [[0,0]]);

    $("#filterCPU").click(function () {
        
        contentCPU.css("visibility", "hidden");

        var timeframe = $('#timeframeCPU option:selected').val();
        proxyHub.server.refresh(vmName, 0, timeframe);
    });

    $("#filterNetwork").click(function () {
        
        contentNetwork.css("visibility", "hidden");

        var timeframe = $('#timeframeNetwork option:selected').val();
        proxyHub.server.refresh(vmName, 1, timeframe);
    });

    $("#filterStorage").click(function () {
        
        contentStorage.css("visibility", "hidden");

        var timeframe = $('#timeframeStorage option:selected').val();
        proxyHub.server.refresh(vmName, 2, timeframe);
    });

    function initData(numbers) {
        var totalPoints = numbers.length;

        var dataValues = [],
            dataDate = [];

        for (var i = 0; i < totalPoints; ++i) {
            var n = numbers[i].Value;
            var t = new Date(Date.parse(numbers[i].Timestamp));

            dataValues.push(n);
            dataDate.push(t);
        }            

        return getZipPoints(dataDate, dataValues);
    }
    
    function getZipPoints(dates, values) {
        var res = [];
        for (var i = 0; i < values.length; ++i) {

            res.push([dates[i], values[i]]);
            //res.push([i, dataValues[i]]);
        }

        return res;
    }

    //function getData(y, dates, values) {

    //    if (data.length > 0) {
    //        values = values.slice(1);
    //        dates = dates.slice(1);
    //    }

    //    values.push(y.Value);
    //    dates.push(y.Timestamp);

    //    return getZipPoints(dates, values);
    //}

    // Proxy created on the fly 
    var proxyHub = $.connection.chartHub;

    proxyHub.client.init = function (typeFlot, numbers) {
        
        if (typeFlot == type.cpu.value) {
            
            plotCPU = createPlot(containerPlotCPU, type.cpu.name, numbers);

            for (var i = 0; i < numbers.length; ++i) {

                var n = numbers[i].Value;
                var dt = new Date(Date.parse(numbers[i].Timestamp));

                var d = [dt.getDate(), dt.getMonth() + 1, dt.getFullYear()];
                var t = [dt.getHours(), dt.getMinutes(), dt.getSeconds()];
                var dateStr = d.join("/") + " " + t.join(":");
            }
            
            contentCPU.css("visibility", "visible");
        } else if (typeFlot == type.network.value) {
            
            plotNetwork = createPlot(containerPlotNetwork, type.network.name, numbers);

            for (var i = 0; i < numbers.length; ++i) {

                var n = numbers[i].Value;
                var dt = new Date(Date.parse(numbers[i].Timestamp));

                var d = [dt.getDate(), dt.getMonth() + 1, dt.getFullYear()];
                var t = [dt.getHours(), dt.getMinutes(), dt.getSeconds()];
                var dateStr = d.join("/") + " " + t.join(":");
            }
            
            contentNetwork.css("visibility", "visible");
        } else if (typeFlot == type.storage.value) {

            plotStorage = createPlot(containerPlotStorage, type.storage.name, numbers);

            for (var i = 0; i < numbers.length; ++i) {

                var n = numbers[i].Value;
                var dt = new Date(Date.parse(numbers[i].Timestamp));

                var d = [dt.getDate(), dt.getMonth() + 1, dt.getFullYear()];
                var t = [dt.getHours(), dt.getMinutes(), dt.getSeconds()];
                var dateStr = d.join("/") + " " + t.join(":");
            }
            
            contentStorage.css("visibility", "visible");
        }
        
        //proxyHub.server.runDraw(vmName, typeFlot);
    };

    // Declare a function which the server can invoke 
    proxyHub.client.updateChart = function (typeFlot, number) {
        update(typeFlot, number);
    };

    // Start the connection.
    $.connection.hub.start().done(function () {
        proxyHub.server.refresh(vmName, type.cpu.value, 0);
        proxyHub.server.refresh(vmName, type.network.value, 0);
        proxyHub.server.refresh(vmName, type.storage.value, 0);
    });

    function createPlot(container, titleX, numbers) {
        var series = [{
            //stack: true,
            data: initData(numbers),
            shadowSize: 0, // Drawing is faster without shadows
            //bars: { show: true }
            lines: {
                show: true
            }
        }];

        var plot = $.plot(container, series, {
            grid: {
                hoverable: true,
                autoHighlight: false,
                borderWidth: 1,
                minBorderMargin: 20,
                labelMargin: 10,
                backgroundColor: {
                    colors: ["#fff", "#e4f4f4"]
                },
                margin: {
                    top: 8,
                    bottom: 20,
                    left: 20
                },
            },
            crosshair: {
                mode: "x"
            },
            xaxis: {
                mode: "time",
                timeformat: "%H:%M"
            },
            yaxis: {
                //min: 0,
                //max: 100,
                //tickSize: 30,
                //tickFormatter: function (v) {
                //    return v + " " + meashure;
                //}
            }
            //legend: {
            //    show: true
            //},
            //zoom: {
            //    interactive: true
            //},
            //pan: {
            //    interactive: true
            //}
        });

        var yaxisLabel = $("<div class='axisLabel yaxisLabel'></div>")
            .appendTo(container);

        yaxisLabel.css("margin-top", yaxisLabel.width() / 2 - 20);

        return plot;
    }  

    //function update(typeFlot, y) {

    //    if (typeFlot == type.cpu.value) {
    //        plotCPU.setData([getData(y)]);
    //        plotCPU.draw();
    //    } else if (typeFlot == type.network.value) {
    //        plotNetwork.setData([getData(y)]);
    //        plotNetwork.draw();
    //    } else if (typeFlot == type.storage.value) {
    //        plotStorage.setData([getData(y)]);
    //        plotStorage.draw();
    //    }
    //}
});