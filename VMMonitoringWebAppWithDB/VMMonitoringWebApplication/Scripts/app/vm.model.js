(function (ko, datacontext) {
    datacontext.vmItem = vmItem;

    function vmItem(data) {
        var self = this;
        data = data || {};

        // Persisted properties
        self.name = ko.observable(data.name);
        self.status = ko.observable(data.status);
        self.memory = ko.observable(data.memory);
        self.cpu = ko.observable(data.cpu);
        self.network = ko.observable(data.network);
        self.storage = ko.observable(data.storage);

        self.errorMessage = ko.observable();

        self.toJson = function () { return ko.toJSON(self); };
    };

    function importVMItems(vmItems) {

        return $.map(vmItems || [],
                function (vmItemData) {
                    return datacontext.createVMItem(vmItemData);
                });
    }
})(ko, vmMonitoringApp.datacontext);