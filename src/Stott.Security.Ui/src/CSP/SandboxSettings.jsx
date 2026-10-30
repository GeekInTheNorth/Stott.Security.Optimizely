import { useState, useEffect } from 'react';
import PropTypes from 'prop-types';
import { Alert, Box, Button, Group, Switch, Text } from '@optiaxiom/react';
import { httpGet, httpPost, httpDelete } from '../Common/httpClient';

const SandboxSettings = (props) =>
{
    const [isInherited, setIsInherited] = useState(false);
    const [isSandboxEnabled, setSandboxEnabled] = useState(false);
    const [sandboxVisibility, setSandboxVisibility] = useState('none');
    const [isAllowDownloadsEnabled, setAllowDownloads] = useState(false);
    const [isAllowDownloadsWithoutGestureEnabled, setAllowDownloadsWithoutGesture] = useState(false);
    const [isAllowFormsEnabled, setAllowForms] = useState(false);
    const [isAllowModalsEnabled, setAllowModals] = useState(false);
    const [isAllowOrientationLockEnabled, setAllowOrientationLock] = useState(false);
    const [isAllowPointerLockEnabled, setAllowPointerLock] = useState(false);
    const [isAllowPopupsEnabled, setAllowPopups] = useState(false);
    const [isAllowPopupsToEscapeTheSandboxEnabled, setAllowPopupsToEscapeTheSandbox] = useState(false);
    const [isAllowPresentationEnabled, setAllowPresentation] = useState(false);
    const [isAllowSameOriginEnabled, setAllowSameOrigin] = useState(false);
    const [isAllowScriptsEnabled, setAllowScripts] = useState(false);
    const [isAllowStorageAccessByUserEnabled, setAllowStorageAccessByUser] = useState(false);
    const [isAllowTopNavigationEnabled, setAllowTopNavigation] = useState(false);
    const [isAllowTopNavigationByUserEnabled, setAllowTopNavigationByUser] = useState(false);
    const [isAllowTopNavigationToCustomProtocolEnabled, setAllowTopNavigationToCustomProtocol] = useState(false);

    const [disableSaveButton, setDisableSaveButton] = useState(true);

    const handleSandboxEnabled = (event) => {
        let newSandboxVisbility = event.target.checked ? 'block' : 'none';
        setSandboxEnabled(event.target.checked);
        setSandboxVisibility(newSandboxVisbility);
        setDisableSaveButton(false);
    }

    const handleChangeAllowDownloads = (event) => {
        setAllowDownloads(event.target.checked);
        setAllowDownloadsWithoutGesture(event.target.checked && isAllowDownloadsWithoutGestureEnabled);
        setDisableSaveButton(false);
    }

    const handleChangeAllowDownloadsWithoutGesture = (event) => {
        setAllowDownloadsWithoutGesture(event.target.checked && isAllowDownloadsEnabled);
        setDisableSaveButton(false);
    }

    const handleChangeAllowForms = (event) => {
        setAllowForms(event.target.checked);
        setDisableSaveButton(false);
    }

    const handleChangeAllowModals = (event) => {
        setAllowModals(event.target.checked);
        setDisableSaveButton(false);
    }

    const handleChangeAllowOrientationLock = (event) => {
        setAllowOrientationLock(event.target.checked);
        setDisableSaveButton(false);
    }

    const handleChangeAllowPointerLock = (event) => {
        setAllowPointerLock(event.target.checked);
        setDisableSaveButton(false);
    }

    const handleChangeAllowPopups = (event) => {
        setAllowPopups(event.target.checked);
        setAllowPopupsToEscapeTheSandbox(event.target.checked && isAllowPopupsToEscapeTheSandboxEnabled);
        setDisableSaveButton(false);
    }

    const handleChangeAllowPopupsToEscapeTheSandboxEnabled = (event) => {
        setAllowPopupsToEscapeTheSandbox(event.target.checked && isAllowPopupsEnabled);
        setDisableSaveButton(false);
    }

    const handleChangeAllowPresentation = (event) => {
        setAllowPresentation(event.target.checked);
        setDisableSaveButton(false);
    }

    const handleChangeAllowSameOrigin = (event) => {
        setAllowSameOrigin(event.target.checked);
        setDisableSaveButton(false);
    }

    const handleChangeAllowScripts = (event) => {
        setAllowScripts(event.target.checked);
        setDisableSaveButton(false);
    }

    const handleChangeAllowStorageAccessByUser = (event) => {
        setAllowStorageAccessByUser(event.target.checked);
        setDisableSaveButton(false);
    }

    const handleChangeAllowTopNavigation = (event) => {
        setAllowTopNavigation(event.target.checked);
        setAllowTopNavigationByUser(event.target.checked && isAllowTopNavigationByUserEnabled);
        setAllowTopNavigationToCustomProtocol(event.target.checked && isAllowTopNavigationToCustomProtocolEnabled);
        setDisableSaveButton(false);
    }

    const handleChangeAllowTopNavigationByUser = (event) => {
        setAllowTopNavigationByUser(event.target.checked && isAllowTopNavigationEnabled);
        setDisableSaveButton(false);
    }

    const handleChangeAllowTopNavigationToCustomProtocol = (event) => {
        setAllowTopNavigationToCustomProtocol(event.target.checked && isAllowTopNavigationEnabled);
        setDisableSaveButton(false);
    }

    const handleSaveSandboxSettings  = (event) => {
        event.preventDefault();

        let params = new URLSearchParams();
        params.append('isSandboxEnabled', isSandboxEnabled);
        params.append('isAllowDownloadsEnabled', isAllowDownloadsEnabled);
        params.append('isAllowDownloadsWithoutGestureEnabled', isAllowDownloadsWithoutGestureEnabled);
        params.append('isAllowFormsEnabled', isAllowFormsEnabled);
        params.append('isAllowModalsEnabled', isAllowModalsEnabled);
        params.append('isAllowOrientationLockEnabled', isAllowOrientationLockEnabled);
        params.append('isAllowPointerLockEnabled', isAllowPointerLockEnabled);
        params.append('isAllowPopupsEnabled', isAllowPopupsEnabled);
        params.append('isAllowPopupsToEscapeTheSandboxEnabled', isAllowPopupsToEscapeTheSandboxEnabled);
        params.append('isAllowPresentationEnabled', isAllowPresentationEnabled);
        params.append('isAllowSameOriginEnabled', isAllowSameOriginEnabled);
        params.append('isAllowScriptsEnabled', isAllowScriptsEnabled);
        params.append('isAllowStorageAccessByUserEnabled', isAllowStorageAccessByUserEnabled);
        params.append('isAllowTopNavigationEnabled', isAllowTopNavigationEnabled);
        params.append('isAllowTopNavigationByUserEnabled', isAllowTopNavigationByUserEnabled);
        params.append('isAllowTopNavigationToCustomProtocolEnabled', isAllowTopNavigationToCustomProtocolEnabled);
        if (props.appId) params.append('appId', props.appId);
        if (props.hostName) params.append('hostName', props.hostName);
        httpPost(import.meta.env.VITE_SANDBOX_SAVE_URL, params)
            .then(() => {
                handleShowSuccessToast('Success', 'Content Security Policy Sandbox Settings have been successfully saved.');
            }, () => {
                handleShowFailureToast('Error', 'Failed to save the Content Security Policy Sandbox Settings.');
            });
        
        setDisableSaveButton(true);
    }

    const handleShowSuccessToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(true, title, description);
    const handleShowFailureToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(false, title, description);

    const handleOverrideSettings = () => {
        setIsInherited(false);
        setDisableSaveButton(false);
    };

    const handleRevertToInherited = async () => {
        try {
            await httpDelete(import.meta.env.VITE_SANDBOX_DELETE_URL, { appId: props.appId, hostName: props.hostName });
            handleShowSuccessToast('Success', 'Sandbox settings reverted to inherited.');
            getCspSandboxSettings();
        } catch {
            handleShowFailureToast('Error', 'Failed to revert sandbox settings.');
        }
    };

    const isContextSpecific = !!(props.appId || props.hostName);

    const getCspSandboxSettings = async () => {
        await httpGet(import.meta.env.VITE_SANDBOX_GET_URL, { appId: props.appId, hostName: props.hostName })
            .then((response) => {
                let newSandboxVisbility = response.data.isSandboxEnabled ? 'block' : 'none';

                setSandboxEnabled(response.data.isSandboxEnabled);
                setSandboxVisibility(newSandboxVisbility);
                setAllowDownloads(response.data.isAllowDownloadsEnabled);
                setAllowDownloadsWithoutGesture(response.data.isAllowDownloadsWithoutGestureEnabled);
                setAllowForms(response.data.isAllowFormsEnabled);
                setAllowModals(response.data.isAllowModalsEnabled);
                setAllowOrientationLock(response.data.isAllowOrientationLockEnabled);
                setAllowPointerLock(response.data.isAllowPointerLockEnabled);
                setAllowPopups(response.data.isAllowPopupsEnabled);
                setAllowPopupsToEscapeTheSandbox(response.data.isAllowPopupsToEscapeTheSandboxEnabled);
                setAllowPresentation(response.data.isAllowPresentationEnabled);
                setAllowSameOrigin(response.data.isAllowSameOriginEnabled);
                setAllowScripts(response.data.isAllowScriptsEnabled);
                setAllowStorageAccessByUser(response.data.isAllowStorageAccessByUserEnabled);
                setAllowTopNavigation(response.data.isAllowTopNavigationEnabled);
                setAllowTopNavigationByUser(response.data.isAllowTopNavigationByUserEnabled);
                setAllowTopNavigationToCustomProtocol(response.data.isAllowTopNavigationToCustomProtocolEnabled);
                setIsInherited(response.data.isInherited ?? false);

                setDisableSaveButton(true);
            },
            () => {
                handleShowFailureToast('Error', 'Failed to load the Content Security Policy Sandbox Settings.');
            });
    }

    useEffect(() => {
        getCspSandboxSettings()
    }, [props.appId, props.hostName])

    return(
        <Box className='security-page-container'>
            {isContextSpecific && isInherited && (
                <Alert intent='information' my='16'>
                    <Group justifyContent='space-between' gap='16' w='full'>
                        <Text>These sandbox settings are inherited from the parent configuration.</Text>
                        <Button appearance='primary' size='sm' onClick={handleOverrideSettings}>Override for this context</Button>
                    </Group>
                </Alert>
            )}
            {isContextSpecific && !isInherited && (
                <Alert intent='warning' my='16'>
                    <Group justifyContent='space-between' gap='16' w='full'>
                        <Text>These sandbox settings override the inherited configuration.</Text>
                        <Button appearance='danger-outline' size='sm' onClick={handleRevertToInherited}>Revert to inherited</Button>
                    </Group>
                </Alert>
            )}
            <Text>Defining a sandbox applies restrictions to a page's actions including preventing popups, preventing the execution of plugins and scripts, and enforcing a same-origin policy. Please note that the sandbox will not be enabled if the CSP is in report only mode.</Text>
            <form>
                <fieldset className='security-fieldset' disabled={isContextSpecific && isInherited}>
                <Box my='16'>
                    <Switch checked={isSandboxEnabled} onChange={handleSandboxEnabled}>Enabled sandbox mode.</Switch>
                </Box>
                <Box display={sandboxVisibility} my='16'>
                    <Switch checked={isAllowDownloadsEnabled} onChange={handleChangeAllowDownloads}>Allow for downloads after the user clicks a button or link. <em>(allow-downloads)</em></Switch>
                </Box>
                <Box display={sandboxVisibility} my='16'>
                    <Switch checked={isAllowDownloadsWithoutGestureEnabled} onChange={handleChangeAllowDownloadsWithoutGesture}>Allow for downloads to occur without a gesture from the user. <em>(allow-downloads-without-user-activation)</em></Switch>
                </Box>
                <Box display={sandboxVisibility} my='16'>
                    <Switch checked={isAllowFormsEnabled} onChange={handleChangeAllowForms}>Allow the site to submit forms. If this keyword is not used, this operation is not allowed. <em>(allow-forms)</em></Switch>
                </Box>
                <Box display={sandboxVisibility} my='16'>
                    <Switch checked={isAllowModalsEnabled} onChange={handleChangeAllowModals}>Allow the site to open modal windows. <em>(allow-modals)</em></Switch>
                </Box>
                <Box display={sandboxVisibility} my='16'>
                    <Switch checked={isAllowOrientationLockEnabled} onChange={handleChangeAllowOrientationLock}>Allow the site to disable the ability to lock the screen orientation. <em>(allow-orientation-lock)</em></Switch>
                </Box>
                <Box display={sandboxVisibility} my='16'>
                    <Switch checked={isAllowPointerLockEnabled} onChange={handleChangeAllowPointerLock}>Allow the site to use the pointer lock APIs <em>(allow-pointer-lock)</em></Switch>
                </Box>
                <Box display={sandboxVisibility} my='16'>
                    <Switch checked={isAllowPopupsEnabled} onChange={handleChangeAllowPopups} description={<>Enable this function if you expect your site to use functions like <code>window.open</code>, <code>target="_blank"</code> or <code>showModalDialog</code>.</>}>Allow the use of popups or opening of content in new tabs. If this keyword is not used, that functionality will silently fail. <em>(allow-popups)</em></Switch>
                </Box>
                <Box display={sandboxVisibility} my='16'>
                    <Switch checked={isAllowPopupsToEscapeTheSandboxEnabled} onChange={handleChangeAllowPopupsToEscapeTheSandboxEnabled} description={<>This will allow, for example, a third-party advertisement to be safely sandboxed without forcing the same restrictions upon the page the ad links to.</>}>Allow the popups to open without using the same sandbox restrictions. If this keyword is not used, that functionality will silently fail. <em>(allow-popups-to-escape-sandbox)</em></Switch>
                </Box>
                <Box display={sandboxVisibility} my='16'>
                    <Switch checked={isAllowPresentationEnabled} onChange={handleChangeAllowPresentation}>Allow embedders to have control over whether an iframe can start a presentation session. <em>(allow-presentation)</em></Switch>
                </Box>
                <Box display={sandboxVisibility} my='16'>
                    <Switch checked={isAllowSameOriginEnabled} onChange={handleChangeAllowSameOrigin}>Allow embedded content to be treated as being from its normal origin. If this keyword is not used, the embedded content is treated as being from a unique origin. <em>(allow-same-origin)</em></Switch>
                </Box>
                <Box display={sandboxVisibility} my='16'>
                    <Switch checked={isAllowScriptsEnabled} onChange={handleChangeAllowScripts}>Allow the site to run scripts (but not create pop-up windows). If this keyword is not used, this operation is not allowed. <em>(allow-scripts)</em></Switch>
                </Box>
                <Box display={sandboxVisibility} my='16'>
                    <Switch checked={isAllowStorageAccessByUserEnabled} onChange={handleChangeAllowStorageAccessByUser}>Allow the site to access the parent's storage API. <em>(allow-storage-access-by-user-activation)</em></Switch>
                </Box>
                <Box display={sandboxVisibility} my='16'>
                    <Switch checked={isAllowTopNavigationEnabled} onChange={handleChangeAllowTopNavigation}>Allow navigation of the top level browser context.  If this keyword is not used, this operation is not allowed.<em>(allow-top-navigation)</em></Switch>
                </Box>
                <Box display={sandboxVisibility} my='16'>
                    <Switch checked={isAllowTopNavigationByUserEnabled} onChange={handleChangeAllowTopNavigationByUser}>Allow navigation of the top level browser context only if activated by the user.  If this keyword is not used, this operation is not allowed. <em>(allow-top-navigation-by-user-activation)</em></Switch>
                </Box>
                <Box display={sandboxVisibility} my='16'>
                    <Switch checked={isAllowTopNavigationToCustomProtocolEnabled} onChange={handleChangeAllowTopNavigationToCustomProtocol}>Allow navigation to be handed over to an external application for specific non-fetch schemes. <em>(allow-top-navigation-to-custom-protocols)</em></Switch>
                </Box>
                <Box my='16'>
                    <Button appearance='primary' type='submit' disabled={disableSaveButton || isInherited} onClick={handleSaveSandboxSettings}>Save Changes</Button>
                </Box>
                </fieldset>
            </form>
        </Box>
    )

}

SandboxSettings.propTypes = {
    showToastNotificationEvent: PropTypes.func,
    appId: PropTypes.string,
    hostName: PropTypes.string
};

export default SandboxSettings;
