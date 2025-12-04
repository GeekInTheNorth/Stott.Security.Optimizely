import { useState, useEffect } from 'react';
import PropTypes from 'prop-types';
import { Button, Container, Form } from 'react-bootstrap';
import axios from 'axios';
import FormUrl from '../Common/FormUrl';

function EditSettings(props) {

    const [isCspEnabled, setIsCspEnabled] = useState(false);
    const [isCspReportOnly, setIsCspReportOnly] = useState(false);
    const [isAllowListEnabled, setIsAllowListEnabled] = useState(false);
    
    const [allowListUrl, setAllowListUrl] = useState('');
    const [allowListUrlClassName, setAllowListUrlClassName] = useState('my-3 d-none');
    const [hasAllowListUrlError, setAllowListUrlError] = useState(false);
    const [allowListUrlErrorMessage, setAllowListUrlErrorMessage] = useState('');

    const [isUpgradeInSecureRequestsEnabled, setUpgradeInSecureRequestsEnabled] = useState(false);
    const [isInternalReportingEnabled, setIsInternalReportingEnabled] = useState(false);
    const [isExternalReportingEnabled, setIsExternalReportingEnabled] = useState(false);
    const [isExternalReportingClassName, setIsExternalReportingClassName] = useState('my-3 d-none');
    
    const [externalReportToUrl, setExternalReportToUrl] = useState('');
    const [hasExternalReportToUrl, setHasExternalReportToUrl] = useState(false);
    const [externalReportToUrlErrorMessage, setExternalReportToUrlErrorMessage] = useState('');
    const [disableSaveButton, setDisableSaveButton] = useState(true);

    useEffect(() => {
        getCspSettings()
    }, [])

    const getCspSettings = async () => {
        await axios.get(import.meta.env.VITE_SETTINGS_GET_URL)
            .then((response) => {
                let newAllowListVisbility = response.data.isAllowListEnabled ? 'my-3' : 'my-3 d-none';
                let newExternalUrlVisibility = response.data.useExternalReporting ? 'my-3' : 'my-3 d-none';

                setIsCspReportOnly(response.data.isReportOnly);
                setIsCspEnabled(response.data.isEnabled);
                setIsAllowListEnabled(response.data.isAllowListEnabled);
                setAllowListUrl(response.data.allowListUrl);
                setAllowListUrlClassName(newAllowListVisbility);
                setUpgradeInSecureRequestsEnabled(response.data.isUpgradeInsecureRequestsEnabled);
                setIsInternalReportingEnabled(response.data.useInternalReporting);
                setIsExternalReportingEnabled(response.data.useExternalReporting);
                setExternalReportToUrl(response.data.externalReportToUrl)
                setIsExternalReportingClassName(newExternalUrlVisibility);

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

    const handleAllowListAddress = (newUrl) => {
        setAllowListUrl(newUrl);
        setAllowListUrlError(false);
        setAllowListUrlErrorMessage('');
        setDisableSaveButton(false);
    }

    const handleUpgradeInsecureRequests = (event) => {
        setUpgradeInSecureRequestsEnabled(event.target.checked && isCspEnabled);
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

    const handleExternalReportToUrl = (newUrl) => {
        setExternalReportToUrl(newUrl);
        setHasExternalReportToUrl(false);
        setExternalReportToUrlErrorMessage('');
        setDisableSaveButton(false);
    }

    const handleShowSuccessToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(true, title, description);
    const handleShowFailureToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(false, title, description);

    const handleSaveSettings = (event) => {
        event.preventDefault();

        let params = new URLSearchParams();
        params.append('isEnabled', isCspEnabled);
        params.append('isReportOnly', isCspReportOnly);
        params.append('useInternalReporting', isInternalReportingEnabled);
        params.append('useExternalReporting', isExternalReportingEnabled);
        params.append('externalReportToUrl', externalReportToUrl)
        params.append('isAllowListEnabled', isAllowListEnabled);
        params.append('allowListUrl', allowListUrl);
        params.append('isUpgradeInsecureRequestsEnabled', isUpgradeInSecureRequestsEnabled);
        axios.post(import.meta.env.VITE_SETTINGS_SAVE_URL, params)
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
                    <FormUrl handleOnBlur={handleExternalReportToUrl} currentUrl={externalReportToUrl} hasInvalidResponse={hasExternalReportToUrl}></FormUrl>
                    {hasExternalReportToUrl ? <div className="invalid-feedback d-block">{externalReportToUrlErrorMessage}</div> : ''}
                </Form.Group>
                <Form.Group className='my-3'>
                    <Form.Check type='switch' label='Use Remote CSP Allow List' checked={isAllowListEnabled} onChange={handleIsAllowListEnabled} />
                    <div className='form-text'>Allow the use of a remote Content Security Policy allow list.  When a violation is detected, this allow list will be consulted and used to improve your configuration.</div>
                </Form.Group>
                <Form.Group className={allowListUrlClassName}>
                    <Form.Label>Remote CSP Allow List Address</Form.Label>
                    <FormUrl handleOnBlur={handleAllowListAddress} currentUrl={allowListUrl} hasInvalidResponse={hasAllowListUrlError}></FormUrl>
                    {hasAllowListUrlError ? <div className="invalid-feedback d-block">{allowListUrlErrorMessage}</div> : ''}
                </Form.Group>
                <Form.Group className='my-3'>
                    <Form.Check type='switch' label="Upgrade Insecure Requests" checked={isUpgradeInSecureRequestsEnabled} onChange={handleUpgradeInsecureRequests} />
                    <div className='form-text'>Instructs user agents (browsers) to treat all of this site's insecure URLs (those served over HTTP) as though they have been replaced with secure URLs (those served over HTTPS).  This is intended only for websites with a large number of legacy APIs that need rewriting.</div>
                </Form.Group>
                <Form.Group className='my-3'>
                    <Button type='submit' disabled={disableSaveButton} onClick={handleSaveSettings}>Save Changes</Button>
                </Form.Group>
            </Form>
        </Container>
    )
}

EditSettings.propTypes = {
    showToastNotificationEvent: PropTypes.func
};

export default EditSettings
