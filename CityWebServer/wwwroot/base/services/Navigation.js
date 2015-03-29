'use strict';

define([
    'app'
], function (app) {

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

    app.value('Navigation', navigation);
});
