'use strict';

define([
    'angular',
    'angularRoute',
    'angularHighcharts',
    'base/services/Navigation'
], function (angular, angularRoute) {

    var module = angular.module('citizenModule', [
        'ngRoute',
        'highcharts-ng',
        'baseModule'
    ]);

    module.config([
        '$routeProvider',
        'navigationProvider',
        function ($routeProvider, navigationProvider) {
            $routeProvider.
                when('/citizen/', {
                    templateUrl: 'citizen/views/home.html',
                    controller: 'CitizenCtrl'
                });

            navigationProvider.register('Citizen', '/citizen/');
        }
    ]);

    return module;
});
