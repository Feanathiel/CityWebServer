'use strict';

define([
    'citizen/module'
], function (citizenModule) {
    citizenModule.controller('EducationEmploymentCtrl', function ($scope, Citizen, $interval) {

        var categories = [
            'Uneducated',
            'Elementary',
            'High school',
            'University'
        ];

        var chartConfig = {
            title: {
                text: 'Employment per education level'
            },
            xAxis: {
                categories: categories

            },
            yAxis: [
                {
                    min: 0,
                    title: {
                        text: 'Employees'
                    }
                }
            ],
            series: [
                {
                    name: 'Total',
                    color: 'rgba(126,86,134,.9)',
                    data: [],
                    pointPadding: 0.3
                },
                {
                    name: 'Employed',
                    color: 'rgba(165,170,217,1)',
                    data: [],
                    pointPadding: 0.4
                }
            ],

            options: {
                chart: {
                    type: 'column',
                    animation: {
                        duration: 0
                    }
                },
                tooltip: {
                    shared: true
                },
                plotOptions: {
                    column: {
                        grouping: false,
                        shadow: false,
                        borderWidth: 0
                    }
                }
            }
        };

        var totalSeries = chartConfig.series[0];
        var employedSeries = chartConfig.series[1];

        this.loadEducationEmploymentRate = function () {

            Citizen.getEducationEmploymentRate().then(function (responseData) {
                var data = responseData.data;

                var employment = data.EducationEmployment;

                totalSeries.data = [
                    employment.Uneducated.Employed + employment.Uneducated.Unemployed,
                    employment.Elementary.Employed + employment.Elementary.Unemployed,
                    employment.HighSchool.Employed + employment.HighSchool.Unemployed,
                    employment.University.Employed + employment.University.Unemployed
                ];

                employedSeries.data = [
                    employment.Uneducated.Employed,
                    employment.Elementary.Employed,
                    employment.HighSchool.Employed,
                    employment.University.Employed
                ];
            });
        }

        var promise = $interval(function () {
            this.loadEducationEmploymentRate();
        }.bind(this), 2000);

        this.loadEducationEmploymentRate();

        $scope.$on('$destroy', function () {
            $interval.cancel(promise);
        });

        $scope.educationEmploymentChartConfig = chartConfig;
    });
});
