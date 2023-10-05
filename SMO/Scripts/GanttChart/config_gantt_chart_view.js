function colContent(task) {
    const activity = `<li><a href="#" onclick="clickGridButton('${task.id}', 'ACTIVITY')">ACTIVITY</a></li>`
    const edit = `<i class="fa gantt_button_grid gantt_grid_edit fa-edit p-r-5" onclick="gantt.showLightbox('${task.id}')"></i>`
    return edit;
};

$('#gantt-chart li.dropdown').hover(function () {
    $(this).addClass('open');
},
    function () {
        $(this).removeClass('open');
    });
function toggleCreateTask(e, action) {
    const el = $(e).next('.dropdown').find('ul.dropdown-menu');
    el.toggle();
}
function clickGridButton(id, type) {
    gantt.selectedType = type;
    gantt.createTask(null, id);
}
gantt.config.readonly = true;
gantt.config.undo = false;
// date format https://docs.dhtmlx.com/gantt/desktop__date_format.html
// specifying the date format
gantt.config.date_grid = "%d/%m/%Y";
gantt.config.date_format = "%Y-%m-%d";

const formatFunc = gantt.date.date_to_str("%d/%m/%Y");
const cfg = gantt.config;
const strToDate = gantt.date.str_to_date(cfg.date_format, cfg.server_utc);
// config toolstip
gantt.templates.grid_date_format = function (date, _) {
    return formatFunc(date);
};
gantt.templates.tooltip_date_format = function (date) {
    return formatFunc(date);
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

var textEditor = { type: "text", map_to: "text" };

var dateEditor = gantt.config.editor_types.date;
var dateStartEditor = { type: "date", map_to: "start_date" };
var dateFinishEditor = { type: "date", map_to: "end_date" };

gantt.templates.grid_row_class = function (start_date, end_date, item) {
    if (item.type == "BOQ") return "bg-gantt-green";
};
gantt.templates.task_row_class = function (start_date, end_date, item) {
    if (item.type == "BOQ") return "bg-gantt-green";
};

gantt.config.open_tree_initially = true;

gantt.config.reorder_grid_columns = true;
gantt.config.columns = [
    { name: "wbs", label: "Mã", width: 40, template: gantt.getWBSCode, resize: true },
    { name: "text", label: "Hạng mục", tree: true, width: '*', min_width: 200, resize: true, editor: textEditor },
    { name: "start_date", align: "center", label: "Bắt đầu", resize: true, editor: dateStartEditor },
    { name: "end_date", align: "center", label: "Kết thúc", resize: true, editor: dateFinishEditor },
    { name: "type", label: "Loại", resize: true },
    {
        name: "buttons",
        align: "center",
        label: "#",
        template: colContent,
        resize: true
    }
];

gantt.config.layout = {
    css: "gantt_container",
    cols: [
        {
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
            ]
        },
        { view: "scrollbar", id: "scrollVer" }
    ]
};


gantt.attachEvent("onBeforeLightbox", function (id) {
    if (!isNaN(id)) {
        // if not a number id
        // create new task
        const task = gantt.getTask(id);
        task.type = gantt.selectedType;
        task.projectId = gantt.projectId;
        const cloneTask = task;
        task.start_date = moment(task.start_date).format('YYYY-MM-DDTHH:mm:ss');
        task.end_date = moment(task.end_date).format('YYYY-MM-DDTHH:mm:ss');

        response = gantt.ajax.post(
            'api/task/CreateTask',
            task
        ).then((response) => {
            var res = JSON.parse(response.responseText);
                    if (res && res.status === 'success') {
                        // response is ok
                        cloneTask.start_date = moment(task.start_date).toDate()
                        cloneTask.end_date = moment(task.end_date).toDate()
                        gantt.updateTask(id, cloneTask);
                        gantt.changeTaskId(id, res.tid);

                        if (res.executeMethod) {
                            Message.func.ExecuteFunctionByName(res.executeMethod, window);
                        }
                    } else if (res && res.status === 'error') {
                        showMessage('error', 'Something went wrong! ' + res.message);
                        gantt.deleteTask(id);
                    } else {
                        showMessage('error', 'Something went wrong!');
                        gantt.deleteTask(id);
                    }
                    return false;
        });
        
    }
    // if not a number id
    // show detail task
    return isNaN(id);
});

gantt.attachEvent("onTaskDblClick", function (id, e) {
    calculateGanttDetail();
    //if (!this.callEvent("onBeforeLightbox", [id])) return;
    Forms.LoadGanttAjaxRight(`PS/ProjectStruct/Edit?id=${id}`);
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
gantt.attachEvent("onGanttReady", function () {
    gantt.templates.tooltip_text = function (start, end, task) {
        switch (task.type) {
            case "PROJECT": {
                return "<b>Type:</b> " + "Project" + "<br/>" +
                    "<b>Hạng mục:</b> " + task.text + "<br/>" +
                    "<b>Bắt đầu:</b> " +
                    gantt.templates.tooltip_date_format(start) +
                    "<br/><b>Kết thúc:</b> " + gantt.templates.tooltip_date_format(end) + "<br/>" +
                    "<b>Deployment day: </b>" + displayTooltipValue(task.deployment_day) + "<br/>" +
                    "<b>Progress: </b>" + parseFloat(task.progress) * 100 + "%" + "<br/>" +
                    "<b>Unit: </b>" + displayTooltipValue(task.unit) + "<br/>";
            }
            case "ACTIVITY": {
                return "<b>Hạng mục:</b> " + "Activity" + "<br/>" +
                    "<b>Task:</b> " + task.text + "<br/>" +
                    "<b>Bắt đầu:</b> " +
                    gantt.templates.tooltip_date_format(start) +
                    "<br/><b>Kết thúc:</b> " + gantt.templates.tooltip_date_format(end) + "<br/>" +
                    "<b>Plan volume: </b>" + displayTooltipValue(task.plan_volume) + "<br/>" +
                    "<b>Actual volume: </b>" + displayTooltipValue(task.actual_volume) + "<br/>" +
                    "<b>Progress: </b>" + parseFloat(task.progress) * 100 + "%" + "<br/>" +
                    "<b>Unit: </b>" + displayTooltipValue(task.unit) + "<br/>";
            }
            case "WBS": {
                return "<b>Hạng mục:</b> " + "WBS" + "<br/>" +
                    "<b>Task:</b> " + task.text + "<br/>" +
                    "<b>Bắt đầu:</b> " +
                    gantt.templates.tooltip_date_format(start) +
                    "<br/><b>Kết thúc:</b> " + gantt.templates.tooltip_date_format(end) + "<br/>" +
                    "<b>Plan volume: </b>" + displayTooltipValue(task.plan_volume) + "<br/>" +
                    "<b>Actual volume: </b>" + displayTooltipValue(task.actual_volume) + "<br/>" +
                    "<b>Weight: </b>" + displayTooltipValue(task.weight) + "<br/>" +
                    "<b>Progress: </b>" + parseFloat(task.progress) * 100 + "%" + "<br/>" +
                    "<b>Unit: </b>" + displayTooltipValue(task.unit) + "<br/>";
            }
            default:
                return "<b>Hạng mục:</b> " + task.text + "<br/>" +
                    "<b>Bắt đầu:</b> " +
                    gantt.templates.tooltip_date_format(start) +
                    "<br/><b>Kết thúc:</b> " + gantt.templates.tooltip_date_format(end);
        }
    };
});

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
    task.start_date = moment(getForm().querySelector("[name='startDate']").value, "DD/MM/YYYY").toDate();
    task.end_date = moment(getForm().querySelector("[name='endDate']").value, "DD/MM/YYYY").toDate();
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
        //debugger;
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
        var startDate = moment(value.querySelector("[name=task-list-start-date-hidden]").value, "DD/MM/YYYY").toDate();
        var endDate = moment(value.querySelector("[name=task-list-end-date-hidden]").value, "DD/MM/YYYY").toDate();
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
                //debugger;
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