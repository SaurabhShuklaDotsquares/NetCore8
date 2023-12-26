var map;
(function ($) {
    function MapIndex() {
        var $this = this;
        //var lat = parseFloat($('#Latitude').val()), lng = parseFloat($('#Longitude').val());
        var input = document.getElementById('FullAddress');

        var marker;
        function initMap() {

            var lat = parseFloat($('#Latitude').val()), lng = parseFloat($('#Longitude').val());
            //console.log("map-lat-start-" + lat);
            //console.log("map-lng-start-" + lng);
            if (lat === null || lat === undefined || isNaN(lat)) {
                lat = 51.509865;
                lng = -0.118092;
            }
            // TO MAKE THE MAP APPEAR YOU MUST
            // ADD YOUR ACCESS TOKEN FROM
            // https://account.mapbox.com
            mapboxgl.accessToken = mapBoxAccessToken;
            map = new mapboxgl.Map({
                container: 'mapbox_canvas',
                style: 'mapbox://styles/mapbox/streets-v11',
                center: [lng, lat],
                zoom: 11.15
            });

            // Add zoom and rotation controls to the map.
            map.addControl(new mapboxgl.NavigationControl());

            marker = new mapboxgl.Marker({
                draggable: true,
            }).setLngLat([lng, lat])
                //.setPopup(new mapboxgl.Popup({ offset: 25 }) // add popups
                //    .setHTML('<h3>' + marker.properties.title + '</h3><p>' + marker.properties.description + '</p>'))
                .addTo(map);

            function onDragEnd() {

                //console.log("onDragEnd-lat/long-" + marker.getLngLat().lat);
                //console.log("onDragEnd-lat/long-" + marker.getLngLat().lng);
                $("#Latitude").val(marker.getLngLat().lat);
                $("#Longitude").val(marker.getLngLat().lng);

                var url = "https://api.mapbox.com/geocoding/v5/mapbox.places/" + marker.getLngLat().lng + "," + marker.getLngLat().lat + ".json?types=country,region,district,address,locality,place,postcode,neighborhood&access_token=" + mapBoxAccessToken;
                $.get(url, function (data) {

                    if (data.features.length != null) {
                        $('#Postcode').val('');
                        $('#Country').val('');
                        $('#StreetAddress').val('');
                        $('#Town').val('');
                        $('#City').val('');
                        $('#State').val('');
                        $('#LocationName').val('');

                        //console.log("mark-data-" + data);
                        $.each(data.features, function (index, v) {
                            
                            if (v.place_type[0] == "locality") {
                                $("#LocationName").val(v.text);
                            }
                            if (v.place_type[0] == "country") {
                                $('#Country').val(v.text);
                            }
                            if (v.place_type[0] == "district") {
                                $("#City").val(v.text);
                            }
                            if (v.place_type[0] == "postcode") {
                                $('#Postcode').val(v.text);
                            }
                            if (v.place_type[0] == "place") {
                                $("#Town").val(v.text);
                            }
                            if (v.place_type[0] == "region") {
                                $('#State').val(v.text);
                            }

                            $('#StreetAddress').val(data.features[0].place_name.split(", ")[0]);
                            $("#FullAddress").val(data.features[0].place_name);
                        })
                    }
                });
            }

            marker.on('dragend', onDragEnd);

        }



        function mapBoxAutoComplete() {
            var geocodingClient = mapboxSdk({ accessToken: mapBoxAccessToken });
            //mapboxClient.styles.getStyle(..)
            //    .send()
            function autocompleteSuggestionMapBoxAPI(inputParams, callback) {
                geocodingClient.geocoding.forwardGeocode({
                    query: inputParams,
                    //countries: ['In'],
                    autocomplete: true,
                    limit: 5,
                })
                    .send()
                    .then(response => {
                        const match = response.body;
                        callback(match);
                    });
            }

            function autocompleteInputBox(inp) {
                var currentFocus;
                inp.addEventListener("input", function (e) {
                    var a, b, i, val = this.value;
                    closeAllLists();
                    if (!val) {
                        return false;
                    }
                    currentFocus = -1;
                    a = document.createElement("DIV");
                    a.setAttribute("id", this.id + "autocomplete-list");
                    a.setAttribute("class", "autocomplete-items");
                    this.parentNode.appendChild(a);

                    // suggestion list MapBox api called with callback
                    autocompleteSuggestionMapBoxAPI($('#FullAddress').val(), function (results) {

                        results.features.forEach(function (key) {

                            //console.log(key);
                            b = document.createElement("DIV");
                            b.innerHTML = "<strong>" + key.place_name.substr(0, val.length) + "</strong>";
                            b.innerHTML += key.place_name.substr(val.length);

                            var postcode, countryFullName, countryShortName, streetAddress, city, state, locationName, town;
                            $.each(key.context, function (e, a) {
                                a.id.indexOf("place") >= 0 ? town = a.text :
                                    a.id.indexOf("district") >= 0 ? city = a.text :
                                        a.id.indexOf("postcode") >= 0 ? postcode = a.text :
                                            a.id.indexOf("region") >= 0 ? state = a.text :
                                                a.id.indexOf("country") >= 0 && (countryFullName = a.text, countryShortName = a.short_code)
                            });
                            streetAddress = key.place_name.split(", ")[0];
                            locationName = key.address != null ? key.address : "";

                            b.innerHTML += "<input type='hidden' data-lat='" + key.geometry.coordinates[1] + "' data-lng='" + key.geometry.coordinates[0] + "'  value='" + key.place_name + "'  data-postcode='" + key.postcode + "'  data-countryFullName='" + countryFullName + "'  data-countryShortName='" + countryShortName + "'  data-streetAddress='" + streetAddress + "'  data-town='" + town + "'  data-city='" + city + "'  data-state='" + state + "'  data-locationName='" + locationName + "'>";

                            //console.log("lat-auto-" + key.geometry.coordinates[0]);
                            //console.log("long-auto-" + key.geometry.coordinates[1]);

                            b.addEventListener("click", function (e) {

                                inp.value = $(this).find('input').val();

                                let selLat = $(this).find('input').attr('data-lat');
                                let selLong = $(this).find('input').attr('data-lng');
                                let selPostcode = $(this).find('input').attr('data-postcode') != null && $(this).find('input').attr('data-postcode') != "undefined" ? $(this).find('input').attr('data-postcode') : "";
                                let selCountryFullName = $(this).find('input').attr('data-countryFullName') != null && $(this).find('input').attr('data-countryFullName') != "undefined" ? $(this).find('input').attr('data-countryFullName') : "";
                                let selCountryShortName = $(this).find('input').attr('data-countryShortName') != null && $(this).find('input').attr('data-countryShortName') != "undefined" ? $(this).find('input').attr('data-countryShortName') : "";
                                let selStreetAddress = $(this).find('input').attr('data-streetAddress') != null && $(this).find('input').attr('data-streetAddress') != "undefined" ? $(this).find('input').attr('data-streetAddress') : "";
                                let selTown = $(this).find('input').attr('data-town') != null && $(this).find('input').attr('data-town') != "undefined" ? $(this).find('input').attr('data-town') : "";
                                let selCity = $(this).find('input').attr('data-city') != null && $(this).find('input').attr('data-city') != "undefined" ? $(this).find('input').attr('data-city') : "";
                                let selState = $(this).find('input').attr('data-state') != null && $(this).find('input').attr('data-state') != "undefined" ? $(this).find('input').attr('data-state') : "";
                                let selLocationName = $(this).find('input').attr('data-locationName') != null && $(this).find('input').attr('data-locationName') != "undefined" ? $(this).find('input').attr('data-locationName') : "";

                                $("#Latitude").val(selLat);
                                $("#Longitude").val(selLong);

                                $('#Postcode').val(selPostcode);
                                $('#Country').val(selCountryFullName);
                                $('#StreetAddress').val(selStreetAddress);
                                $('#Town').val(selTown);
                                $('#City').val(selCity);
                                $('#State').val(selState);
                                $('#LocationName').val(selLocationName);

                                initMap();

                                closeAllLists();
                            });
                            a.appendChild(b);
                        });
                    })
                });


                /*execute a function presses a key on the keyboard:*/
                inp.addEventListener("keydown", function (e) {
                    var x = document.getElementById(this.id + "autocomplete-list");
                    if (x) x = x.getElementsByTagName("div");
                    if (e.keyCode == 40) {
                        /*If the arrow DOWN key is pressed,
                        increase the currentFocus variable:*/
                        currentFocus++;
                        /*and and make the current item more visible:*/
                        addActive(x);
                    } else if (e.keyCode == 38) { //up
                        /*If the arrow UP key is pressed,
                        decrease the currentFocus variable:*/
                        currentFocus--;
                        /*and and make the current item more visible:*/
                        addActive(x);
                    } else if (e.keyCode == 13) {
                        /*If the ENTER key is pressed, prevent the form from being submitted,*/
                        e.preventDefault();
                        if (currentFocus > -1) {
                            /*and simulate a click on the "active" item:*/
                            if (x) x[currentFocus].click();
                        }
                    }
                });

                function addActive(x) {
                    /*a function to classify an item as "active":*/
                    if (!x) return false;
                    /*start by removing the "active" class on all items:*/
                    removeActive(x);
                    if (currentFocus >= x.length) currentFocus = 0;
                    if (currentFocus < 0) currentFocus = (x.length - 1);
                    /*add class "autocomplete-active":*/
                    x[currentFocus].classList.add("autocomplete-active");
                }

                function removeActive(x) {
                    /*a function to remove the "active" class from all autocomplete items:*/
                    for (var i = 0; i < x.length; i++) {
                        x[i].classList.remove("autocomplete-active");
                    }
                }

                function closeAllLists(elmnt) {
                    /*close all autocomplete lists in the document,
                    except the one passed as an argument:*/
                    var x = document.getElementsByClassName("autocomplete-items");
                    for (var i = 0; i < x.length; i++) {
                        if (elmnt != x[i] && elmnt != inp) {
                            x[i].parentNode.removeChild(x[i]);
                        }
                    }
                }

                /*execute a function when someone clicks in the document:*/
                document.addEventListener("click", function (e) {
                    closeAllLists(e.target);
                });
            }

            autocompleteInputBox(document.getElementById("FullAddress"));

        }

        $this.init = function () {
            initMap();
            mapBoxAutoComplete();
        };
    }


    $(function () {
        var self = new MapIndex();
        self.init();
    });

}(jQuery));


//(function ($) {
//    function MapIndex() {
//        var $this = this; var image = 'http://www.google.com/intl/en_us/mapfiles/ms/micons/blue-dot.png';
//        var lat = parseFloat($('#Latitude').val()), lng = parseFloat($('#Longitude').val());
//        function initMap() {

//            var map = new google.maps.Map(document.getElementById('map_canvas'), {
//                center: { lat: lat, lng: lng },
//                zoom: 17
//            });
//            var input = document.getElementById('FullAddress');

//            //map.controls[google.maps.ControlPosition.TOP_LEFT].push(input);
//            //if (Global.IsNullOrEmptyString(input.value)) {
//            //    
//            //    setCurrentLocation();
//            //}
//            var autocomplete = new google.maps.places.Autocomplete(input);
//            autocomplete.bindTo('bounds', map);

//            var infowindow = new google.maps.InfoWindow();
//            var marker = new google.maps.Marker({
//                map: map,
//                anchorPoint: new google.maps.Point(0, -29),

//            });
//            marker.setVisible(false);
//            marker.setIcon(({
//                url: image,
//                size: new google.maps.Size(71, 71),
//                origin: new google.maps.Point(0, 0),
//                anchor: new google.maps.Point(17, 34),
//                scaledSize: new google.maps.Size(35, 35)
//            }));
//            latlng = new google.maps.LatLng(lat, lng);
//            marker.setPosition(latlng);
//            marker.setVisible(true);
//            infowindow.setContent('<div><strong>' + $('#StreetAddress').val() + '</strong><br>' +  $('#Town').val() + " " + $('#State').val());
//            infowindow.open(map, marker);
//            marker.addListener("click", () => {
//                infowindow.open(map, marker);
//            });


//            autocomplete.addListener('place_changed', function () {

//                infowindow.close();
//                marker.setVisible(false);
//                var place = autocomplete.getPlace();
//                if (!place.geometry) {
//                    window.alert("Autocomplete's returned place contains no geometry");
//                    return;
//                };


//                if (place.geometry.viewport) {
//                    map.fitBounds(place.geometry.viewport);
//                } else {
//                    map.setCenter(place.geometry.location);
//                    map.setZoom(17);
//                }
//                marker.setIcon(({
//                    url: image,
//                    size: new google.maps.Size(71, 71),
//                    origin: new google.maps.Point(0, 0),
//                    anchor: new google.maps.Point(17, 34),
//                    scaledSize: new google.maps.Size(35, 35)
//                }));

//                marker.setPosition(place.geometry.location);
//                marker.setVisible(true);
//                marker.addListener("click", () => {
//                    infowindow.open(map, marker);
//                });
//                var address = '';
//                if (place.address_components) {
//                    address = [
//                        (place.address_components[0] && place.address_components[0].short_name || ''),
//                        (place.address_components[1] && place.address_components[1].short_name || ''),
//                        (place.address_components[2] && place.address_components[2].short_name || '')
//                    ].join(' ');
//                }

//                infowindow.setContent('<div><strong>' + place.name + '</strong><br>' + address);
//                infowindow.open(map, marker);


//                $('#Postcode').val('');
//                $('#Country').val('');
//                $('#StreetAddress').val('');
//                $('#Town').val('');
//                $('#City').val('');
//                $('#City').val('');
//                $('#State').val('');
//                $('#LocationName').val('');
//                //$('#FullAddress').val('');
//                $('#Latitude').val('');
//                $('#Longitude').val('');
//                for (var i = 0; i < place.address_components.length; i++) {

//                    if (place.address_components[i].types[0] == 'postal_code') {
//                        $('#Postcode').val(place.address_components[i].long_name);
//                    }
//                    if (place.address_components[i].types[0] == 'country') {
//                        $('#Country').val(place.address_components[i].long_name);
//                    }
//                    if (place.address_components[i].types[0] == 'street_number') {
//                        $('#StreetAddress').val(place.address_components[i].long_name);
//                    }
//                    else {
//                        $('#StreetAddress').val(place.name);
//                    }
//                    if (place.address_components[i].types[0] == 'locality') {
//                        $('#City').val(place.address_components[i].short_name);

//                    }
//                    if (place.address_components[i].types[0] == 'administrative_area_level_1') {
//                        $('#State').val(place.address_components[i].long_name);
//                    }
//                    if (place.address_components[i].types[0] == 'sublocality_level_1') {
//                        $('#Town').val(place.address_components[i].short_name);
//                    }
//                    if (place.address_components[i].types[0] == 'route') {
//                        $('#LocationName').val(place.address_components[i].short_name);
//                    }
//                }

//                //$('#FullAddress').val(place.formatted_address);
//                $('#Latitude').val(place.geometry.location.lat());
//                $('#Longitude').val(place.geometry.location.lng());
//            });
//        }




//        $this.init = function () {
//            initMap();
//        };
//    }


//    $(function () {
//        var self = new MapIndex();
//        self.init();
//    });

//}(jQuery));