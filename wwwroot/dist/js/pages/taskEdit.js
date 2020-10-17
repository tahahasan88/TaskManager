

$(document).ready(function () {

    var disabled = false;
    $(".knob").knob({});
    //alert(taskId);
    var assigneePlaceHolder = $("#assignee-placeholder");
    var employeeDiv = $('#employeeDetailDiv');
    var taskEditPlaceHolder = $('#modal-default');
    if (isReadOnlyMode == "True") {
        $("#editBtnDiv").hide();
        $("#deleteBtnDiv").hide();
        $("#updateTaskBtnDiv").hide();
        disabled = true;
    }

    $('#Quality').slider('disable');

    function showEmployeeDetails(userName) {

        $.ajax({
            type: "GET",
            url: thisBaseUrl + "/employeeDetail/Index",
            contentType: "application/json; charset=utf-8",
            data: { userName: userName },
            datatype: "json",
            success: function (data) {
                employeeDiv.html(data);
                employeeDiv.find('.modal').modal('show');
                employeeDiv.show();

                var employeeTasksTable = $('#employeeTasksTable').
                    DataTable({
                        paging: false,
                        info: false,
                        searching: false,
                        //scrollY:        300,
                        //scrollX:        true,
                        //scrollCollapse: true,
                        initComplete: function (settings, json) {
                            var response = settings.json;
                            if (response.pendingCount == 0) {
                                $("#taskCount").hide();
                            }
                            $("#taskCount").html(response.pendingCount);

                            var taskDonutData = {
                                labels: [
                                    'Completed',
                                    'Not Started Yet',
                                    'In Progress',
                                    'On Hold',
                                    'Cancelled'
                                ],
                                datasets: [
                                    {
                                        data: [response.completedCount, response.notStartedCount,
                                        response.progressCount, response.onHoldCount, response.cancelledCount],
                                        backgroundColor: ['#00a65a', '#6c757d', '#f39c12', '#6f42c1', '#343a40'],
                                    }
                                ]
                            }

                            var pieChartCanvas = $('#pieChart').get(0).getContext('2d');
                            var pieData = taskDonutData;
                            var pieOptions = {
                                maintainAspectRatio: false,
                                responsive: true,
                                legend: {
                                    display: false
                                }
                            }
                            //Create pie or douhnut chart
                            // You can switch between pie and douhnut using the method below.
                            var tasksPieChart = new Chart(pieChartCanvas, {
                                type: 'pie',
                                data: pieData,
                                options: pieOptions
                            });

                        },
                        search: { regex: true, caseInsensitive: true },
                        ajax: {
                            "url": thisBaseUrl + "/employeeDetail/LoadEmployeeTasksData",
                            "type": "POST",
                            "datatype": "json",
                            "data": { userName: userName }
                        },
                        columns: [
                            {
                                "render": function (data, type, full, meta) {
                                    return '<a href="' + thisBaseUrl + '/tasks/Edit?id=' + full.taskId + '&username=' + currentUserName + '&viewMode=1"><u>' + full.title + '</u> </a>';
                                }
                            },
                            {
                                "render": function (data, type, full, meta) {
                                    if (full.status == "Not Started") {
                                        return '<span class="badge badge-secondary">Not Started</span>';
                                    }
                                    else
                                        if (full.status == "In Progress") {
                                            return '<span class="badge badge-warning text-white"> In Progress</span > ';
                                        }
                                        else
                                            if (full.status == "On Hold") {
                                                return '<span class="badge badge-secondary">On Hold</span>';
                                            }
                                            else
                                                if (full.status == "Cancelled") {
                                                    return '<span class="badge badge-secondary">Cancelled</span>';
                                                }
                                                else
                                                    return '<span class="badge badge-success">Completed</span>';
                                }
                            },
                            { "data": "sortId", "name": "SortId", "autoWidth": true, "visible": false, "searchable": false },

                        ],
                        order: [[2, 'asc']]
                    });

                var employeefollowUpsTable = $('#employeefollowUpsTable').
                    DataTable({
                        paging: false,
                        info: false,
                        searching: false,
                        //pageLength: 10,
                        //"scrollY": 200,
                        initComplete: function (settings, json) {
                            
                            var response = settings.json;
                            if (response.pendingFollowUps == 0) {
                                $("#followupCount").hide();
                            }
                            $("#followupCount").html(response.pendingFollowUps);
                            var followupsDonutData = {
                                labels: [
                                    'Pending',
                                    'Completed'
                                ],
                                datasets: [
                                    {
                                        data: [response.pendingFollowUps, response.completedFollowUps],
                                        backgroundColor: ['#f39c12', '#00a65a'],
                                    }
                                ]
                            }

                            var pieChartCanvas2 = $('#pieChart2').get(0).getContext('2d');
                            var pieData = followupsDonutData;
                            var pieOptions = {
                                maintainAspectRatio: false,
                                responsive: true,
                                legend: {
                                    display: false
                                }
                            }
                            //Create pie or douhnut chart
                            // You can switch between pie and douhnut using the method below.
                            var followupPieChart = new Chart(pieChartCanvas2, {
                                type: 'pie',
                                data: pieData,
                                options: pieOptions
                            });

                            if (this.api().data().length == 0) {
                                $('#pieChart2').hide();
                                $('#followUpPiDiv').hide();
                                $("#tasksPiDiv").removeClass("col-md-6 d-flex justify-content-center");
                                $("#tasksPiDiv").addClass("col-md-12 d-flex justify-content-center");
                                $("#textset1").hide();
                                $("#textset2").show();
                            }
                        },
                        search: { regex: true, caseInsensitive: true },
                        ajax: {
                            "url": thisBaseUrl + "/employeeDetail/LoadEmployeeFollowUps",
                            "type": "POST",
                            "datatype": "json",
                            "data": { userName: userName }
                        },
                        columns: [
                            {
                                "render": function (data, type, full, meta) {
                                    return '<a href="' + thisBaseUrl + '/tasks/Edit?id=' + full.taskId + '&username=' + currentUserName + '&viewMode=1"><u>' + full.taskInfo + '</u> </a>';
                                }
                            },
                            {
                                "render": function (data, type, full, meta) {
                                    return '<img name="employeeAvatar" src="../dist/img/user2-160x160.jpg" width="40px" height="40px" class="img-circle elevation-2" alt="User Image"></img>'
                                        + '&nbsp;&nbsp;<span>' + full.followUpEmployeeName + '</span>';
                                }
                            },
                            {
                                "render": function (data, type, full, meta) {
                                    if (full.status == "Open") {
                                        return '<span class="badge badge-warning text-white">Open</span>';
                                    }
                                    else if (full.status == "Close") {
                                        return '<span class="badge badge-success">Closed</span>';
                                    }
                                }
                            },
                            { "data": "sortId", "name": "SortId", "autoWidth": true, "visible": false, "searchable": false }
                        ],
                        order: [[3, 'asc']]
                    });

                $("#employee-closePopup-icon").click(function () {
                    $('.modal-backdrop').removeClass("modal-backdrop");
                    employeeDiv.hide();
                });
            },
            error: function () {
                alert("Dynamic content load failed.");
            }
        });
    }


    assigneePlaceHolder.on('click', '[data-save="modal"]', function (event) {
        event.preventDefault();

        var form = $(this).parents('.modal').find('form');
        var actionUrl = form.attr('action');
        $("#AssigneeTaskId").val(taskId);
        $("#spinnerAssignee").show();
        var dataToSend = form.serialize();
        $.post(actionUrl, dataToSend).done(function (data) {
            var newBody = $('.modal-body', data);
            $("#spinnerAssignee").hide();
            console.log('here');
            assigneePlaceHolder.find('.modal-body').replaceWith(newBody);
            var isValid = newBody.find('[name="IsValid"]').val() == 'True';
            if (isValid) {
                alert("Assignee Updated");
                assigneePlaceHolder.find('.modal').modal('hide');
                setFormState();
                appendEmployeeList();
            }
        });

    });


    function setSubTaskSectionState() {

        $.ajax({
            type: "GET",
            url: thisBaseUrl + "/taskemployees/GetUserPermissions",
            contentType: "application/json; charset=utf-8",
            data: { taskId: taskId, userName: currentUserName },
            datatype: "json",
            success: function (data) {
                tmpData = data;
                console.log(tmpData);
                if (tmpData.isSubTaskEditAllowed == false && isReadOnlyMode != "True") {
                    disabled = true;
                }
                else {
                    disabled = false;
                }
                getSubTasksList();
            },
            error: function () {
                alert("Dynamic content load failed.");
            }
        });

    }

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
            if (tmpData.isTaskDeletionAllowed == true && isReadOnlyMode != "True") {

                $("#deleteBtnDiv").show();
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
                $("#deleteBtnDiv").hide();
            }

            if (tmpData.isProgressUpdateAllowed == true && isReadOnlyMode != "True") {
                $("#updateTaskBtnId").show();
                $("#updateTaskBtnId").click(function () {
                    var $buttonClicked = $(this);
                    //var id = $buttonClicked.attr('data-id');
                    var options = { "backdrop": "static", keyboard: true };
                    $.ajax({
                        type: "GET",
                        url: thisBaseUrl + "/tasks/UpdateTaskProgress?username=" + currentUserName,
                        contentType: "application/json; charset=utf-8",
                        data: { id: taskId },
                        datatype: "json",
                        success: function (data) {
                            progressUpdatePlaceHolder.html(data);
                            progressUpdatePlaceHolder.find('.modal').modal(options);
                            progressUpdatePlaceHolder.find('.modal').modal('show');
                            
                            if ($("#taskStatusBadge").html() == 'Cancelled') {
                                $("#Status").val("5");
                            }
                            else if ($("#taskStatusBadge").html() == 'In Progress') {
                                $("#Status").val("2");
                            }
                            else if ($("#taskStatusBadge").html() == 'Completed') {
                                $("#Status").val("3");
                            }
                            else if ($("#taskStatusBadge").html() == 'On Hold') {
                                $("#Status").val("4");
                            }
                            else {
                                $("#Status").val("1");
                            }

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
                $("#updateTaskBtnId").hide();
            }

            if (tmpData.isTaskEditAllowed == true && isReadOnlyMode != "True") {
                $("#editBtnDiv").show();
            }
            else {
                $("#editBtnDiv").hide();
            }

            if (isReadOnlyMode != "True") {
                if (tmpData.isSubTaskEditAllowed == true) {
                    disabled = false;
                }
                else {
                    //i need to discuss with Usman
                    disabled = true;
                }
            }

            getSubTasksList();
        },
        error: function () {
            alert("Dynamic content load failed.");
        }
    });

    }

    
    $("#editTaskBtnId").click(function () {
        var $buttonClicked = $(this);
        var options = { "backdrop": "static", keyboard: true };
        $.ajax({
            type: "GET",
            url: thisBaseUrl + "/tasks/EditTask?taskId=" + taskId + "&username=" + currentUserName,
            contentType: "application/json; charset=utf-8",
            data: null,
            datatype: "json",
            success: function (data) {
                taskEditPlaceHolder.html(data);
                taskEditPlaceHolder.find('.modal').modal(options);
                taskEditPlaceHolder.find('.modal').modal('show');

                var month = $("#TargetVal").val().split("/")[0];
                month = month - 1;
                var date = $("#TargetVal").val().split("/")[1];
                var remaining = $("#TargetVal").val().split("/")[2];
                //month = month.length == 1 ? ("0" + month) : month;
                //date = date.length == 1 ? ("0" + date) : date;
                var year = remaining.split(" ")[0];

                //$("#Target").val(year + "-" + month  + "-" + date);

                $('#reservationdate').datetimepicker({
                    format: 'DD-MMM-yyyy',
                    date: new Date(year, month, date, 0, 0, 0, 0)
                });

            },
            error: function () {
                alert("Dynamic content load failed.");
            }
        });
    });

    taskEditPlaceHolder.on('click', '[data-save="modal"]', function (event) {
        event.preventDefault();

        var form = $(this).parents('.modal').find('form');
        var actionUrl = form.attr('action');
        $(this).parents('.modal').find('#AssigneeCode').val($("#assigneeDrpDown").val());
        
        $("#Id").val(taskId);
        //$("#TargetVal").val($("#Target").val());
        var dateInstance = $("#Target").val().split("-");
        var date = dateInstance[0];
        var month = dateInstance[1];
        var year = dateInstance[2];

        var monthDict = {
            "Jan": 1, "Feb": 2, "Mar": 3, "Apr": 4, "May": 5, "Jun": 6,
            "Jul": 7, "Aug": 8, "Sep": 9, "Oct": 10, "Nov": 11, "Dec": 12
        };

        $("#Target").html(monthDict[month] + "/" + date + "/" + year);
        $("#Target").val(monthDict[month] + "/" + date + "/" + year);
        $("#TargetVal").val(monthDict[month] + "/" + date + "/" + year);


        var dataToSend = form.serialize();
        var buttonClickedId = $(this).attr('id');
        $("#spinnerCreate").show();
       

        $.post(actionUrl + '?username=' + currentUserName, dataToSend).done(function (data) {
            var newbody = $('.modal-body', data);
            taskEditPlaceHolder.find('.modal-body').replaceWith(newbody);
            var isValid = newbody.find('[name="IsValid"]').val() == 'True';
            $("#spinnerCreate").hide();
            if (isValid) {
                alert("Task updated");
                var month = $("#TargetVal").val().split("/")[0];
                var date = $("#TargetVal").val().split("/")[1];
                var year = $("#TargetVal").val().split("/")[2];

                const monthNames = ["Jan", "Feb", "Mar", "Apr", "May", "Jun",
                    "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"
                ];

                $("#taskTarget").html(date + "-" + monthNames[month - 1] + "-" + year);
                $("#taskPriority").html($("#PriorityId option:selected").text());

                $("#taskTitle").html($("#Title").val());
                $("#taskDescription").html($("#Description").val());

                setFormState();
                taskEditPlaceHolder.find('.modal').modal('hide');
                appendEmployeeList();
                getHistoryList();
            }
            else {
                dateInstance = $("#TargetVal").val().split('/');
                year = dateInstance[2];
                month = dateInstance[0];
                date = dateInstance[1];
                $('#reservationdate').datetimepicker({
                    format: 'DD-MMM-yyyy',
                    date: new Date(year, month - 1, date, 0, 0, 0, 0)
                });
            }
        });
    });


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
    $("#taskProgressShowId").html($("#taskProgressId").val() + "%");

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
    else if (status == '4') {
        $("#taskStatusBadge").html("On Hold");
        $("#taskStatusBadge").addClass('badge badge-secondary');
    }
    else {
        $("#taskStatusBadge").html("Cancelled");
        $("#taskStatusBadge").addClass('badge badge-secondary');
    }

    $("#taskProgressDivId").attr("aria-valuenow", $("#TaskProgress").val());
    $("#taskProgressDivId").css("width", $("#TaskProgress").val() + "%");

    $("#taskRemarks").html(remarks);

    $.post(actionUrl + "?username=" + currentUserName, dataToSend).done(function (data) {
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
            setSubTaskSectionState();
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

    function isSubTaskCreationAllowed() {
        isAllowed = true;
        taskStatus = $("#taskStatusBadge").html();
        if (taskStatus != 'In Progress' && taskStatus != 'Not Started') {
            isAllowed = false;
        }
        return isAllowed;
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
        "<input class=\"iCheckBox\" type=\"checkbox\" id=\"checkboxtaskid\" ischecked name=\"completecheck\" " + disableThis + ">" +
        "&nbsp;<input name = \"subTaskInput\" id=\"inputtaskid\" value=\"tasktitle\" " + disableThis + " class=\"w-100\" />" +
        "</div>" +
        "<div class=\"col-6 d-flex align-items-center\">" +
        //"<i class=\"far fa-user-circle mr-1\" style=\"font-size:24px\" aria-hidden=\"true\">" +
        //"</i>" +
        "<img name=\"subTaskEmployeeAvatar\" src=\"avatarImage\" width=\"40px\" height=\"40px\" class=\"img-circle elevation-2\" alt=\"User Image\" />" +
        "<a " + disableThis + " href=\"javascript:void(null);\" class=\"text-underline\" name=\"employeeLink\" rel=\"nofollow noreferrer\">linkUserName</a>" +
        "<div class=\"form-group mb-0 w-100\" name=\"employeeDrpDwndiv\" style=\"display:none;\">" +
        "employeeOptions" +
        "</div>" +
        "</div>" +
        "<div class=\"col-1\">" +
        "<i name=\"delete\" id=\"deletetaskid\"  " + disableThis + " class=\"fas fa-trash-alt\"></i>" +
        "</div>" +
        "</div>";

    var addSubTaskButtonHtml = "<div class=\"row\">" +
        "<span id=\"noSubTaskSpan\" style=\"display: none;\">There are no sub tasks.</span>" +
        "<div id=\"subTaskAddDiv\" class=\"input-group mb-2\">" +
        "<input type=\"text\" id=\"newTaskInputId\" class=\"form-control\">" +
        "<span class=\"input-group-append\">" +
        "<button " + disableThis + " id=\"addSubtaskBtnId\" type=\"button\" class=\"btn btn-info btn-flat\">Add Sub Task</button>" +
        "</span>" +
        "</div>" +
        "</div>";

    var newBody = "";
    $.ajax({
        type: "GET",
        url: thisBaseUrl + "/subtasks/SubTaskList?username=" + currentUserName,
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
                    newBody = newBody.replace("linkUserName", tmpData.subTaskVMList[i].subTaskEmployeeName);
                    newBody = newBody.replace("ischecked", isSubTaskCompleted);
                    newBody = newBody.replace("avatarImage", tmpData.subTaskVMList[i].avatarImage);

                    var dropdown = "";
                    for (var k = 0; k < allEmployees.length; k++) {

                        var spacing = '';
                        if (allEmployees[k].departmentLevel!= 0) {
                            for (m = 0; m < allEmployees[k].departmentLevel; m++) {
                                spacing += "&nbsp;";
                            }
                        }
                        if (allEmployees[k].isDeptName != true) {
                            spacing = spacing + "&nbsp;&nbsp;";
                        }

                        if (tmpData.subTaskVMList[i].subTaskUserName == allEmployees[k].tagUserName) {
                            dropdown += "<option value='" + allEmployees[k].tagUserName+"' selected>" + spacing + allEmployees[k].tagName + "</option>";
                        }
                        else {
                            if (allEmployees[k].isDeptName == true) {
                                dropdown += "<option disabled>" + spacing + allEmployees[k].tagName + "</option>";
                            }
                            else {
                                dropdown += "<option value='" + allEmployees[k].tagUserName +"'>" + spacing + allEmployees[k].tagName + "</option>";
                            }
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
                                url: thisBaseUrl + "/subtasks/DeleteSubTask?subTaskId=" + $(this).attr('id') + "&username=" + currentUserName,
                                contentType: "application/json; charset=utf-8",
                                data: null,
                                datatype: "json",
                                success: function (data) {
                                    subTaskPlaceHolder.html('');
                                    getSubTasksList();
                                    appendEmployeeList();
                                    getHistoryList();
                                },
                                error: function () {
                                    alert("Dynamic content load failed.");
                                }
                            });
                        }
                    });
                }

                $('input.iCheckBox').on('ifChecked ifUnchecked', function (event) {
                    var description = $(this).closest('.row').find('input[name ="subTaskInput"]').val();
                    var isCompleted = $(this).is(':checked');
                    var subTaskId = $(this).attr('id');
                    var assignee = $("select[name='employeeDrpDwn'] option:selected").val();
                    updateSubTask(subTaskId, taskId, description, assignee, isCompleted);
                });


                if (disabled == false) {
                    $('input[name ="subTaskInput"]').change(function () {
                        var description = $(this).closest('.row').find('input[name ="subTaskInput"]').val();
                        var isCompleted = $(this).is(':checked');
                        var subTaskId = $(this).attr('id');
                        var assignee = $("select[name='employeeDrpDwn'] option:selected").val();
                        updateSubTask(subTaskId, taskId, description, assignee, isCompleted);
                    });
                }
            }
            else {

                subTaskPlaceHolder.html(addSubTaskButtonHtml);
                if (disabled == true) {
                    $("#noSubTaskSpan").show();
                }
            }

            if ((isReadOnlyMode == "True") || (disabled == true)) {
                $("#subTaskAddDiv").hide();
            }

            $('input.iCheckBox').iCheck({
                checkboxClass: 'icheckbox_square-blue',
                radioClass: 'iradio_square-blue',
                increaseArea: '50%' // optional
            });

            $("#addSubtaskBtnId").click(function () {
                //var taskId = taskId;
                if (isSubTaskCreationAllowed() == true) {
                    var description = $("#newTaskInputId").val();
                    var isCompleted = false;
                    var assignee = "";
                    if (description == "") {
                        alert("please enter description");
                    }
                    else
                        addSubTask(taskId, description, isCompleted, assignee);
                }
                else {
                    alert("Operation not allowed in status " + taskStatus + ". Please change the status to In Progress.");
                }
            });

            $("select[name='employeeDrpDwn']").focusout(function () {
                var end = $(this).closest('.row').find("select[name='employeeDrpDwn'] option:selected").text();
                end = end.replaceAll("&nbsp;", "");
                end = end.trimStart();
                $(this).closest('.row').find('[name="employeeDrpDwndiv"]').hide();
                $(this).closest('.row').find('[name="employeeLink"]').html(end);
                $(this).closest('.row').find('[name="employeeLink"]').show();
            });

            $("select[name='employeeDrpDwn']").change(function () {
                var end = this.value;
                var text = $("select[name='employeeDrpDwn'] option:selected").text();
                $(this).closest('.row').find('[name="employeeDrpDwndiv"]').hide();
                $(this).closest('.row').find('[name="employeeLink"]').html(text);
                $(this).closest('.row').find('[name="employeeLink"]').show();

                var description = $(this).closest('.row').find('input[name ="subTaskInput"]').val();
                var isCompleted = $(this).is(':checked');
                var subTaskId = $(this).attr('id');
                end = end.replaceAll("&nbsp;", "");
                end = end.trimStart();
                var assignee = end;
                updateSubTask(subTaskId, taskId, description, assignee, isCompleted);
            });

            if (disabled == false) {
                $("a[name='employeeLink']").click(function () {
                    $(this).hide();
                    $(this).closest('.row').find('[name="employeeDrpDwndiv"]').show();
                });
            }
            else {
                $("i[name='delete']").hide();
                $("a[name='employeeLink']").removeClass('text-underline');
                $("a[name='employeeLink']").removeAttr("href");
            }

        },
        error: function () {
            alert("Dynamic content load failed.");
        }
    });
}

function updateSubTask(subtaskId, taskId, description, assignee, isCompleted) {

    $.post(thisBaseUrl + "/subtasks/Update?username=" + currentUserName,
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

    $.post(thisBaseUrl + "/subtasks/Create?username=" + currentUserName,
        { description: description, taskId: taskId, isCompleted: isCompleted, assignee: assignee },
        function (returnedData) {
            if (returnedData.subTaskExist == true) {
                alert('sub task with same title already exists');
            }
            else if (returnedData.subTaskAdded == true) {
                getHistoryList();
                appendEmployeeList();
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

    var cardBody = "<div class=\"d-flex justify-content-between flex-wrap-xs\">" +
        "<div class=\"d-flex align-items-center\">" +
        "<img name=\"employeeAvatar\" src=\"../dist/img/user2-160x160.jpg\" width=\"40px\" height=\"40px\" class=\"img-circle elevation-2\" alt=\"User Image\"></img>" +
        "&nbsp;<a href=\"javascript: void(null);\" data-username=\"employeeusername\" name=\"employeeDetailLink\">employeeName</a>&nbsp" +
        "<i class=\"fas fa-wifi\" aria-hidden=\"true\"></i>" +
        "</div>" +
        "<span class=\"float-right text-md d-flex align-items-center\">capacity</span>" +
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
                    employeeHtmlBody = employeeHtmlBody.replace("employeeName", tmpData[i].employeeName);
                    employeeHtmlBody = employeeHtmlBody.replace("employeeusername", tmpData[i].userName);
                    employeeHtmlBody = employeeHtmlBody.replace("capacity", getTaskDescription(tmpData[i].capacityId));

                });
                if (i < data.length - 1) {
                    employeeHtmlBody += "</br></br>";
                }
            });
            $("#people-placeholder").html('');
            $("#people-placeholder").html(employeeHtmlBody);

            $("a[name='employeeDetailLink']").click(function () {
                showEmployeeDetails($(this).attr("data-username"));
            });
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
    else if (historyType == "update") {
        historyTypeId = updateAuditType;
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
    else if (historyTypeId == updateAuditType) {
        historyType = "Update";
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
        console.log(filteredHistory[i].historyDate);
        table.rows.add([{
            "Date": filteredHistory[i].historyDate,
            "Type": getHistoryTypeDescription(filteredHistory[i].type),
            "Description": filteredHistory[i].description,
            "Action By": filteredHistory[i].actionBy
        }]).draw();
    }
    //sort by desc..
    table.order([0, 'desc']).draw();

}

setFormState();
setBadgeStyle();
appendEmployeeList();
getHistoryList();
});