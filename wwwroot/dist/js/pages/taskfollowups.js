var inboxTable = $('#inboxTable').DataTable({
    //scrollY:        300,
    //scrollX:        true,
    //scrollCollapse: true,
    paging: true,
    //fixedColumns:   {
    //    leftColumns: 2
    //},

    columnDefs: [{
        orderable: false,
        targets: 0
    }],
    select: {
        //style:    'os',
        style: 'os',
        selector: 'td:first-child'
    },
    ajax: {
        "url": thisBaseUrl + "/taskfollowups/LoadTaskInBoxData?username=" + currentUserName,
        "type": "POST",
        "datatype": "json"
    },
    columns: [
        { "data": "followUpFrom", "name": "Follow Up From", "autoWidth": true, "visible": true },
        {
            "render": function (data, type, full, meta) {
                return '<a href="' + thisBaseUrl + '/tasks/Edit?id=' + full.taskId + '"><u>' + full.taskInfo + '</u> </a>';
            }
        },
        { "data": "remarks", "name": "Remarks", "autoWidth": true },
        {
            "render": function (data, type, full, meta) {
                if (full.status == "Not Started") {
                    return '<span class="badge badge-danger">Not Started</span>';
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
    order: [[1, 'desc']]
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
                return '<a href="' + thisBaseUrl + '/tasks/Edit?id=' + full.taskId + '"><u>' + full.taskInfo + '</u> </a>';
            }
        },
        { "data": "remarks", "name": "Remarks", "autoWidth": true },
        {
            "render": function (data, type, full, meta) {
                if (full.status == "Not Started") {
                    return '<span class="badge badge-danger">Not Started</span>';
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
    order: [[1, 'desc']]
});

