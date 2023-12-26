var IsLimitUploadImageCount = -1;
var IsLimitUploadVideoCount = -1;
var IsRepeatCount = 0;
var TypeIV = 0;

var count_, cls_, obj_, TypeIV_;


(function ($) {
    function FileUploadCustom() {
        var $this = this;
        this._validFileExtensions = [".jpg", ".jpeg", ".bmp", ".gif", ".png"];
        this._validVideoFileExtensions = [".mp4", ".mov", ".wmv", ".webem", ".avi", ".flv"];
        var message1 = "";
        var message2 = "";
        var message3 = "";
        var message4 = "";
        var message5 = "";
        var message6 = "";
        var message7 = "";
        var messageCount = 0;
        var messageCount2 = 0;
        var messageCount3 = 0;
        var messageCount4 = 0;
        var messageCount5 = 0;
        var messageCount6 = 0;
        var messageCount7 = 0;
        this.setFileDetail = function ($uploader, file, $imgControl, $streamControl) {
            message1 = "";
            message2 = "";
            message3 = "";
            message4 = "";
            message5 = "";
            message6 = "";
            message7 = "";
            if (file != null) {
                
                var ext = file.type.toLowerCase();
                if ($uploader.attr("accept").indexOf('image/*') >= 0 && $.inArray(ext, ['image/gif', 'image/png', 'image/jpg', 'image/jpeg']) == -1) {
                    $uploader.value = '';
                    $imgControl.attr('src', domain + "/images/upload-image.png");
                    message2 = "Only " + $this._validFileExtensions.join(", ") + " images are allowed* ";
                    //Global.ShowMessage("Only " + $this._validFileExtensions.join(", ") + " images are allowed* ", Global.MessageType.Error);
                    //return false;
                }
                else if ($uploader.attr("accept").indexOf('image/*') >= 0 && Math.round(file.size / (1024 * 1024)) > 5) {
                    $imgControl.attr('src', domain + "/images/upload-image.png" + "?h=145&w=212");
                    $uploader.value = '';
                    $uploader.value = domain + "/images/upload-image.png" + "?h=145&w=212";
                    message1 = 'Max upload image size is 5MB only';
                    //Global.ShowMessage('Max upload image size is 5MB only', Global.MessageType.Error);
                    //return false;
                }

                //if ($uploader.attr("accept").indexOf('video/mp4') >= 0 && $.inArray(ext, ['video/mp4']) == -1) {
                //    $uploader.value = '';
                //    $imgControl.attr('src', domain + "/images/upload-image.png");
                //    message3 = "Only " + $this._validVideoFileExtensions.join(", ") + " images are allowed*";
                //    //Global.ShowMessage("Only " + $this._validVideoFileExtensions.join(", ") + " images are allowed*", Global.MessageType.Error);
                //    //return false;
                //}
                if ($uploader.attr("accept").indexOf('video/*') >= 0 && $.inArray(ext, ['video/mp4', 'video/mov', 'video/wmv', 'video/webem', 'video/avi', 'video/flv']) == -1) {
                    $uploader.value = '';
                    $imgControl.attr('src', domain + "/images/upload-image.png");
                    message3 = "Only " + $this._validVideoFileExtensions.join(", ") + " videos are allowed*";
                    //Global.ShowMessage("Only " + $this._validVideoFileExtensions.join(", ") + " images are allowed*", Global.MessageType.Error);
                    //return false;
                }
                else if ($uploader.attr("accept").indexOf('video/*') >= 0 && Math.round(file.size / (1024 * 1024)) > 15) {
                    $imgControl.attr('src', domain + "/images/upload-image.png" + "?h=145&w=212");
                    $uploader.value = '';
                    $uploader.value = domain + "/images/upload-image.png" + "?h=145&w=212";
                    message4 = 'Max upload video size is 15MB only';
                    //Global.ShowMessage('Max upload video size is 5MB only', Global.MessageType.Error);
                    // return false;
                }
                else if (IsLimitUploadImageCount != -1) {
                    if (TypeIV_ == "i") {
                        $imgControl.attr('src', domain + "/images/upload-image.png" + "?h=145&w=212");
                        $uploader.value = '';
                        message6 = 'Maximum ' + IsLimitUploadImageCount + ' file can be uploaded';
                        //Global.ShowMessage('Max upload video size is 5MB only', Global.MessageType.Error);
                        // return false;
                    }
                }
                else if (IsLimitUploadImageCount == -1) {
                    if (TypeIV_ == "i") {
                        onSelectImageCallPackage(count_, cls_, obj_, TypeIV_);
                    }
                }



                if (IsLimitUploadVideoCount != -1) {
                    if (TypeIV_ == "v") {
                        $imgControl.attr('src', domain + "/images/upload-image.png" + "?h=145&w=212");
                        $uploader.value = '';
                        message6 = 'Maximum ' + IsLimitUploadVideoCount + ' file can be uploaded';
                        //Global.ShowMessage('Max upload video size is 5MB only', Global.MessageType.Error);
                        // return false;
                    }
                }
                else if (IsLimitUploadVideoCount == -1) {
                    if (TypeIV_ == "v") {
                        onSelectVideoCallPackage(count_, cls_, obj_, TypeIV_);
                    }
                }


                if (message1 != "" && messageCount == 0) {
                    messageCount = 1;
                    Global.ShowMessage(message1, Global.MessageType.Error);
                    return false;
                }
                if (message1 != "" && messageCount == 1) {
                    return false;
                }

                if (message2 != "" && messageCount2 == 0) {
                    messageCount2 = 1;
                    Global.ShowMessage(message2, Global.MessageType.Error);
                    return false;
                }
                if (message2 != "" && messageCount2 == 1) {
                    return false;
                }

                if (message3 != "" && messageCount3 == 0) {
                    messageCount3 = 1;
                    Global.ShowMessage(message3, Global.MessageType.Error);
                    return false;
                }
                if (message3 != "" && messageCount3 == 1) {
                    return false;
                }

                if (message4 != "" && messageCount4 == 0) {
                    messageCount4 = 1;
                    Global.ShowMessage(message4, Global.MessageType.Error);
                    return false;
                }
                if (message4 != "" && messageCount4 == 1) {
                    return false;
                }

                if (message6 != "" && IsRepeatCount == 0) {
                    IsRepeatCount = 1;
                    messageCount6 = 1;
                    Global.ShowMessage(message6, Global.MessageType.Error);
                    return false;
                }

                if (message7 != "" && IsRepeatCount == 0) {
                    IsRepeatCount = 1;
                    Global.ShowMessage(message7, Global.MessageType.Error);
                    return false;
                }

                if (message5 != "" && messageCount5 == 0) {
                    messageCount5 = 1;
                    Global.ShowMessage(message5, Global.MessageType.Error);
                    return false;
                }
                if (message5 != "" && messageCount5 == 1) {
                    return false;
                }

                if (message6 != "" && messageCount6 == 1) {
                    return false;
                }

                if (file.name.length > 150) {
                    $imgControl.attr('src', domain + "/images/upload-image.png" + "?h=145&w=212");
                    $uploader.value = '';
                    message5 = 'File name too long, max file name can be 150 character\'s only.';
                    //Global.ShowMessage('File name too long, max file name can be 150 character\'s only.', Global.MessageType.Error);
                    // return false;
                }
                else {
                    var reader = new FileReader();
                    reader.onload = function (e) {
                        $imgControl.fadeOut('fast', function () {
                            $imgControl.attr('src', e.target.result);
                            $streamControl.val(e.target.result);
                            $imgControl.fadeIn('fast');
                        });
                    };
                    reader.readAsDataURL(file);

                    return true;
                }
            }
        };
        message1 = "";
        messageCount = 0;

        message2 = "";
        messageCount2 = 0;

        message3 = "";
        messageCount3 = 0;

        message4 = "";
        messageCount4 = 0;

        message5 = "";
        messageCount5 = 0;

        message6 = "";
        messageCount6 = 0;

        message7 = "";
        messageCount7 = 0;

        this.reindexTitleControls = function () {

        };

        this.resetImageCtrlIndexing = function ($uploader) {

            var propertyName = $uploader.data("property-name");
            var titlePropertyName = $uploader.data("title-property-name");
            var parentControlName = $uploader.data("parent-ctrl-name");
            var $ctrlContainer = $uploader.parents("div.app-upload-img").find("div.actual-uploaded:last img, div.actual-uploaded:last video").parents("div.actual-uploaded:first");
            var $imagesMainContainer = $ctrlContainer.parent();
            var $entityId = $ctrlContainer.find("[ID$=_EntityId]");
            var $sectionId = $ctrlContainer.find("[ID$=_SectionId]");
            var $name = $ctrlContainer.find("[ID$=_Name]");
            var $fileSize = $ctrlContainer.find("[ID$=_FileSize]");
            var $fileOriginalName = $ctrlContainer.find("[ID$=_FileOriginalName]");
            //var $id = $ctrlContainer.find("[ID$=_Id]");
            var $id = $ctrlContainer.find("[ID$=_FId]");
            var $fileStreams = $ctrlContainer.find("[ID$=_FileStreams]");
            var $fileExtension = $ctrlContainer.find("[ID$=_FileExtension]");
            var $imageOrder = $ctrlContainer.find("[ID$=_ImageOrder]");



            if (parentControlName && Global.IsNotNullOrEmptyString(parentControlName)) {
                if (parentControlName && parentControlName.length > 0) {
                    var parentPropertyName = propertyName.substring(0, propertyName.lastIndexOf('['));

                    var $parentCtrl = $uploader.parents(".flexible-content-box:first").find('[id^="' + parentPropertyName.replace(/\[/g, '_').replace(/\]/g, '_').replace(/\./g, '_') + '_"][id$="_' + parentControlName + '"]:first');
                    if ($parentCtrl && $parentCtrl.length > 0) {
                        parentPropertyName = $parentCtrl.attr("name");
                        parentPropertyName = parentPropertyName.substring(0, parentPropertyName.lastIndexOf('.') + 1);
                    }
                }

                if (parentPropertyName && Global.IsNotNullOrEmptyString(parentPropertyName)) {
                    propertyName = parentPropertyName + (propertyName.substring(propertyName.lastIndexOf(']') + 2, propertyName.length));
                }
            }
            Global.ResetCtrlIdIndexing(propertyName, $imagesMainContainer, $entityId, $sectionId, $name, $fileSize, $fileOriginalName, $id, $fileStreams, $fileExtension, $imageOrder);

            var $uploader1 = $("[ID$=_Title]").first().parents(".app-upload-img:first").find("input[type=file].app-img-uploader");

            $uploader1.parents("div.app-upload-img").find("div.actual-uploaded").each(function () {
                $entityId = $(this).find("[ID$=_EntityId]");
                var imageTitlePropertyName = $entityId.attr("name").substring(0, $entityId.attr("name").lastIndexOf("]") + 1) + ".Titles";
                var $titleContainer = $entityId.parent().find("div.img-title-container:first");
                var $imageTitle = $(this).find("[ID$=_Title]");
                //var $imageDescription = $(this).find("[ID$=_Description]");
                var $imageTitleLanguageId = $(this).find("[ID$=_LanguageId]");
                if ($titleContainer.length > 0) {
                    Global.ResetCtrlIdIndexing(imageTitlePropertyName, $titleContainer, $imageTitle, $imageTitleLanguageId);
                }
            });
            //$this.reindexTitleControls();
        };

        this.addNoImage = function ($imgContainer) {
            $("<div/>", {
                class: "app-uploaded-img app-no-image",
                html: function () {
                    $("<div/>", {
                        class: "reapeat-upload-img",
                        html: function () {
                            $("<img/>", {
                                src: domain + "/images/upload-image.png",
                                alt: "no-image",
                            }).appendTo(this);
                        }
                    }).appendTo(this);
                }
            }).appendTo($imgContainer);
        };

        this.fnRemoveImage = function ($imgCtrl, $uploader) {
            Global.Confirm("Confirmation!", "Are you sure you want to remove?", function () {
                if (!$uploader.prop("multiple")) {
                    var $valuesContainer = $imgCtrl.parents("div.app-upload-img:first");
                    if ($valuesContainer.find('video').length == 1) {
                        $valuesContainer.find("label.upload-photo:first").html("<i class='fa fa-plus'></i> Add video");
                    } else {
                        $valuesContainer.find("label.upload-photo:first").html("<i class='fa fa-plus'></i> Add image");
                    }
                }

                var imglength = $imgCtrl.parents("div.uploaded-images-continer").find("div.app-uploaded-img").length;

                if (imglength == 1) {
                    $this.addNoImage($imgCtrl.parents("div.uploaded-images-continer"));
                }

                $imgCtrl.parents("div.app-uploaded-img:first").remove();

                if ($uploader.parents("div.app-upload-img").find("div.actual-uploaded:last img, div.actual-uploaded:last video").parents("div.actual-uploaded:first").length > 0) {
                    $this.resetImageCtrlIndexing($uploader);
                } else {

                    $('.app-img-uploader').each(function () {

                        if ($(this).parents("div.app-upload-img").find("div.actual-uploaded:last img, div.actual-uploaded:last video").parents("div.actual-uploaded:first").length > 0) {
                            $this.resetImageCtrlIndexing($(this));
                            //return false;
                        }
                    });

                }

                //if (!$uploader.prop("multiple")) {

                //    if ($uploader[0] !== $('.app-img-uploader:first')[0]) {
                //        $this.resetImageCtrlIndexing($('.app-img-uploader:first'));
                //    } else {
                //        $this.resetImageCtrlIndexing($('.app-img-uploader:eq(1)'));
                //    }
                //} else {
                //    if (imglength == 1) {
                //        $this.resetImageCtrlIndexing($('.app-img-uploader:eq(0)'));
                //    } else {
                //        $this.resetImageCtrlIndexing($uploader);
                //    }
                //}


                //var propertyName = $uploader.data("property-name");
                //var parentControlName = $uploader.data("parent-ctrl-name");
                //var $ctrlContainer = $imgCtrl.parents("div.actual-uploaded:first");
                //var $entityId = $ctrlContainer.find("[ID$=_EntityId]");
                //var $sectionId = $ctrlContainer.find("[ID$=_SectionId]");
                //var $name = $ctrlContainer.find("[ID$=_Name]");
                //var $fileSize = $ctrlContainer.find("[ID$=_FileSize]");
                //var $fileOriginalName = $ctrlContainer.find("[ID$=_FileOriginalName]");
                //var $id = $ctrlContainer.find("[ID$=_Id]");
                //var $fileStreams = $ctrlContainer.find("[ID$=_FileStreams]");

                //if (parentControlName && Global.IsNotNullOrEmptyString(parentControlName)) {
                //    if (parentControlName && parentControlName.length > 0) {
                //        var parentPropertyName = propertyName.substring(0, propertyName.lastIndexOf('['));

                //        var $parentCtrl = $uploader.parents(".flexible-content-box:first").find('[id^="' + parentPropertyName.replace(/\[/g, '_').replace(/\]/g, '_').replace(/\./g, '_') + '_"][id$="_' + parentControlName + '"]:first');
                //        if ($parentCtrl && $parentCtrl.length > 0) {
                //            parentPropertyName = $parentCtrl.attr("name");
                //            parentPropertyName = parentPropertyName.substring(0, parentPropertyName.lastIndexOf('.') + 1);
                //        }
                //    }

                //    if (parentPropertyName && Global.IsNotNullOrEmptyString(parentPropertyName)) {
                //        propertyName = parentPropertyName + (propertyName.substring(propertyName.lastIndexOf(']') + 2, propertyName.length));
                //    }
                //}

                //Global.ResetCtrlIdIndexing(propertyName, $entityId, $sectionId, $name, $fileSize, $fileOriginalName, $id, $fileStreams);
            });
        }

        this.bindInitialEvents = function () {
            $(document).on("change", "input[type=file].app-img-uploader", function (e) {
                //
                //alert('aaa '+$(this).val());
                //
                var $uploader = $(this);
                var $imgCtrl = null, $streamCtrl = null;
                var propertyName = $uploader.data("property-name");
                var titlePropertyName = $uploader.data("title-property-name");
                var parentControlName = $uploader.data("parent-ctrl-name");
                var propertyId = propertyName.replace(/\[/g, '_').replace(/\]/g, '_').replace(/\./g, '_');
                if (parentControlName && Global.IsNotNullOrEmptyString(parentControlName)) {
                    if (parentControlName && parentControlName.length > 0) {
                        var parentPropertyName = propertyName.substring(0, propertyName.lastIndexOf('.')) + '.';

                        var $parentCtrl = $uploader.parents(".flexible-content-box:first").find('[id^="' + parentPropertyName.replace(/\[/g, '_').replace(/\]/g, '_').replace(/\./g, '_') + '_"][id$="_' + parentControlName + '"]:first');
                        if ($parentCtrl && $parentCtrl.length > 0) {
                            parentPropertyName = $parentCtrl.attr("name");
                            parentPropertyName = parentPropertyName.substring(0, parentPropertyName.lastIndexOf('.') + 1);
                        }
                    }

                    if (parentPropertyName && Global.IsNotNullOrEmptyString(parentPropertyName)) {
                        propertyName = parentPropertyName + (propertyName.substring(propertyName.lastIndexOf(']') + 2, propertyName.length));
                        propertyId = propertyName.replace(/\[/g, '_').replace(/\]/g, '_').replace(/\./g, '_');
                    }
                }

                $(e.target.files).each(function (index, file) {
                    var $imgContainer = $uploader.parents("div.app-upload-img:first").find("div.uploaded-images-continer");
                    var imageLength = parseInt($(".sort").length) + 1;
                    $("<div/>", {
                        class: "app-uploaded-img actual-uploaded",
                        html: function () {
                            $("<div/>", {
                                class: "reapeat-upload-img",
                                html: function () {
                                    var prefixId = propertyId + "_0__";
                                    var prefixName = propertyName + "[0].";
                                    var prefixIdImage = propertyId + "_" + imageLength + "__";
                                    var prefixNameImage = propertyName + "[" + imageLength + "].";
                                    $("<input/>", {
                                        type: "hidden",
                                        id: prefixId + "EntityId",
                                        name: prefixName + "EntityId",
                                        value: $uploader.data("entityId")
                                    }).appendTo(this);
                                    $("<input/>", {
                                        type: "hidden",
                                        class: "hiddenImgOrder",
                                        id: prefixId + "ImageOrder",
                                        name: prefixName + "ImageOrder",
                                        value: imageLength
                                    }).appendTo(this);
                                    $("<input/>", {
                                        type: "hidden",
                                        id: prefixId + "SectionId",
                                        name: prefixName + "SectionId",
                                        value: $uploader.data("sectionid")
                                    }).appendTo(this);
                                    $("<input/>", {
                                        type: "hidden",
                                        id: prefixId + "Name",
                                        name: prefixName + "Name",
                                        value: file.name.replace(/[^a-z0-9'.\s]/gi, '').replace(/[_\s]/g, '_')
                                    }).appendTo(this);
                                    $("<input/>", {
                                        type: "hidden",
                                        id: prefixId + "FileSize",
                                        name: prefixName + "FileSize",
                                        value: file.size
                                    }).appendTo(this);
                                    $("<input/>", {
                                        type: "hidden",
                                        id: prefixId + "FileOriginalName",
                                        name: prefixName + "FileOriginalName",
                                        value: file.name.replace(/[^a-z0-9'.\s]/gi, '').replace(/[_\s]/g, '_')
                                    }).appendTo(this);
                                    $("<input/>", {
                                        type: "hidden",
                                        id: prefixId + "FileExtension",
                                        name: prefixName + "FileExtension",
                                        value: "." + file.name.split('.').pop()
                                    }).appendTo(this);

                                    $("<input/>", {
                                        type: "hidden",
                                        id: prefixId + "FId",
                                        name: prefixName + "FId",
                                        value: "0"
                                    }).appendTo(this);
                                    $("<input/>", {
                                        type: "hidden",
                                        id: prefixId + "FileStreams",
                                        name: prefixName + "FileStreams",
                                        value: ""
                                    }).appendTo(this);
                                    $streamCtrl = $(this).find("[ID$=_FileStreams]:first");


                                    if ($uploader.attr("accept").indexOf('image/*') >= 0) {
                                        $("<img/>", {
                                            src: "app-uploaded-img",
                                            class: "sort showfle",
                                            "data-index": imageLength,
                                            alt: file.name
                                        }).appendTo(this);
                                        $imgCtrl = $(this).find("img");
                                    }

                                    //if ($imgCtrl == null && $uploader.attr("accept").indexOf('video/mp4') >= 0) {
                                    //    $("<video/>", {
                                    //        //src: "app-uploaded-img",
                                    //        controls: "",
                                    //        preload: "metadata",
                                    //        html: function () {
                                    //            $("<source/>", {
                                    //                src: "app-uploaded-img",
                                    //                type: 'video/mp4;codecs="avc1.42E01E, mp4a.40.2"',
                                    //                "data-original": ""
                                    //            }).appendTo(this);
                                    //        }
                                    //    }).appendTo(this);
                                    //    $imgCtrl = $(this).find("video");
                                    //}
                                    if ($imgCtrl == null && $uploader.attr("accept").indexOf('video/*') >= 0) {
                                        $("<video/>", {
                                            //src: "app-uploaded-img",
                                            class: "showfle",
                                            controls: "",
                                            preload: "metadata",
                                            html: function () {
                                                $("<source/>", {
                                                    src: "app-uploaded-img",
                                                    type: 'video/mp4;codecs="avc1.42E01E, mp4a.40.2"',
                                                    "data-original": ""
                                                }).appendTo(this);
                                            }
                                        }).appendTo(this);
                                        $imgCtrl = $(this).find("video");
                                    }

                                    if (titlePropertyName && titlePropertyName != "") {
                                        $("<div/>", {
                                            class: "img-title-container",
                                            html: function () {
                                                $("<div/>", {
                                                    class: "dropdown",
                                                    html: function () {
                                                        $("<button/>", {
                                                            id: prefixId + "ImageTitle",
                                                            class: "dropdown-toggle",
                                                            type: "button",
                                                            "data-toggle": "dropdown",
                                                            "aria-haspopup": true,
                                                            "aria-expanded": false,
                                                            html: function () {
                                                                $("<i/>", {
                                                                    class: "fa fa-language"
                                                                }).appendTo(this);
                                                                $("<span/>", {
                                                                    class: "caret"
                                                                }).appendTo(this);
                                                            }
                                                        }).appendTo(this);
                                                        $("<ul/>", {
                                                            class: "dropdown-menu",
                                                            "aria-labelledby": prefixId + "ImageTitle",
                                                            style: "z-index:9999;",
                                                            html: function () {
                                                                var $dropdownContainer = $(this);


                                                                $(languages).each(function (titleIndex, element) {
                                                                    var titlePrefixId = prefixId + titlePropertyName + "_" + titleIndex + "__";
                                                                    var titlePrefixName = prefixName + titlePropertyName + "[" + titleIndex + "].";

                                                                    $("<li/>", {
                                                                        html: function () {
                                                                            $("<div/>", {
                                                                                class: "form-group",
                                                                                html: function () {
                                                                                    $("<input/>", {
                                                                                        id: titlePrefixId + "LanguageId",
                                                                                        name: titlePrefixName + "LanguageId",
                                                                                        type: "hidden",
                                                                                        value: element.LanguageId
                                                                                    }).appendTo(this);

                                                                                    $("<input/>", {
                                                                                        id: titlePrefixId + "FileUploadId",
                                                                                        name: titlePrefixName + "FileUploadId",
                                                                                        type: "hidden"
                                                                                    }).appendTo(this);

                                                                                    $("<label/>", {
                                                                                        for: titlePrefixId + "Title",
                                                                                        text: element.Language
                                                                                    }).appendTo(this);

                                                                                    $("<input/>", {
                                                                                        id: titlePrefixId + "Title",
                                                                                        name: titlePrefixName + "Title",
                                                                                        type: "text",
                                                                                        class: "form-control",
                                                                                        autocomplete: "off",
                                                                                        placeholder: "Title"
                                                                                    }).attr('maxlength', '250').appendTo(this);
                                                                                }
                                                                            }).appendTo(this);

                                                                            //$("<div/>", {
                                                                            //    class: "form-group",
                                                                            //    html: function () {
                                                                            //        $("<input/>", {
                                                                            //            id: titlePrefixId + "Description",
                                                                            //            name: titlePrefixName + "Description",
                                                                            //            type: "text",
                                                                            //            class: "form-control",
                                                                            //            autocomplete: "off",
                                                                            //            placeholder: "Description"
                                                                            //        }).attr('maxlength', '500').appendTo(this);

                                                                            //    }
                                                                            //}).appendTo(this);


                                                                        }
                                                                    }).appendTo($dropdownContainer);
                                                                });

                                                            }
                                                        }).appendTo(this);
                                                    }
                                                }).appendTo(this);
                                            }
                                        }).appendTo(this);
                                    }

                                }
                            }).appendTo(this);
                            $("<div/>", {
                                class: "user-photo-controls",
                                html: function () {
                                    $("<div/>", {
                                        class: "btn delete-photo",
                                        html: function () {
                                            $("<span/>", {
                                                html: "&nbsp;" + file.name,
                                                title: file.name
                                            }).appendTo(this);

                                            $("<i/>", {
                                                title: "Remove",
                                                class: "fa fa-close"
                                            }).on("click", function () {
                                                $this.fnRemoveImage($imgCtrl, $uploader);
                                            }).appendTo(this);
                                        }
                                    }).appendTo(this);
                                }
                            }).appendTo(this);
                        }
                    }).appendTo($imgContainer);
                    var isShown = $this.setFileDetail($uploader, file, $imgCtrl, $streamCtrl);

                    if (!isShown) {
                        $imgCtrl.parents("div.app-uploaded-img:first").remove();
                    }
                    else {

                        var $valuesContainer = $imgContainer.parents("div.app-upload-img:first");
                        if (!$uploader.prop("multiple")) {
                            $uploader.parents("div.app-upload-img:first").find("div.app-uploaded-img:first").remove();
                            if ($valuesContainer.find("video").length == 1) {
                                $valuesContainer.find("label.upload-photo:first").html("Change video");
                            } else {
                                $valuesContainer.find("label.upload-photo:first").html("Change image");
                            }


                        }
                        else {
                            $uploader.parents("div.app-upload-img:first").find("div.app-uploaded-img.app-no-image").remove();
                        }

                        $('.app-img-uploader').each(function () {
                            var $uploader = $(this).parents(".app-upload-img:first").find("input[type=file].app-img-uploader");
                            if ($(this).parents("div.app-upload-img").find("div.actual-uploaded:last img, div.actual-uploaded:last video").parents("div.actual-uploaded:first").length > 0) {
                                $this.resetImageCtrlIndexing($uploader);
                                //return false;
                            }
                        });

                        //$this.resetImageCtrlIndexing($uploader);
                        //var $ctrlContainer = $imgCtrl.parents("div.actual-uploaded:first");
                        //var $entityId = $ctrlContainer.find("[ID$=_EntityId]");
                        //var $sectionId = $ctrlContainer.find("[ID$=_SectionId]");
                        //var $name = $ctrlContainer.find("[ID$=_Name]");
                        //var $fileSize = $ctrlContainer.find("[ID$=_FileSize]");
                        //var $fileOriginalName = $ctrlContainer.find("[ID$=_FileOriginalName]");
                        //var $id = $ctrlContainer.find("[ID$=_Id]");
                        //var $fileStreams = $ctrlContainer.find("[ID$=_FileStreams]");
                        //Global.ResetCtrlIdIndexing(propertyName, $entityId, $sectionId, $name, $fileSize, $fileOriginalName, $id, $fileStreams);
                    }
                });
                message1 = "";
                messageCount = 0;

                messageCount2 = 0;
                messageCount3 = 0;
                messageCount4 = 0;
                messageCount5 = 0;
                reorderImages();
            });

            $(document).on("click", "div.app-uploaded-img div.delete-photo i.fa-close", function (e) {
                var $imgCtrl = $(this).parents("div.app-uploaded-img:first").find("img");
                if ($imgCtrl.length == 0) {
                    $imgCtrl = $(this).parents("div.app-uploaded-img:first").find("video");
                }
                var $uploader = $(this).parents(".app-upload-img:first").find("input[type=file].app-img-uploader");
                $this.fnRemoveImage($imgCtrl, $uploader);

            });
        };



        this.init = function () {
            $this.bindInitialEvents();
        };
    }

    $(function () {
        var self = new FileUploadCustom();
        self.init();
        $.resetImageCtrlIndexing = self.resetImageCtrlIndexing;
        $('.app-img-uploader').each(function () {
            var $uploader = $(this).parents(".app-upload-img:first").find("input[type=file].app-img-uploader");
            if ($(this).parents("div.app-upload-img").find("div.actual-uploaded:last img, div.actual-uploaded:last video").parents("div.actual-uploaded:first").length > 0) {
                self.resetImageCtrlIndexing($uploader);
                //return false;
            }
        });
    });

    $(document).ready(function () {

    });


}(jQuery));


function reorderImages() {
    //return;
    $(".Activity_Container").each(function (index) {
        //alert('a ' + index);
        var $controls = $(this).find(':input');

        var replaceWhat = "_" + "0" + "__";
        var ReplaceFrom = "_" + index + "__";
        // alert(ReplaceFrom);
        // Reorder the index starting from 0
        $controls.each(function (ind) {
            for (var k = 0; k < 30; k++) {
                // 
                if ($(this).attr('id') != undefined) {

                    ReplaceFrom = "_" + index + "__";
                    replaceWhat = "_" + k + "__";
                    if ($(this).attr('id').indexOf(replaceWhat) != -1) { $(this).attr('id', $(this).attr('id').replace(replaceWhat, ReplaceFrom)) };

                }
                if ($(this).attr('name') != undefined) {
                    ReplaceFrom = "[" + index + "]"
                    replaceWhat = "[" + k + "]";
                    if ($(this).attr('name').indexOf(replaceWhat) != -1) { $(this).attr('name', $(this).attr('name').replace(replaceWhat, ReplaceFrom)) };

                }


                //
                if ($(this).attr('id') != undefined) {
                    ReplaceFrom = "PackageDetailActivityImages_0__"
                    replaceWhat = "PackageDetailActivityImages_" + k + "__";
                    if ($(this).attr('id').indexOf(replaceWhat) != -1) { $(this).attr('id', $(this).attr('id').replace(replaceWhat, ReplaceFrom)) };


                }
                if ($(this).attr('name') != undefined) {
                    replaceWhat = "PackageDetailActivityImages[" + k + "]";
                    ReplaceFrom = "PackageDetailActivityImages[0]";
                    if ($(this).attr('name').indexOf(replaceWhat) != -1) { $(this).attr('name', $(this).attr('name').replace(replaceWhat, ReplaceFrom)) };

                }

                //
                if ($(this).attr('id') != undefined) {
                    ReplaceFrom = "PackageDetailActivityVideos_0__"
                    replaceWhat = "PackageDetailActivityVideos_" + k + "__";
                    if ($(this).attr('id').indexOf(replaceWhat) != -1) { $(this).attr('id', $(this).attr('id').replace(replaceWhat, ReplaceFrom)) };


                }
                if ($(this).attr('name') != undefined) {
                    replaceWhat = "PackageDetailActivityVideos[" + k + "]";
                    ReplaceFrom = "PackageDetailActivityVideos[0]";
                    if ($(this).attr('name').indexOf(replaceWhat) != -1) { $(this).attr('name', $(this).attr('name').replace(replaceWhat, ReplaceFrom)) };

                }
            }


            //// 
            // if ($(this).attr('id') != undefined) {

            //     ReplaceFrom = "_" + index + "__";
            //     replaceWhat = "_" + "0" + "__";
            //     if ($(this).attr('id').indexOf(replaceWhat) != -1) { $(this).attr('id', $(this).attr('id').replace(replaceWhat, ReplaceFrom)) };
            //     replaceWhat = "_" + "1" + "__";
            //     if ($(this).attr('id').indexOf(replaceWhat) != -1) { $(this).attr('id', $(this).attr('id').replace(replaceWhat, ReplaceFrom)) };
            //     replaceWhat = "_" + "2" + "__";
            //     if ($(this).attr('id').indexOf(replaceWhat) != -1) { $(this).attr('id', $(this).attr('id').replace(replaceWhat, ReplaceFrom)) };
            //     replaceWhat = "_" + "3" + "__";
            //     if ($(this).attr('id').indexOf(replaceWhat) != -1) { $(this).attr('id', $(this).attr('id').replace(replaceWhat, ReplaceFrom)) };
            //     replaceWhat = "_" + "4" + "__";
            //     if ($(this).attr('id').indexOf(replaceWhat) != -1) { $(this).attr('id', $(this).attr('id').replace(replaceWhat, ReplaceFrom)) };
            // }
            // if ($(this).attr('name') != undefined) {
            //     ReplaceFrom = "[" + index + "]"
            //     replaceWhat = "[" + "0" + "]";                   
            //     if ($(this).attr('name').indexOf(replaceWhat) != -1) { $(this).attr('name', $(this).attr('name').replace(replaceWhat, ReplaceFrom)) };
            //     replaceWhat = "[" + "1" + "]";
            //     if ($(this).attr('name').indexOf(replaceWhat) != -1) { $(this).attr('name', $(this).attr('name').replace(replaceWhat, ReplaceFrom)) };
            //     replaceWhat = "[" + "2" + "]";
            //     if ($(this).attr('name').indexOf(replaceWhat) != -1) { $(this).attr('name', $(this).attr('name').replace(replaceWhat, ReplaceFrom)) };
            //     replaceWhat = "[" + "3" + "]";
            //     if ($(this).attr('name').indexOf(replaceWhat) != -1) { $(this).attr('name', $(this).attr('name').replace(replaceWhat, ReplaceFrom)) };
            //     replaceWhat = "[" + "4" + "]";
            //     if ($(this).attr('name').indexOf(replaceWhat) != -1) { $(this).attr('name', $(this).attr('name').replace(replaceWhat, ReplaceFrom)) };
            // }


            // //
            // if ($(this).attr('id') != undefined) {
            //     ReplaceFrom = "PackageDetailActivityImages_0__"
            //     replaceWhat = "PackageDetailActivityImages_0__";                  
            //     if ($(this).attr('id').indexOf(replaceWhat) != -1) { $(this).attr('id', $(this).attr('id').replace(replaceWhat, ReplaceFrom)) };
            //     replaceWhat = "PackageDetailActivityImages_1__";
            //     if ($(this).attr('id').indexOf(replaceWhat) != -1) { $(this).attr('id', $(this).attr('id').replace(replaceWhat, ReplaceFrom)) };
            //     replaceWhat = "PackageDetailActivityImages_2__";
            //     if ($(this).attr('id').indexOf(replaceWhat) != -1) { $(this).attr('id', $(this).attr('id').replace(replaceWhat, ReplaceFrom)) };
            //     replaceWhat = "PackageDetailActivityImages_3__";
            //     if ($(this).attr('id').indexOf(replaceWhat) != -1) { $(this).attr('id', $(this).attr('id').replace(replaceWhat, ReplaceFrom)) };
            //     replaceWhat = "PackageDetailActivityImages_4__";
            //     if ($(this).attr('id').indexOf(replaceWhat) != -1) { $(this).attr('id', $(this).attr('id').replace(replaceWhat, ReplaceFrom)) };

            // }
            // if ($(this).attr('name') != undefined) {
            //     replaceWhat = "PackageDetailActivityImages[0]";
            //     ReplaceFrom = "PackageDetailActivityImages[0]";
            //     if ($(this).attr('name').indexOf(replaceWhat) != -1) { $(this).attr('name', $(this).attr('name').replace(replaceWhat, ReplaceFrom)) };
            //     replaceWhat = "PackageDetailActivityImages[1]";
            //     if ($(this).attr('name').indexOf(replaceWhat) != -1) { $(this).attr('name', $(this).attr('name').replace(replaceWhat, ReplaceFrom)) };
            //     replaceWhat = "PackageDetailActivityImages[2]";
            //     if ($(this).attr('name').indexOf(replaceWhat) != -1) { $(this).attr('name', $(this).attr('name').replace(replaceWhat, ReplaceFrom)) };
            //     replaceWhat = "PackageDetailActivityImages[3]";
            //     if ($(this).attr('name').indexOf(replaceWhat) != -1) { $(this).attr('name', $(this).attr('name').replace(replaceWhat, ReplaceFrom)) };
            //     replaceWhat = "PackageDetailActivityImages[4]";
            //     if (replaceWhat.indexOf(replaceWhat) != -1) { $(this).attr('name', $(this).attr('name').replace(replaceWhat, ReplaceFrom)) };
            // }
        });

    });

    $(".custom-control").each(function (index) {
        //$(this).find("input[type='checkbox']").bootstrapSwitch('destroy');
        //$(this).find("input[type='checkbox']").bootstrapSwitch();
    });


    //// Function to initialize Bootstrap Switch
    //function initBootstrapSwitch() {
    //    $('.custom-control-input').bootstrapSwitch();
    //}

    //// Function to rebind Bootstrap Switch controls
    //function rebindBootstrapSwitch() {
    //    // Destroy the existing switches
    //    $('.custom-control-input').bootstrapSwitch('destroy');

    //    // Initialize them again
    //    initBootstrapSwitch();
    //}

    //// Call the initialization function
    //initBootstrapSwitch();

    //// Example: Change index and rebind the switches
    //$('#changeIndexButton').click(function () {
    //    // Simulate a change in the controls' index
    //    // For example, you can add or remove controls dynamically here.

    //    // Rebind the switches after the index change
    //    rebindBootstrapSwitch();
    //});


}

function onSelectVideoCallPackage(count, cls, obj, type) {
    IsLimitUploadVideoCount = -1;
    TypeIV = type;

    count_ = count;
    cls_ = cls;
    obj_ = obj;
    TypeIV_ = type;
    var Lgth = $(obj).closest("." + cls).find(".showfle").length;
    if (Lgth >= count) {
        IsRepeatCount = 0;
        IsLimitUploadVideoCount = count;
    }
}
function onSelectImageCallPackage(count, cls, obj, type) {
    IsLimitUploadImageCount = -1;
    TypeIV = type;
    count_ = count;
    cls_ = cls;
    obj_ = obj;
    TypeIV_ = type;
    var Lgth = $(obj).closest("." + cls).find(".showfle").length;
    if (Lgth >= count) {
        IsRepeatCount = 0;
        IsLimitUploadImageCount = count;
    }
}

function ResetObject(obj) {
    $(obj).removeAttr("type");
    $(obj).attr("type", "file");
    IsRepeatCount = 0;
    //document.getElementById(obj.id).reset();
}
