
// QueueSpy controllers

var queuespyApp = angular.module('queuespyApp', ['ngRoute', 'queuespyControllers']);

queuespyApp.config(['$routeProvider', function ($routeProvider){
	$routeProvider.
		when('/login', {
			templateUrl: 'html/login.html',
			controller: 'LoginController'
		}).
		when('/version', {
			templateUrl: 'html/version.html',
			controller: 'VersionController'
		}).
		when('/users', {
			templateUrl: 'html/users.html',
			controller: 'UserController'
		}).
		when('/heartbeats', {
			templateUrl: 'html/heartbeats.html',
			controller: 'HeartbeatController'
		}).
		otherwise({
			redirectTo: '/version'
		});
}]);

queuespyApp.factory('authInterceptor', function ($rootScope, $q, $window) {
	return {
		request: function (config) {
			config.headers = config.headers || {};
			if($window.sessionStorage.token) {
				config.headers.Authorization = 'Bearer ' + $window.sessionStorage.token;
			}
			return config;
		},
		response: function (response) {
			if(response.status == 401) {
				// TODO: better handling of unauthorized response
				console.log('Unauthorized access to API attempted');
			}
			return response || $q.when(response);
		}
	};
});

queuespyApp.config(function ($httpProvider) {
	$httpProvider.interceptors.push('authInterceptor');
});

var queuespyControllers = angular.module('queuespyControllers', []);

queuespyControllers.controller('LoginController', function ($scope, $http, $window) {
	$scope.message = '';
	$scope.user = { email: '', password: '' };
	$scope.submit= function () {
		$http
			.post('/api/login', $scope.user)
			.success(function (data, status, headers, config) {
				$window.sessionStorage.token = data.token;
				$scope.message = 'Login successful. Welcome.';
			})
			.error(function (data, status, headers, config) {
				// Erase the token if the user fails to login
				delete $window.sessionStorage.token;

				$scope.message = 'Error: Invalid email or password';
			});
	};
});

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

queuespyControllers.controller('HeartbeatController', function ($scope, $http) {
	$http.get('/api/heartbeats').success(function(data){
		$scope.heartbeats = data;
	});
});