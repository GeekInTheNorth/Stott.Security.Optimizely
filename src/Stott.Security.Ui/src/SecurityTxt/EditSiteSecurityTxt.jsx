import { useState } from 'react'
import { httpGet, httpPost } from '../Common/httpClient';
import { Alert, Button, Dialog, DialogBody, DialogContent, DialogFooter, DialogHeader, Field, Textarea } from '@optiaxiom/react'
import ContextSelector from '../Common/ContextSelector';

function EditSiteSecurityTxt(props) {

    const [showModal, setShowModal] = useState(false)
    const [id, setId] = useState(props.id ?? '')
    const [appId, setAppId] = useState(props.appId ?? '')
    const [siteContent, setSiteContent] = useState('')
    const [isDefault, setIsDefault] = useState(true)
    const [specificHost, setSpecificHost] = useState('')
    const [isEditable, setIsEditable] = useState(true)

    const handleSiteContentChange = (event) => {
        setSiteContent(event.target.value);
    }

    const handleContextChange = (newAppId, newHostName) => {
        setAppId(newAppId || '');
        setSpecificHost(newHostName || '');
        setIsDefault(!newHostName);
    };

    const handleShowEditModal = async () => {
        await httpGet(import.meta.env.VITE_APP_SECURITYTXT_EDIT, { id: id, appId: appId })
            .then((response) => {
                if (response.data) {
                    setId(response.data.id);
                    setAppId(response.data.appId);
                    setSiteContent(response.data.content);
                    setIsDefault(!response.data.specificHost);
                    setSpecificHost(response.data.specificHost ?? '');
                    setIsEditable(response.data.isEditable ?? true);
                    setShowModal(true);
                }
                else{
                    handleShowFailureToast('Failure', 'An error was encountered when trying to retrieve your security.txt content.');
                }
            },
            () => {
                handleShowFailureToast('Failure', 'An error was encountered when trying to retrieve your security.txt content.');
            });
    }

    const handleSaveSecurityTxtContent = async () => {
        let params = new URLSearchParams();
        params.append('id', id);
        params.append('appId', appId);
        params.append('specificHost', specificHost);
        params.append('content', siteContent);

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

    const handleCloseModal = () => {
        setShowModal(false);
    }

    const handleShowSuccessToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(true, title, description);
    const handleShowFailureToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(false, title, description);
    const handleReload = () => props.reloadEvent && props.reloadEvent();

    return (
        <>
            <Button appearance='primary' onClick={handleShowEditModal}>Edit</Button>
            <Dialog open={showModal} onOpenChange={setShowModal}>
                <DialogContent size='lg'>
                    <DialogHeader>Edit Security.txt Configuration</DialogHeader>
                    <DialogBody>
                        <ContextSelector appId={appId} hostName={specificHost || null} onContextChange={handleContextChange} />
                        {isDefault && <Alert intent='information'>
                            Please note that security.txt content for a host of 'Default' will be used where security.txt content has not been set for a specific host.
                        </Alert>}
                        <Field label='Security.txt Content'>
                            <Textarea name='securityTxtContent' rows={10} onChange={handleSiteContentChange} value={siteContent} />
                        </Field>
                    </DialogBody>
                    <DialogFooter>
                        <Button appearance='default' type='button' onClick={handleCloseModal}>Cancel</Button>
                        {isEditable && <Button appearance='primary' type='button' onClick={handleSaveSecurityTxtContent}>Save Changes</Button>}
                    </DialogFooter>
                </DialogContent>
            </Dialog>
        </>
    )
}

export default EditSiteSecurityTxt;
