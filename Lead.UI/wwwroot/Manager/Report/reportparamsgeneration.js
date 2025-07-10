var selectedReportName = document.getElementById("idReportName");
var previewButton = document.getElementById("idPreview");


if (previewButton != null) {
    document.addEventListener("DOMContentLoaded", function (event) {
        HideNShowParamField(selectedReportName.value);
    });
}

const toggleSelection = function(menuItem, columnId) {
    // Deselect all menu items in the same column
    const column = document.getElementById(columnId);
    const menuItems = column.getElementsByClassName('reportmenu-item');
    for (const item of menuItems) {
        if (item !== menuItem) {
            item.classList.remove('selected');
            selectedReportName.value = "";
        }
    }
    //selecting the clicked menu
    menuItem.classList.toggle('selected');
    // if (menuItem.classList.contains('selected')) {
    //     alert(`Selected: ${menuItem.innerText}`);
    // }
    selectedReportName.value = menuItem.innerText;
    HideNShowParamField(selectedReportName.value);
}

previewButton.addEventListener("click", function (e) {
    //alert(selectedReportName.value);
    if (selectedReportName.value == "Empty Report") {
        alert("Please select a report!")
        e.preventDefault();
    }
});

const HideNShowParamField = function (reportName) {
    $.ajax({
        url: '/ReportMiddleware/GetWhichFormControllWillBeShow',
        type: 'GET',
        data: { reportName },
        dataType: 'json',
        contentType: 'application/json',
        success: function (data) {
            $.each(data, function (key, value) {
                if (value.IsHideOrShow == true) { $("#" + value.FieldNameId + "").show(); }
                if (value.IsHideOrShow == false) { $("#" + value.FieldNameId + "").hide(); }
            });
        }
    });
}

//const openNewTab = function() {
//    // Get form input values
//    var reportName = document.getElementById("idReportName").value;
//    var startDate = document.getElementById("StartDate").value;
//    var endDate = document.getElementById("EndDate").value;
//    var clientId = document.getElementById("ClientId").value;

//    // Construct the URL based on the form data
//    var url = "/ReportMiddleware/ViewRdlReport?reportName=" + reportName + "&startDate=" + startDate + "&endDate=" + endDate + "&clientId=" + clientId;

//    // Open the URL in a new tab
//    window.open(url, '_blank');
//}