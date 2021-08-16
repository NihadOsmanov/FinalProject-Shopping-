//Global Search

$(document).ready(function () {
    let search;

    $(document).on("keyup", "#search-home-input", function () {
        search = $(this).val().trim();
        $(`#home-search #global-search`).remove();
        console.log("ok")
        if (search.length > 0) {
            $.ajax({
                url: '/Home/Search?search=' + search,
                type: "Get",
                success: function (res) {
                    $(`#home-search`).append(res)
                }
            });
        }
    })
})


$(document).ready(function () {
    $(document).on("click", "#AddBasket", function () {
        var productId = $(this).next().val();
            $.ajax({
                url: '/Home/AddToBasket?id=' + productId,
                type: "Get",
                success: function (res) {
                    $(".rounded-circle").empty();
                    $(".rounded-circle").append(res);
                }
            });
    })
})

$(document).ready(function () {
    $(document).on("click", "#increase", function () {
        var productId = $(this).val();
        $.ajax({
            url: '/Home/Increase?id=' + productId,
            type: "Get",
            success: function (res) {
                $("#cart_basket").empty();
                $("#cart_basket").append(res);
                console.log(res);
            }
        });
    })
})

$(document).ready(function () {
    $(document).on("click", "#decrease", function () {
        var productId = $(this).val();
        $.ajax({
            url: '/Home/Decrease?id=' + productId,
            type: "Get",
            success: function (res) {
                $("#cart_basket").empty();
                $("#cart_basket").append(res);
            }
        });
    })
})

$(document).ready(function () {
    $(document).on("click", "#remove", function () {
        var productId = $(this).next().val();
        $.ajax({
            url: '/Home/RemoveAll?id=' + productId,
            type: "Get",
            success: function (res) {
                $("#cart_basket").empty();
                $("#cart_basket").append(res);
            }
        });
    })
})


//Shop Search

$(document).ready(function () {
    let search;

    $(document).on("keyup", "#shopSearch", function () {
        search = $(this).val().trim();
        $("#new-shop").empty()

        //$(`#new-shop #shop-product-search`).remove();
        if (search.length > 0) {
            $.ajax({
                url: '/Shop/Search?search=' + search,
                type: "Get",
                success: function (res) {
                    $("#old-courses").css("display", "none")
                    $(`#new-shop`).append(res)
                }
            });
        }
        else {
            $("#old-courses").css("display", "block")
        }
    })
})

//Subscriber

$(document).ready(function () {
    let subscriber;
    $(document).on("click", `#btn-subs`, function () {

        $("#span-subs").empty();

        if (isAuthenticated = false) {
            subscriber = $("#inp-subs").val();

            $.ajax({
                url: "Home/Subscriber?email=" + subscriber,
                type: "Get",
                success: function (res) {
                    $("#span-subs").append(res);
                }
            });
        }
        else {
            $.ajax({
                url: "Home/Subscriber?email=",
                type: "Get",
                success: function (res) {
                    $("#span-subs").append(res);
                }
            });
        }

    });
});

// Comments

let name;
let email;
let message;
function comment(path) {
    $(document).on('click', `.reply-btn`, function (e) {
        e.preventDefault();
        if (isAuthenticated) {
            message = $(`.message`).val();

            $.ajax({
                url: `${path}`,
                data: {
                    "name": "",
                    "email": "",
                    "message": message
                },
                type: "Post",
                success: function (res) {
                    $(`#comment-list`).append(res)
                    $(`.message`).val("");
                }
            });
        }
        else {
            name = $(`.name`).val();
            email = $(`.email`).val();
            message = $(`.message`).val();

            $.ajax({
                url: `${path}`,
                data: {
                    "name": name,
                    "email": email,
                    "message": message
                },
                type: "Post",
                success: function (res) {
                    $(`#comment-list`).append(res)
                    $(`.name`).val("");
                    $(`.email`).val("");
                    $(`.message`).val("");
                }
            });
        }
    })
}

if ($("#reply-button").hasClass("reply-btn")) {
    comment("/Blog/BlogComment/");
}

// Send Mesage

$(document).on('click', `#buttonCon`, function () {
    $(`#buttonCon`).prop("disabled", true);
    $("#ConEmailError").empty();
    $("#ConSubjectError").empty();
    $("#ConMessageError").empty();
    $("#ConNameError").empty();
    $("#Form-contact").empty();
    let inputMessageCon = $("#Message-contact").val();
    let inputSubjectCon = $("#Subject-contact").val();
    if (inputMessageCon == 0) {
        let conNullMessage = "Message can't be empty !!!";
        $("#ConMessageError").append(conNullMessage);
        $(`#buttonCon`).removeProp("disabled")
        console.log("ok1")
    }
    if (inputSubjectCon == 0) {
        let conNullSubject = "Subject can't be empty !!!";
        $("#ConSubjectError").append(conNullSubject);
        $(`#buttonCon`).removeProp("disabled")
    }
    if (isAuthenticated) {
        console.log(inputSubjectCon);
        $.ajax({
            url: `/Contact/MessageToMe/`,
            data: {
                "Subject": inputSubjectCon,
                "Message": inputMessageCon,
            },
            type: "Post",
            success: function (res) {
                console.log(res)
                $("#Form-contact").append(res);

                $(`#buttonCon`).removeProp("disabled")

                $("#Message-contact").val("");
                $("#Subject-contact").val("");
            }
        });
    }
    else {
        let inputEmailCon = $("#Email-contact").val();
        let inputNameCon = $("#Name-contact").val();

        if (inputNameCon == 0) {
            let conNullName = "Name can't be empty !!!";
            $("#ConNameError").append(conNullName);
            $(`#buttonCon`).removeProp("disabled")
        }
        if (ValidateEmail(inputEmailCon) == true) {
            $.ajax({
                url: `/Contact/MessageToMe/`,
                data: {
                    "Name": inputNameCon,
                    "Email": inputEmailCon,
                    "Subject": inputSubjectCon,
                    "Message": inputMessageCon,
                },
                type: "Post",
                success: function (res) {
                    console.log(res)
                    $("#Form-contact").append(res);
                    $("#Message-contact").val("");
                    $("#Email-contact").val("");
                    $("#Name-contact").val("");
                    $("#Subject-contact").val("");
                    $(`#buttonCon`).removeProp("disabled")

                }
            });

        } else {
            let conNullError = "Please write Email !!!";
            $("#ConEmailError").append(conNullError);
            $(`#buttonCon`).removeProp("disabled")

        }
    }
})

$(document).ready(function () {
    let arr = [];

    $(document).on("click", ".random", function () {
        let productId = $(this).attr("id")
        if (arr.includes(productId)) {
            const index = arr.indexOf(productId);
            if (index > -1) {
                arr.splice(index, 1);
            }
        } else {
            arr.push(productId);
        }

        let test = JSON.stringify(arr);

        $.ajax({
            url: "shop/filtering?productId=" + test,
            type: "Post",
            success: function (res) {
                console.log(res);
            }
        });
    })
})

//Email Validate
function ValidateEmail(email) {
    var mailformat = /^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$/;

    if (email.match(mailformat)) {
        return true;
    }
    else {
        return false;
    }
}