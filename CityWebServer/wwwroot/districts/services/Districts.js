districtsServices.factory('Districts', function($http) {
    return{
        getDistricts: function() {
            return $http.get("/Api/Districts/districts.json").success(function(response) {
                return response.data;
            });
        }
    }
});