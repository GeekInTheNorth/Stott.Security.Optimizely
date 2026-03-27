import { useState, useEffect } from 'react';
import axios from 'axios';
import { Button, Modal, ListGroup } from 'react-bootstrap';
import PropTypes from 'prop-types';

function ContextSwitcher({ appId, hostName, onContextChange }) {

    const [showModal, setShowModal] = useState(false);
    const [applications, setApplications] = useState([]);

    const loadApplications = async () => {
        try {
            const response = await axios.get(import.meta.env.VITE_APP_APPLICATIONS_LIST);
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

    return (
        <>
            <div className="d-flex align-items-center gap-2 mb-3 p-2 bg-light border rounded">
                <strong>Context:</strong>
                <span>{getContextLabel()}</span>
                <Button variant="outline-primary" size="sm" onClick={() => setShowModal(true)}>
                    Switch Context
                </Button>
            </div>

            <Modal show={showModal} onHide={() => setShowModal(false)} size="lg">
                <Modal.Header closeButton>
                    <Modal.Title>Select Application Context</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <ListGroup>
                        {applications.map((app) => (
                            <div key={app.appId ?? 'global'}>
                                <ListGroup.Item action active={appId === app.appId && !hostName} onClick={() => handleSelectApp(app)} className="ps-3">
                                    <strong>{app.appName}</strong>
                                    <div className="small">{app.appId ? 'Application-level configuration' : 'Global configuration'}</div>
                                </ListGroup.Item>

                                {app.availableHosts && app.availableHosts.filter((host) => host.hostName).map((host) => (
                                    <ListGroup.Item key={`${app.appId}-${host.hostName}`} action active={appId === app.appId && hostName === host.hostName} onClick={() => handleSelectHost(app, host)} className="ps-5">
                                        {host.displayName}
                                        <div className="small">Host-specific configuration</div>
                                        {(host.hostType && host.hostLanguage) && (<div className="small">Type: {host.hostType}, Language: {host.hostLanguage}</div>)}
                                    </ListGroup.Item>
                                ))}
                            </div>
                        ))}
                    </ListGroup>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="secondary" onClick={() => setShowModal(false)}>Cancel</Button>
                </Modal.Footer>
            </Modal>
        </>
    );
}

ContextSwitcher.propTypes = {
    appId: PropTypes.string,
    hostName: PropTypes.string,
    onContextChange: PropTypes.func.isRequired
};

export default ContextSwitcher;
