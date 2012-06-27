function OpenPreviewDialog() {
    ExecuteOrDelayUntilScriptLoaded(function () {
        var regex = new RegExp("[\\?&]"+"ID"+"=([^&#]*)");
        var qs = regex.exec(window.location.href);
        var options = SP.UI.$create_DialogOptions();
        //alert(qs[1]);
        options.url = SP.Utilities.Utility.getLayoutsPageUrl('SharepointEmails/PreviewPage.aspx');
        options.url += "?Source=" + document.URL+"&ID="+qs[1];
        /*alert('Navigating to dialog at: ' + options.url);*/
        options.width = 400;
        options.height = 300;
        options.title = 'Preview';
        SP.UI.ModalDialog.showModalDialog(options);
    }, "sp.js")
}

function InitSeBool(id, onchecked, onunchecked) {
    $("#" + id).bind("change", function () {
        if ($(this).is(":checked")) {
            updateRow(document.getElementById(onchecked), "");
            updateRow(document.getElementById(onunchecked), "none");
        }
        else {
            updateRow(document.getElementById(onunchecked), "");
            updateRow(document.getElementById(onchecked), "none");
        }
    });

    $("#" + id).change();
}

function updateRow(obj, hide) {
    if (obj == null) return;
    var row = getParentByTagName(obj, "tr");
    if (row != false) {
        row.style.display = hide;
    }
}

function getParentByTagName(obj, tag) {
    var obj_parent = obj.parentNode;
    if (!obj_parent) return false;
    if (obj_parent.tagName.toLowerCase() == tag) return obj_parent;
    else return getParentByTagName(obj_parent, tag);
}
