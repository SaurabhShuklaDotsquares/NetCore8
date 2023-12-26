/*global window, $*/
var Global = {
    SelectedFiles: [],
    MessageType: {
        Success: 0,
        Error: 1,
        Warning: 2,
        Info: 3
    },
    AdminPageLimit:10
};

Global.FormHelper = function (formElement, options, onSucccess, onError, loadingElementId, onComplete) {
    "use strict";
    var settings = {};
    settings = $.extend({}, settings, options);
    $.validator.unobtrusive.parse(formElement);
    if (settings.validateSettings !== null && settings.validateSettings !== undefined) {
        formElement.validate(settings.validateSettings);
    }


    formElement.off("submit").submit(function (e) {

        e.preventDefault();
        e.stopImmediatePropagation();

        var submitBtn = formElement.find(':submit');
        if (formElement.validate().valid() && formElement.valid()) {
            var submitHtml = submitBtn.filter(':focus').addClass('submitting').html();

            if (options && options.beforeSubmit) {
                if (!options.beforeSubmit()) {
                    return false;
                }
            }

            $.ajax(formElement.attr("action"), {
                type: "POST",
                data: formElement.serializeArray(),

                beforeSend: function () {
                    loader(true);
                    submitBtn.filter('.submitting').html('<i class="fa fa-refresh fa-spin"></i> Submitting...');
                    $(':input[type="submit"]').prop('disabled', true);
                },
                success: function (result) {
                    loader(false);
                    if (onSucccess === null || onSucccess === undefined) {
                        if (result.isSuccess) {
                            window.location.href = result.redirectUrl;
                        } else {
                            if (settings.updateTargetId) {
                                var datatresult = (result.message == null || result.message == undefined) ? ((result.data == null || result.data == undefined) ? result : result.data) : result.message;
                                $("#" + settings.updateTargetId).html(datatresult);
                            }
                        }
                    } else {
                        onSucccess(result);
                    }
                },
                error: function (jqXHR, status, error) {

                    loader(false);
                    if (onError !== null && onError !== undefined) {
                        onError(jqXHR, status, error);
                        $("#loadingElement").hide();
                    }
                },
                complete: function (result) {
                    loader(false);
                    submitBtn.filter('.submitting').html(submitHtml).removeClass('submitting');
                    $(':input[type="submit"]').prop('disabled', false);
                }
            });
        }

        e.preventDefault();
    });
    return formElement;
};
Global.FormHelperWithFiles = function (formElement, options, onSucccess, onError, loadingElementId, onComplete) {
    //"use strict";
    //var settings = {};
    //settings = $.extend({}, settings, options);
    //formElement.validate(settings.validateSettings);
    var settings = {};
    settings = $.extend({}, settings, options);
    $.validator.unobtrusive.parse(formElement);
    if (settings.validateSettings !== null && settings.validateSettings !== undefined) {
        formElement.validate(settings.validateSettings);
    }
    formElement.off("submit").submit(function (e) {

        if (options && options.beforeSubmit) {
            if (!options.beforeSubmit()) {
                return false;
            }
        }

        var formdata = new FormData();
        formElement.find('input[type="file"]:not(:disabled)').each(function (i, elem) {
            if (elem.files && elem.files.length) {
                for (var i = 0; i < elem.files.length; i++) {
                    var file = elem.files[i];
                    formdata.append(elem.getAttribute('name'), file);
                }
            }
        });

        $.each(formElement.serializeArray(), function (i, item) {
            formdata.append(item.name, item.value);
        });


        var submitBtn = formElement.find(':submit');

        if (formElement.valid()) {
            submitBtn.find('i').removeClass("fa fa-arrow-circle-right");
            submitBtn.find('i').addClass("fa fa-refresh");
            submitBtn.prop('disabled', true);
            submitBtn.find('span').html('Submiting..');
            $.ajax(formElement.attr("action"), {
                type: "POST",
                data: formdata,
                contentType: false,
                processData: false,
                beforeSend: function () {
                    loader(true);
                    if (settings.loadingElementId != null || settings.loadingElementId != undefined) {
                        $("#" + settings.loadingElementId).show();
                        submitBtn.hide();
                    }
                },
                success: function (result) {
                    loader(false);
                    if (onSucccess === null || onSucccess === undefined) {
                        if (result.isSuccess) {
                            window.location.href = result.redirectUrl;
                        } else {
                            if (settings.updateTargetId) {
                                var datatresult = (result.message == null || result.message == undefined) ? ((result.data == null || result.data == undefined) ? result : result.data) : result.message;
                                $("#" + settings.updateTargetId).html(datatresult);
                            }
                        }
                    } else {
                        onSucccess(result);
                    }
                },
                error: function (jqXHR, status, error) {

                    loader(false);
                    if (onError !== null && onError !== undefined) {
                        onError(jqXHR, status, error);
                        $("#loadingElement").hide();
                    }
                },
                complete: function (result) {
                    loader(false);
                    if (onComplete === null || onComplete === undefined) {
                        if (settings.loadingElementId != null || settings.loadingElementId != undefined) {
                            $("#" + settings.loadingElementId).hide();
                        }
                        submitBtn.find('i').removeClass("fa fa-refresh");
                        submitBtn.find('i').addClass("fa fa-arrow-circle-right");
                        submitBtn.find('span').html('Submit');
                        submitBtn.prop('disabled', false);
                    } else {
                        onComplete(result);
                    }
                }
            });
        }

        e.preventDefault();
    });

    return formElement;
};

Global.AjaxPost = function (options, onSucccess, onError, onComplete, asyncRequest) {
    "use strict";
    asyncRequest = asyncRequest === null || asyncRequest === undefined || asyncRequest === true;
    if (options && options.beforeSubmit) {
        if (!options.beforeSubmit()) {
            return false;
        }
    }
    var formdata = new FormData();
    if (options && options.updateFormData) {
        var updateformdata = options.updateFormData(formdata);
        if (updateformdata !== null && updateformdata !== undefined) {
            formdata = updateformdata;
        }
    }
    $.ajax(options.url, {
        type: "POST",
        data: formdata,
        contentType: false,
        processData: false,
        async: asyncRequest,
        success: function (result) {
            if (onSucccess === null || onSucccess === undefined) {
                if (result.isSuccess) {
                    window.location.href = result.redirectUrl;
                } else {
                    if (settings.updateTargetId) {
                        var datatresult = (result.message == null || result.message == undefined) ? ((result.data == null || result.data == undefined) ? result : result.data) : result.message;
                        $("#" + settings.updateTargetId).html(datatresult);
                    }
                }
            } else {
                onSucccess(result);
            }
        },
        error: function (jqXHR, status, error) {
            if (onError !== null && onError !== undefined) {
                onError(jqXHR, status, error);
                $("#loadingElement").hide();
            }
        }
    });
};



Global.IsNull = function (o) { return typeof o === "undefined" || typeof o === "unknown" || o == null };
Global.IsNotNull = function (o) { return !Global.IsNull(o); };
Global.IsNullOrEmptyString = function (str) {
    return Global.IsNull(str) || typeof str === "string" && $.trim(str).length == 0
};
Global.IsNotNullOrEmptyString = function (str) { return !Global.IsNullOrEmptyString(str); };

Global.GridHelper = function (gridElement, options) {
    if ($(gridElement).length > 0) {
        var settings = {};        
        options.iDisplayLength = Global.AdminPageLimit;              
        settings = $.extend({}, settings, options);
        return $(gridElement).dataTable(settings);
    }
};

Global.FormValidationReset = function (formElement, validateOption) {
    if ($(formElement).data('validator')) {
        $(formElement).data('validator', null);
    }

    $(formElement).validate(validateOption);
    return $(formElement);
};

Global.DateProcess = function process(date) {
    var parts = date.split("/");
    return new Date(parts[2], parts[1] - 1, parts[0]);
}
Global.Confirm = function (title, message, okCallback, cancelCallback) {
    return alertify.confirm(title, message, function () {
        if (okCallback)
            okCallback();
    }, function () {
        if (cancelCallback)
            cancelCallback();
    }).set({ transition: 'fade', 'closable': false });
};
Date.prototype.isSameDateAs = function (pDate) {
    return (
        this.getFullYear() === pDate.getFullYear() &&
        this.getMonth() === pDate.getMonth() &&
        this.getDate() === pDate.getDate()
    );
}
Global.ShowMessage = function (message, type) {
    //alertify.set({ delay: 8000 });
    alertify.set('notifier', 'position', 'top-left');
    if (type === Global.MessageType.Success) {
        alertify.success(message);
    }
    else if (type === Global.MessageType.Error) {
        alertify.error(message);
    }
    else if (type === Global.MessageType.Warning) {
        alertify.warning(message);
    }
    else if (type === Global.MessageType.Info) {
        alertify.message(message);
    }
};
Global.Alert = function (title, message, callback) {
    alertify.alert(title, message, function () {
        if (callback)
            callback();
    }).set({ transition: 'fade' });
    if (title === "Alert!") {
        $('.ajs-content').addClass('error-text');
    }
    else {
        $('.ajs-content').removeClass('error-text');
    }
};

$(document).on('keypress', '.number', function (event) {
    //Added by arnav
    if ((event.which < 48 || event.which > 57) && event.which != 8 && event.which != 0) {
        event.preventDefault();
    }
});

/*Arnav*/
$(document).on('keypress', '.decimal', function (event) {
    if ((event.which != 46 || $(this).val().indexOf('.') != -1) && (event.which < 48 || event.which > 57) && event.which != 8 && event.which != 0) {
        event.preventDefault();
    }
});

$(document).on('keypress', '.decimalNegative', function (event) {
    if ((event.which != 46 || $(this).val().indexOf('.') != -1) && (event.which != 45 || $(this).val().indexOf('-') != -1) && (event.which < 48 || event.which > 57) && event.which != 8 && event.which != 0) {
        event.preventDefault();
    }
});

$(document).on('change', '.number, .decimal, .decimalNegative', function () { if ($(this).val() != "0" && $(this).val() != "" && !$.isNumeric($(this).val())) { alert("Please enter a valid value"); return false; } });

$(document).on('focus', '.number, .decimal, .decimalNegative', function (event) {
    var default_value = 0;
    if ($(this).val() == default_value) $(this).val("");
});

$(document).on('blur', '.number, .decimal, .decimalNegative', function (event) {
    var default_value = 0;
    if ($(this).val().length == 0) $(this).val(default_value);
});

$(document).on('bind', '.disablecopy', function (e) {
    e.preventDefault();
});

$(document).on('cut copy paste', '.disablecopy', function (e) {
    e.preventDefault();
});

$(".disableRightClick").on("contextmenu", function (e) {
    return false;
});

Global.TotalCartItem = function getCartItemCount() {
    $.post("JsonData/GetCartItemCount", function (result) {
        var spnTotalCartItem = $('#spnTotalCartItem');
        spnTotalCartItem.html('');
        spnTotalCartItem.removeAttr("class");

        if (result != null) {
            if (result != 0) {
                spnTotalCartItem.attr('class', 'caret solid-blue');
                spnTotalCartItem.html(result);
            }
        }
    })
}


Global.CartMessage = function showCartMessage(message) {
    var dvNotifyBar = $('#dvNotifyBar');
    var spnNotifyBar = $('#spnNotifyBar');
    spnNotifyBar.html(message);

    dvNotifyBar.removeClass('hide');
    dvNotifyBar.addClass('show');

    dvNotifyBar.fadeTo(3000, 500).slideUp(500, function () {
        dvNotifyBar.slideUp(500);
        dvNotifyBar.removeClass('show');
        dvNotifyBar.addClass('hide');

    });
}

$("div[role=dialog].modal").on('show.bs.modal', function (event) {
    var button = $(event.relatedTarget); // Button that triggered the modal
    var url = button.attr("href");
    if (url !== "") {
        var modal = $(this);
        // note that this will replace the content of modal-content everytime the modal is opened
        modal.find('.modal-content').load(url);
    }
}).on('hidden.bs.modal', function (e) {
    $(this).removeData('bs.modal');
    $(this).find(".modal-content").empty();
});

$(document).on('click', '.closenotification', function () {
    var dvNotifyBar = $('#dvNotifyBar');
    dvNotifyBar.removeClass('show');
    dvNotifyBar.addClass('hide');
});

Global.CartMessageForPayment = function showCartMessageForPayment(message, redirectUrl) {
    var dvNotifyBar = $('#dvNotifyBar');
    var spnNotifyBar = $('#spnNotifyBar');
    spnNotifyBar.html(message);

    dvNotifyBar.removeClass('hide');
    dvNotifyBar.addClass('show');

    dvNotifyBar.fadeTo(3000, 500).slideUp(500, function () {
        dvNotifyBar.slideUp(500);
        dvNotifyBar.removeClass('show');
        dvNotifyBar.addClass('hide');
        window.location = redirectUrl;
    });
}

Global.showsPartial = function ($url, $divId) {
    $.ajax({
        url: $url,
        type: 'GET',
        async: false,
        crossDomain: true,
        cache: false,
        success: function (htmlElement) {
            $('#' + $divId).empty().html(htmlElement);
        }
    });
};

Global.SingleDatePicker = function (parentElement, options) {
    var defaultOptions = {
        drops: 'down',
        autoUpdateInput: false,
        singleDatePicker: true,
        autoApply: false,
        locale: {
            format: dateFormatClient,
        }
    };

    if (options) {
        defaultOptions = $.extend({}, defaultOptions, options);
    }

    parentElement.find('.datepicker').daterangepicker(defaultOptions).on('apply.daterangepicker', function (ev, picker) {
        $(this).val(picker.endDate.format(defaultOptions.locale.format)).change();
    })
        .on('keypress paste', function (e) {
            e.preventDefault();
            return false;
        }).attr("autocomplete", "off");
};
$(document).on('keypress keyup', '.form-control', function (e) {
    if (e.which === 32 && !this.value.length)
        e.preventDefault();
});

String.prototype.lines = function () { return this.split(/\r*\n/); }
String.prototype.lineCount = function () { return this.lines().length; }

Global.bindMaxLength = function () {
    //$('input[maxlength], textarea[maxlength]').off("maxlength").maxlength({
    //    alwaysShow: true, //if true the threshold will be ignored and the remaining length indication will be always showing up while typing or on focus on the input. Default: false.
    //    // threshold: 10, //Ignored if alwaysShow is true. This is a number indicating how many chars are left to start displaying the indications. Default: 10
    //    warningClass: "form-text text-muted mt-1", //it's the class of the element with the indicator. By default is the bootstrap "badge badge-success" but can be changed to anything you'd like.
    //    limitReachedClass: "form-text text-muted mt-1", //it's the class the element gets when the limit is reached. Default is "badge badge-danger". Replace with text-danger if you want it to be red.
    //    //separator: ' of ', //represents the separator between the number of typed chars and total number of available chars. Default is "/".
    //    //preText: 'You have ', //is a string of text that can be outputted in front of the indicator. preText is empty by default.
    //    //postText: ' chars remaining.', //is a string outputted after the indicator. postText is empty by default.
    //    showMaxLength: true, //showMaxLength: if false, will display just the number of typed characters, e.g. will not display the max length. Default: true.
    //    showCharsTyped: true, //if false, will display just the remaining length, e.g. will display remaining lenght instead of number of typed characters. Default: true.
    //    placement: 'centered-right', //is a string, object, or function, to define where to output the counter. 
    //    //Possible string values are: bottom( default option ), left, top, right, bottom - right, top - right, top - left, bottom - left and centered - right.
    //    //Are also available: ** bottom - right - inside ** (like in Google's material design, **top-right-inside**, **top-left-inside** and **bottom-left-inside**. 
    //    //stom placements can be passed as an object, with keys top, right, bottom, left, and position.These are passed to $.fn.css.
    //    //A custom function may also be passed.This method is invoked with the { $element } Current Input, the { $element } MaxLength Indicator, and the Current Input's Position { bottom height left right top width }.

    //    appendToParent: true, // appends the maxlength indicator badge to the parent of the input rather than to the body.
    //    //message: an alternative way to provide the message text, i.e. 'You have typed %charsTyped% chars, %charsRemaining% of %charsTotal% remaining'. %charsTyped%, %charsRemaining% and %charsTotal% will be replaced by the actual values. This overrides the options separator, preText, postText and showMaxLength. Alternatively you may supply a function that the current text and max length and returns the string to be displayed. For example, function(currentText, maxLength) { return '' + Math.ceil(currentText.length / 160) + ' SMS Message(s)';}
    //    utf8: true, //if true the input will count using utf8 bytesize/encoding. For example: the '£' character is counted as two characters.
    //    showOnReady:true, // shows the badge as soon as it is added to the page, similar to alwaysShow
    //    twoCharLinebreak:true, //count linebreak as 2 characters to match IE/Chrome textarea validation
    //    //customMaxAttribute: String -- allows a custom attribute to display indicator without triggering native maxlength behaviour. Ignored if value greater than a native maxlength attribute. 'overmax' class gets added when exceeded to allow user to implement form validation.
    //    allowOverMax:false //Will allow the input to be over the customMaxLength. Useful in soft max situations.
    //});

    !function ($) {
        $.fn.maxlength = function () {
            $(this).each(function () {
                var max = $(this).attr('maxlength');

                if (max <= 0 || max === undefined) {
                    throw new Error('maxlength attribute must be defined and greater than 0');
                }
                if ($(this).attr("data-max-length") == undefined) {
                    $(this).attr("data-max-length", $(this).attr('maxlength'));
                }
                if (!$(this).parent().hasClass('input-group')) {
                    $(this).wrap("<div class=\"input-group\"></div>");
                }
                if ($(this).parent().find("div.input-group-append").length == 0) {
                    $(this).after("<div class=\"input-group-append maxlength-container\"><span class=\"input-group-text maxlength\"></span></div>");
                }

                $(this).bind('input', function (e) {
                    var max = $(this).attr('data-max-length');
                    var val = $(this).val();
                    var cur = 0;

                    if (val) {
                        cur = (val.length + (val.lineCount() > 1 ? (val.lineCount() - 1) : 0));
                    }

                    if (cur > max) {
                        $(this).val(val.substring(0, (val.length - (cur - max))));
                        val = $(this).val();
                        cur = (val.length + (val.lineCount() > 1 ? (val.lineCount() - 1) : 0));
                    }

                    var left = max - cur;
                    var $errorMessageSpan = $("span[data-valmsg-for='" + $(this).attr("name") + "'");
                    if (left < 0) {
                        $(this).parent().find("div.input-group-append span").addClass("text-danger");
                        $errorMessageSpan.text("Please do not enter more than " + max + " characters.");
                        $errorMessageSpan.attr("data-valmsg-replace", "false");
                    } else {
                        $(this).parent().find("div.input-group-append span").removeClass("text-danger");
                        $errorMessageSpan.text("");
                        $errorMessageSpan.attr("data-valmsg-replace", "true");
                    }

                    $(this).next(".maxlength-container").find(".maxlength").text(left.toString());

                    return this;
                }).trigger('input');

            });
            return this;
        };
    }(window.jQuery);
    $('input[maxlength]:not(.no-maxlength), textarea[maxlength]:not(.ckeditor,.no-maxlength)').off("maxlength").maxlength();
};
Global.bindMaxLength();

Global.ResetCtrlIdIndexing = function (newPropertyName, $container) {
    var args = [];
    for (var i = 2; i < arguments.length; i++) {
        args.push(arguments[i]);
    }
    "use strict";
    $(args).each(function () {
        var ctrlName = $(this).attr("name");
        
        if (ctrlName) {
            
            var propertyName = ctrlName.substring(0, ctrlName.lastIndexOf("["));
            ctrlName = ctrlName.substring(ctrlName.lastIndexOf(".") + 1, ctrlName.length);
            var propertyId = propertyName.replace(/\[/g, '_').replace(/\]/g, '_').replace(/\./g, '_');
            var newPropertyId = newPropertyName != null ? newPropertyName.replace(/\[/g, '_').replace(/\]/g, '_').replace(/\./g, '_') : propertyId;
            var titleName = "";
            var actualIndex = 0;
            var titlePropertyName = "";

            if (newPropertyName.indexOf(".") > 0) {
                titleName = newPropertyName.substring(newPropertyName.indexOf("."), newPropertyName.length);
                actualIndex = newPropertyName.substring(newPropertyName.indexOf("["), newPropertyName.indexOf("]") + 1);
                titlePropertyName = newPropertyName.substring(0, newPropertyName.indexOf("["));
            }
            // 22-02-2022

            if (newPropertyName && Global.IsNotNullOrEmptyString(newPropertyName)) {
                var index = 0;
                $('[id^="' + newPropertyId + '_"][id$="_' + ctrlName + '"]').each(function (i, element) {
                    var curentCtrlName = $(this).attr("name");
                    var isChildCtrl = curentCtrlName.replace(newPropertyName, '').split(']').length > 2;
                    //if (!isChildCtrl) {
                    //    $(this).attr("id", newPropertyId + "_" + index + "__" + ctrlName).attr("name", newPropertyName + "[" + index + "]." + ctrlName);
                    //    index++;
                    //}
                    if (!isChildCtrl) {
                        if (titleName == "Titles") {
                            $(this).attr("id", titlePropertyName + "_" + actualIndex + "__" + titleName + "_0__" + ctrlName).attr("name", titlePropertyName + "[" + actualIndex + "]." + titleName + "[0]." + ctrlName);
                        }
                        else {
                            $(this).attr("id", newPropertyId + "_" + index + "__" + ctrlName).attr("name", newPropertyName + "[" + index + "]." + ctrlName);
                        }
                        index++;
                    }
                });
            }
            if (newPropertyName != propertyName) {
                index = 0;
                $container.find('[id^="' + propertyId + '_"][id$="_' + ctrlName + '"]').each(function (i, element) {
                    var curentCtrlName = $(this).attr("name");
                    var isChildCtrl = curentCtrlName.replace(propertyName, '').split(']').length > 2;
                    //if (!isChildCtrl) {
                    //    $(this).attr("id", newPropertyId + "_" + index + "__" + ctrlName).attr("name", newPropertyName + "[" + index + "]." + ctrlName);
                    //    index++;
                    //}
                    if (!isChildCtrl) {
                        if (titleName == "Titles") {
                            $(this).attr("id", titlePropertyName + "_" + actualIndex + "__" + titleName + "_0__" + ctrlName).attr("name", titlePropertyName + "[" + actualIndex + "]." + titleName + "[0]." + ctrlName);
                        }
                        else {
                            $(this).attr("id", newPropertyId + "_" + index + "__" + ctrlName).attr("name", newPropertyName + "[" + index + "]." + ctrlName);
                        }
                        index++;
                    }
                });
            }


            index = 0;
            if (newPropertyName && Global.IsNotNullOrEmptyString(newPropertyName)) {
                $('label[for^="' + newPropertyId + '_"][for$="_' + ctrlName + '"]').each(function (i, element) {
                    var isChildCtrl = $(this).attr("for").replace(newPropertyId, '').split('__').length > 2;
                    //if (!isChildCtrl) {
                    //    $(this).attr("for", newPropertyId + "_" + index + "__" + ctrlName);
                    //    index++;
                    //}
                    if (!isChildCtrl) {
                        if (titleName == "Titles") {
                            $(this).attr("for", titlePropertyName + "_" + actualIndex + "__" + titleName + "_0__" + ctrlName);
                        }
                        else {
                            $(this).attr("for", newPropertyId + "_" + index + "__" + ctrlName);
                        }
                        index++;
                    }
                });
            }
            if (newPropertyName != propertyName) {
                index = 0;
                $container.find('label[for^="' + propertyId + '_"][for$="_' + ctrlName + '"]').each(function (i, element) {
                    var isChildCtrl = $(this).attr("for").replace(propertyId, '').split('__').length > 2;
                    //if (!isChildCtrl) {
                    //    $(this).attr("for", newPropertyId + "_" + index + "__" + ctrlName);
                    //    index++;
                    //}
                    if (!isChildCtrl) {
                        if (titleName == "Titles") {
                            $(this).attr("for", titlePropertyName + "_" + actualIndex + "__" + titleName + "_0__" + ctrlName);
                        }
                        else {
                            $(this).attr("for", newPropertyId + "_" + index + "__" + ctrlName);
                        }
                        index++;
                    }
                });
            }
        }
    });
};

Global.AttachEventCKEditor = function (instance) {
    CKEDITOR.instances[instance].on("instanceReady", function (e) {
        this.on("change", function () {
            CKEDITOR.instances[instance].updateElement();
        });
    });
}

Global.resetFlexibleContentBoxCtrlIndexing = function ($addNewButton) {
    var $appendToControl = $addNewButton.parents(".main-dynamically-added-container:first").find($addNewButton.data("append-to-control"));
    var propertyName = $addNewButton.data("property-name");
    var controlNames = $addNewButton.data("control-names");
    var parentControlName = $addNewButton.data("parent-control-name");
    controlNames = controlNames.split(',');
    var propertyId = propertyName.replace(/\[/g, '_').replace(/\]/g, '_').replace(/\./g, '_');
    var newPropertyName = propertyName;


    $(controlNames).each(function (index, ctrlName) {
        if (parentControlName && parentControlName.length > 0) {
            var parentPropertyName = propertyName.substring(0, propertyName.lastIndexOf('['));

            var $parentCtrl = $appendToControl.parents(".flexible-content-box:first").find('[id^="' + parentPropertyName.replace(/\[/g, '_').replace(/\]/g, '_').replace(/\./g, '_') + '_"][id$="_' + parentControlName + '"]:first');
            if ($parentCtrl && $parentCtrl.length > 0) {
                parentPropertyName = $parentCtrl.attr("name");
                parentPropertyName = parentPropertyName.substring(0, parentPropertyName.lastIndexOf('.') + 1);
            }
        }

        if (parentPropertyName && Global.IsNotNullOrEmptyString(parentPropertyName)) {
            newPropertyName = parentPropertyName + (propertyName.substring(propertyName.lastIndexOf(']') + 2, propertyName.length));
            propertyId = newPropertyName.replace(/\[/g, '_').replace(/\]/g, '_').replace(/\./g, '_');
        }

        var $ctrl = $appendToControl.find('[id$="_' + ctrlName + '"]:first')
        if ($ctrl) {
            Global.ResetCtrlIdIndexing(newPropertyName, $appendToControl, $ctrl);
        }
    });
};

Global.bindSorting = function (selector, handleClass, itemClass, fnDragEndCallBack, fnDragStartCallBack) {
    if (typeof Sortable != 'undefined')
        if (Sortable) {
            $(selector).each(function (index, element) {
                if (fnDragEndCallBack && fnDragStartCallBack) {
                    Sortable.create(element, {
                        sort: true,
                        handle: handleClass,
                        draggable: itemClass,
                        animation: 150,
                        easing: "cubic-bezier(1, 0, 0, 1)",
                        onEnd: fnDragEndCallBack,
                        onStart: fnDragStartCallBack
                    });
                }
                else {
                    Sortable.create(element, {
                        sort: true,
                        handle: handleClass,
                        draggable: itemClass,
                        animation: 150,
                        easing: "cubic-bezier(1, 0, 0, 1)"
                    });
                }
            });
        }
}

setTimeout(function () {
    if ($(".main-dynamically-added-container").length > 0) {
        var $itemsInSections = ''; //null;
        Global.bindSorting(".toptip_container", ".card-header", ".flexible-content-box", function (event) {
            $(event.item).parents("div.main-dynamically-added-container:first").find(".flexible-content-box").each(function (index, element) {


                var $mainCointainer = $(this).data('id');
                if ($mainCointainer == 'ownerckeditor') {
                    var editorlength = 1;
                    $('.ownerckedi').each(function (index, ctrlName) {
                        var ids = $(this).attr('id');
                        var datavalue = CKEDITOR.instances[ids].getData();
                        setTimeout(function () {
                            CKEDITOR.instances[ids].destroy();
                            CKEDITOR.replace(ids, { 'width': '100%', 'height': 100 });
                        }, 100);
                        editorlength = (editorlength + 1)
                    });
                }


                var $mainCointainer = $(this).parents("div.main-dynamically-added-container:first");
                var displayOrderProperty = $mainCointainer.find("button.add-multiple-section").data("display-order-property");
                if (displayOrderProperty && displayOrderProperty != "") {
                    $(element).find("[ID$=__" + displayOrderProperty + "]").val(index);
                }
                else if ($(element).find("[ID$=__DisplayOrder]").length > 0) {
                    displayOrderProperty = "DisplayOrder";
                    $(element).find("[ID$=__" + displayOrderProperty + "]").val(index);
                }
                $itemsInSections = $mainCointainer.find(".flexible-content-box i.fa-plus").parent();
                $itemsInSections.trigger("click");

            });
        }, function (event) {
            if ($itemsInSections && $itemsInSections.length > 0) {
                $itemsInSections.parent().trigger("click");


            }
        });
    }
}, 200);


$(document).on("click", "button.remove-dynamically-added i.fa-remove", function () {

    var $container = $(this).parents(".flexible-content-box:first");
    var $addNewButton = $container.parents(".main-dynamically-added-container:first").find("button.add-multiple-section:last");
    var controlNames = $addNewButton.data("control-names");
    var callback = $addNewButton.data("remove-fncallback");

    controlNames = controlNames.split(',');

    $container.fadeOut("fast", function () {
        $container.remove();
        $(controlNames).each(function (index, ctrlName) {
            Global.resetFlexibleContentBoxCtrlIndexing($addNewButton);
        });
        $(".flexible-content-box input[type=file].app-img-uploader").each(function () {
            $.resetImageCtrlIndexing($(this));
        });
        $(".main-dynamically-added-container .main-dynamically-added-container button.add-multiple-section").each(function () {
            //alert('5');
            Global.resetFlexibleContentBoxCtrlIndexing($(this));
        });
        if (callback && Global.IsNotNullOrEmptyString(callback)) {
            eval(callback);
        }
    });
});
$(document).on("click", "button.add-multiple-section", function () {
    var $button = $(this);
    var url = $(this).data("action-url");
    var $appendToControl = $(this).parents(".main-dynamically-added-container:first").find($(this).data("append-to-control"));
    var propertyName = $(this).data("property-name");
    var controlNames = $(this).data("control-names");
    var parentControlName = $(this).data("parent-control-name");
    var ctrlIndex = $(this).data("index");
    var callback = $(this).data("fncallback");
    controlNames = controlNames.split(',');
    var propertyId = propertyName.replace(/\[/g, '_').replace(/\]/g, '_').replace(/\./g, '_');
    var newPropertyName = propertyName;

    Global.AjaxPost({ url: url }, function (result) {
        $appendToControl.append(result).promise().done(function () {


            $appendToControl.find("[ID$=__DisplayOrder]").each(function (index) {
                if ($appendToControl.find("[ID$=__DisplayOrder]").length > 0) {
                    displayOrderProperty = "DisplayOrder";
                    $appendToControl.find("[ID$=__" + displayOrderProperty + "]").eq(index).val(index);
                }

            });

            $(controlNames).each(function (index, ctrlName) {
                if (parentControlName && parentControlName.length > 0) {
                    var parentPropertyName = propertyName.substring(0, propertyName.lastIndexOf('['));
                    //parentPropertyName = parentPropertyName.substring(0, parentPropertyName.lastIndexOf('_'));

                    var $parentCtrl = $appendToControl.parents(".flexible-content-box:first").find('[id^="' + parentPropertyName.replace(/\[/g, '_').replace(/\]/g, '_').replace(/\./g, '_') + '_"][id$="_' + parentControlName + '"]:first');
                    if ($parentCtrl && $parentCtrl.length > 0) {
                        parentPropertyName = $parentCtrl.attr("name");
                        parentPropertyName = parentPropertyName.substring(0, parentPropertyName.lastIndexOf('.') + 1);
                    }
                }

                if (parentPropertyName && Global.IsNotNullOrEmptyString(parentPropertyName)) {
                    newPropertyName = parentPropertyName + (propertyName.substring(propertyName.lastIndexOf(']') + 2, propertyName.length));
                }

                var $ctrl = $appendToControl.find('[id^="' + propertyId + '_"][id$="_' + ctrlName + '"]:first');
                if ($ctrl) {
                    Global.ResetCtrlIdIndexing(newPropertyName, $appendToControl, $ctrl);
                }
            });

            Global.bindMaxLength();

            if (callback && Global.IsNotNullOrEmptyString(callback)) {
                eval(callback);
            }

            var $currentFlexibleContent = $appendToControl.find('.flexible-content-box:last');
            $currentFlexibleContent.find('textarea.ckeditor').each(function () {
                var ckeditorCtrlId = $(this).attr('id');
                if (ckeditorCtrlId != undefined && Global.IsNotNullOrEmptyString(ckeditorCtrlId) && ckeditorCtrlId.length > 0) {
                    CKEDITOR.replace(ckeditorCtrlId, {});
                    Global.AttachEventCKEditor(ckeditorCtrlId);
                }
            });

            $('html, body').animate({
                scrollTop: $button.offset().top - ($currentFlexibleContent.height() + 150)
            }, 400);


            $button.focus();
        });


    });

});

String.prototype.replaceBetween = function (start, end, value) {
    return this.substring(0, start) + value + this.substring(end);
};
function reindexLast(ele, index) {

    var i = $(ele).attr("name");
    var start = i.lastIndexOf("[");
    var end = i.lastIndexOf("]") + 1;
    i = i.replaceBetween(start, end, "[" + index + "]");
    $(ele).attr("name", i);
}

Global.ReIndexList = function (list) {
    if (list.length) {

        var i = 0;
        list.each(function (f, g) {
            $(g).find(":input.reindex:not(:disabled)").each(function (h, j) {
                reindexLast(j, i);
            });
            i++
        });
    }
};
//$(".alert-success,.alert-danger").fadeTo(8000, 500).slideUp(500, function () {
//    $(".alert-success,.alert-danger").slideUp(500);
//});
$(".alert-success,.alert-danger").slideUp(7000);

//Global.SelectAutoComplete = function (url, selectId, placeholder) {
//    $("." + selectId).select2({
//        placeholder: placeholder,
//        multiple: true,
//        allowClear: true,
//        tags: false,
//        async: true,
//        minimumInputLength: 0,
//        closeOnSelect: false,
//        width: 'resolve',
//        escapeMarkup: function (markup) { return markup; },
//        ajax: {
//            url: domain + url,
//            dataType: 'json',
//            data: function (params) {
//                return {
//                    search: params.term,
//                    page: params.page
//                };
//            },
//            processResults: function (response, params) {
//                params.page = params.page || 0;
//                var items = [];
//                for (var i = 0; i < response.data.length; i++) {
//                    items.push({
//                        id: response.data[i].id,
//                        text: response.data[i].name,
//                    });
//                }
//                return {
//                    results: items,
//                    pagination: {
//                        more: (params.page * 5) < response.totalItems
//                    }
//                };
//            },
//        },
//        cache: true,
//    });
//}

Global.ValidateTabCtrl = function ($form) {
    $('.tab-no-validation input, .tab-no-validation select, .tab-no-validation textarea').each(function () {
        $(this).rules('remove', 'required');
    });
    var _validator = $form.data('validator');
    _validator.settings.ignore = "";
    try {
        $form.valid();
    } catch (error) {

    }
    var isValid = $form.validate().valid();
    if (!isValid) {


        var $ctrlToFocus = null;
        var makeActiveTab = function (ctrlObject) {
            if (!_validator.element(ctrlObject)) {
                var $ctrl = $(ctrlObject);
                var items = $form.validate();
                var $tabls = $ctrl.parents('div.tab-pane');
                if ($tabls.length > 0) {
                    if ($ctrlToFocus == null) {
                        $tabls.each(function (index, item) {
                            var tabId = $(item).attr('id');
                            $('.nav-tabs a[href="#' + tabId + '"]').tab('show');
                            //var tabId = $ctrl.parents('div.tab-pane').find('div.tab-pane.fade').attr('id')
                            //var innerTabId = $(this).parents("div.tab-pane:first").attr('id');
                            //$('.nav-tabs a[href="#' + innerTabId + '"]').tab('show');
                            //$('.nav-tabs a[href="#' + tabId + '"]').tab('show');
                            $ctrlToFocus = $ctrl;
                            isValid = false;
                        });
                    }
                }
                else {
                    $ctrlToFocus = $ctrl;
                }
                return false;
            }
            return true;
        }

        $form.find('input[type=text], select, textarea').each(function () {
            if ($ctrlToFocus == null && !_validator.element(this)) {
                if (!makeActiveTab(this)) {
                    $ctrlToFocus.focus();
                }
            }
        });
    }
    return isValid;
}


$(document).off("keypress", "input[pattern]").on("keypress", "input[pattern]", function (event) {
    var pattern = $(this).attr("pattern");
    var regxPattern = new RegExp(pattern);
    return regxPattern.test($(this).val() + event.originalEvent.key);
});


Global.TabAutocollapse = function (menu, maxHeight) {
    var nav = $(menu)
    var navHeight = nav.innerHeight()
    if (navHeight >= maxHeight) {

        $(menu + ' .dropdown').removeClass('d-none');
        $(".navbar-nav").removeClass('w-auto').addClass("w-100");

        while (navHeight > maxHeight) {
            //  add child to dropdown
            var children = nav.children(menu + ' li:not(:last-child)');
            var count = children.length;
            $(children[count - 1]).prependTo(menu + ' .dropdown-menu');
            navHeight = nav.innerHeight();
        }
        $(".navbar-nav").addClass("w-auto").removeClass('w-100');

    }
    else {

        var collapsed = $(menu + ' .dropdown-menu').children(menu + ' li');

        if (collapsed.length === 0) {
            $(menu + ' .dropdown').addClass('d-none');
        }

        while (navHeight < maxHeight && (nav.children(menu + ' li').length > 0) && collapsed.length > 0) {
            //  remove child from dropdown
            collapsed = $(menu + ' .dropdown-menu').children('li');
            $(collapsed[0]).insertBefore(nav.children(menu + ' li:last-child'));
            navHeight = nav.innerHeight();
        }

        if (navHeight > maxHeight) {
            Global.TabAutocollapse(menu, maxHeight);
        }

    }
};
var loader = function (isShow) {
    if (isShow == null || isShow == undefined) {
        $("div.loader").toggle();
    }
    else if (isShow) {
        $("div.loader").show();
    }
    else {
        $("div.loader").hide();
    }
}
$(document).ready(function () {
    if (typeof (CKEDITOR) !== 'undefined') {
        CKEDITOR.config.height = 120;
    }
});
//$(document).ready(function () {
//    CKEDITOR.replace('ckeditor', {
//        customConfig: {
//            uiColor: '#ffffff',
//            toolbarGroups: [
//                { name: 'links' }, { name: 'insert' },
//                { name: 'basicstyles', groups: ['basicstyles', 'cleanup', "Bold", "Italic", "Link", "BulletedList", "NumberedList", "BlockQuote", "Underline", "Strike", "Subscript", "Superscript", "-", "RemoveFormat"] },
//                { name: 'styles' },
//                { name: 'colors' }
//            ],
//            skin: 'kama',
//            resize_enabled: false,
//            removePlugins: 'elementspath,save,magicline',
//            colorButton_foreStyle: {
//                element: 'font',
//                attributes: { 'color': '#(color)' }
//            },
//            height: 188,
//            removeDialogTabs: 'image:advanced;link:advanced',
//            removeButtons: 'Subscript,Superscript,Anchor,Source,Table',
//            format_tags: 'p;h1;h2;h3;pre;div',
//            allowedContent: true,
//        }
//    });
//});


Global.rebindValidators = function (formid) {
    var $form = $(`#${formid}`);
    $form.unbind();
    $form.data("validator", null);
    $.validator.unobtrusive.parse($form);
    $form.validate($form.data("unobtrusiveValidation").options);
}

