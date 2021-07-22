$(function removeWitespace() {
    $("#plateNumber")
        .change(function (e) {
            let value = $(this).val().toUpperCase();
            let result = value.replace(/\s/g, "");
            $("#plateNumber").val(result);
        });
});