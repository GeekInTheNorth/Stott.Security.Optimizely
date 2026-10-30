import { useState } from 'react';
import PropTypes from 'prop-types';
import { Button, Dialog, DialogBody, DialogContent, DialogFooter, DialogHeader, Field, Text } from "@optiaxiom/react";
import FormSelect from '../Common/FormSelect';
import { httpPost } from '../Common/httpClient';

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

    const handleSelectSource = (value) => {
        setSelectedSource(value);
    }

    const handleSelectDirective = (value) => {
        setSelectedDirective(value);
    }

    const handleSaveCspViolation = () => {
        let params = new URLSearchParams();
        params.append('source', selectedSource);
        params.append('directive', selectedDirective);
        if (props.appId) params.append('appId', props.appId);
        if (props.hostName) params.append('hostName', props.hostName);
        httpPost(import.meta.env.VITE_PERMISSION_APPEND_URL, params)
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
            <Button appearance='primary' onClick={handleShowConvertModal}>Add CSP Entry</Button>
            <Dialog open={showConvertModal} onOpenChange={setShowConvertModal}>
                <DialogContent>
                    <DialogHeader>Allow List CSP Violation</DialogHeader>
                    <DialogBody>
                        <Text>Are you sure you would like to update your Content Security Policy with the following Source and Directive?</Text>
                        <Field label='Source'>
                            <FormSelect value={selectedSource} onValueChange={handleSelectSource} options={cspSourceSuggestions.map((item) => ({ label: item, value: item }))} />
                        </Field>
                        <Field label='Directive'>
                            <FormSelect value={selectedDirective} onValueChange={handleSelectDirective} options={cspDirectiveSuggestions.map((item) => ({ label: item, value: item }))} />
                        </Field>
                    </DialogBody>
                    <DialogFooter>
                        <Button appearance='default' onClick={handleCloseConvertModal}>No</Button>
                        <Button appearance='primary' onClick={handleSaveCspViolation}>Yes</Button>
                    </DialogFooter>
                </DialogContent>
            </Dialog>
        </>
    )
}

ConvertCspViolation.propTypes = {
    cspViolationUrl: PropTypes.string.isRequired,
    cspViolationDirective: PropTypes.string.isRequired,
    cspSourceSuggestions: PropTypes.arrayOf(PropTypes.string),
    cspDirectiveSuggestions: PropTypes.arrayOf(PropTypes.string),
    showToastNotificationEvent: PropTypes.func,
    appId: PropTypes.string,
    hostName: PropTypes.string
};

export default ConvertCspViolation;
