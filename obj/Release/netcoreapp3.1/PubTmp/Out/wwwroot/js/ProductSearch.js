$(".ProductSearch").on('keyup blur', function () {
    var value = $(this).val();
    if (value.length > 3) {
        $.ajax({
            url: '/Shop/SearchProducts',
            type: 'GET',
            data: {
                'query': value
            },
            dataType: 'html',
            success: function (data) {
                console.log(data);
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