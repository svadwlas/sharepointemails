var switcher = new Object();
$(document).ready(function () { switcher.Init(); });
switcher.Init = function () {
    var infotext = $("#SwitchesInformation").val();
    // alert(infotext);
    switcher.swInfo = jQuery.parseJSON(infotext);
    switcher.allFieldsId = new Array();
    for (i in switcher.swInfo) {
        for (j in switcher.swInfo[i].switches) {
            var fields = switcher.swInfo[i].switches[j].fields;
            for (k in fields) {
                var id = fields[k].fieldId;
                if ($.inArray(id, switcher.allFieldsId) == -1) {
                    switcher.allFieldsId.push(id);
                }
            }
        }
    }
    //alert(swInfo);
    for (i in switcher.swInfo) {
        $("#" + switcher.swInfo[i].field.fieldId).bind("change", switcher.update)
    }

    switcher.update();
};

switcher.update = function () {
    var notShow = new Array();

    for (infoKey in switcher.swInfo) {
        var info = switcher.swInfo[infoKey];
        for (key in info.switches) {
            var sw = info.switches[key];
            var el = $("#" + info.field.fieldId);
            if (el && switcher.compare(el, sw.value)) {
                for (fieldKey in sw.fields) {
                    if ($.inArray(sw.fields[fieldKey].fieldId, notShow) == -1)
                        notShow.push(sw.fields[fieldKey].fieldId);
                }
            }
        }
    }
    for (i in switcher.allFieldsId) {
        var id = switcher.allFieldsId[i];
        if ($.inArray(id, notShow) != -1) {
            $("#" + id).closest("tr").hide();
        }
        else {
            $("#" + id).closest("tr").show();
        }
    }
}

switcher.compare = function (element, value) {
    if (!element) return false;
    if (element.is("input")) {
        if (element.attr("type") == "checkbox") {
            return element.is(":checked") && value == "true" || !element.is(":checked") && value == "false";
        }
    }
    alert("Not supported element type");
}

switcher.getParentByTagName = function (obj, tag) {
    var obj_parent = obj.parentNode;
    if (!obj_parent) return false;
    if (obj_parent.tagName.toLowerCase() == tag) return obj_parent;
    else return getParentByTagName(obj_parent, tag);
};
