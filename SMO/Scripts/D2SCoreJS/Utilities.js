function ShowSelectedPO() {
    var arrayPoDetail = [];
    var countSelected = 0;
    $("#trPoSelected").hide();
    $('.chkItem').each(function () {
        if (this.checked) {
            countSelected++;
            arrayPoDetail = arrayPoDetail.concat($(this).data("po-detail"));
            grouped = arrayPoDetail.reduce((map => (r, a) => {
                map.set(a.Material.TEXT, map.get(a.Material.TEXT) || r[r.push({ MATERIAL_NAME: a.Material.TEXT, QUANTITY: 0, APPROVE_QUANTITY: 0, UNIT_CODE : "L" }) - 1]);
                map.get(a.Material.TEXT).QUANTITY += a.QUANTITY;
                map.get(a.Material.TEXT).APPROVE_QUANTITY += a.APPROVE_QUANTITY;
                map.get(a.Material.TEXT).UNIT_CODE = a.UNIT_CODE;

                return r;
            })(new Map), []);
        }
    });
    if (countSelected > 0) {
        $("#trPoSelected").show();
        $("#spanCountPoSelected").html(countSelected);
        var htmlDetail = "";
        $.each(grouped, function () {
            htmlDetail += "<div>";

            htmlDetail += "<b class = 'col-red'>";
            htmlDetail += this.MATERIAL_NAME;
            htmlDetail += "</b>";

            htmlDetail += "<b class = 'col-indigo' style = 'float:right;'>";
            htmlDetail += FormatNumber(this.QUANTITY) + " | " + FormatNumber(this.APPROVE_QUANTITY) + " " + this.UNIT_CODE;
            htmlDetail += "</b>";

            htmlDetail += "</div>";
        });

        $("#divPoDetailSelected").html(htmlDetail);
    }
}

function FormatNumber(num) {
    return num.toString().replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1.')
}

//Chuyen tu xau sang so Float
function StringToFloat(pString) {
    //Convert sang so he so 10
    var vFloat = parseFloat(pString);
    if (isNaN(vFloat)) {
        return 0;
    }
    else {
        return vFloat;
    }
}
//Chuyen tu xau sang so Int
function StringToInt(pString) {
    //Convert sang so he so 10
    var vInt = parseInt(pString, 10);
    if (isNaN(vInt)) {
        return 0;
    }
    else {
        return vInt;
    }
}

function StringToDouble(pString) {
    //Convert sang so he so 10
    var vFloat = parseFloat(pString);
    if (isNaN(vFloat)) {
        return 0;
    }
    else {
        return vFloat;
    }
}

function JResetAllRowSelected(name) {
    jQuery("input[name=" + name + "]").each(function () {
        $("#LO" + this.value).css("background-color", "#F3F6FA");
    });
}


function JShowRowSelected(name) {
    jQuery("input[name=" + name + "]").each(function () {
        if ($('#' + this.id).is(':checked')) {
            //$('#LO'+this.value).attr("class", "midle_row");   
            $("#LO" + this.value).css("background-color", "#FFFFCC");
        }
        else {
            //$('#LO'+this.value).attr("class", "odd_row"); 
            $("#LO" + this.value).css("background-color", "#F3F6FA");
        }
    });
}

function JCheckAll(id, name) {
    $('input:checkbox[name=' + name + ']').attr('checked', $('#' + id).is(':checked'));
    JShowRowSelected(name);
}



function JSelectBuySellOnclick(type) {
    var vBoolean = true;
    if (type == "BUY") {
        jQuery("input[name=chkOrder]").each(function () {

            if ($('#hdnOrder' + this.value).val().indexOf("B") >= 0) {
                $('#' + this.id).attr('checked', true);
            }
            else {
                $('#' + this.id).attr('checked', false);
                vBoolean = false;
            }
        });
    }
    else {
        jQuery("input[name=chkOrder]").each(function () {
            if ($('#hdnOrder' + this.value).val().indexOf("S") >= 0) {
                $('#' + this.id).attr('checked', true);
            }
            else {
                $('#' + this.id).attr('checked', false);
                vBoolean = false;
            }
        });
    }
    if (vBoolean)
        $('#chkAllOrder').attr('checked', true);
    else
        $('#chkAllOrder').attr('checked', false);
    JShowRowSelected("chkOrder");
}

function JItemOnclick(id, itemId, itemName) {
    if (!$('#' + itemId).is(':checked')) {
        $('#' + id).attr('checked', false);
        //$('#LO'+$('#'+itemId).val()).attr("class", "odd_row");   
        $('#LO' + $('#' + itemId).val()).css("background-color", "#F3F6FA");
    }
    else {
        var vBoolean = true;
        jQuery("input[name=" + itemName + "]").each(function () {
            if (vBoolean && !$('#' + this.id).is(':checked')) {
                vBoolean = false;
            }
            if ($('#' + this.id).is(':checked')) {
                //$('#LO'+this.value).attr("class","midle_row");      
                $("#LO" + this.value).css("background-color", "#FFFFCC");
            }
            else {
                //$('#LO'+this.value).attr("class","odd_row"); 
                $("#LO" + this.value).css("background-color", "#F3F6FA");
            }
        });
        if (vBoolean) $('#' + id).attr('checked', true);
    }


}

function JGetAllCheckboxID(name) {
    var vList = "";
    jQuery("input[type=checkbox][name=" + name + "]").each(function () {
        if ($('#' + this.id).is(':checked')) {
            vList = ListAppend(vList, jQuery(this).val(), ",");
        }
    });
    return vList;
}

// Lay phan tu dau tien cua danh sach
function ListGetFirst(the_list, the_separator) {
    if (the_list == "") return "";
    arr_value = the_list.split(the_separator);
    return arr_value[0];
}

// Kiem tra phan tu the_element co trong danh sach the_list hay khong
function ListHaveElement(the_list, the_element, the_separator) {
    try {
        if (the_list == "") return -1;
        if (the_list == the_element) return 1;
        if (the_list.indexOf(the_separator) == -1) return -1;
        arr_value = the_list.split(the_separator);
        for (var i = 0; i < arr_value.length; i++) {
            if (arr_value[i] == the_element) {
                return i;
            }
        }
    } catch (e) {; }
    return -1;
}
//Dem so phan tu trong danh sach
function ListCountElement(the_list, the_separator) {
    if (the_list == "") return -1;
    arr_value = the_list.split(the_separator);
    if (arr_value.length > 0) {
        return arr_value.length;
    }
    return -1;
}
// add a value to a list
function ListAppend(the_list, the_value, the_separator) {
    var list = the_list;
    the_value = the_value + ""; //Chuyen the_value sang kieu xau
    if (list == "") list = the_value;
    else if (the_value != "") list = list + the_separator + the_value;
    return list;
}


function OnChangeSelectBox(selectbox) {
    $(selectbox).each(function (index) {
        $(this).next().html($(this).find("option:selected").text());
        $(this).attr("title", $(this).find("option:selected").text());
    });
}

function OnFocusSelectBox(selectbox) {
    $(selectbox).each(function (index) {
        $(this).next().addClass("divImageFocus");
        
        $(this).on('keyup', function () {
            $(this).change();
        });
    });
}

function OnBlurSelectBox(selectbox) {
    $(selectbox).each(function (index) {
        $(this).next().removeClass("divImageFocus");
    });
}

function OnClickAdvanceSearch(object) {
    $('[hid=details]').toggle(300);
    var advance = $(object);
    if (advance.attr("toggle") == "true") {
        advance.attr("toggle", "false");
        $(advance).removeAttr("src");
        $(advance).attr("src", "/Images/advance_search2.png");
    } else {
        advance.attr("toggle", "true");
        $(advance).removeAttr("src");
        $(advance).attr("src", "/Images/advance_search.png");
    }
}

function ShowSearchAdvance() {
    $('.divSearchAdvanceHidden').toggle(100);
}


function SearchAdvance(form, element, value, type, object) {
    ResetForm(form);
    if (type == 'textbox' || type == 'hidden') {
        $(element).val(value);
    } else if (type == 'selectlist') {
        $(element + " option[value='" + value + "']").attr("selected", "selected");
    }
    $(object).parent().hide();
    $(form).submit();

}

function ResetForm(form) {
    $(form).find(':input').each(function() {
        var type = this.type, tag = this.tagName.toLowerCase();
        if (type == 'text' || type == 'password' || tag == 'textarea' || type == 'hidden')
            this.value = '';
        else if (type == 'checkbox' || type == 'radio')
            this.checked = false;
        else if (tag == 'select')
            this.selectedIndex = -1;
    });
}

function ActiveMD(object, url, code) {
    //alert();
    $.ajax({
        url: url,
        type: "POST",
        data: { Code: code, Value: $(object).is(":checked") }
    }).done(function () {

    });
}

/*
----------------------------------------------------------------------------------------
- So ra menu con tren menu chinh
----------------------------------------------------------------------------------------
*/
function ShowChildMenu(ParentMenu) {
    var name = $(ParentMenu).attr("name");
    if ($(ParentMenu).attr("toggle") == "true") {
        $(ParentMenu).attr("toggle", "false");
        $(ParentMenu).children().first().attr("src", "/Images/arrow1.png");
    }
    else {
        $(ParentMenu).attr("toggle", "true");
        $(ParentMenu).children().first().attr("src", "/Images/arrow2.png");
    }
    $(ParentMenu).parent().children("#divChildMenu[name = '" + name + "']").toggle(500);

    $(".parentmenuactive").removeClass("parentmenuactive");
    $(this).addClass("parentmenuactive");
}

function LoadMenuForm(menu) {
    $(".childmenuactive").removeClass("childmenuactive");
    $(menu).children().addClass("childmenuactive");

    $(".parentmenuactive").removeClass("parentmenuactive");
    $(menu).closest("#divChildMenu").prev().addClass("parentmenuactive");


    var url = $(menu).attr("url");
    Forms.LoadAjax(url);
}

function LoadMenuFromDialog(menu, url) {
    $(".divMenuActive").addClass("divMenuInActive");
    $(".divMenuActive").removeClass("divMenuActive");
    $(menu).addClass("divMenuActive");
    $(menu).removeClass("divMenuInActive");
    Forms.LoadAjax(url);
}

function TogglePanelLogin() {
    $('#divLogin').toggle('slide', { direction: 'up' }, 500);
}

function TogglePanelDetail(value) {
    $(value).toggle('slide', { direction: 'up' }, 1000);
}

function AlertAndRedirectToLogin(response) {
    alert("Đã hết phiên đăng nhập. Xin mời đăng nhập lại");
    window.location = '/Authorize/Logout';
}

function ConvertDateInJson(value) {
    try {
        var date = new Date(value);
        var mm = date.getMonth() + 1;
        var dd = date.getDate();

        return [
            (dd > 9 ? '' : '0') + dd,
            (mm > 9 ? '' : '0') + mm,
            date.getFullYear()
        ].join('/');
    } catch (e) {
        return value;
    }
}

function ConvertDateTimeInJson(value) {
    try {
        var date = new Date(value);
        var mm = date.getMonth() + 1;
        var dd = date.getDate();
        var hh = date.getHours()
        var mi = date.getMinutes();
        var ss = date.getSeconds();

        return [
            (dd > 9 ? '' : '0') + dd,
            (mm > 9 ? '' : '0') + mm,
            date.getFullYear()
        ].join('/') + " " + (hh > 9 ? '' : '0') + hh + ":" + (mi > 9 ? '' : '0') + mi + ":" + (ss > 9 ? '' : '0') + ss;
    } catch (e) {
        return value;
    }

    //if (value != null && jQuery.type(value) == 'string' && value.indexOf('/Date') != -1) {
    //    var date = new Date(parseInt(value.substr(6)));
    //    var mm = date.getMonth() + 1;
    //    var dd = date.getDate();

    //    return [
    //        (dd > 9 ? '' : '0') + dd,
    //        (mm > 9 ? '' : '0') + mm,
    //        date.getFullYear()
    //    ].join('/') +  date.getHours() + ":" + date.getMinutes() + ":" + date.getSeconds();
    //} else {
    //    return value;
    //}
}

//Convert được những string dạng "20151201" -> 01/12/2015
function ConvertStringToDateT(value) {
    if (value != null && jQuery.type(value) == 'string') {
        var date = $.datepicker.parseDate("yymmdd", value);
        return $.datepicker.formatDate("dd/mm/yy", date);
    } else {
        return value;
    }
}
//Maintan language
function MaintainLanguageForm(formCode) {
    Forms.ShowDialogBootStrap('/AD/Language/MaintainLanguageForm?formCode=' + formCode, 'Maintain language', 'fa-cog');
}

//Cấm dùng các nút Ctrl như D,T,1,2...
//function DisabledCtrl() {
//    $(document).on('keydown', 'input[type="text"], textarea', function (event) {
//        if (event.ctrlKey) {
//            var charCode = String.fromCharCode(event.keyCode);
//            if (charCode == 'D' || charCode =='M') {
//                event.preventDefault();
//            }
//        }
//    });
//}

function DisabledCtrl() {
    $(document).keydown(function (event) {
        if (event.ctrlKey) {
            var charCode = String.fromCharCode(event.keyCode);
            if (charCode == 'D' || charCode == 'M') {
                event.preventDefault();
            }
        }
    });
}

function PlayBeep() {
    var beep = new Beep(22050);
    beep.play(500, 1, [Beep.utils.amplify(8000)]);
}

function SetDivHeightToBottom() {
    var windowHeight = $(window).height();
    var height = windowHeight - $("#divTableReponsive").offset().top - 10;
    $("#divTableReponsive").css("height", height + "px");
}

function ShowHideDiv(obj,timeOut) {
    $(obj).show();
    setTimeout(function () {
        $(obj).hide();
    }, timeOut);
}

function ExportTableToExcel(tableID) {
    var table = $(tableID);
    $(table).table2excel({
        name: "Data",
        filename: "export_data",//do not include extension
        fileext: ".xlsx" // file extension
    });
}

function DownloadFile(fileId) {
    $('<iframe>', {
        src: "/CM/ReferenceFile/DownloadFile/" + fileId,
        frameborder: 0,
        scrolling: 'no'
    }).appendTo('body').hide();

    //window.location = "/EB/EB/DownloadFile/" + fileId;
    //alert("Tính năng đang phát triển!");
}


function HideFunctionFile() {
    $('.file-function').css('display', 'none');
}

function ClickFile(obj) {
    $('.file-function').css('display', 'none');
    $(obj).find('.file-function').css('display', 'table');
}

function PreviewFile(fileId) {
    var div = $('<div>', { class: 'view-file-online' });
    $(div).load("/CM/ReferenceFile/ViewFileOnline/" + fileId);
    $('body').append(div);
    $(div).show();
}

function OpenApplication(fileId, fileExt) {
    fileExt = fileExt.toLowerCase();
    var ajaxParams = {
        url: "/CM/ReferenceFile/MoveFileToTemp",
        type: 'POST',
        dataType: 'json',
        data: { id: fileId },
        success: function (response) {
            if (response.State == true) {
                var url = document.location.origin + response.ExtData;
                var href = "";
                if (fileExt == ".xlsx" || fileExt == ".xls" || fileExt == ".csv") {
                    href = "ms-excel:ofv|u|" + url;
                }
                else if (fileExt == ".docx" || fileExt == ".doc") {
                    href = "ms-word:ofv|u|" + url;
                }
                else if (fileExt == ".ppt" || fileExt == ".pptx") {
                    href = "ms-powerpoint:ofv|u|" + url;
                }
                else if (fileExt == ".mpp") {
                    href = "ms-project:ofv|u|" + url;
                }

                var link = document.createElement('a');
                link.href = href;
                document.body.appendChild(link);
                link.click();
            } else {
                alert("Có lỗi xẩy ra!");
            }
        }
    };
    Forms.Ajax(ajaxParams);
}