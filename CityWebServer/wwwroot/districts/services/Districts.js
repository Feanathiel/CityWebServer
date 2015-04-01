'use strict';

define(['districts/module'], function (module) {
    module.factory('Districts', function ($http) {
        return {
            getDistricts: function () {
                return $http.get("/Api/Districts/Districts.json").success(function (response) {
                    return response.data;
                });
            },
            getCarReasons: function() {
                return $http.get("/Api/Districts/CarReasons.json").success(function (response) {
                    return response.data;
                });
            },
            getVitals: function() {
                return $http.get("/Api/Districts/Vitals.json").success(function (response) {
                    return response.data;
                });
            }
        }
    });
});
