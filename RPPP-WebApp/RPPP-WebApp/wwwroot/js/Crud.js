$(async function () {
    $(document).on('click', '#dohvatiPodrucja', function (event) {
        var controller = $("#controller").val();
        var start = $("#start").val();
        var size = $("#size").val();
        var sort = $("#sort").val();
        getData(`${window.location.protocol}//${window.location.host}/rppp/02/${controller}?jtStartIndex=${start}&jtPageSize=${size}&jtSorting=${sort}`)
            .then(function (res) {
                $("#podrucja").val(JSON.stringify(res));
            })
            .catch(function (err) {
                var msg = err;
                if (err["responseJSON"]["detail"] != "")
                    msg = err["responseJSON"]["detail"];
                $("#podrucja").val(err);
            });
    })
});

$(async function () {
    $(document).on('click', '#dohvatiPodrucje', function (event) {
        var controller = $("#controller").val();
        var id = parseInt($("#id").val());
        getData(`${window.location.protocol}//${window.location.host}/rppp/02/${controller}/${id}`)
            .then(function (res) {
                $("#podrucje").val(JSON.stringify(res));
            })
            .catch(function (err) {
                var msg = err;
                if (err["responseJSON"]["detail"] != "")
                    msg = err["responseJSON"]["detail"];
                $("#podrucje").val(msg);
            });
    })
});

$(async function () {
    $(document).on('click', '#DohvatiKriticnosti', function (event) {
        var controller = $("#controller").val();
        var start = $("#start").val();
        var size = $("#size").val();
        var sort = $("#sort").val();
        getData(`${window.location.protocol}//${window.location.host}/rppp/02/${controller}?jtStartIndex=${start}&jtPageSize=${size}&jtSorting=${sort}`)
            .then(function (res) {
                $("#kriticnosti").val(JSON.stringify(res));
            })
            .catch(function (err) {
                var msg = err;
                if (err["responseJSON"]["detail"] != "")
                    msg = err["responseJSON"]["detail"];
                $("#kriticnosti").val(err);
            });
    })
});

$(async function () {
    $(document).on('click', '#DohvatiKriticnost', function (event) {
        var controller = $("#controller").val();
        var id = parseInt($("#id").val());
        getData(`${window.location.protocol}//${window.location.host}/rppp/02/${controller}/${id}`)
            .then(function (res) {
                $("#kriticnost").val(JSON.stringify(res));
            })
            .catch(function (err) {
                var msg = err;
                if (err["responseJSON"]["detail"] != "")
                    msg = err["responseJSON"]["detail"];
                $("#kriticnost").val(msg);
            });
    })
});

$(async function () {
    $(document).on('click', '#dohvatiTipoveOpreme', function (event) {
        var controller = $("#controller").val();
        var start = $("#start").val();
        var size = $("#size").val();
        var sort = $("#sort").val();
        getData(`${window.location.protocol}//${window.location.host}/rppp/02/${controller}?jtStartIndex=${start}&jtPageSize=${size}&jtSorting=${sort}`)
            .then(function (res) {
                $("#tipoviOpreme").val(JSON.stringify(res));
            })
            .catch(function (err) {
                var msg = err;
                if (err["responseJSON"]["detail"] != "")
                    msg = err["responseJSON"]["detail"];
                $("#tipoviOpreme").val(err);
            });
    })
});

$(async function () {
    $(document).on('click', '#dohvatiTipOpreme', function (event) {
        var controller = $("#controller").val();
        var id = parseInt($("#id").val());
        getData(`${window.location.protocol}//${window.location.host}/rppp/02/${controller}/${id}`)
            .then(function (res) {
                $("#tipOpreme").val(JSON.stringify(res));
            })
            .catch(function (err) {
                var msg = err;
                if (err["responseJSON"]["detail"] != "")
                    msg = err["responseJSON"]["detail"];
                $("#tipOpreme").val(msg);
            });
    })
});

$(async function () {
    $(document).on('click', '#dohvatiFiltrirano', function (event) {
        var controller = $("#controller").val();
        var filter = $("#filter").val();
        getData(`${window.location.protocol}//${window.location.host}/rppp/02/${controller}/count?filter=${filter}`)
            .then(function (res) {
                $("#broj").val(JSON.stringify(res));
            })
            .catch(function (err) {
                var msg = err;
                if (err["responseJSON"]["detail"] != "")
                    msg = err["responseJSON"]["detail"];
                $("#broj").val(msg);
            });
    })
});

$(async function () {
    $(document).on('click', '#obrisi', function (event) {
        var controller = $("#controller").val();
        var id = $("#brisanje").val();
        deleteData(`${window.location.protocol}//${window.location.host}/rppp/02/${controller}/${id}`)
            .then(function (res) {
                $("#obrisano").val("Uspješno obrisano. Id=" + id);
            })
            .catch(function (err) {
                var msg = err;
                if (err["responseJSON"]["detail"] != "")
                    msg = err["responseJSON"]["detail"];
                $("#obrisano").val(msg);
            });
    })
});

$(async function () {
    var controller = $("#controller").val();
    $($("#urediId")).change(function () {
        var id = parseInt($("#urediId").val());
        getData(`${window.location.protocol}//${window.location.host}/rppp/02/${controller}/${id}`)
            .then(function (res) {
                var naziv = $("#naziv").val();
                $("#noviNaziv").val(res[`${naziv}`]);
            });
    });
    $(document).on('click', '#uredi', function (event) {
        var id = $("#urediId").val();
        putData(`${window.location.protocol}//${window.location.host}/rppp/02/${controller}/${id}`)
            .then(function (res) {
                $("#uredeno").val("Uspješno uređeno. Id=" + id);
            })
            .catch(function (err) {
                var msg = err;
                if (err["responseJSON"]["detail"] != "")
                    msg = err["responseJSON"]["detail"];
                $("#uredeno").val(msg);
            });
    })
});

$(async function () {
    $(document).on('click', '#dodaj', function (event) {
        var controller = $("#controller").val();
        postData(`${window.location.protocol}//${window.location.host}/rppp/02/${controller}`)
            .then(function (res) {
                $("#dodano").val(JSON.stringify(res));
            })
            .catch(function (err) {
                var msg = err;
                if (err["responseJSON"]["detail"] != "")
                    msg = err["responseJSON"]["detail"];
                $("#dodano").val(msg);
            });
    })
});

function getData(ajaxurl) {
    return $.ajax({
        url: ajaxurl,
        type: 'GET',
    });
};

function deleteData(ajaxurl) {
    return $.ajax({
        url: ajaxurl,
        type: 'DELETE',
    });
};

function putData(ajaxurl) {
    return $.ajax({
        url: ajaxurl,
        type: 'PUT',
        dataType: 'json',
        data: $("form#uredivanje").serialize(),
    });
};

function postData(ajaxurl) {
    return $.ajax({
        url: ajaxurl,
        type: 'POST',
        dataType: 'json',
        data: $("form#dodavanje").serialize(),
    });
};
