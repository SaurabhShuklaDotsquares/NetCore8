(function ($) {
    function Index() {
        var $this = this;
        this.grid = null;

        this.bindInitialEvents = function () {
            $(document).off('click', ".btnDelete").on('click', ".btnDelete", function () {
                //userId = $(this).data('name');
                //Global.Confirm('Confirm to Delete', 'Are you sure you want to delete User?', function () {
                //    $this.deleteUser(userId);
                //});
            });

            $(document).off('change', ".ddlFltr").on('change', ".ddlFltr", function () {
                $this.grid.empty();
                $this.loadGrid();
            });
            $("#UserRoleTypeId").select2();
            $("#ddlItemStatus").select2();
        };

        this.loadGrid = function () {
            var $itemStatus = $("#ddlItemStatus").val();
            var $cId = $("#companyId").val();
            var isArchived = false;
            //if ($itemStatus == 'archived') {
            //    isArchived = true;
            //}
            $this.grid = new Global.GridHelper('#gridUsers',
                {
                    "direction": "rtl",
                    "dom": '<"pull-left"B><"pull-right"f>rt<"row"<"col-md-8 d-flex flex-wrap align-items-center showing"li><"col-md-4"p>>',
                    //  "searching": true,
                    "destroy": true,
                    "bPaginate": true,
                    "sPaginationType": "simple_numbers",
                    "bProcessing": true,
                    "bServerSide": true,
                    "bAutoWidth": false,
                    "stateSave": false,
                    "sAjaxSource": "/Admin/AdminUser/Index",
                    "fnServerData": function (url, data, callback) {
                        //data.push({ "name": "companyId", "value": $cId });
                        data.push({ "name": "status", "value": $itemStatus });
                        //data.push({ "name": "bookingEngineSupplierId", "value": $("#BookingEngineSupplierId").val() });
                        data.push({ "name": "userRoleTypeId", "value": $("#UserRoleTypeId").val() });
                        //data.push({ "name": "saCompanySelectId", "value": $("#SACompanySelectId").val() });
                        data.push({ "name": "txtSearchFilter", "value": $("#txtSearchFilter").val() });
                        data.push({ "name": "isArchived", "value": isArchived });
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
                        { "targets": [0], sortable: false, searchable: false, visible: false },
                        {
                            "targets": [1], sortable: false, searchable: false, visible: true,
                            render: function (data, type, row, meta) {
                                return row[1];
                            }
                        },
                        {
                            "targets": [2], sortable: true, searchable: false, visible: true,
                            render: function (data, type, row, meta) {
                                
                                if (row[9] == "1") {
                                    var json = {
                                        'text': row[2],
                                    };
                                    return $('<a/>', json).get(0).outerHTML;
                                }
                                else {
                                    var json = {
                                        class: "text-decoration-none",
                                        style: "color:#1c83c6",
                                        'text': row[2],
                                        'href': "javascript:void(0)",
                                        'cursor': "none",
                                        'href': "/Admin/adminuser/ViewUserDetails/" + row[0],
                                        'data-toggle': "modal",
                                        'data-target': "#modal-view",
                                    };
                                    return $('<a/>', json).get(0).outerHTML;
                                }
                            }
                        },
                        { "targets": [3], sortable: true, searchable: true, visible: true },
                        { "targets": [4], sortable: false, searchable: false, visible: true },                        
                        { "targets": [5], sortable: false, searchable: false, visible: true ,"className": "text-center" },
                        {
                            "targets": [6], sortable: false, searchable: false, visible: true,
                            render: function (data, type, row, meta) {
                                var texthtml = '';
                                if (row[6]!=null&&row[6].length > 40) {
                                    texthtml = $("<span/>", {
                                        title: row[6],
                                        text: row[6].slice(0, 40) + "..."
                                    }).get(0).outerHTML;
                                } else {
                                    texthtml = $("<span/>", {
                                        title: row[6],
                                        text: row[6]
                                    }).get(0).outerHTML;
                                }
                                return texthtml;
                            }
                        },
                        { "targets": [7], sortable: true, searchable: true, visible: true, "className": "text-center" },
                        {
                            "targets": [8], sortable: false, searchable: false, visible: true, "className": "text-right",
                            render: function (data, type, row, meta) {                                
                                var json = "";
                                
                                if (row[9] == "1") {
                                   
                                }
                                else {
                                    if (data === "true" || data === "True") {
                                        json = '<div class="custom-control custom-switch custom-switch-off-danger custom-switch-on-success"><input id="customSwitch' + row[0] + '" value="' + row[0] + '" checked="true" type="checkbox" class="custom-control-input" ><label class="custom-control-label" for="customSwitch' + row[0] + '"></label></div>';
                                    }
                                    else {
                                        json = '<div class="custom-control custom-switch custom-switch-off-danger custom-switch-on-success"><input id="customSwitch' + row[0] + '" value="' + row[0] + '" type="checkbox" class="custom-control-input" ><label class="custom-control-label" for="customSwitch' + row[0] + '"></label></div>';
                                    }
                                }

                                return json;
                            }
                        },
                        {
                            "targets": [9], sortable: false, searchable: false, visible: true, "className": "text-center",
                            render: function (data, type, row, meta) {
                                var actionButtons = '';
                             //   actionButtons += $("<a/>", {
                                //    title: "Manage User",
                                //    class: "btn btn-default btn-sm",
                                //    href: "/adminuser/manage/" + row[0],
                                //    html: $("<i/>", {
                                //        class: "fa fa-eye"
                                //    }).get(0).outerHTML
                                //}).get(0).outerHTML + "&nbsp; ";

                                if (row[9] == "1") {

                                }
                                else {
                                    actionButtons += $("<a/>", {
                                        title: "Edit User",
                                        href: "/Admin/adminuser/manage/" + row[0],
                                        html: $("<i/>", {
                                            class: "fa fa-edit btnEdit"
                                        }).get(0).outerHTML
                                    }).get(0).outerHTML + "&nbsp; ";

                                    actionButtons += $("<a/>", {
                                        href: "/Admin/adminuser/ViewUserDetails/" + row[0],
                                        id: "view",
                                        title: 'View',
                                        oncontextmenu: 'return false',
                                        'data-toggle': "modal",
                                        'data-target': "#modal-view",
                                        html: $("<i/>", {
                                            class: "fa fa-eye btnView"
                                        })
                                    }).get(0).outerHTML;

                                    actionButtons += $("<a/>", {
                                        href: "/Admin/AdminUser/Delete/" + row[0],
                                        id: "AdminUserDeleteModalId",
                                        title: 'Delete',
                                        oncontextmenu: 'return false',
                                        'data-toggle': "modal",
                                        'data-target': "#modal-delete-adminuser",
                                        html: $("<i/>", {
                                            class: "fa fa-trash btnDelete"
                                        })
                                    }).get(0).outerHTML;
                                }                               

                                return actionButtons;
                            }
                        }
                    ],
                    "fnDrawCallback": function (oSettings) {
                        $this.initGridControlsWithEvents();
                        $('.form-checkbox').bootstrapSwitch();

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
            $(document).off('keyup', "#txtSearchFilter").on('keyup', "#txtSearchFilter", function () {
                var table = $this.grid.DataTable();
                table.search($("#txtSearchFilter").val()).draw();
            });
        };

       

        this.deleteUser = function (userId) {
            Global.AjaxPost({
                url: "/Admin/adminuser/delete/",
                updateFormData: function (formdata) {
                    formdata.append("id", userId);
                }
            }, function (result) {
                if (result.isSuccess === true) {
                    Global.ShowMessage(result.message, Global.MessageType.Success);
                    $this.grid.empty();
                    $this.loadGrid();
                }
                else if (result.isSuccess === false) {
                    Global.ShowMessage(result.errorMessage, Global.MessageType.Error);
                } else {
                    Global.ShowMessage("Sorry you are not authorised to perform this activity, please contact to system administrator.", Global.MessageType.Error);
                }
            }, function () {
                Global.Alert("Alert!", "There is somthing wrong.");
            })
        };


        this.initGridControlsWithEvents = function () {
           
            $('.custom-control-input').bind("click", function () {
                
                var switchElement = this;
                $.post(domain + '/adminuser/UpdateStatus', { id: this.value }, function (result) {
                    
                    if (!result.isSuccess) {
                        if ($(switchElement).is(":checked") == true) {
                            $(switchElement).attr('checked', true);
                        } else {
                            $(switchElement).attr('checked', true);
                        }

                        Global.ShowMessage(result.errorMessage, Global.MessageType.Error);
                    }
                    else {
                        if (result.message == "User Status Active successfully.") {
                            Global.ShowMessage(result.message, Global.MessageType.Success);                           
                        }
                        else {
                            Global.ShowMessage(result.message, Global.MessageType.Error);                           
                        }
                    }
                });
            });
        };


        this.init = function () {
            $this.loadGrid();
            $this.bindInitialEvents();
        };
    }

    $(function () {
        var self = new Index();
        self.init();
    });
}(jQuery));