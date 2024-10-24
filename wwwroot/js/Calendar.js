document.addEventListener('DOMContentLoaded', function () {
    var calendarEl = document.getElementById('calendar');
    var calendar = new FullCalendar.Calendar(calendarEl, {
        initialView: 'timeGridWeek',
        
        headerToolbar: {
            left: '',
            center: 'title',
            right: ''
        },
        allDaySlot: false,
        slotMinTime: "08:00:00",
        slotMaxTime: "11:00:00",
        //events: @Html.Raw(Newtonsoft.Json.JsonConvert.SerializeObject(Model.CalendarEvents)), esta fila me sale error, no me deja cargar el calendario
        datesSet: function () {
            var titleElement = document.querySelector('.fc-toolbar-title');
            if (titleElement) {
                titleElement.textContent = 'Horario Semanal';
            }
            changeSlotLabels();
            changeDayHeaders();
        }
    });
    calendar.render();

    function changeSlotLabels() {
        var labels = document.querySelectorAll('.fc-timegrid-slot-label-cushion');
        if (labels.length > 0) {
            if (labels[0]) labels[0].textContent = 'Mañana';  // 8am
            if (labels[1]) labels[1].textContent = 'Tarde';   // 9am
            if (labels[2]) labels[2].textContent = 'Noche';   // 10am
        }
    }

    function changeDayHeaders() {
        var dayHeaders = document.querySelectorAll('.fc-col-header-cell-cushion');
        if (dayHeaders.length > 0) {
            var headers = ['Domingo', 'Lunes', 'Martes', 'Miércoles', 'Jueves', 'Viernes', 'Sábado'];
            dayHeaders.forEach((header, index) => {
                if (headers[index]) {
                    header.textContent = headers[index];
                }
            });
        }
    }
});
