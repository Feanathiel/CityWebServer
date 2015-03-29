'use strict';

define([
        'angular',
        'angularRoute',
        'base/index',
        /* Need to load these dynamically somehow */
        'server/index',
        'districts/index',
        'citizen/index'
    ],
    function (angular, angularRoute) {

        var app = angular.module('app', [
            'ngRoute',
            'baseModule',
            /* Need to load these dynamically somehow */
            'serverModule',
            'districtsModule',
            'citizenModule'
        ]);

        app.config([
            '$routeProvider',
            function($routeProvider) {
                $routeProvider.otherwise('/');
            }
        ]);

        return app;
    });