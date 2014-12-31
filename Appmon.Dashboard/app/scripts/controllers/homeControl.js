function homeControl($scope, $q, $http) {
    "use strict";

    $scope.url = '/api/builds';
    $scope.content = {};
    $scope.isInProgress = true;

    $scope.tabs = [{
        title: 'CI Build',
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

    $scope.config = function (title) {
        return {
            title: title,
            tooltips: true,
            labels: false,
            mouseover: function () { },
            mouseout: function () { },
            click: function () { },
            legend: {
                display: true,
                position: 'right'
            }
        };
    };



    $scope.fetchContent();

    function renderChart() {

        $scope.totalBuildsPie = {
            series: ['Failed', 'Succeeded'],
            data: [{
                x: "Failed",
                y: [Enumerable.From($scope.content.value).Count(function (x) { return x.status == "failed"; })],
            }, {
                x: "Succeed",
                y: [Enumerable.From($scope.content.value).Count(function (x) { return x.status == "succeeded"; })]
            }]
        };

        $scope.todayBuildsPie = {
            series: ['Failed', 'Succeeded'],
            data: [{
                x: "Failed",
                y: [Enumerable.From($scope.content.value).Count(function (x) { return moment(x.finishTime).format("M/D/YYYY").toLocaleString() == moment(new Date().toLocaleString()).format("M/D/YYYY").toLocaleString() && x.status == "failed"; })],
            }, {
                x: "Succeed",
                y: [Enumerable.From($scope.content.value).Count(function (x) { return moment(x.finishTime).format("M/D/YYYY").toLocaleString() == moment(new Date().toLocaleString()).format("M/D/YYYY").toLocaleString() && x.status == "succeeded"; })]
            }]
        };

        $scope.lastWeekBuildsPie = {
            series: ['Failed', 'Succeeded'],
            data: [{
                x: "Failed",
                y: [Enumerable.From($scope.content.value).Count(function (x) { return moment(new Date().toLocaleString()).diff(moment(x.finishTime), 'days') <= 7 && x.status == "failed"; })],
            }, {
                x: "Succeed",
                y: [Enumerable.From($scope.content.value).Count(function (x) { return moment(new Date().toLocaleString()).diff(moment(x.finishTime), 'days') <= 7 && x.status == "succeeded"; })]
            }]
        };
    }
}
