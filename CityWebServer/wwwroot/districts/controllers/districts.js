'use strict';

districtsModule.controller('DistrictsCtrl', function ($scope, Districts, $interval) {

    var chart = initializeChart($scope);

    this.loadDistricts = function () {
        Districts.getDistricts().then(function(data) {
            $scope.data = data.data;

            updateChart($scope.data, chart);
        });
    };

    $interval(function() {
        this.loadDistricts();
    }.bind(this), 2000);

    this.loadDistricts();
});
