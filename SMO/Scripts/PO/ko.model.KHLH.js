

(function (ko, handlers, unwrap, extend, register) {
    extend(handlers, {
        inputMask: {
            init: function (element, valueAccessor, allBindingsAccessor) {
                var $element = $(element);
                var mask = valueAccessor();

                var options = {
                    autoUnmask: true,
                    rightAlign: false,
                    showMaskOnHover: false,
                    showMaskOnFocus: false,
                    allowMinus: false,
                    allowPlus: false
                }

                ko.utils.extend(options, allBindingsAccessor().inputMaskOptions);

                if (mask === 'money') {
                    options.alias = 'numeric';
                    options.groupSeparator = '.';
                    options.autoGroup = true;
                    options.digits = 0;
                    options.radixPoint = ',';
                    options.digitsOptional = false;
                    options.prefix = '';

                    $element.inputmask('decimal', options);
                } else
                    $element.inputmask(mask, options);
            }
        },
    });
}(ko, ko.bindingHandlers, ko.unwrap, ko.utils.extend, ko.utils.registerEventHandler));


var IsChange = false;
function CheckChange(obj) {
    var p = ko.computed(function () {
        return ko.toJS(obj);
    });
    p.subscribe(function (value) {
        IsChange = true;
    });
}

function AppoveAll() {
    $.each(model.ObjListDetail(), function (key, value) {
        value.APPROVE_QUANTITY(value.QUANTITY());
    });
}

function AddDetail(material) {
    model.AddDetail(number, material);
    number = number + 1;
}

function RemoveDetail(obj) {
    model.RemoveDetail($(obj).attr("order"));
}

var Detail = function (order, marterialCode, quantity, approveQuantity, unitCode) {
    var self = this;
    self.ORDER = ko.observable(order);
    self.QUANTITY = ko.observable(quantity);
    self.APPROVE_QUANTITY = ko.observable(approveQuantity);
    self.UNIT_CODE = ko.observable(unitCode);
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
    self.ClickAppove = function () {
        self.APPROVE_QUANTITY(self.QUANTITY());
    }
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
        if (self.ObjListDetail().filter(function (data) {
            return data.MATERIAL_CODE() === material;
        }).length !== 0) {
            Message.func.AlertDanger({ Message: { Code: "", Message: "Mặt hàng này đã được chọn!" } });
            return false;
        }

        self.ObjListDetail.push(new Detail(order, material, 1000, 0, 'L'));
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