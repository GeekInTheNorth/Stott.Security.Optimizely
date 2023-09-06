import React, { useState } from 'react';
import { Modal, Button } from "react-bootstrap";
import axios from 'axios';

const ConvertCspViolation = (props) => {

    const cspSourceSuggestions = props.cspSourceSuggestions ?? [ props.cspViolationUrl ];
    const cspDirectiveSuggestions = props.cspDirectiveSuggestions ?? [ props.cspViolationDirective ];
    
    const [showConvertModal, setShowConvertModal] = useState(false);
    const [selectedSource, setSelectedSource] = useState(cspSourceSuggestions[0])
    const [selectedDirective, setSelectedDirective] = useState(cspDirectiveSuggestions[0])

    const handleShowSuccessToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(true, title, description);
    const handleShowFailureToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(false, title, description);
    const handleCloseConvertModal = () => setShowConvertModal(false);
    const handleShowConvertModal = () => setShowConvertModal(true);

    const handleSelectSource = (event) => {
        setSelectedSource(event.target.value);
    }

    const handleSelectDirective = (event) => {
        setSelectedDirective(event.target.value);
    }

    const handleSaveCspViolation = () => {
        let params = new URLSearchParams();
        params.append('source', selectedSource);
        params.append('directive', selectedDirective);
        axios.post(process.env.REACT_APP_PERMISSION_APPEND_URL, params)
            .then(() => {
                handleShowSuccessToast('Source Saved', 'Successfully saved the source: ' + selectedSource);
                handleCloseConvertModal();
            },
            () => {
                handleShowFailureToast('Error', 'Failed to save the source: ' + selectedSource);
                handleCloseConvertModal();
            });
    }

    return (
        <>
            <Button variant='primary' onClick={handleShowConvertModal} className="mx-1">Create CSP Entry</Button>
            <Modal show={showConvertModal} onHide={handleCloseConvertModal}>
                <Modal.Header closeButton>
                    Whitelist CSP Violation
                </Modal.Header>
                <Modal.Body>
                    <p>Would you like to update your Content Security Policy with the following Source and Directive?</p>
                    <div className='input-group my-3'>
                        <label className='input-group-text'>Source</label>
                        <select className='form-select' value={selectedSource} onChange={handleSelectSource}>
                            { cspSourceSuggestions.map((item, index) => <option key={index}>{item}</option>)}
                        </select>
                    </div>
                    <div className='input-group my-3'>
                        <label className='input-group-text'>Directive</label>
                        <select className='form-select' value={selectedDirective} onChange={handleSelectDirective}>
                            { cspDirectiveSuggestions.map((item, index) => <option key={index}>{item}</option>)}
                        </select>
                    </div>
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