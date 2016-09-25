var app = angular.module('main');

app.controller('PlayerController', function($scope, $log, $stateParams, $sce) {
	$scope.video = $sce.trustAsResourceUrl(decodeURIComponent($stateParams.video));
	console.log($scope.video);
});