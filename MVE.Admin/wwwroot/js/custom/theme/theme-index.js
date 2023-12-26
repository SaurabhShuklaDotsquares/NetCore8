(function ($) {
    function ThemeIndex() {
        var $this = this;
        var themeGrid = '';
        var pathwithDomain = domain + "\\UploadFiles\\Theme\\";
        function initializeGrid() {
            if ($.fn.DataTable.isDataTable(themeGrid)) {
                $(themeGrid).DataTable().destroy();
            }
            var itemStatus = $("#ddlItemStatus").val();
            themeGrid = new Global.GridHelper('#grid-theme-details', {
                "columnDefs": [
                    { "targets": [0], "visible": false },
                    { "targets": [1], "visible": true, "sortable": false, "searchable": false },
                    {
                        "targets": [2], "sortable": false, "searchable": false,
                        "render": function (data, type, row, meta) {
                            var srcImageName = row[2]

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
                                            // src: "/Theme/Thumbnail?width=150&height=100&imageFile="+(currentDir + srcImageName),
                                            src: "/Admin/Admin/Theme/Thumbnail?width=100&height=80&imageFile=" + srcImageName,
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
                                            //src: "/Theme/Thumbnail?width=150&height=100&imageFile="+(currentDir + srcImageName),
                                            src: "/Admin/Theme/Thumbnail?width=100&height=80&imageFile=" + srcImageName,
                                            alt: "No Image"
                                        })
                                    })
                                }).get(0).outerHTML;
                            }

                            return actionLink;

                        }


                    },
                    { "targets": [3], "sortable": true, "searchable": true }, //Name
                    { "targets": [4], "sortable": false, "searchable": true }, //Description
                    { "targets": [5], "sortable": true, "searchable": true, "className": "text-center" },
                    {
                        "targets": [6], "sortable": false, "searchable": false, "data": "6", "className": "text-right",
                        "render": function (data, type, row, meta) {
                            //var json = {
                            //    type: "checkbox",
                            //    class: "switchBox switch-small",
                            //    value: row[0],
                            //    'data-on': "success",
                            //    'data-off': "danger",
                            //    "data-on-text": "Yes",
                            //    "data-off-text": "No"
                            //};

                            //if (data === "True") {
                            //    json.checked = true;
                            //}
                            //json = $('<input/>', json).get(0).outerHTML;

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
                        "targets": [7], "data": "0", "searchable": false, "sortable": false, "className": "text-center",
                        "render": function (data, type, row, meta) {
                            var actionLink = '';

                            actionLink = $("<a />", {
                                href: "/Admin/Theme/AddEditTheme/" + row[0],
                                id: "themeEditModalId",
                                oncontextmenu: 'return false',
                                'data-toggle': "modal",
                                'data-target': "#modal-add-edit-theme",
                                title: 'Edit',
                                html: $("<i/>", {
                                    class: "fa fa-edit btnEdit"
                                })
                            }).get(0).outerHTML + "&nbsp;";


                            actionLink += $("<a/>", {
                                href: "/Admin/Theme/Delete/" + row[0],
                                id: "themeDeleteModalId",
                                title: 'Delete',
                                oncontextmenu: 'return false',
                                'data-toggle': "modal",
                                'data-target': "#modal-delete-theme",
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
                "sAjaxSource": "/Admin/" + "Theme/Index",
                "fnServerData": function (url, data, callback) {
                    data.push({ "name": "status", "value": itemStatus });
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

            ///--------------- initilize on change active-inactive filter---------------
            $(document).off('change', ".ddlFltr").on('change', ".ddlFltr", function () {
                themeGrid.empty();
                initializeGrid()
            });


            $(document).on("submit", "#frm-add-edit-theme", function (event) {

                event.preventDefault();
                event.stopImmediatePropagation();
                

                var categoryNameValue = $('#Name').val();
                if (categoryNameValue != "") {

                    $('[data-valmsg-for="Name"]').text('');
                } else {
                    $('[data-valmsg-for="Name"]').text('Name is required.');
                    return false;
                }
                //var categoryShortNameValue = $('#ShortName').val();
                //if (categoryShortNameValue != "") {

                //    $('[data-valmsg-for="ShortName"]').text('');
                //} else {
                //    $('[data-valmsg-for="ShortName"]').text('Short name is required.');
                //    return false;
                //}




                var fileNameValue = $('#ImageName').val();
                var flagImageInput = $('#theme_file');

                if (fileNameValue) {
                    $('[data-valmsg-for="File"]').text('');
                } else {
                    if (flagImageInput[0] && flagImageInput[0].files[0]) {

                        $('[data-valmsg-for="File"]').text('');
                    } else {
                        $('[data-valmsg-for="File"]').text('Image is required.');
                        return false;
                    }
                }


                var formData = new FormData(this);
                var url = this.action;
                $.ajax({
                    url: url,
                    type: 'POST',
                    data: formData,
                    success: function (result) {
                        
                        if (result.isSuccess == true) {
                            Global.ShowMessage(result.message, Global.MessageType.Success);
                              initializeGrid();
                            //themeGrid.fnDrawCallback();
                            //themeGrid.draw();

                            $("#modal-add-edit-theme").modal('hide');
                        } else {
                            Global.ShowMessage(result.errorMessage, Global.MessageType.Error);
                        }
                    },
                    error: function (result) {
                        Global.ShowMessage(result.errorMessage == undefined ? "Please Check all details" : result.errorMessage, Global.MessageType.Error);
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

            //$('.switchBox').bootstrapSwitch()
            //    .on('switchChange.bootstrapSwitch', function () {
            //        var switchElement = this;
            //        var themeMessageDiv = $('#themeMessageDiv');
            //        $.get(domain + '/Theme/UpdateStatus', { id: this.value }, function (result) {
            //            if (!result.isSuccess) {
            //                $(switchElement).bootstrapSwitch('toggleState', true);

            //            }
            //            else {

            //                Global.ShowMessage(result.data, 0);
            //            }

            //        });
            //    });

            $('.custom-control-input').bind("click", function () {

                var switchElement = this;
                var themeMessageDiv = $('#themeMessageDiv');
                $.get(domain + '/Theme/UpdateStatus', { id: this.value }, function (result) {
                    
                    if (!result.isSuccess) {
                        //$(switchElement).bootstrapSwitch('toggleState', true);
                        if ($(switchElement).is(":checked") == true) {
                            $(switchElement).attr('checked', true);
                        } else {
                            $(switchElement).attr('checked', true);
                        }
                    }
                    else {
                        if (result.message == " Theme Status  Active successfully.") {
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

            $("#modal-add-edit-theme").on('shown.bs.modal', function (e) {

                

                var fileNameValue = $('#ImageName').val();
                var flagImageInput = $('#theme_file');

                if (fileNameValue) {
                    $('#[data-valmsg-for="File"]').text('');
                } else {
                    if (flagImageInput[0] && flagImageInput[0].files[0]) {
                        $('[data-valmsg-for="File"]').text('');
                    } else {
                        $('[data-valmsg-for="File"]').text('Image is required.');
                        return false;
                    }
                }
                setTimeout(function () {
                    var formAddEdittheme = new Global.FormHelperWithFiles($("#frm-add-edit-theme"), {
                        updateTargetId: "validation-summary",
                        validateSettings: {
                            ignore: []
                        }
                    }, function onSuccess(result) {
                        if (result) {

                            var themeMessageDiv = $('#themeMessageDiv');
                            if (result.status) {
                                if (Global.IsNotNullOrEmptyString(result.message)) {
                                    themeMessageDiv.addClass('alert-success').removeClass('alert-danger');
                                    themeMessageDiv.empty().html(result.message);
                                    initializeGrid();
                                    $("#modal-add-edit-theme").modal('hide');
                                    themeMessageDiv.show();
                                }
                            }
                            else {
                                if (Global.IsNotNullOrEmptyString(result.message)) {
                                    themeMessageDiv.addClass('alert-danger').removeClass('alert-success');
                                    themeMessageDiv.empty().html(result.message);
                                    initializeGrid();
                                    $("#modal-add-edit-theme").modal('hide');
                                    themeMessageDiv.show();
                                }
                            }


                            window.setTimeout(function () {
                                themeMessageDiv.html('');
                                themeMessageDiv.hide();
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
            
            //////   initilizeModel();
            $('.fancybox-buttons').fancybox({
                openEffect: 'elastic',
                closeEffect: 'swing',
                prevEffect: 'changeOut',
                nextEffect: 'changeIn',
                closeBtn: true,
                helpers: {
                    title: {
                        type: 'inside'
                    },
                    buttons: {}
                },

                afterLoad: function () {
                    this.title = 'Image ' + (this.index + 1) + ' of ' + this.group.length + (this.title ? ' - ' + this.title : '');
                }
            });
            $(document).on('change', '#theme_file', function (e) {
                
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
            $("#ddlItemStatus").select2();
        };
    }

    $(function () {
        var self = new ThemeIndex();
        self.init();
    })
})(jQuery)