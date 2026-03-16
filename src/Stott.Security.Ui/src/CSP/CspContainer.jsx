import { useState } from 'react';
import { Nav } from 'react-bootstrap';
import PropTypes from 'prop-types';
import ContextSwitcher from './ContextSwitcher';
import EditSettings from './EditSettings';
import PermissionList from './PermissionList';
import SandboxSettings from './SandboxSettings';
import ViolationReport from './ViolationReport';

function CspContainer({ showToastNotificationEvent, initialTab }) {

    const [activeTab, setActiveTab] = useState(initialTab || 'settings');
    const [appId, setAppId] = useState(null);
    const [hostName, setHostName] = useState(null);

    const handleContextChange = (newAppId, newHostName) => {
        setAppId(newAppId);
        setHostName(newHostName);
    };

    return (
        <>
            <ContextSwitcher appId={appId} hostName={hostName} onContextChange={handleContextChange} />
            <Nav variant="tabs" activeKey={activeTab} onSelect={(key) => setActiveTab(key)} className="mb-3">
                <Nav.Item>
                    <Nav.Link eventKey="settings">Settings</Nav.Link>
                </Nav.Item>
                <Nav.Item>
                    <Nav.Link eventKey="sources">Sources</Nav.Link>
                </Nav.Item>
                <Nav.Item>
                    <Nav.Link eventKey="sandbox">Sandbox</Nav.Link>
                </Nav.Item>
                <Nav.Item>
                    <Nav.Link eventKey="violations">Violations</Nav.Link>
                </Nav.Item>
            </Nav>

            {activeTab === 'settings' && (
                <EditSettings
                    showToastNotificationEvent={showToastNotificationEvent}
                    appId={appId}
                    hostName={hostName}
                />
            )}
            {activeTab === 'sources' && (
                <PermissionList
                    showToastNotificationEvent={showToastNotificationEvent}
                    appId={appId}
                    hostName={hostName}
                />
            )}
            {activeTab === 'sandbox' && (
                <SandboxSettings
                    showToastNotificationEvent={showToastNotificationEvent}
                    appId={appId}
                    hostName={hostName}
                />
            )}
            {activeTab === 'violations' && (
                <ViolationReport
                    showToastNotificationEvent={showToastNotificationEvent}
                    appId={appId}
                    hostName={hostName}
                />
            )}
        </>
    );
}

CspContainer.propTypes = {
    showToastNotificationEvent: PropTypes.func,
    initialTab: PropTypes.string
};

export default CspContainer;
