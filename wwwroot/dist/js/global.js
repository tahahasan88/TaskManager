$("#employeeDetailId").html(currentUserName);

$(".nav-sidebar .nav-item .nav-link").each(function () {
    $(this).removeClass("active");
    var href = $(this).attr('href');
    if (href == thisPage) {
        $(this).addClass('active');
    }
});