function homeControl($scope, $q, $http) {
    "use strict";

    $scope.url = '/api/builds';
    $scope.content = {};
    $scope.isInProgress = true;

    $scope.tabs = [{
        title: 'CI Builds',
        url: 'one.tpl.html'
    }, {
        title: 'Two',
        url: 'two.tpl.html'
    }, {
        title: 'Three',
        url: 'three.tpl.html'
    }];

    $scope.currentTab = 'one.tpl.html';

    $scope.onClickTab = function (tab) {
        $scope.currentTab = tab.url;
    }

    $scope.isActiveTab = function (tabUrl) {
        return tabUrl == $scope.currentTab;
    }

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
