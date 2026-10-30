import PropTypes from 'prop-types';
import { AlertDialog, AlertDialogAction, AlertDialogBody, AlertDialogCancel, AlertDialogContent, AlertDialogFooter, AlertDialogHeader } from '@optiaxiom/react';

function ConfirmationModal(props) {
    const title = props.title ?? 'Confirm';
    const message = props.message ?? 'Are you sure?';
    const confirmLabel = props.confirmLabel ?? 'Confirm';
    const cancelLabel = props.cancelLabel ?? 'Cancel';
    const confirmAppearance = props.confirmAppearance ?? 'danger';

    const handleOpenChange = (open) => !open && props.onCancel();

    const handleConfirm = (event) => {
        // Prevent the action from closing the dialog via onOpenChange, the caller closes it in onConfirm
        event.preventDefault();
        props.onConfirm();
    };

    return (
        <AlertDialog open={props.show} onOpenChange={handleOpenChange}>
            <AlertDialogContent>
                <AlertDialogHeader>{title}</AlertDialogHeader>
                <AlertDialogBody>{message}</AlertDialogBody>
                <AlertDialogFooter>
                    <AlertDialogCancel>{cancelLabel}</AlertDialogCancel>
                    <AlertDialogAction appearance={confirmAppearance} onClick={handleConfirm}>{confirmLabel}</AlertDialogAction>
                </AlertDialogFooter>
            </AlertDialogContent>
        </AlertDialog>
    );
}

ConfirmationModal.propTypes = {
    show: PropTypes.bool.isRequired,
    title: PropTypes.string,
    message: PropTypes.node,
    confirmLabel: PropTypes.string,
    cancelLabel: PropTypes.string,
    confirmAppearance: PropTypes.string,
    onConfirm: PropTypes.func.isRequired,
    onCancel: PropTypes.func.isRequired
};

export default ConfirmationModal;
