(function ($) {
    $.fn.clearItems = function (title, message) {
        if (this.children().length > 0) {
            var element = this;
            BootstrapDialog.show({
                type: BootstrapDialog.TYPE_DANGER,
                title: title,
                message: message,
                buttons: [{
                    icon: 'glyphicon glyphicon-remove',
                    label: 'Cancel',
                    action: function (dialog) {
                        dialog.close();
                    }
                },
                {
                    icon: 'glyphicon glyphicon-ok',
                    label: 'Ok',
                    action: function (dialog) {
                        element.empty();
                        dialog.close();
                    }
                }]
            })
        }
        return this;
    };

    $.fn.addItem = function (url) {
        var element = this;
        $.ajax({
            async: true,
            url: url,
            type: 'GET',
            success: function (partialView) {
                var item = $(partialView).appendTo(element);
                item.EnableDisableItemUpDown();
                item.prev().EnableDisableItemUpDown();
            }})
        return this;
    };
    
    $.fn.deleteItem = function () {
        var parent = this.parents('[data-item-container]')
        this.remove();
        parent.EnableDisableItemUpDowns();
    }

    $.fn.moveItemUp = function () {
        var previous = this.prev();
        if (previous != null && previous.length > 0)
        {
            this.insertBefore(previous);
            this.EnableDisableItemUpDown();
            previous.EnableDisableItemUpDown();
        }
    }

    $.fn.moveItemDown = function () {
        var next = this.next();
        if (next != null && next.length > 0) {
            this.insertAfter(next);
            this.EnableDisableItemUpDown();
            next.EnableDisableItemUpDown();
        }
    }

    $.fn.EnableDisableItemUpDowns = function () {
        for (let item of this.children("[data-item-row]"))
        {
            $(item).EnableDisableItemUpDown();
        }
    }

    $.fn.EnableDisableItemUpDown = function () {
        if (this != null) {
            if (this.prev() == null || this.prev().length == 0) {
                this.find('[data-toggle = "MoveItemUp"]').attr("Disabled", true);
            }
            else {
                this.find('[data-toggle = "MoveItemUp"]').removeAttr("Disabled");
            }
            if (this.next() == null || this.next().length == 0) {
                this.find('[data-toggle = "MoveItemDown"]').attr("Disabled", true);
            }
            else {
                this.find('[data-toggle = "MoveItemDown"]').removeAttr("Disabled");
            }
        }
    }
}(jQuery));
