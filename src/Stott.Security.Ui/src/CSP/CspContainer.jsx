import { useState } from 'react';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@optiaxiom/react';
import PropTypes from 'prop-types';
import ContextSwitcher from '../Common/ContextSwitcher';
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
            <Tabs value={activeTab} onValueChange={setActiveTab}>
                <TabsList mb='16'>
                    <TabsTrigger value="settings">Settings</TabsTrigger>
                    <TabsTrigger value="sources">Sources</TabsTrigger>
                    <TabsTrigger value="sandbox">Sandbox</TabsTrigger>
                    <TabsTrigger value="violations">Violations</TabsTrigger>
                </TabsList>

                <TabsContent value="settings">
                    <EditSettings
                        showToastNotificationEvent={showToastNotificationEvent}
                        appId={appId}
                        hostName={hostName}
                    />
                </TabsContent>
                <TabsContent value="sources">
                    <PermissionList
                        showToastNotificationEvent={showToastNotificationEvent}
                        appId={appId}
                        hostName={hostName}
                    />
                </TabsContent>
                <TabsContent value="sandbox">
                    <SandboxSettings
                        showToastNotificationEvent={showToastNotificationEvent}
                        appId={appId}
                        hostName={hostName}
                    />
                </TabsContent>
                <TabsContent value="violations">
                    <ViolationReport
                        showToastNotificationEvent={showToastNotificationEvent}
                        appId={appId}
                        hostName={hostName}
                    />
                </TabsContent>
            </Tabs>
        </>
    );
}

CspContainer.propTypes = {
    showToastNotificationEvent: PropTypes.func,
    initialTab: PropTypes.string
};

export default CspContainer;
