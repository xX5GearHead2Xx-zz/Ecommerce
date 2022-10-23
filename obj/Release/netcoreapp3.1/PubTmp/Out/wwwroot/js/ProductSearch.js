var previousvalue = "";

$(".ProductSearch").on('keyup blur', function () {
    //previous value stops the search from searching unecessarily
    var value = $(this).val();
    if (value.length > 3 && previousvalue !== value) {
        previousvalue = value;
        $.ajax({
            url: '/Shop/SearchProducts',
            type: 'GET',
            data: {
                'query': value
            },
            dataType: 'html',
            success: function (data) {
                $("#ProductSearchResults").empty();
                $("#ProductSearchResults").append(data);
            },
            error: function (request, error) {
                alert("Request: " + JSON.stringify(request));
            }
        });
    } else {
        $("#ProductSearchResults").empty();
    }
});