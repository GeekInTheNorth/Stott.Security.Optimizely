import { useState, useEffect } from 'react';
import { httpGet } from './httpClient';
import { Button, Dialog, DialogBody, DialogClose, DialogContent, DialogFooter, DialogHeader, Group, Listbox, ListboxRadioItem, Text } from '@optiaxiom/react';
import PropTypes from 'prop-types';

function ContextSwitcher({ appId, hostName, onContextChange }) {

    const [showModal, setShowModal] = useState(false);
    const [applications, setApplications] = useState([]);

    const loadApplications = async () => {
        try {
            const response = await httpGet(import.meta.env.VITE_APP_APPLICATIONS_LIST);
            if (response.data && Array.isArray(response.data)) {
                setApplications(response.data);
            }
        } catch {
            // Silently fail - applications list may not be available
        }
    };

    useEffect(() => {
        loadApplications();
    }, []);

    const getContextLabel = () => {
        if (!appId) return 'All Applications';
        if (!hostName) return appId;
        return `${appId} - ${hostName}`;
    };

    const handleSelectApp = (app) => {
        onContextChange(app.appId, null, app.appName);
        setShowModal(false);
    };

    const handleSelectHost = (app, host) => {
        onContextChange(app.appId, host.hostName, `${app.appName} - ${host.displayName}`);
        setShowModal(false);
    };

    const isAppSelected = (app) => appId === app.appId && !hostName;

    const isHostSelected = (app, host) => appId === app.appId && hostName === host.hostName;

    const handleItemKeyDown = (event, selectAction) => {
        if (event.key === 'Enter' || event.key === ' ') {
            event.preventDefault();
            selectAction();
        }
    };

    return (
        <>
            <Group gap="8" mb="16" p="8" bg="bg.secondary" border="1" borderColor="border.default" rounded="md" alignItems="center">
                <Text asChild fontWeight="600"><span>Context:</span></Text>
                <Text asChild><span>{getContextLabel()}</span></Text>
                <Button appearance="default" size="sm" onClick={() => setShowModal(true)}>
                    Switch Context
                </Button>
            </Group>

            <Dialog open={showModal} onOpenChange={setShowModal}>
                <DialogContent size="lg">
                    <DialogHeader borderB="1" borderColor="border.default">Select Application Context</DialogHeader>
                    <DialogBody>
                        <Listbox role="listbox" aria-label="Application Context">
                            {applications.map((app) => (
                                <div key={app.appId ?? 'global'}>
                                    <ListboxRadioItem className="context-switcher-item" role="option" tabIndex={0} aria-selected={isAppSelected(app)} bg={isAppSelected(app) ? 'bg.accent.subtle' : undefined} onClick={() => handleSelectApp(app)} onKeyDown={(event) => handleItemKeyDown(event, () => handleSelectApp(app))}
                                        description={`${app.appId ? 'Application-level configuration' : 'Global configuration - All Applications'}${app.hasMultipleHosts ? '' : ' - All Hosts'}`}>
                                        <Text fontWeight="600">{app.appName}</Text>
                                    </ListboxRadioItem>

                                    {app.hasMultipleHosts && app.availableHosts && app.availableHosts.filter((host) => host.hostName).map((host) => (
                                        <ListboxRadioItem key={`${app.appId}-${host.hostName}`} className="context-switcher-item" role="option" tabIndex={0} aria-selected={isHostSelected(app, host)} bg={isHostSelected(app, host) ? 'bg.accent.subtle' : undefined} onClick={() => handleSelectHost(app, host)} onKeyDown={(event) => handleItemKeyDown(event, () => handleSelectHost(app, host))}
                                            description={`Host-level configuration, Type: ${host.hostType}, Language: ${host.hostLanguage}`} pl="32">
                                            {host.displayName}
                                        </ListboxRadioItem>
                                    ))}
                                </div>
                            ))}
                        </Listbox>
                    </DialogBody>
                    <DialogFooter>
                        <DialogClose appearance="default">Cancel</DialogClose>
                    </DialogFooter>
                </DialogContent>
            </Dialog>
        </>
    );
}

ContextSwitcher.propTypes = {
    appId: PropTypes.string,
    hostName: PropTypes.string,
    onContextChange: PropTypes.func.isRequired
};

export default ContextSwitcher;
