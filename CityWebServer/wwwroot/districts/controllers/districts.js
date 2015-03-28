'use strict';

define([
    'districts/module',
    'districts/assets/js/script'
], function (districtsModule) {
    districtsModule.controller('DistrictsCtrl', function ($scope, Districts, $interval) {
        var chart = initializeChart($scope);

        this.loadDistricts = function () {
            Districts.getDistricts().then(function (data) {
                $scope.data = data.data;

                updateChart($scope.data, chart);
            });
        };

        var promise = $interval(function () {
            this.loadDistricts();
        }.bind(this), 2000);

        this.loadDistricts();

        $scope.$on('$destroy', function () {
            $interval.cancel(promise);
        });
    });
});


