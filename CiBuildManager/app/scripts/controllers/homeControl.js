function homeControl($scope, $q, $http) {
    "use strict";

    $scope.url = 'content.json';
    $scope.content = {};
    $scope.isInProgress = true;

    $scope.fetchContent = function () {
        $q.all([$http.get($scope.url)]).then(function (values) {
            $scope.content = values[0].data;
            $scope.isInProgress = false;

            renderChart();
        });
    }

    $scope.dateOptions = { format: 'dd/mm/yyyy' }
    $scope.fromDate = "";
    $scope.toDate = "";
   // $scope.status = "Failed";
    $scope.formatDate = function (value) {
        return moment(value).format("M/D/YYYY h:mm:ss A").toLocaleString();
    }

    $scope.fetchContent();

    function renderChart() {
        $scope.someData = [[['failed', Enumerable.From($scope.content.value).OrderBy(function (x) {
            return x.status;
        }).Count(function (x) { return x.status == "failed"; })], ['success', Enumerable.From($scope.content.value).OrderBy(function (x) {
            return x.status;
        }).Count(function (x) { return x.status == "success"; })]]];

        $scope.myChartOpts = {
            seriesDefaults: {
                // Make this a pie chart.
                renderer: jQuery.jqplot.PieRenderer,
                rendererOptions: {
                    // Put data labels on the pie slices.
                    // By default, labels show the percentage of the slice.
                    showDataLabels: true
                }
            },
            legend: { show: true, location: 'e' }
        };
    }
}
