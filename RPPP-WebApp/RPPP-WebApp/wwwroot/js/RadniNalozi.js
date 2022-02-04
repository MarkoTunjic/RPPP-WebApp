$(document).on('click', '.deleterow', function () {
    event.preventDefault();
    var tr = $(this).parents("tr");
    tr.remove();
    clearOldMessage();
});

$(function () {
    $(".form-control").bind('keydown', function (event) {
        if (event.which === 13) {
            event.preventDefault();
        }
    });

    $("#radnilist-pocetakrada, #radnilist-trajanjerada, #radnilist-opisrada, #radnilist-nazivuredaja").bind('keydown', function (event) {
        if (event.which === 13) {
            event.preventDefault();
            dodajRadniList();
        }
    });


    $("#radnilist-dodaj").click(function () {
        event.preventDefault();
        dodajRadniList();
    });
});

function dodajRadniList() {
    var statusID = parseInt($("#radnilist-idStatus").val());
    var uredajID = parseInt($("#radnilist-idUredaj").val());
    var timID = parseInt($("#radnilist-idTimZaOdrzavanje").val());
    var pocetak = $("#radnilist-pocetakrada").val();
    var trajanje = parseInt($("#radnilist-trajanjerada").val());
    var uredaj = $("#radnilist-nazivuredaja").val()
    var opisRada = $("#radnilist-opisrada").val();
    if (pocetak != '' && trajanje != '' && opisRada != '' && uredaj != '' && statusID && uredajID && timID) {
        if ($("[name='RadniListovi[" + status + "].IdStatus'").length > 0) {
            alert('Radni list s odabranim statusom je vec u radnom nalogu');
            return;
        }

        var status = $("#radnilist-status").val();
        var timZaOdrz = $("#radnilist-timzaodrzavanje").val()
        var template = $('#template').html();

        //Alternativa ako su hr postavke sa zarezom //http://haacked.com/archive/2011/03/19/fixing-binding-to-decimals.aspx/
        //ili ovo http://intellitect.com/custom-model-binding-in-asp-net-core-1-0/

        template = template.replace(/--status--/g, status)
            .replace(/--pocetak--/g, pocetak)
            .replace(/--trajanje--/g, trajanje)
            .replace(/--opis--/g, opisRada)
            .replace(/--uredaj--/g, uredaj)
            .replace(/--idStatus--/g, statusID)
            .replace(/--idUredaj--/g, uredajID)
            .replace(/--idTimZaOdrzavanje--/g, timID)
            .replace(/--tim--/g, timZaOdrz);
        $(template).find('tr').insertBefore($("#table-radnilistovi").find('tr').last());

        $("#radnilist-idStatus").val('');
        $("#radnilist-idUredaj").val('')
        $("#radnilist-idTimZaOdrzavanje").val('')
        $("#radnilist-status").val('');
        $("#radnilist-pocetakrada").val('');
        $("#radnilist-trajanjerada").val('');
        $("#radnilist-opisrada").val('');
        $("#radnilist-nazivuredaja").val('');
        $("#radnilist-timzaodrzavanje").val('');

        clearOldMessage();
    }
}