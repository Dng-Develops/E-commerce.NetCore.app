//get elements
const el = document.getElementById("mockTest");
const cardName = document.getElementById("cardName");
const cardNumber = document.getElementById("cardNumber");
const cardExpMonth = document.getElementById("cardExpMonth");
const cardExpYear = document.getElementById("cardExpYear");
const carCvc = document.getElementById("carCvc");

el.addEventListener("click", () => {
    cardName.value = "John Doe"
    cardNumber.value = "5528790000000008"
    cardExpMonth.value = "12"
    cardExpYear.value = "2030"
    carCvc.value = "123"
})