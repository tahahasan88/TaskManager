$(document).ready(function () {

    //Initialize Select2 Elements
    $('.select2').select2();
    var progressFrom = '0';
    var progressTo = '';
    var fromTargetDate = '';
    var toTargetDate = '';

    

    //Initialize Select2 Elements
    $('.select2bs4').select2({
        theme: 'bootstrap4'
    });
    //$(".slider").slider('enable');

    var employeeDiv = $('#employeeDetailDiv');

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
                            if (this.api().data().length == 0) {
                                $("#followupCount").hide();
                            }
                            $("#followupCount").html(this.api().data().length);
                            var response = settings.json;
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
                                    return '<img name="employeeAvatar" src="../dist/img/user2-160x160.jpg" width="20%" height="20%" class="img-circle elevation-2" alt="User Image"></img>'
                                        + '&nbsp;&nbsp;<span>' + full.followUpFrom + '</span>';
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

    var allEmployeeList = [];

    $('#targetFromdate').datetimepicker({
        format: 'DD-MMM-yyyy'

    });

    $('#targetTodate').datetimepicker({
        format: 'DD-MMM-yyyy'
    });

    $("#fromRange").click(function () {
        $("#calendarFromtrigger").click();
    });

    $("#toRange").click(function () {
        $("#calendarTotrigger").click();
    });

    $("#clearTaskNameId").click(function () {
        $("#taskNameId").val("");
    });

    $("#clearStatusId").click(function () {
        $("#statusSelectValuesId").val('').trigger('change');
    });

    $("#defaultStatusId").click(function () {
        $("#statusSelectValuesId").val('').trigger('change');
        $("#statusSelectValuesId").val($("#statusSelectValuesId option:first").val()).trigger('change');
    });

    $("#defaultAssignUserId").click(function () {
        $("#assignSelectValuesId").val('').trigger('change');
        $("#assignSelectValuesId").val(currentUserName).trigger('change');
    });

    $("#defaultCreatedByUserId").click(function () {
        $("#createdByValuesId").val('').trigger('change');
        $("#createdByValuesId").val(currentUserName).trigger('change');
    });

    $("#clearAssignId").click(function () {
        $("#assignSelectValuesId").val('').trigger('change');
    });

    $("#clearProgressId").click(function () {
        $("#progressFromSelectId").val($("#progressFromSelectId option:first").val());
        $("#progressToSelectId").val($("#progressToSelectId option:first").val());
    });

    $("#clearTargetId").click(function () {
        $("#fromRange").val('');
        $("#toRange").val('');
    });

    $("#clearCreatedById").click(function () {
        $("#createdByValuesId").val('').trigger('change');
    });

    $('#statusSelectValuesId').select2({
        allowClear: true,
        minimumResultsForSearch: -1,
        width: 500
    });

    $("#assignSelectValuesId").select2({
        allowClear: true,
        minimumResultsForSearch: -1,
        width: 500
    });

    $("#createdByValuesId").select2({
        allowClear: true,
        minimumResultsForSearch: -1,
        width: 500
    });

    $("#filter-submit-id").click(function () {

        var taskName = ($("#taskNameId").val() != "") ? $("#taskNameId").val() : "";
        var createdBy = "";
        $('#createdByValuesId > option:selected').each(function () {
            createdBy += $(this).val() + "|";
        });
        if (createdBy.length > 0) {
            createdBy = createdBy.substring(0, createdBy.length - 1);
        }

        var selectStatuses = '';
        $('#statusSelectValuesId > option:selected').each(function () {
            selectStatuses += $(this).val() + "|";
        });
        if (selectStatuses.length > 0) {
            selectStatuses = selectStatuses.substring(0, selectStatuses.length - 1);
        }

        var assignedToValues = '';
        $('#assignSelectValuesId > option:selected').each(function () {
            assignedToValues += $(this).val() + "|";
        });
        if (assignedToValues.length > 0) {
            assignedToValues = assignedToValues.substring(0, assignedToValues.length - 1);
        }

        $('#progressFromSelectId > option:selected').each(function () {
            progressFrom = $(this).val();
        });

        $('#progressToSelectId > option:selected').each(function () {
            progressTo = $(this).val();
        });

        var isValid = true;
        if (progressTo != '') {
            if (parseInt(progressTo) < parseInt(progressFrom)) {
                alert('Progress range is not valid. Progress to value can not be less than progress from value');
                isValid = false;
            }
        }

        if ($("#fromRange").val() != '') {
            var d = new Date($("#fromRange").val());
            fromTargetDate = d;
        }
        if ($("#toRange").val() != '') {
            var d = new Date($("#toRange").val());
            toTargetDate = d;

            if (fromTargetDate != '') {
                if (toTargetDate.getTime() < fromTargetDate.getTime()) {
                    alert('Target to Date can not be less than Target from range');
                    isValid = false;
                }
            }
        }

        if (isValid) {
            tasksTable.column(2).search(taskName, true, false)
                .column(3).search(selectStatuses, true, false)
                .column(5).search(assignedToValues, true, false)
                .column(8).search(createdBy, true, false)
                .draw();
            updateTasksSummaryGraph();
        }
    });

    function updateTasksSummaryGraph() {

        var completedTasks = 0;
        var notStartedTasks = 0;
        var inProgressTasks = 0;
        var cancelledTasks = 0;
        var totalTasks = 0;
        var onHoldTasks = 0;

        var data = tasksTable.rows({ page: 'current' }).data();
        data.each(function (value, index) {
            if (value.status == 'Not Started') {
                notStartedTasks = notStartedTasks + 1;
            }
            else if (value.status == 'Completed') {
                completedTasks = completedTasks + 1;
            }
            else if (value.status == 'In Progress') {
                inProgressTasks = inProgressTasks + 1;
            }
            else if (value.status == 'On Hold') {
                onHoldTasks = onHoldTasks + 1;
            }
            else if (value.status == 'Cancelled') {
                cancelledTasks = cancelledTasks + 1;
            }
            totalTasks = totalTasks + 1;
        });

        console.log(notStartedTasks);
        $("#notStartedTaskId").val(notStartedTasks).trigger('change');
        $("#inProgressTaskId").val(inProgressTasks).trigger('change');
        $("#cancelledTasksId").val(cancelledTasks).trigger('change');
        $("#completedTasksId").val(completedTasks).trigger('change');
        $("#onHoldTaskId").val(onHoldTasks).trigger('change');
        $("#totalTasksId").val(totalTasks).trigger('change');
    }


    $("#filter-reset-id").click(function () {

        $("#taskNameId").val("");

        $("#statusSelectValuesId").val('').trigger('change');
        $("#assignSelectValuesId").val('').trigger('change');
        $("#createdByValuesId").val('').trigger('change');
        $("#progressFromSelectId").val($("#progressFromSelectId option:first").val());
        $("#progressToSelectId").val($("#progressToSelectId option:first").val());
        $("#fromRange").val('');
        progressFrom = '0';
        progressTo = '';
        $("#toRange").val('');
        $("#createdById").val('');
        fromTargetDate = '';
        toTargetDate = '';

        tasksTable.column(2).search('', true, false)
            .column(3).search('', true, false)
            .column(5).search('', true, false)
            .column(8).search('', true, false)
            .draw();
        updateTasksSummaryGraph();
    });


    var summaryplaceHolder = $("#summaryRow");

    function validateTargetDateFilter(thisTargetDate) {

        if (thisTargetDate != '') {
            thisTargetDate = new Date(thisTargetDate);
        }
        var min = '';
        if (fromTargetDate != '') {
            min = new Date(fromTargetDate);;
        }

        if (min != '') {
            if (toTargetDate != '') {
                var max = new Date(toTargetDate);
                max.setHours(max.getHours() + 23);
                if (thisTargetDate.getTime() >= min.getTime() && thisTargetDate <= max.getTime()) {
                    return true;
                }
                else
                    return false;
            }
            else {
                if (thisTargetDate.getTime() >= min.getTime()) {
                    return true;
                }
                else
                    return false;
            }
        }
        else {
            min = new Date('0001-01-01T00:00:00');
            if (toTargetDate != '') {
                var max = new Date(toTargetDate);
                max.setHours(max.getHours() + 23);
                if (thisTargetDate.getTime() >= min.getTime() && thisTargetDate.getTime() <= max.getTime()) {
                    return true;
                }
                else
                    return false;
            }
            else if (thisTargetDate.getTime() >= min.getTime()) {
                return true;
            }
            else
                return false;
        }
    }


    function validateProgressFilter(thisProgress) {
        if (isNaN(progressFrom)) {
            progressFrom = '0';
        }
        thisProgress = parseInt(thisProgress);
        var min = parseInt(progressFrom);
        if (min != 0) {
            if (progressTo != 'Any' && progressTo != '') {
                var max = parseInt(progressTo);
                if (thisProgress >= min && thisProgress <= max) {
                    return true;
                }
                else
                    return false;
            }
            else {
                if (thisProgress >= min) {
                    return true;
                }
                else
                    return false;
            }
        }
        else {
            if (progressTo != 'Any' && progressTo != '') {
                var max = parseInt(progressTo);
                if (thisProgress >= min && thisProgress <= max) {
                    return true;
                }
                else
                    return false;
            }
            else if (thisProgress >= min) {
                return true;
            }
            else
                return false;
        }
    }


    /* Custom filtering function which will search data in column four between two values */
    $.fn.dataTable.ext.search.push(
        function (settings, data, dataIndex) {
            
            var isValid = validateProgressFilter(data[10]);
            if (isValid) {
                isValid = validateTargetDateFilter(data[9]);
            }
            return isValid;
        }
    );

    //var baseUrl = "@Html.Raw(this.Context.Request.Scheme + "://" + this.Context.Request.Host + this.Context.Request.Path)";*@
    var tasksTable = $('#tasksTable')
        .on('init.dt', function () {


        }).on('page.dt', function () {
            setTimeout(
                function () {
                    $(".knob").knob({});
                }, 100);
        }).
        DataTable({
            paging: true,
            initComplete: function (settings, json) {
                $("a[name='employeeDetailLink']").click(function () {
                    showEmployeeDetails($(this).html());
                });
                $(".knob").knob({});
            },
            columnDefs: [{
                orderable: false,
                className: 'select-checkbox',
                targets: 0
            }
            ],
            select: {
                style: 'multiple',
                selector: 'td:first-child'
            },
            search: { regex: true, caseInsensitive: true },
            ajax: {
                "url": thisBaseUrl + "/tasks/LoadTasksData?username=" + currentUserName,
                "type": "POST",
                "datatype": "json"
            },
            columns: [
                {
                    data: null, render: function (data, type, row) {
                        return "";
                    }
                },
                { "data": "taskId", "name": "TaskId", "autoWidth": true, "visible": false, "searchable": true },
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
                {

                    "render": function (data, type, full, meta) {

                        var color = "";
                        if (full.status == "Not Started") {
                            color = 'ffc107';
                        }
                        else
                            if (full.status == "In Progress") {
                                color = 'ffc107';
                            }
                            else
                                if (full.status == "On Hold") {
                                    color = '6c757d';
                                }
                                else
                                    if (full.status == "Cancelled") {
                                        color = '6c757d';
                                    }
                                    else
                                        color = '28a745';

                        return '<input type="range" max="100" style="background:#ffc107;color:#ffc107;" value=\"' + full.progress + '' + '\"' +
                            " class=\"slider\"></input> <output id=\"taskProgressId\">" + full.progress + "</output>";
                    }
                },
                {
                    "render": function (data, type, full, meta) {
                        return '<img name="employeeAvatar" src="../dist/img/user2-160x160.jpg" width="20%" height="20%" class="img-circle elevation-2" alt="User Image"></img>'
                            + '&nbsp;&nbsp;<a href="javascript:void(null);" name="employeeDetailLink">' + full.assignedTo + '</a>';
                    }
                },
                {
                    "render": function (data, type, full, meta) {
                        return '<span title="' + full.lastUpdated + '" name="lastupdatedLink">' + jQuery.timeago(full.lastUpdated) + '</span>';
                    }
                },
                {
                    "render": function (data, type, full, meta) {
                        if (full.isEditable == true) {
                            return '<a class="btn btn-info" href="' + thisBaseUrl + '/tasks/Edit?id=' + full.taskId + '&username=' + currentUserName + '">Edit</a>';
                        }
                        else {
                            return "";
                        }
                    }
                },
                { "data": "createdBy", "name": "Created By", "autoWidth": true, "visible": false, "searchable": true },
                { "data": "targetDate", "name": "Target Date", "autoWidth": true, "visible": false, "searchable": true },
                { "data": "progress", "name": "ProgressVal", "autoWidth": true, "visible": false, "searchable": true },
            ],
            order: [[1, 'desc']]
        });

    var taskCreationPlaceHolder = $('#modal-default');
    var taskMultipleCreationPlaceHolder = $('#modal-multiple');
    var taskFollowUpPlaceHolder = $('#modal-followup');

    $("#create-task-btnId").click(function () {
        var $buttonClicked = $(this);
        var options = { "backdrop": "static", keyboard: true };
        $.ajax({
            type: "GET",
            url: thisBaseUrl + "/tasks/Create",
            contentType: "application/json; charset=utf-8",
            data: { userName: currentUserName },
            datatype: "json",
            success: function (data) {
                taskCreationPlaceHolder.html(data);
                taskCreationPlaceHolder.find('.modal').modal(options);
                taskCreationPlaceHolder.find('.modal').modal('show');

                $("#addMultipleCreationLink").click(function () {
                    $.ajax({
                        type: "GET",
                        url: thisBaseUrl + "/tasks/CreateMultiple",
                        contentType: "application/json; charset=utf-8",
                        data: null,
                        datatype: "json",
                        success: function (data) {
                            taskMultipleCreationPlaceHolder.html(data);
                            taskMultipleCreationPlaceHolder.find('.modal').modal(options);
                            taskMultipleCreationPlaceHolder.find('.modal').modal('show');
                        },
                        error: function () {
                            alert("Dynamic content load failed.");
                        }
                    });
                });
            },
            error: function () {
                alert("Dynamic content load failed.");
            }
        });
    });



    $("#take-followup-btnId").click(function () {
        var $buttonClicked = $(this);

        var row_data = tasksTable.rows('.selected').data();
        if (row_data.length > 0) {
            var options = { "backdrop": "static", keyboard: true };
            $.ajax({
                type: "GET",
                url: thisBaseUrl + "/taskfollowups/Create",
                contentType: "application/json; charset=utf-8",
                data: null,
                datatype: "json",
                success: function (data) {
                    taskFollowUpPlaceHolder.html(data);
                    taskFollowUpPlaceHolder.find('.modal').modal(options);
                    taskFollowUpPlaceHolder.find('.modal').modal('show');
                },
                error: function () {
                    alert("Dynamic content load failed.");
                }
            });
        }
        else {
            alert("Please select task for followup");
        }
    });


    function getSummaryInfo() {

        $.ajax({
            type: "GET",
            url: thisBaseUrl + "/TaskSummary/list?username=" + currentUserName,
            contentType: "application/json; charset=utf-8",
            data: null,
            datatype: "json",
            success: function (data) {
                $.each(data, function (i, val) {
                    tmpData = data[i];

                    $(".knob").knob({});
                    $('#notStartedTaskId').trigger(
                        'configure',
                        {
                            "min": 0,
                            "max": tmpData.totalTasksCount,
                            "fgColor": "#6c757d",
                            "cursor": false
                        }
                    );
                    $("#notStartedTaskId").val(tmpData.notStartedTasksCount).trigger('change');
                    $('#inProgressTaskId').trigger(
                        'configure',
                        {
                            "min": 0,
                            "max": tmpData.totalTasksCount,
                            "fgColor": "#f39c12",
                            "cursor": false
                        }
                    );
                    $("#inProgressTaskId").val(tmpData.inProgressTasksCount).trigger('change');
                    $('#cancelledTasksId').trigger(
                        'configure',
                        {
                            "min": 0,
                            "max": tmpData.totalTasksCount,
                            "fgColor": "#343a40",
                            "cursor": false
                        }
                    );
                    $("#cancelledTasksId").val(tmpData.cancelledTasksCount).trigger('change');

                    $('#completedTasksId').trigger(
                        'configure',
                        {
                            "min": 0,
                            "max": tmpData.totalTasksCount,
                            "fgColor": "#00a65a",
                            "cursor": false
                        }
                    );

                    $("#completedTasksId").val(tmpData.completedTasksCount).trigger('change');
                    $('#onHoldTaskId').trigger(
                        'configure',
                        {
                            "min": 0,
                            "max": tmpData.totalTasksCount,
                            "fgColor": "#6f42c1",
                            "cursor": false
                        }
                    );
                    $("#onHoldTaskId").val(tmpData.onHoldTasksCount).trigger('change');

                    $('#totalTasksId').trigger(
                        'configure',
                        {
                            "min": 0,
                            "max": tmpData.totalTasksCount,
                            "fgColor": "#3c8dbc",
                            "cursor": false
                        }
                    );
                    $("#totalTasksId").val(tmpData.totalTasksCount).trigger('change');
                    //$("input.knob").trigger('change');
                });
            },
            error: function () {
                alert("Dynamic content load failed.");
            }
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
                        allEmployeeList.push(tmpData[i].userName);
                        var o = new Option(tmpData[i].userName, tmpData[i].userName);
                        /// jquerify the DOM object 'o' so we can use the html method
                        $(o).html(tmpData[i].userName);
                        $("#assignSelectValuesId").append(o);
                        var o1 = new Option(tmpData[i].userName, tmpData[i].userName);
                        $(o1).html(tmpData[i].userName);
                        $("#createdByValuesId").append(o1);
                    });
                });
            },
            error: function () {
                alert("Dynamic content load failed.");
            }
        });
    }

    taskFollowUpPlaceHolder.on('click', '[data-save="modal"]', function (event) {
        event.preventDefault();

        var row_data = tasksTable.rows('.selected').data();
        var listOfTaskIds = "";
        $.each(row_data, function (i, val) {
            tmpData = row_data[i];
            listOfTaskIds += tmpData["taskId"] + ","
        });
        listOfTaskIds = listOfTaskIds.substring(0, listOfTaskIds.length - 1);
        $("#ListofTasks").val(listOfTaskIds);

        var form = $(this).parents('.modal').find('form');
        var actionUrl = form.attr('action');
        var dataToSend = form.serialize();
        $("#spinnerFollowUpCreate").show();

        $.post(actionUrl, dataToSend).done(function (data) {
            var newBody = $('.modal-body', data);
            $("#spinnerFollowUpCreate").hide();
            taskFollowUpPlaceHolder.find('.modal-body').replaceWith(newBody);
            var isValid = newBody.find('[name="IsValid"]').val() == 'True';
            if (isValid) {
                taskFollowUpPlaceHolder.find('.modal').modal('hide');
                tasksTable.ajax.reload(function () {
                    setTimeout(
                        function () {
                            $(".knob").knob({});
                        }, 100);
                });
            }
        });
    });


    taskMultipleCreationPlaceHolder.on('click', '[data-save="modal"]', function (event) {
        event.preventDefault();
        var enteredText = $(this).parents('.modal').find('#ListOfTaskTitles').val();
        var rows = enteredText.split(/\r?\n/);
        var listOfTaskNames = "";
        for (var i = 0; i < rows.length; i++) {
            if (rows[i].trim() != "") {
                listOfTaskNames += rows[i] + ","
            }
        }
        listOfTaskNames = listOfTaskNames.substring(0, listOfTaskNames.length - 1);
        $("#ListOfTaskTitles").val(listOfTaskNames);
        $(this).parents('.modal').find('#Target').val($("#Target").val());
        $(this).parents('.modal').find('#TaskProgress').val($("#TaskProgress").val());
        $(this).parents('.modal').find('#Description').val($("#Description").val());
        $(this).parents('.modal').find('#PriorityId').val($("#PriorityId").val());
        $(this).parents('.modal').find('#AssigneeCode').val($("#AssigneeCode").val());

        var form = $(this).parents('.modal').find('form');
        var actionUrl = form.attr('action');
        var dataToSend = form.serialize();
        var buttonClickedId = $(this).attr('id');
        $.post(actionUrl, dataToSend).done(function (data) {
            var newbody = $('.modal-body', data);
            taskMultipleCreationPlaceHolder.find('.modal-body').replaceWith(newbody);
            var isValid = newbody.find('[name="IsValid"]').val() == 'True';
            if (isValid) {

                taskMultipleCreationPlaceHolder.find('.modal').modal('hide');
                taskCreationPlaceHolder.find('.modal').modal('hide');
                tasksTable.ajax.reload(function () {
                    setTimeout(
                        function () {
                            $(".knob").knob({});
                        }, 100);
                });
            }
        });
    });

    taskCreationPlaceHolder.on('click', '[data-save="modal"]', function (event) {
        event.preventDefault();

        var form = $(this).parents('.modal').find('form');
        var actionUrl = form.attr('action');
        $(this).parents('.modal').find('#AssigneeCode').val($("#assigneeDrpDown").val());
        var dataToSend = form.serialize();
        var buttonClickedId = $(this).attr('id');
        $("#spinnerCreate").show();
        
        $.post(actionUrl, dataToSend).done(function (data) {
            var newbody = $('.modal-body', data);
            taskCreationPlaceHolder.find('.modal-body').replaceWith(newbody);
            var isValid = newbody.find('[name="IsValid"]').val() == 'True';
            $("#spinnerCreate").hide();
            if (isValid) {
                if (buttonClickedId == "task-submit-btnId") {
                    taskCreationPlaceHolder.find('.modal').modal('hide');
                }
                else {
                    getSummaryInfo();
                    alert("Task added");
                }
                tasksTable.ajax.reload(function () {
                    setTimeout(
                        function () {
                            $(".knob").knob({});
                        }, 100);
                });
            }
        });
    });

    getAllEmployees();
    getSummaryInfo();

});