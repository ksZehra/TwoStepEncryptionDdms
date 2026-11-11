<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DataEditList.aspx.cs" Inherits="TwoStepEncryptionDdms.DataEditList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphButon" runat="server">
    <style type="text/css">
        .lblSessionCss {
            float: left !important;
        }
    </style>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphSagIcerik" runat="server">
    <style type="text/css">
        body {
            background-color: #eee;
        }

        table th {
            font-weight: bold !important;
        }

        table th, table td {
            text-align: center;
            font: 12px Tahoma, Geneva, sans-serif;
            height: 20px !important;
            border: 1px solid lightGray;
        }

        table tr:nth-child(even) {
            background-color: #f2f2f2;
        }

        .pagination li:hover {
            cursor: pointer;
        }

        table tbody tr {
            display: none;
        }
    </style>
    <div style="width: 100%; border: 1px solid gray; padding: 10px 0 10px 0; margin-bottom: 10px;" runat="server" id="divKey">
        <div class="form-group row" style="padding-left: 1% !important;">
            <asp:Label runat="server" class="col-sm-2 col-form-label col-form-label-sm" Style="width: 4.5%; padding-top: 5px;">Anahtar:</asp:Label>
            <div class="col-sm-10" style="width: 95%;">
                <asp:TextBox class="form-control" ID="tbKey" MaxLength="32" placeholder="32 karakter olacak şekilde anahtarı giriniz..." Width="100%" Height="30px" runat="server" ClientIDMode="Static"></asp:TextBox>
            </div>
        </div>

        <center>
            <asp:Button ID="btnKey" class="btn btn-success" runat="server" Text="Kontrol Et" Width="150px" OnClick="btnKey_Click" ClientIDMode="Static" />
        </center>
    </div>

    <div class="alert alert-info" runat="server" id="divAlert" style="width: 100%; text-align: center; padding: 10px;" visible="false">
        <label style="font-size: 15px;">Mavi renkli alanlar şifrelenerek kaydedilmiştir.</label>
    </div>

    <h4 style="font-weight: bold; margin-top: 0; margin-bottom: 15px; text-align: center; margin-top: 20px;" runat="server" id="tabloBaslik"></h4>

    <div id="divContainer" runat="server" visible="false">
        <div class="form-group" style="padding-bottom: 30px !important;">
            <!--		Show Numbers Of Rows 		-->
            <label style="font-size: 15px; float: left; margin-top: 5px;">Kayıt Sayısı:</label>
            <select class="form-control" name="state" id="maxRows" style="width: 20%; float: left; margin: 0 0 0 5px; padding: 1px 6px; height: 28px !important;">
                <option value="5000">Tüm Kayıtları Gör</option>
                <option value="5">5</option>
                <option value="10">10</option>
                <option value="20">20</option>
                <option value="30">30</option>
                <option value="40">40</option>
                <option value="50">50</option>
                <option value="70">70</option>
                <option value="100">100</option>
            </select>
        </div>

        <div style="font-size: 12px; width: 100%; overflow-x: scroll;">
            <asp:Literal ID="ltList" runat="server"></asp:Literal>
        </div>

        <!--		Start Pagination -->
        <div class='pagination-container'>
            <nav>
                <ul class="pagination" style="margin-top: 10px;">

                    <li data-page="prev">
                        <span style="font-size: 12px !important;">< <span class="sr-only">(current)</span></span>
                    </li>
                    <!--	Here the JS Function Will Add the Rows -->
                    <li data-page="next" id="prev">
                        <span style="font-size: 12px !important;">> <span class="sr-only">(current)</span></span>
                    </li>
                </ul>
            </nav>
        </div>
    </div>

    <script type="text/javascript">

        $(function () {

            $('#btnKey').click(function () {
                if (document.getElementById("tbKey").value == "") {
                    alert("Görüntülemek istediğiniz tablonun içerisinde şifrelenmesini istediğiniz alanlar olduğu için 'anahtar' bilgisi boş bırakılamaz!");
                    return false
                }
            });
        });

        $(".redirect").on("click", function () {
            window.location.href = $(this).attr("redirect");
        })

        getPagination('#table-id');

        function getPagination(table) {
            var lastPage = 1;

            $('#maxRows')
                .on('change', function (evt) {

                    lastPage = 1;
                    $('.pagination')
                        .find('li')
                        .slice(1, -1)
                        .remove();
                    var trnum = 0; // reset tr counter
                    var maxRows = parseInt($(this).val()); // get Max Rows from select option

                    if (maxRows == 5000) {
                        $('.pagination').hide();
                    } else {
                        $('.pagination').show();
                    }

                    var totalRows = $(table + ' tbody tr').length; // numbers of rows
                    $(table + ' tr:gt(0)').each(function () {
                        // each TR in  table and not the header
                        trnum++; // Start Counter
                        if (trnum > maxRows) {
                            // if tr number gt maxRows

                            $(this).hide(); // fade it out
                        }
                        if (trnum <= maxRows) {
                            $(this).show();
                        } // else fade in Important in case if it ..
                    }); //  was fade out to fade it in
                    if (totalRows > maxRows) {
                        // if tr total rows gt max rows option
                        var pagenum = Math.ceil(totalRows / maxRows); // ceil total(rows/maxrows) to get ..
                        //	numbers of pages
                        for (var i = 1; i <= pagenum;) {
                            // for each page append pagination li
                            $('.pagination #prev')
                                .before(
                                    '<li data-page="' +
                                    i +
                                    '">\
								  <span>' +
                                    i++ +
                                    '<span class="sr-only">(current)</span></span>\
								</li>'
                                )
                                .show();
                        } // end for i
                    } // end if row count > max rows
                    $('.pagination [data-page="1"]').addClass('active'); // add active class to the first li
                    $('.pagination li').on('click', function (evt) {
                        // on click each page
                        evt.stopImmediatePropagation();
                        evt.preventDefault();
                        var pageNum = $(this).attr('data-page'); // get it's number

                        var maxRows = parseInt($('#maxRows').val()); // get Max Rows from select option

                        if (pageNum == 'prev') {
                            if (lastPage == 1) {
                                return;
                            }
                            pageNum = --lastPage;
                        }
                        if (pageNum == 'next') {
                            if (lastPage == $('.pagination li').length - 2) {
                                return;
                            }
                            pageNum = ++lastPage;
                        }

                        lastPage = pageNum;
                        var trIndex = 0; // reset tr counter
                        $('.pagination li').removeClass('active'); // remove active class from all li
                        $('.pagination [data-page="' + lastPage + '"]').addClass('active'); // add active class to the clicked
                        // $(this).addClass('active');					// add active class to the clicked
                        limitPagging();
                        $(table + ' tr:gt(0)').each(function () {
                            // each tr in table not the header
                            trIndex++; // tr index counter
                            // if tr index gt maxRows*pageNum or lt maxRows*pageNum-maxRows fade if out
                            if (
                                trIndex > maxRows * pageNum ||
                                trIndex <= maxRows * pageNum - maxRows
                            ) {
                                $(this).hide();
                            } else {
                                $(this).show();
                            } //else fade in
                        }); // end of for each tr in table
                    }); // end of on click pagination list
                    limitPagging();
                })
                .val(10)
                .change();

            // end of on select change

            // END OF PAGINATION
        }

        function limitPagging() {
            if ($('.pagination li').length > 7) {
                if ($('.pagination li.active').attr('data-page') <= 3) {
                    $('.pagination li:gt(10)').hide();
                    $('.pagination li:lt(10)').show();
                    $('.pagination [data-page="next"]').show();
                } if ($('.pagination li.active').attr('data-page') > 3) {
                    $('.pagination li:gt(0)').hide();
                    $('.pagination [data-page="next"]').show();
                    for (let i = (parseInt($('.pagination li.active').attr('data-page')) - 2); i <= (parseInt($('.pagination li.active').attr('data-page')) + 2); i++) {
                        $('.pagination [data-page="' + i + '"]').show();

                    }

                }
            }
        }

    </script>

</asp:Content>
