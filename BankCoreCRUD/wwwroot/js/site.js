// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(function () {
    $("#productPhotoFile").on('change', function () {
        readURL(this);
        uploadImage();
    });

    //$("#productPhoto").on('change', function () {
    //    uploadImage();
    //});

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
                this.ProductName = productName;
                this.AddDate = addDate;
                this.Price = price;
                this.IsAvailable = isAvailable;
                this.ImageUrl = imageUrl;
                this.ModelId = modelId;
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
                this.CategoryName = categoryName;
                this.Products;
            }

            addProducts(prods) {
                this.Products = prods.allProducts;
            }
        };

        let aProduct = new Product($("#productNameId").val(), $("#productDateId").val(), $("#productPriceId").val(), $("#isAvailableId").val(), $("#productPhotoUrl").val(), $("#modelSelect").val());

        if (String($("#productsDiv").html()).trim().length == 0) {
            let aCategory = new Category($("#CategoryName").val());

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
                productList.newProduct($(prodDiv).find("[name='Products[" + index + "].ProductName']").val(), $(prodDiv).find("[name='Products[" + index + "].AddDate']").val(), $(prodDiv).find("[name='Products[" + index + "].Price']").val(), $(prodDiv).find("[name='Products[" + index + "].IsAvailable']").val(), $(prodDiv).find("[name='Products[" + index + "].ImageUrl']").val(), $(prodDiv).find("[name='Products[" + index + "].ModelId']").val());
            });
            console.log(productList);
            let theCategory = new Category($("#CategoryName").val());
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
});

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
        url: 'UploadImage',
        type: 'POST',
        data: data,
        /*cache: false,*/
        /*dataType: 'json',*/
        processData: false,
        contentType: false,
        success: function (res) {
            $("#productPhotoUrl").val(res);
        },
        error: function (jqXHR, textStatus, errorThrown) { alert('error on upload'); }

    });
}

function removeMe(el) {
    $(el).parent().parent().parent().remove();
}