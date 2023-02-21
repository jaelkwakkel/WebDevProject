// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

class GDPR {

    constructor() {
        // this.showStatus();
        this.showContent();
        this.bindEvents();

        if (this.cookieStatus() !== 'accept') this.showGDPR();
    }

    bindEvents() {
        let buttonAccept = document.querySelector('.gdpr-consent__button--accept');
        buttonAccept.addEventListener('click', () => {
            this.cookieStatus('accept');
            // this.showStatus();
            this.showContent();
            this.hideGDPR();
            this.saveDateTime();
        });

        let buttonReject = document.querySelector('.gdpr-consent__button--reject');
        buttonReject.addEventListener('click', () => {
            this.cookieStatus('reject');
            // this.showStatus();
            this.showContent();
            this.hideGDPR();
            this.saveDateTime();
        });
    }


    showContent() {
        this.resetContent();
        const status = this.cookieStatus() == null ? 'not-chosen' : this.cookieStatus();
        const element = document.querySelector(`.content-gdpr-${status}`);
        element.classList.add('show');
    }

    resetContent() {
        const classes = [
            '.content-gdpr-accept',
            '.content-gdpr-reject',
            '.content-gdpr-not-chosen'];

        for (const c of classes) {
            document.querySelector(c).classList.add('hide');
            document.querySelector(c).classList.remove('show');
        }
    }

    showStatus() {
        document.getElementById('content-gpdr-consent-status').innerHTML =
            this.cookieStatus() == null ? 'niet gekozen' : this.cookieStatus();
    }

    cookieStatus(status) {
        if (status) localStorage.setItem('gdpr-consent-choice', status);

//student uitwerking

        return localStorage.getItem('gdpr-consent-choice');
    }


//student uitwerking


    saveDateTime() {
        let currentDateObject = {
            date: new Intl.DateTimeFormat('nl-NL').format(new Date()),
            time: new Date().getHours().toString() + ":" + new Date().getMinutes().toString()
        }
        localStorage.setItem('gdpr-consent-date', JSON.stringify(currentDateObject));
    }

    hideGDPR() {
        document.querySelector(`.gdpr-consent`).classList.add('hide');
        document.querySelector(`.gdpr-consent`).classList.remove('show');
    }

    showGDPR() {
        document.querySelector(`.gdpr-consent`).classList.add('show');
    }
}

const gdpr = new GDPR();

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
    return usr_input == uniquechar;
}
