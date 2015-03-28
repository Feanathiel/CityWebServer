'use strict';

define(['citizen/module'], function (module) {
    module.factory('Citizen', function ($http) {
        return {
            getAgeDistribution: function () {
                return $http.get("/Api/Citizen/Age.json").success(function (response) {
                    return response.data;
                });
            },
            getBirthAndDeathRate: function () {
                return $http.get("/Api/Citizen/BirthAndDeath.json").success(function (response) {
                    return response.data;
                });
            },
            getEducationEmploymentRate: function () {
                return $http.get("/Api/Citizen/EducationEmployment.json").success(function (response) {
                    return response.data;
                });
            }
        }
    });
});
