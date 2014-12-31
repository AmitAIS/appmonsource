﻿var app = angular.module('ci', ['ui.bootstrap', 'ngRoute', 'smart-table', 'ui.chart']);


app.config(function ($routeProvider) {
    $routeProvider.caseSensitive = false;
    $routeProvider
        .when('/', { templateUrl: 'app/views/home.html', controller: 'homeControl' })
        .otherwise({ redirectTo: '/' });
});
