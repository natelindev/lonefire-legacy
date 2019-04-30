$(document).ready(function() {
    var table = $('#dataTable').DataTable( {
        language: {
            "url": "/js/datatables_language_zh.json"
        },
        autoWidth: false,
        columns : [
            { width : '30px' },
            { width : '30px' },
            { width : '30px' },
            { width : '30px' },
            { width : '30px' }
        ], 
        columnDefs: [
            {"orderable": false, "targets": 0 },
            {"orderable": false, "targets": 1 },
            {"orderable": false, "targets": 2 },
            {"orderable": false, "targets": 3 },
            {"orderable": false, "targets": 4 }

        ]
    });

    //用于实现锁定用户的数据提示与实际的锁定功能
    $('#lockModal').on('show.bs.modal', function(e) {

        //get data-id attribute of the clicked element
        var lockItemID = $(e.relatedTarget).data('lock-id');
        var lockItemName = $(e.relatedTarget).data('name');
        var lockItemType = $(e.relatedTarget).data('type');
        $('#locktextid').val(lockItemID);
        $('#locktextName').val(lockItemName);

        $('#lockModalLabel').text("确定要锁定该"+ lockItemType +"?")
        $('#lockIDLabel').text("即将锁定的"+ lockItemType +"编号为:")
        $('#lockNameLabel').text("即将锁定的"+ lockItemType +"名称为:");

        $('#lock_input').val(lockItemID);

    });

    //锁定功能
    var str = '<option value="">取消锁定</option>';
    str += '<option value="'+moment().add(15, 'minutes').format('YYYY-MM-DD HH:mm:ss')+'">'+'15分钟'+'</option>';
    str += '<option value="'+moment().add(2, 'hours').format('YYYY-MM-DD HH:mm:ss')+'">'+'2小时'+'</option>';
    str += '<option value="'+moment().add(24, 'hours').format('YYYY-MM-DD HH:mm:ss')+'">'+'24小时'+'</option>';
    str += '<option value="'+moment().add(7, 'days').format('YYYY-MM-DD HH:mm:ss')+'">'+'7天'+'</option>';
    str += '<option value="'+moment().add(99, 'years').format('YYYY-MM-DD HH:mm:ss')+'">'+'永久'+'</option>';
    $('#lock_select').append(str);

    $('#lock_select').on( 'keyup change', function () {
        $('#lock_end').val(this.value);
    });
});

