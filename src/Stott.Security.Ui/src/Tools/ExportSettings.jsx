import { useState } from 'react';
import { httpGet } from '../Common/httpClient';
import PropTypes from 'prop-types';
import { Box, Button, Dialog, DialogBody, DialogClose, DialogContent, DialogFooter, DialogHeader, Text } from '@optiaxiom/react';
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

        await httpGet(import.meta.env.VITE_TOOLS_EXPORT, params)
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

    return (
        <>
            <Box my='24'>
                <Text mb='8'>Export all CSP, CORS and other security headers for a selected context.</Text>
                <Button appearance='primary' onClick={handleOpenModal}>Export</Button>
            </Box>
            <Dialog open={showModal} onOpenChange={setShowModal}>
                <DialogContent size='lg'>
                    <DialogHeader>Export Settings</DialogHeader>
                    <DialogBody>
                        <Text>Select a source Application and/or Host Name context to export settings for.</Text>
                        <ContextSelector appId={appId} hostName={hostName} onContextChange={handleContextChange} />
                    </DialogBody>
                    <DialogFooter>
                        <DialogClose appearance='default'>Cancel</DialogClose>
                        <Button appearance='primary' onClick={getSettings}>Export</Button>
                    </DialogFooter>
                </DialogContent>
            </Dialog>
        </>
    )
}

ExportSettings.propTypes = {
    showToastNotificationEvent: PropTypes.func
};

export default ExportSettings;
