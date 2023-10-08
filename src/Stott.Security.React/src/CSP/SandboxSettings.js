import React, { useState, useEffect } from 'react';
import { Button, Container, Form } from 'react-bootstrap';
import axios from 'axios';

const SandboxSettings = (props) =>
{
    const [isSandboxEnabled, setSandboxEnabled] = useState(false);
    const [sandboxVisibility, setSandboxVisibility] = useState('my-3 d-none');
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
        let newSandboxVisbility = event.target.checked ? 'my-3' : 'my-3 d-none';
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
        axios.post(process.env.REACT_APP_SANDBOX_SAVE_URL, params)
            .then(() => {
                handleShowSuccessToast('Success', 'Content Security Policy Sandbox Settings have been successfully saved.');
            }, () => {
                handleShowFailureToast('Error', 'Failed to save the Content Security Policy Sandbox Settings.');
            });
        
        setDisableSaveButton(true);
    }

    const handleShowSuccessToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(true, title, description);
    const handleShowFailureToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(false, title, description);
    
    const getCspSandboxSettings = async () => {
        await axios.get(process.env.REACT_APP_SANDBOX_GET_URL)
            .then((response) => {
                let newSandboxVisbility = response.data.isSandboxEnabled ? 'my-3' : 'my-3 d-none';

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

                setDisableSaveButton(true);
            },
            () => {
                handleShowFailureToast('Error', 'Failed to load the Content Security Policy Sandbox Settings.');
            });
    }

    useEffect(() => {
        getCspSandboxSettings()
    }, [])

    return(
        <Container fluid='md'>
            <label>Defining a sandbox applies restrictions to a page's actions including preventing popups, preventing the execution of plugins and scripts, and enforcing a same-origin policy. Please note that the sandbox will not be enabled if the CSP is in report only mode.</label>
            <Form>
                <Form.Group className='my-3'>
                    <Form.Check type='switch' label={<>Enabled sandbox mode.</>} checked={isSandboxEnabled} onChange={handleSandboxEnabled} />
                </Form.Group>
                <Form.Group className={sandboxVisibility}>
                    <Form.Check type='switch' label={<>Allow for downloads after the user clicks a button or link. <em>(allow-downloads)</em></>} checked={isAllowDownloadsEnabled} onChange={handleChangeAllowDownloads} />
                </Form.Group>
                <Form.Group className={sandboxVisibility}>
                    <Form.Check type='switch' label={<>Allow for downloads to occur without a gesture from the user. <em>(allow-downloads-without-user-activation)</em></>} checked={isAllowDownloadsWithoutGestureEnabled} onChange={handleChangeAllowDownloadsWithoutGesture} />
                </Form.Group>
                <Form.Group className={sandboxVisibility}>
                    <Form.Check type='switch' label={<>Allow the site to submit forms. If this keyword is not used, this operation is not allowed. <em>(allow-forms)</em></>} checked={isAllowFormsEnabled} onChange={handleChangeAllowForms} />
                </Form.Group>
                <Form.Group className={sandboxVisibility}>
                    <Form.Check type='switch' label={<>Allow the site to open modal windows. <em>(allow-modals)</em></>} checked={isAllowModalsEnabled} onChange={handleChangeAllowModals} />
                </Form.Group>
                <Form.Group className={sandboxVisibility}>
                    <Form.Check type='switch' label={<>Allow the site to disable the ability to lock the screen orientation. <em>(allow-orientation-lock)</em></>} checked={isAllowOrientationLockEnabled} onChange={handleChangeAllowOrientationLock} />
                </Form.Group>
                <Form.Group className={sandboxVisibility}>
                    <Form.Check type='switch' label={<>Allow the site to use the pointer lock APIs <em>(allow-pointer-lock)</em></>} checked={isAllowPointerLockEnabled} onChange={handleChangeAllowPointerLock} />
                </Form.Group>
                <Form.Group className={sandboxVisibility}>
                    <Form.Check type='switch' label={<>Allow the use of popups or opening of content in new tabs. If this keyword is not used, that functionality will silently fail. <em>(allow-popups)</em></>} checked={isAllowPopupsEnabled} onChange={handleChangeAllowPopups} />
                    <div className='form-text'>Enable this function if you expect your site to use functions like <code>window.open</code>, <code>target="_blank"</code> or <code>showModalDialog</code>.</div>
                </Form.Group>
                <Form.Group className={sandboxVisibility}>
                    <Form.Check type='switch' label={<>Allow the popups to open without using the same sandbox restrictions. If this keyword is not used, that functionality will silently fail. <em>(allow-popups-to-escape-sandbox)</em></>} checked={isAllowPopupsToEscapeTheSandboxEnabled} onChange={handleChangeAllowPopupsToEscapeTheSandboxEnabled} />
                    <div className='form-text'>This will allow, for example, a third-party advertisement to be safely sandboxed without forcing the same restrictions upon the page the ad links to.</div>
                </Form.Group>
                <Form.Group className={sandboxVisibility}>
                    <Form.Check type='switch' label={<>Allow embedders to have control over whether an iframe can start a presentation session. <em>(allow-presentation)</em></>} checked={isAllowPresentationEnabled} onChange={handleChangeAllowPresentation} />
                </Form.Group>
                <Form.Group className={sandboxVisibility}>
                    <Form.Check type='switch' label={<>Allow embedded content to be treated as being from its normal origin. If this keyword is not used, the embedded content is treated as being from a unique origin. <em>(allow-same-origin)</em></>} checked={isAllowSameOriginEnabled} onChange={handleChangeAllowSameOrigin} />
                </Form.Group>
                <Form.Group className={sandboxVisibility}>
                    <Form.Check type='switch' label={<>Allow the site to run scripts (but not create pop-up windows). If this keyword is not used, this operation is not allowed. <em>(allow-scripts)</em></>} checked={isAllowScriptsEnabled} onChange={handleChangeAllowScripts} />
                </Form.Group>
                <Form.Group className={sandboxVisibility}>
                    <Form.Check type='switch' label={<>Allow the site to access the parent's storage API. <em>(allow-storage-access-by-user-activation)</em></>} checked={isAllowStorageAccessByUserEnabled} onChange={handleChangeAllowStorageAccessByUser} />
                </Form.Group>
                <Form.Group className={sandboxVisibility}>
                    <Form.Check type='switch' label={<>Allow navigation of the top level browser context.  If this keyword is not used, this operation is not allowed.<em>(allow-top-navigation)</em></>} checked={isAllowTopNavigationEnabled} onChange={handleChangeAllowTopNavigation} />
                </Form.Group>
                <Form.Group className={sandboxVisibility}>
                    <Form.Check type='switch' label={<>Allow navigation of the top level browser context only if activated by the user.  If this keyword is not used, this operation is not allowed. <em>(allow-top-navigation-by-user-activation)</em></>} checked={isAllowTopNavigationByUserEnabled} onChange={handleChangeAllowTopNavigationByUser} />
                </Form.Group>
                <Form.Group className={sandboxVisibility}>
                    <Form.Check type='switch' label={<>Allow navigation to be handed over to an external application for specific non-fetch schemes. <em>(allow-top-navigation-to-custom-protocols)</em></>} checked={isAllowTopNavigationToCustomProtocolEnabled} onChange={handleChangeAllowTopNavigationToCustomProtocol} />
                </Form.Group>
                <Form.Group className='my-3'>
                    <Button type='submit' disabled={disableSaveButton} onClick={handleSaveSandboxSettings}>Save Changes</Button>
                </Form.Group>
            </Form>
        </Container>
    )

}

export default SandboxSettings;