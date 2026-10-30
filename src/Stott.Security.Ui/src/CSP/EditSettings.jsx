import { useState, useEffect } from 'react';
import PropTypes from 'prop-types';
import { Alert, Box, Button, Field, Group, Switch, Text } from '@optiaxiom/react';
import { httpGet, httpPost, httpDelete } from '../Common/httpClient';
import FormUrl from '../Common/FormUrl';

function EditSettings(props) {

    const [isInherited, setIsInherited] = useState(false);
    const [isCspEnabled, setIsCspEnabled] = useState(false);
    const [isCspReportOnly, setIsCspReportOnly] = useState(false);
    const [isAllowListEnabled, setIsAllowListEnabled] = useState(false);
    
    const [allowListUrl, setAllowListUrl] = useState('');
    const [allowListUrlDisplay, setAllowListUrlDisplay] = useState('none');
    const [hasAllowListUrlError, setAllowListUrlError] = useState(false);
    const [allowListUrlErrorMessage, setAllowListUrlErrorMessage] = useState('');

    const [isUpgradeInSecureRequestsEnabled, setUpgradeInSecureRequestsEnabled] = useState(false);
    const [isInternalReportingEnabled, setIsInternalReportingEnabled] = useState(false);
    const [isExternalReportingEnabled, setIsExternalReportingEnabled] = useState(false);
    const [externalReportingDisplay, setExternalReportingDisplay] = useState('none');
    
    const [externalReportToUrl, setExternalReportToUrl] = useState('');
    const [hasExternalReportToUrl, setHasExternalReportToUrl] = useState(false);
    const [externalReportToUrlErrorMessage, setExternalReportToUrlErrorMessage] = useState('');
    const [disableSaveButton, setDisableSaveButton] = useState(true);

    useEffect(() => {
        getCspSettings()
    }, [props.appId, props.hostName])

    const getCspSettings = async () => {
        await httpGet(import.meta.env.VITE_SETTINGS_GET_URL, { appId: props.appId, hostName: props.hostName })
            .then((response) => {
                let newAllowListVisbility = response.data.isAllowListEnabled ? 'flex' : 'none';
                let newExternalUrlVisibility = response.data.useExternalReporting ? 'flex' : 'none';

                setIsCspReportOnly(response.data.isReportOnly);
                setIsCspEnabled(response.data.isEnabled);
                setIsAllowListEnabled(response.data.isAllowListEnabled);
                setAllowListUrl(response.data.allowListUrl);
                setAllowListUrlDisplay(newAllowListVisbility);
                setUpgradeInSecureRequestsEnabled(response.data.isUpgradeInsecureRequestsEnabled);
                setIsInternalReportingEnabled(response.data.useInternalReporting);
                setIsExternalReportingEnabled(response.data.useExternalReporting);
                setExternalReportToUrl(response.data.externalReportToUrl)
                setExternalReportingDisplay(newExternalUrlVisibility);
                setIsInherited(response.data.isInherited ?? false);

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
        let newAllowListVisbility = event.target.checked ? 'flex' : 'none';

        setIsAllowListEnabled(event.target.checked);
        setAllowListUrlError(false);
        setAllowListUrlErrorMessage('');
        setAllowListUrlDisplay(newAllowListVisbility);
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
        let newExternalReportingVisbility = newExternalReportingEnabled ? 'flex' : 'none';

        setIsExternalReportingEnabled(newExternalReportingEnabled);
        setExternalReportingDisplay(newExternalReportingVisbility);
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

    const handleOverrideSettings = () => {
        setIsInherited(false);
        setDisableSaveButton(false);
    };

    const handleRevertToInherited = async () => {
        try {
            await httpDelete(import.meta.env.VITE_SETTINGS_DELETE_URL, { appId: props.appId, hostName: props.hostName });
            handleShowSuccessToast('Success', 'Settings reverted to inherited.');
            getCspSettings();
        } catch {
            handleShowFailureToast('Error', 'Failed to revert settings.');
        }
    };

    const isContextSpecific = !!(props.appId || props.hostName);

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
        if (props.appId) params.append('appId', props.appId);
        if (props.hostName) params.append('hostName', props.hostName);
        httpPost(import.meta.env.VITE_SETTINGS_SAVE_URL, params)
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
        <Box className='security-page-container'>
            {isContextSpecific && isInherited && (
                <Alert intent='information' my='16'>
                    <Group justifyContent='space-between' gap='16' w='full'>
                        <Text>These settings are inherited from the parent configuration.</Text>
                        <Button appearance='primary' size='sm' onClick={handleOverrideSettings}>Override for this context</Button>
                    </Group>
                </Alert>
            )}
            {isContextSpecific && !isInherited && (
                <Alert intent='warning' my='16'>
                    <Group justifyContent='space-between' gap='16' w='full'>
                        <Text>These settings override the inherited configuration.</Text>
                        <Button appearance='danger-outline' size='sm' onClick={handleRevertToInherited}>Revert to inherited</Button>
                    </Group>
                </Alert>
            )}
            <form>
                <fieldset className='security-fieldset' disabled={isContextSpecific && isInherited}>
                <Box my='16'>
                    <Switch checked={isCspEnabled} onChange={handleIsCspEnabledChange} description='Enabling the Content Security Policy will apply the header to all requests from both content routes and CMS backend routes.'>Enable Content Security Policy (CSP)</Switch>
                </Box>
                <Box my='16'>
                    <Switch checked={isCspReportOnly} onChange={handleIsCspReportOnly} description='Only report violations of the Content Security Policy within the browser console and to enabled reporting endpoints.'>Use Report Only Mode</Switch>
                </Box>
                <Box my='16'>
                    <Switch checked={isInternalReportingEnabled} onChange={handleIsInternalReportingEnabled} description='Report Content Security Policy violations to this Add-Ons reporting endpoints.'>Use Internal Reporting Endpoints</Switch>
                </Box>
                <Box my='16'>
                    <Switch checked={isExternalReportingEnabled} onChange={handleIsExternalReportingEnabled} description='Report Content Security Policy violations to external reporting endpoints.'>Use External Reporting Endpoints</Switch>
                </Box>
                <Field label='External Report-To Endpoint' display={externalReportingDisplay} my='16' error={hasExternalReportToUrl ? externalReportToUrlErrorMessage : undefined}>
                    <FormUrl handleOnBlur={handleExternalReportToUrl} currentUrl={externalReportToUrl} hasInvalidResponse={hasExternalReportToUrl}></FormUrl>
                </Field>
                <Box my='16'>
                    <Switch checked={isAllowListEnabled} onChange={handleIsAllowListEnabled} description='Allow the use of a remote Content Security Policy allow list.  When a violation is detected, this allow list will be consulted and used to improve your configuration.'>Use Remote CSP Allow List</Switch>
                </Box>
                <Field label='Remote CSP Allow List Address' display={allowListUrlDisplay} my='16' error={hasAllowListUrlError ? allowListUrlErrorMessage : undefined}>
                    <FormUrl handleOnBlur={handleAllowListAddress} currentUrl={allowListUrl} hasInvalidResponse={hasAllowListUrlError}></FormUrl>
                </Field>
                <Box my='16'>
                    <Switch checked={isUpgradeInSecureRequestsEnabled} onChange={handleUpgradeInsecureRequests} description="Instructs user agents (browsers) to treat all of this site's insecure URLs (those served over HTTP) as though they have been replaced with secure URLs (those served over HTTPS).  This is intended only for websites with a large number of legacy APIs that need rewriting.">Upgrade Insecure Requests</Switch>
                </Box>
                <Box my='16'>
                    <Button appearance='primary' type='submit' disabled={disableSaveButton || (isContextSpecific && isInherited)} onClick={handleSaveSettings}>Save Changes</Button>
                </Box>
                </fieldset>
            </form>
        </Box>
    )
}

EditSettings.propTypes = {
    showToastNotificationEvent: PropTypes.func,
    appId: PropTypes.string,
    hostName: PropTypes.string
};

export default EditSettings
