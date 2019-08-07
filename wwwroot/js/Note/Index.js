$(document).ready(function() {
    $.fn.dataTable.ext.errMode = 'none';

    var table = $('#dataTable').DataTable( {
        orderCellsTop: true,
        language: {
            "url": "/js/dt-note.json"
        },
        autoWidth: false,
        columns : [
            { width : '40px' },
            { width: '140px' },
            {
                width: '60px', "render": function (status) {
                    switch (status) {
                        case 0:
                        case '0':
                        case '公开':
                        case 'Public':
                            return '<p class="text-success">公开</p>'
                            break;
                        case 1:
                        case '1':
                        case '私密':
                        case 'Private':
                            return '<p class="text-danger">私密</p>'
                            break;
                    }
                }
            },
            { width: '60px' },
            { width : '100px' }
        ], 
        columnDefs: [
            {"orderable": false, "targets": 4 }
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