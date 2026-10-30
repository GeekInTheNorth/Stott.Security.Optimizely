import { useState, useEffect } from 'react';
import { toaster } from '@optiaxiom/react';
import CspContainer from './CSP/CspContainer';
import AuditHistory from './Audit/AuditHistory';
import PermissionsPolicyContainer from './PermissionsPolicy/PermissionsPolicyContainer';
import EditCorsSettings from './Cors/EditCorsSettings';
import HeaderPreview from './Preview/HeaderPreview';
import ToolsContainer from './Tools/ToolsContainer';
import StottSecurityProvider from './Context/StottSecurityContext';
import SecurityTxtConfigurationList from './SecurityTxt/SecurityTxtConfigurationList';
import CustomHeadersContainer from './CustomHeaders/CustomHeadersContainer';

function App() {

    const [showContentSecurityPolicy, setShowContentSecurityPolicy] = useState(false);
    const [cspInitialTab, setCspInitialTab] = useState('settings');
    const [showCorsSettings, setShowCorsSettings] = useState(false);
    const [showResponseHeaders, setShowResponseHeaders] = useState(false);
    const [showPermissionsPolicy, setShowPermissionsPolicy] = useState(false);
    const [showHeaderPreview, setShowHeaderPreview] = useState(false);
    const [showAuditHistory, setShowAuditHistory] = useState(false);
    const [showTools, setShowTools] = useState(false);
    const [showSecurityTxtConfiguration, setShowSecurityTxtConfiguration] = useState(false);
    const [containerTitle, setContainerTitle] = useState('Content Security Policy');

    // The title is not displayed as the toast intent already conveys success or failure
    const showToastNotificationEvent = (isSuccess, title, description) => {
        toaster.create(description, { intent: isSuccess === true ? 'success' : 'danger' });
    };

    const handleSelect = (key) => {
        setContainerTitle('');
        setShowContentSecurityPolicy(false);
        setCspInitialTab('settings');
        setShowCorsSettings(false);
        setShowResponseHeaders(false);
        setShowPermissionsPolicy(false);
        setShowHeaderPreview(false);
        setShowAuditHistory(false);
        setShowTools(false);
        setShowSecurityTxtConfiguration(false);
        switch(key){
            case 'content-security-policy':
                setContainerTitle('Content Security Policy');
                setShowContentSecurityPolicy(true);
                break;
            case 'csp-settings':
                setContainerTitle('Content Security Policy');
                setCspInitialTab('settings');
                setShowContentSecurityPolicy(true);
                break;
            case 'csp-source':
                setContainerTitle('Content Security Policy');
                setCspInitialTab('sources');
                setShowContentSecurityPolicy(true);
                break;
            case 'csp-sandbox':
                setContainerTitle('Content Security Policy');
                setCspInitialTab('sandbox');
                setShowContentSecurityPolicy(true);
                break;
            case 'csp-violations':
                setContainerTitle('Content Security Policy');
                setCspInitialTab('violations');
                setShowContentSecurityPolicy(true);
                break;
            case 'cors-settings':
                setContainerTitle('CORS Settings');
                setShowCorsSettings(true);
                break;
            case 'response-headers':
                setContainerTitle('Response Headers');
                setShowResponseHeaders(true);
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
            case 'security-txt':
                setContainerTitle('Security.txt Files');
                setShowSecurityTxtConfiguration(true);
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
                handleSelect('content-security-policy');
            }
        };

        window.addEventListener('hashchange', handleHashChange);
        handleHashChange();

        return () => {
            window.removeEventListener('hashchange', handleHashChange);
        }
    });

    return (
        <StottSecurityProvider showToastNotificationEvent={showToastNotificationEvent}>
            <div className="container-fluid p-2 bg-dark text-light">
                <p className="my-0 h5">Stott Security | {containerTitle}</p>
            </div>
            <div className="container-fluid security-app-container">
                { showContentSecurityPolicy ? <CspContainer showToastNotificationEvent={showToastNotificationEvent} initialTab={cspInitialTab}></CspContainer> : null }
                { showCorsSettings ? <EditCorsSettings showToastNotificationEvent={showToastNotificationEvent}></EditCorsSettings> : null }
                { showResponseHeaders ? <CustomHeadersContainer showToastNotificationEvent={showToastNotificationEvent}></CustomHeadersContainer> : null }
                { showPermissionsPolicy ? <PermissionsPolicyContainer showToastNotificationEvent={showToastNotificationEvent}></PermissionsPolicyContainer> : null }
                { showHeaderPreview ? <HeaderPreview></HeaderPreview> : null }
                { showSecurityTxtConfiguration ? <SecurityTxtConfigurationList showToastNotificationEvent={showToastNotificationEvent}></SecurityTxtConfigurationList> : null }
                { showAuditHistory ? <AuditHistory showToastNotificationEvent={showToastNotificationEvent}></AuditHistory> : null }
                { showTools ? <ToolsContainer showToastNotificationEvent={showToastNotificationEvent}></ToolsContainer> : null }
            </div>
        </StottSecurityProvider>
    )
}

export default App
