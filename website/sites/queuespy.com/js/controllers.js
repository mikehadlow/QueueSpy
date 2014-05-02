// Queuespy.com AngularJS controllers

var queuespyWebApp = angular.module('queuespyWebApp', ['ngRoute', 'queuespyWebControllers']);

queuespyWebApp.config(['$routeProvider', function ($routeProvider){
	$routeProvider
		.when('/', {
			templateUrl: 'html/signup-form.html',
			controller: 'SignupController'
		})
		.when('/confirm', {
			templateUrl: 'html/signup-confirm.html',
			controller: 'SignupConfirmController'
		})
		.otherwise({
			redirectTo: '/'
		});
}]);

var queuespyWebControllers = angular.module('queuespyWebControllers', []);

queuespyWebControllers.controller('SignupController', function ($scope, $http, $location) {

	$scope.message = '';
	$scope.user = { email: '', password: '' };
	$scope.submit = function () {
		$http
			.post('/api/user', $scope.user)
			.success(function (data, status, headers, config) {
				$location.path('/confirm');
			})
			.error(function (data, status, headers, config) {
				$scope.message = data.message;
			});
	};

});

queuespyWebControllers.controller('SignupConfirmController', function ($scope) {

});