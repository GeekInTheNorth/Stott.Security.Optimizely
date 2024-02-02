import React, { useState, useEffect } from 'react';
import { Alert, Button, Container, Form } from 'react-bootstrap';
import axios from 'axios';

function EditSettings(props) {

    const [isCspEnabled, setIsCspEnabled] = useState(false);
    const [isCspReportOnly, setIsCspReportOnly] = useState(false);
    const [isAllowListEnabled, setIsAllowListEnabled] = useState(false);
    const [allowListUrl, setAllowListUrl] = useState('');
    const [allowListUrlClassName, setAllowListUrlClassName] = useState('my-3 d-none');
    const [isUpgradeInSecureRequestsEnabled, setUpgradeInSecureRequestsEnabled] = useState(false);
    const [isNonceEnabled, setIsNonceEnabled] = useState(false);
    const [isStrictDynamicEnabled, setIsStrictDynamicEnabled] = useState(false);

    const [disableSaveButton, setDisableSaveButton] = useState(true);

    const [hasAllowListUrlError, setAllowListUrlError] =  useState(false);
    const [allowListUrlErrorMessage, setAllowListUrlErrorMessage] =  useState('');

    useEffect(() => {
        getCspSettings()
    }, [])

    const getCspSettings = async () => {
        await axios.get(process.env.REACT_APP_SETTINGS_GET_URL)
            .then((response) => {
                let newAllowListVisbility = response.data.isAllowListEnabled ? 'my-3' : 'my-3 d-none';

                setIsCspReportOnly(response.data.isReportOnly);
                setIsCspEnabled(response.data.isEnabled);
                setIsAllowListEnabled(response.data.isAllowListEnabled);
                setAllowListUrl(response.data.allowListUrl);
                setAllowListUrlClassName(newAllowListVisbility);
                setUpgradeInSecureRequestsEnabled(response.data.isUpgradeInsecureRequestsEnabled);
                setIsNonceEnabled(response.data.isNonceEnabled);
                setIsStrictDynamicEnabled(response.data.isStrictDynamicEnabled);
                setDisableSaveButton(true);
            },
            () => {
                handleShowFailureToast('Error', 'Failed to load the Content Security Policy Settings.');
            });
    }

    const handleIsCspEnabledChange = (event) => { 
        setIsCspEnabled(event.target.checked); 
        setIsCspReportOnly(event.target.checked && isCspReportOnly);
        setUpgradeInSecureRequestsEnabled(event.target.checked && isUpgradeInSecureRequestsEnabled);
        setIsNonceEnabled(event.target.checked && isNonceEnabled);
        setIsStrictDynamicEnabled(event.target.checked && isStrictDynamicEnabled);
        setDisableSaveButton(false);
    }

    const handleIsCspReportOnly = (event) => {
        setIsCspReportOnly(event.target.checked && isCspEnabled);
        setDisableSaveButton(false);
    }

    const handleIsAllowListEnabled = (event) => {
        let newAllowListVisbility = event.target.checked ? 'my-3' : 'my-3 d-none';

        setIsAllowListEnabled(event.target.checked);
        setAllowListUrlError(false);
        setAllowListUrlErrorMessage('');
        setAllowListUrlClassName(newAllowListVisbility);
        setDisableSaveButton(false);
    }

    const handleAllowListAddress = (event) => {
        setAllowListUrl(event.target.value);
        setAllowListUrlError(false);
        setAllowListUrlErrorMessage('');
        setDisableSaveButton(false);
    }

    const handleUpgradeInsecureRequests = (event) => {
        setUpgradeInSecureRequestsEnabled(event.target.checked && isCspEnabled);
        setDisableSaveButton(false);
    }

    const handleIsNonceEnabled = (event) => {
        setIsNonceEnabled(event.target.checked && isCspEnabled);
        setDisableSaveButton(false);
    }

    const handleIsStrictDynamicEnabled = (event) => {
        setIsStrictDynamicEnabled(event.target.checked && isCspEnabled);
        setDisableSaveButton(false);
    }

    const handleShowSuccessToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(true, title, description);
    const handleShowFailureToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(false, title, description);

    const handleSaveSettings = (event) => {
        event.preventDefault();

        let params = new URLSearchParams();
        params.append('isEnabled', isCspEnabled);
        params.append('isReportOnly', isCspReportOnly);
        params.append('isAllowListEnabled', isAllowListEnabled);
        params.append('allowListUrl', allowListUrl);
        params.append('isUpgradeInsecureRequestsEnabled', isUpgradeInSecureRequestsEnabled);
        params.append('isNonceEnabled', isNonceEnabled);
        params.append('isStrictDynamicEnabled', isStrictDynamicEnabled);
        axios.post(process.env.REACT_APP_SETTINGS_SAVE_URL, params)
            .then(() => {
                handleShowSuccessToast('Success', 'CSP Settings have been successfully saved.');
            }, (error) => {
                if(error.response && error.response.status === 400) {
                    var validationResult = error.response.data;
                    validationResult.errors.forEach(function (error) {
                        if (error.propertyName === 'AllowListUrl') {
                            setAllowListUrlError(true);
                            setAllowListUrlErrorMessage(error.errorMessage);
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
                    <Form.Check type='switch' label='Use Remote CSP Allow List' checked={isAllowListEnabled} onChange={handleIsAllowListEnabled} />
                    <div className='form-text'>Allow the use of a remote Content Security Policy allow list.  When a violation is detected, this allow list will be consulted and used to improve your configuration.</div>
                </Form.Group>
                <Form.Group className={allowListUrlClassName}>
                    <Form.Label>Remote CSP Allow List Address</Form.Label>
                    <Form.Control type='text' placeholder='Enter Remote CSP Allow List Address' value={allowListUrl} onChange={handleAllowListAddress} />
                    {hasAllowListUrlError ? <div className="invalid-feedback d-block">{allowListUrlErrorMessage}</div> : ""}
                </Form.Group>
                <Form.Group className='my-3'>
                    <Form.Check type='switch' label="Upgrade Insecure Requests" checked={isUpgradeInSecureRequestsEnabled} onChange={handleUpgradeInsecureRequests} />
                    <div className='form-text'>Instructs user agents (browsers) to treat all of this site's insecure URLs (those served over HTTP) as though they have been replaced with secure URLs (those served over HTTPS).</div>
                    <Alert variant='warning' show={isUpgradeInSecureRequestsEnabled} className='my-2 p-2'>
                        Please note that the Upgrade Insecure Requests setting is intended for web sites with insecure legacy URLs that need to be rewritten and should not normally be enabled.
                    </Alert>
                </Form.Group>
                <Form.Group className='my-3'>
                    <Form.Check type='switch' label="Generate Nonce" checked={isNonceEnabled} onChange={handleIsNonceEnabled} />
                    <div className='form-text'>Generate a nonce value for script and style tags.  This is a unique value for each page request that prevents replay attacks.</div>
                    <Alert variant='warning' show={isNonceEnabled} className='my-2 p-2'>
                        Please note that the nonce will only be generated for content pages rendered to the website visitor and through the headers API for headless solutions. This is due to the CMS interface not being compatible with nonce enabled content security policies.
                    </Alert>
                </Form.Group>
                <Form.Group className='my-3'>
                    <Form.Check type='switch' label="Use Strict Dynamic" checked={isStrictDynamicEnabled} onChange={handleIsStrictDynamicEnabled} />
                    <div className='form-text'>By using 'strict-dynamic', trust can be extended from a script tag with a nonce or hash to any additional script it loads.</div>
                </Form.Group>
                <Form.Group className='my-3'>
                    <Button type='submit' disabled={disableSaveButton} onClick={handleSaveSettings}>Save Changes</Button>
                </Form.Group>
            </Form>
        </Container>
    )
}

export default EditSettings