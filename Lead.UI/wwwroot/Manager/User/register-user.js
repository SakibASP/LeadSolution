
const passwordInput = document.getElementById('Input_Password');
const passwordError = document.getElementById('PasswordError');
document.addEventListener('DOMContentLoaded', function () {
    if (window.innerWidth > 768) {
        document.querySelector('.sidebar-toggle').click();
    }
    passwordInput.addEventListener("focusout", function () {
        PasswordValidatiion();
    })
});

document.getElementById('registerForm').addEventListener('submit', function (event) {
    event.preventDefault(); // Prevent the form from submitting for validation
    // Validation
    if (PasswordValidatiion()) {
        this.submit();  // Use the form's submit method to submit it
    }
});

const PasswordValidatiion = function () {
    var IsValid = true;
    // Regular expression to validate the password
    const lowerCasePattern = /[a-z]/;  // Check for at least one lowercase letter
    const upperCasePattern = /[A-Z]/;  // Check for at least one uppercase letter
    const nonAlphanumericPattern = /[^a-zA-Z0-9]/;  // Check for at least one non-alphanumeric character

    if (!lowerCasePattern.test(passwordInput.value)) {
        passwordError.textContent = "Password must contain at least one lowercase letter.";
        IsValid = false;
    } else if (!upperCasePattern.test(passwordInput.value)) {
        passwordError.textContent = "Password must contain at least one uppercase letter.";
        IsValid = false;
    } else if (!nonAlphanumericPattern.test(passwordInput.value)) {
        passwordError.textContent = "Password must contain at least one non-alphanumeric character.";
        IsValid = false;
    } else {
        passwordError.textContent = "";  // Clear error message if validation passes
        IsValid = true;
    }
    return IsValid;
}