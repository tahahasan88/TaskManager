﻿

$(document).ready(function () {

    var disabled = false;
    $(".knob").knob({});
    //alert(taskId);

function setFormState() {

    //Check delete permissions
    $.ajax({
        type: "GET",
        url: thisBaseUrl + "/taskemployees/GetUserPermissions",
        contentType: "application/json; charset=utf-8",
        data: { taskId: taskId, userName: currentUserName },
        datatype: "json",
        success: function (data) {
            tmpData = data;
            console.log(tmpData);
            if (tmpData.isTaskDeletionAllowed == true) {
                //disabled = true;
                $("#deleteTaskBtnId").removeClass();
                $("#deleteTaskBtnId").addClass('btn btn-block btn-danger btn-sm');

                $("#deleteTaskBtnId").click(function () {
                    var result = confirm("Are you sure you want to delete this task?");
                    if (result) {
                        $.ajax({
                            type: "POST",
                            url: thisBaseUrl + "/tasks/DeleteTask?id=" + taskId,
                            contentType: "application/json; charset=utf-8",
                            data: null,
                            datatype: "json",
                            success: function (data) {
                                if (data.isDeleted == true) {
                                    location.href = thisBaseUrl + "/tasks/DeleteRedirect?id=" + taskId;
                                }
                            },
                            error: function () {
                                alert("Dynamic content load failed.");
                            }
                        });
                    }
                });
            }
            else {
                $("#deleteTaskBtnId").removeClass();
                $("#deleteTaskBtnId").addClass('btn btn-block btn-secondary btn-sm');
            }

            if (tmpData.isTaskEditAllowed == true) {
                $("#updateTaskBtnId").removeClass();
                $("#updateTaskBtnId").addClass('btn  btn-primary btn-sm');

                $("#updateTaskBtnId").click(function () {
                    var $buttonClicked = $(this);
                    //var id = $buttonClicked.attr('data-id');
                    var options = { "backdrop": "static", keyboard: true };
                    $.ajax({
                        type: "GET",
                        url: thisBaseUrl + "/tasks/UpdateTaskProgress",
                        contentType: "application/json; charset=utf-8",
                        data: { id: taskId },
                        datatype: "json",
                        success: function (data) {
                            progressUpdatePlaceHolder.html(data);
                            progressUpdatePlaceHolder.find('.modal').modal(options);
                            progressUpdatePlaceHolder.find('.modal').modal('show');
                            if ($("#taskProgressInput").val() > 0 && $("#taskProgressInput").val() < 100) {
                                $("#Status").val("2");
                            }
                            else if ($("taskProgressInput").val() == 0 || $("taskProgressInput").val() == null) {
                                $("#Status").val("1");
                            }
                            else
                                $("#Status").val("3");

                            $("#taskProgressInput").change(function () {
                                if ($(this).val() > 0 && $(this).val() < 100) {
                                    $("#Status").val("2");
                                }
                                else if ($(this).val() == 0) {
                                    $("#Status").val("1");
                                }
                                else
                                    $("#Status").val("3");
                            });

                        },
                        error: function () {
                            alert("Dynamic content load failed.");
                        }
                    });
                });
            }
            else {
                $("#updateTaskBtnId").removeClass();
                $("#updateTaskBtnId").addClass('btn  btn-secondary btn-sm');
            }
            if (tmpData.isSubTaskEditAllowed == true) {
                disabled = false;
            }
            else {
                disabled = true;
            }
            getSubTasksList();
        },
        error: function () {
            alert("Dynamic content load failed.");
        }
    });


}


function setBadgeStyle() {

    $("#taskStatusBadge").removeClass();
    if (taskStatus == 'Completed') {

        $("#taskStatusBadge").addClass('badge badge-success');
    }
    else if (taskStatus == 'In Progress') {
        $("#taskStatusBadge").addClass('badge badge-warning');
    }
    else if (taskStatus == 'Not Started') {
        $("#taskStatusBadge").addClass('badge badge-secondary');
    }
    else if (taskStatus == 'On Hold') {
        $("#taskStatusBadge").addClass('badge badge-secondary');
    }
    else if (taskStatus == 'Cancelled') {
        $("#taskStatusBadge").addClass('badge badge-secondary');
    }
}


var table = $('#historyTable').DataTable({
    columns: [
        { data: "Date" },
        { data: "Type" },
        { data: "Description" },
        { data: "Action By" }
    ]
});

var progressUpdatePlaceHolder = $("#modal-update-placeholder");
var subTaskPlaceHolder = $("#subTaskPlaceHolder");
var historyList = [];
var historyTypeList = [];
var employeeList = [];
var allEmployeeList = [];

function initializeHistoryTable() {
    historyTypeList = [];
    $.each($("input[class='historyType']:checked"), function () {
        historyTypeList.push($(this).val());
    });

    updateHistoryTable(historyTypeList);
}


$('.historyType').click(function () {
    historyTypeList = [];
    $.each($("input[class='historyType']:checked"), function () {
        historyTypeList.push($(this).val());
    });

    updateHistoryTable(historyTypeList);
});



progressUpdatePlaceHolder.on('click', '[data-save="modal"]', function (event) {
    event.preventDefault();

    var form = $(this).parents('.modal').find('form');
    var actionUrl = form.attr('action');
    $("#TaskProgress").val($("#taskProgressId").val());

    var dataToSend = form.serialize();
    $("#progressSpinner").show();
    var status = $("#Status").val();
    var taskProgress = $("#TaskProgress").val();
    var remarks = $("#Remarks").val();

    $("#taskStatusBadge").removeClass();
    if (status == '1') {
        $("#taskStatusBadge").html("Not Started");
        $("#taskStatusBadge").addClass('badge badge-secondary');
        //$("#taskStatusBadge").val("In");
    }
    else if (status == '2') {
        $("#taskStatusBadge").html("In Progress");
        $("#taskStatusBadge").addClass('badge badge-warning');
    }
    else if (status == '3') {
        $("#taskStatusBadge").html("Completed");
        $("#taskStatusBadge").addClass('badge badge-success');
    }
    $("#taskProgressUpdateId").val(taskProgress + '%');
    $("#taskRemarks").html(remarks);

    $.post(actionUrl, dataToSend).done(function (data) {
        var newBody = $('.modal-body', data);
        $("#progressSpinner").hide();
        console.log('here');
        progressUpdatePlaceHolder.find('.modal-body').replaceWith(newBody);
        var isValid = newBody.find('[name="IsValid"]').val() == 'True';
        if (isValid) {
            getHistoryList();
            setTimeout(
                function () {
                    $("input.knob").trigger('change');
                }, 100);
            progressUpdatePlaceHolder.find('.modal').modal('hide');
        }
    });
});


function getHistoryList() {

    historyList = [];
    $.ajax({
        type: "GET",
        url: thisBaseUrl + "/history/list?taskId=" + taskId,
        contentType: "application/json; charset=utf-8",
        data: null,
        datatype: "json",
        success: function (data) {
            $.each(data, function (i, val) {
                tmpData = data[i];
                $.each(tmpData, function (i, val) {
                    historyList.push(tmpData[i]);
                });
            });
            initializeHistoryTable();
        },
        error: function () {
            alert("Dynamic content load failed.");
        }
    });
}

function getSubTasksList() {

    var disableThis = "";
    if (disabled == true) {
        disableThis = "disabled"
    }
    var selectedDrownHtml = "<select name=\"employeeDrpDwn\" class=\"form-control\" id=\"empdropdownId\">setoptions</select>";
    var allEmployees = [];

    var subTaskBody = "<div class=\"row\">" +
        "<div class=\"col-5 pl-0 d-flex align-items-center\">" +
        "<input class=\"mr-1\" type=\"checkbox\" id=\"checkboxtaskid\" ischecked name=\"completecheck\" " + disableThis + ">" +
        "<input name = \"subTaskInput\" id=\"inputtaskid\" value=\"tasktitle\" " + disableThis + " class=\"w-100\" />" +
        "</div>" +
        "<div class=\"col-6 d-flex align-items-center\">" +
        "<i class=\"far fa-user-circle mr-1\" style=\"font-size:24px\" aria-hidden=\"true\">" +
        "</i>" +
        "<a " + disableThis + " href=\"javascript:void(null);\" name=\"employeeLink\" rel=\"nofollow noreferrer\"><u>linkUserName</u></a>" +
        "<div class=\"form-group mb-0 w-100\" name=\"employeeDrpDwndiv\" style=\"display:none;\">" +
        "employeeOptions" +
        "</div>" +
        "</div>" +
        "<div class=\"col-1\">" +
        "<i name=\"delete\" id=\"deletetaskid\"  " + disableThis + " class=\"fas fa-trash-alt\"></i>" +
        "</div>" +
        "</div>";

    var addSubTaskButtonHtml = "<div class=\"row\">" +
        "<div class=\"input-group mb-2\">" +
        "<input type=\"text\" id=\"newTaskInputId\" class=\"form-control\">" +
        "<span class=\"input-group-append\">" +
        "<button " + disableThis + " id=\"addSubtaskBtnId\" type=\"button\" class=\"btn btn-info btn-flat\">Add Sub Task</button>" +
        "</span>" +
        "</div>" +
        "</div>";

    var newBody = "";
    $.ajax({
        type: "GET",
        url: thisBaseUrl + "/subtasks/SubTaskList",
        contentType: "application/json; charset=utf-8",
        data: { id: taskId },
        datatype: "json",
        success: function (data) {

            $.each(data, function (i, val) {
                tmpData = data[i];

                $.each(tmpData.employeeVMList, function (j, value) {
                    allEmployees.push(tmpData.employeeVMList[j]);
                });

                $.each(tmpData.subTaskVMList, function (i, val) {
                    newBody += subTaskBody;
                    var isSubTaskCompleted = "";
                    if (tmpData.subTaskVMList[i].isCompleted == true) {
                        isSubTaskCompleted = "checked";
                    }
                    newBody = newBody.replace("checkboxtaskid", tmpData.subTaskVMList[i].subTaskId);
                    newBody = newBody.replace("inputtaskid", tmpData.subTaskVMList[i].subTaskId);
                    newBody = newBody.replace("deletetaskid", tmpData.subTaskVMList[i].subTaskId);
                    newBody = newBody.replace("tasktitle", tmpData.subTaskVMList[i].description);
                    newBody = newBody.replace("linkUserName", tmpData.subTaskVMList[i].subTaskAssignee);
                    newBody = newBody.replace("ischecked", isSubTaskCompleted);
                    var dropdown = "";
                    for (var j = 0; j < allEmployees.length; j++) {
                        if (tmpData.subTaskVMList[i].subTaskAssignee == allEmployees[j].userName) {
                            dropdown += "<option selected>" + allEmployees[j].userName + "</option>";
                        }
                        else {
                            dropdown += "<option>" + allEmployees[j].userName + "</option>";
                        }

                    }
                    var thisdropDownHtml = selectedDrownHtml;
                    thisdropDownHtml = thisdropDownHtml.replace("setoptions", dropdown);
                    newBody = newBody.replace("employeeOptions", thisdropDownHtml);
                    newBody = newBody.replace("empdropdownId", tmpData.subTaskVMList[i].subTaskId);
                    newBody += "<br/>"
                });

            });
            if (newBody.length > 0) {

                newBody += addSubTaskButtonHtml;
                subTaskPlaceHolder.html(newBody);

                if (disabled == false) {
                    $('i[name ="delete"]').click(function () {
                        var result = confirm("Are you sure you want to delete this subtask?");
                        if (result) {
                            $.ajax({
                                type: "POST",
                                url: thisBaseUrl + "/subtasks/DeleteSubTask?subTaskId=" + $(this).attr('id'),
                                contentType: "application/json; charset=utf-8",
                                data: null,
                                datatype: "json",
                                success: function (data) {
                                    subTaskPlaceHolder.html('');
                                    getSubTasksList();
                                    appendEmployeeList();
                                },
                                error: function () {
                                    alert("Dynamic content load failed.");
                                }
                            });
                        }
                    });
                }

                $('input[name ="completecheck"]').click(function () {

                    var description = $(this).closest('.row').find('input[name ="subTaskInput"]').val();
                    var isCompleted = $(this).is(':checked');
                    var subTaskId = $(this).attr('id');
                    var assignee = $(this).closest('.row').find('[name="subTaskassignee"]').html()
                    var taskId = taskId;
                    updateSubTask(subTaskId, taskId, description, assignee, isCompleted);
                });
                if (disabled == false) {
                    $('input[name ="subTaskInput"]').change(function () {
                        var description = $(this).closest('.row').find('input[name ="subTaskInput"]').val();
                        var isCompleted = $(this).is(':checked');
                        var subTaskId = $(this).attr('id');
                        var assignee = $(this).closest('.row').find('[name="subTaskassignee"]').html()
                        var taskId = taskId;
                        updateSubTask(subTaskId, taskId, description, assignee, isCompleted);
                    });
                }
            }
            else {
                newBody += addSubTaskButtonHtml;
                subTaskPlaceHolder.html(newBody);
            }

            $("#addSubtaskBtnId").click(function () {
                //var taskId = taskId;
                var description = $("#newTaskInputId").val();
                var isCompleted = false;
                var assignee = "";
                if (description == "") {
                    alert("please enter description");
                }
                else
                    addSubTask(taskId, description, isCompleted, assignee);
            });

            $("select[name='employeeDrpDwn']").focusout(function () {
                var end = this.value;
                $(this).closest('.row').find('[name="employeeDrpDwndiv"]').hide();
                $(this).closest('.row').find('[name="employeeLink"]').html(end);
                $(this).closest('.row').find('[name="employeeLink"]').show();
            });

            $("select[name='employeeDrpDwn']").change(function () {
                var end = this.value;
                $(this).closest('.row').find('[name="employeeDrpDwndiv"]').hide();
                $(this).closest('.row').find('[name="employeeLink"]').html(end);
                $(this).closest('.row').find('[name="employeeLink"]').show();

                var description = $(this).closest('.row').find('input[name ="subTaskInput"]').val();
                var isCompleted = $(this).is(':checked');
                var subTaskId = $(this).attr('id');
                var assignee = end;
                var taskId = taskId;
                updateSubTask(subTaskId, taskId, description, assignee, isCompleted);

            });

            if (disabled == false) {
                $("a[name='employeeLink']").click(function () {
                    $(this).hide();
                    $(this).closest('.row').find('[name="employeeDrpDwndiv"]').show();
                });
            }

        },
        error: function () {
            alert("Dynamic content load failed.");
        }
    });
}

function updateSubTask(subtaskId, taskId, description, assignee, isCompleted) {

    $.post(thisBaseUrl + "/subtasks/Update",
        { id: subtaskId, description: description, taskId: taskId, assignee: assignee, isCompleted: isCompleted },
        function (returnedData) {
            if (returnedData.subTaskExist == true) {
                alert('sub task already exists');
            }
            else if (returnedData.subTaskUpdated == true) {
                getHistoryList();
                appendEmployeeList();
                alert("sub task updated");
            }
        }).fail(function () {
            console.log("error");
        });
}

function addSubTask(taskId, description, isCompleted, assignee) {

    $.post(thisBaseUrl + "/subtasks/Create",
        { description: description, taskId: taskId, isCompleted: isCompleted, assignee: assignee },
        function (returnedData) {
            if (returnedData.subTaskExist == true) {
                alert('sub task with same title already exists');
            }
            else if (returnedData.subTaskAdded == true) {
                getHistoryList();
                alert("sub task added");
                getSubTasksList();
            }

        }).fail(function () {
            console.log("error");
        });

}

function getAllEmployees() {

    $.ajax({
        type: "GET",
        url: thisBaseUrl + "/TaskEmployees/AllEmployees",
        contentType: "application/json; charset=utf-8",
        data: null,
        datatype: "json",
        success: function (data) {
            $.each(data, function (i, val) {
                tmpData = data[i];
                $.each(tmpData, function (i, val) {
                    allEmployeeList.push(tmpData[i]);
                });
            });
        },
        error: function () {
            alert("Dynamic content load failed.");
        }
    });
}

function getTaskDescription(taskCapacityId) {

    description = "";
    if (taskCapacityId == taskCapacityCreator) {
        description = "Creator";
    }
    else if (taskCapacityId == taskCapacityAssignee) {
        description = "Assignee";
    }
    else if (taskCapacityId == taskCapacityFollower) {
        description = "Follower";
    }
    else if (taskCapacityId == taskCapacityExTaskAssignee) {
        description = "Ex-Task Assignee";
    }
    else if (taskCapacityId == taskCapacitySubTaskAssignee) {
        description = "Sub Task Assignee";
    }
    return description;
}

function appendEmployeeList() {

    var cardBody = "<div class=\"d-flex justify-content-between\">" +
        "<div class=\"d-flex align-items-center\">" +
        "<i class=\"far fa-user-circle\" style=\"font-size:24px\" aria-hidden=\"true\">" +
        "</i>&nbsp;userName&nbsp" +
        "<i class=\"fas fa-wifi\" aria-hidden=\"true\"></i>" +
        "</div>" +
        "<span class=\"float-right text-md\">capacity</span>" +
        "</div>";

    var employeeHtmlBody = "";
    $.ajax({
        type: "GET",
        url: thisBaseUrl + "/TaskEmployees/list?taskId=" + taskId,
        contentType: "application/json; charset=utf-8",
        data: null,
        datatype: "json",
        success: function (data) {
            $.each(data, function (i, val) {
                tmpData = data[i];
                $.each(tmpData, function (i, val) {
                    employeeHtmlBody += cardBody;
                    employeeList.push(tmpData[i]);
                    employeeHtmlBody = employeeHtmlBody.replace("userName", tmpData[i].userName);
                    employeeHtmlBody = employeeHtmlBody.replace("capacity", getTaskDescription(tmpData[i].capacityId));

                });
                if (i < data.length - 1) {
                    employeeHtmlBody += "</br></br>";
                }
            });
            $("#people-placeholder").html('');
            $("#people-placeholder").html(employeeHtmlBody);
        },
        error: function () {
            alert("Dynamic content load failed.");
        }
    });
}

function getHistoryType(historyType) {
    var historyTypeId = 0;
    if (historyType == "progress") {
        historyTypeId = progressAuditType;
    }
    else if (historyType == "assignment") {
        historyTypeId = assignmentAuditType;
    }
    if (historyType == "followup") {
        historyTypeId = followUpAuditType;
    }
    else if (historyType == "followupresponse") {
        historyTypeId = followUpResponseAuditType;
    }
    else if (historyType == "subtasks") {
        historyTypeId = subTaskAuditType;
    }
    return historyTypeId;
}

function getHistoryTypeDescription(historyTypeId) {
    var historyType = "";
    if (historyTypeId == progressAuditType) {
        historyType = "Progress";
    }
    else if (historyTypeId == assignmentAuditType) {
        historyType = "Assignment";
    }
    if (historyTypeId == followUpAuditType) {
        historyType = "Follow up";
    }
    else if (historyTypeId == followUpResponseAuditType) {
        historyType = "follow up response";
    }
    else if (historyTypeId == subTaskAuditType) {
        historyType = "Sub tasks";
    }
    return historyType;
}


function updateHistoryTable(typeList) {

    table.rows().remove().draw();
    var filteredHistory = [];
    for (var i = 0; i < typeList.length; i++) {
        var thisHistory = []
        thisHistory = historyList.filter(function (history) {
            return history.type == getHistoryType(typeList[i]);
        });
        filteredHistory = filteredHistory.concat(thisHistory);
    }

    for (var i = 0; i < filteredHistory.length; i++) {
        table.rows.add([{
            "Date": filteredHistory[i].historyDate,
            "Type": getHistoryTypeDescription(filteredHistory[i].type),
            "Description": filteredHistory[i].description,
            "Action By": filteredHistory[i].actionBy
        }]).draw();
    }
}

setFormState();
setBadgeStyle();
appendEmployeeList();
getHistoryList();
});