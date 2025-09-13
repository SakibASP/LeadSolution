document.addEventListener("DOMContentLoaded", function () {
    document.querySelectorAll(".btn-view").forEach(function (btn) {
        btn.addEventListener("click", function () {
            let row = this.closest("tr");
            let formMasterId = row.querySelector(".form-Master-id").value;
            if (formMasterId) {
                console.log(`MASTER ID: ${formMasterId}`);
            }
        });
    });
});