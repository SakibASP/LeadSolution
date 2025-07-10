document.addEventListener("DOMContentLoaded", function () {
    $("#RoleId").on("change", function () {
        //uncheck all checkbox before load
        $('input:checkbox').each(function () {
            $(this).prop("checked", false);
        });

        let RoleId = $(this).children("option:selected").val();
        //alert(RoleId);
        $.ajax({
            url: '/AdminRights/GetRoleWiseSelectedPages',
            type: "GET",
            dataType: 'json',
            data: { roleId: RoleId },
            //contentType: "application/json; charset=utf-8",        
            success: function (data) {
                $.each(data, function (key, value) {
                    $.each(value.MenuSelections, function (key, v) {
                        //alert(v.MenuId);
                        if (v.IsSelected == true)
                            $('#' + v.MenuId).prop('checked', true);
                        else
                            $('#' + v.MenuId).prop('checked', false);
                    });

                });
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.status);
                alert(thrownError);
            }
        });
    });

});

/**
 * Select Parent item and it will check/uncheck child items accordingly
 * Select Child item and it will check parent items accordingly
**/
$('input[type=checkbox]').on("click", function () {
    //alert(this.checked);
    if (this.checked) {
        $(this).closest('ul').siblings('input:checkbox').prop('checked', true);
        $(this).closest('ul').parent().closest('ul').prevAll('input:checkbox').prop('checked', true);
        //alert('Item checked')
    }
    $(this).parent().find('li input[type=checkbox]').prop('checked', this.checked);

});


const submit_form = function () {
    var RoleId = $("#RoleId").val();
    if (RoleId == "") {
        return $.toast({
            heading: 'Failed!',
            text: 'No role selected.',
            position: 'top-right',
            loaderBg: '#ff6849',
            icon: 'error',
            hideAfter: 1000, // This specifies how long the toast will be visible in milliseconds (3 seconds in this case)
            stack: false // This ensures that only one toast will be shown at a time
        });
    }
    var Model = [];
    $('input:checkbox').each(function () {
        var item = {};
        //here the item name should be the same as your model
        item["MenuId"] = $(this).attr("id");
        item["IsSelected"] = $(this).prop("checked");
        Model.push(item);
    });
    //alert(Model);
    //alert(RoleId);
    $.ajax({
        url: '/AdminRights/UpdateRecords',
        type: "POST",
        data: { model: Model, roleId: RoleId },//formated_data,
        dataType: 'JSON',
        //contentType: "application/json; charset=utf-8",
        success: function (data) {
            //alert(data);
            $.toast({
                heading: 'Success',
                text: data,
                position: 'top-right',
                loaderBg: '#ff6849',
                icon: 'success',
                hideAfter: 1000, // This specifies how long the toast will be visible in milliseconds (3 seconds in this case)
                stack: false // This ensures that only one toast will be shown at a time
            });
        },
        error: function (req, status, err) {
            alert(req.status);
            alert(err);
            $.toast({
                heading: 'Failed!',
                text: 'Something went wrong.',
                position: 'top-right',
                loaderBg: '#ff6849',
                icon: 'error',
                hideAfter: 2000, // This specifies how long the toast will be visible in milliseconds (3 seconds in this case)
                stack: false // This ensures that only one toast will be shown at a time
            });
        }
    });

}