function homeControl($scope, $q, $http) {
    "use strict";

    $scope.content = {};
    $scope.isInProgress = true;

    $scope.tabs = [{
        title: 'Build Information',
        url: 'one.tpl.html'
    }, {
        title: 'Dependency Health Check',
        url: 'two.tpl.html'
    }, {
        title: 'Performance and Load Testing',
        url: 'three.tpl.html'
    }, {
        title: 'Application Summary',
        url: 'four.tpl.html'
    }];

    $scope.currentTab = 'one.tpl.html';

    $scope.onClickTab = function (tab) {
        $scope.currentTab = tab.url;
    }

    $scope.isActiveTab = function (tabUrl) {
        return tabUrl == $scope.currentTab;
    }

    $scope.fetchContent = function () {
        $q.all([$http.get('/api/build')]).then(function (values) {
            $scope.content = values[0].data;
            //$scope.isInProgress = false;
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

    $q.all([$http.get('/api/healthcheck')]).then(function (values) {
        $scope.healthcheck = values[0].data;
    });

    $q.all([$http.get('/api/appsummary')]).then(function (values) {
        $scope.appSummary = values[0].data.results;
    });

    $q.all([$http.get('/api/performance')]).then(function (values) {
        $scope.perfMetrics = values[0].data;
    });

    function renderChart() {
        $scope.someData = [[['Failure', Enumerable.From($scope.content.value).OrderBy(function (x) {
            return x.status;
        }).Count(function (x) { return x.status == "failed"; })], ['Success', Enumerable.From($scope.content.value).OrderBy(function (x) {
            return x.status;
        }).Count(function (x) { return x.status == "succeeded"; })]]];

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
