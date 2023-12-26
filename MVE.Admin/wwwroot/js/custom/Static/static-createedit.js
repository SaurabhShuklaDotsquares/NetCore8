(function ($) {
    'use strict';
    function PageCreateEdit() {
        var $this = this, formAddEdit;

       function attachEventCKEditor(instance) {
            CKEDITOR.instances[instance].on("instanceReady", function () {
                this.on("change", function () {
                    CKEDITOR.instances[instance].updateElement();
                });
            });
        }
        function initilizeForm() {

            $(document).on("submit", "#frm-staticpages", function (event) {
                event.preventDefault();
                event.stopImmediatePropagation();
                var formData = new FormData(this);
                var url = this.action;
                $.ajax({
                    url: url,
                    type: 'POST',
                    data: formData,
                    success: function (result) {
                        if (result.isSuccess == true) {
                            Global.ShowMessage(result.message, Global.MessageType.Success);
                            window.setTimeout(function () {
                                window.location.href = "/Admin/static/Index";
                            }, 2000);
                        } else {
                            Global.ShowMessage(result.errorMessage, Global.MessageType.Error);

                        }
                    },
                    error: function (result) {
                        Global.ShowMessage(result.errorMessage == undefined ? "Undefiend Requset" : result.errorMessage, Global.MessageType.Error);
                    },
                    cache: false,
                    contentType: false,
                    processData: false
                });
                return false;
            });


        }
        function InitializeFormControls() {

            //CKEDITOR.disableAutoInline = true; // Disable auto inline mode
            //CKEDITOR.on('instanceReady', function (e) {
            //    if (e.editor.name === 'Content') {
            //        e.editor.on('change', function () {
            //            e.editor.updateElement();
            //        });
            //    }
            //});

            CKEDITOR.replace('Content');
            attachEventCKEditor('Content');
            //new Global.FormHelper($("#frm-create-edit-page form"), { updateTargetId: "validation-summary", validateSettings: { ignore: 'true' } });
        }
        $this.init = function () {
            InitializeFormControls();
            initilizeForm()
        };
    }
    $(function () {
        var self = new PageCreateEdit();
        self.init();
    });
}(jQuery));

