window.vmMonitoringApp.vmListViewModel = (function (ko, datacontext) {

    var vmItems = ko.observableArray(),
        error = ko.observable();
    
    datacontext.getVMItems(vmItems, error); // load vmItems

    return {
        vmItems: vmItems,
        error: error
    };

})(ko, vmMonitoringApp.datacontext);

// Initiate the Knockout bindings
ko.applyBindings(window.vmMonitoringApp.vmListViewModel);
