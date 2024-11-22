// Animación al hacer scroll: Cargar las tarjetas cuando entren y salgan de la vista
document.addEventListener("DOMContentLoaded", function () {
    const otbCards = document.querySelectorAll(".otb-card");

    const observer = new IntersectionObserver(entries => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                // Añadir clase 'show' para que se animen cuando entren en la vista
                entry.target.classList.add('show');
                entry.target.classList.remove('hide-down');
            } else {
                // Añadir clase 'hide-down' para que se deslicen hacia abajo al salir de la vista
                entry.target.classList.remove('show');
                entry.target.classList.add('hide-down');
            }
        });
    }, {
        threshold: 0.1  // Detecta cuando el 10% de la tarjeta entra o sale de la vista
    });

    otbCards.forEach(card => observer.observe(card));
});

// Filtrar las tarjetas según el texto del buscador
function filterOtbs() {
    const input = document.getElementById("searchBox").value.toLowerCase();
    const cards = document.querySelectorAll(".otb-card");

    cards.forEach(card => {
        const name = card.querySelector(".card-title").innerText.toLowerCase();

        if (name.includes(input)) {
            card.classList.remove('slide-up');
            card.classList.add('slide-down');
            card.parentElement.style.display = "block"; // Muestra la tarjeta
        } else {
            card.classList.remove('slide-down');
            card.classList.add('slide-up');

            setTimeout(() => {
                card.parentElement.style.display = "none";
            }, 500); // Tiempo suficiente para que la animación termine
        }
    });
}
function goToDetails(otbId) {
    window.location.href = `/Citizen/CisternCalendar?SelectedOtb=${otbId}`;
}

// Valida la entrada: no permite números, caracteres especiales, ni espacios al inicio o consecutivos
function validateInput() {
    const searchBox = document.getElementById("searchBox");
    const invalidChars = /[^a-zA-Z\s]/g; // Solo permite letras y espacios

    searchBox.value = searchBox.value
        .replace(invalidChars, '')         // Elimina caracteres especiales y números
        .replace(/^\s+/g, '')              // Elimina espacios al inicio
        .replace(/\s{2,}/g, ' ');          // Reemplaza múltiples espacios por uno
}