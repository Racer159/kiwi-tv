var app = angular.module('main');

app.controller('PlayerController', function($scope, $log, $stateParams, $sce) {
	$scope.video = $sce.trustAsResourceUrl(decodeURIComponent($stateParams.video));
	console.log(Hls.isSupported());
	if(Hls.isSupported()) {
		var video = document.getElementById('video');
		var hls = new Hls();
		hls.loadSource($scope.video);
		hls.attachMedia(video);
		hls.on(Hls.Events.MANIFEST_PARSED,function() {
			video.play();
		});
	}
});