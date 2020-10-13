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


var inboxTable = $('#inboxTable').DataTable({
    //scrollY:        300,
    //scrollX:        true,
    //scrollCollapse: true,
    paging: true,
    //fixedColumns:   {
    //    leftColumns: 2
    //},
    initComplete: function (settings, json) {
        $("a[name='employeeDetailLink']").click(function () {
            showEmployeeDetails($(this).attr("data-usercode"));
        });
    },
    columnDefs: [{
        orderable: false,
        targets: 0
    }],
    ajax: {
        "url": thisBaseUrl + "/taskfollowups/LoadTaskInBoxData?username=" + currentUserName,
        "type": "POST",
        "datatype": "json"
    },
    columns: [
        {
            "render": function (data, type, full, meta) {
                return '<img name="employeeAvatar" src="../dist/img/user2-160x160.jpg" width="40px" height="40px" class="img-circle elevation-2" alt="User Image"></img>'
                    + '&nbsp;&nbsp;<a href="javascript:void(null);" data-usercode=' + full.followUpFrom + ' name="employeeDetailLink">' + full.followUpEmployeeName + '</a>';
            }
        },
        {
            "render": function (data, type, full, meta) {
                return '<a href="' + thisBaseUrl + '/tasks/Edit?id=' + full.taskId + '&username=' + currentUserName + '&viewMode=1"><u>' + full.taskInfo + '</u> </a>';
            }
        },
        { "data": "remarks", "name": "Remarks", "autoWidth": true },
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
        { "data": "updatedDate", "name": "Date", "autoWidth": true }

    ],
    order: [[4, 'desc']]
});



var outboxTable = $('#outboxTable').DataTable({
    //scrollY:        300,
    //scrollX:        true,
    //scrollCollapse: true,
    paging: true,
    //fixedColumns:   {
    //    leftColumns: 2
    //},
    select: {
        //style:    'os',
        style: 'multiple',
        selector: 'td:first-child'
    },
    ajax: {
        "url": thisBaseUrl + "/taskfollowups/LoadTaskOutBoxData?username=" + currentUserName,
        "type": "POST",
        "datatype": "json"
    },
    columns: [
        {
            "render": function (data, type, full, meta) {
                return '<a href="' + thisBaseUrl + '/tasks/Edit?id=' + full.taskId + '&username=' + currentUserName + '&viewMode=1"><u>' + full.taskInfo + '</u> </a>';
            }
        },
        { "data": "remarks", "name": "Remarks", "autoWidth": true },
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
        { "data": "followUpDate", "name": "Date", "autoWidth": true, "visible": true }
    ],
    order: [[3, 'desc']]
});

