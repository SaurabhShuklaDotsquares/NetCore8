(function ($) {
    function RoleIndex() {
        var $this = this;
        var roleDetailsGrid = '';
        var roleType = $('#RoleType').val();
        function initializeGrid() {
            if ($.fn.DataTable.isDataTable(roleDetailsGrid)) {
                $(roleDetailsGrid).DataTable().destroy();
            }
            roleDetailsGrid = new Global.GridHelper('#grid-user-role-details', {
                "columnDefs": [
                    { "targets": [0], "visible": false },
                    { "targets": [1], "visible": true, "sortable": false, "searchable": false },
                    { "targets": [2], "sortable": true, "searchable": true },
                    { "targets": [3], "sortable": true, "searchable": true, "className": "text-center" },
                    {
                        "targets": [4], "sortable": false, "searchable": false, "data": "4", "className": "text-right",
                        "render": function (data, type, row, meta) {
                            var json = '';
                            if (row[2] == "SuperAdmin") {

                                if (data === "true" || data === "True") {
                                    json = '<div class="custom-control custom-switch custom-switch-off-danger custom-switch-on-success"><input id="customSwitch' + row[0] + '" value="' + row[0] + '" checked="true" type="checkbox" class="custom-control-input"><label class="custom-control-label" for="customSwitch' + row[0] + '"></label></div>';
                                }
                                else {
                                    json = '<div class="custom-control custom-switch custom-switch-off-danger custom-switch-on-success"><input id="customSwitch' + row[0] + '" value="' + row[0] + '" type="checkbox" class="custom-control-input" ><label class="custom-control-label" for="customSwitch' + row[0] + '"></label></div>';
                                }

                            }
                            else {
                                if (row[2] != "SuperAdmin") {

                                    if (data === "true" || data === "True") {
                                        json = '<div class="custom-control custom-switch custom-switch-off-danger custom-switch-on-success"><input id="customSwitch' + row[0] + '" value="' + row[0] + '" checked="true" type="checkbox" class="custom-control-input" ><label class="custom-control-label" for="customSwitch' + row[0] + '"></label></div>';
                                    }
                                    else {
                                        json = '<div class="custom-control custom-switch custom-switch-off-danger custom-switch-on-success"><input id="customSwitch' + row[0] + '" value="' + row[0] + '" type="checkbox" class="custom-control-input" ><label class="custom-control-label" for="customSwitch' + row[0] + '"></label></div>';
                                    }
                                }
                            }


                            return json;
                        }
                    },
                    {
                        "targets": [5], "data": "0", "searchable": false, "sortable": false, "className": "text-center",
                        "render": function (data, type, row, meta) {
                            var actionLink = '';

                            if (row[2] != "SuperAdmin") {
                                actionLink = $("<a />", {
                                    href: "/Admin/UserRole/AddEdit/" + row[0],
                                    id: "roleEditModalId",
                                    oncontextmenu: 'return false',
                                    'data-toggle': "modal",
                                    'data-target': "#modal-add-edit-role",
                                    title: 'Edit',
                                    html: $("<i/>", {
                                        class: "fa fa-edit btnEdit"
                                        /*  style: "color:#0000FF"*/
                                        /*font-size: "17px"*/
                                    })
                                }).get(0).outerHTML + "&nbsp;";

                                actionLink += $("<a/>", {
                                    href: "/Admin/UserRole/Permission/" + row[0],
                                    id: "permissionId",
                                    title: 'Permission',
                                    html: $("<i/>", {
                                        class: "fa fa-check-square btnPermission"
                                        /*style: "color:#009900"*/
                                        //,
                                        //font-size: "17px"
                                    })
                                }).get(0).outerHTML + "&nbsp;";

                                actionLink += $("<a/>", {
                                    href: "/Admin/UserRole/Delete/" + row[0],
                                    id: "RoleDeleteModalId",
                                    title: 'Delete',
                                    oncontextmenu: 'return false',
                                    'data-toggle': "modal",
                                    'data-target': "#modal-delete-role",
                                    html: $("<i/>", {
                                        class: "fa fa-trash btnDelete"
                                        /*style: "color:#ff0000"*/
                                        /*font-size: "17px"*/
                                    })
                                }).get(0).outerHTML;
                            }

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
                "sAjaxSource": "/Admin/" + "UserRole/Index",
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
            //if ($('.switchBox').data('bootstrapSwitch')) {
            //    $('.switchBox').off('switchChange.bootstrapSwitch');
            //    $('.switchBox').bootstrapSwitch('destroy');
            //}

            //$('.switchBox').bootstrapSwitch()
            //    .on('switchChange.bootstrapSwitch', function () {
            //        var switchElement = this;
            //        var $roleMessageDiv = $('#roleMessageDiv');
            //        $.get(domain + '/UserRole/UpdateStatus', { id: this.value }, function (result) {
            //            if (!result.isSuccess) {
            //                $(switchElement).bootstrapSwitch('toggleState', true);
            //            }
            //            else {
            //                Global.ShowMessage(result.data, 0);
            //            }
            //        });
            //    });



            //if ($('.custom-control-input').data('bootstrapSwitch')) {
            //    $('.custom-control-input').off('switchChange.bootstrapSwitch');
            //    $('.custom-control-input').bootstrapSwitch('destroy');
            //}



            //$('.custom-control-input').bind("click", function () {
            //    var switchElement = this;
            //    var $roleMessageDiv = $('#roleMessageDiv');
            //    $.get(domain + '/UserRole/UpdateStatus', { id: this.value }, function (result) {
            //        if (!result.isSuccess) {
            //            //$(switchElement).bootstrapSwitch('toggleState', true);
            //            if ($(switchElement).is(":checked") == true) {
            //                $(switchElement).attr('checked', true);
            //            } else {
            //                $(switchElement).attr('checked', true);
            //            }
            //        }
            //        else {
            //            Global.ShowMessage(result.data, 0);
            //        }
            //    });
            //});


            $('.custom-control-input').bind("click", function () {
                var switchElement = this;
                
                $.post(domain + '/UserRole/UpdateStatus', { id: this.value }, function (result) {
                    if (!result.isSuccess) {
                       
                        //$(switchElement).bootstrapSwitch('toggleState', true);
                        if ($(switchElement).is(":checked") == true) {
                            $(switchElement).attr('checked', true);
                        } else {
                            $(switchElement).attr('checked', true);
                        }

                        Global.ShowMessage(result.errorMessage, Global.MessageType.Error);
                    }

                    else {
                       
                        //if (result.message != "" && result.isSuccess == true) {
                        if (result.message == "Role Active successfully.") {

                            Global.ShowMessage(result.message, Global.MessageType.Success);
                        }
                        else if (result.message == "Role Inactive successfully.") {
                            Global.ShowMessage(result.message, Global.MessageType.Error);

                        } else {

                            Global.ShowMessage(result.errorMessage, Global.MessageType.Error);

                        }

                    }


                });
            });


        }

        function initilizeModel() {
            $("#modal-add-edit-role").on('shown.bs.modal', function (e) {
                
                setTimeout(function () {
                    var formAddEditRole = new Global.FormHelper($("#frm-add-edit-role"), {
                        updateTargetId: "validation-summary",
                        validateSettings: {
                            ignore: []
                        }
                    }, function onSuccess(result) {
                        
                        if (result) {
                            var $roleMessageDiv = $('#roleMessageDiv');
                            if (Global.IsNotNullOrEmptyString(result.message)) {
                                $roleMessageDiv.addClass('alert-success').removeClass('alert-danger');
                                $roleMessageDiv.empty().html(result.message);
                                initializeGrid();
                                $("#modal-add-edit-role").modal('hide');
                                $roleMessageDiv.show();
                            }
                           
                            window.setTimeout(function () {
                                $roleMessageDiv.html('');
                                $roleMessageDiv.hide();
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
        var self = new RoleIndex();
        self.init();
    })
})(jQuery)