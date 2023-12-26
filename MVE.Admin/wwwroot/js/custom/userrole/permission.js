(function ($) {
    function PermissionIndex() {
        var $this = this;
        function initializeForm() {
            var roleId = $("#Id").val();
            $('#jstreediv').on('loaded.jstree', function (e, data) {
                var ids = $('#PermissionSelectedIds').val().split(',');
                if (ids.length > 0) {
                    var permissionIds = ids;
                    for (var i = 0; i < permissionIds.length; i++) {
                        data.instance.select_node(permissionIds[i]);
                    }
                    $(this).jstree("open_all");
                }

            }).jstree({
                "checkbox": {
                    "three_state": true,
                    "keep_selected_style": true
                },
                'core': {
                    "check_callback": true,
                    'force_text': true,
                    "themes": { "stripes": true },
                    'data': {
                        "dataType": "json",
                        "contentType": "application/json;",
                        'url': "/Admin/UserRole/GetPermissions?id=" + roleId,
                        'data': function (node) {
                            return node;
                        },
                        "success": function (retval) {
                            return retval.d;
                        }

                    }
                },
                "plugins": [
                    "search", "checkbox", "types", "wholerow"
                ]
            });

            function uiGetParents(loSelectedNode) {
                try {
                    var selectedText = loSelectedNode.node.text;
                    if (loSelectedNode.selected.length > 0 && selectedText !== 'List') {

                        var lsSelectedID = loSelectedNode.node.id;
                        var loParent = $("#" + lsSelectedID);
                        //var loParent = loParent.parent().parent();
                        var firstChild = $(loParent).find('ul >li:first');
                        loSelectedNode.instance.select_node(firstChild);
                    }

                }
                catch (err) {
                    alert('Error in uiGetParents');
                }
            };
            $('#jstreediv').on('select_node.jstree', function (e, data) {
                var loMainSelected = data;
                uiGetParents(loMainSelected);
            });

            var formAddEditPermission = new Global.FormHelper($("#frm-add-edit-permission"), {
                updateTargetId: "validation-summary",
                validateSettings: {
                    ignore: []
                },
                beforeSubmit: function () {
                    var selectedPermissions = $("#jstreediv").jstree("get_selected");
                    $("#PermissionSelectedIds").val(selectedPermissions);
                    return true;
                }
            }, function onSuccess(result) {

                if (result) {
                    var $permissionMessageDiv = $('#permissionMessageDiv');
                    if (Global.IsNotNullOrEmptyString(result.message)) {
                        $permissionMessageDiv.addClass('alert-success').removeClass('alert-danger');
                        $permissionMessageDiv.empty().html(result.message);
                        $permissionMessageDiv.show();
                    }

                    window.setTimeout(function () {
                        $permissionMessageDiv.html('');
                        $permissionMessageDiv.hide();
                    }, 5000);
                }
            });



        }

        //$("#btnSubmit").click(function () {
        //    $("#PermissionSelectedIds").val($("#jstreediv").jstree("get_selected"));
        //});

        $this.init = function () {
            initializeForm();
        };
    }

    $(function () {
        var self = new PermissionIndex();
        self.init();
    })
})(jQuery)