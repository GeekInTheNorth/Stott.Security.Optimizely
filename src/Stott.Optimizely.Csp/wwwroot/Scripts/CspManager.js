$(document).ready(function () {

    let myModal = new bootstrap.Modal(document.getElementById('cspEditSourceModal'), {
        keyboard: false
    });

    $('.js-edit-directive-btn').click(function () {
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

        myModal.toggle();
    });
});
