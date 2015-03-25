'use strict';

var districtsModule = angular.module('districtsModule', [
    'districtsControllers',
    'districtsServices'
]);

var districtsControllers = angular.module('districtsControllers', []);
var districtsServices = angular.module('districtsServices', ['ngResource']);

districtsModule.config([
    '$routeProvider',
    function ($routeProvider) {
        $routeProvider.
            when('/districts/', {
                templateUrl: 'districts/views/districts.html',
                controller: 'DistrictsCtrl'
            });
    }
]);
