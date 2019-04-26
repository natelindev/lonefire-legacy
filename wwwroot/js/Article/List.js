
$(document).ready(function() {

    var table = $('#dataTable').DataTable( {
        language: {
            "url": "/js/dt-article.json"
        },
        autoWidth: false,
        columns : [
            { width : '300px' },
            { width : '100px' }
        ],
        columnDefs: [
            {"orderable": false, "targets": 0 }
        ]
    });
                
});