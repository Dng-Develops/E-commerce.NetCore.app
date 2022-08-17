//get elements
const el = document.getElementById("mockTest");
const cardName = document.getElementById("cardName");
const cardNumber = document.getElementById("cardNumber");
const cardExpMonth = document.getElementById("cardExpMonth");
const cardExpYear = document.getElementById("cardExpYear");
const carCvc = document.getElementById("carCvc");

const mockList = [
    {
        cName : "John Doe",
        cNumber: "5528790000000008",
        cExpMonth : "12",
        cdExpYear : "2030",
        cCvc : "123",

    },
    {
        cName: "ObiWan Kenobi",
        cNumber: "5890040000000016",
        cExpMonth: "12",
        cdExpYear: "2030",
        cCvc: "123",

    },
    {
        cName: "Richard Dawkins",
        cNumber: "5526080000000006",
        cExpMonth: "12",
        cdExpYear: "2030",
        cCvc: "123",

    },
    {
        cName: "Morgan Freeman",
        cNumber: "4766620000000001",
        cExpMonth: "12",
        cdExpYear: "2030",
        cCvc: "123",

    },
    {
        cName: "Joe Citizen",
        cNumber: "4603450000000000",
        cExpMonth: "12",
        cdExpYear: "2030",
        cCvc: "123",

    }
]




el.addEventListener("click", () => {
    let randNum = Math.floor(Math.random() * mockList.length);
    cardName.value = mockList[randNum].cName
    cardNumber.value = mockList[randNum].cNumber
    cardExpMonth.value = mockList[randNum].cExpMonth
    cardExpYear.value = mockList[randNum].cdExpYear
    carCvc.value = mockList[randNum].cCvc
   
})