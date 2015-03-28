'use strict';

define(['citizen/module'], function (module) {
    module.factory('Citizen', function ($http) {
        return {
            getAgeDistribution: function () {
                return $http.get("/Api/Citizen/Age.json").success(function (response) {
                    return response.data;
                });
            }
        }
    });
});
