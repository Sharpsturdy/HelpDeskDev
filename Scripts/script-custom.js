$(document).ready(function () {
   
});


//$("#fileinput").fileinput();

//Attach code to attachment delete/undelete action
$(".btn-custom-delete, .btn-custom-undelete").click(function () {
    //alert("Delete button clicked");   
    $(this).closest("tr").toggleClass("delete");

    $(".btn-custom-delete, .btn-custom-undelete", $(this).closest("tr")).toggleClass("hidden");

    var tmp = [];
    $(".delete .fileID").each(function () {
        tmp.push($(this).text());
    });

    alert(tmp.join(","));
    $("#deleteField").val(tmp.join(","));


});

$("#addLink").click(function () {

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
    $("table.links tbody").append("<tr class=\"normal danger\"><td><a href=\"" + linkURL + "\" target=\"_blank\">" + linkText + "</a></td>" +
        "<td><input type=\"button\" value=\"Cancel\" class=\"btn btn-default btn-custom-cancel\"></tr>");

    $("#linkText").val("");
    $("#linkURL").val("");

    var tmp = [];
    $("table.links tr.normal a").each(function () {
        tmp.push($(this).text() + "^" + $(this).attr('href'));
    });
    alert(tmp.join("|"));
    $("#links").val(tmp.join("|"));

});

$(".links .btn-custom-delete").each(function () {

    $(this).closest("tr").toggleClass("normal delete");

});