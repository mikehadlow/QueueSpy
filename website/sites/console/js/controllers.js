// QueueSpy 

var queuespyApp = angular.module('queuespyApp', ['ngRoute', 'queuespyControllers']);

queuespyApp.config(['$routeProvider', function ($routeProvider){
	$routeProvider.
		when('/login', {
			templateUrl: 'html/login.html',
			controller: 'LoginController'
		}).
		when('/forgotten-password', {
			templateUrl: 'html/forgotten-password.html',
			controller: 'ForgottenPasswordController'
		}).
		when('/password-reset', {
			templateUrl: 'html/password-reset.html',
			controller: 'PasswordResetController'
		}).
		when('/logout', {
			template: ' ',
			controller: 'LogoutController'
		}).
		when('/account', {
			templateUrl: 'html/account.html',
			controller: 'AccountController'
		}).
        when('/brokers', {
            templateUrl: 'html/brokers.html',
            controller: 'BrokerController'
        }).
        when('/broker-add', {
            templateUrl: 'html/broker-add.html',
            controller: 'BrokerAddController'
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

queuespyApp.factory('authInterceptor', function ($rootScope, $q, $window, $location) {
	return {
		request: function (config) {
			config.headers = config.headers || {};
			if($window.sessionStorage.token) {
				config.headers.Authorization = 'Bearer ' + $window.sessionStorage.token;
			}
			return config;
		},
		responseError: function (response) {
			if(response.status === 401) {
				// TODO: better handling of unauthorized response
				$location.path('/login');
			}
			return $q.reject(response);
		}
	};
});

queuespyApp.config(function ($httpProvider) {
	$httpProvider.interceptors.push('authInterceptor');
});

// Controllers

var queuespyControllers = angular.module('queuespyControllers', []);

queuespyControllers.controller('ProfileController', function ($scope, $window, $rootScope) {

	var onLoggedIn = function () {
		$scope.showLogin = false;
		$scope.email = $window.sessionStorage.email;
	};

	var onLoggedOut = function () {
		$scope.showLogin = true;
		$scope.email = "";
	};

	if(!$window.sessionStorage.token) {
		onLoggedOut();
	} else {
		onLoggedIn();
	}

	$rootScope.$on("LoginController.login", function (event) {
		onLoggedIn();
	});

	$rootScope.$on("LogoutController.logout", function (event) {
		onLoggedOut();
	});
});

queuespyControllers.controller('LoginController', function ($scope, $http, $window, $location, $rootScope) {
	$scope.message = '';
	$scope.user = { email: '', password: '' };
	$scope.submit = function () {
		$http
			.post('/api/login', $scope.user)
			.success(function (data, status, headers, config) {
				$window.sessionStorage.token = data.token;
				var user = angular.fromJson($window.atob(data.token.split('.')[1]));
				$window.sessionStorage.email = user.email;
				$window.sessionStorage.userId = user.userId;
				$rootScope.$emit("LoginController.login");
				$location.path('/');
			})
			.error(function (data, status, headers, config) {
				// Erase the token if the user fails to login
				delete $window.sessionStorage.token;

				$scope.message = 'Error: Invalid email or password';
			});
	};
});

queuespyControllers.controller('ForgottenPasswordController', function ($scope, $http, $window, $location, $rootScope) {
	$scope.email = '';
	$scope.submit = function () {
		$http
			.post('/api/user/forgottenPassword', { "email": $scope.email })
			.success(function (data, status, headers, config) {
				$scope.message = 'We have sent you an email to reset your password.';
			})
			.error(function (data, status, headers, config) {
				$scope.message = data.message;
			});
	};
});

queuespyControllers.controller('PasswordResetController', function ($scope, $http, $routeParams) {
	$scope.password = '';
	$scope.submit = function () {
		$http
			.post('/api/user/passwordReset', { "newPassword": $scope.password, "token": $routeParams.token })
			.success(function () {
				$scope.message = 'Your password has been successfully reset. Please log in as usual.';
			})
			.error(function (data) {
				$scope.message = data.message;
			});
	};
});

queuespyControllers.controller('LogoutController', function ($window, $location, $rootScope) {
	delete $window.sessionStorage.token;
	$rootScope.$emit("LogoutController.logout");
	$location.path('/');
});

queuespyControllers.controller('AccountController', function ($scope, $http, $window, $location, $rootScope) {
	$scope.user = {};
	$scope.user.email = window.sessionStorage.email;

	$scope.changePw = { oldPassword: '', newPassword: '' };

	$scope.submitChangePassword = function () {
		$http
			.post('/api/user/changePassword', $scope.changePw)
			.success(function (data, status, headers, config) {
				$scope.changePw = { oldPassword: '', newPassword: '' };
				$scope.changePasswordMessage = 'Password successfully changed.'
			})
			.error(function (data, status, headers, config) {
				$scope.changePasswordMessage = data.message;
			});
	};

	$scope.cancelAccount = function () {
		$http
			.post('/api/user/cancelAccount', {})
			.success(function (data, status, headers, config) {
				// $scope.cancelAccountMessage = 'Your account has been cancelled.'
				delete $window.sessionStorage.token;
				$rootScope.$emit("LogoutController.logout");
				$location.path('/');
			})
			.error(function (data, status, headers, config) {
				// $scope.cancelAccountMessage = data.message;
			});
	};
});

queuespyControllers.controller('BrokerController', function ($scope, $http) {
    $scope.brokers = [];
    $http.get('/api/broker').success(function(data) {
        $scope.brokers = data;
    });
});

queuespyControllers.controller('BrokerAddController', function ($scope, $http) {
    var nullBroker = {
        url: '',
        username: '',
        password: ''
    };

    $scope.broker = nullBroker;

    $scope.submit = function () {
        $http
            .post('/api/broker', $scope.broker)
            .success(function (data, status, headers, config) {
                $scope.message = 'Broker registered.';
                $scope.broker = nullBroker;
            })
            .error(function (data, status, headers, config) {
                $scope.message = data.message;
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
