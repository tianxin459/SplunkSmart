
    
    (function() {
        'use strict';
    
        // let urlPrefix = 'http://localhost/SplunkSmart/';
        let urlPrefix = '';
        let tryoutUrl=urlPrefix+'Splunk';
        // Usage:
        // 
        // Creates:
        // 
    
        angular
            .module('app')
            .component('monitor', {
                // template:'<h1>lala1 - {{$ctrl.query}}</h1><input type="text" ng-model="$ctrl.query">',
                templateUrl: 'templates/monitor.html',
                controller: ctrlController,
                controllerAs: '$ctrl',
                bindings: {
                    query:'@',
                    title:'@',
                    eventCount:'@',
                    timeEarliest:'@',
                    threshold:'<',
                    watermark:'@',
                },
            });
    
        ctrlController.$inject = ['$scope','$http'];
        function ctrlController($scope,$http) {
            let $ctrl = this;
            let interval = 30000;
            $ctrl.classStatus = "loading";
    
            $ctrl.$onInit = function() {
                console.log($ctrl.threshold+'---');
                if(!$ctrl.threshold)
                {
                    $ctrl.threshold = 50;
                    console.log('init - '+$ctrl.threshold)
                }
                $ctrl.runMonitor();
            };
            $ctrl.$onChanges = function(changesObj) { };
            $ctrl.$onDestroy = function() { };

            $ctrl.runMonitor = ()=>{
                setInterval(()=>{
                    $ctrl.callSplunker($ctrl.query,$ctrl.timeEarliest);
                },interval);
            };

            //call the proxy service
            $ctrl.callSplunker = (query, timeEarliest)=>{
                $ctrl.classStatus = "loading";
                let url = tryoutUrl + `?query=${query}&timeEarliest=${timeEarliest}`;
                $http.get(tryoutUrl)
                .then(resp=>{
                    $ctrl.eventCount = resp.data;
                    if($ctrl.eventCount>$ctrl.threshold)
                    {
                        $ctrl.classStatus = "warning";
                    }
                    else
                    {
                        $ctrl.classStatus = "";
                    }
                });
            }
        }
        
    })();