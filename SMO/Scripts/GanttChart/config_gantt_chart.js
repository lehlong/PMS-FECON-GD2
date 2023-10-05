function escapeHTML(html) {
    return html.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;');
}

function colContent(task) {
    const wbs = `<li><a href="#;" onclick="clickGridButton('${task.id}', 'WBS')">WBS</a></li>`
    const boq = `<li><a href="#" onclick="clickGridButton('${task.id}', 'BOQ')">BOQ</a></li>`
    const activity = `<li><a href="#" onclick="clickGridButton('${task.id}', 'ACTIVITY')">ACTIVITY</a></li>`
    const edit = `<i class="fa gantt_button_grid gantt_grid_edit fa-edit p-r-5" onclick="gantt.showLightbox('${task.id}')"></i>`
    const deleteIcon = `<i class="fa gantt_button_grid gantt_grid_edit fa-trash p-r-5" onclick="onDeleteTask('${task.id}')"></i>`
    let action = '';
    if (editable == "False") {
        return edit;
    } else {
        switch (task.type) {
            case "PROJECT":
                if (structType == "BOQ") {
                    return `<i class="fa gantt_button_grid gantt_grid_edit fa-edit p-r-5" onclick="gantt.showLightbox('${task.id}')"></i>` +
                        `<i class="fa gantt_button_grid gantt_grid_add fa-plus create-task  p-r-5" onclick="clickGridButton('${task.id}', 'BOQ')"></i>`;
                } else {
                    return `<i class="fa gantt_button_grid gantt_grid_edit fa-edit p-r-5" onclick="gantt.showLightbox('${task.id}')"></i>` +
                        `<i class="fa gantt_button_grid gantt_grid_add fa-plus create-task  p-r-5" onclick="clickGridButton('${task.id}', 'WBS')"></i>`;
                }
                break;
            case "BOQ":
                return edit +
                    `<i class="fa gantt_button_grid gantt_grid_add fa-plus create-task  p-r-5" onclick="clickGridButton('${task.id}', 'BOQ')"></i>`
                    + deleteIcon
            case "WBS":
                action = wbs + activity;
                break;
            case "ACTIVITY":
                return edit
                    + `<i class="fa gantt_button_grid gantt_grid_add fa-plus create-task p-r-5" onclick="clickGridButton('${task.id}', 'ACTIVITY')"></i>`
                    + deleteIcon
            default:
                return "";
        }
        return `
            ${edit}
            <i class="fa gantt_button_grid gantt_grid_add fa-plus create-task  p-r-5" onclick="toggleCreateTask(this)"></i>
            <div class="dropdown" style="position:absolute; left:260px;">
                                <ul class="dropdown-menu pull-right">
                                    ${action}
                                </ul>
                            </li>
            </div>
            ${task.type === 'PROJECT' ? '' : deleteIcon}`
    }
};

$('#gantt-chart li.dropdown').hover(function () {
    $(this).addClass('open');
},
    function () {
        $(this).removeClass('open');
    });
function toggleCreateTask(e) {
    const el = $(e).next('.dropdown').find('ul.dropdown-menu');
    el.toggle();
}
function clickGridButton(id, type) {
    const task = gantt.getTask(id)
    const { order, $local_index } = task;
    const parentId = task.parent === 0 ? task.id : task.parent
    const orderNewTask = task.parent === 0 ? 0 : order + 1
    const index = task.parent === 0 ? 0 : $local_index + 1
    gantt.selectedType = type;
    gantt.createTask({
        text: `New task ${orderNewTask + 1}`, type, order: orderNewTask
    },
        parentId,
        index);
}
function onDeleteTask(taskId) {
    Swal.fire({
        title: 'Xóa hạng mục?',
        text: "Bạn có chắc muốn Xóa hạng mục này và những hạng mục con không?",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Xóa',
        cancelButtonText: 'Hủy'
    }).then((result) => {
        if (result.isConfirmed) {
            gantt.deleteTask(taskId)
        }
    })
}
gantt.config.undo = true;
// date format https://docs.dhtmlx.com/gantt/desktop__date_format.html
// specifying the date format
gantt.config.date_grid = "%d/%m/%Y";
gantt.config.date_format = "%Y-%m-%d";

gantt.plugins({
    auto_scheduling: true
});

gantt.config.inline_editors_date_processing = "keepDates";
gantt.config.auto_scheduling = true;
gantt.config.drag_links = false;

const formatFunc = gantt.date.date_to_str("%d/%m/%Y");
const cfg = gantt.config;
const strToDate = gantt.date.str_to_date(cfg.date_format, cfg.server_utc);
// config toolstip
gantt.templates.grid_date_format = function (date, _) {
    return formatFunc(date);
};
gantt.templates.tooltip_date_format = function (date) {
    return formatFunc(moment(date).toDate());
};
gantt.templates.parse_date = function (date) {
    return strToDate(date);
};
gantt.templates.format_date = function (date) {
    return moment(date).format("MM/DD/YYYY");
};
gantt.templates.task_date = function (date) {
    return formatFunc(date);
};

var getInput = function (node) {
    return node.querySelector("input");
};

var getSelect = function (node) {
    return node.querySelector("select");
};

gantt.config.editor_types.unit_editor = {
    show: function (id, column, config, placeholder) {
        // called when input is displayed, put html markup of the editor into placeholder 
        // and initialize your editor if needed:
        placeholder.innerHTML = $("#tmplUnit").html();
        $('.selectUnit').select2({ width: "250px" });
    },
    hide: function () {
        // called when input is hidden 
        // destroy any complex editors or detach event listeners from here
    },

    set_value: function (value, id, column, node) {
        const units = gantt.serverList("unit");
        var value = _.chain(units)
            .map(x => ({ ...x, display: `${x.key} - ${x.label}` }))
            .find(["display", value])
            .value()?.label ?? value;

        $(getSelect(node)).val(value).change();
    },

    get_value: function (id, column, node) {
        const units = gantt.serverList("unit")
        const value = getSelect(node).value || '';
        return _.chain(units)
            .map(x => ({ ...x, display: `${x.key} - ${x.label}` }))
            .find(["display", value])
            .value()?.key ?? value
        return "";
    },

    is_changed: function (value, id, column, node) {
        var currentValue = this.get_value(id, column, node);
        return value != currentValue;
        //called before save/close. Return true if new value differs from the original one
        //returning true will trigger saving changes, returning false will skip saving 
    },

    is_valid: function (value, id, column, node) {
        var currentValue = this.get_value(id, column, node);
        const units = gantt.serverList("unit")
        return _.find(units, ["key", currentValue]);
        // validate, changes will be discarded if the method returns false
    },

    save: function (id, column, node) {
        // only for inputs with map_to:auto. complex save behavior goes here
    },
    focus: function (node) {
        var input = getSelect(node);
        if (!input) {
            return;
        }
        if (input.focus) {
            input.focus();
        }

        if (input.select) {
            input.select();
        }
    }
}
gantt.config.editor_types.boq_editor = {
    show: function (id, column, config, placeholder) {
        // called when input is displayed, put html markup of the editor into placeholder 
        // and initialize your editor if needed:
        placeholder.innerHTML = $("#tmplBoq").html();
        $('.selectBoq').select2({ width: '300px' });
    },
    hide: function () {
        // called when input is hidden 
        // destroy any complex editors or detach event listeners from here
    },

    set_value: function (value, id, column, node) {
        const boqCodes = gantt.serverList("boqCode")
        const boqs = gantt.serverList("boq")
        const key = _.chain(boqCodes)
            .find(["label", value])
            .value()?.key ?? ''

        $(getSelect(node)).val(_.find(boqs, ["key", key])?.key ?? '').change();
        //getInput(node).value = _.find(boqs, ["key", key])?.label ?? '';
    },

    get_value: function (id, column, node) {
        const boqs = gantt.serverList("boq")
        const value = getSelect(node).value || '';
        const key = _.chain(boqs)
            .find(["label", value])
            .value()?.key ?? value
        return key
    },

    is_changed: function (value, id, column, node) {
        var currentValue = this.get_value(id, column, node);
        return value != currentValue;
        //called before save/close. Return true if new value differs from the original one
        //returning true will trigger saving changes, returning false will skip saving 
    },

    is_valid: function (value, id, column, node) {
        var currentValue = this.get_value(id, column, node);
        const boqs = gantt.serverList("boqCode")
        return _.find(boqs, ["key", currentValue]);
        // validate, changes will be discarded if the method returns false
    },

    save: function (id, column, node) {
        // only for inputs with map_to:auto. complex save behavior goes here
    },
    focus: function (node) {
        var input = getSelect(node);
        if (!input) {
            return;
        }
        if (input.focus) {
            input.focus();
        }

        if (input.select) {
            input.select();
        }
    }
}
gantt.config.editor_types.status_editor = {
    show: function (id, column, config, placeholder) {
        placeholder.innerHTML = $("#tmplStatus").html();
        $('.selectStatus').select2({
            width: "200px",
            minimumResultsForSearch: Infinity
        });
    },
    hide: function () {
        // called when input is hidden 
        // destroy any complex editors or detach event listeners from here
    },

    set_value: function (value, id, column, node) {
        const units = gantt.serverList("statuses")
        $(getSelect(node)).val(_.find(units, ["key", value])?.key ?? value).change();
    },

    get_value: function (id, column, node) {
        const statuses = gantt.serverList("statuses")
        const value = getSelect(node).value || '';
        return _.find(statuses, ["label", value])?.key ?? value
    },

    is_changed: function (value, id, column, node) {
        var currentValue = this.get_value(id, column, node);
        return value != currentValue;
        //called before save/close. Return true if new value differs from the original one
        //returning true will trigger saving changes, returning false will skip saving 
    },

    is_valid: function (value, id, column, node) {
        // validate, changes will be discarded if the method returns false
        var currentValue = this.get_value(id, column, node);
        const units = gantt.serverList("statuses")
        return _.find(units, ["key", currentValue]);
    },

    save: function (id, column, node) {
        // only for inputs with map_to:auto. complex save behavior goes here
    },
    focus: function (node) {
        var input = getSelect(node);
        if (!input) {
            return;
        }
        if (input.focus) {
            input.focus();
        }

        if (input.select) {
            input.select();
        }
    }
}

gantt.config.editor_types.quantity_editor = {
    show: function (id, column, config, placeholder) {
        // called when input is displayed, put html markup of the editor into placeholder
        // and initialize your editor if needed:
        const task = gantt.getTask(id)
        const { unitCode } = task
        let html = ''
        if (unitCode === "%") {
            html = "<input class='txtQuantity' data-inputmask=\"'alias': 'percentage', 'max': 10000000, 'min': -9999999, 'allowMinus': true, 'groupSeparator': ',','digits': 3, 'autoGroup':true\" type='text' name='" + column.name + "'/>";
        } else {
            html = "<input class='txtQuantity' data-inputmask=\"'alias': 'decimal', 'groupSeparator': ',','digits': 3, 'autoGroup':true\" type='text' name='" + column.name + "'/>";
        }
        placeholder.innerHTML = html;
        $(".txtQuantity").inputmask();
    },
    hide: function (node) {
        var input = getInput(node);
        //var input = getInput(node);
        //console.log(getInput(node).value);
        //this.set_value(input.value);
        input.remove();
        // called when input is hidden 
        // destroy any complex editors or detach event listeners from here
    },

    set_value: function (value, id, column, node) {
        const task = gantt.getTask(id)
        const { unitCode } = task
        if (unitCode === "%") {
            getInput(node).value = value * 100;
        } else {
            getInput(node).value = value;
        }
    },

    get_value: function (id, column, node) {
        const task = gantt.getTask(id)
        const { unitCode } = task
        let unformattedMask = 0
        if (unitCode === "%") {
            unformattedMask = Inputmask.unmask(getInput(node).value, { alias: 'percentage', digits: 5, groupSeparator: ',', autoGroup: true });
            if (unformattedMask) {
                unformattedMask /= 100
            }
        } else {
            unformattedMask = Inputmask.unmask(getInput(node).value, { alias: 'decimal', groupSeparator: ',', autoGroup: true });
        }
        //this.set_value(getInput(node).value, id, column, node);
        return unformattedMask || 0;
    },

    is_changed: function (value, id, column, node) {
        var currentValue = this.get_value(id, column, node);
        return value != currentValue;
        //called before save/close. Return true if new value differs from the original one
        //returning true will trigger saving changes, returning false will skip saving 
    },

    is_valid: function (value, id, column, node) {
        // validate, changes will be discarded if the method returns false
        //var currentValue = this.get_value(id, column, node);
        return true;
    },

    save: function (id, column, node) {
        // only for inputs with map_to:auto. complex save behavior goes here
    },
    focus: function (node) {
        var input = getInput(node);
        if (!input) {
            return;
        }
        if (input.focus) {
            input.focus();
        }

        if (input.select) {
            input.select();
        }
    }
}

gantt.config.editor_types.price_editor = {
    show: function (id, column, config, placeholder) {
        // called when input is displayed, put html markup of the editor into placeholder 
        // and initialize your editor if needed:
        var html = "<input class='txtQuantity' data-inputmask=\"'alias': 'decimal', 'groupSeparator': ',','digits': 0, 'autoGroup':true\" type='text' name='" + column.name + "'/>";
        placeholder.innerHTML = html;
        $(".txtQuantity").inputmask();
    },
    hide: function (node) {
        var input = getInput(node);
        //var input = getInput(node);
        //console.log(getInput(node).value);
        //this.set_value(input.value);
        input.remove();
        // called when input is hidden 
        // destroy any complex editors or detach event listeners from here
    },

    set_value: function (value, id, column, node) {
        getInput(node).value = value;
    },

    get_value: function (id, column, node) {
        var unformattedMask = Inputmask.unmask(getInput(node).value, { alias: 'decimal', groupSeparator: ',', autoGroup: true });
        //this.set_value(getInput(node).value, id, column, node);
        return unformattedMask || 0;
    },

    is_changed: function (value, id, column, node) {
        var currentValue = this.get_value(id, column, node);
        return value != currentValue;
        //called before save/close. Return true if new value differs from the original one
        //returning true will trigger saving changes, returning false will skip saving 
    },

    is_valid: function (value, id, column, node) {
        // validate, changes will be discarded if the method returns false
        var currentValue = this.get_value(id, column, node);
        return true;
    },

    save: function (id, column, node) {
        // only for inputs with map_to:auto. complex save behavior goes here
    },
    focus: function (node) {
        var input = getInput(node);
        if (!input) {
            return;
        }
        if (input.focus) {
            input.focus();
        }

        if (input.select) {
            input.select();
        }
    }
}

var textEditor = { type: "text", map_to: "text" };
var codeEditor = { type: "text", map_to: "code" };

var dateEditor = gantt.config.editor_types.date;
var dateStartEditor = { type: "date", map_to: "start_date" };
var dateFinishEditor = { type: "date", map_to: "end_date" };
//const unitEditor = { type: "select", map_to: "unitCode", options: gantt.serverList("unit") };
//const boqEditor = { type: "select", map_to: "referenceBoqId", options: gantt.serverList("boq") };
const numberQuantityEditor = { type: "quantity_editor", map_to: "quantity" };
//const numberQuantityEditor = { type: "number", map_to: "quantity"};
//const numberPriceEditor = { type: "number", map_to: "price" };
const numberPriceEditor = { type: "price_editor", map_to: "price" };
const unitEditor = { type: "unit_editor", map_to: "unitCode", }
const boqEditor = { type: "boq_editor", map_to: "referenceBoqId", }
const statusEditor = { type: "status_editor", map_to: "status", }

//gantt.templates.grid_row_class = function (start_date, end_date, item) {
//    if (item.type == "BOQ") return "bg-gantt-green";
//};
//gantt.templates.task_row_class = function (start_date, end_date, item) {
//    if (item.type == "BOQ") return "bg-gantt-green";
//};

gantt.config.open_tree_initially = true;

gantt.config.reorder_grid_columns = true;

function formatTotal(obj) {
    const { total } = obj || {};
    return Number(total).toLocaleString("en-US", { maximumFractionDigits: 0 })
}
function showIconType(obj) {
    const { type } = obj || {};
    if (type === "CHECK_LIST") {
        return null;
    }
    return `<image src="Content/Images/IconProject/${type}.png"></image>`
}
function showCheckbox(obj) {
    const { selected, id, type } = obj || {};
    //if (type === "PROJECT") {
    //    return ''
    //}
    return `<input type="checkbox" class = "ckbStruct" onChange="changeSelectedRow('${id}')" style = "position: relative; opacity: 1; left: 0;" name="selected_${id}" id="selected_${id}">`
}

//gantt.attachEvent("onTaskClick", function (id, e) {
//    //debugger;
//    var checkbox = gantt.utils.dom.closest(e.target, ".gantt-checkbox-column");

//    if (checkbox) {
//        checkbox.checked = !!checkbox.checked;
//        gantt.getTask(id).selected = true;
//        return false;
//    } else {
//        return true;
//    }
//});

function changeSelectedRow(id) {
    const task = gantt.getTask(id)
    const { selected, type } = task
    task.selected = !selected;

    gantt.silent(() => {
        gantt.updateTask(id);
        if (type == "PROJECT") {
            gantt.eachTask(function (taskEach) {
                taskEach.selected = !selected;
                const { id } = taskEach;
                gantt.updateTask(id);
            });

            $('input:checkbox[class="ckbStruct"]').prop('checked', !selected);
        }
    });
    //gantt.render();

}
function showUnitName(obj) {
    const { unitCode } = obj || {};
    const units = gantt.serverList("unit")
    if (unitCode) {
        return _.find(units, ["key", unitCode])?.label
    }
}
function showBoqName(obj) {
    const { referenceBoqId } = obj || {};
    const boqs = gantt.serverList("boq")
    if (referenceBoqId) {
        return _.find(boqs, ["key", referenceBoqId])?.label
    }
}

function sleep(milliseconds) {
    var start = new Date().getTime();
    for (var i = 0; i < 1e7; i++) {
        if ((new Date().getTime() - start) > milliseconds) {
            break;
        }
    }
}

function templateDisplayStatus(obj) {
    const { status } = obj || {};
    const units = gantt.serverList("statuses")
    return _.find(units, ["key", status])?.label ?? status;
}
function templateDisplayBoq(obj) {
    const { referenceBoqId } = obj || {};
    const boqCode = gantt.serverList("boqCode")
    return _.find(boqCode, ["key", referenceBoqId])?.label ?? '';
}
function formatQuantity(obj) {
    const { quantity, unitCode } = obj || {};
    if (quantity) {
        if (unitCode === "%") {
            return numeral(quantity).format('0,0.[000]%')
        } else {
            return numeral(quantity).format('0,0.[000]')
        }
    }
}
function formatPrice(obj) {
    const { price } = obj || {};
    if (price) {
        return Number(price).toLocaleString("en-US", { maximumFractionDigits: 0 })
    }
}

const getGanttColumns = () => {
    const columns = [
        { name: "selected", label: "#", resize: true, align: "center", width: 50, template: showCheckbox},
        { name: "type", label: "Loại", resize: true, template: showIconType, align: "center", width: 50 },
        {
            name: "buttons",
            width: 70,
            label: "#",
            template: colContent,
            resize: true,
            /*hideReadonly: true*/
        },
        //{ name: "wbs", label: "Mã", width: 40, template: gantt.getWBSCode, resize: true },
        { name: "code", label: "Code", width: 100, resize: true, editor: codeEditor },
        {
            name: "text", label: "Hạng mục", tree: true, width: '*', min_width: 300, resize: true, editor: textEditor,
            template: function (obj) {
                return escapeHTML(obj.text)
            }
        },
        { name: "start_date", align: "center", label: "Bắt đầu", resize: true, editor: dateStartEditor },
        { name: "end_date", align: "center", label: "Kết thúc", resize: true, editor: dateFinishEditor },
        { name: "unitCode", label: "Đơn vị tính", resize: true, editor: unitEditor, align: "center" },
        { name: "status", label: "Trạng thái CV", resize: true, editor: statusEditor, template: templateDisplayStatus },
        //{ name: "unitCode", label: "Đơn vị tính", resize: true, editor: unitEditor, template: showUnitName, align: "center" },
        { name: "quantity", label: "Khối lượng", resize: true, editor: numberQuantityEditor, template: formatQuantity, align: "right" },
        { name: "price", label: "Đơn giá", resize: true, editor: numberPriceEditor, template: formatPrice, align: "right" },
        { name: "total", label: "Thành tiền", resize: true, template: formatTotal, align: "right" },
        { name: "referenceBoqId", label: "Liên kết BOQ", resize: true, editor: boqEditor, template: templateDisplayBoq, align: "center" },
        { name: "vendorName", label: "Thầu phụ/NCC", resize: true },
        { name: "contractCode", label: "Số hợp đồng", resize: true, align: "center" },
    ]

    const isReadonly = gantt.config.readonly
    if (!isReadonly) {
        return columns
    }
    return _.filter(columns, ({ hideReadonly }) => !hideReadonly)
}

gantt.config.columns = getGanttColumns();

gantt.ext.inlineEditors.attachEvent("onBeforeSave", function (state) {
    const { columnName, newValue } = state
    if (columnName == "quantity" || columnName == "price")
        if (newValue == "") {
            newValue = 0;
        }
    //state.newValue = gantt.date.str_to_date("%Y/%m/%d %H:%i")(state.newValue)
    return true;
});


gantt.config.layout = {
    css: "gantt_container",
    cols: [
        {
            hide_empty: true,
            width: 400,
            min_width: 300,
            rows: [
                { view: "grid", scrollX: "gridScroll", scrollable: true, scrollY: "scrollVer" },
                { view: "scrollbar", id: "gridScroll", group: "horizontal" }
            ]
        },
        { resizer: true, width: 1 },
        {
            rows: [
                { view: "timeline", scrollX: "scrollHor", scrollY: "scrollVer" },
                { view: "scrollbar", id: "scrollHor", group: "horizontal" }
            ],
            hide_empty: true,
        },
    ]
};
const inlineEditors = gantt.ext.inlineEditors;
gantt.attachEvent("onRowDragEnd", function (id, target) {
    // https://docs.dhtmlx.com/gantt/api__gantt_onrowdragend_event.html
    updateTasksOrder();
});

gantt.attachEvent("onTaskDblClick", function (id) {
    const isReadonly = gantt.config.readonly
    if (!isReadonly) {
        return;
    }
    gantt.showLightbox(id)
});
inlineEditors.attachEvent("onBeforeEditStart", function (state) {
    const { id, columnName } = state;
    const task = gantt.getTask(id);
    if (task.type === "PROJECT") {
        return false;
    }
    if (task.type !== "WBS" && task.type !== "ACTIVITY" && columnName === "referenceBoqId") {
        return false;
    }
    if (["ACTIVITY", "PROJECT"].includes(task.type) && columnName === "code") {
        return false;
    }
    return true;
});
inlineEditors.attachEvent("onBeforeSave", function (state) {
    const { columnName, newValue, id } = state;
    if (columnName === "code" && newValue) {
        const existTasks = gantt.getTaskBy("code", newValue)
        const task = gantt.getTask(id);

        if (existTasks.length === 1 && existTasks[0].type === "PROJECT" && task.type === "WBS") {
            return true;
        }
        if (existTasks.length) {
            gantt.message({
                type: "confirm-warning",
                text: `Mã code ${newValue} đã tồn tại ở hạng mục ${existTasks[0].text}`
            })
            return false;
        }
    }
});
gantt.attachEvent("onExpand", function () {
    const $body = $('body');
    if (!$body.hasClass('ls-closed')) {
        $body.toggleClass("ls-closed")
    }
});
gantt.attachEvent("onBeforeTaskDrag", function (id, mode, e) {
    const task = gantt.getTask(id)
    return task.type !== "PROJECT"
});
gantt.attachEvent("onBeforeLightbox", function (id) {
    if (!isNaN(id)) {
        // if not a number id
        // create new task
        const task = gantt.getTask(id);
        const parentTask = gantt.getTask(task.parent)
        task.type = gantt.selectedType;
        task.projectId = gantt.projectId;
        task.start_date = parentTask.start_date
        task.end_date = parentTask.end_date
        task.user = gantt.user;

        response = gantt.ajax.post(
            'api/task/CreateTask',
            {
                ...task,
                start_date: moment(task.start_date).format('YYYY-MM-DDTHH:mm:ss'),
                end_date: moment(task.end_date).format('YYYY-MM-DDTHH:mm:ss')
            }
        ).then((response) => {
            var res = JSON.parse(response.responseText);
            if (res && res.status === 'success') {
                // response is ok
                gantt.updateTask(id, { ...task, code: res.code, status: res.statusCode });
                gantt.changeTaskId(id, res.tid);
                gantt.render();
                if (res.executeMethod) {
                    Message.func.ExecuteFunctionByName(res.executeMethod, window);
                }
                updateTasksOrder();

            } else if (res && res.status === 'error') {
                showMessage('error', 'Something went wrong! ' + res.message);
                gantt.deleteTask(id);
            } else {
                showMessage('error', 'Something went wrong!');
                gantt.deleteTask(id);
            }
            gantt.refreshData();
            return false;
        });

    }
    // if not a number id
    // show detail task
    return isNaN(id);
});

gantt.showLightbox = async function (id) {
    // code of the custom form
    calculateGanttDetail();
    if (!this.callEvent("onBeforeLightbox", [id])) return;
    Forms.LoadGanttAjaxRight(`PS/ProjectStruct/Edit?id=${id}`);
}
// return false to discard the resize action
gantt.attachEvent("onGridResizeEnd", function (_, new_width) {
    calculateGanttDetail(_, new_width);
    return true;
});
gantt.attachEvent("onColumnResizeEnd", function (_, column, new_width) {
    const { width: old_width } = column;
    calculateGanttDetail(_, $(".gantt_layout_cell.grid_cell").first()?.width() + new_width - old_width);
    return true;
});
gantt.attachEvent('onError', function (errorMessage) {
    return true;
});
//gantt.config.tooltip_hide_timeout = 3000;

function displayTooltipValue(value) {
    return value ? value : "";
}
// multiple scale
gantt.config.subscales = [
    { unit: "week", step: 1, date: "Tuần #%W" }
];
gantt.config.scale_height = 54;

// highlight weekend
gantt.templates.scale_cell_class = function (date) {
    if (date.getDay() === 0 || date.getDay() === 6) {
        return "weekend";
    }
};

gantt.templates.timeline_cell_class = function (task, date) {
    if (date.getDay() === 0 || date.getDay() === 6) {
        return "weekend";
    }
};

// server list
gantt.serverList("priority", [
    { key: 1, label: "Cao" },
    { key: 2, label: "Vừa" },
    { key: 3, label: "Thấp" }
]);

gantt.serverList("status", [
    { key: 1, label: "Đang làm" },
    { key: 2, label: "Hoàn thành" },
    { key: 3, label: "Đợi" }
]);

// set labels
var labels = gantt.locale.labels;
labels.column_status = labels.section_status = "Tình trạng";
labels.column_owner = labels.section_owner = "Giao cho";
labels.column_approver = labels.section_approver = "Người duyệt";
labels.column_priority = labels.section_priority = "Độ ưu tiên";
labels.column_description = labels.section_description = "Mô tả";
labels.column_checkList = labels.section_checkList = "Check List";
labels.column_comment = labels.section_comment = "Bình luận";

// light box
// sự kiện khi click đúp vào task
gantt.config.lightbox.sections = [
    { name: "owner", height: 30, map_to: "owner_id", type: "textarea" },
    { name: "approver", height: 30, map_to: "approver_id", type: "textarea" },
    { name: "status", height: 30, map_to: "status_id", type: "select", options: gantt.serverList("status") },
    { name: "time", type: "duration", map_to: "auto" },
    { name: "priority", height: 30, map_to: "priority", type: "select", options: gantt.serverList("priority") },
    { name: "description", height: 58, map_to: "text", type: "textarea", focus: true },
    { name: "checkList", height: 16, type: "template", map_to: "checkList_template" },
    { name: "comment", type: "textarea", map_to: "comment" }
];

var taskId = null;

// fires before a new link is added to the Gantt chart
gantt.attachEvent("onBeforeLinkAdd", function (linkId, link) {
    // link construct: {source: '', target: '', type: }
    //excludes overtaking the target task by the source task
    //in case of creating "finish_to_start" links
    if (link.type === 0) {
        var sourceTask = gantt.getTask(link.source);
        var targetTask = gantt.getTask(link.target);
        if (sourceTask.end_date >= targetTask.start_date) {
            showMessage('warning', 'This link is illegal');
            return false;
        }
    }
});

function getForm() {
    return document.getElementById("custom-form");
}

function save() {
    var task = gantt.getTask(taskId);

    task.text = getForm().querySelector("[name='description']").value;
    task.owner = getForm().querySelector("[name='owner']").value;
    task.approver = getForm().querySelector("[name='approver']").value;
    task.status = getForm().querySelector("[name='status']").value;
    task.start_date = moment(getForm().querySelector("[name='startDate']").value, "DD/MM/YYYY", true).toDate();
    task.end_date = moment(getForm().querySelector("[name='endDate']").value, "DD/MM/YYYY", true).toDate();
    task.priority = getForm().querySelector("[name='priority']").value;
    task.comment = getForm().querySelector("[name='comment']").value;

    if (task.$new) {
        delete task.$new;
        gantt.addTask(task, task.parent);
    } else {
        gantt.updateTask(task.id);
    }

    gantt.hideLightbox();
}

function cancel() {
    var task = gantt.getTask(taskId);
    if (task.$new) {
        gantt.deleteTask(task.id);
    } else {
        task.removedTasks = new Array();
    }
    gantt.hideLightbox();
}

function remove() {
    gantt.deleteTask(taskId);
    gantt.hideLightbox();
}

// type: infor, warning, error
function showMessage(type, text) {
    gantt.message({ type: type, text: text, expire: 5000 });
}

function addTaskRowGantt(item) {
    var $this = item;
    if ($("#custom-form #content-new-task-container").hasClass('hidden')) {
        $($this).addClass("btn-success");
        $($this).html("Add");
        $($this).removeClass("btn-primary");
        $("#custom-form #content-new-task-container").removeClass('hidden');
        $("#custom-form #cancel-add-task").removeClass('hidden');

        $('#custom-form #task-list-owner-gantt').val(null).trigger('change');
        $('#custom-form #task-list-owner-gantt').next().css('width', $('#custom-form #owner').siblings('span').css('width'));
    } else {
        if (!$('#custom-form #content-new-task').val()) {
            return;
        }
        // Get the template HTML and remove it from the doumenthe template HTML and remove it from the doument
        var time = new Date().getTime();
        var taskRow = document.querySelector("#template-task-row-gantt .checklist-row");
        $(taskRow).attr('id', `task-row-${time}`);
        var cbx = taskRow.querySelector("input[type=checkbox]");
        $(cbx).attr('id', `cbx_${time}`);
        $(cbx).removeAttr('checked');

        var startDate = taskRow.querySelector("input[name=task-list-start-date-hidden]");
        $(startDate).attr('id', `task_list_start_date_${time}`);
        $(startDate).val($("#custom-form #task-list-start-date").val());

        var endDate = taskRow.querySelector("input[name=task-list-end-date-hidden]");
        $(endDate).attr('id', `task_list_end_date_${time}`);
        $(endDate).val($("#custom-form #task-list-end-date").val());

        var owner = taskRow.querySelector("input[name=task-list-owner-hidden]");
        $(owner).attr('id', `task_list_owner_${time}`);
        $(owner).val($("#custom-form #task-list-owner-gantt").val());

        var label = taskRow.querySelector("label");
        $(label).attr('for', `cbx_${time}`);

        var taskContent = $('#custom-form #content-new-task').val();
        var taskContentTd = taskRow.querySelector(".task-content");
        $(taskContentTd).html(taskContent);

        var previewTemplate = taskRow.parentNode.innerHTML;

        $('#custom-form #task-list-table').append(previewTemplate);

        // reset button
        $('#custom-form #content-new-task').val('');
        $('#custom-form #cmdAddTask').addClass('disabled');
        $('#custom-form #cmdAddTask').css('cursor', 'default');
        $("#custom-form #content-new-task-container").addClass('hidden');

        $($this).removeClass("btn-success");
        $("#custom-form #cancel-add-task").addClass('hidden');
        $("#custom-form #content-new-task-container").addClass('hidden');
        $($this).html("Add an item");
        $($this).addClass("btn-primary");

        initTooltipsterGantt($('#task-list-table .tooltipster-row-gantt:last'));
    }
}

$("#custom-form #cancel-add-task").on('click', function () {
    $('#custom-form #content-new-task').val('');
    $('#custom-form #add-item').removeClass("btn-success");
    $("#custom-form #cancel-add-task").addClass('hidden');
    $("#custom-form #content-new-task-container").addClass('hidden');
    $('#custom-form #add-item').html("Add an item");
    $('#custom-form #add-item').addClass("btn-primary");
});

$('#content-new-task').on('keyup', function () {
    if (!$(this).val()) {
        $('#cmdAddTask').addClass('disabled');
        $('#cmdAddTask').css('cursor', 'default');
    } else {
        $('#cmdAddTask').removeClass('disabled');
        $('#cmdAddTask').css('cursor', 'pointer');
    }
});

function getListAddedTask() {
    var saveTasks = new Array();
    $('#task-list-table .checklist-row').each(function (index, value) {
        console.log(value);
        var status = value.querySelector("[name='cbx_status']").checked;
        var content = value.querySelector(".task-content").innerHTML;
        var elementId = value.querySelector("[name='cbx_status']").id.substring('cbx_'.length);
        var startDate = moment(value.querySelector("[name=task-list-start-date-hidden]").value, "DD/MM/YYYY", true).toDate();
        var endDate = moment(value.querySelector("[name=task-list-end-date-hidden]").value, "DD/MM/YYYY", true).toDate();
        var owner = $(value.querySelector("[name=task-list-owner-hidden]")).val();
        var id = elementId.includes("-") ? elementId : "";
        saveTasks.push(new CheckList(status, content, id, startDate, endDate, owner));
    });
    return saveTasks;
}

function tooltipsterTemplateGantt(content, owner, startDate, endDate) {
    var template = $('#tooltipster-template-gantt');
    template.find('.tooltipster-template-content').html(content);
    template.find('.tooltipster-template-owner').html(owner);
    template.find('.tooltipster-template-start-date').html(startDate);
    template.find('.tooltipster-template-end-date').html(endDate);
    return template.html();
}

function initTooltipsterGantt(item) {
    item.tooltipster({
        functionBefore: function (instance, helper) {
            var content = helper.origin.querySelector('.task-content').innerHTML;
            var owner = helper.origin.querySelector('[name=task-list-owner-hidden]').value;
            var startDate = helper.origin.querySelector('[name=task-list-start-date-hidden]').value;
            var endDate = helper.origin.querySelector('[name=task-list-end-date-hidden]').value;

            instance.content(tooltipsterTemplateGantt(content, owner, startDate, endDate));
        },
        multiple: true,
        contentAsHTML: true
        //content: 'My first tooltip',
    });
}

$(document).bind('paste', function (e) {
    gantt.ext.inlineEditors.hide()
    e.preventDefault()
    e.stopPropagation()

    const selectedNode = gantt.ext.keyboardNavigation.getActiveNode();
    if (!selectedNode) {
        return;
    }
    const ignoreColumns = ["selected", "type", "total", "vendorName", "contractCode"]
    if (gantt.structType === "COST") {
        ignoreColumns.push("code")
    } else if (gantt.structType === "BOQ") {
        ignoreColumns.push("referenceBoqId")
    }

    const { id, column } = selectedNode
    if (ignoreColumns.includes(column)) {
        return;
    }

    const pastedText = (e.originalEvent.clipboardData || window.clipboardData)?.getData('text').trim().split(/\r?\n */).map(r => r.split(/\t/));

    const columnNames = _.map(gantt.getGridColumns(), 'name')
    const indexSelectCell = columnNames.indexOf(column);

    if (pastedText.length === 0) {
        return;
    }
    let rowId = id
    const columnsPasted = columnNames.slice(indexSelectCell, indexSelectCell + pastedText[0].length)
    const boqs = gantt.serverList("boq")
    const statuses = gantt.serverList("statuses")

    const errors = []
    let currentErrorsLength = 0
    gantt.batchUpdate(() => {
        for (var i = 0; i < pastedText.length; i++) {
            const currentTask = gantt.getTask(rowId)
            var taskIndex = gantt.getGlobalTaskIndex(rowId);
            const objModifiedValue = _.chain(columnsPasted)
                .zipObject(pastedText[i])
                .omit(ignoreColumns)
                .mapValues((value, key, currentObj) => {
                    switch (key) {
                        case "status":
                            if (!value) {
                                errors.push({ type: errorType.REQUIRED_FIELD, taskIndex })
                            }
                            const statusId = _.chain(statuses)
                                .find(["label", ("" + value).trim()])
                                .value()?.key
                            if (!statusId) {
                                errors.push({ type: errorType.WRONG_STATUS, taskIndex })
                            }
                            return statusId
                        case "text":
                            if (!value) {
                                errors.push({ type: errorType.REQUIRED_FIELD, taskIndex })
                            }
                            return value
                        case "code":
                            errors.push(validateCode(value, currentTask, taskIndex))
                            return value
                        case "unitCode":
                            errors.push(validateUnitCode(value, currentTask, taskIndex))
                            return value
                        case "price":
                            const price = convertToNumber(value)
                            errors.push(validatePrice(price, currentTask, taskIndex))
                            return price
                        case "referenceBoqId":
                            const key = _.chain(boqs)
                                .find(({ label }) => label.startsWith(`${value} - `))
                                .value()?.key
                            if (!key && value?.trim()) {
                                errors.push({ type: errorType.WRONG_BOQ_LINK, taskIndex })
                            }
                            return key
                        case "start_date":
                            if (!value) {
                                errors.push({ type: errorType.REQUIRED_FIELD, taskIndex })
                            } else {
                                errors.push(validateStartDate(value, currentTask, taskIndex, currentObj))
                            }
                            return moment(value, "DD/MM/YYYY", true).toDate()
                        case "end_date":
                            if (!value) {
                                errors.push({ type: errorType.REQUIRED_FIELD, taskIndex })
                            } else {
                                errors.push(validateEndDate(value, currentTask, taskIndex, currentObj))
                            }
                            return moment(value, "DD/MM/YYYY", true).toDate()
                        case "quantity":
                            const unit = _.get(currentObj, "unitCode", _.get(currentTask, "unitCode"))
                            let quantity = value
                            if (value.includes("%") && unit === "%") {
                                quantity = value.trim().substring(0, value.trim().length - 1)
                                quantity = convertToNumber(quantity)
                                errors.push(validateQuantity(quantity, currentTask, taskIndex))
                            } else {
                                quantity = convertToNumber(value)
                                errors.push(validateQuantity(quantity, currentTask, taskIndex))
                            }

                            if (unit === "%") {
                                return quantity / 100
                            }
                            return quantity
                        default:
                            return value
                    }
                })
                .value()

            if (_.chain(errors).omitBy(_.isNil).size().value() === currentErrorsLength) {
                gantt.updateTask(rowId, { ...currentTask, ...objModifiedValue })
            } else {
                currentErrorsLength = _.chain(errors).omitBy(_.isNil).size().value()
            }

            rowId = gantt.getNext(rowId)
            if (!rowId) {
                return;
            }
        }
    });
    gantt.render();
    if (currentErrorsLength > 0) {
        // show error message
        const realErrors = _.chain(errors)
            .omitBy(_.isNil)
            .groupBy('type')
            .map((err, type) => {
                return `${getMessageError(type)}. Lỗi tại các dòng: ${_.chain(err).map(({ taskIndex }) => taskIndex + 1).uniq().join(", ").value()}`
            })
            .join('<br/>')
            .value()

        Message.func.AlertDanger({ Message: { Code: '', Message: 'Cập nhật dữ liệu không thành công', Detail: realErrors } });
    }
});
const convertToNumber = (text) => {
    if (typeof text === "number") {
        return numeral(isNaN(text) ? 0 : text).format('0.[000]')
    } else if (typeof text === "string") {
        return numeral(isNaN(+text.replaceAll(",", "")) ? 0 : +text.replaceAll(",", "")).format('0.[000]')
    }
}
const validateCode = (code, currentTask, taskIndex) => {
    if (gantt.getTaskBy(({ code: taskCode, id }) => taskCode === code && id !== currentTask.id).length > 0) {
        return { type: errorType.DUPPLICATE_CODE, taskIndex }
    }
}
const validateUnitCode = (code, currentTask, taskIndex) => {
    const units = gantt.serverList("unit")
    if (!_.map(units, 'key').includes(code)) {
        return { type: errorType.WRONG_UNIT_CODE, taskIndex }
    }
}
const validatePrice = (price, currentTask, taskIndex) => {
    if (price < 0) {
        return { type: errorType.PRICE_BELOW_ZERO, taskIndex }
    } else if (isNaN(price)) {
        return { type: errorType.WRONG_FORMAT_NUMBER, taskIndex }
    }
}
const validateStartDate = (startDate, currentTask, taskIndex, updateObj) => {
    if (!moment(startDate, "DD/MM/YYYY", true).isValid()) {
        return { type: errorType.WRONG_FORMAT_DATE, taskIndex }
    } else {
        const { end_date: endDateUpdateObj } = updateObj || {}
        if (!endDateUpdateObj) {
            const { end_date: endDateCurrentObj } = currentTask || {}
            if (moment(endDateCurrentObj).isBefore(moment(startDate, "DD/MM/YYYY", true))) {
                return { type: errorType.START_DATE_GREATER_END_DATE, taskIndex }
            }
        }
    }
}
const validateEndDate = (endDate, currentTask, taskIndex, updateObj) => {
    if (!moment(endDate, "DD/MM/YYYY", true).isValid()) {
        return { type: errorType.WRONG_FORMAT_DATE, taskIndex }
    } else {
        const { start_date: startDateUpdateObj } = updateObj || {}
        if (!startDateUpdateObj) {
            const { start_date: startDateCurrentObj } = currentTask || {}
            if (moment(moment(endDate, "DD/MM/YYYY", true)).isBefore(startDateCurrentObj)) {
                return { type: errorType.START_DATE_GREATER_END_DATE, taskIndex }
            }
        } else {
            if (moment(endDate, "DD/MM/YYYY", true).isBefore(moment(startDateUpdateObj, "DD/MM/YYYY", true))) {
                return { type: errorType.START_DATE_GREATER_END_DATE, taskIndex }
            }
        }
    }
}
const validateQuantity = (quantity, currentTask, taskIndex) => {
    if (isNaN(quantity)) {
        return { type: errorType.WRONG_FORMAT_NUMBER, taskIndex }
    }
}

const errorType = {
    REQUIRED_FIELD: "REQUIRED_FIELD",
    WRONG_FORMAT_DATE: "WRONG_FORMAT_DATE",
    WRONG_BOQ_LINK: "WRONG_BOQ_LINK",
    WRONG_FORMAT_NUMBER: "WRONG_FORMAT_NUMBER",
    WRONG_UNIT_CODE: "WRONG_UNIT_CODE",
    WRONG_STATUS: "WRONG_STATUS",
    DUPPLICATE_CODE: "DUPPLICATE_CODE",
    PRICE_BELOW_ZERO: "PRICE_BELOW_ZERO",
    START_DATE_GREATER_END_DATE: "START_DATE_GREATER_END_DATE",
}

const getMessageError = (type) => {
    switch (type) {
        case errorType.DUPPLICATE_CODE:
            return 'Mã hạng mục đã tồn tại'
        case errorType.PRICE_BELOW_ZERO:
            return 'Đơn giá không được nhỏ hơn 0'
        case errorType.START_DATE_GREATER_END_DATE:
            return 'Ngày bắt đầu lớn hơn ngày kết thúc'
        case errorType.WRONG_FORMAT_DATE:
            return 'Định dạng ngày không đúng (định dạng ngày dd/MM/yyyy)'
        case errorType.WRONG_FORMAT_NUMBER:
            return 'Định dạng số không đúng (số hợp lệ có dạng 123,456.789)'
        case errorType.WRONG_UNIT_CODE:
            return 'Mã đơn vị tính không đúng'
        case errorType.REQUIRED_FIELD:
            return 'Thiếu giá trị trường bắt buộc (Tên hạng mục, ngày bắt đầu, ngày kết thúc, trạng thái)'
        case errorType.WRONG_STATUS:
            return 'Trạng thái hạng mục không hợp lệ. Trạng thái phải là một trong những giá trị Chưa bắt đầu, Đang thực hiện, Tạm dừng, Hoàn thành'
        case errorType.WRONG_BOQ_LINK:
            return 'Không tìm thấy mã BOQ'
        default:
            return ''
    }
}