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

    $("#kontrolor-ime, #kontrolor-prezime, #kontrolor-oib, #kontrolor-datumZaposlenja, #kontrolor-zaposlenDo, #kontrolor-korisnickoIme, #kontrolor-lozinka").bind('keydown', function (event) {
        if (event.which === 13) {
            event.preventDefault();
            dodajKontrolora();
        }
    });


    $("#kontrolor-dodaj").click(function () {
        event.preventDefault();
        dodajKontrolora();
    });
});

function dodajKontrolora() {
    var sifra = $("#kontrolor-idRang").val();
    var ime = $("#kontrolor-ime").val();
    var prezime = $("#kontrolor-prezime").val();
    var oib = $("#kontrolor-oib").val();
    var datumZaposlenja = $("#kontrolor-datumZaposlenja").val();
    var korisnickoIme = $("#kontrolor-korisnickoIme").val();
    var lozinka = $("#kontrolor-lozinka").val();
    var imeRanga = $("#kontrolor-imeRanga").val();
    var pocetakSmjene = $("#kontrolor-pocetakSmjene").val();

    if (sifra != '' && ime != '' && prezime != '' && oib != '' && datumZaposlenja != '' && korisnickoIme != '' && lozinka != '' && imeRanga != '' && pocetakSmjene != '') {
        if ($("[name='Kontrolori[" + sifra + "].IdRang'").length > 0) {
            alert('Kontrolor sa odabranim rangom je vec u smjeni');
            return;
        }

        var zaposlenDo = $("#kontrolor-zaposlenDo").val();

        var template = $('#template').html();

        //Alternativa ako su hr postavke sa zarezom //http://haacked.com/archive/2011/03/19/fixing-binding-to-decimals.aspx/
        //ili ovo http://intellitect.com/custom-model-binding-in-asp-net-core-1-0/

        template = template.replace(/--idRang--/g, sifra)
            .replace(/--ime--/g, ime)
            .replace(/--prezime--/g, prezime)
            .replace(/--oib--/g, oib)
            .replace(/--datumZaposlenja--/g, datumZaposlenja)
            .replace(/--korisnickoIme--/g, korisnickoIme)
            .replace(/--lozinka--/g, lozinka)
            .replace(/--imeRanga--/g, imeRanga)
            .replace(/--pocetakSmjene--/g, pocetakSmjene)
            .replace(/--zaposlenDo--/g, zaposlenDo);
        $(template).find('tr').insertBefore($("#table-kontrolori").find('tr').last());

        $("#kontrolor-idRang").val('');
        $("#kontrolor-ime").val('');
        $("#kontrolor-prezime").val('');
        $("#kontrolor-oib").val('');
        $("#kontrolor-datumZaposlenja").val('');
        $("#kontrolor-korisnickoIme").val('');
        $("#kontrolor-lozinka").val('');
        $("#kontrolor-imeRanga").val('');
        $("#kontrolor-pocetakSmjene").val('');
        $("#kontrolor-zaposlenDo").val('');

        clearOldMessage();
    }
}