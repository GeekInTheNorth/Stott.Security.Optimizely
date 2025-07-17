import { useState } from "react";
import { Modal, Button } from "react-bootstrap";
import axios from 'axios';
import PropTypes from 'prop-types';

function CspSourceDelete(props) {
    
    const [showDeleteModal, setShowDeleteModal] = useState(false);
    const cspOriginalId = props.id;
    const cspOriginalSource = props.source;

    const handleReloadSources = () => props.reloadSources();
    const handleCloseDeleteModal = () => setShowDeleteModal(false);
    const handleShowDeleteModal = () => setShowDeleteModal(true);
    const handleShowSuccessToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(true, title, description);
    const handleShowFailureToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(false, title, description);

    const handleCommitDelete = () => {
        axios.delete(import.meta.env.VITE_APP_CSP_SOURCES_DELETE + cspOriginalId)
            .then(() => {
                // update visual state to match what has been saved.
                handleShowSuccessToast('Source Deleted', 'Successfully deleted the source: ' + cspOriginalSource);
                setShowDeleteModal(false);
                handleReloadSources();
            },
            () => {
                handleShowFailureToast('Error', 'Failed to delete the source: ' + cspOriginalSource);
            });
    };

    return (
        <>
            <Button variant='danger' onClick={handleShowDeleteModal} className="mx-1 text-nowrap">Delete</Button>

            <Modal show={showDeleteModal} onHide={handleCloseDeleteModal}>
                <Modal.Header>
                    Delete Source
                </Modal.Header>
                <Modal.Body>
                    <p>Are you sure you want to delete the following source?</p>
                    <p className='fw-bold'>{cspOriginalSource}</p>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant='danger' type='submit' onClick={handleCommitDelete}>Delete</Button>
                    <Button variant='secondary' onClick={handleCloseDeleteModal}>Close</Button>
                </Modal.Footer>
            </Modal>
        </>
    )
}

CspSourceDelete.propTypes = {
    id: PropTypes.string.isRequired,
    source: PropTypes.string.isRequired,
    reloadSources: PropTypes.func.isRequired,
    showToastNotificationEvent: PropTypes.func.isRequired
};

export default CspSourceDelete