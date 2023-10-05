var IsChange = false;
function CheckChange(obj) {
    var p = ko.computed(function () {
        return ko.toJS(obj);
    });
    p.subscribe(function (value) {
        IsChange = true;
    });
}

function AddDetail(material) {
    model.AddDetail(number, material);
    number = number + 1;
}

function RemoveDetail(obj) {
    model.RemoveDetail($(obj).attr("order"));
}

var Detail = function (order, marterialCode, quantity, unitCode, batch) {
    var self = this;
    self.ORDER = ko.observable(order);
    self.QUANTITY = ko.observable(quantity);
    self.UNIT_CODE = ko.observable(unitCode);
    self.BATCH = ko.observable(unitCode);
    self.MATERIAL_CODE = ko.observable(marterialCode);
    self.MATERIAL_TEXT = ko.computed(function () {
        var find = lstMaterial.filter(function (data) {
            return data.CODE == self.MATERIAL_CODE();
        });
        if (find.length != 0) {
            if (find[0].TEXT.indexOf("FO") != -1)
                self.UNIT_CODE("KG");
            return find[0].TEXT;
        }
    }, this);
};

var Model = function () {
    var self = this;
    this.ObjHeader = new Object();
    this.ObjListDetail = ko.observableArray([]);
    this.ObjListDetail.removeAll();

    /*
    ---------------------------------------------------------------------------------
    - Thêm mới detail
    ---------------------------------------------------------------------------------
    */
    this.AddDetail = function (order, material) {
        //if (self.ObjListDetail().filter(function (data) {
        //    return data.MATERIAL_CODE() === material;
        //}).length !== 0) {
        //    //Message.func.AlertDanger({ Message: { Code: "", Message: "Mặt hàng này đã được chọn!" } });
        //    return false;
        //}

        self.ObjListDetail.push(new Detail(order, material, 1000, 'L', 'N40'));
        Forms.CompleteUI();
    };

    /*
    ---------------------------------------------------------------------------------
    - Xóa detail
    ---------------------------------------------------------------------------------
    */
    this.RemoveDetail = function (order) {
        self.ObjListDetail.remove(function (item) {
            return item.ORDER() == order;
        });
    };
}