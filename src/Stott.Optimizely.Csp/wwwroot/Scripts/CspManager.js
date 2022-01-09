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
        let directivesList = directives.split(',');

        $('.js-modal-id').val(id);
        $('.js-modal-source').val(source);
        $('.js-modal-directive').prop('checked', false);

        for (const directive of directivesList) {
            $('input[data-directive=' + directive + ']').prop('checked', true);
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
                $('.js-success-source').text(source);
                editModal.hide();
                failureModal.hide();
                sucessModal.show();
            })
            .fail(function (data) {
                if (data.status === 400) {

                    editModal.show();
                    sucessModal.hide();
                    failureModal.hide();

                    let errors = data.responseJSON.Errors;
                    errors.forEach(function (error) {
                        if (error.PropertyName === 'Source') {
                            $('.js-edit-source-error').html(error.ErrorMessage);
                            $('.js-edit-source-error').addClass('d-inline');
                        } else if (error.PropertyName === 'Directives') {
                            $('.js-edit-directives-error').html(error.ErrorMessage);
                            $('.js-edit-directives-error').addClass('d-inline');
                        }
                    });
                }
                else {
                    $('.js-failure-source').text(source);
                    editModal.hide();
                    sucessModal.hide();
                    failureModal.show();
                }
            });
    });

    $('.js-create-source').click(function () {
        $('.js-modal-id').val('00000000-0000-0000-0000-000000000000');
        $('.js-modal-source').val('');
        $('.js-modal-directive').prop('checked', false);

        sucessModal.hide();
        failureModal.hide();
        editModal.show();
    });

    $('.js-modal-source').keypress(function () {
        $('.js-edit-source-error').removeClass('d-inline');
    });

    $('.js-modal-directive').change(function () {
        $('.js-edit-directives-error').removeClass('d-inline');
    });

});
