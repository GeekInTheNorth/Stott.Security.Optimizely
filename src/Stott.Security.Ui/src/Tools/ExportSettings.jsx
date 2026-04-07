import { useState } from 'react';
import axios from 'axios';
import PropTypes from 'prop-types';
import { Button, Modal } from 'react-bootstrap';
import ContextSelector from '../Common/ContextSelector';

function ExportSettings(props) {

    const [showModal, setShowModal] = useState(false);
    const [appId, setAppId] = useState(null);
    const [hostName, setHostName] = useState(null);

    const handleContextChange = (newAppId, newHostName) => {
        setAppId(newAppId);
        setHostName(newHostName);
    };

    const handleShowFailureToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(false, title, description);

    const downloadFile = ({ data, fileName, fileType }) => {
        const blob = new Blob([data], { type: fileType });
        const a = document.createElement('a');
        a.download = fileName;
        a.href = window.URL.createObjectURL(blob);
        const clickEvt = new MouseEvent('click', {
          view: window,
          bubbles: true,
          cancelable: true,
        });
        a.dispatchEvent(clickEvt);
        a.remove();
    }

    const getSettings = async () => {
        const params = {};
        if (appId) params.appId = appId;
        if (hostName) params.hostName = hostName;

        await axios
            .get(import.meta.env.VITE_TOOLS_EXPORT, { params })
            .then((response) => {
                downloadFile({
                    data: JSON.stringify(response.data),
                    fileName: 'stott-security-settings.json',
                    fileType: 'text/json',
                });
                setShowModal(false);
            },
            () => {
                handleShowFailureToast("Error", "Failed to retrieve settings to export.");
            });
    }

    const handleOpenModal = () => { setShowModal(true); };
    const handleCloseModal = () => { setShowModal(false); };

    return (
        <>
            <div className='my-4'>
                <label className='form-label'>Export all CSP, CORS and other security headers for a selected context.</label><br/>
                <Button variant='success' onClick={handleOpenModal}>Export</Button>
            </div>
            <Modal show={showModal} onHide={handleCloseModal} size='lg'>
                <Modal.Header closeButton className="py-2">
                    <Modal.Title>Export Settings</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <p>Select a source Application and/or Host Name context to export settings for.</p>
                    <ContextSelector appId={appId} hostName={hostName} onContextChange={handleContextChange} />
                </Modal.Body>
                <Modal.Footer>
                    <Button variant='success' onClick={getSettings}>Export</Button>
                    <Button variant='danger' onClick={handleCloseModal}>Cancel</Button>
                </Modal.Footer>
            </Modal>
        </>
    )
}

ExportSettings.propTypes = {
    showToastNotificationEvent: PropTypes.func
};

export default ExportSettings;
