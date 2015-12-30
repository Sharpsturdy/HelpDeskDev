$(document).ready(function () {

    //$("#fileinput").fileinput();

    //Attach code to attachment delete/undelete action
    $(".btn-custom-delete, .btn-custom-undelete").click(function () {

        $(this).closest("tr").toggleClass("delete");

        $(".btn-custom-delete, .btn-custom-undelete", $(this).closest("tr")).toggleClass("hidden");

        var tmp = [];
        $(".delete .fileID").each(function () {
            tmp.push($(this).text());
        });

        //alert(tmp.join(","));
        $("#deleteField").val(tmp.join(","));


    });

    $(".links #addLink").click(function () {

        var linkText = $("#linkText").val();
        var linkURL = $("#linkURL").val();
        if (linkText == "" && linkURL == "") {
            alert("Please enter both link text and URL to link to");
            return;
        } else if (linkText == "") {
            alert("Please enter the link text");
            return;
        } else if (linkURL == "") {
            alert("Please enter the link URL");
            return;
        }
        $("table.links tbody").append("<tr class=\"normal warning\"><td><a href=\"" + linkURL + "\" target=\"_blank\">" + linkText + "</a></td>" +
            "<td><input type=\"button\" value=\"Remove\" class=\"btn btn-default btn-custom-cancel\"></tr>");

        $("#linkText").val("");
        $("#linkURL").val("");

        refreshLinks();

    });

    $(".links .btn-custom-action").click(function () {

        $(this).closest("tr").toggleClass("normal delete");
        if (this.value == "Delete") {
            this.value = "Undelete";
        } else {
            this.value = "Delete";
        }

        refreshLinks();
    });

});


$(document).on("click", ".btn-custom-cancel", function () {

    $(this).closest("tr").remove();

    refreshLinks();
});

function refreshLinks() {
    //
    var tmp = [];
    $("table.links tr.normal a").each(function () {
        tmp.push($(this).text() + "^" + $(this).attr('href'));
    });
    
    $("#links").val(tmp.join("|"));
}
