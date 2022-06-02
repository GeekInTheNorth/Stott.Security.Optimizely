import React, { useState } from 'react';
import { Tab, Tabs, Toast, ToastContainer } from 'react-bootstrap';
import PermissionList from './PermissionList';
import EditSettings from './EditSettings';
import EditLegacyHeaderSettings from './EditLegacyHeaderSettings'
import ViolationReport from './ViolationReport';

function CspContainer() {

    const [showToastNotification, setShowToastNotification] = useState(false);
    const [toastTitle, setToastTitle] = useState('');
    const [toastDescription, setToastDescription] = useState('');
    const [toastHeaderClass, setToastHeaderClass] = useState('');
    const [showCspSettings, setShowCspSettings] = useState(true);
    const [showCspSources, setShowCspSources] = useState(false);
    const [showCspViolations, setShowCspViolations] = useState(false);
    const [showSecurityHeaders, setShowSecurityHeaders] = useState(false);

    const showToastNotificationEvent = (isSuccess, title, description) => {
        if (isSuccess === true){
            setToastHeaderClass('bg-success text-white');
        } else{
            setToastHeaderClass('bg-danger text-white');
        }

        setShowToastNotification(false);
        setToastTitle(title);
        setToastDescription(description)
        setShowToastNotification(true);
    };
    const closeToastNotification = () => setShowToastNotification(false);

    const handleSelect = (key) => {
        setShowCspSettings(false);
        setShowCspSources(false);
        setShowCspViolations(false);
        setShowSecurityHeaders(false);
        switch(key){
            case 'csp-settings':
                setShowCspSettings(true);
                break;
            case 'csp-source':
                setShowCspSources(true);
                break;
            case 'csp-violations':
                setShowCspViolations(true);
                break;
            case 'legacy-headers':
                setShowSecurityHeaders(true);
                break;
            default:
                // No default required
                break;
        }
    }

    return (
        <>
            <Tabs defaultActiveKey='csp-settings' id='uncontrolled-tab-example' className='mb-2' onSelect={handleSelect}>
                <Tab eventKey='csp-settings' title='CSP Settings'>
                    { showCspSettings ? <EditSettings showToastNotificationEvent={showToastNotificationEvent}></EditSettings> : null }
                </Tab>
                <Tab eventKey='csp-source' title='CSP Sources'>
                    { showCspSources ? <PermissionList showToastNotificationEvent={showToastNotificationEvent}></PermissionList> : null }
                </Tab>
                <Tab eventKey='csp-violations' title='CSP Violations'>
                    { showCspViolations ? <ViolationReport showToastNotificationEvent={showToastNotificationEvent}></ViolationReport> : null }
                </Tab>
                <Tab eventKey='legacy-headers' title='Security Headers'>
                    { showSecurityHeaders ? <EditLegacyHeaderSettings showToastNotificationEvent={showToastNotificationEvent}></EditLegacyHeaderSettings> : null }
                </Tab>
            </Tabs>
            <ToastContainer className="p-3" position='middle-center'>
                <Toast onClose={closeToastNotification} show={showToastNotification} delay={4000} autohide={true}>
                    <Toast.Header className={toastHeaderClass}>
                        <strong className="me-auto">{toastTitle}</strong>
                    </Toast.Header>
                    <Toast.Body>{toastDescription}</Toast.Body>
                </Toast>
            </ToastContainer>
        </>
    )
}

export default CspContainer