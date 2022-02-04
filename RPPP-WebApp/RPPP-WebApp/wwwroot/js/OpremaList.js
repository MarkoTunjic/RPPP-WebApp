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

    $("#uredaj-naziv, #uredaj-proizvodac, #uredaj-godinaProizvodnje, #uredaj-idOprema, #uredaj-tipStanja").bind('keydown', function (event) {
        if (event.which === 13) {
            event.preventDefault();
            dodajUredaj();
        }
    });


    $("#uredaj-dodaj").click(function () {
        event.preventDefault();
        dodajUredaj();
    });
});

function dodajUredaj() {
    var idStanje = parseInt($("#uredaj-idStanje").val());
    var naziv = $("#uredaj-naziv").val();
    var proizvodac = $("#uredaj-proizvodac").val();
    var godinaProizvodnje = $("#uredaj-godinaProizvodnje").val();
    var idOprema = parseInt($("#uredaj-idOprema").val());
    if (naziv != '' && proizvodac != '' && godinaProizvodnje != '') {
        if ($("[name='Uredaji[" + idOprema + "].IdOprema'").length > 0) {
            alert('Uređaj je već u opremi');
            return;
        }

        var tipStanja = $("#uredaj-tipStanja").val();

        var template = $('#template').html();

        //Alternativa ako su hr postavke sa zarezom //http://haacked.com/archive/2011/03/19/fixing-binding-to-decimals.aspx/
        //ili ovo http://intellitect.com/custom-model-binding-in-asp-net-core-1-0/

        template = template.replace(/--idStanje--/g, idStanje)
            .replace(/--tipStanja--/g, tipStanja)
            .replace(/--naziv--/g, naziv)
            .replace(/--proizvodac--/g, proizvodac)
            .replace(/--godinaProizvodnje--/g, godinaProizvodnje)
            .replace(/--idOprema--/g, idOprema)
        $(template).find('tr').insertBefore($("#table-uredaji").find('tr').last());

        $("#uredaj-idStanje").val('');
        $("#uredaj-tipStanja").val('');
        $("#uredaj-naziv").val('');
        $("#radnik-proizvodac").val('');
        $("#radnik-godinaProizvodnje").val('');
        $("#radnik-idOprema").val('');

        clearOldMessage();
    }
}