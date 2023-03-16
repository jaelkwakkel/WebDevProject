let captcha;

let uniquechar = "";

function generate() {

    // Clear old input
    document.getElementById("CaptchaText").value = "";

    uniquechar = "";
    // Access the element to store
    // the generated captcha
    captcha = document.getElementById("image");
    const randomchar = "0123456789";

    // Generate captcha for length of
    // 5 with random character
    for (let i = 0; i < 6; i++) {
        uniquechar += randomchar.charAt(Math.random() * randomchar.length)
    }

    const ctx = captcha.getContext("2d");
    ctx.font = "40px Roboto";
    ctx.fillStyle = "#a0a0f0";
    ctx.clearRect(0, 0, captcha.width, captcha.height);
    ctx.fillText(uniquechar, captcha.width / 4, captcha.height / 2);
}

const form = document.querySelector("form");

form.addEventListener("submit", async (event) => {
    if (!checkCaptcha()) {
        event.preventDefault();
        document.getElementById("IncorrectCaptchaWarning").innerHTML = "Incorrect captcha";
        generate();
    }
})

function checkCaptcha() {
    const usr_input = document.getElementById("CaptchaText").value;
    // Check whether the input is equal
    // to generated captcha or not
    return usr_input === uniquechar;
}
