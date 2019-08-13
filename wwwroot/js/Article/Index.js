$(document).ready(function() {
    $.fn.dataTable.ext.errMode = 'none';

    var table = $('#dataTable').DataTable( {
        orderCellsTop: true,
        processing: true,
        language: {
            "url": "/js/dt-article.json"
        },
        ajax: {
            'url': "Article/AjaxGetArticles"
        },
        deferLoading : 10,
        autoWidth: false,
        columns : [
            { data:"ArticleID", width : '40px' },
            { data:"Title", width : '140px' },
            {
                data: "Author", width: '60px', "render": function (author) {
                    if (author.indexOf(admin_tag) !== -1) {
                        return author.replace(admin_tag,'') + '<span class="badge mx-1 badge-primary">' + admin_tag + '</span>'
                    } else if (author.indexOf(empty_user_tag) !== -1) {
                        return author.replace(empty_user_tag, '') + '<span class="badge mx-1 badge-primary">' + empty_user_tag + '</span>'
                    }
                    return author; 
                }
            },
            { data:"Tag", width : '100px' },
            {
                data: "AddTime", width: '100px', "render": function (time) {
                    return moment(time).format("YYYY-MM-DD");
                },
            },
            {
                data: "Status", width: '100px', "render": function (status) {
                    switch (status) {
                        case 1:
                        case '1':
                        case 'Approved':
                        case '已通过':
                            return '<p class="text-success">已通过</p>'
                            break;
                        case 0:
                        case '0':
                        case 'Submitted':
                        case '已提交':
                            return '<p class="text-primary">已提交</p>'
                            break;
                        case 2:
                        case '2':
                        case 'Rejected':
                        case '未通过':
                            return '<p class="text-danger">未通过</p>'
                            break;
                    }
                },
            },
            {
                width: '200px', "render": function (content, type, row) {
                    if (content) {
                        return content
                    } else {
                        return '<div class="d-flex flex-wrap">'
                            + '<a class="btn btn-sm btn-outline-warning mx-auto mt-1" href="Article/Edit/' + row.ArticleID + '"><i class="material-icons">edit</i></a>'
                            + '<a class="btn btn-sm btn-outline-info mx-auto mt-1" href="Article/Details/' + row.ArticleID + '"><i class="material-icons">list</i></a>'
                            + '<a href="#" data-toggle="modal" class="btn btn-outline-danger btn-sm mx-auto mt-1" data-target="#deleteModal" data-delete-id="' + row.ArticleID + '" data-name="' + row.Title +'" data-type="文章" data-field="标题">'
                            + '<i class="material-icons">delete</i></a>'
                            + '</div>';
                    }
                }
            }
        ], 
        columnDefs: [
            { "orderable": false, "targets": 6 }
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