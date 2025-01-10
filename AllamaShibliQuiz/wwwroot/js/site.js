// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
let i = 0;
let txt = 'An engaging and challenging platform for students to showcase their academic knowledge, critical thinking skills and creativity.';
let speed = 50;
function typeWriter() {
    if (!document.getElementById("typeWriterText")) {
        return;
    }
    if (i < txt.length) {
        document.getElementById("typeWriterText").innerHTML += txt.charAt(i);
        i++;
        setTimeout(typeWriter, speed);
    }
}
typeWriter();

const studentsTable = document.getElementById('studentsTable');
if (studentsTable) {
    new DataTable('#studentsTable', {
        ordering: false,
        layout: {
            topStart: {
                buttons: ['excel', 'pdf']
            }
        }
    });

}

const myModalEl = document.getElementById('detailsModal');
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
    myModalEl.addEventListener('hide.bs.modal', event => {
        document.getElementById("member-detail-body").innerHTML = "";
    });
}

const myModalStudent = document.getElementById('studentModal');
if (myModalStudent) {
    myModalStudent.addEventListener('show.bs.modal', event => {
        var button = $(event.relatedTarget);
        var itemId = button.data('id');

        $.ajax({
            url: "/Admin/StudentDetails",
            data: { id: itemId },
            success: function (result) {
                document.getElementById("student-detail-body").innerHTML = result;
            }
        });
    });
    myModalStudent.addEventListener('hide.bs.modal', event => {
        document.getElementById("student-detail-body").innerHTML = "";
    });
}

//const homeModalElement = document.getElementById('welcomeModal')
//if (homeModalElement) {
//    const welcomeModal = new bootstrap.Modal('#welcomeModal');
//    if (welcomeModal) {
//        welcomeModal.show();
//    }
//}

var schoolSelect = document.getElementById('school');
if (schoolSelect) {
    var otherSchoolName = document.getElementById('otherSchoolName');
    if (schoolSelect.value === '0') {
        otherSchoolName.style.display = 'block';
    } else {
        otherSchoolName.style.display = 'none';
    }
}

function isNumber(evt) {
    evt = (evt) ? evt : window.event;
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if (charCode > 31 && (charCode < 48 || charCode > 57)) {
        return false;
    }
    return true;
}

function disableSubmitButton() {
    document.getElementById('submitButton').disabled = true;
}
