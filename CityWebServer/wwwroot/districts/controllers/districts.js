'use strict';

define([
    'districts/module'
], function (districtsModule) {
    districtsModule.controller('DistrictsCtrl', function ($scope, Districts, $interval) {
        var lastTime = 0;
        var rs = [];

        var chartConfig = {
            title: {
                text: 'Statistics'
            },
            xAxis: {
                type: 'datetime',
                tickPixelInterval: 150
            },
            yAxis: {
                minPadding: 0.2,
                maxPadding: 0.2,
                title: {
                    text: 'Value',
                    margin: 80
                }
            },
            series: rs,
            options: {
                chart: {
                    defaultSeriesType: 'spline',
                    animation: {
                        duration: 0
                    }
                }
            }
        };

        var keepNLast = function (data, num) {
            var keep = Math.max(data.length - num, 0);

            if (keep > 0) {
                data.splice(0, keep);
            }
        }

        function addOrUpdateSeries(seriesName, value, valueName) {
            var series;
            var matchFound = false;
            if (rs.length > 0) {
                for (var s = 0; s < rs.length; s++) {
                    if (rs[s].name == seriesName) {
                        series = rs[s];
                        matchFound = true;
                        s = rs.length; // Stop looping
                    }
                }
            }

            if (!matchFound) {
                var seriesOptions = {
                    id: seriesName,
                    name: seriesName,
                    data: [{ x: valueName, y: value }]
                };

                rs.push(seriesOptions);
            }
            else {
                series.data.push({ x: valueName, y: value });
                keepNLast(series.data, 20);
            }
        }

        function updateChart(vm) {
            var updatedSeries = [];
            var districts = vm.Districts;

            for (var i = 0; i < districts.length; i++) {
                var district = districts[i];

                var seriesName = district.DistrictName + " - Population";
                var population = district.TotalPopulationCount;

                addOrUpdateSeries(seriesName, population, Date.parse(vm.Time));
                updatedSeries.push(seriesName);
            }
        }

        this.loadDistricts = function () {
            Districts.getDistricts().then(function (data) {
                $scope.data = data.data;

                var date = Date.parse($scope.data.Time);

                if (lastTime !== date) {
                    updateChart($scope.data);

                    lastTime = date;
                }
            });
        };

        var promise = $interval(function () {
            this.loadDistricts();
        }.bind(this), 2000);

        this.loadDistricts();

        $scope.$on('$destroy', function () {
            $interval.cancel(promise);
        });

        $scope.chartConfig = chartConfig;
    });
});


