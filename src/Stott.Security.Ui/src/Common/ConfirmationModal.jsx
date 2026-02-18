import PropTypes from 'prop-types';
import { Button, Modal } from 'react-bootstrap';

function ConfirmationModal(props) {
    const title = props.title ?? 'Confirm';
    const message = props.message ?? 'Are you sure?';
    const confirmLabel = props.confirmLabel ?? 'Confirm';
    const cancelLabel = props.cancelLabel ?? 'Cancel';
    const confirmVariant = props.confirmVariant ?? 'danger';

    return (
        <Modal show={props.show} onHide={props.onCancel}>
            <Modal.Header closeButton>
                <Modal.Title>{title}</Modal.Title>
            </Modal.Header>
            <Modal.Body>{message}</Modal.Body>
            <Modal.Footer>
                <Button variant={confirmVariant} onClick={props.onConfirm}>{confirmLabel}</Button>
                <Button variant='secondary' onClick={props.onCancel}>{cancelLabel}</Button>
            </Modal.Footer>
        </Modal>
    );
}

ConfirmationModal.propTypes = {
    show: PropTypes.bool.isRequired,
    title: PropTypes.string,
    message: PropTypes.node,
    confirmLabel: PropTypes.string,
    cancelLabel: PropTypes.string,
    confirmVariant: PropTypes.string,
    onConfirm: PropTypes.func.isRequired,
    onCancel: PropTypes.func.isRequired
};

export default ConfirmationModal;
