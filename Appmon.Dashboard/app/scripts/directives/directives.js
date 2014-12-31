app.directive('bDatepicker', function () {
    return {
        require: '?ngModel',
        restrict: 'A',
        link: function ($scope, element, attrs, controller) {
            var updateModel;
            updateModel = function (ev) {
                element.datepicker('hide');
                element.blur();
                return $scope.$apply(function () {
                    return controller.$setViewValue(ev.date);
                });
            };
            if (controller != null) {
                controller.$render = function () {
                    if (controller.$viewValue.length > 0) {
                        element.datepicker().data().datepicker.date = controller.$viewValue;
                        element.datepicker('setValue');
                        element.datepicker('update');
                    }
                    return controller.$viewValue;
                };
            }
            return attrs.$observe('bDatepicker', function (value) {
                var options;
                options = {};
                if (angular.isObject(value)) {
                    options = value;
                }
                if (typeof (value) === "string" && value.length > 0) {
                    options = angular.fromJson(value);
                }
                return element.datepicker(options).on('changeDate', updateModel);
            });
        }
    };
});
    //.directive('tabControl', function () {
    //    return {
    //        restrict: 'E',
    //        templateUrl: 'tabControlTemplate',
    //        scope: {
    //            id: '@id',
    //            klass: '@class',
    //        },
    //        transclude: true,
    //        controller: ['$scope', function ($scope) {
    //            $scope.tabs = []

    //            this.addTab = function (tab) {
    //                $scope.tabs.push(tab);
    //            }

    //            $scope.selectTab = function (tab) {
    //                for (var i = 0; i < $scope.tabs.length; i++) {
    //                    if (tab.name != $scope.tabs[i].name) {
    //                        $scope.tabs[i].selected = false;
    //                    }
    //                    else {
    //                        $scope.tabs[i].selected = true;
    //                    }
    //                }
    //            }
    //        }]
    //    };
    //})
    //.directive('tab', function () {
    //    return {
    //        restrict: 'E',
    //        templateUrl: 'tabTemplate',
    //        transclude: true,
    //        replace: true,
    //        scope: {
    //            id: '@id',
    //            name: '@name',
    //        },
    //        require: '^tabControl',
    //        link: function (scope, element, attrs, ctrl) {
    //            scope.selected = attrs.selected == "" || attrs.selected == true
    //            ctrl.addTab(scope);
    //        }
    //    };
    //});