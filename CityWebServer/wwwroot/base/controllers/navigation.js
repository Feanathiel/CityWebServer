'use strict';

define([
    'base/module',
    'base/services/Navigation'
], function (baseModule) {
    baseModule.controller('NavCtrl', function ($scope, $http, Navigation) {

        Navigation.register('Home', '/');
        Navigation.register('Log', '/log/');
        Navigation.register('Districts', '/districts/');
        Navigation.register('Citizen', '/citizen/');

        $scope.Links = Navigation.list();

        $http.get('base/data/navigation.json').success(function (data) {
            $scope.CityName = data.CityName;
        });
    });
});
