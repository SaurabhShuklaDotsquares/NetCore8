(function ($) {
    function DashboardData() {
        var $this = this;
        this.bindInitialEvents = function () {
            $(document).off('change', "#ddlTimeFilter").on('change', "#ddlTimeFilter", function () {
                $this.loadGrid();
            });
        };

        this.loadGrid = function () {
            GetWebUser();
            //GetOwnerUsers();
            // GetPaymentReceives();
            //GetOwnerNewAccommodations();
        };


        this.init = function () {
            $this.loadGrid();
            $this.bindInitialEvents();
            initializeChart('');
            initializeEarningChart('');
            $("#divBookingAndEnquiryChart").show();
            $("#divEarningChart").show();

            $(document).off('change', "#ddlChartBooking").on('change', "#ddlChartBooking", function (e) {
                initializeChart($('#ddlChartBooking :selected').val());
            });

            $(document).off('change', "#ddlchartEarning").on('change', "#ddlchartEarning", function () {
                initializeEarningChart($('#ddlchartEarning :selected').val());
            });
        };
    }

    $(function () {
        var self = new DashboardData();
        self.init();
    });

    function GetWebUser() {
        var $this = this;
        this.grid = null;

        this.loadWebUserGrid = function () {
            $this.grid = new Global.GridHelper('#gridWebUsers', {
                "columnDefs": [
                    { "targets": [0], "visible": false },
                    { "targets": [1], title: "S.No.", "visible": true, "sortable": false, "searchable": false },
                    //{ "targets": [2], title: "Country Name", "sortable": true, "searchable": true },
                    {
                        "targets": [2], "data": "0", title: "Country Name", "searchable": false, "sortable": false,
                        "render": function (data, type, row, meta) {
                            var json = {
                                class: "text-decoration-none",
                                style: "color:#1c83c6",
                                'text': row[2],
                                'href': "/Admin/managebooking/index/" + row[0],
                            };
                            return $('<a/>', json).get(0).outerHTML;
                        }
                    },
                    { "targets": [3], title: "Sales", "sortable": true, "searchable": true, "className": "text-right" },
                ],
                "direction": "rtl",
                "dom": '<"pull-left"B><"pull-right"f>rt<"row"<"col-md-8 d-flex flex-wrap align-items-center showing"li><"col-md-4"p>>',
                "searching": false,
                "destroy": true,
                "bPaginate": false,
                "bInfo": false,
                //"sPaginationType": "simple_numbers",
                "bProcessing": true,
                "bServerSide": true,
                "bAutoWidth": false,
                "stateSave": false,
                "sAjaxSource": "/Admin/Dashboard/GetCountrieswisesaleslist",
                "fnServerData": function (url, data, callback) {
                    data.push({ "name": "timeFilter", "value": $("#ddlTimeFilter").val() });
                    $.ajax({
                        "url": url,
                        "data": data,
                        "success": callback,
                        "contentType": "application/x-www-form-urlencoded; charset=utf-8",
                        "dataType": "json",
                        "type": "POST",
                        "cache": false,
                        "error": function () {

                        }
                    });
                },
                "fnDrawCallback": function (oSettings) {
                    if (oSettings._iDisplayLength > oSettings.fnRecordsDisplay()) {
                        $(oSettings.nTableWrapper).find('.dataTables_paginate').hide();
                    }
                    else {
                        $(oSettings.nTableWrapper).find('.dataTables_paginate').show();
                    }
                    //$('#gridWebUsers_wrapper').css("overflow-y", "scroll");
                    $('#gridWebUsers_wrapper').css("max-height", "400px");
                },
            });
        };

        $this.loadWebUserGrid();
    }

    function GetOwnerUsers() {
        var $this = this;
        this.grid = null;

        this.loadOwnerUserGrid = function () {
            $this.grid = new Global.GridHelper('#gridOwnerUsers', {
                "columnDefs": [
                    { "targets": [0], title: "S.No.", "visible": false },
                    { "targets": [1], title: "S.No.", "visible": false },
                    { "targets": [2], title: "S.No", "visible": true, "sortable": false, "searchable": false },
                    { "targets": [3], title: "Name", "sortable": true, "searchable": true },
                    { "targets": [4], title: "Email", "sortable": false, "searchable": true },
                    { "targets": [5], title: "Phone Number", "sortable": true, "searchable": true },
                    { "targets": [6], title: "Created Date", "sortable": true, "searchable": true },
                    { "targets": [7], title: "Enabled", "sortable": false, "searchable": false },


                ],
                "direction": "rtl",
                "destroy": true,
                "bPaginate": true,
                "sPaginationType": "simple_numbers",
                "bProcessing": true,
                "bServerSide": true,
                "bAutoWidth": false,
                "stateSave": false,
                "sAjaxSource": "/Admin/Dashboard/GetOwnerUsers",
                "fnServerData": function (url, data, callback) {
                    data.push({ "name": "timeFilter", "value": $("#ddlTimeFilter").val() });
                    $.ajax({
                        "url": url,
                        "data": data,
                        "success": callback,
                        "contentType": "application/x-www-form-urlencoded; charset=utf-8",
                        "dataType": "json",
                        "type": "POST",
                        "cache": false,
                        "error": function () {

                        }
                    });
                },
                "fnDrawCallback": function (oSettings) {
                    if (oSettings._iDisplayLength > oSettings.fnRecordsDisplay()) {
                        $(oSettings.nTableWrapper).find('.dataTables_paginate').hide();
                    }
                    else {
                        $(oSettings.nTableWrapper).find('.dataTables_paginate').show();
                    }
                },
            });
        };
        $this.loadOwnerUserGrid();
    }

    function GetPaymentReceives() {
        var $this = this;
        this.grid = null;

        this.loadPaymentGrid = function () {
            $this.grid = new Global.GridHelper('#gridPaymentReceiveds', {
                "direction": "rtl",
                "destroy": true,
                "bPaginate": true,
                "sPaginationType": "simple_numbers",
                "bProcessing": true,
                "bServerSide": true,
                "bAutoWidth": false,
                "stateSave": false,
                "sAjaxSource": "/Admin/Dashboard/GetPaymentReceives",
                "fnServerData": function (url, data, callback) {
                    data.push({ "name": "timeFilter", "value": $("#ddlTimeFilter").val() });
                    var rec = JSON.stringify(data);
                    $.ajax({
                        "url": url,
                        "data": data,
                        "success": callback,
                        "contentType": "application/x-www-form-urlencoded; charset=utf-8",
                        "dataType": "json",
                        "type": "POST",
                        "cache": false,
                        "error": function () {

                        }
                    });
                },
                "columnDefs": [
                    { "targets": [0], title: "S.No.", sortable: false, searchable: false, visible: false },
                    {
                        "targets": [1], title: "S.No.", sortable: false, searchable: false, visible: true,
                        render: function (data, type, row, meta) {

                            return row[1];
                        }
                    },
                    {
                        "targets": [2], title: "Reference", sortable: true, searchable: false, visible: true,
                    },
                    { "targets": [3], title: "Paid Amount", sortable: false, searchable: false, visible: true },
                    { "targets": [4], title: "Total Amount", sortable: false, searchable: false, visible: true },
                    { "targets": [5], title: "Status", sortable: false, searchable: false, visible: true },
                    { "targets": [6], title: "Customer Name", sortable: false, searchable: false, visible: true },
                    { "targets": [7], title: "Customer Email", sortable: false, searchable: false, visible: false },
                    { "targets": [8], title: "Date", sortable: false, searchable: false, visible: false }
                ],

                "fnDrawCallback": function (oSettings) {
                    $('.divoverlay').addClass('hide');
                    if (oSettings._iDisplayLength > oSettings.fnRecordsDisplay()) {
                        $('.dataTables_paginate').hide();
                    }
                    else {
                        $('.dataTables_paginate').show();
                    }
                    $('.pagination .active a').css('background-color', '#e99701');
                    $('.pagination .active a').css('border-color', '#e99701');
                }
            });
        };

        $this.loadPaymentGrid();
    }

    function GetOwnerNewAccommodations() {
        var $this = this;
        this.grid = null;

        this.loadNewAccommodationGrid = function () {
            $this.grid = new Global.GridHelper('#gridNewOwnerProperty', {
                "direction": "rtl",
                "destroy": true,
                "bPaginate": true,
                "sPaginationType": "simple_numbers",
                "bProcessing": true,
                "bServerSide": true,
                "bAutoWidth": false,
                "stateSave": false,
                "sAjaxSource": "/Admin/Dashboard/GetOwnerNewAccommodations",
                "fnServerData": function (url, data, callback) {
                    data.push({ "name": "timeFilter", "value": $("#ddlTimeFilter").val() });
                    var rec = JSON.stringify(data);
                    $.ajax({
                        "url": url,
                        "data": data,
                        "success": callback,
                        "contentType": "application/x-www-form-urlencoded; charset=utf-8",
                        "dataType": "json",
                        "type": "POST",
                        "cache": false,
                        "error": function () {

                        }
                    });
                },
                "columnDefs": [
                    { "targets": [0], title: "S.No.", sortable: false, searchable: false, visible: false },
                    {
                        "targets": [1], title: "S.No.", sortable: false, searchable: false, visible: true,
                        render: function (data, type, row, meta) {

                            return row[1];
                        }
                    },
                    {
                        "targets": [2], title: "Title", sortable: true, searchable: false, visible: true
                    },
                    { "targets": [3], title: "Date Updated", sortable: false, searchable: false, visible: true },
                    { "targets": [4], title: "Post Date", sortable: false, searchable: false, visible: true },
                    { "targets": [5], title: "Expiry Date", sortable: false, searchable: false, visible: true },
                    { "targets": [6], title: "Destinations", sortable: false, searchable: false, visible: true },

                    { "targets": [7], title: "Staylists subdomain", sortable: false, searchable: false, visible: false },
                    { "targets": [8], title: "Booking engine", sortable: false, searchable: false, visible: false },
                    { "targets": [9], title: "Staylists display mode", sortable: false, searchable: false, visible: false },
                    {
                        "targets": [10], title: "Enabled", sortable: true, searchable: false, visible: true
                    }
                ],

                "fnDrawCallback": function (oSettings) {
                    $('.divoverlay').addClass('hide');
                    if (oSettings._iDisplayLength > oSettings.fnRecordsDisplay()) {
                        $('.dataTables_paginate').hide();
                    }
                    else {
                        $('.dataTables_paginate').show();
                    }
                    $('.pagination .active a').css('background-color', '#e99701');
                    $('.pagination .active a').css('border-color', '#e99701');
                }
            });
        };

        $this.loadNewAccommodationGrid();
    }

    function initializeChart(values) {

        $.get(domain + '/Dashboard/GetChartData', {
            type: values
        }, function (result) {
            var bookings = result.data.bookingList;
            var users = result.data.userList;
            //var earnings = result.data.enquiryList;
            var xLables = result.data.xLablesList;

            var bookingScore = [];
            var bookingLables = [];

            $.each(bookings, function (key, value) {
                bookingLables.push(value.key);
                bookingScore.push(value.value);
            });

            var userScore = [];
            var userLables = [];
            $.each(users, function (key, value) {
                userLables.push(value.key);
                userScore.push(value.value);
            });

            //var earningScore = [];
            //var earningLables = [];
            //$.each(earnings, function (key, value) {
            //    earningScore.push(value.key);
            //    earningLables.push(value.value);
            //});

            var maxVal = Math.max.apply(0, bookingScore);
            var maxValUser = Math.max.apply(0, userScore);
            //var maxValEarning = Math.max.apply(0, earningScore);
            maxVal = maxVal > maxValUser ? maxVal : maxValUser;

            var maxItemCount = userScore.length > bookingScore.length ? userScore.length : bookingScore.length;

            var stepCount = parseInt(maxVal / 10);
            stepCount = stepCount == 0 ? 1 : stepCount;

            var yItems = [];
            for (var i = 0; i < maxVal; i = i + stepCount) {
                yItems.push(i);
            }

            var chartcontainerdiv = "BookingsAndEnquiry";
            $('#BookingsAndEnquiry').remove(); // this is my <canvas> element
            $('.ChartBookingsAndEnquiry').append('<canvas id="BookingsAndEnquiry" style="min-height:500px;max-height:500px;width:100% !important;"></canvas>');

            initalizeChartLine(yItems, xLables, bookingScore, userScore, chartcontainerdiv);

        }).fail(function () {

        });
    }
    function initializeEarningChart(values) {

        $.get(domain + '/Dashboard/GetEarningChartData', {
            type: values
        }, function (result) {

            var earnings = result.data.earningList;
            var xLables = result.data.xLablesList;


            var earningScore = [];
            var earningLables = [];
            $.each(earnings, function (key, value) {
                earningLables.push(value.key);
                earningScore.push(value.value);
            });

            var maxVal = Math.max.apply(0, earningScore);

            var maxItemCount = earningScore.length;

            var stepCount = parseInt(maxVal / 10);
            stepCount = stepCount == 0 ? 1 : stepCount;

            var yItems = [];
            for (var i = 0; i < maxVal; i = i + stepCount) {
                yItems.push(i);
            }

            var chartcontainerdiv = "Earning";
            $('#Earning').remove(); // this is my <canvas> element
            $('.ChartEarning').append('<canvas id="Earning" style="min-height:500px;max-height:500px;width:100% !important;"></canvas>');

            initalizeEarningChartLine(yItems, xLables, earningScore, chartcontainerdiv);

        }).fail(function () {

        });
    }
}(jQuery));