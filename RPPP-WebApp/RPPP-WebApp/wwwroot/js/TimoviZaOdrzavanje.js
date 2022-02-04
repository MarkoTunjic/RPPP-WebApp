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

    $("#radnik-ime, #radnik-prezime, #radnik-certifikat, #radnik-istekCertifikata,#radnik-dezuran").bind('keydown', function (event) {
        if (event.which === 13) {
            event.preventDefault();
            dodajRadnika();
        }
    });


    $("#radnik-dodaj").click(function () {
        event.preventDefault();
        dodajRadnika();
    });
});

function dodajRadnika() {
    var sifra = $("#radnik-idStrucnaSprema").val();
    var ime = $("#radnik-ime").val();
    var prezime = $("#radnik-prezime").val();
    var dezuran = parseInt($("#radnik-dezuran").val());
    if (sifra != '' && ime != '' && prezime != '' && (dezuran >= 0 && dezuran <= 1)) {
        if ($("[name='Radnici[" + sifra + "].IdStrucnaSprema'").length > 0) {
            alert('Radnik sa odabranom stručnom spremom je vec u timu');
            return;
        }

        var razina = $("#radnik-strucnaSprema").val();

        var certifikat = $("#radnik-certifikat").val();

        var datumIsteka = $("#radnik-istekCertifikata").val();

        var template = $('#template').html();

        //Alternativa ako su hr postavke sa zarezom //http://haacked.com/archive/2011/03/19/fixing-binding-to-decimals.aspx/
        //ili ovo http://intellitect.com/custom-model-binding-in-asp-net-core-1-0/

        template = template.replace(/--idStrucnaSprema--/g, sifra)
            .replace(/--razina--/g, razina)
            .replace(/--ime--/g, ime)
            .replace(/--prezime--/g, prezime)
            .replace(/--certifikat--/g, certifikat)
            .replace(/--istekCertifikata--/g, datumIsteka)
            .replace(/--dezuran--/g, dezuran);
        $(template).find('tr').insertBefore($("#table-radnici").find('tr').last());

        $("#radnik-idStrucnaSprema").val('');
        $("#radnik-strucnaSprema").val('');
        $("#radnik-ime").val('');
        $("#radnik-prezime").val('');
        $("#radnik-certifikat").val('');
        $("#radnik-istekCertifikata").val('');
        $("#radnik-dezuran").val('');

        clearOldMessage();
    }
}