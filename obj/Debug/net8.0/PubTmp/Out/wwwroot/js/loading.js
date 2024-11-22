// loading.js
window.addEventListener("load", function () {
    const loader = document.getElementById("loader");
    const mainContent = document.getElementById("mainContent");

    // Espera 2 segundos antes de ocultar el cargador y mostrar el contenido
    setTimeout(() => {
        loader.style.display = "none"; // Oculta el cargador
        mainContent.style.display = "block"; // Muestra el contenido principal
    }, 1500); // 2000 ms = 2 segundos
});


//Codigo para cuando carge la paguina
// loading.js
//window.addEventListener("load", function () {
//    const loader = document.getElementById("loader");
//    const mainContent = document.getElementById("mainContent");

//    // Ocultar el cargador y mostrar el contenido principal una vez que se haya cargado todo
//    loader.style.display = "none"; // Oculta el cargador
//    mainContent.style.display = "block"; // Muestra el contenido principal
//});