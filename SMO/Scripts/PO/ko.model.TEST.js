function AddDetail(material) {
    model.AddDetail(material);
}

function RemoveDetail(obj) {
    model.RemoveDetail($(obj).attr("material"));
}

var Contract = function () {
    var self = this;
    self.PAYMENT_TERM = ko.observable();
    self.SOLD_PARTY = ko.observable();
    self.SHIP_PARTY = ko.observable();
    self.DATE_TO = ko.observable();
    self.DATE_FROM = ko.observable();
}

var Condition = function () {
    var self = this;
    self.CONDITION_CODE = ko.observable();
    self.CONDITION_NAME = ko.observable();
    self.AMOUNT = ko.observable();
    //self.AMOUNT.subscribe(function (newValue) {
    //    self.IS_CHANGE("X");
    //});
    self.CURRENCY = ko.observable();
    self.PER = ko.observable();
    self.UNIT = ko.observable();
    self.ALLOW_EDIT = ko.observable();
    self.MATERIAL_CODE = ko.observable();
    self.ITEM_NUMBER = ko.observable();
    self.IS_CHANGE = ko.computed(function () {
        if (self.ALLOW_EDIT()) {
            return "X";
        }
    });
    self.STEP_NUMBER = ko.observable();
    self.CONDITION_NUMBER = ko.observable();
    self.MATERIAL_TEXT = ko.computed(function () {
        var find = lstMaterial.filter(function (data) {
            return data.CODE == self.MATERIAL_CODE();
        });
        if (find.length != 0) {
            return find[0].TEXT;
        }
    }, this);
}

var Detail = function (itemNumber, marterialCode, quantity, approveQuantity, unitCode) {
    var self = this;
    self.ITEM_NUMBER = ko.observable(itemNumber);
    self.QUANTITY = ko.observable(quantity);
    self.APPROVE_QUANTITY = ko.observable(approveQuantity);
    self.UNIT_CODE = ko.observable(unitCode);
    self.MATERIAL_CODE = ko.observable(marterialCode);
    self.MATERIAL_TEXT = ko.computed(function () {
        var find = lstMaterial.filter(function (data) {
            return data.CODE == self.MATERIAL_CODE();
        });
        if (find.length != 0) {
            return find[0].TEXT;
        }
    }, this);

    self.AMOUNT = ko.computed(function () {
        var findCondition = model.ObjListConditionSyn.filter(function (data) {
            return data.MATERIAL_CODE == self.MATERIAL_CODE() && data.CONDITION_CODE == "PR00";
        });
        if (findCondition.length != 0) {
            return Number(findCondition[0].AMOUNT);
        }
    }, this);

    self.CURRENCY = ko.computed(function () {
        var findCondition = model.ObjListConditionSyn.filter(function (data) {
            return data.MATERIAL_CODE == self.MATERIAL_CODE() && data.CONDITION_CODE == "PR00";
        });
        if (findCondition.length != 0) {
            return findCondition[0].CURRENCY;
        }
    }, this);

    self.PER = ko.computed(function () {
        var findCondition = model.ObjListConditionSyn.filter(function (data) {
            return data.MATERIAL_CODE == self.MATERIAL_CODE() && data.CONDITION_CODE == "PR00";
        });
        if (findCondition.length != 0) {
            return findCondition[0].PER;
        }
    }, this);

    self.SUM = ko.computed(function () {
        return self.AMOUNT() * (self.QUANTITY() / self.PER());
    }, this);
};

var Model = function () {
    var self = this;
    this.ObjContract = new Contract();
    this.ObjHeader = new Object();
    this.ObjListDetail = ko.observableArray([]);
    this.ObjListDetail.removeAll();
    this.ObjListCondition = ko.observableArray([]);
    this.ObjListConditionSyn = ko.observableArray([]);
    this.ObjListContractSyn = ko.observableArray([]);
    this.ObjListMDCondition = ko.observableArray([]);

    /*
    ---------------------------------------------------------------------------------
    - Thêm mới detail
    ---------------------------------------------------------------------------------
    */
    this.AddDetail = function (material) {
        if (self.ObjListDetail().filter(function (data) {
            return data.MATERIAL_CODE() === material;
        }).length !== 0) {
            alert("Mặt hàng này đã được chọn.");
            return;
        }

        var findCondition = self.ObjListConditionSyn.filter(function (data) {
            return data.MATERIAL_CODE == material;
        });

        var findContractDetail = self.ObjListContractSyn.filter(function (data) {
            return data.MATERIAL_CODE == material;
        });

        if (findCondition.length == 0 || findContractDetail.length == 0) {
            return;
        }

        self.ObjListDetail.push(
            new Detail(findCondition[0].ITEM_NUMBER, material, 10, 0, findContractDetail[0].UNIT)
        );

        Forms.CompleteUI();

        $.each(self.ObjListMDCondition(), function () {
            var mdCondition = this;
            var find = self.ObjListConditionSyn.filter(function (data) {
                return (data.CONDITION_CODE == mdCondition.CONDITION_CODE() && data.MATERIAL_CODE == material);
            });

            // Lấy condition được đồng bộ về gán cho mặt hàng
            if (find.length != 0) {
                var condition = new Condition();
                condition.CONDITION_CODE(mdCondition.CONDITION_CODE());
                condition.CONDITION_NAME(mdCondition.CONDITION_NAME());
                condition.AMOUNT(find[0].AMOUNT);
                condition.CURRENCY(find[0].CURRENCY);
                condition.PER(find[0].PER);
                condition.UNIT(find[0].UNIT);
                condition.ALLOW_EDIT(mdCondition.ALLOW_EDIT());
                condition.MATERIAL_CODE(find[0].MATERIAL_CODE);
                condition.ITEM_NUMBER(findCondition[0].ITEM_NUMBER);
                //condition.IS_CHANGE("");
                condition.STEP_NUMBER(find[0].STEP_NUMBER);
                condition.CONDITION_NUMBER(find[0].CONDITION_NUMBER);
                self.ObjListCondition.push(condition);
            }
            //} else {
            //    var condition = new Condition();
            //    condition.CONDITION_CODE(mdCondition.CONDITION_CODE());
            //    condition.CONDITION_NAME(mdCondition.CONDITION_NAME());
            //    condition.AMOUNT(mdCondition.AMOUNT);
            //    condition.CURRENCY(mdCondition.CURRENCY);
            //    condition.PER(mdCondition.PER);
            //    condition.UNIT(mdCondition.UNIT);
            //    condition.ALLOW_EDIT(mdCondition.ALLOW_EDIT());
            //    condition.MATERIAL_CODE(mdCondition.MATERIAL_CODE);
            //    condition.ITEM_NUMBER(findCondition[0].ITEM_NUMBER);
            //    condition.IS_CHANGE("");
            //    condition.STEP_NUMBER(mdCondition.STEP_NUMBER);
            //    condition.CONDITION_NUMBER(mdCondition.CONDITION_NUMBER);
            //    self.ObjListCondition.push(condition);
            //}
        });
    };

    /*
    ---------------------------------------------------------------------------------
    - Xóa detail
    ---------------------------------------------------------------------------------
    */
    this.RemoveDetail = function (material) {
        self.ObjListDetail.remove(function (item) {
            return item.MATERIAL_CODE() == material;
        });

        self.ObjListCondition.remove(function (item) {
            return item.MATERIAL_CODE() == material;
        });
    };

    /*
    ---------------------------------------------------------------------------------
    - Xóa all detail
    ---------------------------------------------------------------------------------
    */
    this.RemoveAllDetail = function () {
        self.ObjListDetail.removeAll();
        self.ObjListCondition.removeAll();
    };
}