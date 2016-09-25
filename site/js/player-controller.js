var app = angular.module('main');

app.controller('PlayerController', function($scope, $log, $stateParams, $sce) {
	console.log($stateParams);
	$scope.video = $sce.trustAsResourceUrl($stateParams.video);
});