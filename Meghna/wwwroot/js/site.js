// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


const GetSingleProduct = (productId) => {
    $.ajax({
        type: "GET",
        dataType: "json",
        url: '@Url.ActionLink("GetSingleProduct","Product")',
        data: { productId: productId },
        success: (result) => {

        },
        error: (err) => {

        }
    });
}
