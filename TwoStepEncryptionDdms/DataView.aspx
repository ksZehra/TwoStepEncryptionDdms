<%@ Page Title="Listelenen Tablo" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DataView.aspx.cs" Inherits="TwoStepEncryptionDdms.DataView" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphButon" runat="server">
    <style>
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
            <asp:Label runat="server" class="col-sm-2 col-form-label col-form-label-sm" Style="width: 4.5%; padding-top: 5px;">Anahtar: </asp:Label>
            <div class="col-sm-10" style="width: 95%;">
                <asp:TextBox class="form-control" ID="tbKey" MaxLength="32" placeholder="32 karakter olacak şekilde anahtarı giriniz..." Width="100%" Height="30px" runat="server" ClientIDMode="Static"></asp:TextBox>
            </div>
        </div>

        <center>
            <asp:Button ID="btnKey" class="btn btn-success" runat="server" Text="Kontrol Et" Width="150px" OnClick="btnKey_Click" ClientIDMode="Static" />
        </center>
    </div>

    <div class="alert alert-info" runat="server" id="divAlert" style="width: 100%; text-align: center; padding: 10px;" visible="false">
        <label>Mavi renkli alanlar şifrelenerek kaydedilmiştir.</label>
    </div>

    <h4 style="font-weight: bold; margin-top: 0; margin-bottom: 10px; text-align: center; margin-top: 20px;" runat="server" id="tabloBaslik"></h4>

    <div id="divContainer" runat="server" visible="false">

        <div style="margin-bottom: 20px; border: 1px solid Gray; padding: 10px; display: none;">

            <label style="margin-bottom: 15px;">Arama Yapabilmek için Bilgileri Giriniz</label>

            <div class="form-group row">
                <asp:Label runat="server" class="col-sm-2 col-form-label col-form-label-sm" Style="width: 11%; padding-top: 6px; font: 13px Tahoma, Geneva, sans-serif !important;">Sütun Adı :</asp:Label>
                <div class="col-sm-10" style="width: 30%; padding-left: 0px; font: 11px Tahoma, Geneva, sans-serif !important;">
                    <asp:TextBox class="form-control cls-font" ID="tbSutunAdi" MaxLength="120" Height="26px" placeholder="Sütun adını giriniz..." runat="server" ClientIDMode="Static"></asp:TextBox>
                </div>
            </div>

            <div class="form-group row">
                <asp:Label runat="server" class="col-sm-2 col-form-label col-form-label-sm" Style="width: 11%; padding-top: 6px; font: 13px Tahoma, Geneva, sans-serif !important;">Aranacak Kelime :</asp:Label>
                <div class="col-sm-10" style="width: 30%; padding-left: 0px; font: 11px Tahoma, Geneva, sans-serif !important;">
                    <asp:TextBox class="form-control cls-font" ID="tbKelime" MaxLength="120" Height="26px" placeholder="Aramak istediğiniz kelimeyi giriniz..." runat="server" ClientIDMode="Static"></asp:TextBox>
                </div>
            </div>

            <asp:Button ID="btnKelimeAra" CssClass="btn btn-warning btn-sm" Width="8%" runat="server" Text="Ara" ClientIDMode="Static" OnClick="btnKelimeAra_Click" />
            <asp:Button ID="btnTemizle" runat="server" Text="Temizle" Width="8%" CssClass="btn btn-info btn-sm" float="left" OnClick="btnTemizle_Click" />
        </div>


        <div class="form-group" style="padding-bottom: 30px !important;">
            <!--		Show Numbers Of Rows 		-->
            <label style="float: left; margin-top: 5px;">Kayıt Sayısı:</label>
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

        getPagination('#table-id');
        //getPagination('.table-class');
        //getPagination('table');

        /*					PAGINATION 
        - on change max rows select options fade out all rows gt option value mx = 5
        - append pagination list as per numbers of rows / max rows option (20row/5= 4pages )
        - each pagination li on click -> fade out all tr gt max rows * li num and (5*pagenum 2 = 10 rows)
        - fade out all tr lt max rows * li num - max rows ((5*pagenum 2 = 10) - 5)
        - fade in all tr between (maxRows*PageNum) and (maxRows*pageNum)- MaxRows 
        */


        function getPagination(table) {
            var lastPage = 1;

            $('#maxRows')
                .on('change', function (evt) {
                    //$('.paginationprev').html('');						// reset pagination

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
            // alert($('.pagination li').length)

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

        //Tablonun en başındaki sutüna ID ekler ve tüm sütunların birer artmasını sağlar
        $(function () {
            // Just to append id number for each row
            $('table tr:eq(0)').prepend('<th> NO </th>');

            var id = 0;

            $('table tr:gt(0)').each(function () {
                id++;
                $(this).prepend('<td>' + id + '</td>');
            });
        });
    </script>
</asp:Content>
