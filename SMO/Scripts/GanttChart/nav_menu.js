if(!window.ganttModules){
	window.ganttModules = {};
}

ganttModules.menu = (function(){
	function addClass(node, className){
		node.className += " " + className;
	}

	function removeClass(node, className){
		node.className = node.className.replace(new RegExp(" *" + className.replace(/\-/g, "\\-"), "g"), "");
	}

	function getButton(name){
		return document.querySelector(".gantt-controls [data-action='"+name+"']");
	}

	function disableButton(name){
		addClass(getButton(name), "menu-item-disabled");
	}

	function enableButton(name){
		removeClass(getButton(name), "menu-item-disabled");
	}

	function refreshZoomBtns(){
		var zoom = ganttModules.zoom;
		if(zoom.canZoomIn()){
			enableButton("zoomIn");
		}else{
			disableButton("zoomIn");
		}
		if(zoom.canZoomOut()){
			enableButton("zoomOut");
		}else{
			disableButton("zoomOut");
		}
	}

	const menu = {
		zoomIn: function(){
			var zoom = ganttModules.zoom;
			zoom.zoomIn();
			refreshZoomBtns();
		},
		zoomOut: function(){
			ganttModules.zoom.zoomOut();
			refreshZoomBtns();
		},
		fullscreen: function () {
			gantt.expand();
			
		},
		collapseAll: function(){
			gantt.eachTask(function(task){
				task.$open = false;
			});
			gantt.render();

		},
		expandAll: function(){
			gantt.eachTask(function(task){
				task.$open = true;
            });

			gantt.render();
		},
		undo: function () {
			gantt.undo()
		},
		redo: function () {
			gantt.redo()
		},
		changeStatus: function () {
			const selectedTasks = gantt.getTaskBy("selected", true) || [];
			if (selectedTasks.length === 0) {
				return alert("Phải chọn ít nhất 1 hạng mục công việc để thay đổi trạng thái")
            }
			Swal.fire({
				title: 'Thay đổi trạng thái công việc?',
				text: `Chọn trạng thái để thay đổi trạng thái của ${selectedTasks.length} hạng mục công việc`,
				icon: 'warning',
				input: 'select',
				inputOptions: {
					'01': 'Chưa bắt đầu',
					'02': 'Đang thực hiện',
					'03': 'Tạm dừng',
					'04': 'Hoàn thành',
                },
				inputPlaceholder: 'Chọn trạng thái',
				inputValidator: (value) => {
					return new Promise((resolve) => {
						if (value) {
							resolve()
						} else {
							resolve('Trạng thái không được để trống.')
						}
					})
				},
				showCancelButton: true,
				confirmButtonColor: '#3085d6',
				cancelButtonColor: '#d33',
				confirmButtonText: 'Thay đổi',
				cancelButtonText: 'Hủy'
			}).then((result) => {
				if (result.isConfirmed) {
					const status = result.value
					Forms.ShowLoading();
					const ajaxParams = {
						url: `PS/ProjectStruct/UpdateStatuses`,
						type: "POST",
						data: {
							structuresId: selectedTasks.map(x => x.id),
							status,
							projectId: selectedTasks[0].projectId
						},
						dataType: 'json',
						success: function (response) {
							Message.execute(response);
							const { State } = response
							if (State) {
								gantt.silent(() => {
									$.each(selectedTasks, (_index, task) => {
										task.selected = false;
										task.status = status;
										const { id } = task;
										gantt.updateTask(id);
									})
								});
								gantt.render();
                            }
						}
					};
					Forms.Ajax(ajaxParams)
				}
			})
		},
		importFile: function (projectId, type, disabled) {
			if (disabled === "True") {
				return alert("\u004b\u0068\u00f4\u006e\u0067 \u0074\u0068\u1ec3 \u0049\u006d\u0070\u006f\u0072\u0074 \u0064\u1eef \u006c\u0069\u1ec7\u0075 \u006b\u0068\u0069 \u0064\u1ef1 \u00e1\u006e \u0111\u00e3 \u1edf \u0074\u0072\u1ea1\u006e\u0067 \u0074\u0068\u00e1\u0069 \u0050\u0068\u00ea \u0064\u0075\u0079\u1ec7\u0074\u002e")
            }
			Forms.LoadAjaxModal(`PS/ProjectStruct/ImportMsProject?projectId=${projectId}&type=${type}`);
        }
	};


	return {
		setup: function(){
			var navBar = document.querySelector(".gantt-controls");
			gantt.event(navBar, "click", function(e){
				var target = e.target || e.srcElement;
				while(!target.hasAttribute("data-action") && target !== document.body){
					target = target.parentNode;
				}

				if(target && target.hasAttribute("data-action")){
					var action = target.getAttribute("data-action");
					var projectId = target.getAttribute("data-project-id");
					var structureType = target.getAttribute("data-structure-type");
					var disabled = target.getAttribute("data-disabled");
					if(menu[action]){
						menu[action](projectId, structureType, disabled);
					}
				}
			});

            this.setup = function () { };
		}
	};
})(gantt);