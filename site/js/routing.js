var app = angular.module('main');

/*PROVIDES URL ROUTING FOR THE APP*/
app.config(function ($stateProvider, $urlRouterProvider) {
	// DEFAULT ROUTE
	$urlRouterProvider.otherwise("/home");
	
	$stateProvider.state('home', {
		url: '/home',
		templateUrl: 'partials/home.html',
		controller: 'HomeController'
	}).state('faq', {
		url: '/faq',
		templateUrl: 'partials/faq.html',
		controller: 'FaqController'
	});
});