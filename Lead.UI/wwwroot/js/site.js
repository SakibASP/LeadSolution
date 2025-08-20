$(function () {
    // ========= Sidebar handling =========
    const $sidebar = $(".sidebar");
    const $body = $("body");

    const handleSidebarResize = () => {
        const width = $(window).width();
        if (width < 768) {
            $sidebar.find(".collapse").collapse("hide");
        }
        if (width < 480 && !$sidebar.hasClass("toggled")) {
            $body.addClass("sidebar-toggled");
            $sidebar.addClass("toggled");
            $sidebar.find(".collapse").collapse("hide");
        }
    };

    // Run on load + resize
    handleSidebarResize();
    $(window).on("resize", handleSidebarResize);

    // ========= Dropdown search =========
    $(document).on("keyup", "#drpSearch", function () {
        const value = $(this).val().toLowerCase();
        $(".custom-dropdown-item").each(function () {
            $(this).toggle($(this).text().toLowerCase().includes(value));
        });
    });

    // Select item from dropdown
    $(document).on("click", ".custom-dropdown-item", function () {
        $("#drpSearch").text($(this).text());
        $("#BusinessId").val($(this).data("value")); // keep value in hidden field if needed
    });

    // ========= Toast alerts auto-hide =========
    hideTempMessage();
});

// Hiding success/error messages after 2s
const hideTempMessage = () => {
    document.querySelectorAll(".toast-alert").forEach(alert => {
        setTimeout(() => {
            alert.style.transition = "opacity 0.5s ease";
            alert.style.opacity = "0";
            setTimeout(() => alert.remove(), 500);
        }, 2000);
    });
};

const showTempMessage = (id) => {
    const alert = document.getElementById(id);
    if (!alert) return;
    alert.style.opacity = "1";
    setTimeout(() => {
        alert.style.opacity = "0";
    }, 2000);
};

// ========= Confirm delete =========
const confirmDelete = (deleteUrl) => {
    Swal.fire({
        title: "Are you sure?",
        text: "Do you want to delete this record?",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#d33",
        cancelButtonColor: "#3085d6",
        confirmButtonText: "Yes, delete it!",
        cancelButtonText: "Cancel"
    }).then(result => {
        if (result.isConfirmed) window.location.href = deleteUrl;
    });
};

// ========= Reload with SweetAlert =========
const reloadWithAlertAsync = async (success, message) => {
    if (success) {
        await Swal.fire({
            title: "Success!",
            text: message,
            icon: "success",
            showConfirmButton: false,
            timer: 1000,
            timerProgressBar: true,
            allowOutsideClick: false,
            allowEscapeKey: false,
            willClose: () => window.location.reload()
        });
    } else {
        await Swal.fire({
            title: "Error!",
            text: message,
            icon: "error",
            showConfirmButton: false,
            timer: 3000,
            timerProgressBar: true,
            allowOutsideClick: false,
            allowEscapeKey: false
        });
    }
};
