<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DataEditList.aspx.cs" Inherits="TwoStepEncryptionDdms.DataEditList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphButon" runat="server">
    <div class="page-heading">
        <div>
            <h1 class="page-title">Kayıt Düzenle / Sil</h1>
            <p class="page-subtitle">İşlem yapmak istediğiniz kaydı seçin.</p>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphSagIcerik" runat="server">
    <div class="key-card" id="divKey" runat="server">
        <div class="key-card-icon" aria-hidden="true">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round">
                <rect x="4" y="10" width="16" height="10" rx="2"></rect>
                <path d="M8 10V7a4 4 0 0 1 8 0v3"></path>
            </svg>
        </div>
        <div class="key-card-body">
            <h2 class="key-card-title">Bu veri kümesi şifreli</h2>
            <p class="key-card-text">Kayıtları görüntülemek için 32 karakterlik anahtarı girin.</p>
            <div class="key-form">
                <asp:TextBox class="form-control" ID="tbKey" MaxLength="32" placeholder="32 karakterlik anahtar" runat="server" ClientIDMode="Static" autocomplete="off"></asp:TextBox>
                <asp:Button ID="btnKey" CssClass="btn btn-primary" runat="server" Text="Kontrol Et" OnClick="btnKey_Click" ClientIDMode="Static" />
            </div>
            <div class="key-status">
                <span class="badge badge-success">Anahtar doğrulandı</span>
                <span>Şifreli alanlar bu oturumda çözülebilir.</span>
            </div>
        </div>
    </div>

    <div class="notice notice-info" runat="server" id="divAlert" visible="false">
        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true">
            <circle cx="12" cy="12" r="9"></circle>
            <path d="M12 11v5"></path>
            <path d="M12 8h.01"></path>
        </svg>
        <div>Mavi renkli alanlar şifrelenerek kaydedilmiştir.</div>
    </div>

    <div class="card" id="divContainer" runat="server" visible="false">
        <div class="card-header">
            <h2 class="card-title" runat="server" id="tabloBaslik"></h2>
            <span class="card-note">Şifreli sütunlar mavi olarak işaretlenir</span>
        </div>
        <div class="card-body">
            <div class="table-toolbar">
                <label for="maxRows" class="table-toolbar-label">Kayıt sayısı</label>
                <select class="form-control input-sm" name="state" id="maxRows">
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

            <div class="table-responsive">
                <asp:Literal ID="ltList" runat="server"></asp:Literal>
            </div>

            <!--		Start Pagination -->
            <div class="pagination-container">
                <nav>
                    <ul class="pagination">

                        <li data-page="prev">
                            <span aria-hidden="true">‹</span>
                            <span class="sr-only">Önceki sayfa</span>
                        </li>
                        <!--	Here the JS Function Will Add the Rows -->
                        <li data-page="next" id="prev">
                            <span aria-hidden="true">›</span>
                            <span class="sr-only">Sonraki sayfa</span>
                        </li>
                    </ul>
                </nav>
            </div>
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
            $(table).addClass('is-paginated');

            $('#maxRows')
                .on('change', function (evt) {

                    lastPage = 1;
                    $('.pagination')
                        .find('li')
                        .slice(1, -1)
                        .remove();
                    var trnum = 0; // reset tr counter
                    var maxRows = parseInt($(this).val()); // get Max Rows from select option
                    var totalRows = $(table + ' tbody tr').length; // numbers of rows

                    if (maxRows == 5000 || totalRows === 0 || totalRows <= maxRows) {
                        $('.pagination-container').hide();
                    } else {
                        $('.pagination-container').show();
                    }

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
