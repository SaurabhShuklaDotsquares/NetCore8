
(function ($) {
    function Index() {
        var $this = this;
        this.grid = null;

        this.bindInitialEvents = function () {
            //$(document).off('click', ".btnDelete").on('click', ".btnDelete", function () {
            //    userId = $(this).data('name');
            //    Global.Confirm('Confirm to Delete', 'Are you sure you want to delete User?', function () {
            //        $this.deleteUser(userId);
            //    });
            //});

            $('#fltrStartDate,#fltrEndDate').daterangepicker({
                singleDatePicker: true,
                timePicker: false,
                autoUpdateInput: false,
                showDropdowns: true,
                locale: {
                    format: dateFormatCalenderWithoutTime
                },
            }).on('apply.daterangepicker', function (ev, picker) {
                $(this).val(picker.endDate.format(dateFormatCalenderWithoutTime)).change();

                $('#' + ev.delegateTarget.name).removeClass('error').next('span').remove();
            }).on('keypress paste', function (e) {
                e.preventDefault();
                return false;
            }).attr("autocomplete", "off");

            $(document).off('change', ".fltrSDate").on('change', ".fltrSDate", function () {

                var newStartDate = $(this).val(); // Get the new start date from an input field

                // Update the minDate option in the date range picker
                $('#fltrEndDate').daterangepicker({
                    singleDatePicker: true,
                    timePicker: false,
                    autoUpdateInput: false,
                    showDropdowns: true,
                    minDate: newStartDate,
                    locale: {
                        format: dateFormatCalenderWithoutTime
                    },
                }).on('apply.daterangepicker', function (ev, picker) {
                    $(this).val(picker.endDate.format(dateFormatCalenderWithoutTime)).change();

                    $('#' + ev.delegateTarget.name).removeClass('error').next('span').remove();
                }).on('keypress paste', function (e) {
                    e.preventDefault();
                    return false;
                }).attr("autocomplete", "off");


               // $this.grid.empty();
               // $this.loadGrid();
            });
            $(document).off('change', ".fltrEDate").on('change', ".fltrEDate", function () {

                var newEndDate = $(this).val(); // Get the new start date from an input field

                // Update the minDate option in the date range picker
                $('#fltrStartDate').daterangepicker({
                    singleDatePicker: true,
                    timePicker: false,
                    autoUpdateInput: false,
                    showDropdowns: true,
                    maxDate: newEndDate,
                    locale: {
                        format: dateFormatCalenderWithoutTime
                    },
                }).on('apply.daterangepicker', function (ev, picker) {
                    $(this).val(picker.endDate.format(dateFormatCalenderWithoutTime)).change();

                    $('#' + ev.delegateTarget.name).removeClass('error').next('span').remove();
                }).on('keypress paste', function (e) {
                    e.preventDefault();
                    return false;
                }).attr("autocomplete", "off");
                //}
                //************** */

                //$this.grid.empty();
               // $this.loadGrid();
            });

            //$(document).off('change', ".ddlFltr").on('change', ".ddlFltr", function () {
            //    $this.grid.empty();
            //    $this.loadGrid();
            //});
            $("#UserRoleTypeId").select2();
            $("#ddlItemStatus").select2();

            $(document).on('click', '#btnSearch', function (e) {
                $this.loadGrid();
            });

            $(document).on('change', '#IsPassUpdate', function (e) {
                if (this.checked) {
                    $("#updatepass").show();
                    $("#NewPassword").val('');
                    $("#ConfirmPassword").val('');
                }
                else {
                    $("#updatepass").hide();
                }
            });

            $(document).on('click', '#btnReset', function (e) {
                location.reload();
            });

            //$(document).on('click', '#btnExport', function (e) {
            //    
            //    var Data = gridData_;
            //    Data.push({ "name": "status", "value": status_ });
            //    Data.push({ "name": "fStartDate", "value": fStartDate_ });
            //    Data.push({ "name": "fEndDate", "value": fEndDate_ });
            //    Data.push({ "name": "txtSearchFilter", "value": txtSearchFilter_ });

            //    $.ajax({
            //        url: "/Admin/FrontUser/Export",
            //        "data": Data,
            //        "success": function (data) {
            //            // Show success message before initiating download
            //            alert('File download initiated!');

            //            // Initiate file download by creating a temporary link
            //            var blob = new Blob([data]);
            //            var link = document.createElement('a');
            //            link.href = window.URL.createObjectURL(blob);
            //            link.download = 'UserDetails.csv';
            //            link.click();
            //        },
            //        "contentType": "application/x-www-form-urlencoded; charset=utf-8",
            //        "dataType": "json",
            //        "type": "POST",
            //        "cache": false,
            //        "error": function () {

            //        }
            //    });

            //    //var downloadAnchor = document.createElement('a');
            //    //downloadAnchor.href = '/YourController/YourServerEndpoint'; // Replace with your endpoint

            //    //// Trigger download
            //    //downloadAnchor.click();

            //    //$.ajax({
            //    //    url: "/Admin/FrontUser/Export",
            //    //    method: 'GET',
            //    //    success: function (data) {
            //    //        // Show success message before initiating download
            //    //        alert('File download initiated!');

            //    //        // Initiate file download by creating a temporary link
            //    //        var blob = new Blob([data]);
            //    //        var link = document.createElement('a');
            //    //        link.href = window.URL.createObjectURL(blob);
            //    //        link.download = 'UserDetails.csv';
            //    //        link.click();
            //    //    },
            //    //    error: function () {
            //    //        alert('Failed to download file.');
            //    //    }
            //    //});
            //});

        };

        this.loadGrid = function () {
            var $itemStatus = $("#ddlItemStatus").val();

            $("#statusE").val($itemStatus);
            $("#fStartDateE").val($("#fltrStartDate").val());
            $("#fEndDateE").val($("#fltrEndDate").val());
            $("#txtSearchFilterE").val($("#txtSearchFilter").val());

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
                    "sAjaxSource": "/Admin/FrontUser/Index",
                    "fnServerData": function (url, data, callback) {
                        
                        $("#requestE").val(data);

                        data.push({ "name": "status", "value": $itemStatus });
                        data.push({ "name": "fStartDate", "value": $("#fltrStartDate").val() });
                        data.push({ "name": "fEndDate", "value": $("#fltrEndDate").val() });
                        data.push({ "name": "txtSearchFilter", "value": $("#txtSearchFilter").val() });
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
                        { "targets": [0], sortable: false, searchable: false, visible: true },
                        {
                            "targets": [1], sortable: true, searchable: true, visible: true,
                            render: function (data, type, row, meta) {

                                return row[1];
                            }
                        },
                        {
                            "targets": [2], sortable: true, searchable: true, visible: true
                        },
                        {
                            "targets": [3], title: "Image", width: "15%", "sortable": false, "searchable": false,
                            "render": function (data, type, row, meta) {
                                var actionLink = $("<div />", {
                                    class: "thumb thumbnailGallery",
                                    html: $("<a />", {
                                        href: row[3] == "" ? "/Admin/images/upload-image.png" : $("#hddFrontDomain").val() + "/uploads/users/" + row[3],
                                        id: "img_" + row[0],
                                        class: "fancybox-buttons thumbnail",
                                        oncontextmenu: 'return false',
                                        'data-fullpath': row[3],
                                        'data-fancybox-group': "button",
                                        target: "_blank",
                                        title: row[3],
                                        html: $("<img/>", {
                                            class: "img-responsive",
                                            style: "width:70px; height:70px",
                                            imageFile: "",
                                            src: row[3] == "" ? "/Admin/images/upload-image.png" : $("#hddFrontDomain").val() + "/uploads/users/" + row[3],
                                            alt: 'No Image'
                                        })
                                    })
                                }).get(0).outerHTML + "&nbsp;";

                                return actionLink;
                            }
                        },
                        { "targets": [4], sortable: false, searchable: false, visible: true },

                        { "targets": [5], sortable: false, searchable: false, visible: true, "className": "text-center" },
                        {
                            "targets": [6], sortable: false, searchable: false, visible: true, "className": "text-center",
                            "render": function (data, type, row, meta) {
                                var texthtml = '';
                                if (row[6] != null && row[6].length > 30) {
                                    texthtml = $("<span/>", {
                                        title: row[6],
                                        text: row[6].slice(0, 30) + "..."
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
                        { "targets": [7], sortable: true, searchable: false, visible: true },
                        {
                            "targets": [8], sortable: false, searchable: false, visible: true, "className": "text-right",
                            render: function (data, type, row, meta) {
                                var json = '';
                                if (data === "true" || data === "True" || data == "Active" || data == "active") {
                                    json = '<div class="custom-control custom-switch custom-switch-off-danger custom-switch-on-success"><input data-col="status" id="customSwitch' + row[1] + '" value="' + row[1] + '" checked="true" type="checkbox" class="custom-control-input"><label class="custom-control-label" for="customSwitch' + row[1] + '"></label></div>';
                                }
                                else {
                                    json = '<div class="custom-control custom-switch custom-switch-off-danger custom-switch-on-success"><input data-col="status" id="customSwitch' + row[1] + '" value="' + row[1] + '" type="checkbox" class="custom-control-input" ><label class="custom-control-label" for="customSwitch' + row[1] + '"></label></div>';
                                }
                                return json;
                            }
                        },
                        {
                            "targets": [9], title: "Action", width: "7%", sortable: false, searchable: false, visible: true, "className": "text-center",
                            render: function (data, type, row, meta) {
                                var actionButtons = '';
                                //  if (row[12] == "False") {                               
                                actionButtons += $("<a/>", {
                                    id: "UserEditModalId",
                                    oncontextmenu: 'return false',
                                    'data-toggle': "modal",
                                    'data-target': "#modal-add-edit-User",
                                    title: "Edit user",
                                    href: "/Admin/FrontUser/AddEditUser/" + row[1],
                                    html: $("<i/>", {
                                        class: "fa fa-edit btnEdit"
                                    }).get(0).outerHTML
                                }).get(0).outerHTML + "&nbsp; ";
                                //  }

                                actionButtons += $("<a/>", {
                                    href: "/Admin/FrontUser/ViewFrontUser/" + row[1],
                                    id: "viewModalId",
                                    title: 'View',
                                    oncontextmenu: 'return false',
                                    'data-toggle': "modal",
                                    'data-target': "#modal-user-view",
                                    html: $("<i/>", {
                                        class: "fa fa-eye btnView"
                                    })
                                }).get(0).outerHTML;

                                actionButtons += $("<a/>", {
                                    href: "/Admin/FrontUser/Delete/" + row[1],
                                    id: "UserDeleteModalId",
                                    title: 'Delete',
                                    oncontextmenu: 'return false',
                                    'data-toggle': "modal",
                                    'data-target': "#modal-delete-user",
                                    html: $("<i/>", {
                                        class: "fa fa-trash btnDelete"
                                    })
                                }).get(0).outerHTML;



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

        this.ExportList = function () {
            alert('aaaaa');
        };

        //this.deleteUser = function (userId) {
        //    Global.AjaxPost({
        //        url: "/Admin/frontuser/delete/",
        //        updateFormData: function (formdata) {
        //            formdata.append("id", userId);
        //        }
        //    }, function (result) {
        //        if (result.isSuccess === true) {
        //            Global.ShowMessage(result.message, Global.MessageType.Success);
        //            $this.grid.empty();
        //            $this.loadGrid();
        //        }
        //        else if (result.isSuccess === false) {
        //            Global.ShowMessage(result.errorMessage, Global.MessageType.Error);
        //        } else {
        //            Global.ShowMessage("Sorry you are not authorised to perform this activity, please contact to system administrator.", Global.MessageType.Error);
        //        }
        //    }, function () {
        //        Global.Alert("Alert!", "There is somthing wrong.");
        //    })
        //};

        //function showPassword() {
        //    alert("ffdf");
        //    $("#toggle_pwd").click(function () {
        //        $(this).toggleClass("fa-eye fa-eye-slash");
        //        var type = $(this).hasClass("fa-eye-slash") ? "password" : "text";
        //        $("#Password").attr("type", type);
        //    });

        //}     

        this.initGridControlsWithEvents = function () {

            $('.custom-control-input').bind("click", function () {
                var switchElement = this;
                var $switchElement = $(this);
                $.post(domain + '/FrontUser/UserActiveInactive', { id: this.value, data: switchElement.checked, coltype: $switchElement.data('col') }, function (result) {
                    
                    if (!result.isSuccess) {
                        if ($(switchElement).is(":checked") == true) {
                            $(switchElement).attr('checked', true);
                        } else {
                            $(switchElement).attr('checked', true);
                        }
                        Global.ShowMessage(result.errorMessage, Global.MessageType.Error);
                    }
                    else {
                        Global.ShowMessage(result.message, Global.MessageType.Success);
                    }
                });
            });
        };



        function initilizeModel() {

            $("#modal-add-edit-User").on('shown.bs.modal', function (e) {

                setTimeout(function () {
                    var formAddEdittheme = new Global.FormHelperWithFiles($("#frm-add-edit-user"), {
                        updateTargetId: "validation-summary",
                        validateSettings: {
                            ignore: []
                        }
                    }, function onSuccess(result) {
                        
                        if (result) {
                            var themeMessageDiv = $('#userMessageDiv');
                            if (result.status) {
                                if (Global.IsNotNullOrEmptyString(result.message)) {
                                    themeMessageDiv.addClass('alert-success').removeClass('alert-danger');
                                    themeMessageDiv.empty().html(result.message);
                                    $this.loadGrid();
                                    $("#modal-add-edit-User").modal('hide');
                                    themeMessageDiv.show();
                                }
                            }
                            else {
                                if (Global.IsNotNullOrEmptyString(result.message)) {
                                    themeMessageDiv.addClass('alert-danger').removeClass('alert-success');
                                    themeMessageDiv.empty().html(result.message);
                                    $this.loadGrid();
                                    $("#frm-add-edit-user").modal('hide');
                                    themeMessageDiv.show();
                                }
                            }
                            window.setTimeout(function () {
                                themeMessageDiv.html('');
                                themeMessageDiv.hide();
                            }, 2000);
                        }
                    });

                    $('.form-checkbox').bootstrapSwitch('onText', 'Yes');
                    $('.form-checkbox').bootstrapSwitch('offText', 'No');
                }, 700);
            }).on('hidden.bs.modal', function (e) {
                $(this).removeData('bs.modal');
            });

        }



        this.init = function () {
            initilizeModel();
            $this.loadGrid();
            $this.bindInitialEvents();


        };
    }


    $(function () {
        var self = new Index();
        self.init();

    });

}(jQuery));




