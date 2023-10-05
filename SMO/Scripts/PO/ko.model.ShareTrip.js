var Detail = function () {
	var self = this;
	self.PO_CODE = ko.observable();
	self.MODUL = ko.observable();
	self.DO_SAP = ko.observable();
	self.EX_DATE = ko.observable();
	self.DOC_DATE = ko.observable();
	self.OIC_PBATCH = ko.observable();
	self.OIC_PTRIP = ko.observable();
	self.STATUS_TEXT = ko.observable();
	self.STATUS_COLOR = ko.observable();
	self.VENDOR_CODE = ko.observable();
	self.IS_DISABLE_CHECK = ko.observable(false);
	self.ListTrip = ko.observableArray([]);
	self.INFO_DETAIL = ko.observable();
	self.TITLE_DETAIL = ko.observable();
    self.LINK_TO_PO = ko.computed(function () {
        return "Forms.LoadAjax('/PO/" + self.MODUL() + "/Detail?id=" + self.PO_CODE() + "')";
	}, this);
};

var Trip = function (order) {
    var self = this;
    self.AUTOID = "ck" + Math.round((Math.random() * 100000));
    self.PKID = ko.observable();
	self.Detail = ko.observable();
	self.ITEM_NUMBER = ko.observable(order);
	self.NAME = ko.computed(function () {
		return "Chuyến " + self.ITEM_NUMBER();
	}, this);
	self.IS_CHECK = ko.observable(false);
	self.OnClick = "";
	self.Detail.subscribe(function (newValue) {
		self.OnClick = "CheckTrip('" + self.Detail().PO_CODE() + "', '" +  self.ITEM_NUMBER() + "', this)";
	});
};

var Model = function () {
    var self = this;
    self.VEHICLE_CODE = ko.observable();
    self.DOC_DATE = ko.observable();
	this.ObjListShareTrip = ko.observableArray([]);
	this.ObjListShareTripDetail = ko.observableArray([]);

	self.AddTrip = function () {
		number = number + 1;
		self.ObjListShareTrip.push(new Trip(number));
		$.each(self.ObjListShareTripDetail(), function () {
			var newTrip = new Trip(number);
			newTrip.Detail(this);
			this.ListTrip.push(newTrip);
		});
	};
};

function CheckTrip(poCode, numberOrder, obj){
	$.each(model.ObjListShareTripDetail(), function () {
		if (this.PO_CODE() == poCode) {
		    $.each(this.ListTrip(), function () {
				var trip = this;
				if(trip.ITEM_NUMBER() != numberOrder){
					trip.IS_CHECK(false);
				} else {
				    trip.IS_CHECK(true);
				}
			});
			return;
		}
		//$(obj).prop('checked', $(obj).is(":checked"));
	});
	setTimeout(function () {
	    ValidateNguoiVanTai();
	}, 500);
};

function ValidateNguoiVanTai() {
    $("#divNoti").html("");
    $("#cmdSave").show();

    $.each(model.ObjListShareTrip(), function () {
        var numberOrder = this.ITEM_NUMBER();
        var lstNguoiVanTai = [];
        $.each(model.ObjListShareTripDetail(), function () {
            var detail = this;
            $.each(detail.ListTrip(), function () {
                var trip = this;
                console.log(trip.IS_CHECK());
                if (trip.ITEM_NUMBER() == numberOrder && trip.IS_CHECK() == true) {
                    lstNguoiVanTai.push(detail.OIC_PBATCH() + detail.OIC_PTRIP());
                    console.log(detail.OIC_PBATCH() + detail.OIC_PTRIP());
                }
            });
        });
        var find = lstNguoiVanTai.filter(function (a) {
            return lstNguoiVanTai.indexOf(a) !== lstNguoiVanTai.lastIndexOf(a)
        });

        if (lstNguoiVanTai.length > 1) {
            var check = false;
            var first = lstNguoiVanTai[0];
            $.each(lstNguoiVanTai, function () {
                if (this != first) {
                    check = true;
                }
            });

            if (check) {
                //$("#divNoti").show("");
                $("#cmdSave").hide();
                $("#divNoti").append("<div style='border: 1px solid; padding:10px; margin-bottom:5px;'>Thông tin người lái xe giữa các đơn hàng trong chuyến " + numberOrder + " không giống nhau.</div>");
            }
        }
    });
}