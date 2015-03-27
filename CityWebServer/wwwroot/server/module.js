﻿'use strict';

define([
    'angular',
    'angularRoute'
], function(angular, angularRoute) {

    var module = angular.module('serverModule', [
        'ngRoute'
    ]);

    module.config([
        '$routeProvider',
        function($routeProvider) {
            $routeProvider.
                when('/', {
                    templateUrl: 'server/views/home.html',
                    controller: 'HomeCtrl'
                });
        }
    ]);

    return module;
});