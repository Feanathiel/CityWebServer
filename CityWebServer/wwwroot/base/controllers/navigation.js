'use strict';

define(['app'], function (app) {
    app.controller('NavCtrl', function ($scope, $http) {
        $http.get('base/data/navigation.json').success(function (data) {
            $scope.CityName = data.CityName;
            $scope.Links = data.Links;
        });
    });
});
