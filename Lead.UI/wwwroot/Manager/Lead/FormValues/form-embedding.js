document.addEventListener('DOMContentLoaded', function () {
    const drpBusinessId = document.getElementById('drpBusinessId');
    const businessId = document.getElementById('BusinessId');

    if (drpBusinessId && businessId) {
        // On page load, set hidden input value
        businessId.value = drpBusinessId.value;
        console.log(businessId.value);
        // On dropdown change, update hidden input value
        drpBusinessId.addEventListener('change', function () {
            console.log(businessId.value);
            businessId.value = this.value;
        });
    }
});


const generateEmbeddableForm = () => {
    const businessId = document.querySelector('[name="BusinessId"]').value;
    if (!businessId) {
        alert("Please select a Business first.");
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
    const hiddenInput = document.createElement('input');
    hiddenInput.type = 'hidden';
    hiddenInput.name = 'BusinessId';
    hiddenInput.value = businessId;
    clonedForm.prepend(hiddenInput);

    // Set ID for submission handler
    clonedForm.id = 'embeddedForm';

    // Script for submitting to your API endpoint
    const script = `
        <script>
        document.getElementById('embeddedForm').addEventListener('submit', async function(e) {
            e.preventDefault();

            const formData = new FormData(this);

            const payload = {
                BusinessId: parseInt(formData.get("BusinessId")) || 0,
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

            const res = await fetch('https://localhost:44306/api/v1/FormValues/add', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(payload)
            });

            alert(res.ok ? '✅ Submitted successfully!' : '❌ Submission failed.');
        });
        <\/script>
        `.trim();


    // Final embeddable HTML
    let fullHtml = `
        <!-- Start Embedded Contact Form -->
        <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
        <style>
            body { font-family: sans-serif; padding: 20px; background: #f9f9f9; }
            form { max-width: 600px; margin: 0 auto; }
        </style>

        ${clonedForm.outerHTML}


        ${script}
        <!-- End Embedded Contact Form -->
        `.trim();

    fullHtml = fullHtml.replace(/>\s+</g, '><').replace(/\s+/g, ' ').trim();

    // Output to textarea
    document.getElementById('embedCode').value = fullHtml;
}



const copyEmbedCode = () => {
    const embedTextarea = document.getElementById("embedCode");
    embedTextarea.select();
    embedTextarea.setSelectionRange(0, 99999); // Mobile

    navigator.clipboard.writeText(embedTextarea.value)
        .then(() => {
            Swal.fire({
                icon: 'success',
                title: 'Copied!',
                text: '✅ Copied to clipboard!',
                showConfirmButton: false,
                timer: 1500
            });
        })
        .catch(() => {
            Swal.fire({
                icon: 'error',
                title: 'Oops...',
                text: '❌ Failed to copy',
                showConfirmButton: false,
                timer: 1500
            });
        });
}


const clearEmbedCode = ()=> {
    document.getElementById("embedCode").value = "";
}