﻿(function FreeDrawM(window) {

    /**
     * Invoked once DOM is ready, and then goodness knows what happens after that.
     *
     * @method beginExample
     * @return {void}
     */
    var beginExample = function beginExample() {

        // Setup Leaflet: http://leafletjs.com/examples/quick-start.html
        var mapContainer = window.document.querySelector('section.map'),
            map = L.map(mapContainer).setView([51.505, -0.09], 14);
        L.tileLayer('https://tiles.lyrk.org/lr/{z}/{x}/{y}?apikey=b86b18b0645848bea383827fdccb878e').addTo(map);

        var freeDraw = window.freeDraw = new L.FreeDraw({
            mode: L.FreeDraw.MODES.DELETE | L.FreeDraw.MODES.CREATE | L.FreeDraw.MODES.EDIT
        });

        freeDraw.options.setBoundariesAfterEdit(true);
        freeDraw.options.allowMultiplePolygons(true);
        freeDraw.options.allowPolygonMerging(true);
        freeDraw.options.exitModeAfterCreate(false);
        freeDraw.options.setPolygonSimplification(true);
        freeDraw.options.setHullAlgorithm('Wildhoney/ConcaveHull');

        var interest;

        freeDraw.on('markers', function getMarkers(eventData) {
            interest = eventData.latLngs;
        });

        $("#submit").click(function () {
            $.ajax({
                type: "POST",
                url: "/api/AreaOfInterest/" + $("#username").val(),
                data: JSON.stringify(interest),
                contentType: "application/json",
                success: function () {
                    interest = [];
                }
            });
        });

        $("#find").click(function () {
            $.ajax({
                type: "GET",
                url: "/api/AreaOfInterest/?latLngs=" + JSON.stringify(interest),
                success: function (response) {
                    interest = [];
                    console.log(response);
                }
            });
        });

        map.addLayer(freeDraw);

    };

    // Hold onto your hats!
    window.document.addEventListener('DOMContentLoaded', beginExample);

})(window);