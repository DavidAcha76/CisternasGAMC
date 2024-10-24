$(document).ready(function () {
    // Función para cargar las OTBs según el distrito seleccionado
    function loadOtbsForSelectedDistrict() {
        var selectedDistrito = $('#distritoSelect').val();
        if (selectedDistrito) {
            // Hacer la petición AJAX para obtener las OTBs
            $.ajax({
                url: '/Index?handler=Otbs', // Endpoint al método del backend
                type: 'GET',
                data: { district: selectedDistrito },
                success: function (data) {
                    // Limpiar el combobox de OTB
                    $('#otbSelect').empty();

                    // Añadir las nuevas OTBs directamente
                    $.each(data, function (index, otb) {
                        $('#otbSelect').append('<option value="' + otb.otbId + '">' + otb.name + '</option>');
                    });

                    // Si hay OTBs, selecciona la primera por defecto
                    if (data.length > 0) {
                        $('#otbSelect').val(data[0].otbId);
                    }
                }
            });
        } else {
            // Si no se selecciona distrito, limpiamos el select de OTB
            $('#otbSelect').empty();
        }
    }

    // Cuando el usuario cambie el distrito
    $('#distritoSelect').change(function () {
        loadOtbsForSelectedDistrict();
    });

    // Ejecutar el filtro de OTB al cargar la página automáticamente
    loadOtbsForSelectedDistrict(); // Cargar las OTBs al inicio
});
