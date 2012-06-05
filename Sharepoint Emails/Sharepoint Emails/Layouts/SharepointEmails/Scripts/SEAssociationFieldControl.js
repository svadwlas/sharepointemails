function OpenDialog() {
    ExecuteOrDelayUntilScriptLoaded(function () {
        var options = SP.UI.$create_DialogOptions();
        options.url = SP.Utilities.Utility.getLayoutsPageUrl('SharepointEmails/ChooseItemPage.aspx');
        options.url += "?Source=" + document.URL;
        /*alert('Navigating to dialog at: ' + options.url);*/
        options.width = 400;
        options.height = 300;
        options.title = 'Choose element';
        options.dialogReturnValueCallback = Function.createDelegate(null, CloseCallback);
        SP.UI.ModalDialog.showModalDialog(options);
    }, "sp.js")
}
function CloseCallback(result, returnValue) {
    alert('Result from dialog was: ' + result);
    if (result === SP.UI.DialogResult.OK) {
        alert('You clicked OK');
    }
    else if (result == SP.UI.DialogResult.cancel) {
        alert('You clicked Cancel');
    }
    function onOk() {
        SP.UI.ModalDialog.commonModalDialogClose(SP.UI.DialogResult.OK, 'Ok clicked'); return false;
    }
}