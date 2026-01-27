import { useState } from 'react';
import axios from 'axios';
import { Button, Modal } from 'react-bootstrap';

function DeleteSiteSecurityTxt(props) {

    const [showModal, setShowModal] = useState(false)

    const handleCloseModal = () => {
        setShowModal(false);
    }

    const handleShowDeleteModal = () => {
        setShowModal(true);
    }

    const handleDeleteContent = async () => {
        let url = ''.concat(import.meta.env.VITE_APP_SECURITYTXT_DELETE, props.id, '/');
        await axios.delete(url)
            .then(() => {
                handleShowSuccessToast('Success', 'Your security.txt content for \'' + props.appName + '\' was successfully deleted.');
                setShowModal(false);
                handleReload();
            },
            () => {
                handleShowFailureToast('Failure', 'An error was encountered when trying to delete your security.txt content for \'' + props.appName + '\'.');
                setShowModal(false);
            });
    }

    const handleShowSuccessToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(true, title, description);
    const handleShowFailureToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(false, title, description);
    const handleReload = () => props.reloadEvent && props.reloadEvent();

    return(
        <>
            <Button variant='danger' className='text-nowrap' onClick={handleShowDeleteModal}>Delete</Button>
            <Modal show={showModal}>
                <Modal.Header closeButton onClick={handleCloseModal}>
                    <Modal.Title>Delete Security.txt Configuration</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <label>Are you sure you want to delete this configuration for '{props.appName}'?</label>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant='danger' type='button' onClick={handleDeleteContent}>Delete</Button>
                    <Button variant='secondary' type='button' onClick={handleCloseModal}>Cancel</Button>
                </Modal.Footer>
            </Modal>
        </>
    )
}

export default DeleteSiteSecurityTxt;