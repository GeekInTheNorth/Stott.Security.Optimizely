import React, { useState } from "react";
import { Modal, Button } from "react-bootstrap";
import axios from 'axios';

function DeletePermission(props) {
    
    const [showDeleteModal, setShowDeleteModal] = useState(false);
    const cspOriginalId = props.id;
    const cspOriginalSource = props.source;

    const handleReloadSources = () => props.reloadSources();
    const handleCloseDeleteModal = () => setShowDeleteModal(false);
    const handleShowDeleteModal = () => setShowDeleteModal(true);
    const handleCommitDelete = () => {
        let params = new URLSearchParams();
        params.append('id', cspOriginalId );
        axios.post('https://localhost:44344/CspPermissions/Delete/', params)
            .then(() => {
                    // update visual state to match what has been saved.
                setShowDeleteModal(false);
                handleReloadSources();
            });
    };

    return (
        <>
            <Button variant='danger' onClick={handleShowDeleteModal} className="mx-1">Delete</Button>

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

export default DeletePermission