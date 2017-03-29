function RegisterCollapseElement(toggle, target) {
    $(target).addClass("glyphicon").addClass("glyphicon-collapse-up")

    $(toggle).on("shown.bs.collapse", function () {
        $(target).removeClass("glyphicon-collapse-down").addClass("glyphicon-collapse-up");
    });

    $(toggle).on("hidden.bs.collapse", function () {
        $(target).removeClass("glyphicon-collapse-up").addClass("glyphicon-collapse-down");
    });
}

function DeleteGenerator(id, name, callback)
{
    BootstrapDialog.show({
        type: BootstrapDialog.TYPE_DANGER,
        title: "Confirm Delete",
        message: "Are you sure you wish to delete the generator " + name + "? This action cannot be undone.",
        buttons: [{
            icon: 'glyphicon glyphicon-remove',
            label: 'Cancel',
            action: function (dialog) {
                dialog.close();
                return false;
            }
        },
        {
            icon: 'glyphicon glyphicon-ok',
            label: 'Ok',
            action: function (dialog) {
                dialog.close();
                callback();
                return true;
            }
        }]
    })
}
