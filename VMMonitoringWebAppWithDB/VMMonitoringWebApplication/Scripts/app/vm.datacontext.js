window.vmMonitoringApp = window.vmMonitoringApp || {};

window.vmMonitoringApp.datacontext = (function () {

    var datacontext = {
        getVMItems: getVMItems
    };

    return datacontext;

    function getVMItems(vmItemsObservable, errorObservable) {
        return ajaxRequest("get", vmListUrl())
            .done(getSucceeded)
            .fail(getFailed);

        function getSucceeded(data) {
            var mappedVMItems = $.map(data, function (item) { return new createVMItem(item); });
            vmItemsObservable(mappedVMItems);
        }

        function getFailed() {
            errorObservable("Error retrieving virtual mashine lists.");
        }
    }

    function createVMItem(data) {
        return new datacontext.vmItem(data); // vmItem is injected by todo.model.js
    }

    // Private
    function clearErrorMessage(entity) { entity.errorMessage(null); }
    function ajaxRequest(type, url, data, dataType) { // Ajax helper
        var options = {
            dataType: dataType || "json",
            contentType: "application/json",
            cache: false,
            type: type,
            data: data ? data.toJson() : null
        };

        return $.ajax(url, options);
    }

    // routes
    function vmListUrl(name)
    {
        var root = $("#root").val();

        if (root == "/")
            return "/api/vmlist/" + (name || "");
        else
            return root + "api/vmlist/" + (name || "");
    }

    function continueExecution(returnUrl) { window.location.href = "/" + (returnUrl || ""); }
})();