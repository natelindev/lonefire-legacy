
$(document).ready(function() {

    //用于实现删除的数据提示与实际的删除功能
    $('#deleteModal').on('show.bs.modal', function(e) {

        //get data-id attribute of the clicked element
        var ItemID = $(e.relatedTarget).data('delete-id');
        var ItemName = $(e.relatedTarget).data('name');
        var ItemType = $(e.relatedTarget).data('type');
        $('#textid').val(ItemID);
        $('#textName').val(ItemName);

        $('#deleteModalLabel').text("确定要删除该"+ ItemType +"?")
        $('#IDLabel').text("即将删除的"+ ItemType +"编号为:")
        $('#NameLabel').text("即将删除的"+ ItemType +"名称为:");

        $('#delete_input').val(ItemID);

    });

});