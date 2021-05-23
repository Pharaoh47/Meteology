// SPA primary js file
// We use knockout https://knockoutjs.com/


// Primary viewmodel
function MeteologyViewModel() {
    // Typical for knockout
    var self = this;

    // Our cities and temperatures
    self.cities = ko.observableArray([]);
    self.temperatures = ko.observableArray([]);

    // Selected city id
    self.selectedCity = ko.observable();

    // Initial fill of cities list
    $.getJSON("/api/city/list", function (data) {
        self.cities(data);
    });

    // City select even
    this.selectedCityChanged = function (obj, event) {
        // Load temperature list
        $.getJSON("/api/temperature/list/" + self.selectedCity(), function (data) {
            // And apply it to ko observable array
            self.temperatures(data);
        });
    };
    // it is for format date to beauty stance
    this.readableDate = function (dateString) {
        let date = new Date(dateString);
        return date.toLocaleDateString("ru-RU");
    };
};

// Initial knockout bind
ko.applyBindings(new MeteologyViewModel());