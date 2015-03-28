'use strict';

define(['districts/module'], function (module) {
    module.factory('Districts', function ($http) {
        return {
            getDistricts: function () {
                return $http.get("/Api/Districts/Districts.json").success(function (response) {
                    return response.data;
                });
            }
        }
    });
});
