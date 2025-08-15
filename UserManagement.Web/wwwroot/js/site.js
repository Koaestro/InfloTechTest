// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function confirmThenSubmit(event, msg) {
    event.preventDefault();

    return Swal.fire({
        title: "Confirmation",
        text: msg,
        showCancelButton: true,
        confirmButtonText: "Confirm",
    }).then((result) => {
        if (result.isConfirmed) {
            event.target.submit();
        }
    });
}
