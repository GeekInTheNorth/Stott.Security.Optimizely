import React, { useState, useEffect } from 'react';
import { Toast, ToastContainer } from 'react-bootstrap';
import EditCorsSettings from './cors/EditCorsSettings';
import { CspProvider } from './csp/CspContext';
import CspContainer from './csp/CspContainer';
import AuditHistory from './audit/AuditHistory';
import PermissionsPolicyContainer from './permissionpolicy/PermissionsPolicyContainer';
import { PermissionPolicyProvider } from './permissionpolicy/PermissionPolicy';
import SecurityHeaderContainer from './responseheaders/SecurityHeaderContainer';
import HeaderPreview from './preview/HeaderPreview';
import ToolsContainer from './tools/ToolsContainer';

const NAVIGATION_ITEMS = {
    'csp-settings': {
        title: 'CSP Settings',
        component: (props) => (
            <CspProvider>
                <CspContainer {...props} />
            </CspProvider>
        )
    },
    'cors-settings': {
        title: 'CORS Settings',
        component: (props) => <EditCorsSettings {...props} />
    },
    'all-security-headers': {
        title: 'Response Headers',
        component: (props) => <SecurityHeaderContainer {...props} />
    },
    'permissions-policy': {
        title: 'Permissions Policy',
        component: (props) => (
            <PermissionPolicyProvider>
                <PermissionsPolicyContainer {...props} />
            </PermissionPolicyProvider>
        )
    },
    'audit-history': {
        title: 'Audit History',
        component: (props) => <AuditHistory {...props} />
    },
    'header-preview': {
        title: 'Header Preview',
        component: (props) => <HeaderPreview {...props} />
    },
    'tools': {
        title: 'Tools',
        component: (props) => <ToolsContainer {...props} />
    }
};

function NavigationContainer() {
    const [toast, setToast] = useState({
        show: false,
        title: '',
        description: '',
        headerClass: ''
    });
    const [currentView, setCurrentView] = useState('csp-settings');

    const showToastNotificationEvent = (isSuccess, title, description) => {
        setToast({
            show: true,
            title,
            description,
            headerClass: isSuccess ? 'bg-success text-white' : 'bg-danger text-white'
        });
    };

    const closeToastNotification = () => {
        setToast(prev => ({ ...prev, show: false }));
    };

    const handleSelect = (key) => {
        if (NAVIGATION_ITEMS[key]) {
            setCurrentView(key);
        }
    };

    useEffect(() => {
        const handleHashChange = () => {
            const hash = window.location.hash?.substring(1);
            const validHash = NAVIGATION_ITEMS[hash] ? hash : 'csp-settings';
            handleSelect(validHash);
        };

        window.addEventListener('hashchange', handleHashChange);
        handleHashChange();

        return () => {
            window.removeEventListener('hashchange', handleHashChange);
        };
    }, []);

    const currentItem = NAVIGATION_ITEMS[currentView];
    const CurrentComponent = currentItem.component;

    return (
        <>
            <div className="container-fluid p-2 bg-dark text-light">
                <p className="my-0 h5">Stott Security | {currentItem.title}</p>
            </div>
            <div className="container-fluid security-app-container">
                <CurrentComponent showToastNotificationEvent={showToastNotificationEvent} />
                <ToastContainer className="p-3" position='middle-center'>
                    <Toast onClose={closeToastNotification} show={toast.show} delay={5000} autohide={true}>
                        <Toast.Header className={toast.headerClass}>
                            <strong className="me-auto">{toast.title}</strong>
                        </Toast.Header>
                        <Toast.Body>{toast.description}</Toast.Body>
                    </Toast>
                </ToastContainer>
            </div>
        </>
    );
}

export default NavigationContainer; 