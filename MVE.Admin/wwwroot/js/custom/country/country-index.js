(function ($) {
    function CountryIndex() {
        var $this = this, formAddEditcountry;
        var CountryGrid = ''; var pathwithDomain = domain + "\\UploadFiles\\Country\\";
        function initializeGrid() {
            if ($.fn.DataTable.isDataTable(CountryGrid)) {
                $(CountryGrid).DataTable().destroy();
            }
            var itemStatus = $("#ddlItemStatus").val();
            CountryGrid = new Global.GridHelper('#grid-country-details', {
                "columnDefs": [
                    { "targets": [0], "visible": false },
                    { "targets": [1], "visible": true, "sortable": false, "searchable": false },                    
                    {
                        "targets": [2], "sortable": false, "searchable": false,
                        "render": function (data, type, row, meta) {
                            var srcImageName = row[2]
                            if (row[2] == "") {
                                var actionLink = $("<span />", {
                                    text: "No Image"
                                }).get(0).outerHTML;
                            } else {
                                if (row[2].split(".")[1] == ".pdf") {
                                    srcImageName = "pdf.png";
                                    var actionLink = $("<div />", {
                                        class: "thumb thumbnailGallery",
                                        html: $("<a />", {
                                            href: pathwithDomain + row[2],
                                            id: "img_" + row[0],
                                            class: "fancybox-buttons thumbnail",
                                            oncontextmenu: 'return false',
                                            'data-fullpath': pathwithDomain + row[2],
                                            'data-fancybox-group': "button",
                                            target: "_blank",
                                            title: row[2],
                                            html: $("<img/>", {
                                                class: "img-responsive",
                                                imageFile: "",
                                                src: "/Admin/Country/Thumbnail?width=100&height=80&imageFile=" + srcImageName,
                                                alt: "No Image"
                                            })
                                        })
                                    }).get(0).outerHTML ;
                                } else {
                                    var actionLink = $("<div />", {
                                        class: "thumb thumbnailGallery",
                                        html: $("<a />", {
                                            href: pathwithDomain + row[2],
                                            id: "img_" + row[0],
                                            class: "fancybox-buttons thumbnail",
                                            oncontextmenu: 'return false',
                                            'data-fullpath': pathwithDomain + row[2],
                                            'data-fancybox-group': "button",
                                            target: "_blank",
                                            title: row[2],
                                            html: $("<img/>", {
                                                class: "img-responsive",
                                                imageFile: "",
                                                src: "/Admin/Country/Thumbnail?width=100&height=80&imageFile=" + srcImageName,
                                                alt: "No Image"
                                            })
                                        })
                                    }).get(0).outerHTML ;
                                }
                            }
                            return actionLink;
                        }
                    },
                    { "targets": [3], "sortable": true, "searchable": true }, //Name
                    //{ "targets": [4], "sortable": false, "searchable": true }, //Description
         

                    { "targets": [4], "sortable": true, "searchable": true, "className": "text-center" },
                    {
                        "targets": [5], "sortable": false, "searchable": false, "data": "5", "className": "text-right",
                        "render": function (data, type, row, meta) {
                            var json ="";
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
                        "targets": [6], "data": "0", "searchable": false, "sortable": false, "className": "text-center",
                        "render": function (data, type, row, meta) {
                            var actionLink = '';

                            actionLink += $("<a/>", {
                                href: "/Admin/ManageDestinationGuide/index?id=" + row[0],
                                id: "viewDestinationManager",
                                class: "AddNewPackageToIncludeFeedbackcls",
                                title: 'Destination Manager',
                                /*html: "<img src='" + domain + "/images/view-quote.png' />"*/
                                html: $("<i/>", {
                                    class: "fa fa-gear text-info"
                                })
                            }).get(0).outerHTML + "&nbsp;";


                            actionLink += $("<a/>", {
                                href: "/Admin/VisaGuide/index?id=" + row[0],
                                id: "viewVisaGuideManager",
                                class: "AddNewPackageToIncludeFeedbackcls",
                                title: 'VisaGuide Manager',
                                html: $("<i/>", {
                                    class: "fa fa-id-card text-info"
                                })
                            }).get(0).outerHTML + "&nbsp;";


                            actionLink += $("<a/>", {
                                href: "/Admin/Country/ViewCountryVisaGuid/" + row[0],
                                id: "viewvisaModalId",
                                /*class: "btn btn-primary btn-sm",*/
                                title: 'View Visa Guide',
                                oncontextmenu: 'return false',
                                'data-toggle': "modal",
                                'data-target': "#modal-view-visaguide",
                                html: $("<i/>", {
                                    class: "fa fa-eye btnView"
                                })
                            }).get(0).outerHTML + "&nbsp;";



                            actionLink += $("<a />", {
                                href: "/Admin/Country/AddEditCountry/" + row[0],
                                id: "countryEditModalId",
                                /*class: "btn btn-primary btn-sm",*/
                                oncontextmenu: 'return false',
                                'data-toggle': "modal",
                                'data-target': "#modal-add-edit-country",
                                title: 'Edit',
                                html: $("<i/>", {
                                    class: "fa fa-edit btnEdit"
                                })
                            }).get(0).outerHTML + "&nbsp;";


                            actionLink += $("<a/>", {
                                href: "/Admin/Country/ViewCountry/" + row[0],
                                id: "viewModalId",
                                /*class: "btn btn-primary btn-sm",*/
                                title: 'View',
                                oncontextmenu: 'return false',
                                'data-toggle': "modal",
                                'data-target': "#modal-view-country",
                                html: $("<i/>", {
                                    class: "fa fa-eye btnView"
                                })
                            }).get(0).outerHTML + "&nbsp;";
                            actionLink += $("<a/>", {
                                href: "/Admin/Country/Delete/" + row[0],
                                id: "countryDeleteModalId",
                               /* class: "btn btn-danger btn-sm",*/
                                title: 'Delete',
                                oncontextmenu: 'return false',
                                'data-toggle': "modal",
                                'data-target': "#modal-delete-country",
                                html: $("<i/>", {
                                    class: "fa fa-trash btnDelete"
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
                "sAjaxSource": "/Admin/" + "Country/Index",
                "fnServerData": function (url, data, callback) {
                    data.push({ "name": "status", "value": itemStatus });
                    data.push({ "name": "fStartDate", "value": $("#fltrStartDate").val() });
                    data.push({ "name": "fEndDate", "value": $("#fltrEndDate").val() });
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


            //------------------------ country add edit work--------------------------------------------
            $(document).on("submit", "#newsLetterForm", function (event) {
                event.preventDefault();
                event.stopImmediatePropagation();
                
                var fileNameValue = $('#FileName').val();
                var flagImageInput = $('#country_file');

                if (fileNameValue) {
                    $('[data-valmsg-for="FlagImage"]').text('');
                } else {
                    if (flagImageInput[0] && flagImageInput[0].files[0]) {
                       
                        $('[data-valmsg-for="FlagImage"]').text('');
                    } else {
                        $('[data-valmsg-for="FlagImage"]').text('Image is required');
                        return false;
                    }
                }

                var formData = new FormData(this);
                var url = this.action;//this[0].action; // if this does not work then use '@Url.Action("Create","NewsletterSubscriptions")'
                $.ajax({
                    url: url,
                    type: 'POST',
                    data: formData,
                    success: function (result) {
                        if (result.isSuccess == true) {
                            Global.ShowMessage(result.message, Global.MessageType.Success);
                            initializeGrid();
                            $("#modal-add-edit-country").modal('hide');
                        } else {
                            Global.ShowMessage(result.message, Global.MessageType.Error);
                        }
                    },
                    error: function (result) {
                        Global.ShowMessage(result.message == undefined ? "Please Check all details" : result.message, Global.MessageType.Error);
                    },
                    cache: false,
                    contentType: false,
                    processData: false
                });
                return false;
            });


        }

        function initGridControlsWithEvents() {
            //if ($('.switchBox').data('bootstrapSwitch')) {
            //    $('.switchBox').off('switchChange.bootstrapSwitch');
            //    $('.switchBox').bootstrapSwitch('destroy');
            //}

            //$('.switchBox').bootstrapSwitch().on('switchChange.bootstrapSwitch', function () {
            //    var switchElement = this;
            //    $.post(domain + '/Country/UpdateStatus', { id: this.value }, function (result) {
            //        if (!result.isSuccess) {
            //            $(switchElement).bootstrapSwitch('toggleState', true);
            //            Global.ShowMessage(result.errorMessage, Global.MessageType.Error);
            //        }
            //        else {
            //            Global.ShowMessage(result.message, Global.MessageType.Success);

            //        }

            //    });
            //});

            $('.custom-control-input').bind("click", function () {
                var switchElement = this;
                $.post(domain + '/Country/UpdateStatus', { id: this.value }, function (result) {
                    
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
                        if (result.message == "Country  Active successfully.") {
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
            $("#modal-add-edit-country").on('shown.bs.modal', function (e) {
                setTimeout(function () {

                    $('.form-checkbox').bootstrapSwitch('onText', 'Yes');
                    $('.form-checkbox').bootstrapSwitch('offText', 'No');
                }, 300);

            }).on('hidden.bs.modal', function (e) {
                $(this).removeData('bs.modal');
            });


            ///--------------- initilize on change active-inactive filter---------------
            $(document).off('change', ".ddlFltr").on('change', ".ddlFltr", function () {
                CountryGrid.empty();
                initializeGrid()
            });


            ///--------------- initilize datepicker---------------
            $('#fltrStartDate,#fltrEndDate').daterangepicker({
                singleDatePicker: true,
                timePicker: false,
                autoUpdateInput: false,
                showDropdowns: true,
                locale: {
                    format: dateFormatCalenderWithoutTime
                }
            }).on('apply.daterangepicker', function (ev, picker) {
                $(this).val(picker.endDate.format(dateFormatCalenderWithoutTime)).change();

                $('#' + ev.delegateTarget.name).removeClass('error').next('span').remove();
            }).on('keypress paste', function (e) {
                e.preventDefault();
                return false;
            }).attr("autocomplete", "off");
            $(document).off('change', ".fltrSDate").on('change', ".fltrSDate", function () {
                CountryGrid.empty();
                initializeGrid();
            });
            $(document).off('change', ".fltrEDate").on('change', ".fltrEDate", function () {
                CountryGrid.empty();
                initializeGrid();
            });


        }

        $this.init = function () {
            initializeGrid();
            initilizeModel();
            $("#ddlItemStatus").select2();
            Imagefancyboxbuttons();


            $(document).on('change', '#country_file', function (e) {
                var reader = new FileReader();
                var files = event.target.files;
                if (files.length > 0) {
                    var extension = files[0].name.substr((files[0].name.lastIndexOf('.') + 1));
                    if (extension != "jpeg" && extension != "jpg" && extension != "png" && extension != "JPEG" && extension != "JPG" && extension != "PNG") {
                        Global.ShowMessage('Please choose jpeg/jpg/ png.', Global.MessageType.Error);
                        $('#imgCategory').val('');
                        return;
                    }
                    reader.onload = function () {
                        var output = document.getElementById('imgCategory');
                        output.src = reader.result;
                    };
                    reader.readAsDataURL(event.target.files[0]);
                }
            });
        };
    }

    $(function () {
        var self = new CountryIndex();
        self.init();
    })
})(jQuery)


function CheckValidation(obj) {
    
    var countryid = $("#modal-add-edit-country").find('.modal-body').find("#Id").val();
    var msg = "";
    if ($("#Name").val() == null || $("#Name").val() == "") {
        msg = "Name required*";
        $("#validationmsg").html(msg);
        return false;
    }
    if ($("#Code").val() == null || $("#Code").val() == "") {
        msg = "Code required*";
        $("#validationmsg").html(msg);
        return false;
    }
    if (countryid == null || countryid == "" || countryid == "0") {
        if ($("#country_file").val() == null || $("#country_file").val() == "") {
            msg = "Image required*";
            $("#validationmsg").html(msg);
            return false;
        }
    }
    else {
        $("#validationmsg").html(msg);
    }

}
