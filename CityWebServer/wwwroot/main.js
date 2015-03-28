'use strict';

require.config({
    paths: {
        angular: 'factory/angular/angular',
        angularRoute: 'factory/angular/angular-route',
        jquery: "factory/jquery/jquery-2.1.3.min",
        highcharts: "factory/highcharts/highcharts",
        angularHighcharts: "factory/highcharts-ng/highcharts-ng"
    },
    shim: {
        angular: { 'exports': 'angular' },
        angularRoute: {
            deps: ['angular']
        },
        highcharts: {
            exports: "highcharts",
            deps: ["jquery"]
        },
        angularHighcharts: {
            exports: "angularHighcharts",
            deps: ['angular', 'highcharts']
        }
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
