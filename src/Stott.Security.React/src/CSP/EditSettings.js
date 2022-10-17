import React, { useState, useEffect } from 'react';
import { Button, Container, Form } from 'react-bootstrap';
import axios from 'axios';

function EditSettings(props) {

    const [isCspEnabled, setIsCspEnabled] = useState(false);
    const [isCspReportOnly, setIsCspReportOnly] = useState(false);
    const [isWhitelistEnabled, setIsWhitelistEnabled] = useState(false);
    const [whitelistAddress, setWhitelistAddress] = useState('');
    const [disableSaveButton, setDisableSaveButton] = useState(true);

    const [hasWhitelistAddressError, setWhitelistAddressError] =  useState(false);
    const [whitelistAddressErrorMessage, setWhitelistAddressErrorMessage] =  useState('');

    useEffect(() => {
        getCspSettings()
    }, [])

    const getCspSettings = async () => {
        const response = await axios.get(process.env.REACT_APP_SETTINGS_GET_URL)
        setIsCspReportOnly(response.data.isReportOnly);
        setIsCspEnabled(response.data.isEnabled);
        setIsWhitelistEnabled(response.data.isWhitelistEnabled);
        setWhitelistAddress(response.data.whitelistAddress);
        setDisableSaveButton(true);
    }

    const handleIsCspEnabledChange = (event) => { 
        setIsCspEnabled(event.target.checked); 
        setIsCspReportOnly(event.target.checked && isCspReportOnly);
        setDisableSaveButton(false);
    }

    const handleIsCspReportOnly = (event) => {
        setIsCspReportOnly(event.target.checked && isCspEnabled);
        setDisableSaveButton(false);
    }

    const handleIsWhitelistEnabled = (event) => {
        setIsWhitelistEnabled(event.target.checked);
        setWhitelistAddressError(false);
        setWhitelistAddressErrorMessage('');
        setDisableSaveButton(false);
    }

    const handleWhitelistAddress = (event) => {
        setWhitelistAddress(event.target.value);
        setWhitelistAddressError(false);
        setWhitelistAddressErrorMessage('');
        setDisableSaveButton(false);
    }

    const handleShowSuccessToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(true, title, description);
    const handleShowFailureToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(false, title, description);

    const handleSaveSettings = (event) => {
        event.preventDefault();

        let params = new URLSearchParams();
        params.append('isEnabled', isCspEnabled);
        params.append('isReportOnly', isCspReportOnly);
        params.append('isWhitelistEnabled', isWhitelistEnabled);
        params.append('whitelistAddress', whitelistAddress);
        axios.post(process.env.REACT_APP_SETTINGS_SAVE_URL, params)
            .then(() => {
                handleShowSuccessToast('Success', 'CSP Settings have been successfully saved.');
            }, (error) => {
                if(error.response.status === 400) {
                    var validationResult = error.response.data;
                    validationResult.errors.forEach(function (error) {
                        if (error.propertyName === 'WhitelistAddress') {
                            setWhitelistAddressError(true);
                            setWhitelistAddressErrorMessage(error.errorMessage);
                        }
                    });
                }
                else{
                    handleShowFailureToast('Error', 'Failed to save the CSP Settings.');
                }
            });
        setDisableSaveButton(true);
    }

    return (
        <Container fluid='md'>
            <Form>
                <Form.Group className='my-3'>
                    <Form.Check type='switch' label='Active' checked={isCspEnabled} onChange={handleIsCspEnabledChange} />
                    <div className='form-text'>Enabling the Content Security Policy will apply the header to content pages only.</div>
                </Form.Group>
                <Form.Group className='my-3'>
                    <Form.Check type='switch' label="Report Only Mode" checked={isCspReportOnly} onChange={handleIsCspReportOnly} />
                    <div className='form-text'>Only report violations of the Content Security Policy.</div>
                </Form.Group>
                <Form.Group className='my-3'>
                    <Form.Check type='switch' label='Use Remote CSP Whitelist' checked={isWhitelistEnabled} onChange={handleIsWhitelistEnabled} />
                    <div className='form-text'>Allow the use of a remote Content Security Policy whitelist.  When a violation is detected, this whitelist will be consulted and used to improve your configuration.</div>
                </Form.Group>
                <Form.Group className='my-3'>
                    <Form.Label>Remote CSP Whitelist Address</Form.Label>
                    <Form.Control type='text' placeholder='Enter Remote CSP Whitelist Address' value={whitelistAddress} onChange={handleWhitelistAddress} />
                    {hasWhitelistAddressError ? <div className="invalid-feedback d-block">{whitelistAddressErrorMessage}</div> : ""}
                </Form.Group>
                <Form.Group className='my-3'>
                    <Button type='submit' disabled={disableSaveButton} onClick={handleSaveSettings}>Save Changes</Button>
                </Form.Group>
            </Form>
        </Container>
    )
}

export default EditSettings