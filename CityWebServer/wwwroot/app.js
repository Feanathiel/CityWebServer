'use strict';

define([
        'angular',
        'angularRoute',
        /* Need to load these dynamically somehow */
        'server/index',
        'districts/index',
        'citizen/index'
    ],
    function (angular, angularRoute) {

        var app = angular.module('app', [
            'ngRoute',
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