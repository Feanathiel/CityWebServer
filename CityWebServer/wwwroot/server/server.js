'use strict';

var serverModule = angular.module('serverModule', [
    'serverControllers',
    'serverServices'
]);

var serverControllers = angular.module('serverControllers', []);
var serverServices = angular.module('serverServices', ['ngResource']);

serverModule.config([
    '$routeProvider',
    function($routeProvider) {
        $routeProvider.
            when('/', {
                templateUrl: 'server/views/home.html',
                controller: 'HomeCtrl'
            });
    }
]);
