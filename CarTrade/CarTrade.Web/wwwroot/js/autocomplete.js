$(function auto() {
    $("#searchVehicle").autocomplete({
        source: function (request, response) {
            delay: 10000;
            var myData = { "prefix": request.term };
            var token = $("input[name='__RequestVerificationToken']").val();
            $.ajax({
                url: '/Vehicles/AutoComplete/',
                headers: {
                    "RequestVerificationToken": token
                },
                data: JSON.stringify(myData),
                dataType: "json",
                type: "POST",
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    response($.map(data, function (item) {
                        return item;
                    }))
                },
                failure: function (response) {
                    alert(response.responseText);
                }
            });
        },
        select: function (e, i) {
        },
        minLength: 0
    }).focus(function () {
        $(this).autocomplete("search");
    });
});
