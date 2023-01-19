import React, { useState } from 'react';
import { Tab, Tabs, Toast, ToastContainer } from 'react-bootstrap';
import PermissionList from '../CSP/PermissionList';
import EditSettings from '../CSP/EditSettings';
import ViolationReport from '../CSP/ViolationReport';
import SandboxSettings from '../CSP/SandboxSettings';
import AuditHistory from '../Audit/AuditHistory';
import SecurityHeaderContainer from '../Security/SecurityHeaderContainer';

function NavigationContainer() {

    const [showToastNotification, setShowToastNotification] = useState(false);
    const [toastTitle, setToastTitle] = useState('');
    const [toastDescription, setToastDescription] = useState('');
    const [toastHeaderClass, setToastHeaderClass] = useState('');
    const [showCspSettings, setShowCspSettings] = useState(true);
    const [showCspSources, setShowCspSources] = useState(false);
    const [showCspViolations, setShowCspViolations] = useState(false);
    const [showSandboxSettings, setShowSandboxSettings] = useState(false);
    const [showAllSecurityHeaders, setShowAllSecurityHeaders] = useState(false);
    const [showAuditHistory, setShowAuditHistory] = useState(false);

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
        setShowSandboxSettings(false);
        setShowAllSecurityHeaders(false);
        setShowAuditHistory(false);
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
            case 'csp-sandbox':
                setShowSandboxSettings(true);
                break;
            case 'all-security-headers':
                setShowAllSecurityHeaders(true);
                break;
            case 'audit-history':
                setShowAuditHistory(true);
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
                <Tab eventKey='csp-sandbox' title='CSP Sandbox'>
                    { showSandboxSettings ? <SandboxSettings showToastNotificationEvent={showToastNotificationEvent}></SandboxSettings> : null }
                </Tab>
                <Tab eventKey='csp-violations' title='CSP Violations'>
                    { showCspViolations ? <ViolationReport showToastNotificationEvent={showToastNotificationEvent}></ViolationReport> : null }
                </Tab>
                <Tab eventKey='all-security-headers' title='Security Headers'>
                    { showAllSecurityHeaders ? <SecurityHeaderContainer showToastNotificationEvent={showToastNotificationEvent}></SecurityHeaderContainer> : null }
                </Tab>
                <Tab eventKey='audit-history' title='Audit History'>
                    { showAuditHistory ? <AuditHistory></AuditHistory> : null }
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

export default NavigationContainer