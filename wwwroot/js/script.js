const areas = document.querySelectorAll('area');
const selectDistrict = document.getElementById('distritoSelect');

// Función para actualizar el valor del select al hacer clic en un área del mapa
areas.forEach(area => {
    area.addEventListener('click', function (event) {
        event.preventDefault(); // Previene la navegación por defecto
        const district = area.getAttribute('data-district');
        selectDistrict.value = district;
        console.log(`Distrito seleccionado: ${district}`);
    });
});

function onDistritoChange() {
    // Obtener el valor seleccionado
    var selectedValue = document.getElementById('distritoSelect').value;

    // Aquí puedes realizar la acción que necesites con el valor seleccionado
    console.log("Distrito seleccionado: " + selectedValue);

    // Ejemplo de redireccionar o hacer algo con el valor seleccionado:
    // window.location.href = '/tu-ruta?distrito=' + selectedValue;
}