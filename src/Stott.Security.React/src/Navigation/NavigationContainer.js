import React, { useState } from 'react';
import { Tab, Tabs, Toast, ToastContainer } from 'react-bootstrap';
import PermissionList from '../CSP/PermissionList';
import ViolationReport from '../CSP/ViolationReport';
import AuditHistory from '../Audit/AuditHistory';
import SecurityHeaderContainer from '../Security/SecurityHeaderContainer';
import EditCorsSettings from '../Cors/EditCorsSettings';
import HeaderPreview from '../Preview/HeaderPreview';
import CspSettingsContainer from '../CSP/CspSettingsContainer';
import ToolsContainer from '../Tools/ToolsContainer';

function NavigationContainer() {

    const [showToastNotification, setShowToastNotification] = useState(false);
    const [toastTitle, setToastTitle] = useState('');
    const [toastDescription, setToastDescription] = useState('');
    const [toastHeaderClass, setToastHeaderClass] = useState('');
    const [showCspSettings, setShowCspSettings] = useState(true);
    const [showCspSources, setShowCspSources] = useState(false);
    const [showCspViolations, setShowCspViolations] = useState(false);
    const [showCorsSettings, setShowCorsSettings] = useState(false);
    const [showAllSecurityHeaders, setShowAllSecurityHeaders] = useState(false);
    const [showHeaderPreview, setShowHeaderPreview] = useState(false);
    const [showAuditHistory, setShowAuditHistory] = useState(false);
    const [showTools, setShowTools] = useState(false);

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
        setShowCorsSettings(false);
        setShowAllSecurityHeaders(false);
        setShowHeaderPreview(false);
        setShowAuditHistory(false);
        setShowTools(false);
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
            case 'cors-settings':
                setShowCorsSettings(true);
                break;
            case 'all-security-headers':
                setShowAllSecurityHeaders(true);
                break;
            case 'audit-history':
                setShowAuditHistory(true);
                break;
            case 'header-preview':
                setShowHeaderPreview(true);
                break;
            case 'tools':
                setShowTools(true);
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
                    { showCspSettings ? <CspSettingsContainer showToastNotificationEvent={showToastNotificationEvent}></CspSettingsContainer> : null }
                </Tab>
                <Tab eventKey='csp-source' title='CSP Sources'>
                    { showCspSources ? <PermissionList showToastNotificationEvent={showToastNotificationEvent}></PermissionList> : null }
                </Tab>
                <Tab eventKey='csp-violations' title='CSP Violations'>
                    { showCspViolations ? <ViolationReport showToastNotificationEvent={showToastNotificationEvent}></ViolationReport> : null }
                </Tab>
                <Tab eventKey='cors-settings' title='CORS'>
                    { showCorsSettings ? <EditCorsSettings showToastNotificationEvent={showToastNotificationEvent}></EditCorsSettings> : null }
                </Tab>
                <Tab eventKey='all-security-headers' title='Misc Headers'>
                    { showAllSecurityHeaders ? <SecurityHeaderContainer showToastNotificationEvent={showToastNotificationEvent}></SecurityHeaderContainer> : null }
                </Tab>
                <Tab eventKey='header-preview' title='Preview'>
                    { showHeaderPreview ? <HeaderPreview></HeaderPreview> : null }
                </Tab>
                <Tab eventKey='audit-history' title='Audit'>
                    { showAuditHistory ? <AuditHistory showToastNotificationEvent={showToastNotificationEvent}></AuditHistory> : null }
                </Tab>
                <Tab eventKey='tools' title='Tools'>
                    { showTools ? <ToolsContainer showToastNotificationEvent={showToastNotificationEvent}></ToolsContainer> : null }
                </Tab>
            </Tabs>
            <ToastContainer className="p-3" position='middle-center'>
                <Toast onClose={closeToastNotification} show={showToastNotification} delay={5000} autohide={true}>
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