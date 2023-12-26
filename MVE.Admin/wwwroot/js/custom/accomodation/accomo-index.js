(function ($) {
    function AccomoIndex() {
        var $this = this;
        var accomodationGrid = '';
        function initializeGrid() {
            if ($.fn.DataTable.isDataTable(accomodationGrid)) {
                $(accomodationGrid).DataTable().destroy();
            }
            accomodationGrid = new Global.GridHelper('#grid-accomodation-details', {
                "columnDefs": [
                    { "targets": [0], "visible": false },
                    { "targets": [1], "visible": true, "sortable": false, "searchable": false },
                    { "targets": [2], "sortable": true, "searchable": true },
                    { "targets": [3], "sortable": true, "searchable": true },
                    { "targets": [4], "sortable": true, "searchable": true },
                    {
                        "targets": [5], "sortable": false, "searchable": false, "data": "5",
                        "render": function (data, type, row, meta) {
                            var json = {
                                    type: "checkbox",
                                    class: "switchBox switch-small",
                                    value: row[0],
                                    'data-on': "success",
                                    'data-off': "danger",
                                    "data-on-text": "Yes",
                                    "data-off-text": "No"
                                };

                                if (data === "True") {
                                    json.checked = true;
                                }
                                json = $('<input/>', json).get(0).outerHTML;
                          
                            return json;
                        }
                    },
                    {
                        "targets": [6], "data": "0", "searchable": false, "sortable": false,
                        "render": function (data, type, row, meta) {
                            var actionLink = '';

                            actionLink = $("<a />", {
                                href: "/Admin/AccomodationType/AddEdit/" + row[0],
                                id: "accomodationEditModalId",
                                class: "btn btn-primary btn-sm",
                                oncontextmenu: 'return false',
                                'data-toggle': "modal",
                                'data-target': "#modal-add-edit-accomodation",
                                title: 'Edit',
                                html: $("<i/>", {
                                    class: "fa fa-edit"
                                })
                            }).get(0).outerHTML + "&nbsp;";


                            actionLink += $("<a/>", {
                                href: "/Admin/AccomodationType/Delete/" + row[0],
                                id: "accomodationDeleteModalId",
                                class: "btn btn-danger btn-sm",
                                title: 'Delete',
                                oncontextmenu: 'return false',
                                'data-toggle': "modal",
                                'data-target': "#modal-delete-accomodation",
                                html: $("<i/>", {
                                    class: "fa fa-trash"
                                })
                            }).get(0).outerHTML;

                            return actionLink;
                        }
                    }
                ],
                "direction": "rtl",
                "bPaginate": true,
                "sPaginationType": "simple_numbers",
                "bProcessing": true,
                "bServerSide": true,
                "bAutoWidth": false,
                "stateSave": false,
                "sAjaxSource": "/Admin/" + "AccomodationType/Index",
                "fnServerData": function (url, data, callback) {
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
                    initGridControlsWithEvents();
                    if (oSettings._iDisplayLength > oSettings.fnRecordsDisplay()) {
                        $(oSettings.nTableWrapper).find('.dataTables_paginate').hide();
                    }
                    else {
                        $(oSettings.nTableWrapper).find('.dataTables_paginate').show();
                    }
                },
            });

            // table = userDetailsGrid.DataTable();
        }

        function initGridControlsWithEvents() {
            if ($('.switchBox').data('bootstrapSwitch')) {
                $('.switchBox').off('switchChange.bootstrapSwitch');
                $('.switchBox').bootstrapSwitch('destroy');
            }

            $('.switchBox').bootstrapSwitch()
                .on('switchChange.bootstrapSwitch', function () {
                    var switchElement = this;
                    var MessageDiv = $('#MessageDiv');
                    $.get(domain + '/AccomodationType/UpdateStatus', { id: this.value }, function (result) {
                        if (!result.isSuccess) {
                            $(switchElement).bootstrapSwitch('toggleState', true);

                        }
                        else {

                            Global.ShowMessage(result.data, 0);
                        }

                    });
                });
        }

        function initilizeModel() {
            $("#modal-add-edit-accomodation").on('shown.bs.modal', function (e) {
                setTimeout(function () {
                    var formAddEditaccomodation = new Global.FormHelper($("#frm-add-edit-accomodation"), {
                        updateTargetId: "validation-summary",
                        validateSettings: {
                            ignore: []
                        }
                    }, function onSuccess(result) {
                        if (result) {                            
                            var MessageDiv = $('#MessageDiv');
                            if (Global.IsNotNullOrEmptyString(result.message)) {
                                MessageDiv.addClass('alert-success').removeClass('alert-danger');
                                MessageDiv.empty().html(result.message);
                                initializeGrid();
                                $("#modal-add-edit-accomodation").modal('hide');
                                MessageDiv.show();
                            }

                            window.setTimeout(function () {
                                MessageDiv.html('');
                                MessageDiv.hide();
                            }, 5000);
                        }
                    });

                    $('.form-checkbox').bootstrapSwitch('onText', 'Yes');
                    $('.form-checkbox').bootstrapSwitch('offText', 'No');
                }, 700);
            }).on('hidden.bs.modal', function (e) {
                $(this).removeData('bs.modal');
            });
        }

        $this.init = function () {
            initializeGrid();
            initilizeModel();
        };
    }

    $(function () {
        var self = new AccomoIndex();
        self.init();
    })
})(jQuery)