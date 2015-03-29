'use strict';

define([
    'base/module'
], function (baseModule) {

    var navigationItems = [];

    var navigation = {
        list: function () {
            return navigationItems;
        },
        register: function(title, url) {
            navigationItems.push({
                'Title': title, 
                'Url': url
            });
        }
    };

    baseModule.value('Navigation', navigation);
});
