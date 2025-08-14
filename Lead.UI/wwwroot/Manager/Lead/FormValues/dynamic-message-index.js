document.addEventListener("DOMContentLoaded", function () {
    document.querySelectorAll(".btn-view").forEach(function (btn) {
        btn.addEventListener("click", function () {
            let row = this.closest("tr");
            let businessId = row.querySelector(".business-id").value;
            let submissionId = row.querySelector(".submission-id").value;

            console.log(`Business ID: ${businessId}\nSubmission ID: ${submissionId}`);
        });
    });
});