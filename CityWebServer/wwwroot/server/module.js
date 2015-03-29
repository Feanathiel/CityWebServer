'use strict';

define([
    'angular',
    'angularRoute',
    'base/services/Navigation'
], function(angular, angularRoute) {

    var module = angular.module('serverModule', [
        'ngRoute',
        'baseModule'
    ]);

    module.config([
        '$routeProvider',
        'navigationProvider',
        function ($routeProvider, navigationProvider) {
            $routeProvider.
                when('/', {
                    templateUrl: 'server/views/home.html',
                    controller: 'HomeCtrl'
                })
                .when('/log/', {
                    templateUrl: 'server/views/log.html',
                    controller: 'LogCtrl'
                });

            navigationProvider.register('Home', '/');
            navigationProvider.register('Log', '/log/');
        }
    ]);

    return module;
});