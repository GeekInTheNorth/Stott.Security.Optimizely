import React, { useState } from 'react';
import { Modal, Button } from "react-bootstrap";
import axios from 'axios';

const ConvertCspViolation = (props) => {

    const [showConvertModal, setShowConvertModal] = useState(false);
    const cspViolationUrl = props.cspViolationUrl;
    const cspViolationDirective = props.cspViolationDirective;

    const handleShowSuccessToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(true, title, description);
    const handleShowFailureToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(false, title, description);
    const handleCloseConvertModal = () => setShowConvertModal(false);
    const handleShowConvertModal = () => setShowConvertModal(true);

    const handleSaveCspViolation = () => {
        let params = new URLSearchParams();
        params.append('source', cspViolationUrl);
        params.append('directive', cspViolationDirective);
        axios.post(process.env.REACT_APP_PERMISSION_APPEND_URL, params)
            .then(() => {
                handleShowSuccessToast('Source Saved', 'Successfully saved the source: ' + cspViolationUrl);
                handleCloseConvertModal();
            },
            () => {
                handleShowFailureToast('Error', 'Failed to save the source: ' + cspViolationUrl);
                handleCloseConvertModal();
            });
    };

    return (
        <>
            <Button variant='primary' onClick={handleShowConvertModal} className="mx-1">Create CSP Entry</Button>

            <Modal show={showConvertModal} onHide={handleCloseConvertModal}>
                <Modal.Header closeButton>
                    Whitelist CSP Violation
                </Modal.Header>
                <Modal.Body>
                    <p>Are you sure you wish to add <strong>{cspViolationUrl}</strong> with the directive of <strong>{cspViolationDirective}</strong> to your Content Security Policy?</p>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant='primary' type='submit' onClick={handleSaveCspViolation}>Yes</Button>
                    <Button variant='secondary' onClick={handleCloseConvertModal}>No</Button>
                </Modal.Footer>
            </Modal>
        </>
    )
}

export default ConvertCspViolation;