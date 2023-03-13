window.setTimeout(function () {
    $(".alert").fadeTo(500, 0).SlideUp(500, function () {
        $(this).remove();
    });
}, 3000);
