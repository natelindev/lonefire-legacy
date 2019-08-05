$(document).ready(function() {
    $.fn.dataTable.ext.errMode = 'none';

    var table = $('#dataTable').DataTable( {
        orderCellsTop: true,
        language: {
            "url": "/js/dt-comment.json"
        },
        autoWidth: false,
        columns : [
            { width : '40px' },
            { width : '140px' },
            { width: '60px' },
            { width: '40px' },
            { width: '140px' },
            { width: '60px' },
            { width : '100px' }
        ], 
        columnDefs: [
            {"orderable": false, "targets": 6 }
        ],
        order: [[ 0, "desc" ]]
    });

    //按时间搜索
    $('#timeSearch').on( 'keyup change', function () {
        if ( table.column(4).search() !== this.value ) {
                table
                    .column(4)
                    .search( this.value )
                    .draw();
        }
    });

});