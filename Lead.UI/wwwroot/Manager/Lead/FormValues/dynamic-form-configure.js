const drpBusinessId = document.getElementById('drpBusinessId');
const businessId = document.getElementById('BusinessId');
const txtApiKey = document.getElementById("txtApiKey");
const apiKey = document.getElementById("ApiKey");
const embedCode = document.getElementById("EmbedCode");
const dynamicForm = document.getElementById('dynamicForm');
const btnGenerateCode = document.getElementById('btnGenerateCode');

document.addEventListener('DOMContentLoaded', function () {
    onGenerateCodeClick();
    populateBusinessIdAndApiKey();
});

// ============ on page load and button click events start =============
const onGenerateCodeClick = () => {
    btnGenerateCode.addEventListener('click', function () {
        const formContainer = document.querySelector('.form-container');
        if (!formContainer || getComputedStyle(formContainer).display === 'none') {
            // Form is hidden, show SweetAlert
            Swal.fire({
                icon: 'warning',
                title: '⚠️ No theme selected',
                text: 'Please select a theme to display it first.',
                confirmButtonText: 'OK'
            });
            return; // prevent modal
        }
        // If form is visible, open modal manually
        const modal = new bootstrap.Modal(document.getElementById('embedCodeModal'));
        modal.show();
    });
}

const populateBusinessIdAndApiKey = () => {
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
}

const setFormTheme = (themeClass, btn) => {
    // Remove existing theme classes
    dynamicForm.classList.remove('dark-theme', 'white-theme', 'transparent-theme');

    // Add the selected theme
    dynamicForm.classList.add(themeClass);

    // Make form visible (if it was hidden)
    dynamicForm.style.display = 'block';
    // Highlight active button
    document.querySelectorAll('.theme-btn').forEach(b => b.classList.remove('theme-active'));
    btn.classList.add('theme-active');
}

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
// ============ on page load and button click events end ===============



// ============ api calls start ===========
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
// ============ api calls end =============



// ============ sweet alerts start ================
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
// ============ sweet alerts end ==================



//================ Generate Embeddable Form Code =================
const generateEmbeddableForm = () => {
    if (!businessId || businessId.value == '') {
        businessNotFound();
        return;
    }
    if (!apiKey || apiKey.value == '') {
        apiKeyNotFound();
        return;
    }

    const clonedForm = dynamicForm.cloneNode(true);

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

    // Set ID for submission handler
    clonedForm.id = 'embeddedForm';

    //CSS for form design
    const cssDesign = `
        <link href="https://localhost:7131/css/lead/formvalues/dynamic-form.css" rel="stylesheet" />
        <link href="https://cdn.jsdelivr.net/npm/sweetalert2@11/dist/sweetalert2.min.css" rel="stylesheet" />
    `.trim();

    //dropdown load
    let dropdownScript = `
        function initDropdowns() {
            document.querySelectorAll('#myDynamicFormWrapper .dropdown .btn').forEach(btn => {
                btn.addEventListener('click', function(e) {
                    e.stopPropagation();
                    const dropdown = btn.parentElement;
                    const isShown = dropdown.classList.contains('show');

                    document.querySelectorAll('#myDynamicFormWrapper .dropdown').forEach(d => d.classList.remove('show'));
                    if (!isShown) dropdown.classList.add('show');
                });
            });

            document.addEventListener('click', function(e) {
                if (!(e.target.tagName === 'INPUT' && e.target.id.startsWith('drpSearch_'))) {
                    document.querySelectorAll('#myDynamicFormWrapper .dropdown').forEach(d => d.classList.remove('show'));
                }
            });
        }

        async function populateDropdown(index, formDetailId, param1 = null) {
            try {
                const businessId = document.getElementById("BusinessId").value;
                const apiKey = document.getElementById("ApiKey").value;

                const res = await fetch(\`https://localhost:44306/api/v1/utility/get-client-dropdown?Id=\${formDetailId}&param1=\${param1 ?? ""}\`, {
                    method: 'GET',
                    headers: { 'X-Api-Key': apiKey, 'X-Business-Id': businessId },
                });

                if (!res.ok) return;
                const result = await res.json();

                const container = document.getElementById(\`dropdownItems_\${index}\`);
                const selectedSpan = document.getElementById(\`selectedValue_\${index}\`);
                const hiddenInput = document.getElementById(\`hiddenValue_\${index}\`);
                if (!container || !selectedSpan || !hiddenInput) return;

                container.innerHTML = "";

                if (result.isSuccess && Array.isArray(result.data)) {
                    result.data.forEach(item => {
                        const div = document.createElement('div');
                        div.className = 'dropdown-item';
                        div.textContent = item.name;
                        div.dataset.value = item.id;

                        div.addEventListener('click', () => {
                            selectedSpan.textContent = item.name;
                            hiddenInput.value = item.id;
                            container.parentElement.classList.remove('show');

                            if (typeof onDropdownChange === 'function') {
                                onDropdownChange(index, item.id);
                            }
                        });

                        container.appendChild(div);
                    });
                }

                const searchInput = document.getElementById(\`drpSearch_\${index}\`);
                if (searchInput) {
                    searchInput.addEventListener('input', function () {
                        const filter = this.value.toLowerCase();
                        container.querySelectorAll('.dropdown-item').forEach(div => {
                            div.style.display = div.textContent.toLowerCase().includes(filter) ? '' : 'none';
                        });
                    });
                }

            } catch (err) {
                console.error("Error fetching dropdown:", err);
            }
        }

        function onDropdownChange(currentIndex, selectedValue) {
            const nextIndex = currentIndex + 1;
            const formDetailHidden = document.querySelector(\`input[name="Inputs[\${nextIndex}].FormDetailId"]\`);
            if (formDetailHidden) populateDropdown(nextIndex, formDetailHidden.value, selectedValue);
        }

        document.addEventListener("DOMContentLoaded", () => {
            initDropdowns();
        `;
    // dynamically generate populateDropdown calls for all hidden inputs
    const processedIndexes = new Set();
    clonedForm.querySelectorAll("input[type=hidden][name$='.FormDetailId']").forEach((hidden) => {
        const match = hidden.name.match(/Inputs\[(\d+)\]/);
        if (!match) return;

        const index = match[1];

        // Skip if this index is already processed
        if (processedIndexes.has(index)) return;
        processedIndexes.add(index);

        const container = document.getElementById(`dropdownItems_${index}`);
        const selectedSpan = document.getElementById(`selectedValue_${index}`);
        const hiddenInput = document.getElementById(`hiddenValue_${index}`);

        if (!container || !selectedSpan || !hiddenInput) return;

        dropdownScript += `populateDropdown(${index}, ${hidden.value});\n`;
    });
    dropdownScript += "});".trim(); // close DOMContentLoaded

    // form submit script 
    const submitScript = `
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
                        'X-Business-Id': businessId
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
        });`
        .trim();

    // Script for submitting to your API endpoint
    const script = `
        <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
        <script>
            ${submitScript}
            ${dropdownScript}
        </script>
        `.trim();

    // Final embeddable HTML
    let fullHtml = `
        <!-- Start Embedded Contact Form -->
        ${cssDesign}
        <div id="myDynamicFormWrapper">
            ${clonedForm.outerHTML}
        </div>
        ${script}
        <!-- End Embedded Contact Form -->
        `.trim();

    // making minimum version of HTML
    fullHtml = fullHtml.replace(/>\s+</g, '><').replace(/\s+/g, ' ').trim();

    // Output to textarea
    embedCode.value = fullHtml;
}

