import React, { useState, useEffect } from 'react';
import { Button, Container, Form } from 'react-bootstrap';
import axios from 'axios';

function EditSettings(props) {

    const [isCspEnabled, setIsCspEnabled] = useState(false);
    const [isCspReportOnly, setIsCspReportOnly] = useState(false);
    const [isAllowListEnabled, setIsAllowListEnabled] = useState(false);
    
    const [allowListUrl, setAllowListUrl] = useState('');
    const [allowListUrlClassName, setAllowListUrlClassName] = useState('my-3 d-none');
    const [hasAllowListUrlError, setAllowListUrlError] =  useState(false);
    const [allowListUrlErrorMessage, setAllowListUrlErrorMessage] =  useState('');

    const [isUpgradeInSecureRequestsEnabled, setUpgradeInSecureRequestsEnabled] = useState(false);
    const [isNonceEnabled, setIsNonceEnabled] = useState(false);
    const [isStrictDynamicEnabled, setIsStrictDynamicEnabled] = useState(false);
    const [isInternalReportingEnabled, setIsInternalReportingEnabled] = useState(false);
    const [isExternalReportingEnabled, setIsExternalReportingEnabled] = useState(false);
    const [isExternalReportingClassName, setIsExternalReportingClassName] = useState('my-3 d-none');
    
    const [externalReportToUrl, setExternalReportToUrl] = useState('');
    const [hasExternalReportToUrl, setHasExternalReportToUrl] =  useState(false);
    const [externalReportToUrlErrorMessage, setExternalReportToUrlErrorMessage] =  useState('');
    
    const [externalReportUriUrl, setExternalReportUriUrl] = useState('');
    const [hasExternalReportUriUrl, setHasExternalReportUriUrl] =  useState(false);
    const [externalReportUriUrlErrorMessage, setExternalReportUriUrlErrorMessage] =  useState('');

    const [disableSaveButton, setDisableSaveButton] = useState(true);

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
        setIsStrictDynamicEnabled(event.target.checked && isNonceEnabled && isStrictDynamicEnabled);
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
        setIsStrictDynamicEnabled(event.target.checked && isCspEnabled && isStrictDynamicEnabled);
        setDisableSaveButton(false);
    }

    const handleIsStrictDynamicEnabled = (event) => {
        setIsStrictDynamicEnabled(event.target.checked && isCspEnabled && isNonceEnabled);
        setDisableSaveButton(false);
    }

    const handleIsInternalReportingEnabled = (event) => {
        setIsInternalReportingEnabled(event.target.checked && isCspEnabled);
        setDisableSaveButton(false);
    }

    const handleIsExternalReportingEnabled = (event) => {
        let newExternalReportingEnabled = event.target.checked && isCspEnabled;
        let newExternalReportingVisbility = newExternalReportingEnabled ? 'my-3' : 'my-3 d-none';

        setIsExternalReportingEnabled(newExternalReportingEnabled);
        setIsExternalReportingClassName(newExternalReportingVisbility);
        setDisableSaveButton(false);
    }

    const handleExternalReportToUrl = (event) => {
        setExternalReportToUrl(event.target.value);
        setHasExternalReportToUrl(false);
        setExternalReportToUrlErrorMessage('');
        setDisableSaveButton(false);
    }

    const handleExternalReportUriUrl = (event) => {
        setExternalReportUriUrl(event.target.value);
        setHasExternalReportUriUrl(false);
        setExternalReportUriUrlErrorMessage('');
        setDisableSaveButton(false);
    }

    const handleShowSuccessToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(true, title, description);
    const handleShowFailureToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(false, title, description);

    const handleSaveSettings = (event) => {
        event.preventDefault();

        let params = new URLSearchParams();
        params.append('isEnabled', isCspEnabled);
        params.append('isReportOnly', isCspReportOnly);
        params.append('isInternalReportingEnabled', isInternalReportingEnabled);
        params.append('isExternalReportingEnabled', isExternalReportingEnabled);
        params.append('externalReportToUrl', externalReportToUrl)
        params.append('externalReportUriUrl', externalReportUriUrl)
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
                    var toastMessage = 'Unable to save the CSP Settings.';
                    validationResult.errors.forEach(function (error) {
                        if (error.propertyName === 'AllowListUrl') {
                            setAllowListUrlError(true);
                            setAllowListUrlErrorMessage(error.errorMessage);
                            toastMessage += ' ' + error.errorMessage;
                        } else if (error.propertyName === 'ExternalReportToUrl') {
                            setHasExternalReportToUrl(true);
                            setExternalReportToUrlErrorMessage(error.errorMessage);
                            toastMessage += ' ' + error.errorMessage;
                        } else if (error.propertyName === 'ExternalReportUriUrl') {
                            setHasExternalReportUriUrl(true);
                            setExternalReportUriUrlErrorMessage(error.errorMessage);
                            toastMessage += ' ' + error.errorMessage;
                        }
                    });

                    handleShowFailureToast('Error', toastMessage);
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
                    <Form.Check type='switch' label='Use Report Only Mode' checked={isCspReportOnly} onChange={handleIsCspReportOnly} />
                    <div className='form-text'>Only report violations of the Content Security Policy within the browser console and to enabled reporting endpoints.</div>
                </Form.Group>
                <Form.Group className='my-3'>
                    <Form.Check type='switch' label='Use Internal Reporting Endpoints' checked={isInternalReportingEnabled} onChange={handleIsInternalReportingEnabled} />
                    <div className='form-text'>Report Content Security Policy violations to this Add-Ons reporting endpoints.</div>
                </Form.Group>
                <Form.Group className='my-3'>
                    <Form.Check type='switch' label='Use External Reporting Endpoints' checked={isExternalReportingEnabled} onChange={handleIsExternalReportingEnabled} />
                    <div className='form-text'>Report Content Security Policy violations to external reporting endpoints.</div>
                </Form.Group>
                <Form.Group className={isExternalReportingClassName}>
                    <Form.Label>External Report-To Endpoint</Form.Label>
                    <Form.Control type='text' placeholder='Enter external remote report-to address' value={externalReportToUrl} onChange={handleExternalReportToUrl} />
                    {hasExternalReportToUrl ? <div className="invalid-feedback d-block">{externalReportToUrlErrorMessage}</div> : ''}
                </Form.Group>
                <Form.Group className={isExternalReportingClassName}>
                    <Form.Label>External Report-Uri Endpoint</Form.Label>
                    <Form.Control type='text' placeholder='Enter external remote report-uri address' value={externalReportUriUrl} onChange={handleExternalReportUriUrl} />
                    {hasExternalReportUriUrl ? <div className="invalid-feedback d-block">{externalReportUriUrlErrorMessage}</div> : ''}
                </Form.Group>
                <Form.Group className='my-3'>
                    <Form.Check type='switch' label='Use Remote CSP Allow List' checked={isAllowListEnabled} onChange={handleIsAllowListEnabled} />
                    <div className='form-text'>Allow the use of a remote Content Security Policy allow list.  When a violation is detected, this allow list will be consulted and used to improve your configuration.</div>
                </Form.Group>
                <Form.Group className={allowListUrlClassName}>
                    <Form.Label>Remote CSP Allow List Address</Form.Label>
                    <Form.Control type='text' placeholder='Enter Remote CSP Allow List Address' value={allowListUrl} onChange={handleAllowListAddress} />
                    {hasAllowListUrlError ? <div className="invalid-feedback d-block">{allowListUrlErrorMessage}</div> : ''}
                </Form.Group>
                <Form.Group className='my-3'>
                    <Form.Check type='switch' label="Upgrade Insecure Requests" checked={isUpgradeInSecureRequestsEnabled} onChange={handleUpgradeInsecureRequests} />
                    <div className='form-text'>Instructs user agents (browsers) to treat all of this site's insecure URLs (those served over HTTP) as though they have been replaced with secure URLs (those served over HTTPS).  This is intended only for websites with a large number of legacy APIs that need rewriting.</div>
                </Form.Group>
                <Form.Group className='my-3'>
                    <Form.Check type='switch' label="Generate Nonce" checked={isNonceEnabled} onChange={handleIsNonceEnabled} />
                    <div className='form-text'>Generate a nonce value for script and style tags.  This is a unique value for each page request that prevents replay attacks.</div>
                    <div className='form-text'>Please note that the nonce will only be generated for content pages rendered to the website visitor and through the headers API for headless solutions. This is due to the CMS interface not being compatible with nonce enabled content security policies.</div>
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