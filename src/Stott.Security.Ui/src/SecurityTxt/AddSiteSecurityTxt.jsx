import { useState } from 'react'
import axios from 'axios';
import { Alert, Button, Modal } from 'react-bootstrap'
import ContextSelector from '../Common/ContextSelector';

function AddSiteSecurityTxt(props) {

    const [showModal, setShowModal] = useState(false);
    const [appId, setAppId] = useState(null);
    const [hostName, setHostName] = useState(null);
    const [siteContent, setSiteContent] = useState('');
    const [isDefault, setIsDefault] = useState(true);

    const handleCloseModal = () => {
        setShowModal(false);
    }

    const handleShowEditModal = () => {
        setAppId(null);
        setHostName(null);
        setSiteContent('');
        setIsDefault(true);
        setShowModal(true);
    }

    const handleContextChange = (newAppId, newHostName) => {
        setAppId(newAppId);
        setHostName(newHostName);
        setIsDefault(!newHostName);
    };

    const handleSaveSecurityTxtContent = async () => {
        let params = new URLSearchParams();
        params.append('appId', appId);
        params.append('specificHost', hostName || '');
        params.append('Content', siteContent);

        await axios.post(import.meta.env.VITE_APP_SECURITYTXT_SAVE, params)
            .then(() => {
                handleShowSuccessToast('Success', 'Your security.txt content changes were successfully applied.');
                setShowModal(false);
                handleReload();
            },
            (error) => {
                if (error.response && error.response.status === 409) {
                    handleShowFailureToast('Failure', error.response.data);
                    setShowModal(false);
                }
                else {
                    handleShowFailureToast('Failure', 'An error was encountered when trying to save your security.txt content.');
                    setShowModal(false);
                }
            });
    }

    const handleSiteContentChange = (event) => {
        setSiteContent(event.target.value);
    }

    const handleShowSuccessToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(true, title, description);
    const handleShowFailureToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(false, title, description);
    const handleReload = () => props.reloadEvent && props.reloadEvent();

    return(
        <>
            <Button variant='primary' onClick={handleShowEditModal} className='text-nowrap p-3'>Add Configuration</Button>
            <Modal show={showModal} size='xl'>
                <Modal.Header closeButton onClick={handleCloseModal}>
                    <Modal.Title>Create Security.txt Configuration</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <ContextSelector appId={appId} hostName={hostName} onContextChange={handleContextChange} />
                    <Alert variant='primary' show={isDefault} className='my-2 p-2'>
                        Please note that security.txt content for a host of 'Default' will be used where security.txt content has not been set for a specific host.
                    </Alert>
                    <div className='mb-3'>
                        <label>Security.txt Content</label>
                        <textarea className='form-control large-text-area' name='securityTxtContent' cols='60' rows='10' onChange={handleSiteContentChange} value={siteContent}></textarea>
                    </div>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant='primary' type='button' onClick={handleSaveSecurityTxtContent}>Save Changes</Button>
                    <Button variant='secondary' type='button' onClick={handleCloseModal}>Cancel</Button>
                </Modal.Footer>
            </Modal>
        </>
    )
}

export default AddSiteSecurityTxt;
