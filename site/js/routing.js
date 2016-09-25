var app = angular.module('main');

/*PROVIDES URL ROUTING FOR THE APP*/
app.config(function ($stateProvider, $urlRouterProvider) {
	// DEFAULT ROUTE
	$urlRouterProvider.otherwise("/home");
	
	$stateProvider.state('home', {
		url: '/home',
		templateUrl: 'site/partials/home.html',
		controller: 'HomeController'
	}).state('faq', {
		url: '/faq',
		templateUrl: 'site/partials/faq.html',
		controller: 'FaqController'
	}).state('player', {
		url: '/player?video',
		templateUrl: 'site/partials/player.html',
		controller: 'PlayerController'
	});
});