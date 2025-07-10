
//import pager from '../services/item-sale-service.js';
import clientUserService from '../services/client-user-service.js';


if (typeof jQuery === 'undefined') {
    throw new Error('Requires jQuery library.');
}

const userService = (function ($) {
    'use strict';

    const _tableId = 'detailsTable tbody';

    let GetURLParameter = function (sParam) {
        var sPageURL = window.location.search.substring(1);
        var sURLVariables = sPageURL.split('&');
        for (var i = 0; i < sURLVariables.length; i++) {
            var sParameterName = sURLVariables[i].split('=');
            if (sParameterName[0] == sParam) {
                return sParameterName[1];
            }
        }
    }

    const id = GetURLParameter('AutoId');
    //alert(typeof(id));
    if (typeof (id) != "undefined") {
        //For Editing the existing Records
        if (id > 0) {
            var data = { "MasterId": id };
            /*alert(data);*/
            clientUserService.getUserService(
                data,
                function (res) {
                    var detArr = [];
                    $("#MasterId").val(res.result.ClientUserMaster.AutoId);
                    $("#ClientId").val(res.result.ClientUserMaster.ClientId);
                    $("#TotalAmount").val(res.result.ClientUserMaster.TotalAmount);
                    //alert(res.result.ClientUserMaster.IsActive.toString());
                    $("#IsActive").val(res.result.ClientUserMaster.IsActive.toString());
                    if (res.result.ClientUserMaster.StartDate != null) {
                        let startDate = new Date(res.result.ClientUserMaster.StartDate);
                        $("#StartDate").val(startDate.toInputFormat());
                    }
                    if (res.result.ClientUserMaster.EndDate != null) {
                        let endDate = new Date(res.result.ClientUserMaster.EndDate);
                        $("#EndDate").val(endDate.toInputFormat());
                    }

                    $.each(res.result.ClientUserDetails, function (i, v) {
                        $("#detailsTable tbody").append(
                            '<tr>' +
                            '<td> <a id="DetailId_' + i + '" data-itemId="' + v.AutoId + '" href="#" class="btn  btn-default a-btn-slide-text deleteItem "><span class="fa fa-trash" aria-hidden="true" title="Delete"></span></a></td >' +
                            '<td> <select class="form-control" id="serviceList_' + i + '" value="' + (v.ServiceId) + '" class="form-control"/></td>' +
                            '<td> <input type="number" id="amount_' + i + '" placeholder="Amount" value="' + (v.Amount) + '" class="form-control"/></td>' +
                            '</tr > '
                        )

                        populateServiceList(i, v.ServiceId);
                        let idx = i - 1;
                        if (idx >= 0) {
                            DisableTrash(idx);
                        }
                    });

                    $("#save").html("Update");

                    CalcNetAmount();
                },
                function (error) {
                    console.log(error);
                }
            );
        }
        else {
            var detArr = [];
            //First Row
            detArr.push(' <tr> ' +
                '<td> <a id="DetailId_' + 0 + '" data-itemId="' + 0 + '" href="#" class="btn  btn-default a-btn-slide-text deleteItem"><span class="fa fa-trash" aria-hidden="true" title="Delete"></span></a></td >' +
                '<td> <select style="padding: 5px" class="form-control" id="serviceList_' + 0 + '" /></td>' +
                '<td> <input style="padding: 5px" type="number" id="amount_' + 0 + '" placeholder="Amount" class="form-control" /></td>' +
                '</tr> ')
            $("#detailsTable tbody").append(detArr);
            $("#save").html("Save");
        }
    }

    let Confirmation = function () {
        return confirm("Are you sure?");
    }

    let Validate = function () {
        let isValid = true;
        $.each($("#detailsTable tbody tr"), function () {
            var idx = $(this).closest("tr").index();
            let amount = parseFloat($('#amount_' + idx).val());
            let serviceItem = parseFloat($('#serviceList_' + idx).val());

            if (isNaN(amount)) {
                alert('Please enter required field - Amount');
                isValid = false;
            }
            else if (isNaN(serviceItem)) {
                alert('Please enter required field - Service');
                isValid = false;
            }
            
        });

        if ($('#StartDate').val() == '') {
            alert('Please enter required field - Start Date');
            isValid = false;
        }
        else if ($('#EndDate').val() == '') {
            alert('Please enter required field - End Date');
            isValid = false;
        }

        return isValid;
    }


    let ledger_validate = function () {
        $('#detailsTable tbody tr').each(function () {
            breakOut = false;
            var idx = $(this).closest("tr").index();
            //alert($('#itemList_' + idx).val());
            if ($('#serviceList_' + idx).val() == null) {
                $('#serviceList_' + idx).focus();
                breakOut = true;
                return false;
            }
        });

        if (breakOut) {
            breakOut = false;
            return false;
        }
        else {
            return true;
        }
    }

    let isNumberKey = function (evt) {
        var charCode = (evt.which) ? evt.which : evt.keyCode
        if (charCode > 31 && (charCode < 48 || charCode > 57))
            return false;
        return true;
    }

    let CalcNetAmount = function () {
        var totAamount = 0;
        $.each($("#detailsTable tbody tr"), function () {
            let idx = $(this).closest("tr").index();

            let iamount = parseFloat($('#amount_' + idx).val());
            if (!isNaN(iamount)) {
                totAamount += iamount;
            }
        });
        //alert(totAamount);
        $("#TotalAmount").val(totAamount.toFixed(2));
    }



    return {
        GetURLParameter: GetURLParameter,
        isNumberKey: isNumberKey,
        CalcNetAmount: CalcNetAmount,
        ledger_validate: ledger_validate,
        Validate: Validate,
        GetServiceList: GetServiceList,
        Confirmation: Confirmation
    }

})(jQuery);

$("#detailsTable tbody").on("keyup",function () {
    //code here
    userService.CalcNetAmount();
});

$("#addNewItem").on("click",function (e) {
    e.preventDefault();
    $("#MasterId").val('');
    $("#detailsTable tbody tr").remove();
    $("#save").html("Save Service");
    $('#newClientModal').modal('show');
});

//Adding new row in the table
$("#addMore").on("click",function (e) {
    //alert("Test");
    e.preventDefault();
    var detailsTableBody = $("#detailsTable tbody");
    var lastrowIndex = $("#detailsTable tbody tr:last-child").index();
    var productItem = '<tr>' +
        '<td> <a id="DetailId_' + (lastrowIndex + 1) + '" data-itemId="' + 0 + '" href="#" class="btn  btn-default a-btn-slide-text deleteItem"><span class="fa fa-trash" aria-hidden="true" title="Delete"></span></a></td >' +
        '<td> <select class="form-control serviceList" id="serviceList_' + (lastrowIndex + 1) + '" /></td>' +
        '<td> <input type="number" id="amount_' + (lastrowIndex + 1) + '" placeholder="Amount" class="form-control" /></td >' +
        '</tr> '

    detailsTableBody.append(productItem);
    userService.GetServiceList(lastrowIndex + 1);
    $('#serviceList_' + (lastrowIndex + 1)).select2();
    
    DisableTrash(lastrowIndex);
});


$(document).on('click', 'a.deleteItem', function (e) {
    e.preventDefault();
    /*alert($(this).attr('data-itemId'));*/
    var $self = $(this);
    if ($(this).attr('data-itemId') == "0") {
        $(this).parents('tr').css("background-color", "#FF3700").fadeOut(800, function () {
            $(this).remove();
            let idx = $("#detailsTable tbody tr:last-child").index();
            EnableTrash(idx);
            userService.CalcNetAmount();
        });
    } else {
        var data = { id: $(this).attr('data-itemId') };
        if (confirm('Are you sure you want to delete this item?')) {
            clientUserService.deleteUserServiceDetails(
                data,
                function (response) {
                    $self.parents('tr').css("background-color", "#FF3700").fadeOut(800, function () {
                        $(this).remove();
                        let idx = $("#detailsTable tbody tr:last-child").index();
                        EnableTrash(idx);
                        userService.CalcNetAmount();
                    });
                }, function (error) {
                    alert('error');
                })
        }
        else {
            return false;
        }
    }
});

$("#save").on("click",function (e) {
    e.preventDefault();
    var IsConfirm = userService.Confirmation();
    if (IsConfirm) {
        if (userService.Validate()) {
            var serviceArr = [];
            serviceArr.length = 0;
            //Transfer Details
            $.each($("#detailsTable tbody tr"), function () {
                var idx = $(this).closest("tr").index();
                serviceArr.push({
                    AutoId: $('#DetailId_' + idx).attr('data-itemid'),
                    MasterId: $('#MasterId').val(),
                    ServiceId: $('#serviceList_' + idx).val(),
                    Amount: $('#amount_' + idx).val(),
                });
            });

            var serviceMaster = {
                AutoId: $("#MasterId").val(),
                StartDate: $('#StartDate').val(),
                EndDate: $('#EndDate').val(),
                ClientId: $('#ClientId').val(),
                TotalAmount: $('#TotalAmount').val(),
                IsActive: $('#IsActive').val()
            };

            let data = {
                ClientUserMaster: serviceMaster,
                ClientUserDetails: serviceArr
            };

            //let _test = $('#IsActive').val();
            //alert(_test);

            if (data.ClientUserDetails.length > 0) {
                if ($("#save").html() == "Update") {
                    clientUserService.updateUserService(
                        data,
                        function (response) {
                            $.toast({
                                heading: 'Success',
                                text: 'User Service Updated.',
                                position: 'top-right',
                                loaderBg: '#ff6849',
                                icon: 'success',
                                hideAfter: 1000, // This specifies how long the toast will be visible in milliseconds (3 seconds in this case)
                                stack: false // This ensures that only one toast will be shown at a time
                            });

                            //window.location.href = response.redirectTo;
                            setTimeout(function () {
                                // Redirect to the specified URL after one seconds
                                window.location.href = response.redirectTo;
                            }, 1000);
                        }, function (error) {
                            alert(error);
                        })
                }
                else {
                    clientUserService.saveUserService(
                        data,
                        function (response) {
                            $.toast({
                                heading: 'Success',
                                text: 'User Service Created.',
                                position: 'top-right',
                                loaderBg: '#ff6849',
                                icon: 'success',
                                hideAfter: 1000, // This specifies how long the toast will be visible in milliseconds (3 seconds in this case)
                                stack: false // This ensures that only one toast will be shown at a time
                            });

                            //window.location.href = response.redirectTo;
                            setTimeout(function () {
                                // Redirect to the specified URL after one seconds
                                window.location.href = response.redirectTo;
                            }, 1000);
                        }, function (error) {
                            alert(error);
                        })
                }
            }
            else {
                $.toast({
                    heading: 'Error',
                    text: 'Failed to create item.',
                    position: 'top-right',
                    loaderBg: '#ff6849',
                    icon: 'error',
                    hideAfter: 2000, // This specifies how long the toast will be visible in milliseconds (3 seconds in this case)
                    stack: false // This ensures that only one toast will be shown at a time
                });
            }
        }
    }
});

const DisableTrash = function (idx) {
    //alert('disableTrash ' + idx);
    $("#DetailId_" + idx).addClass('disabled');
}
const EnableTrash = function (idx) {
    //alert('enableTrash ' + idx);
    $("#DetailId_" + idx).removeClass('disabled');
}
