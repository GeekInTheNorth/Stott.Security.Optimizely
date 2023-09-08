import React, { useState, useEffect } from 'react';
import { Alert, Button, Container, Form } from 'react-bootstrap';
import axios from 'axios';

function EditSettings(props) {

    const [isCspEnabled, setIsCspEnabled] = useState(false);
    const [isCspReportOnly, setIsCspReportOnly] = useState(false);
    const [isWhitelistEnabled, setIsWhitelistEnabled] = useState(false);
    const [whitelistUrl, setWhitelistUrl] = useState('');
    const [isUpgradeInSecureRequestsEnabled, setUpgradeInSecureRequestsEnabled] = useState(false);
    const [disableSaveButton, setDisableSaveButton] = useState(true);

    const [hasWhitelistUrlError, setWhitelistUrlError] =  useState(false);
    const [whitelistUrlErrorMessage, setWhitelistUrlErrorMessage] =  useState('');

    useEffect(() => {
        getCspSettings()
    }, [])

    const getCspSettings = async () => {
        const response = await axios.get(process.env.REACT_APP_SETTINGS_GET_URL);
        setIsCspReportOnly(response.data.isReportOnly);
        setIsCspEnabled(response.data.isEnabled);
        setIsWhitelistEnabled(response.data.isWhitelistEnabled);
        setWhitelistUrl(response.data.whitelistUrl);
        setUpgradeInSecureRequestsEnabled(response.data.isUpgradeInsecureRequestsEnabled);
        setDisableSaveButton(true);
    }

    const handleIsCspEnabledChange = (event) => { 
        setIsCspEnabled(event.target.checked); 
        setIsCspReportOnly(event.target.checked && isCspReportOnly);
        setUpgradeInSecureRequestsEnabled(event.target.checked && isUpgradeInSecureRequestsEnabled);
        setDisableSaveButton(false);
    }

    const handleIsCspReportOnly = (event) => {
        setIsCspReportOnly(event.target.checked && isCspEnabled);
        setDisableSaveButton(false);
    }

    const handleIsWhitelistEnabled = (event) => {
        setIsWhitelistEnabled(event.target.checked);
        setWhitelistUrlError(false);
        setWhitelistUrlErrorMessage('');
        setDisableSaveButton(false);
    }

    const handleWhitelistAddress = (event) => {
        setWhitelistUrl(event.target.value);
        setWhitelistUrlError(false);
        setWhitelistUrlErrorMessage('');
        setDisableSaveButton(false);
    }

    const handleUpgradeInsecureRequests = (event) => {
        setUpgradeInSecureRequestsEnabled(event.target.checked && isCspEnabled);
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
        params.append('whitelistUrl', whitelistUrl);
        params.append('isUpgradeInsecureRequestsEnabled', isUpgradeInSecureRequestsEnabled);
        axios.post(process.env.REACT_APP_SETTINGS_SAVE_URL, params)
            .then(() => {
                handleShowSuccessToast('Success', 'CSP Settings have been successfully saved.');
            }, (error) => {
                if(error.response.status === 400) {
                    var validationResult = error.response.data;
                    validationResult.errors.forEach(function (error) {
                        if (error.propertyName === 'WhitelistUrl') {
                            setWhitelistUrlError(true);
                            setWhitelistUrlErrorMessage(error.errorMessage);
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
                    <Form.Check type='switch' label='Enable Content Security Policy (CSP)' checked={isCspEnabled} onChange={handleIsCspEnabledChange} />
                    <div className='form-text'>Enabling the Content Security Policy will apply the header to all requests from both content routes and CMS backend routes.</div>
                </Form.Group>
                <Form.Group className='my-3'>
                    <Form.Check type='switch' label="Use Report Only Mode" checked={isCspReportOnly} onChange={handleIsCspReportOnly} />
                    <div className='form-text'>Only report violations of the Content Security Policy.</div>
                </Form.Group>
                <Form.Group className='my-3'>
                    <Form.Check type='switch' label='Use Remote CSP Whitelist' checked={isWhitelistEnabled} onChange={handleIsWhitelistEnabled} />
                    <div className='form-text'>Allow the use of a remote Content Security Policy whitelist.  When a violation is detected, this whitelist will be consulted and used to improve your configuration.</div>
                </Form.Group>
                <Form.Group className='my-3'>
                    <Form.Label>Remote CSP Whitelist Address</Form.Label>
                    <Form.Control type='text' placeholder='Enter Remote CSP Whitelist Address' value={whitelistUrl} onChange={handleWhitelistAddress} />
                    {hasWhitelistUrlError ? <div className="invalid-feedback d-block">{whitelistUrlErrorMessage}</div> : ""}
                </Form.Group>
                <Form.Group className='my-3'>
                    <Form.Check type='switch' label="Upgrade Insecure Requests" checked={isUpgradeInSecureRequestsEnabled} onChange={handleUpgradeInsecureRequests} />
                    <div className='form-text'>Instructs user agents (browsers) to treat all of this site's insecure URLs (those served over HTTP) as though they have been replaced with secure URLs (those served over HTTPS).</div>
                    <Alert variant='warning' show={isUpgradeInSecureRequestsEnabled} className='my-2 p-2'>
                        Please note that the Upgrade Insecure Requests setting is intended for web sites with insecure legacy URLs that need to be rewritten and should not normally be enabled.
                    </Alert>
                </Form.Group>
                <Form.Group className='my-3'>
                    <Button type='submit' disabled={disableSaveButton} onClick={handleSaveSettings}>Save Changes</Button>
                </Form.Group>
            </Form>
        </Container>
    )
}

export default EditSettings