$(document).ready(function () {

    let editModal = new bootstrap.Modal(document.getElementById('cspEditSourceModal'), {
        keyboard: false
    });

    let sucessModal = new bootstrap.Modal(document.getElementById('cspSuccessModal'), {
        keyboard: false
    });

    let failureModal = new bootstrap.Modal(document.getElementById('cspFailureModal'), {
        keyboard: false
    });

    $('.js-edit-source-btn').click(function () {
        let id = $(this).data('id');
        let source = $(this).data('source');
        let directives = $(this).data('directives');
        let directivesList = directives.split(",");

        $('.js-modal-id').val(id);
        $('.js-modal-source').val(source);
        $('.js-modal-directive').prop("checked", false);

        for (const directive of directivesList) {
            $('input[data-directive=' + directive + ']').prop("checked", true);
        }

        editModal.show();
    });

    $('.js-save-source-btn').click(function () {
        let id = $('.js-modal-id').val();
        let source = $('.js-modal-source').val();
        let directives = [];

        $('input.js-modal-directive:checked').each(function (){
            directives.push($(this).data('directive'));
        })

        $.post('/CspPermissions/Save/', { id: id, source: source, directives: directives })
            .done(function () {
                $(".js-success-source").text(source);
                editModal.hide();
                sucessModal.show();
            })
            .fail(function () {
                $(".js-failure-source").text(source);
                editModal.hide();
                failureModal.show();
            });
    });
});
