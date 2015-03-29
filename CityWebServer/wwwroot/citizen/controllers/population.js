'use strict';

define([
    'citizen/module'
], function (citizenModule) {
    citizenModule.controller('PopulationCtrl', function ($scope, Citizen, $interval) {
        var lastTime = 0;

        var chartConfig = {
            title: {
                text: 'Population'
            },
            xAxis: {
                type: 'datetime'
            },
            yAxis: {
                title: {
                    text: 'Total'
                }
            },
            series: [
                {
                    name: 'Population',
                    type: 'area',
                    pointInterval: 24 * 3600 * 1000,
                    pointStart: Date.UTC(2006, 0, 1),
                    data: [],
                    color: '#7FB2F0'
                }
            ],
           
            options: {
                chart: {
                    animation: {
                        duration: 0
                    }
                },
                plotOptions: {
                    area: {
                        fillColor: {
                            linearGradient: { x1: 0, y1: 0, x2: 0, y2: 1 },
                            stops: [
                                [0, Highcharts.getOptions().colors[0]],
                                [1, Highcharts.Color(Highcharts.getOptions().colors[0]).setOpacity(0).get('rgba')]
                            ]
                        },
                        lineWidth: 1,
                        states: {
                            hover: {
                                lineWidth: 1
                            }
                        },
                        threshold: null
                    }
                },
                legend: {
                    enabled: false
                }
            }
        };

        var populationSeries = chartConfig.series[0];

        var keepNLast = function (data, num) {
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

                    var total = ageDistribution.Seniors +
                        ageDistribution.Adults +
                        ageDistribution.YoungAdults +
                        ageDistribution.Teens +
                        ageDistribution.Children;

                    populationSeries.data.push({ x: date, y: total });

                    keepNLast(populationSeries.data, 7 * 13);

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

        $scope.populationChartConfig = chartConfig;
    });
});
