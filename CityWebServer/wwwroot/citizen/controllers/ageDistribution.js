'use strict';

define([
    'citizen/module'
], function (citizenModule) {
    citizenModule.controller('AgeDistributionCtrl', function($scope, Citizen, $interval) {

        var lastTime = 0;

        var chartConfig = {
            title: {
                text: 'Age distribution'
            },
            xAxis: {
                type: 'datetime',
                minRange: 14 * 24 * 3600000 // fourteen days
            },
            yAxis: {
                title: {
                    text: 'Percent'
                }
            },
            series: [
                {
                    name: 'Seniors',
                    data: [],
                    color: '#ADD5F7'
                },
                {
                    name: 'Adults',
                    data: [],
                    color: '#7FB2F0'
                },
                {
                    name: 'Young adults',
                    data: [],
                    color: '#4E7AC7'
                },
                {
                    name: 'Teens',
                    data: [],
                    color: '#35478C'
                },
                {
                    name: 'Children',
                    data: [],
                    color: '#16193B'
                }
            ],
            options: {
                chart: {
                    type: 'area',
                    animation: {
                        duration: 0
                    }
                },
                plotOptions: {
                    area: {
                        stacking: 'percent',
                        lineColor: '#ffffff',
                        lineWidth: 1,
                        marker: {
                            lineWidth: 1,
                            lineColor: '#ffffff'
                        }
                    },
                    series: {
                        pointStart: Date.UTC(2010, 0, 1),
                        pointInterval: 24 * 3600 * 1000 // one day
                    }
                },
                tooltip: {
                    pointFormat: '<span style="color:{series.color}">{series.name}</span>: <b>{point.percentage:.1f}%</b> ({point.y:,.0f})<br/>',
                    shared: true
                }
            }
        };

        var seniorSeries = chartConfig.series[0];
        var adultSeries = chartConfig.series[1];
        var youngAdultSeries = chartConfig.series[2];
        var teenSeries = chartConfig.series[3];
        var childSeries = chartConfig.series[4];

        var keepNLast = function(data, num) {
            var keep = Math.max(data.length - num, 0);

            if (keep > 0) {
                data.splice(0, keep);
            }
        }

        this.loadPopulation = function () {

            Citizen.getAgeDistribution().then(function (responseData) {
                var data = responseData.data;

                var date = Date.parse(data.GameTime);

                if (lastTime !== date) {

                    var ageDistribution = data.Distribution;

                    seniorSeries.data.push({ x: date, y: ageDistribution.Seniors });
                    adultSeries.data.push({ x: date, y: ageDistribution.Adults });
                    youngAdultSeries.data.push({ x: date, y: ageDistribution.YoungAdults });
                    teenSeries.data.push({ x: date, y: ageDistribution.Teens });
                    childSeries.data.push({ x: date, y: ageDistribution.Children });

                    keepNLast(seniorSeries.data, 7 * 13);
                    keepNLast(adultSeries.data, 7 * 13);
                    keepNLast(youngAdultSeries.data, 7 * 13);
                    keepNLast(teenSeries.data, 7 * 13);
                    keepNLast(childSeries.data, 7 * 13);

                    lastTime = date;
                }
            });
        }

        var promise = $interval(function () {
            this.loadPopulation();
        }.bind(this), 2000);

        this.loadPopulation();

        $scope.$on('$destroy', function () {
            $interval.cancel(promise);
        });

        $scope.ageDistributionChartConfig = chartConfig;
    });
});
