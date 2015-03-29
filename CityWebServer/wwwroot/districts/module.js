'use strict';

define([
    'angular',
    'angularRoute',
    'angularHighcharts',
    'base/services/Navigation'
], function(angular, angularRoute) {

    var module = angular.module('districtsModule', [
        'ngRoute',
        'highcharts-ng',
        'baseModule'
    ]);

    module.config([
        '$routeProvider',
        'navigationProvider',
        function ($routeProvider, navigationProvider) {
            $routeProvider.
                when('/districts/', {
                    templateUrl: 'districts/views/districts.html',
                    controller: 'DistrictsCtrl'
                });

            navigationProvider.register('Districts', '/districts/');
        }
    ]);

    return module;
});
