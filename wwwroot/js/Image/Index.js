$(document).ready(function() {
    $.fn.dataTable.ext.errMode = 'none';

    var table = $('#dataTable').DataTable( {
        orderCellsTop: true,
        language: {
            "url": "/js/dt-image.json"
        },
        autoWidth: false,
        columns : [
            { width : '40px' },
            { width: '140px' },
            {width: '60px'},
            { width: '60px' },
            { width : '100px' }
        ], 
        columnDefs: [
            {"orderable": false, "targets": 4 }
        ],
        order: [[ 0, "desc" ]]
    });

    //按名称搜索
    $('#NameSearch').on( 'keyup change', function () {
        if ( table.column(1).search() !== this.value ) {
                table
                    .column(1)
                    .search( this.value )
                    .draw();
        }
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