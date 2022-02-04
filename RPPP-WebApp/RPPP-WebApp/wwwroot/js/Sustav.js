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

    $("#podsustav-naziv, #podsustav-opis, #podsustav-osjetljivost, #podsustav-nazivLokacije, #podsustav-stupanjKriticnosti, #podsustav-ucestalostOdrzavanja").bind('keydown', function (event) {
        if (event.which === 13) {
            event.preventDefault();
            dodajRadnika();
        }
    });


    $("#podsustav-dodaj").click(function () {
        event.preventDefault();
        dodajRadnika();
    });
});

function dodajRadnika() {

    var sifra = $("#podsustav-idStupanjKriticnosti").val();
    var naziv = $("#podsustav-naziv").val();
    var opis = $("#podsustav-opis").val();
    var osjetljivost = parseInt($("#podsustav-osjetljivost").val());
    var nazivLokacije = $("#podsustav-nazivLokacije").val();
    
    var ucestalostOdrzavanja = parseInt($("#podsustav-ucestalostOdrzavanja").val());
    var idLokacija = parseInt($("#podsustav-idLokacija").val());

    console.log(sifra, naziv, nazivLokacije, idLokacija, stupanjKriticnosti)
    if (sifra != '' && stupanjKriticnosti != '') {
        var template = $('#template').html();
        var stupanjKriticnosti = $("#podsustav-stupanjKriticnosti").val();
        template = template.replace(/--IdStupanjKriticnosti--/g, sifra)
            .replace(/--stupanj--/g, stupanjKriticnosti)
            .replace(/--naziv--/g, naziv)
            .replace(/--opis--/g, opis)
            .replace(/--osjetljivost--/g, osjetljivost)
            .replace(/--nazivLokacije--/g, nazivLokacije)
            .replace(/--ucestalostOdrzavanja--/g, ucestalostOdrzavanja)
            .replace(/--IdLokacija--/g, idLokacija)
        $(template).find('tr').insertBefore($("#table-podsustavi").find('tr').last());

        $("#podsustav-idStupanjKriticnosti").val('');
        $("#podsustav-naziv").val('');
        $("#podsustav-opis").val('');
        $("#podsustav-osjetljivost").val('');
        $("#podsustav-nazivLokacije").val('');
        $("#podsustav-stupanjKriticnosti").val('');
        $("#podsustav-ucestalostOdrzavanja").val('');

        clearOldMessage();
    }
    
    
}