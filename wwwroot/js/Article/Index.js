$(document).ready(function() {
    $.fn.dataTable.ext.errMode = 'none';

    var table = $('#dataTable').DataTable( {
        orderCellsTop: true,
        language: {
            "url": "/js/dt-article.json"
        },
        autoWidth: false,
        columns : [
            { width : '40px' },
            { width : '140px' },
            { width : '60px' },
            { width : '100px' },
            { width : '100px' },
            { width : '100px' },
            { width : '200px' }
        ], 
        columnDefs: [
            {"orderable": false, "targets": 6 }
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

    //按作者搜索
    $('#authorSearch').on( 'keyup change', function () {
        if ( table.column(2).search() !== this.value ) {
                table
                    .column(2)
                    .search( this.value )
                    .draw();
        }
    });

    //按类型搜索
    $('#tagSearch').on( 'keyup change', function () {
        if ( table.column(3).search() !== this.value ) {
                table
                    .column(3)
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
      
    //按审核状态搜索
    $('#statusSearch').on( 'keyup change', function () {
        if ( table.column(5).search() !== this.value ) {
                table
                    .column(5)
                    .search( this.value )
                    .draw();
        }
    });

});