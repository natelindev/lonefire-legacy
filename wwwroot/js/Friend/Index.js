$(document).ready(function() {
    $.fn.dataTable.ext.errMode = 'none';

    var table = $('#dataTable').DataTable( {
        orderCellsTop: true,
        language: {
            "url": "/js/dt-friend.json"
        },
        autoWidth: false,
        columns : [
            { width : '20px' },
            { width : '40px' },
            { width : '20px' },
            { width : '100px' }
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