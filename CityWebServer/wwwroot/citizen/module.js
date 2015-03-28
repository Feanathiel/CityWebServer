'use strict';

define([
    'angular',
    'angularRoute',
    'angularHighcharts'
], function (angular, angularRoute) {

    var module = angular.module('citizenModule', [
        'ngRoute',
        'highcharts-ng'
    ]);

    module.config([
        '$routeProvider',
        function ($routeProvider) {
            $routeProvider.
                when('/citizen/', {
                    templateUrl: 'citizen/views/home.html',
                    controller: 'CitizenCtrl'
                });
        }
    ]);

    return module;
});
