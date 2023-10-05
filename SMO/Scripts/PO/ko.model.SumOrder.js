
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

function CheckHiddenSoPhieu() {
    if (Header.IS_MORE_MATERIAL() == true) {
        $(".hddSoPhieu").hide();
        $(".dplSoPhieu").show();
    }
}

function OnChangeIsMoreMaterial(obj) {
    $(".hddSoPhieu").toggle();
    $(".dplSoPhieu").toggle();
    OnChangeBatch();
    model.ObjListDetail.removeAll();
}

function AddDetail(material, batchCode) {
    model.AddDetail(number, material, batchCode);
    number = number + 1;
    if (Header.IS_MORE_MATERIAL() == true) {
        $(".hddSoPhieu").hide();
    }
}

function RemoveDetail(obj) {
    model.RemoveDetail($(obj).attr("order"));
}

var Detail = function (order, marterialCode, quantity, numberOrder, unitCode, transmodeCode, batchCode) {
    var self = this;
    self.ORDER = ko.observable(order);
    self.QUANTITY = ko.observable(quantity);
    self.NUMBER_ORDER = ko.observable(numberOrder);
    self.UNIT_CODE = ko.observable(unitCode);
    self.MATERIAL_CODE = ko.observable(marterialCode);
    self.TRANSMODE_CODE = ko.observable(transmodeCode);
    self.BATCH_CODE = ko.observable(batchCode);
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
    self.SUM = ko.computed(function () {
        var result = 0;
        if (Header.IS_MORE_MATERIAL() == false) {
            result = self.QUANTITY() * self.NUMBER_ORDER();
        }
        else {
            result = self.QUANTITY() * Header.NUMBER_ORDER();
        }
        return result;
    });

    self.SUM_TOTAL = ko.observable();
};

var Model = function () {
    var self = this;
    this.ObjHeader = new Object();
    this.ObjListDetail = ko.observableArray([]);
    this.ObjListDetail.removeAll();
    this.ObjListTotal = ko.observableArray([]);
    /*
    ---------------------------------------------------------------------------------
    - Thêm mới detail
    ---------------------------------------------------------------------------------
    */
    this.AddDetail = function (order, material, batchCode) {
        if (Header.IS_MORE_MATERIAL() == true) {
            if (self.ObjListDetail().filter(function (data) {
                return data.MATERIAL_CODE() === material;
            }).length !== 0) {
                return false;
            }
        }
        self.ObjHeader.BATCH_CODE(batchCode);
        self.ObjListDetail.push(new Detail(order, material, 1000, 1, 'L', "ZT", batchCode));

        if (self.ObjListTotal().filter(function (data) {
            return data.MATERIAL_CODE() === material;
        }).length == 0) {
            self.ObjListTotal.push(new Detail("", material, 0, 0));
        }

        CalTotal();
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
        CalTotal();
    };

}

function CalTotal() {
    setTimeout(function () {
        model.ObjListTotal.removeAll();
        $.each(model.ObjListDetail(), function () {
            var detail = this;
            var find = model.ObjListTotal().filter(function (data) {
                return data.MATERIAL_CODE() === detail.MATERIAL_CODE();
            });

            if (find.length == 0) {
                var total = new Detail("", detail.MATERIAL_CODE(), detail.SUM(), detail.NUMBER_ORDER());
                total.SUM_TOTAL(detail.SUM());
                model.ObjListTotal.push(total);
            }
            else {
                find[0].NUMBER_ORDER(parseInt(find[0].NUMBER_ORDER()) + parseInt(detail.NUMBER_ORDER()));
                find[0].SUM_TOTAL(parseInt(find[0].SUM_TOTAL()) + parseInt(detail.SUM()));
            }
        });

        if (Header.IS_MORE_MATERIAL() == true) {
            $(".hddSoPhieu").hide();
        }
    }, 300);
}