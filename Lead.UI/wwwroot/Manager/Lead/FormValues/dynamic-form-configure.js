const drpBusinessId = document.getElementById('drpBusinessId');
const businessId = document.getElementById('BusinessId');
const txtApiKey = document.getElementById("txtApiKey");
const apiKey = document.getElementById("ApiKey");
const embedCode = document.getElementById("EmbedCode");

document.addEventListener('DOMContentLoaded', function () {
    if (drpBusinessId && businessId) {
        // On page load, set hidden input value
        businessId.value = drpBusinessId.value;
        apiKey.value = txtApiKey.value;
        // On dropdown change, update hidden input value
        drpBusinessId.addEventListener('change', function () {
            businessId.value = this.value;
            this.form.submit();
        });
    }
});

const updateFormSettings = async () => {
    if (!businessId || businessId.value == '') {
        businessNotFound();
        return;
    }

    // Collect all checkbox states
    const formSelectDetails = Array.from(document.querySelectorAll('.form-check-input')).map(chk => ({
        FormDetailId: parseInt(chk.id.replace("chk_", "")),
        IsChecked: chk.checked
    }));

    // Prepare the payload
    const payload = {
        BusinessId: parseInt(businessId.value),
        FormSelectDetails: formSelectDetails
    };

    try {
        const response = await fetch('/FormValues/UpdateFormSettings', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(payload)
        });

        if (!response.ok) {
            throw new Error(`HTTP error! Status: ${response.status}`);
        }

        const data = await response.json();
        await reloadWithAlertAsync(data.isSuccess, data.message);

    } catch (err) {
        console.error("Error updating form settings:", err);
        Swal.fire({
            title: 'Error!',
            text: 'Error updating form settings.',
            icon: 'error',
            confirmButtonText: 'OK',
            allowOutsideClick: false,
            allowEscapeKey: false
        });
    }
};


const generateNewApiKey = async () => {
    if (!businessId || businessId.value == '') {
        businessNotFound();
        return;
    }
    const result = await Swal.fire({
        title: 'Generate New API Key?',
        text: "This will replace the existing key and you must generate a new form with the new key.",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, generate it!'
    });

    if (!result.isConfirmed) return;

    try {
        const response = await fetch('/FormValues/GenerateNewApiKey', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(businessId.value)
        });

        if (!response.ok) {
            throw new Error(`HTTP error! Status: ${response.status}`);
        }

        const data = await response.json();

        txtApiKey.value = data.apiKey;
        apiKey.value = data.apiKey;

        await reloadWithAlertAsync(data.isSuccess, data.message);

    } catch (err) {
        console.error(err);
        Swal.fire({
            title: 'Error!',
            text: 'Something went wrong while generating the key.',
            icon: 'error',
            confirmButtonColor: '#3085d6'
        });
    }
};


const copyEmbedCode = () => {
    embedCode.select();
    embedCode.setSelectionRange(0, 99999); // Mobile

    navigator.clipboard.writeText(embedCode.value)
        .then(() => {
            successCopy();
        })
        .catch(() => {
            failedCopy();
        });
}

const copyApiKey = () => {
    txtApiKey.select();
    txtApiKey.setSelectionRange(0, 99999); // For mobile
    navigator.clipboard.writeText(txtApiKey.value)
        .then(() => {
            successCopy();
        })
        .catch(() => {
            failedCopy();
        });
}

const clearEmbedCode = () => {
    embedCode.value = "";
}

const successCopy = () => {
    Swal.fire({
        icon: 'success',
        title: 'Copied!',
        text: '✅ Copied to clipboard!',
        showConfirmButton: false,
        timer: 1500
    });
}

const failedCopy = () => {
    Swal.fire({
        icon: 'error',
        title: 'Oops...',
        text: '❌ Failed to copy',
        showConfirmButton: false,
        timer: 1500
    });
}

const businessNotFound = () => {
    Swal.fire({
        title: 'Warning!',
        text: 'Please select a Business first.',
        icon: 'warning',
        confirmButtonText: 'OK',
        allowOutsideClick: false,
        allowEscapeKey: false
    });
}

const apiKeyNotFound = () => {
    Swal.fire({
        title: 'Warning!',
        text: 'Please generate an API key first.',
        icon: 'warning',
        confirmButtonText: 'OK',
        allowOutsideClick: false,
        allowEscapeKey: false
    });
}

const generateEmbeddableForm = () => {
    if (!businessId || businessId.value == '') {
        businessNotFound();
        return;
    }
    if (!apiKey || apiKey.value == '') {
        apiKeyNotFound();
        return;
    }

    const form = document.getElementById('dynamicForm');
    const clonedForm = form.cloneNode(true);

    // Remove validation attributes
    clonedForm.removeAttribute('asp-action');
    clonedForm.removeAttribute('method');
    clonedForm.removeAttribute('novalidate');

    // Remove Razor-specific and framework-generated attributes
    clonedForm.querySelectorAll('*').forEach(el => {
        [...el.attributes].forEach(attr => {
            if (attr.name.startsWith('asp-') || attr.name.startsWith('data-') || attr.name.startsWith('b-')) {
                el.removeAttribute(attr.name);
            }
        });
    });

    // Excluding Hidden inputs
    //clonedForm.querySelectorAll('input[type="hidden"]').forEach(el => {
    //    if (el.name !== 'BusinessId') {
    //        el.remove();
    //    }
    //});

    // Remove all span.validation messages
    clonedForm.querySelectorAll('span.text-danger').forEach(el => el.remove());

    // Remove the BusinessId dropdown and its label completely
    const businessSelectWrapper = clonedForm.querySelector('.form-group.mb-3');
    if (businessSelectWrapper) businessSelectWrapper.remove();

    // Insert BusinessId as hidden input at top of form
    //const hiddenInput = document.createElement('input');
    //hiddenInput.type = 'hidden';
    //hiddenInput.name = 'BusinessId';
    //hiddenInput.value = businessId.value;
    //clonedForm.prepend(hiddenInput);

    // Set ID for submission handler
    clonedForm.id = 'embeddedForm';

    // Script for submitting to your API endpoint
    const script = `
        <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
        <script>
            document.getElementById('embeddedForm').addEventListener('submit', async function(e) {
                e.preventDefault();
                const formData = new FormData(this);
                const businessId = formData.get("BusinessId");
                const apiKey = formData.get("ApiKey");
                const payload = {
                    BusinessId: parseInt(businessId) || 0,
                    Inputs: []
                };
                formData.forEach((value, key) => {
                    const matchValue = key.match(/^Inputs\\[(\\d+)\\]\\.Value$/);
                    const matchFormDetailId = key.match(/^Inputs\\[(\\d+)\\]\\.FormDetailId$/);
                    if (matchValue) {
                        const index = parseInt(matchValue[1]);
                        if (!payload.Inputs[index]) payload.Inputs[index] = {};
                        payload.Inputs[index].Value = value;
                    }
                    if (matchFormDetailId) {
                        const index = parseInt(matchFormDetailId[1]);
                        if (!payload.Inputs[index]) payload.Inputs[index] = {};
                        payload.Inputs[index].FormDetailId = parseInt(value) || null;
                    }
                });

                console.log("Payload to send:", payload);

                try {
                    const res = await fetch('https://localhost:44306/api/v1/FormValues/add', {
                        method: 'POST',
                        headers: {
                            'Content-Type': 'application/json',
                            'X-Api-Key': apiKey,
                            'X-Business-Id': businessId  // string here
                        },
                        body: JSON.stringify(payload)
                    });

                    if (!res.ok) {
                        let errorMessage = \`Submission failed. Status: \${res.status}\`;
                        try {
                            const errorData = await res.json();
                            if (errorData && errorData.message) {
                                errorMessage = errorData.message;
                            }
                        } catch {
                            // ignore JSON parsing errors
                        }
                        console.error('API error:', errorMessage);
                        await Swal.fire({
                            icon: 'error',
                            title: '❌ Submission failed.',
                            text: errorMessage,
                            confirmButtonText: 'OK'
                        });
                        return;
                    }

                    await Swal.fire({
                        icon: 'success',
                        title: '✅ Submitted successfully!',
                        confirmButtonText: 'OK'
                    });

                } catch (err) {
                    console.error('Network or unexpected error:', err);
                    await Swal.fire({
                        icon: 'error',
                        title: '❌ Submission failed.',
                        text: 'A network or unexpected error occurred. Please try again.',
                        confirmButtonText: 'OK'
                    });
                }
            });
        </script>
`.trim();



    // Final embeddable HTML
    let fullHtml = `
        <!-- Start Embedded Contact Form -->
        <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
            <link href="https://cdn.jsdelivr.net/npm/sweetalert2@11/dist/sweetalert2.min.css" rel="stylesheet" />
        <style>
            body { font-family: sans-serif; padding: 20px; background: #f9f9f9; }
            form { max-width: 600px; margin: 0 auto; }
        </style>
        ${clonedForm.outerHTML}
        ${script}
        <!-- End Embedded Contact Form -->
        `.trim();

    //fullHtml = fullHtml.replace(/>\s+</g, '><').replace(/\s+/g, ' ').trim();

    // Output to textarea
    embedCode.value = fullHtml;
}

