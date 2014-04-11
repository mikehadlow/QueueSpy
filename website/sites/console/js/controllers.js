
// QueueSpy controllers

var queuespyApp = angular.module('queuespyApp', []);

queuespyApp.controller('VersionController', function ($scope, $http) {
	$http.get('/api/version').success(function(data){
		$scope.version = data;
	});
});