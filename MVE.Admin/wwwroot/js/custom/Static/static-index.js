(function ($) {
    function StaticIndex() {
        var $this = this, formStatic;

        function initializeGrid() {

            var gridstaticpage = new Global.GridHelper('#grid-staticpage', {
                "columnDefs": [
                    {
                        "targets": [0],
                        "visible": false,
                        "searchable": false
                    },
                    {
                        "targets": [1],
                        "visible": true,
                        "sortable": false,
                        "searchable": true

                    },
                    {
                        "targets": [2],
                        "visible": true,
                        "sortable": true,
                        "searchable": true
                    },
                    {
                        "targets": [3],
                        "visible": true,
                        "sortable": true,
                        "searchable": true,
                        "render": function (data, type, row, meta) {
                            var texthtml = '';
                            if (row[3] =="privacy-policy") {
                                texthtml = $("<a/>", {
                                    href: frontDomain + "/privacy-policy",
                                    class: "text-decoration-none",
                                    style: "color:#1c83c6",
                                    'cursor': "none",
                                    target: "_blank",
                                    text: row[3]
                                }).get(0).outerHTML;
                            }
                            if (row[3] == "terms-condition") {
                                texthtml = $("<a/>", {
                                    href: frontDomain + "/terms-condition",
                                    class: "text-decoration-none",
                                    style: "color:#1c83c6",
                                    'cursor': "none",
                                    target: "_blank",
                                    text: row[3]
                                }).get(0).outerHTML;
                            }
                            if (row[3] == "about-us") {
                                texthtml = $("<a/>", {
                                    href: frontDomain + "/about-us",
                                    class: "text-decoration-none",
                                    style: "color:#1c83c6",
                                    'cursor': "none",
                                    target: "_blank",
                                    text: row[3]
                                }).get(0).outerHTML;
                            }
                            return texthtml;
                        }
                    },
                    {
                        "targets": [4],
                        "sortable": true,
                        "searchable": true
                    },
                    {
                        "targets": [5],
                        "sortable": true,
                        "searchable": true
                    },

                    {
                        "targets": [6],
                        "visible": true,
                        "searchable": false
                    },
                    {
                        "targets": 7,
                        "visible": true,
                        "searchable": false,
                        "data": "7",
                        //"render": function (data, type, row, meta) {
                        //    var json = {
                        //        type: "checkbox",
                        //        class: "custom-control custom-switch custom-switch-off-danger custom-switch-on-success",
                        //        value: row[0],
                        //        'data-on': "success",
                        //        'data-off': "danger"
                        //    };

                        //    if (data === "True") {
                        //        json.checked = true;
                        //    }
                        //    return $('<input/>', json).get(0).outerHTML;
                        //}
                        "render": function (data, type, row, meta) {
                            var json = "";
                            if (data === "true" || data === "True") {
                                json = '<div class="custom-control custom-switch custom-switch-off-danger custom-switch-on-success"><input id="customSwitch' + row[0] + '" value="' + row[0] + '" checked="true" type="checkbox" class="custom-control-input" ><label class="custom-control-label" for="customSwitch' + row[0] + '"></label></div>';
                            }
                            else {
                                json = '<div class="custom-control custom-switch custom-switch-off-danger custom-switch-on-success"><input id="customSwitch' + row[0] + '" value="' + row[0] + '" type="checkbox" class="custom-control-input" ><label class="custom-control-label" for="customSwitch' + row[0] + '"></label></div>';
                            }

                            return json;
                        }
                    },

                    {
                        "targets": 8,
                        "data": "0",
                        "searchable": false,
                        "sortable": false,
                        "render": function (data, type, row, meta) {
                            var actionLink = $("<a/>", {
                                href: domain + "/Static/CreateEdit/" + row[0],
                                id: "editPostModal",
                                //class: "btn btn-primary btn-sm",
                                'title': "Edit",
                                html: $("<i/>", {
                                    class: "fa fa-edit btnEdit"
                                }),
                            }).get(0).outerHTML + "&nbsp";




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
                "sAjaxSource": domain + "/Static/Index",
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
                "stateSave": false,
                "stateSaveCallback": function (settings, data) {
                    localStorage.setItem('DataTables_' + settings.sInstance, JSON.stringify(data))
                },
                "stateLoadCallback": function (settings) {
                    return JSON.parse(localStorage.getItem('DataTables_' + settings.sInstance))
                }
            });
            table = gridstaticpage.DataTable();
        }
        function initGridControlsWithEvents() {
            //if ($('.switchBox').data('bootstrapSwitch')) {
            //    $('.switchBox').off('switchChange.bootstrapSwitch');
            //    $('.switchBox').bootstrapSwitch('destroy');
            //}

            //$('.switchBox').bootstrapSwitch()
            //    .on('switchChange.bootstrapSwitch', function () {
            //        var switchElement = this;
            //        $.get(domain + '/Static/activeStatus', { id: this.value }, function (result) {
            //            if (!result.isSuccess) {
            //                $(switchElement).bootstrapSwitch('toggleState', true);
            //            }
            //            else {

            //                alertify.success(result.data);
            //                // $("#validation-summary").html(result.data);

            //            }
            //        })
            //    });
            $('.custom-control-input').bind("click", function () {
                var switchElement = this;
                
                $.post(domain + '/Static/activeStatus', { id: this.value }, function (result) {
                      if (!result.isSuccess) {
                        
                        if ($(switchElement).is(":checked") == true) {
                            $(switchElement).attr('checked', true);
                        } else {
                            $(switchElement).attr('checked', true);
                        }

                        Global.ShowMessage(result.errorMessage, Global.MessageType.Error);
                    }
                    else {
                        if (result.message == "Status  Active successfully.") {
                            Global.ShowMessage(result.message, Global.MessageType.Success);
                        }
                        else {
                            Global.ShowMessage(result.message, Global.MessageType.Error);
                          
                        }
                    }
                });
            });

        }
        function initilizeModel() {
            $("#modal-delete").on('hidden.bs.modal', function (e) {
                $(this).removeData('bs.modal');
            });
        }
        $this.init = function () {
            initializeGrid();
            initilizeModel();
        }
    }

    $(function () {
        var self = new StaticIndex();
        self.init();
    })
})(jQuery)