'use strict';

define(['server/module'], function (module) {
    module.factory('Log', function ($http) {
        return {
            getLogLines: function () {
                return $http.get("/Api/Server/LogLines.json").success(function (response) {
                    return response.data;
                });
            }
        }
    });
});
