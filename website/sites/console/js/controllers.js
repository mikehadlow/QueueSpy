
// QueueSpy controllers

var queuespyApp = angular.module('queuespyApp', ['ngRoute', 'queuespyControllers']);

queuespyApp.config(['$routeProvider', function ($routeProvider){
	$routeProvider.
		when('/version', {
			templateUrl: 'html/version.html',
			controller: 'VersionController'
		}).
		when('/users', {
			templateUrl: 'html/users.html',
			controller: 'UserController'
		}).
		otherwise({
			redirectTo: '/version'
		});
}]);

var queuespyControllers = angular.module('queuespyControllers', []);


queuespyControllers.controller('VersionController', function ($scope, $http) {
	$http.get('/api/version').success(function(data){
		$scope.version = data;
	});
});

queuespyControllers.controller('UserController', function ($scope, $http) {
	$http.get('/api/user').success(function(data){
		$scope.users = data;
	});
});