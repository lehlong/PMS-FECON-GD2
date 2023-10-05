var Notify = function (pkId, contents, raw_contents, isRead, isCounted) {
    var self = this;
    self.PKID = ko.observable(pkId);
    self.CONTENTS = ko.observable(contents);
    self.RAW_CONTENTS = ko.observable(raw_contents);
    self.IS_REAED = ko.observable(isRead);
    self.IS_COUNTED = ko.observable(isCounted);
    self.CLASS_IS_NOT_READED = ko.computed(function () {
        if (self.IS_REAED() == false) {
            return "li-is-not-readed";
        };
        return "";
    }, this);
}

var ModelNotify = function () {
    var self = this;
    self.IntCountNew = ko.observable();
    self.ObjListNotify = ko.observableArray([]);
}