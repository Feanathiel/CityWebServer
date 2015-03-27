'use strict';

require.config({
    paths: {
        angular: 'factory/angular/angular',
        angularRoute: 'factory/angular/angular-route'
    },
    shim: {
        'angular': { 'exports': 'angular' },
        'angularRoute': {'exports': 'angular'}
    }
});

var dependencies = [
    'angular',
    'app',
    'base/controllers/navigation'
];

var bootDependencies = dependencies;

require(
    bootDependencies,
    function() {
        var $html = angular.element(document.getElementsByTagName('html')[0]);
        $html.ready(function() {
            angular.bootstrap(document, ['app']);
        });
    });
