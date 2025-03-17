$(function () {
    // Product add functionality
    $("#productPhotoFile").on('change', function () {
        readURL(this);
        uploadImage();
    });

    $("#isAvailableId").on('click', function () {
        if ($(this).is(":checked")) {
            $(this).val("true");
        } else {
            $(this).val("false");
        }
    });

    $("#addProductButton").on('click', function () {
        class Product {
            constructor(productName, addDate, price, isAvailable, imageUrl, modelId) {
                this.CustName = productName;
                this.DOB = addDate;
                this.Balance = price;
                this.IsActive = isAvailable;
                this.ImageUrl = imageUrl;
                this.TranID = modelId;
            }
        };

        class Prods {
            constructor() {
                this.products = [];
            }

            newProduct(productName, addDate, price, isAvailable, imageUrl, modelId) {
                let prod = new Product(productName, addDate, price, isAvailable, imageUrl, modelId);
                this.products.push(prod);
                return prod;
            }

            get allProducts() {
                return this.products;
            }
        };

        class Category {
            constructor(categoryName) {
                this.AccType = categoryName;
                this.Customers;
            }

            addProducts(prods) {
                this.Customers = prods.allProducts;
            }
        };

        let aProduct = new Product($("#productNameId").val(),
            $("#productDateId").val(), $("#productPriceId").val(), $("#isAvailableId").val(), $("#productPhotoUrl").val(), $("#modelSelect").val());
        console.log({ aProduct });

        if (String($("#productsDiv").html()).trim().length == 0) {
            let aCategory = new Category($("#AccType").val());
            console.log(aCategory);
            console.log(aProduct);
            $.ajax({
                method: "POST",
                url: "PostForViewComponent",
                data: {
                    category: aCategory,
                    product: aProduct
                },
                success: function (result) {
                    $("#productsDiv").html(result);
                }
            });
        } else {
            let productList = new Prods();
            $.each($(".visually-hidden"), function (index, prodDiv) {
                productList.newProduct($(prodDiv).find("[name='Customers[" + index + "].CustName']").val(),
                    $(prodDiv).find("[name='Customers[" + index + "].DOB']").val(),
                    $(prodDiv).find("[name='Customers[" + index + "].Balance']").val(),
                    $(prodDiv).find("[name='Customers[" + index + "].IsActive']").val(),
                    $(prodDiv).find("[name='Customers[" + index + "].ImageUrl']").val(),
                    $(prodDiv).find("[name='Customers[" + index + "].TranID']").val());
            });
            console.log(productList);
            let theCategory = new Category($("#AccType").val());
            theCategory.addProducts(productList);
            console.log(theCategory);
            $.ajax({
                method: "POST",
                url: "PostForViewComponent",
                data: {
                    category: theCategory,
                    product: aProduct
                },
                success: function (result) {
                    $("#productsDiv").html(result);
                }
            });
        }
    });

    // Handle file selection in edit form
    $(document).on('change', '#editProductPhotoFile', function () {
        readEditURL(this);
        // Wait for the preview to load before uploading
        uploadEditImage();
    });

    // Product update functionality
    $(document).on('click', '#updateProductBtn', function () {
        console.log("Update button clicked");

        // First check if there's a file selected but not yet uploaded
        if ($("#editProductPhotoFile")[0].files.length > 0 && !$("#editProductPhotoUrl").val()) {
            // Upload the image first, then submit when done
            uploadEditImage(function () {
                submitEditProductForm();
            });
        } else {
            // No new file or already uploaded, proceed with form submission
            submitEditProductForm();
        }
    });

    // Ready function for edit buttons
    $(".edit-product-btn").click(function () {
        console.log("Edit button clicked");
        $(".productEditSection").show();
    });
});

// Add/Edit helper functions
function readURL(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            $('#productPhoto').attr('src', e.target.result);
        }
        reader.readAsDataURL(input.files[0]);
    }
}

function uploadImage() {
    var data = new FormData();
    data.append("file", $("#productPhotoFile")[0].files[0]);

    $.ajax({
        url: '/Account/UploadImage', // Use absolute path
        type: 'POST',
        data: data,
        processData: false,
        contentType: false,
        success: function (res) {
            $("#productPhotoUrl").val(res);
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.error("Upload error:", textStatus, errorThrown, jqXHR.responseText);
            alert('Error on upload: ' + textStatus);
        }
    });
}

function removeMe(el) {
    $(el).parent().parent().parent().remove();
}

function editProduct(el, productId) {
    // Get the closest product card
    let productCard = $(el).closest('.card');
    console.log("Editing product:", productId);

    // Load the edit form via AJAX
    $.ajax({
        url: '/Categories/GetProductForEdit',
        type: 'GET',
        data: { productId: productId },
        success: function (result) {
            console.log("Edit form loaded");
            $("#editProductPartial").html(result);

            // Scroll to the edit form on mobile
            if (window.innerWidth < 768) {
                $('html, body').animate({
                    scrollTop: $("#editProductPartial").offset().top - 20
                }, 500);
            }
        },
        error: function (xhr, status, error) {
            console.error("AJAX Error:", status, error);
            console.log("Response:", xhr.responseText);
            alert('Error loading product edit form');
        }
    });
}

function readEditURL(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            $('#editProductPhoto').attr('src', e.target.result);
        }
        reader.readAsDataURL(input.files[0]);
    }
}

function uploadEditImage(callback) {
    var fileInput = $("#editProductPhotoFile")[0];
    if (!fileInput || fileInput.files.length === 0) {
        // No file selected, just call callback if provided
        if (callback) callback();
        return;
    }

    var data = new FormData();
    data.append("file", fileInput.files[0]);

    $.ajax({
        url: '/Categories/UploadImage', // Use absolute path with leading slash
        type: 'POST',
        data: data,
        processData: false,
        contentType: false,
        success: function (res) {
            console.log("Image uploaded successfully:", res);
            $("#editProductPhotoUrl").val(res);
            // Call the callback function if provided
            if (callback) callback();
        },
        error: function (xhr, status, error) {
            console.error("Image upload error:", status, error);
            console.log("Response:", xhr.responseText);
            alert('Error uploading image: ' + status);
        }
    });
}

function submitEditProductForm() {
    // Make sure the checkbox value is set correctly
    if ($('#editIsAvailable').is(':checked')) {
        $('#editIsAvailable').val('true');
    } else {
        $('#editIsAvailable').val('false');
    }

    // Get form data
    var formData = {
        ProductId: $('#editProductId').val(),
        CategoryId: $('#editCategoryId').val(),
        ProductName: $('#editProductName').val(),
        Price: $('#editProductPrice').val(),
        AddDate: $('#editProductDate').val(),
        IsAvailable: $('#editIsAvailable').is(':checked'),
        ImageUrl: $('#editProductPhotoUrl').val(),
        ModelId: $('#editModelSelect').val()
    };

    console.log("Submitting form data:", formData);

    // Send update request
    $.ajax({
        url: '/Categories/UpdateProduct',
        type: 'POST',
        data: formData,
        success: function (result) {
            // Refresh product list
            $("#productsDiv").html(result);
            // Clear edit form
            $('#editProductPartial').html('');
            // Show success message
            alert('Product updated successfully');
        },
        error: function (xhr, status, error) {
            console.error("Update error:", status, error);
            console.log("Response:", xhr.responseText);
            alert('Error updating product: ' + status);
        }
    });
}

function cancelEdit() {
    // Clear edit form
    $('#editProductPartial').html('');
}