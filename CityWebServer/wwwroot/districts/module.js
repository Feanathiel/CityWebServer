'use strict';

define([
    'angular',
    'angularRoute'
], function(angular, angularRoute) {

    var module = angular.module('districtsModule', [
        'ngRoute'
    ]);

    module.config([
        '$routeProvider',
        function($routeProvider) {
            $routeProvider.
                when('/districts/', {
                    templateUrl: 'districts/views/districts.html',
                    controller: 'DistrictsCtrl'
                });
        }
    ]);

    return module;
});
