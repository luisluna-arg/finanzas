import movementsApi from "../api/movementsApi";

const create = function (data) {
    console.log("data", data);
    movementsApi.create(data,
        (response) => {
            console.log("success response", response);
        },
        (response) => {
            console.log("error response", response);
        })
}

const MovementsService = {
    create
};

export default MovementsService;