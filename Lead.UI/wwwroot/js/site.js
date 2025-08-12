// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

$(function () {
    if ($(window).width() < 768) {
        $('.sidebar .collapse').collapse('hide');
    };

    // Toggle the side navigation when window is resized below 480px
    if ($(window).width() < 480 && !$(".sidebar").hasClass("toggled")) {
        $("body").addClass("sidebar-toggled");
        $(".sidebar").addClass("toggled");
        $('.sidebar .collapse').collapse('hide');
    };
});


//$(function () {
//    const sidebarKey = 'sidebar-toggled';

//    // 1. Always start minimized if no preference
//    if (!localStorage.getItem(sidebarKey)) {
//        localStorage.setItem(sidebarKey, 'true'); // true = minimized
//    }

//    // 2. Apply the state on load
//    const isToggled = localStorage.getItem(sidebarKey) === 'true';
//    toggleSidebar(isToggled);

//    // 3. Toggle on button click and update localStorage
//    $('#sidebarToggle').on('click', function () {
//        const isCurrentlyToggled = $('body').hasClass('sidebar-toggled');
//        toggleSidebar(!isCurrentlyToggled);
//        localStorage.setItem(sidebarKey, !isCurrentlyToggled);
//    });

//    // Helper function to toggle
//    function toggleSidebar(toggled) {
//        if (toggled) {
//            $('body').addClass('sidebar-toggled');
//            $('.sidebar').addClass('toggled');
//            $('.sidebar .collapse').collapse('hide');
//        } else {
//            $('body').removeClass('sidebar-toggled');
//            $('.sidebar').removeClass('toggled');
//            $('.sidebar .collapse').collapse('show');
//        }
//    }
//});


// Write your JavaScript code.
document.addEventListener("DOMContentLoaded", (event) => {
    typeWriting(event);
    hideTempMessage();
});

// Hiding the success or error message after 2 seconds
const hideTempMessage = () => {
    const alerts = document.querySelectorAll(".toast-alert");
    alerts.forEach(alert => {
        setTimeout(() => {
            alert.style.transition = "opacity 0.5s ease";
            alert.style.opacity = "0";
            setTimeout(() => alert.remove(), 500); // Remove after fade
        }, 2000); // Show for 2 seconds
    });
}

// Hiding the success or error message after 2 seconds
const showTempMessage = (id) => {
    const alert = document.getElementById(id);
    if (alert) {
        alert.style.opacity = "1";
        setTimeout(() => {
            alert.style.opacity = "0";
        }, 2000);
    }
}

//Type writing element
const typeWriting = (e) => {
    e.preventDefault();
    const heading = document.getElementById("typewriting");
    if (heading) {
        const text = heading.textContent; // Text you want to display
        let delay = 150; // Adjust typing speed (milliseconds)
        let index = 0;

        heading.textContent = "";
        const typeWriter = () => {
            if (index < text.length) {
                heading.innerHTML += text.charAt(index);
                index++;
                setTimeout(typeWriter, delay); // Adjust typing speed (milliseconds)
            }
        }

        typeWriter();
    }
}

//Payment type change effect
const paymentType = document.getElementById("PaymentId");
//alert(paymentType);
if (paymentType != null) {
    paymentType.addEventListener("change", (e) => {
        $('#SubPaymentId').empty(null);
        $('#SubPaymentId').append(
            '<option disabled selected>' +
            'Please select...' +
            '</option>'
        );
        GetSubPaymentTypes(paymentType.value);
        e.preventDefault();
    });
}

const GetSubPaymentTypes = async (typeId) => {
    try {
        const reqUrl = `/Home/GetSubPaymentTypes?typeId=${typeId}`;
        const response = await fetch(reqUrl, {
            method: "GET",
            headers: {
                "Content-Type": "application/json"
            }
        });
        if (!response.ok) {
            throw new Error('Network response was not ok');
        }
        const data = await response.json();
        data.forEach(value => {
            $('#SubPaymentId').append(
                '<option value="' + value.Id + '">' +
                value.TypeName +
                '</option>'
            );
        });
    } catch (error) {
        console.error('There was a problem with the fetch operation:', error);
    }
}

//Payment type change effect
const customerId = document.getElementById("CustomerId");
//alert(paymentType);
if (customerId != null) {
    customerId.addEventListener("change", (e) => {
        $('#SubscribeId').empty(null);
        GetSubscriptions(customerId.value);
        e.preventDefault();
    });
}

const GetSubscriptions = async (customerId) => {
    try {
        const reqUrl = `/Home/GetSubscriptions?customerId=${customerId}`;
        const response = await fetch(reqUrl, {
            method: "GET",
            headers: {
                "Content-Type": "application/json"
            }
        });
        if (!response.ok) {
            throw new Error('Network response was not ok');
        }
        const data = await response.json();
        data.forEach(value => {
            $('#SubscribeId').append(
                '<option value="' + value.Id + '">' +
                value.Name +
                '</option>'
            );
        });
    } catch (error) {
        console.error('There was a problem with the fetch operation:', error);
    }
}

const confirmDelete = (deleteUrl) => {
    Swal.fire({
        title: 'Are you sure?',
        text: "Do you want to delete this record?",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#3085d6',
        confirmButtonText: 'Yes, delete it!',
        cancelButtonText: 'Cancel'
    }).then((result) => {
        if (result.isConfirmed) {
            window.location.href = deleteUrl;
        }
    });
}


const reloadWithAlertAsync = async (success, message) => {
    if (success) {
        const result = await Swal.fire({
            title: 'Success!',
            text: message,
            icon: 'success',
            showConfirmButton: false,  // no OK button
            timer: 1000,               // auto close after 2 sec
            timerProgressBar: true,
            allowOutsideClick: false,
            allowEscapeKey: false,
            willClose: () => {
                window.location.reload();
            }
        });
    } else {
        await Swal.fire({
            title: 'Error!',
            text: message,
            icon: 'error',
            showConfirmButton: false,  // no OK button
            timer: 3000,               // longer time for error
            timerProgressBar: true,
            allowOutsideClick: false,
            allowEscapeKey: false
        });
    }
};


