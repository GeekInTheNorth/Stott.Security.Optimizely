import React, { useState, useEffect } from 'react';
import { Toast, ToastContainer } from 'react-bootstrap';
import PermissionList from '../CSP/PermissionList';
import ViolationReport from '../CSP/ViolationReport';
import EditSettings from '../CSP/EditSettings';
import SandboxSettings from '../CSP/SandboxSettings';
import AuditHistory from '../Audit/AuditHistory';
import SecurityHeaderContainer from '../Security/SecurityHeaderContainer';
import EditCorsSettings from '../Cors/EditCorsSettings';
import HeaderPreview from '../Preview/HeaderPreview';
import ToolsContainer from '../Tools/ToolsContainer';

function NavigationContainer() {

    const [showToastNotification, setShowToastNotification] = useState(false);
    const [toastTitle, setToastTitle] = useState('');
    const [toastDescription, setToastDescription] = useState('');
    const [toastHeaderClass, setToastHeaderClass] = useState('');
    const [showCspSettings, setShowCspSettings] = useState(false);
    const [showCspSandbox, setShowCspSandbox] = useState(false);
    const [showCspSources, setShowCspSources] = useState(false);
    const [showCspViolations, setShowCspViolations] = useState(false);
    const [showCorsSettings, setShowCorsSettings] = useState(false);
    const [showAllSecurityHeaders, setShowAllSecurityHeaders] = useState(false);
    const [showHeaderPreview, setShowHeaderPreview] = useState(false);
    const [showAuditHistory, setShowAuditHistory] = useState(false);
    const [showTools, setShowTools] = useState(false);
    const [containerTitle, setContainerTitle] = useState('CSP Settings');

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
        setContainerTitle('');
        setShowCspSettings(false);
        setShowCspSandbox(false);
        setShowCspSources(false);
        setShowCspViolations(false);
        setShowCorsSettings(false);
        setShowAllSecurityHeaders(false);
        setShowHeaderPreview(false);
        setShowAuditHistory(false);
        setShowTools(false);
        switch(key){
            case 'csp-settings':
                setContainerTitle('CSP Settings');
                setShowCspSettings(true);
                break;
            case 'csp-sandbox':
                setContainerTitle('CSP Sandbox');
                setShowCspSandbox(true);
                break;
            case 'csp-source':
                setContainerTitle('CSP Sources');
                setShowCspSources(true);
                break;
            case 'csp-violations':
                setContainerTitle('CSP Violations');
                setShowCspViolations(true);
                break;
            case 'cors-settings':
                setContainerTitle('CORS Settings');
                setShowCorsSettings(true);
                break;
            case 'all-security-headers':
                setContainerTitle('Response Headers');
                setShowAllSecurityHeaders(true);
                break;
            case 'audit-history':
                setContainerTitle('Audit History');
                setShowAuditHistory(true);
                break;
            case 'header-preview':
                setContainerTitle('Header Preview');
                setShowHeaderPreview(true);
                break;
            case 'tools':
                setContainerTitle('Tools');
                setShowTools(true);
                break;
            default:
                // No default required
                break;
        }
    }

    useEffect(() => {
        const handleHashChange = () => {
            var hash = window.location.hash?.substring(1);
            if (hash && hash !== '') {
                handleSelect(hash);
            }
            else {
                handleSelect('csp-settings');
            }
        };

        window.addEventListener('hashchange', handleHashChange);
        handleHashChange();

        return () => {
            window.removeEventListener('hashchange', handleHashChange);
        }
    });

    return (
        <>
            <div class="container-fluid p-2 bg-dark text-light">
                <p class="my-0 h5">Stott Security | {containerTitle}</p>
            </div>
            <div class="container-fluid security-app-container">
                { showCspSettings ? <EditSettings showToastNotificationEvent={showToastNotificationEvent}></EditSettings> : null }
                { showCspSandbox ? <SandboxSettings showToastNotificationEvent={showToastNotificationEvent}></SandboxSettings> : null }
                { showCspSources ? <PermissionList showToastNotificationEvent={showToastNotificationEvent}></PermissionList> : null }
                { showCspViolations ? <ViolationReport showToastNotificationEvent={showToastNotificationEvent}></ViolationReport> : null }
                { showCorsSettings ? <EditCorsSettings showToastNotificationEvent={showToastNotificationEvent}></EditCorsSettings> : null }
                { showAllSecurityHeaders ? <SecurityHeaderContainer showToastNotificationEvent={showToastNotificationEvent}></SecurityHeaderContainer> : null }
                { showHeaderPreview ? <HeaderPreview></HeaderPreview> : null }
                { showAuditHistory ? <AuditHistory showToastNotificationEvent={showToastNotificationEvent}></AuditHistory> : null }
                { showTools ? <ToolsContainer showToastNotificationEvent={showToastNotificationEvent}></ToolsContainer> : null }
                <ToastContainer className="p-3" position='middle-center'>
                    <Toast onClose={closeToastNotification} show={showToastNotification} delay={5000} autohide={true}>
                        <Toast.Header className={toastHeaderClass}>
                            <strong className="me-auto">{toastTitle}</strong>
                        </Toast.Header>
                        <Toast.Body>{toastDescription}</Toast.Body>
                    </Toast>
                </ToastContainer>
            </div>
        </>
    )
}

export default NavigationContainer