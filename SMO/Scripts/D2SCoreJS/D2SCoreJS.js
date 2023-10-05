/*
------------------------------------------------------------------------------
- Control
------------------------------------------------------------------------------
*/
window.TypeControl = {
    LABEL: "LABEL",
    TEXTBOX: "TEXTBOX",
    TEXTAREA: "TEXTAREA",
    SELECTBOX: "SELECTBOX",
    CHECKBOX: "CHECKBOX",
    RADIO: "RADIO",
};

/*
------------------------------------------------------------------------------
- Message
------------------------------------------------------------------------------
*/
window.Message = {
    Config: {
        DefaultMessageAction: "DIALOG",
        DefaultAlertAction: "#divAlert",
        Separator: "_",
    },
    Type: {
        Unknow: "UNKNOW",
        Redirect: "REDIRECT",
        Json: "JSON",
        Message: "MESSAGE",
        Alert: "ALERT",
        Dialog: "DIALOG",
        JsFunction: "JSFUNCTION",
        JsCommand: "JSCOMMAND",
        AlertSuccess : "ALERTSUCCESS",
        AlertInfo : "ALERTINFO",
        AlertWarning : "ALERTWARNING",
        AlertDanger : "ALERTDANGER"
    },
    func: {
        shiftFunction: function (_functionName, _transferObject) {
            Message.temp.recentData = _transferObject;
            if (_functionName && _transferObject) {
                switch (_functionName) {
                    case Message.Type.Redirect:
                        Message.func.Redirect(_transferObject);
                        break;
                    case Message.Type.Json:
                        Message.func.Json(_transferObject);
                        break;
                    case Message.Type.Message:
                        Message.func.Message(_transferObject);
                        break;
                    case Message.Type.Alert:
                        Message.func.Alert(_transferObject);
                        break;
                    case Message.Type.Dialog:
                        Message.func.Dialog(_transferObject);
                        break;
                    case Message.Type.JsFunction:
                        Message.func.JsFunction(_transferObject);
                        break;
                    case Message.Type.JsCommand:
                        Message.func.JsCommand(_transferObject);
                        break;
                    case Message.Type.Unknow:
                        Message.func.Unknow(_transferObject);
                        break;
                    case Message.Type.AlertSuccess:
                        Message.func.AlertSuccess(_transferObject);
                        break;
                    case Message.Type.AlertDanger:
                        Message.func.AlertDanger(_transferObject);
                        break;
                    case Message.Type.AlertInfo:
                        Message.func.AlertInfo(_transferObject);
                        break;
                    case Message.Type.AlertWarning:
                        Message.func.AlertWarning(_transferObject);
                        break;
                    default:
                        Message.executeMultiCommand(_transferObject);
                        break;
                }
            }
        },
        Unknow: function (_transferObject) {
            var result = false;

            return result;
        },
        Redirect: function (_transferObject) {
            var result = false;
            if (_transferObject.ExtData && Object.prototype.toString.call(_transferObject.ExtData) == "[object String]") {
                window.location = _transferObject.ExtData;
                result = true;
            }
            return result;
        },
        Json: function (_transferObject) {
            var result = false;

            return result;
        },
        Message: function (_transferObject) {
            var result = false;
            if (_transferObject) {
                switch (Message.Config.DefaultMessageAction) {
                    case Message.Type.Alert:
                        Message.func.Alert(_transferObject);
                        break;
                    case Message.Type.Dialog:
                        Message.func.Dialog(_transferObject);
                        break;
                }
            }
            return result;
        },
        Alert: function (_transferObject) {
            var result = false;
            if (_transferObject.Message && _transferObject.Message.Message) {
                alert(_transferObject.Message.Message);
                result = true;
            }
            return result;
        },
        Dialog: function (_transferObject) {
            var result = false;
            $(Message.temp.defaultMessageTarget + " " + Message.temp.messageSelector).html("");
            $(Message.temp.defaultMessageTarget + " " + Message.temp.descriptionSelector).html("");
            $(Message.temp.defaultMessageTarget + " " + Message.temp.messageSelector).html(_transferObject.Message.Message);
            $(Message.temp.defaultMessageTarget + " " + Message.temp.descriptionSelector).html(_transferObject.Message.Detail);
            //$(Message.temp.defaultMessageTarget + " " + Message.temp.descriptionSelector).hide();
            //if (_transferObject.Message.Detail == "" || _transferObject.Message.Detail == null) {
            //    $('#btnDetailDialog').hide();
            //} else {
            //    $('#btnDetailDialog').show();
            //}
            $(Message.temp.defaultMessageTarget).modal('show');
            return result;
        },
        JsFunction: function (_transferObject) {
            var result = false;
            if (_transferObject.ExtData && Object.prototype.toString.call(_transferObject.ExtData) == "[object String]") {
                eval(_transferObject.ExtData + "(Message.temp.recentData);");
            }
            return result;
        },
        JsCommand: function (_transferObject) {
            var result = false;
            if (_transferObject.ExtData && Object.prototype.toString.call(_transferObject.ExtData) == "[object String]") {
                eval(_transferObject.ExtData);
            }
            return result;
        },
        
        AlertSuccess: function (_transferObject) {
            var animateEnter = 'animated fadeInDown';
            var animateExit = 'animated fadeOutUp';
            var message = "";
            if (_transferObject.Message && _transferObject.Message.Message) {
                var message = "(MSG" + _transferObject.Message.Code + ") " + _transferObject.Message.Message;
                if (_transferObject.Message.Detail) {
                    message += "<br/>" + _transferObject.Message.Detail
                }
                result = true;
            }

            $.notify({
                message: message
            },
            {
                type: "alert-success",
                allow_dismiss: true,
                newest_on_top: true,
                timer: 2000,
                placement: {
                    from: "top",
                    align: "right"
                },
                animate: {
                    enter: animateEnter,
                    exit: animateExit
                },
                template: '<div data-notify="container" class="bootstrap-notify-container alert alert-dismissible {0} role="alert">' +
                '<button type="button" aria-hidden="true" class="close" data-notify="dismiss">×</button>' +
                '<span data-notify="icon"></span> ' +
                '<span data-notify="title">{1}</span> ' +
                '<span data-notify="message">{2}</span>' +
                '<div class="progress" data-notify="progressbar">' +
                '<div class="progress-bar progress-bar-{0}" role="progressbar" aria-valuenow="0" aria-valuemin="0" aria-valuemax="100" style="width: 0%;"></div>' +
                '</div>' +
                '<a href="{3}" target="{4}" data-notify="url"></a>' +
                '</div>'
            });
        },
        AlertInfo: function (_transferObject) {
            var animateEnter = 'animated fadeInDown';
            var animateExit = 'animated fadeOutUp';
            var message = "";
            if (_transferObject.Message && _transferObject.Message.Message) {
                var message = "(MSG" + _transferObject.Message.Code + ") " + _transferObject.Message.Message;
                if (_transferObject.Message.Detail) {
                    message += "<br/>" + _transferObject.Message.Detail
                }
                result = true;
            }

            $.notify({
                message: message
            },
            {
                type: "alert-info",
                allow_dismiss: true,
                newest_on_top: true,
                timer: 2000,
                placement: {
                    from: "top",
                    align: "right"
                },
                animate: {
                    enter: animateEnter,
                    exit: animateExit
                },
                template: '<div data-notify="container" class="bootstrap-notify-container alert alert-dismissible {0} role="alert">' +
                '<button type="button" aria-hidden="true" class="close" data-notify="dismiss">×</button>' +
                '<span data-notify="icon"></span> ' +
                '<span data-notify="title">{1}</span> ' +
                '<span data-notify="message">{2}</span>' +
                '<div class="progress" data-notify="progressbar">' +
                '<div class="progress-bar progress-bar-{0}" role="progressbar" aria-valuenow="0" aria-valuemin="0" aria-valuemax="100" style="width: 0%;"></div>' +
                '</div>' +
                '<a href="{3}" target="{4}" data-notify="url"></a>' +
                '</div>'
            });
        },
        AlertWarning: function (_transferObject) {
            var animateEnter = 'animated fadeInDown';
            var animateExit = 'animated fadeOutUp';
            var message = "";
            if (_transferObject.Message && _transferObject.Message.Message) {
                var message = "(MSG" + _transferObject.Message.Code + ") " + _transferObject.Message.Message;
                if (_transferObject.Message.Detail) {
                    message += "<br/>" + _transferObject.Message.Detail
                }
                result = true;
            }

            $.notify({
                message: message
            },
            {
                type: "alert-warning",
                allow_dismiss: true,
                newest_on_top: true,
                timer: 4000,
                delay: 0,
                placement: {
                    from: "top",
                    align: "right"
                },
                animate: {
                    enter: animateEnter,
                    exit: animateExit
                },
                template: '<div data-notify="container" class="bootstrap-notify-container alert alert-dismissible {0} role="alert">' +
                '<button type="button" aria-hidden="true" class="close" data-notify="dismiss">×</button>' +
                '<span data-notify="icon"></span> ' +
                '<span data-notify="title">{1}</span> ' +
                '<span data-notify="message">{2}</span>' +
                '<div class="progress" data-notify="progressbar">' +
                '<div class="progress-bar progress-bar-{0}" role="progressbar" aria-valuenow="0" aria-valuemin="0" aria-valuemax="100" style="width: 0%;"></div>' +
                '</div>' +
                '<a href="{3}" target="{4}" data-notify="url"></a>' +
                '</div>'
            });
        },
        AlertDanger: function (_transferObject) {
            var animateEnter = 'animated fadeInDown';
            var animateExit = 'animated fadeOutUp';
            var message = "";
            if (_transferObject.Message && _transferObject.Message.Message) {
                var message = "(MSG" + _transferObject.Message.Code + ") " + _transferObject.Message.Message;
                if (_transferObject.Message.Detail) {
                    message += "<br/>" + _transferObject.Message.Detail
                }
                result = true;
            }

            $.notify({
                message: message
            },
            {
                type: "alert-danger",
                allow_dismiss: true,
                newest_on_top: true,
                timer: 4000,
                delay: 0,
                placement: {
                    from: "top",
                    align: "right"
                },
                animate: {
                    enter: animateEnter,
                    exit: animateExit
                },
                template: '<div data-notify="container" class="bootstrap-notify-container alert alert-dismissible {0} role="alert">' +
                '<button type="button" aria-hidden="true" class="close" data-notify="dismiss">×</button>' +
                '<span data-notify="icon"></span> ' +
                '<span data-notify="title">{1}</span> ' +
                '<span data-notify="message">{2}</span>' +
                '<div class="progress" data-notify="progressbar">' +
                '<div class="progress-bar progress-bar-{0}" role="progressbar" aria-valuenow="0" aria-valuemin="0" aria-valuemax="100" style="width: 0%;"></div>' +
                '</div>' +
                '<a href="{3}" target="{4}" data-notify="url"></a>' +
                '</div>'
            });
        },
    },
    temp: {
        recentData: null,
        defaultMessageTarget: "#divMessageContent",
        messageSelector: "[hid='message']",
        descriptionSelector: "[hid='description']",
    },

    execute: function (_transferObject) {
        var object = _transferObject;
        if (Object.prototype.toString.call(_transferObject) == "[object String]") {
            if (_transferObject.indexOf("AlertAndRedirectToLogin") >= 0) {
                object = $.parseJSON(_transferObject);
            }
        }
        Message.func.shiftFunction(object.Type, object);
    },
    executeMultiCommand: function (_transferObject) {
        if (_transferObject && _transferObject.Type) {
            var commands = _transferObject.Type.split(Message.Config.Separator);
            for (var i = 0; i < commands.length; i++) {
                var commandName = commands[i].trim();
                Message.func.shiftFunction(commandName, _transferObject);
            }
        }
    }
};

/*
------------------------------------------------------------------------------
- Form
------------------------------------------------------------------------------
*/

var isMobile = {
    Android: function () {
        return navigator.userAgent.match(/Android/i);
    },
    BlackBerry: function () {
        return navigator.userAgent.match(/BlackBerry/i);
    },
    iOS: function () {
        return navigator.userAgent.match(/iPhone|iPad|iPod/i);
    },
    Opera: function () {
        return navigator.userAgent.match(/Opera Mini/i);
    },
    Windows: function () {
        return navigator.userAgent.match(/IEMobile/i);
    },
    any: function () {
        return (isMobile.Android() || isMobile.BlackBerry() || isMobile.iOS() || isMobile.Opera() || isMobile.Windows());
    }
};

window.Forms = {
    numberOpenDialog: 0,
    isAuthorize : 1,
    isRunningAjax: false,
    defaultDivSearchAdvance : '#divSearchAdvance',
    defaultHtmlTarget: "#mainContent",
    rightHtmlTarget: "#rightContent",
    ganttDetailRightHtmlTarget: "#rightContentGanttDetail",
    defaultLoading: '#divLoading',
    defaultDivResult : '#divResult',
    autoSubmit: "#SUBMIT#",
    defaultCheckAll: "#chkAll",
    defaultCheckItem: "chkItem",
    GetDefaultAjaxParams: function () {
        var headers = {
            'RequestVerificationToken': $('#antiForgeryToken').val(),
        };
        var result = {
            url: '',
            data: {},
            dataType: "html",
            type: 'GET',
            contentType: 'application/json; charset=UTF-8',
            headers : headers,
            timeout: 120000,
            statusCode: {},
            jsonp: false,
            jsonpCallback: null,
            crossDomain: false,
            success: Forms.AjaxLoadSuccessHandler,
            error: Forms.AjaxErrorHandler,
            complete: Forms.AjaxCompleteHandler,
            htmlTarget : Forms.defaultHtmlTarget
        };
        return result;
    },
    
    GetAjaxParams: function (params) {
        var result = Forms.GetDefaultAjaxParams();
        if (params && $.isPlainObject(params)) {
            $.extend(result, params);
            if (result.type.toLowerCase() == "post") {
                result.data = JSON.stringify(result.data);
            }
        }
        return result;
    },
    
    LoadFormMenu: function (menu) {
        $(Forms.rightHtmlTarget).html("");
        $(Forms.rightHtmlTarget).hide(500);

        $(".active").removeClass("active");
        $(menu).parent().addClass("active");
        $(menu).parent().parent().parent().addClass("active");
        $(Forms.defaultHtmlTarget).html("");
        var url = $(menu).attr("url");
        Forms.LoadAjax(url);
        //$('html, body').animate({ scrollTop: 0 }, 0);
    },

    LoadAjaxRight : function(params){
        if (params) {
            var ajaxParams = {};
            if ($.isPlainObject(params)) {
                ajaxParams = Forms.GetAjaxParams(params);
                ajaxParams.success = function (response) {
                    Forms.AjaxLoadSuccessHandler(response, Forms.rightHtmlTarget);
                };
            } else {
                ajaxParams.url = params;
                ajaxParams.success = function (response) {
                    Forms.AjaxLoadSuccessHandler(response, Forms.rightHtmlTarget);
                };
                ajaxParams = Forms.GetAjaxParams(ajaxParams);
            }
            Forms.ShowLoading();
            Forms.isRunningAjax = true;
            $.ajax(ajaxParams);
        }
    },

    LoadGanttAjaxRight: function (params) {
        if (params) {
            var ajaxParams = {};
            if ($.isPlainObject(params)) {
                ajaxParams = Forms.GetAjaxParams(params);
                ajaxParams.success = function (response) {
                    Forms.AjaxLoadSuccessHandler(response, Forms.ganttDetailRightHtmlTarget);
                };
            } else {
                ajaxParams.url = params;
                ajaxParams.success = function (response) {
                    Forms.AjaxLoadSuccessHandler(response, Forms.ganttDetailRightHtmlTarget);
                };
                ajaxParams = Forms.GetAjaxParams(ajaxParams);
            }
            Forms.ShowLoading();
            Forms.isRunningAjax = true;
            $.ajax(ajaxParams);
        }
    },

    LoadAjax: function (params) {
        if (params) {
            var ajaxParams = {};
            if ($.isPlainObject(params)) {
                ajaxParams = Forms.GetAjaxParams(params);
                ajaxParams.success = function (response) {
                    Forms.AjaxLoadSuccessHandler(response, ajaxParams.htmlTarget);
                };
            } else {
                ajaxParams.url = params;
                ajaxParams.success = function (response) {
                    Forms.AjaxLoadSuccessHandler(response, ajaxParams.htmlTarget);
                };
                ajaxParams = Forms.GetAjaxParams(ajaxParams);
            }
            Forms.ShowLoading();
            Forms.isRunningAjax = true;
            $.ajax(ajaxParams);
        }
    },

    Ajax: function (params) {
        if (params) {
            var ajaxParams = {};
            if ($.isPlainObject(params)) {
                ajaxParams = Forms.GetAjaxParams(params);
            } else {
                ajaxParams.url = params;
                ajaxParams = Forms.GetAjaxParams(ajaxParams);
            }
            Forms.ShowLoading();
            Forms.isRunningAjax = true;
            $.ajax(ajaxParams);
        }
    },
    AjaxWithoutLoading: function (params) {
        if (params) {
            var ajaxParams = {};
            if ($.isPlainObject(params)) {
                ajaxParams = Forms.GetAjaxParams(params);
            } else {
                ajaxParams.url = params;
                ajaxParams = Forms.GetAjaxParams(ajaxParams);
            }
            Forms.isRunningAjax = true;
            $.ajax(ajaxParams);
        }
    },

    ToggleActive: function (url, data, obj) {
        var ajaxParams = {};
        ajaxParams.url = url;
        ajaxParams.type = "POST";
        ajaxParams.data = data;
        ajaxParams.dataType = "json";
        ajaxParams.success = function (response) {
            Forms.HideLoading();
            //$(obj).parent().children().toggle();
            Message.execute(response);
        }
        Forms.Ajax(ajaxParams);
    },

    Back: function (isSubmit, title) {
        Forms.SetTitleHeader(title);
        $(Forms.defaultHtmlTarget).show();
        $(Forms.actionHtmlTarget).html("");
        $(Forms.actionHtmlTarget).hide();
        if (isSubmit) {
            $('#frmSearch').submit();
        }
    },
   

    Save: function (form) {
        $(form).submit();
    },
    
    Close: function (viewId, htmlTarget) {
        if (htmlTarget == undefined || htmlTarget == null || htmlTarget == '') {
            htmlTarget = Forms.defaultHtmlTarget;
        }
        $("#" + viewId).remove();
        $(htmlTarget + ' .child-content').last().show();
    },

    CloseRightContent : function(){
        $(Forms.rightHtmlTarget).hide(500);
    },
    CloseGanttDetailRightContent: function () {
        $(Forms.ganttDetailRightHtmlTarget).hide(500);
    },

    CheckUnAuthorize: function (response) {
        Forms.isAuthorize = 1;
        if (!$.isPlainObject(response)) {
            if (response.indexOf('"Code":"1100"') > -1) {
                var checkAuth = JSON.parse(response);
                Forms.isAuthorize = 0;
                Message.execute(checkAuth);
            }
        }
        else {
            var checkAuth = response;
            if (checkAuth.Message != undefined && checkAuth.Message.Code == "1100") {
                Forms.isAuthorize = 0;
                Message.execute(checkAuth);
            }
        }
    },

    AjaxBeginHandler: function () {
        Forms.ShowLoading();
    },
    
    AjaxLoadSuccessHandler: function (response, htmlTarget) {
        Forms.HideLoading();
        Forms.isRunningAjax = false;
        Forms.CheckUnAuthorize(response);
        if (Forms.isAuthorize == 0) {
            return false;
        }
        if (htmlTarget == Forms.defaultHtmlTarget) {
            $(htmlTarget).children().hide();
            //var newElement = $(response).hide();
            $(htmlTarget).append(response);
            //$(newElement).show();
        }
        else if (htmlTarget == Forms.rightHtmlTarget) {
            $(htmlTarget).html(response);
            $(htmlTarget).show(500);
        } else if (htmlTarget == Forms.ganttDetailRightHtmlTarget) {
            $(htmlTarget).html(response);
            $(htmlTarget).show(500);
        } else if (htmlTarget) {
            $(htmlTarget).children().hide();
            $(htmlTarget).append(response);
        }
        Message.execute(response);
    },    
    
    AjaxSuccessHandler: function (response, status, xhr, viewId, updateTargetId) {
        Forms.isRunningAjax = false;
        Forms.HideLoading();
        Forms.CheckUnAuthorize(response);
        if (Forms.isAuthorize == 0) {
            return false;
        }
        var updateId = Forms.defaultDivResult;
        if (updateTargetId != undefined && updateTargetId != null && updateTargetId != '') {
            updateId = updateTargetId;
        }
        $(viewId + " " + updateId).html(response);
        Message.execute(response);
    },
    
    AjaxErrorHandler: function (xhr, status, error) {
        Forms.isRunningAjax = false;
        Forms.HideLoading();
        var text = "";
        if (xhr.responseText != "" ) {
            text = xhr.responseText;
        }
        if (error != "") {
            text = xhr.responseText + "<br/>" + error;
        }

        if (text != "") {
            alert(text);
        }
        else {
            alert("Lỗi kết nối dữ liệu. Vui lòng Refresh trình duyệt (Ctrl+F5) để cập nhật lại!");
        }
        //text = "<h1>" + error + "</h1>" + text.substr(text.indexOf('</head>'), text.length);
        //$('#dialog-error-modal .modal-body').html(text);
        //$('#dialog-error-modal').modal('show');
    },
    
    AjaxCompleteHandler: function (response) {
        if (!Forms.isRunningAjax) {
            Forms.isRunningAjax = false;
        }
        Forms.HideLoading();
    },

    AjaxMessageHandler: function (response) {
        Forms.isRunningAjax = false;
        Forms.HideLoading();
        $("#cmdCancel").hide();
        Message.execute(response);
    },
    
    ShowLoading: function () {
        $(Forms.defaultLoading).show();
    },
    
    HideLoading: function () {
        $(Forms.defaultLoading).hide();
    },

    CompleteUI: function () {
        $.AdminBSB.input.activate();
        $('.selectpicker').selectpicker('render');
        $('.datepicker').inputmask('dd/mm/yyyy', { placeholder: '__/__/____' });
        $('.datepicker').datepicker({
            format: "dd/mm/yyyy",
            autoclose: true,
            keyboardNavigation: false,
            maxViewMode: 2,
            todayBtn: "linked",
            clearBtn: true,
            todayHighlight: true,
            language: "vi"
        });
        $.validator.unobtrusive.parse('form');

        //$(".btn").one('click', function () {
        //});
    },

    SubmitForm: function (formId) {
        if (formId != undefined && formId != null && formId != '') {
            $("#" + formId).submit();
        }
    },

    LoadAjaxModal: function (params, isNotRemoveModalOrther) {
        if (isNotRemoveModalOrther != true) {
            $('.modal').remove();
        }

        if (params) {
            var ajaxParams = {};
            if ($.isPlainObject(params)) {
                ajaxParams = Forms.GetAjaxParams(params);
                ajaxParams.success = function (response) {
                    Forms.AjaxLoadModalSuccessHandler(response);
                };
            } else {
                ajaxParams.url = params;
                ajaxParams.success = function (response) {
                    Forms.AjaxLoadModalSuccessHandler(response);
                };
                ajaxParams = Forms.GetAjaxParams(ajaxParams);
            }
            Forms.ShowLoading();
            Forms.isRunningAjax = true;
            $.ajax(ajaxParams);
        }
    },

    LoadAjaxModalLarge: function (params, isNotRemoveModalOrther) {
        if (isNotRemoveModalOrther != true) {
            $('.modal').remove();
        }

        if (params) {
            var ajaxParams = {};
            if ($.isPlainObject(params)) {
                ajaxParams = Forms.GetAjaxParams(params);
                ajaxParams.success = function (response) {
                    Forms.AjaxLoadModalSuccessHandler(response, 'lg');
                };
            } else {
                ajaxParams.url = params;
                ajaxParams.success = function (response) {
                    Forms.AjaxLoadModalSuccessHandler(response, 'lg');
                };
                ajaxParams = Forms.GetAjaxParams(ajaxParams);
            }
            Forms.ShowLoading();
            Forms.isRunningAjax = true;
            $.ajax(ajaxParams);
        }
    },

    AjaxLoadModalSuccessHandler: function (response, type) {
        Forms.HideLoading();
        Forms.isRunningAjax = false;
        var modalType = "";
        if (type == "lg") {
            modalType = "modal-lg";
        }

        var div1 = $('<div>', { class: 'modal fade', tabindex: '-1', role: 'dialog' });
        var div2 = $('<div>', { class: 'modal-dialog ' + modalType, role: 'document' });
        var div3 = $('<div>', { class: 'modal-content' });
        $(div3).html(response);
        $(div2).html(div3);
        $(div1).html(div2);
        $(div1).modal();
    },

    ShowModal: function (html, title, icon, modalType) {
        if (icon == undefined) {
            icon = "";
        }
        if (title == undefined) {
            title = "";
        }
        if (modalType == undefined) {
            modalType = "";
        }

        var div1 = $('<div>', { class: 'modal fade', tabindex: '-1', role: 'dialog' });
        var div2 = $('<div>', { class: 'modal-dialog ' + modalType, role: 'document' });
        var div3 = $('<div>', { class: 'modal-content' });


        var divHeader = $('<div>', { class: 'modal-header' });
        var iconHeader = $('<i>', { class: 'material-icons' });
        $(iconHeader).html(icon);
        var h4 = $('<h4>', { class: 'modal-title' });
        $(h4).html(title);
        $(divHeader).append(iconHeader);
        $(divHeader).append(h4);

        var divBody = $('<div>', { class: 'modal-body' });
        $(divBody).html(html);

        var divFooter = $('<div>', { class: 'modal-footer' });
        $(divFooter).html('<button type="button" class="btn btn-link waves-effect" data-dismiss="modal">ĐÓNG</button>');


        $(div3).append(divHeader).append(divBody).append(divFooter);
        $(div2).html(div3);
        $(div1).html(div2);
        $(div1).modal();
    },

    ShowModalLarge: function (html, title, icon) {
        Forms.ShowModal(html, title, icon, 'modal-lg');
    },

    ValidateEngine: function(formId) {
        $("#" + formId).validationEngine({scroll: false, binded: false, showOneMessage: true});
    },
    
    DeleteItems: function (url, checkboxId) {
        var lstSelected = '';
        var numItemDelete = 0;
        var numItem = $('.' + checkboxId).length;
        $('.' + checkboxId).each(function() {
            if (this.checked) {
                lstSelected += $(this).attr('modelId') + ',';
                numItemDelete++;
            }
        });
        if (lstSelected == '') {
            alert("Bạn hãy chọn ít nhất một bản ghi!");
            return;
        }

        if (confirm("Bạn có chắc chắn muốn xóa?")) {
            lstSelected = lstSelected.substring(0, lstSelected.length - 1);
            var ajaxParams = {
                url: url,
                type: "POST",
                data: { pStrListSelected: lstSelected },
                dataType : "json",
                success: function (response) {
                    Message.execute(response);
                    if (response.Message.Code == "1000") {
                        return false;
                    }
                }
            };
            Forms.Ajax(ajaxParams);
        }
    },
    
    ViewDetail: function(url, checkboxId) {
        var selected = '';
        var numItem = 0;
        
        $('.' + checkboxId).each(function () {
            if (this.checked) {
                selected = $(this).attr('modelId');
                numItem++;
            }
        });
        
        if (numItem == 0) {
            alert("Bạn phải chọn bản ghi cần xem!");
            return;
        } else if (numItem > 1) {
            alert("Bạn chỉ được chọn một bản ghi!");
            return;
        }

        Forms.LoadAjax({ url: url, data: { id: selected }});
    },
    
    LoadPopupBrowser: function (url) {
        window.open(url, null, 'width=' + ($(window).width() - 200) + ",height=" + ($(window).height - 200) + ",scrollbars=1,resizable=0,modal=1");
    },

    LoadFormMultilLanguage: function (table, field, code) {
        if (table == undefined || table == "") {
            alert("Chua truyen tham so table");
            return false;
        }
        if (field == undefined || field == "") {
            alert("Chua truyen tham so field");
            return false;
        }

        if (code == undefined || code == "") {
            alert("Chua truyen tham so code");
            return false;
        }
        Forms.LoadPopupBrowser("/MD/MultilLanguage?table=" + table + "&field=" + field + "&code=" + code);
    },

    SubmitByEnter : function(form) {
        $('#' + form).find('input').keypress(function (e) {
            if (e.which == 13) {
                e.preventDefault();
                Forms.SubmitForm(form);
            }
        });
        
        $('#' + form).find('select').keypress(function (e) {
            if (e.which == 13) {
                e.preventDefault();
                Forms.SubmitForm(form);
            }
        });
    },

    CheckAll: function (nameCheckItem, idCheckAll) {
        var checkAll = Forms.defaultCheckAll;
        var checkItem = Forms.defaultCheckItem;
        if (idCheckAll != null) {
            checkAll = idCheckAll;
        }
        if (nameCheckItem != null) {
            checkItem = nameCheckItem;
        }
        $('input:checkbox[name=' + checkItem + ']').prop('checked', $(checkAll).is(':checked'));
    },
    
    CKUpdate : function() {
        for (instance in CKEDITOR.instances){
            CKEDITOR.instances[instance].updateElement();
        }
        return true;
    },
    
    ShowSearchAdvance: function (obj) {
        if ($(Forms.defaultDivSearchAdvance).is(':hidden')) {
            $(obj).children().first().attr('class', 'fa fa-angle-double-up');
        } else {
            $(obj).children().first().attr('class', 'fa fa-angle-double-down');
        }
        $(Forms.defaultDivSearchAdvance).toggle(400);
    },
    
    DisabledDropDown: function () {
        $('option:not(:selected)').attr('disabled', true);
    },
   
    ResetForm: function (formId) {
        $('#' + formId)
        $('#' + formId)[0].reset();
        $('input:visible:enabled:first').focus();
        Forms.CompleteUI();
    },

    MaintainLanguageForm: function (formCode) {
        var params = {
            url: "/AD/Language/MaintainLanguageForm",
            data: { formCode: formCode }
        };
        Forms.LoadAjaxModalLarge(params);
    },

    SynchronizeMD: function (url) {
        var ajaxParams = {
            url: url,
            type: "POST",
            data: { },
            dataType: "json",
            success: function (response) {
                Forms.HideLoading();
                Message.execute(response);
                Forms.HideLoading();
            }
        };
        Forms.Ajax(ajaxParams);
    },

    FloatingScroll: function (obj) {
        if (!isMobile.any()) {
            $(obj).floatingScroll();
        }
    }
};

/*
------------------------------------------------------------------------------
- Paging
------------------------------------------------------------------------------
*/

window.Paging = {
    ddlPage: "#Page",
    ddlNumberRecordPerPage: "#NumberRecordPerPage",
    FirstPage: function (formId) {
        formId = "#" + formId;
        $(formId + " " + Paging.ddlPage).val("1");
        $(formId).submit();
    },
    PrePage: function (formId) {
        formId = "#" + formId;

        var vCurrentPage = parseInt($(formId + " " + Paging.ddlPage).val());
        if (vCurrentPage > 1) {
            $(formId + " " + Paging.ddlPage).val(vCurrentPage - 1);
            $(formId).submit();
        }
    },
    NextPage: function (formId) {
        formId = "#" + formId;
        var vTotalPage = parseInt($(formId + " " + Paging.ddlPage + " option:last").val());
        var vCurrentPage = parseInt($(formId + " " + Paging.ddlPage).val());
        if (vCurrentPage < vTotalPage) {
            $(formId + " " + Paging.ddlPage).val(vCurrentPage + 1);
            $(formId).submit();
        }
    },
    LastPage: function (formId) {
        formId = "#" + formId;
        var vTotalPage = parseInt($(formId + " " + Paging.ddlPage + " option:last").val());
        $(formId + " " + Paging.ddlPage).val(vTotalPage);
        $(formId).submit();
    },
    ChangePage: function (formId) {
        formId = "#" + formId;
        $(formId).submit();
    },
    ChangeRecordPerPage: function (formId) {
        formId = "#" + formId;
        $(formId + " " + Paging.ddlPage).val("1");
        $(formId).submit();
    }
}

