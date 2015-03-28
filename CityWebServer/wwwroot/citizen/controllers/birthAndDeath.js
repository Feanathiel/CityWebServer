'use strict';

define([
    'citizen/module'
], function (citizenModule) {
    citizenModule.controller('BirthAndDeathCtrl', function ($scope, Citizen, $interval) {

        var chartConfig = {
            title: {
                text: 'Birth & death'
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
                    name: 'Birth',
                    type: 'area',
                    pointInterval: 24 * 3600 * 1000,
                    pointStart: Date.UTC(2006, 0, 1),
                    data: [],
                    color: '#90ED7D'
                },
                {
                    name: 'Death',
                    type: 'area',
                    pointInterval: 24 * 3600 * 1000,
                    pointStart: Date.UTC(2006, 0, 1),
                    data: [],
                    color: '#BA3C3D'
                }
            ],
           
            options: {
                chart: {
                    animation: {
                        duration: 0
                    }
                },
                tooltip: {
                    pointFormat: '<span style="color:{series.color}">{series.name}</span>: <b>{point.y:,.0f}</b><br/>',
                    shared: true
                }
            }
        };

        var birthSeries = chartConfig.series[0];
        var deathSeries = chartConfig.series[1];

        var keepNLast = function (data, num) {
            var keep = Math.max(data.length - num, 0);

            if (keep > 0) {
                data.splice(0, keep);
            }
        }

        this.loadPopulation = function () {

            Citizen.getBirthAndDeathRate().then(function (responseData) {
                var data = responseData.data;

                var date = data.GameTime;
                var rate = data.Rate;

                birthSeries.data.push({ name: date, y: rate.Birth });
                deathSeries.data.push({ name: date, y: rate.Death });

                keepNLast(birthSeries.data, 7 * 13);
                keepNLast(deathSeries.data, 7 * 13);
            });
        }

        var promise = $interval(function () {
            this.loadPopulation();
        }.bind(this), 2000);

        this.loadPopulation();

        $scope.$on('$destroy', function () {
            $interval.cancel(promise);
        });

        $scope.birthAndDeathChartConfig = chartConfig;
    });
});
