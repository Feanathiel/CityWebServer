'use strict';

define([
    'districts/module'
], function (districtsModule) {
    districtsModule.controller('VitalsCtrl', function ($scope, Districts, $interval) {

        $scope.Vitals = {};
        $scope.Math = Math;

        this.loadData = function () {
            Districts.getVitals().then(function (data) {
                $scope.CapacityBased = data.data.CapacityBased;
                $scope.PercentageBased = data.data.PercentageBased;
            });
        };

        $scope.GetColorClass = function (item) {
            var percentage = item.Percentage;
            
            if (item.LowerIsBetter) {
                percentage = 100 - percentage;
            }

            if (percentage >= 55) {
                return 'success';
            } else if (percentage >= 45) {
                return 'warning';
            } else {
                return 'danger';
            }
        }

        var promise = $interval(function () {
            this.loadData();
        }.bind(this), 2000);

        this.loadData();

        $scope.$on('$destroy', function () {
            $interval.cancel(promise);
        });
    });
});


