'use strict';

define(['server/module'], function (serverModule) {
    serverModule.controller('LogCtrl', function ($scope, Log) {
        Log.getLogLines().then(function (data) {
            $scope.logLines = data.data;
        });
    });
});
