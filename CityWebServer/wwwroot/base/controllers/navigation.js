'use strict';

define([
    'base/module',
    'base/services/Navigation'
], function (baseModule) {
    baseModule.controller('NavCtrl', function ($scope, $http, navigation) {

        $scope.Links = navigation();

        $http.get('Api/Server/CityName.json').success(function (data) {
            $scope.CityName = data.CityName;
        });
    });
});
