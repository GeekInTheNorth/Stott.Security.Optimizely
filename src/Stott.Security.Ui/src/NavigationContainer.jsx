import React, { useState, useEffect } from 'react';
import { Toast, ToastContainer } from 'react-bootstrap';
import EditCorsSettings from './cors/EditCorsSettings';
import { CspProvider } from './csp/CspContext';
import CspContainer from './csp/CspContainer';
import AuditHistory from './audit/AuditHistory';
import PermissionsPolicyContainer from './permissionpolicy/PermissionsPolicyContainer';
import { PermissionPolicyProvider } from './permissionpolicy/PermissionPolicy';

function NavigationContainer() {
    const [showToastNotification, setShowToastNotification] = useState(false);
    const [toastTitle, setToastTitle] = useState('');
    const [toastDescription, setToastDescription] = useState('');
    const [toastHeaderClass, setToastHeaderClass] = useState('');
    const [showCspSettings, setShowCspSettings] = useState(false);
    const [showCorsSettings, setShowCorsSettings] = useState(false);
    const [showAllSecurityHeaders, setShowAllSecurityHeaders] = useState(false);
    const [showPermissionsPolicy, setShowPermissionsPolicy] = useState(false);
    const [showHeaderPreview, setShowHeaderPreview] = useState(false);
    const [showAuditHistory, setShowAuditHistory] = useState(false);
    const [showTools, setShowTools] = useState(false);
    const [containerTitle, setContainerTitle] = useState('CSP Settings');

    const showToastNotificationEvent = (isSuccess, title, description) => {
        if (isSuccess === true) {
            setToastHeaderClass('bg-success text-white');
        } else {
            setToastHeaderClass('bg-danger text-white');
        }

        setShowToastNotification(false);
        setToastTitle(title);
        setToastDescription(description);
        setShowToastNotification(true);
    };

    const closeToastNotification = () => setShowToastNotification(false);

    const handleSelect = (key) => {
        setContainerTitle('');
        setShowCspSettings(false);
        setShowCorsSettings(false);
        setShowAllSecurityHeaders(false);
        setShowPermissionsPolicy(false);
        setShowHeaderPreview(false);
        setShowAuditHistory(false);
        setShowTools(false);

        switch (key) {
            case 'csp-settings':
                setContainerTitle('CSP Settings');
                setShowCspSettings(true);
                break;
            case 'cors-settings':
                setContainerTitle('CORS Settings');
                setShowCorsSettings(true);
                break;
            case 'all-security-headers':
                setContainerTitle('Response Headers');
                setShowAllSecurityHeaders(true);
                break;
            case 'permissions-policy':
                setContainerTitle('Permissions Policy');
                setShowPermissionsPolicy(true);
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
    };

    useEffect(() => {
        const handleHashChange = () => {
            const hash = window.location.hash?.substring(1);
            if (hash && hash !== '') {
                handleSelect(hash);
            } else {
                handleSelect('csp-settings');
            }
        };

        window.addEventListener('hashchange', handleHashChange);
        handleHashChange();

        return () => {
            window.removeEventListener('hashchange', handleHashChange);
        };
    }, []); // Added dependency array to prevent infinite re-renders

    return (
        <>
            <div className="container-fluid p-2 bg-dark text-light">
                <p className="my-0 h5">Stott Security | {containerTitle}</p>
            </div>
            <div className="container-fluid security-app-container">
                {showCspSettings && <CspProvider>
                    <CspContainer showToastNotificationEvent={showToastNotificationEvent} />
                </CspProvider>}
                {showCorsSettings && <EditCorsSettings showToastNotificationEvent={showToastNotificationEvent} />}
                {/* TODO: Migrate these components when they're available */}
                {showAllSecurityHeaders && <div>Security Headers - Not yet migrated</div>}
                {showPermissionsPolicy && <PermissionPolicyProvider>
                    <PermissionsPolicyContainer showToastNotificationEvent={showToastNotificationEvent}></PermissionsPolicyContainer>
                </PermissionPolicyProvider>}
                {showHeaderPreview && <div>Header Preview - Not yet migrated</div>}
                {showAuditHistory && <AuditHistory showToastNotificationEvent={showToastNotificationEvent}></AuditHistory>}
                {showTools && <div>Tools - Not yet migrated</div>}
                
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
    );
}

export default NavigationContainer; 