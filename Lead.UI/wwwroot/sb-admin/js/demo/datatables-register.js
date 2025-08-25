// Call the dataTables jQuery plugin
$(function () {
    $('#dataTable').DataTable();
    $('#dataTableDesc').DataTable({
        order: [[0, 'desc']]  // Assuming ID is in the first column (index 0)
    });
});
