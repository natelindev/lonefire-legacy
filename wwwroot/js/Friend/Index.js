$(document).ready(function() {
    $.fn.dataTable.ext.errMode = 'none';

    var table = $('#dataTable').DataTable({
        orderCellsTop: true,
        language: {
            "url": "/js/dt-friend.json"
        },
        autoWidth: false,
        columns: [
            {
                width: '2rem', "render": function (link) {
                    return '<p style="max-width:8rem;">' + link + '</p>';
                }
            },
            { width: '5rem' },
            {
                width: '10rem', "render": function (link) {
                    return '<div style="max-width:10rem;word-break:break-all">'+ link+'</div>';
                }
            },
            { width : '10rem' }
        ], 
        columnDefs: [
            {"orderable": false, "targets": 3 }
        ],
        order: [[ 0, "desc" ]]
    });

    //按标题搜索
    $('#titleSearch').on( 'keyup change', function () {
        if ( table.column(1).search() !== this.value ) {
                table
                    .column(1)
                    .search( this.value )
                    .draw();
        }
    });

});