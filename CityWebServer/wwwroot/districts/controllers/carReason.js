'use strict';

define([
    'districts/module'
], function (districtsModule) {
    districtsModule.controller('CarReasonCtrl', function ($scope, Districts, $interval) {

        var typeData = [];
        var typeCarData = [];

        var chartConfig = {
            title: {
                text: 'Statistics'
            },
            yAxis: {
                title: {
                    text: 'Cars'
                }
            },
            series: [
                {
                    name: 'Types',
                    data: typeData,
                    size: '60%',
                    dataLabels: {
                        formatter: function() {
                            return this.y > 5 ? this.point.name : null;
                        },
                        distance: -30
                    }
                },
                {
                    name: 'Subtypes',
                    data: typeCarData,
                    size: '80%',
                    innerSize: '60%',
                    dataLabels: {
                        formatter: function() {
                            return this.y >= 3 ? '<b>' + this.point.name + ':</b> ' + Math.round(this.y * 100) / 100 + '%' : null;
                        }
                    }
                }
            ],
            options: {
                chart: {
                    type: 'pie',
                    animation: {
                        duration: 0
                    }
                },
                plotOptions: {
                    pie: {
                        shadow: false,
                        center: ['50%', '50%']
                    }
                }
            }
        };
        
        this.loadDistricts = function () {
            Districts.getCarReasons().then(function (response) {
                var data = response.data.Categories;

                var colors = Highcharts.getOptions().colors;
                var colorIdx = 0;

                var items = [];

                for (var categoryIdx in data) {
                    var categoryItem = data[categoryIdx];

                    var reasonNames = [];
                    var reasonValues = [];

                    for (var reasonIdx in categoryItem.Reasons) {
                        var reasonItem = categoryItem.Reasons[reasonIdx];

                        reasonNames.push(reasonItem.Reason);
                        reasonValues.push(reasonItem.Percentage);
                    }

                    var item = {
                        y: categoryItem.Percentage,
                        color: colors[colorIdx],
                        drilldown: {
                            name: data[categoryIdx].Category,
                            categories: reasonNames,
                            data: reasonValues,
                            color: colors[colorIdx]
                        }
                    };

                    items.push(item);

                    colorIdx ++;
                }

                typeData.length = 0;
                typeCarData.length = 0;

                for (var i = 0; i < items.length; i += 1) {

                    // add browser data
                    typeData.push({
                        name: items[i].drilldown.name,
                        y: items[i].y,
                        color: items[i].color
                    });

                    // add version data
                    var drillDataLen = items[i].drilldown.data.length;
                    for (var j = 0; j < drillDataLen; j++) {
                        var brightness = 0.2 - (j / drillDataLen) / 5;
                        typeCarData.push({
                            name: items[i].drilldown.categories[j],
                            y: items[i].drilldown.data[j],
                            color: Highcharts.Color(items[i].color).brighten(brightness).get()
                        });
                    }
                }

                if (false);
            });
        };

        
        var promise = $interval(function () {
            this.loadDistricts();
        }.bind(this), 2000);

        this.loadDistricts();

        $scope.$on('$destroy', function () {
            $interval.cancel(promise);
        });

        $scope.carReasonChartConfig = chartConfig;
    });
});


