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

// Write your JavaScript code.
document.addEventListener("DOMContentLoaded", (event) => {
    typeWriting(event);
});

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
