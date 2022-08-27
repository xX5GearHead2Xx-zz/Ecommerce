function AcceptCookies() {
    var Expiry = new Date();
    Expiry.setDate(Expiry.getDate() + 30);//Always 30 days
    console.log(Expiry);
    document.cookie = "CookiesAccepted=True;expires=" + Expiry.toUTCString() + ";path=/";
    location.reload();
}

function getCookie(cname) {
    let name = cname + "=";
    let ca = document.cookie.split(';');
    for (let i = 0; i < ca.length; i++) {
        let c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}

$(document).ready(function () {
    let Cookies = getCookie("CookiesAccepted");
    if (Cookies != "") {
        //User already accepted cookies
    } else {
        var CookieModal = new bootstrap.Modal(document.getElementById("CookieModal"), {});
        CookieModal.show();
        
    }
})