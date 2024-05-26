var dataTable;
$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/admin/productimage/getall' },
        "columns": [
            {
                data: 'imageUrl',
                "render": function (data) {
                    return `<img src="${data}" style="border-radius:5px; border:1px solid #bbb9b9; text-align: center; justify-content: center width: 100px; height: 100px" />`
                },
                "width": "15%"
            },
            { data: 'productTitle', "width": "15%" },
            {
                data: "id",
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                    <a href="/admin/productimage/upsert?id=${data}" class="btn btn-primary mx-2"> <i class="bi bi-pencil-square"></i> Edit </a>
                    <a onClick=Delete('/admin/productimage/delete/${data}') class="btn btn-danger mx-2"> <i class="bi bi-trash-fill"></i>Delete</a>
                    </div>`
                },
                "width": "25%"
            }
        ]
    })
}

function Delete(url) {
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    dataTable.ajax.reload();
                    toastr.success(data.message);
                }
            })
        }
    })
}