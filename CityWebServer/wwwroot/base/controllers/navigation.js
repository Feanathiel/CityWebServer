﻿'use strict';

define([
    'app',
    'base/services/Navigation'
], function (app) {
    app.controller('NavCtrl', function ($scope, $http, Navigation) {

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
