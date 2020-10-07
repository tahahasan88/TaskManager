$("#employeeDetailId").html(currentUserName);

$(".nav-sidebar .nav-item .nav-link").each(function () {
    $(this).removeClass("active");
    var href = $(this).attr('href');
    if (href == thisPage) {
        $(this).addClass('active');
    }
});

getEmployeeDetails(currentUserName);

function getEmployeeDetails(userName) {
    $.ajax({
        type: "GET",
        url: thisBaseUrl + "/employeeDetail/GetEmployeeDetails",
        contentType: "application/json; charset=utf-8",
        data: { userName: userName },
        datatype: "json",
        success: function (data) {
            $("#employeeAvatarId").attr("src", data.avatarImage);
        },
        error: function () {
            alert("Dynamic content load failed.");
        }
    });
}