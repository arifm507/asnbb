// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

new DataTable('#studentsTable', {
    ordering: false,
    layout: {
        topStart: {
            buttons: ['excel', 'pdf']
        }
    }
});

const myModalEl = document.getElementById('detailsModal')
if (myModalEl) {
    myModalEl.addEventListener('show.bs.modal', event => {
        var button = $(event.relatedTarget);
        var itemId = button.data('id');

        $.ajax({
            url: "TeamMemberDetails",
            data: { id: itemId },
            success: function (result) {
                document.getElementById("member-detail-body").innerHTML = result;
            }
        });
    });
}

const myModalStudent = document.getElementById('studentModal')
if (myModalStudent) {
    myModalStudent.addEventListener('show.bs.modal', event => {
        var button = $(event.relatedTarget);
        var itemId = button.data('id');

        $.ajax({
            url: "StudentDetails",
            data: { id: itemId },
            success: function (result) {
                document.getElementById("student-detail-body").innerHTML = result;
            }
        });
    });
}