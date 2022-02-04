$(function () {
    $(document).on('click', '.delete', function (event) {
        if (!confirm("Obrisati zapis?")) {
            event.preventDefault();
        }
        else {
            const url = $(this).data('delete-ajax'); //Provjeri radi li se dinamičko brisanje (ako je definirana adresa u data-delete-ajax)
            if (url !== undefined && url !== '') {
                event.preventDefault();
                deleteAjax($(this), url);
            }
        }
    })
});
function clearOldMessage() {
    $("#tempmessage").siblings().remove();
    $("#tempmessage").removeClass("alert-success");
    $("#tempmessage").removeClass("alert-danger");
    $("#tempmessage").html('');
}