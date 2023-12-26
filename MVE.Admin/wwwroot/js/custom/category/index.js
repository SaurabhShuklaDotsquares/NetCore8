(function ($) {
    function CategoryIndex() {
        var $this = this, formAddEditcategory;
        var CategoryGrid = ''; var pathwithDomain = domain + "\\UploadFiles\\Category\\";
        function initializeGrid() {
            if ($.fn.DataTable.isDataTable(CategoryGrid)) {
                $(CategoryGrid).DataTable().destroy();
            }
            var itemStatus = $("#ddlItemStatus").val();
            CategoryGrid = new Global.GridHelper('#grid-category-details', {
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
                                                src: "/Admin/Category/Thumbnail?width=100&height=80&imageFile=" + srcImageName,
                                                alt: "No Image"
                                            })
                                        })
                                    }).get(0).outerHTML;
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
                                                src: "/Admin/Category/Thumbnail?width=100&height=80&imageFile=" + srcImageName,
                                                alt: "No Image"
                                            })
                                        })
                                    }).get(0).outerHTML;
                                }
                            }
                            return actionLink;
                        }
                    },
                    { "targets": [3], "sortable": true, "searchable": true }, //Name
                    { "targets": [4], "sortable": false, "searchable": true }, //Description


                    { "targets": [5], "sortable": true, "searchable": true, "className": "text-center" },
                    {
                        "targets": [6], "data": "0", "searchable": false, "sortable": false, "className": "text-center", "visible": ($("#ParentId").val() == "0" || $("#ParentId").val() == null ? true : false),
                        "render": function (data, type, row, meta) {
                            var json = {
                                class: "text-decoration-none",
                                style: "color:#1c83c6",
                                'text': row[6],
                                'href': "/Admin/Category/Index?id=" + row[0],
                            };
                            return $('<a/>', json).get(0).outerHTML;
                        }
                    },
                    {
                        "targets": [7], "sortable": false, "searchable": false, "data": "7", "className": "text-right",
                        "render": function (data, type, row, meta) {
                            
                            var json = "";
                            if (data === "true" || data === "True" || data == "Active" || data == "active") {
                                json = '<div class="custom-control custom-switch custom-switch-off-danger custom-switch-on-success"><input data-col="status" id="customSwitch' + row[0] + '" value="' + row[0] + '" checked="true" type="checkbox" class="custom-control-input"><label class="custom-control-label" for="customSwitch' + row[0] + '"></label></div>';
                            }
                            else {
                                json = '<div class="custom-control custom-switch custom-switch-off-danger custom-switch-on-success"><input data-col="status" id="customSwitch' + row[0] + '" value="' + row[0] + '" type="checkbox" class="custom-control-input" ><label class="custom-control-label" for="customSwitch' + row[0] + '"></label></div>';
                            }

                            return json;
                        }
                    },
                    {
                        "targets": [8], "data": "0", "searchable": false, "sortable": false, "className": "text-center",
                        "render": function (data, type, row, meta) {
                            var actionLink = '';
                            actionLink += $("<a />", {
                                href: "/Admin/Category/AddEditCategory/" + row[0],
                                id: "categoryEditModalId",
                                /*class: "btn btn-primary btn-sm",*/
                                oncontextmenu: 'return false',
                                'data-toggle': "modal",
                                'data-target': "#modal-add-edit-category",
                                title: 'Edit',
                                html: $("<i/>", {
                                    class: "fa fa-edit btnEdit"
                                })
                            }).get(0).outerHTML + "&nbsp;";


                            //actionLink += $("<a/>", {
                            //    href: "/Admin/Category/ViewCategory/" + row[0],
                            //    id: "viewModalId",
                            //    /*class: "btn btn-primary btn-sm",*/
                            //    title: 'View',
                            //    oncontextmenu: 'return false',
                            //    'data-toggle': "modal",
                            //    'data-target': "#modal-view-category",
                            //    html: $("<i/>", {
                            //        class: "fa fa-eye btnView"
                            //    })
                            //}).get(0).outerHTML + "&nbsp;";
                            actionLink += $("<a/>", {
                                href: "#",
                                id: "categoryDeleteModalId",
                                /* class: "btn btn-danger btn-sm",*/
                                title: 'Delete',
                                oncontextmenu: 'return false',
                                'data-toggle': "modal",
                                'data-target': "#modal-delete-category",
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
                "sAjaxSource": "/Admin/" + "Category/Index",
                "fnServerData": function (url, data, callback) {
                    data.push({ "name": "status", "value": itemStatus });
                    data.push({ "name": "fStartDate", "value": $("#fltrStartDate").val() });
                    data.push({ "name": "fEndDate", "value": $("#fltrEndDate").val() });
                    data.push({ "name": "ParentId", "value": $("#ParentId").val() });
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


            //------------------------ Category add edit work--------------------------------------------
            $(document).on("submit", "#CategoryForm", function (event) {
                debugger
                event.preventDefault();
                event.stopImmediatePropagation();

                var fileNameValue = $('#ImageFile').val();
                var flagImageInput = $('#category_file');

                if (fileNameValue) {
                    $('[data-valmsg-for="ImageFile"]').text('');
                } else {
                    if (flagImageInput[0] && flagImageInput[0].files[0]) {

                        $('[data-valmsg-for="ImageFile"]').text('');
                    } else {
                        $('[data-valmsg-for="ImageFile"]').text('Image is required');
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
                            $("#modal-add-edit-category").modal('hide');
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
            $(document).on("click", "#categoryDeleteModalId", function (event) {
                debugger
                
            });
            
        }

        function initGridControlsWithEvents() {
            $('.custom-control-input').bind("click", function () {
                var switchElement = this;
                $.post(domain + '/Category/UpdateStatus', { id: this.value }, function (result) {

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
                        if (result.message == "Category  Active successfully.") {
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
            $("#modal-add-edit-category").on('shown.bs.modal', function (e) {
                setTimeout(function () {

                    $('.form-checkbox').bootstrapSwitch('onText', 'Yes');
                    $('.form-checkbox').bootstrapSwitch('offText', 'No');
                }, 300);

            }).on('hidden.bs.modal', function (e) {
                $(this).removeData('bs.modal');
            });


            ///--------------- initilize on change active-inactive filter---------------
            $(document).off('change', ".ddlFltr").on('change', ".ddlFltr", function () {
                CategoryGrid.empty();
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
                CategoryGrid.empty();
                initializeGrid();
            });
            $(document).off('change', ".fltrEDate").on('change', ".fltrEDate", function () {
                CategoryGrid.empty();
                initializeGrid();
            });


        }

        $this.init = function () {
            initializeGrid();
            initilizeModel();
            $("#ddlItemStatus").select2();
            Imagefancyboxbuttons();
            $(document).on('change', '#category_file', function (e) {
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
        var self = new CategoryIndex();
        self.init();
    })
})(jQuery)


function CheckValidation(obj) {

    var categoryid = $("#modal-add-edit-category").find('.modal-body').find("#Id").val();
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
    if (categoryid == null || categoryid == "" || categoryid == "0") {
        if ($("#category_file").val() == null || $("#category_file").val() == "") {
            msg = "Image required*";
            $("#validationmsg").html(msg);
            return false;
        }
    }
    else {
        $("#validationmsg").html(msg);
    }

}
