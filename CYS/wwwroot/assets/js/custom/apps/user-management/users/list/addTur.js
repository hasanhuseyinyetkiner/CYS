"use strict";

// Class definition
var KTUsersAddUser = function () {
    // Shared variables
    const element = document.getElementById('kt_modal_add_user_tur');
    const form = element.querySelector('#kt_modal_add_user_form_tur');
    const modal = new bootstrap.Modal(element);

    // Init add schedule modal
    var initAddUser = () => {

        // Init form validation rules. For more info check the FormValidation plugin's official documentation:https://formvalidation.io/
        var validator = FormValidation.formValidation(
            form,
            {
                fields: {
                    'ustKategoriName': {
                        validators: {
                            notEmpty: {
                                message: 'Kategori İsmini Girin...'
                            }
                        }
                    },
                    
                },

                plugins: {
                    trigger: new FormValidation.plugins.Trigger(),
                    bootstrap: new FormValidation.plugins.Bootstrap5({
                        rowSelector: '.fv-row',
                        eleInvalidClass: '',
                        eleValidClass: ''
                    })
                }
            }
        );
        // Submit button handler
        const submitButton = element.querySelector('[data-kt-users-modal-action-tur="submit"]');
        submitButton.addEventListener('click', e => {
            e.preventDefault();

            // Validate form before submit
            if (validator) {
                validator.validate().then(function (status) {
                    console.log('validated!');

                    if (status == 'Valid') {
                        // Show loading indication
                        submitButton.setAttribute('data-kt-indicator', 'on');

                        // Disable button to avoid multiple click 
                        submitButton.disabled = true;

                        // Simulate form submission. For more info check the plugin's official documentation: https://sweetalert2.github.io/
                        setTimeout(function () {
                            // Remove loading indication
                            submitButton.removeAttribute('data-kt-indicator');

                            // Enable button
                            submitButton.disabled = false;
                            $.ajax({
                                url: "/UstKategori/ustTurEkleJson/",
                                type: "POST",
                                data: {
                                    "ustKategoriAdi": form.querySelector('[name="ustKategoriName"]').value
                                },
                                success: function (returnData) {

                                    if (returnData["status"] == "Error") {
                                        // Show error popup. For more info check the plugin's official documentation: https://sweetalert2.github.io/
                                        Swal.fire({
                                            text: returnData["message"],
                                            icon: "error",
                                            buttonsStyling: false,
                                            confirmButtonText: "Tekrar Eklemeyi dene!",
                                            customClass: {
                                                confirmButton: "btn btn-primary"
                                            }
                                        });

                                    }
                                    else {
                                        Swal.fire({
                                            text: returnData["message"],
                                            icon: "success",
                                            buttonsStyling: false,
                                            customClass: {
                                                confirmButton: "btn btn-primary"
                                            }
                                        }).then(function (result) {
                                            if (result.isConfirmed) {
                                                form.querySelector('[name="email"]').value = "";
                                                form.querySelector('[name="password"]').value = "";
                                                //form.submit(); // submit form
                                            }
                                        });
                                        // Your application has indicated there's an error
                                        window.setTimeout(function () {

                                            // Move to a new location or you can do something else
                                            window.location.href = "../../UstKategori/UstKategoriListesi";

                                        }, 1500);
                                    }
                                },
                                beforeSend: function () {
                                    $(".loaderDiv").show();
                                    $(".loader").show();
                                },
                                complete: function () {
                                    $(".loaderDiv").hide();
                                    $(".loader").hide();
                                },
                                error: function (xhr, status, err) {
                                    if (xhr.status === 999) {
                                        noAuthorize(this.url);
                                    }
                                }
                            });



                            //form.submit(); // Submit form
                        }, 2000);
                    }
                });
            }
        });

        // Cancel button handler
        const cancelButton = element.querySelector('[data-kt-users-modal-action-tur="cancel"]');
        cancelButton.addEventListener('click', e => {
            e.preventDefault();

            Swal.fire({
                text: "Iptal Etmek Istediginize Emin misiniz?",
                icon: "warning",
                showCancelButton: true,
                buttonsStyling: false,
                confirmButtonText: "Evet!",
                cancelButtonText: "Hayır, geri don!",
                customClass: {
                    confirmButton: "btn btn-primary",
                    cancelButton: "btn btn-active-light"
                }
            }).then(function (result) {
                if (result.value) {
                    form.reset(); // Reset form			
                    modal.hide();
                } else if (result.dismiss === 'cancel') {
                    Swal.fire({
                        text: "Formunuz Iptal Edildi.",
                        icon: "error",
                        buttonsStyling: false,
                        confirmButtonText: "Tamam!",
                        customClass: {
                            confirmButton: "btn btn-primary",
                        }
                    });
                }
            });
        });

        // Close button handler
        const closeButton = element.querySelector('[data-kt-users-modal-action-tur="close"]');
        closeButton.addEventListener('click', e => {
            e.preventDefault();

            Swal.fire({
                text: "Iptal Etmek Istediginize Emin misiniz?",
                icon: "warning",
                showCancelButton: true,
                buttonsStyling: false,
                confirmButtonText: "Evet Iptal Et!",
                cancelButtonText: "Hayir Geri Don",
                customClass: {
                    confirmButton: "btn btn-primary",
                    cancelButton: "btn btn-active-light"
                }
            }).then(function (result) {
                if (result.value) {
                    form.reset(); // Reset form			
                    modal.hide();
                } else if (result.dismiss === 'cancel') {
                    Swal.fire({
                        text: "Formunuz Iptal Edildi.",
                        icon: "error",
                        buttonsStyling: false,
                        confirmButtonText: "Anladim",
                        customClass: {
                            confirmButton: "btn btn-primary",
                        }
                    });
                }
            });
        });
    }

    return {
        // Public functions
        init: function () {
            initAddUser();
        }
    };
}();

// On document ready
KTUtil.onDOMContentLoaded(function () {
    KTUsersAddUser.init();
});