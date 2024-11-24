// wwwroot/js/app.js
var app = angular.module('myApp', ['ngMaterial', 'cp.ngConfirm', 'daterangepicker']);
//var app = angular.module('myApp', []);

app.controller('myController', function ($scope) {
    $scope.message = "Hello, AngularJS!";
});