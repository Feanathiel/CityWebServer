'use strict';

define([
    'base/module'
], function (baseModule) {
    baseModule.provider('navigation', function () {
        var navigationItems = [];

        this.register = function (title, url) {
            navigationItems.push({
                'Title': title,
                'Url': url
            });
        };

        this.$get = function () {
            return function () {
                return navigationItems;
            };
        };
    });
});
