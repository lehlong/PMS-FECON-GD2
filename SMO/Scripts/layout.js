$.validator.methods.date = function (value, element) {
    return this.optional(element) || moment(value, "DD/MM/YYYY", true).isValid();
}

var modelNotify = new ModelNotify();
modelNotify.IntCountNew(notifyService.IntCountNew);
if (notifyService.IntCountNew == 0) {
    modelNotify.IntCountNew("");
}
$.each(notifyService.ObjList, function () {
    modelNotify.ObjListNotify.push(new Notify(this.PKID, this.CONTENTS, this.RAW_CONTENTS, this.IS_REAED, this.IS_COUNTED));
});

function NotifyBrowser(pkid, title, desc) {
    try {
        if (!Notification) {
            console.log('Desktop notifications not available in your browser..');
            return;
        }
        if (Notification.permission !== "granted") {
            Notification.requestPermission();
        }
        else {
            var notification = new Notification(title, {
                icon: '/Content/pms-ico.png',
                body: desc,
            });

            // Remove the notification from Notification Center when clicked.
            notification.onclick = function () {
                $('#a' + pkid).click();
                if (modelNotify.IntCountNew() != "" || modelNotify.IntCountNew() != 0) {
                    var total = parseInt(modelNotify.IntCountNew()) - 1;
                    modelNotify.IntCountNew("");
                    if (total > 0) {
                        modelNotify.IntCountNew(total);
                    }
                }
            };
        }
    } catch (e) {
        console.log(e);
    }
}

var notification = $.connection.notificationHub;
notification.client.RefreshNotify = function (data) {
    var service = jQuery.parseJSON(data);
    modelNotify.IntCountNew(service.IntCountNew);
    if (service.IntCountNew == 0) {
        modelNotify.IntCountNew("");
    }
    modelNotify.ObjListNotify.removeAll();
    $.each(service.ObjList, function () {
        modelNotify.ObjListNotify.push(new Notify(this.PKID, this.CONTENTS, this.RAW_CONTENTS, this.IS_REAED, this.IS_COUNTED));
    });
    NotifyBrowser(service.ObjList[0].PKID, "[PMS] THÔNG BÁO", service.ObjList[0].RAW_CONTENTS)

    $("#frmViewAllNotify").submit();
};

notification.client.NotifyIsViewed = function () {
    modelNotify.IntCountNew("");
};

notification.client.NotifyIsReaded = function (pkId) {
    var find = modelNotify.ObjListNotify().filter(function (data) {
        return data.PKID() === pkId;
    });
    if (find.length !== 0) {
        find[0].IS_REAED(true);
    }
    $("#li" + pkId).attr("class", "");
};

$.connection.hub.start().done(function () {
});



function GetListDuAn() {
    try {
        $("#divDanhSachDuAn").hide();
        var ajaxParams = {
            url: '/PS/Project/ListProjectHome',
            type: 'POST',
            success: function (response) {
                response = response.trim();
                if (response != undefined && response != null && response != "") {
                    $("#divDanhSachDuAn").show();
                    $("#divDanhSachDuAn .body").html(response);
                }
            },
            error: function () {

            }
        };
        Forms.Ajax(ajaxParams);
        Forms.HideLoading();
    } catch (e) {
        console.log(e);
    }

}

function GetTopPoDCNB() {
    try {
        $("#divTopPoDCNB").hide();
        var ajaxParams = {
            url: '/PO/PO/TopPoDCNB',
            type: 'POST',
            success: function (response) {
                response = response.trim();
                if (response != undefined && response != null && response != "") {
                    $("#divTopPoDCNB").show();
                    $("#divTopPoDCNB .body").html(response);
                }
            },
            error: function () {

            }
        };
        Forms.Ajax(ajaxParams);
        Forms.HideLoading();
    } catch (e) {
        console.log(e);
    }

}

function GetTopPoMHGL() {
    try {
        $("#divTopPoMHGL").hide();
        var ajaxParams = {
            url: '/PO/PO/TopPoMHGL',
            type: 'POST',
            success: function (response) {
                response = response.trim();
                if (response != undefined && response != null && response != "") {
                    $("#divTopPoMHGL").show();
                    $("#divTopPoMHGL .body").html(response);
                }
            },
            error: function () {
            }
        };
        Forms.Ajax(ajaxParams);
        Forms.HideLoading();
    } catch (e) {
        console.log(e);
    }
}

function GetTopPoXBND() {
    try {
        $("#divTopPoXBND").hide();
        var ajaxParams = {
            url: '/PO/PO/TopPoXBND',
            type: 'POST',
            success: function (response) {
                response = response.trim();
                if (response != undefined && response != null && response != "") {
                    $("#divTopPoXBND").show();
                    $("#divTopPoXBND .body").html(response);
                }
            },
            error: function () {

            }
        };
        Forms.Ajax(ajaxParams);
        Forms.HideLoading();
    } catch (e) {
        console.log(e);
    }
}

function GetTopPoXBTX() {
    try {
        $("#divTopPoXBTX").hide();
        var ajaxParams = {
            url: '/PO/PO/TopPoXBTX',
            type: 'POST',
            success: function (response) {
                response = response.trim();
                if (response != undefined && response != null && response != "") {
                    $("#divTopPoXBTX").show();
                    $("#divTopPoXBTX .body").html(response);
                }
            },
            error: function () {

            }
        };
        Forms.Ajax(ajaxParams);
        Forms.HideLoading();
    } catch (e) {
        console.log(e);
    }
}

function GetTopPoXTHG() {
    try {
        $("#divTopPoXTHG").hide();
        var ajaxParams = {
            url: '/PO/PO/TopPoXTHG',
            type: 'POST',
            success: function (response) {
                response = response.trim();
                if (response != undefined && response != null && response != "") {
                    $("#divTopPoXTHG").show();
                    $("#divTopPoXTHG .body").html(response);
                }
            },
            error: function () {

            }
        };
        Forms.Ajax(ajaxParams);
        Forms.HideLoading();
    } catch (e) {
        console.log(e);
    }
}

function CountPOSaleOffice() {
    try {
        var ajaxParams = {
            url: '/PO/PO/CountPOSaleOffice',
            type: 'POST',
            dataType: 'json',
            success: function (response) {
                $("#divCountTongDonHang").html(response.TongDonHang);
                $("#divCountChoPheDuyet").html(response.ChoDuyet);
                $("#divCountDangLayHang").html(response.DangLayHang);
                $("#divCountDaLayHang").html(response.DaLayHang);
            },
            error: function () {
                console.log(e);
            }
        };
        Forms.Ajax(ajaxParams);
        Forms.HideLoading();
    } catch (e) {
        console.log(e);
    }
}

function CountPOTuyenSau() {
    try {
        var ajaxParams = {
            url: '/PO/PO/CountPOTuyenSau',
            type: 'POST',
            dataType: 'json',
            success: function (response) {
                $("#divCountTongDonHang").html(response.TongDonHang);
                $("#divCountChoPheDuyet").html(response.ChoDuyet);
                $("#divCountDangLayHang").html(response.DangLayHang);
                $("#divCountDaLayHang").html(response.DaLayHang);
            },
            error: function () {
                console.log(e);
            }
        };
        Forms.Ajax(ajaxParams);
        Forms.HideLoading();
    } catch (e) {
        console.log(e);
    }
}

function CountPOCustomer() {
    try {
        var ajaxParams = {
            url: '/PO/PO/CountPOCustomer',
            type: 'POST',
            dataType: 'json',
            success: function (response) {
                $("#divCountTongDonHang").html(response.TongDonHang);
                $("#divCountChoPheDuyet").html(response.ChoDuyet);
                $("#divCountDangLayHang").html(response.DangLayHang);
                $("#divCountDaLayHang").html(response.DaLayHang);
            },
            error: function () {
                console.log(e);
            }
        };
        Forms.Ajax(ajaxParams);
        Forms.HideLoading();
    } catch (e) {
        console.log(e);
    }
}

function CountPOSaleManager() {
    try {
        var ajaxParams = {
            url: '/PO/PO/CountPOSaleManager',
            type: 'POST',
            dataType: 'json',
            success: function (response) {
                $("#divCountTongDonHang").html(response.TongDonHang);
                $("#divCountChoPheDuyet").html(response.ChoDuyet);
                $("#divCountDangLayHang").html(response.DangLayHang);
                $("#divCountDaLayHang").html(response.DaLayHang);
            },
            error: function () {
                console.log(e);
            }
        };
        Forms.Ajax(ajaxParams);
        Forms.HideLoading();
    } catch (e) {
        console.log(e);
    }
}

$(function () {
    try {
        if (Notification.permission !== "granted") {
            Notification.requestPermission();
        }
    } catch (e) {
        console.log("Không hỗ trợ notification");
    }
    //if (Notification.permission !== "granted") {
    //    Notification.requestPermission();
    //}

    $('.alert').click(function (event) {
        event.stopPropagation();
    });

    $('body').click(function (event) {
        if (!($(event.target).is('.alert')) && !($(event.target).is(".btn")) && !($(event.target).is("span"))) {
            $(".alert").remove();
        }
    });

    ko.applyBindings(modelNotify, document.getElementById('divNotify'));
    ko.applyBindings(modelNotify, document.getElementById('divTopNotifyBody'));
    $("#divNotify .body").slimScroll({
        height: '400px'
    });

    //$("#divTopNotify .body").slimScroll({
    //    height: '400px'
    //});
});