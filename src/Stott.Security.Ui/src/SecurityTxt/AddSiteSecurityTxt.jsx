import { useState } from 'react'
import { httpPost } from '../Common/httpClient';
import { Alert, Button, Dialog, DialogBody, DialogContent, DialogFooter, DialogHeader, Field, Textarea } from '@optiaxiom/react'
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

        await httpPost(import.meta.env.VITE_APP_SECURITYTXT_SAVE, params)
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
            <Button appearance='primary' size='lg' onClick={handleShowEditModal}>Add Configuration</Button>
            <Dialog open={showModal} onOpenChange={setShowModal}>
                <DialogContent size='lg'>
                    <DialogHeader>Create Security.txt Configuration</DialogHeader>
                    <DialogBody>
                        <ContextSelector appId={appId} hostName={hostName} onContextChange={handleContextChange} />
                        {isDefault && <Alert intent='information'>
                            Please note that security.txt content for a host of 'Default' will be used where security.txt content has not been set for a specific host.
                        </Alert>}
                        <Field label='Security.txt Content'>
                            <Textarea name='securityTxtContent' rows={10} onChange={handleSiteContentChange} value={siteContent} />
                        </Field>
                    </DialogBody>
                    <DialogFooter>
                        <Button appearance='default' type='button' onClick={handleCloseModal}>Cancel</Button>
                        <Button appearance='primary' type='button' onClick={handleSaveSecurityTxtContent}>Save Changes</Button>
                    </DialogFooter>
                </DialogContent>
            </Dialog>
        </>
    )
}

export default AddSiteSecurityTxt;
