import { useState } from 'react';
import { Nav } from 'react-bootstrap';
import PropTypes from 'prop-types';
import ContextSwitcher from '../Common/ContextSwitcher';
import EditSettings from './EditSettings';
import PermissionList from './PermissionList';
import SandboxSettings from './SandboxSettings';
import ViolationReport from './ViolationReport';

function CspContainer({ showToastNotificationEvent, initialTab }) {

    const [activeTab, setActiveTab] = useState(initialTab || 'settings');
    const [siteId, setSiteId] = useState(null);
    const [hostName, setHostName] = useState(null);

    const handleContextChange = (newSiteId, newHostName) => {
        setSiteId(newSiteId);
        setHostName(newHostName);
    };

    return (
        <>
            <ContextSwitcher siteId={siteId} hostName={hostName} onContextChange={handleContextChange} />
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
                    siteId={siteId}
                    hostName={hostName}
                />
            )}
            {activeTab === 'sources' && (
                <PermissionList
                    showToastNotificationEvent={showToastNotificationEvent}
                    siteId={siteId}
                    hostName={hostName}
                />
            )}
            {activeTab === 'sandbox' && (
                <SandboxSettings
                    showToastNotificationEvent={showToastNotificationEvent}
                    siteId={siteId}
                    hostName={hostName}
                />
            )}
            {activeTab === 'violations' && (
                <ViolationReport
                    showToastNotificationEvent={showToastNotificationEvent}
                    siteId={siteId}
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
