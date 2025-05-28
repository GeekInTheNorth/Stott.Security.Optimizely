import { useEffect, useState } from 'react';
import { useCsp } from "./CspContext";
import { Alert, Button, Container, Form } from "react-bootstrap";

function CspSandbox() {

  const { currentPolicy } = useCsp();

  const [sandboxVisibility, setSandboxVisibility] = useState('my-3 d-none');
  const [sandboxSettings, setSandboxSettings] = useState({
    isSandboxEnabled: false,
    isAllowDownloadsEnabled: false,
    isAllowDownloadsWithoutGestureEnabled: false,
    isAllowFormsEnabled: false,
    isAllowModalsEnabled: false,
    isAllowOrientationLockEnabled: false,
    isAllowPointerLockEnabled: false,
    isAllowPopupsEnabled: false,
    isAllowPopupsToEscapeTheSandboxEnabled: false,
    isAllowPresentationEnabled: false,
    isAllowSameOriginEnabled: false,
    isAllowScriptsEnabled: false,
    isAllowStorageAccessByUserEnabled: false,
    isAllowTopNavigationEnabled: false,
    isAllowTopNavigationByUserEnabled: false,
    isAllowTopNavigationToCustomProtocolEnabled: false
  });
  const [disableSaveButton, setDisableSaveButton] = useState(true);

  const loadSandboxSettings = () => {
    if (currentPolicy && import.meta.env.DEV) {
      console.log('Loading sandbox settings for policy:', currentPolicy);
      // Simulate loading settings from a mock JSON file
      fetch(`/src/csp/data/csp-sandbox-${currentPolicy.id}.mock.json`)
        .then(res => res.json())
        .then(data => {
          setSandboxSettings(data);
          setSandboxVisibility(data.isSandboxEnabled ? 'my-3' : 'my-3 d-none');
          setDisableSaveButton(true); // Disable save button initially
        })
        .catch(error => console.error('Error loading sandbox settings:', error));
    }
  }

  useEffect(() => { loadSandboxSettings(); }, [currentPolicy]);

  const handleSandboxEnabled = (e) => {
    const isEnabled = e.target.checked;
    setSandboxSettings(prevSettings => ({ ...prevSettings, isSandboxEnabled: isEnabled }));
    setSandboxVisibility(isEnabled ? 'my-3' : 'my-3 d-none');
    setDisableSaveButton(false);
  }
  const handleChangeAllowDownloads = (e) => {
    setSandboxSettings(prevSettings => ({ ...prevSettings, isAllowDownloadsEnabled: e.target.checked }));
    setDisableSaveButton(false);
  };
  const handleChangeAllowDownloadsWithoutGesture = (e) => {
    setSandboxSettings(prevSettings => ({ ...prevSettings, isAllowDownloadsWithoutGestureEnabled: e.target.checked }));
    setDisableSaveButton(false);
  };
  const handleChangeAllowForms = (e) => {
    setSandboxSettings(prevSettings => ({ ...prevSettings, isAllowFormsEnabled: e.target.checked }));
    setDisableSaveButton(false);
  };
  const handleChangeAllowModals = (e) => {
    setSandboxSettings(prevSettings => ({ ...prevSettings, isAllowModalsEnabled: e.target.checked }));
    setDisableSaveButton(false);
  }
  const handleChangeAllowOrientationLock = (e) => {
    setSandboxSettings(prevSettings => ({ ...prevSettings, isAllowOrientationLockEnabled: e.target.checked }));
    setDisableSaveButton(false);
  };
  const handleChangeAllowPointerLock = (e) => {
    setSandboxSettings(prevSettings => ({ ...prevSettings, isAllowPointerLockEnabled: e.target.checked }));
    setDisableSaveButton(false);
  };
  const handleChangeAllowPopups = (e) => {
    setSandboxSettings(prevSettings => ({ ...prevSettings, isAllowPopupsEnabled: e.target.checked }));
    setDisableSaveButton(false);
  };
  const handleChangeAllowPopupsToEscapeTheSandboxEnabled = (e) => {
    setSandboxSettings(prevSettings => ({ ...prevSettings, isAllowPopupsToEscapeTheSandboxEnabled: e.target.checked }));
    setDisableSaveButton(false);
  };
  const handleChangeAllowPresentation = (e) => {
    setSandboxSettings(prevSettings => ({ ...prevSettings, isAllowPresentationEnabled: e.target.checked }));
    setDisableSaveButton(false);
  }
  const handleChangeAllowSameOrigin = (e) => {
    setSandboxSettings(prevSettings => ({ ...prevSettings, isAllowSameOriginEnabled: e.target.checked }));
    setDisableSaveButton(false);
  };
  const handleChangeAllowScripts = (e) => {
    setSandboxSettings(prevSettings => ({ ...prevSettings, isAllowScriptsEnabled: e.target.checked }));
    setDisableSaveButton(false);
  };
  const handleChangeAllowStorageAccessByUser = (e) => {
    setSandboxSettings(prevSettings => ({ ...prevSettings, isAllowStorageAccessByUserEnabled: e.target.checked }));
    setDisableSaveButton(false);
  };
  const handleChangeAllowTopNavigation = (e) => {
    setSandboxSettings(prevSettings => ({ ...prevSettings, isAllowTopNavigationEnabled: e.target.checked }));
    setDisableSaveButton(false);
  }
  const handleChangeAllowTopNavigationByUser = (e) => {
    setSandboxSettings(prevSettings => ({ ...prevSettings, isAllowTopNavigationByUserEnabled: e.target.checked }));
    setDisableSaveButton(false);
  };
  const handleChangeAllowTopNavigationToCustomProtocol = (e) => {
    setSandboxSettings(prevSettings => ({ ...prevSettings, isAllowTopNavigationToCustomProtocolEnabled: e.target.checked }));
    setDisableSaveButton(false);
  };

  const handleSaveSandboxSettings = (e) => {
    e.preventDefault();
    // Here you would typically send the sandboxSettings to your backend or save them in some way.
    console.log('Sandbox settings saved:', sandboxSettings);
    setDisableSaveButton(true); // Disable the button after saving
  };

  return (
    <Container fluid='md'>
      <Alert variant='info' className='my-3'>Defining a sandbox applies restrictions to a page's actions including preventing popups, preventing the execution of plugins and scripts, and enforcing a same-origin policy. Please note that the sandbox will not be enabled if the CSP is in report only mode.</Alert>
      <Form>
          <Form.Group className='my-3'>
              <Form.Check type='switch' label={<>Enabled sandbox mode.</>} checked={sandboxSettings.isSandboxEnabled} onChange={handleSandboxEnabled} />
          </Form.Group>
          <Form.Group className={sandboxVisibility}>
              <Form.Check type='switch' label={<>Allow for downloads after the user clicks a button or link. <em>(allow-downloads)</em></>} checked={sandboxSettings.isAllowDownloadsEnabled} onChange={handleChangeAllowDownloads} />
          </Form.Group>
          <Form.Group className={sandboxVisibility}>
              <Form.Check type='switch' label={<>Allow for downloads to occur without a gesture from the user. <em>(allow-downloads-without-user-activation)</em></>} checked={sandboxSettings.isAllowDownloadsWithoutGestureEnabled} onChange={handleChangeAllowDownloadsWithoutGesture} />
          </Form.Group>
          <Form.Group className={sandboxVisibility}>
              <Form.Check type='switch' label={<>Allow the site to submit forms. If this keyword is not used, this operation is not allowed. <em>(allow-forms)</em></>} checked={sandboxSettings.isAllowFormsEnabled} onChange={handleChangeAllowForms} />
          </Form.Group>
          <Form.Group className={sandboxVisibility}>
              <Form.Check type='switch' label={<>Allow the site to open modal windows. <em>(allow-modals)</em></>} checked={sandboxSettings.isAllowModalsEnabled} onChange={handleChangeAllowModals} />
          </Form.Group>
          <Form.Group className={sandboxVisibility}>
              <Form.Check type='switch' label={<>Allow the site to disable the ability to lock the screen orientation. <em>(allow-orientation-lock)</em></>} checked={sandboxSettings.isAllowOrientationLockEnabled} onChange={handleChangeAllowOrientationLock} />
          </Form.Group>
          <Form.Group className={sandboxVisibility}>
              <Form.Check type='switch' label={<>Allow the site to use the pointer lock APIs <em>(allow-pointer-lock)</em></>} checked={sandboxSettings.isAllowPointerLockEnabled} onChange={handleChangeAllowPointerLock} />
          </Form.Group>
          <Form.Group className={sandboxVisibility}>
              <Form.Check type='switch' label={<>Allow the use of popups or opening of content in new tabs. If this keyword is not used, that functionality will silently fail. <em>(allow-popups)</em></>} checked={sandboxSettings.isAllowPopupsEnabled} onChange={handleChangeAllowPopups} />
              <div className='form-text'>Enable this function if you expect your site to use functions like <code>window.open</code>, <code>target="_blank"</code> or <code>showModalDialog</code>.</div>
          </Form.Group>
          <Form.Group className={sandboxVisibility}>
              <Form.Check type='switch' label={<>Allow the popups to open without using the same sandbox restrictions. If this keyword is not used, that functionality will silently fail. <em>(allow-popups-to-escape-sandbox)</em></>} checked={sandboxSettings.isAllowPopupsToEscapeTheSandboxEnabled} onChange={handleChangeAllowPopupsToEscapeTheSandboxEnabled} />
              <div className='form-text'>This will allow, for example, a third-party advertisement to be safely sandboxed without forcing the same restrictions upon the page the ad links to.</div>
          </Form.Group>
          <Form.Group className={sandboxVisibility}>
              <Form.Check type='switch' label={<>Allow embedders to have control over whether an iframe can start a presentation session. <em>(allow-presentation)</em></>} checked={sandboxSettings.isAllowPresentationEnabled} onChange={handleChangeAllowPresentation} />
          </Form.Group>
          <Form.Group className={sandboxVisibility}>
              <Form.Check type='switch' label={<>Allow embedded content to be treated as being from its normal origin. If this keyword is not used, the embedded content is treated as being from a unique origin. <em>(allow-same-origin)</em></>} checked={sandboxSettings.isAllowSameOriginEnabled} onChange={handleChangeAllowSameOrigin} />
          </Form.Group>
          <Form.Group className={sandboxVisibility}>
              <Form.Check type='switch' label={<>Allow the site to run scripts (but not create pop-up windows). If this keyword is not used, this operation is not allowed. <em>(allow-scripts)</em></>} checked={sandboxSettings.isAllowScriptsEnabled} onChange={handleChangeAllowScripts} />
          </Form.Group>
          <Form.Group className={sandboxVisibility}>
              <Form.Check type='switch' label={<>Allow the site to access the parent's storage API. <em>(allow-storage-access-by-user-activation)</em></>} checked={sandboxSettings.isAllowStorageAccessByUserEnabled} onChange={handleChangeAllowStorageAccessByUser} />
          </Form.Group>
          <Form.Group className={sandboxVisibility}>
              <Form.Check type='switch' label={<>Allow navigation of the top level browser context.  If this keyword is not used, this operation is not allowed.<em>(allow-top-navigation)</em></>} checked={sandboxSettings.isAllowTopNavigationEnabled} onChange={handleChangeAllowTopNavigation} />
          </Form.Group>
          <Form.Group className={sandboxVisibility}>
              <Form.Check type='switch' label={<>Allow navigation of the top level browser context only if activated by the user.  If this keyword is not used, this operation is not allowed. <em>(allow-top-navigation-by-user-activation)</em></>} checked={sandboxSettings.isAllowTopNavigationByUserEnabled} onChange={handleChangeAllowTopNavigationByUser} />
          </Form.Group>
          <Form.Group className={sandboxVisibility}>
              <Form.Check type='switch' label={<>Allow navigation to be handed over to an external application for specific non-fetch schemes. <em>(allow-top-navigation-to-custom-protocols)</em></>} checked={sandboxSettings.isAllowTopNavigationToCustomProtocolEnabled} onChange={handleChangeAllowTopNavigationToCustomProtocol} />
          </Form.Group>
          <Form.Group className='my-3'>
              <Button type='submit' disabled={disableSaveButton} onClick={handleSaveSandboxSettings}>Save Changes</Button>
          </Form.Group>
      </Form>
    </Container>
  );
}

export default CspSandbox;