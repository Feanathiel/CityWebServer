'use strict';

/* App Module */

var app = angular.module('app', [
    'ngRoute',
    'serverModule'
]);

app.config(['$routeProvider',
  function($routeProvider) {
    $routeProvider.
      otherwise({
        redirectTo: '/'
      });
  }]);
