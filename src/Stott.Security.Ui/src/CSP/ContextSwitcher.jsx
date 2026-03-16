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
        if (!appId) return 'Global';
        if (!hostName) return appId;
        return `${appId} - ${hostName}`;
    };

    const handleSelectGlobal = () => {
        onContextChange(null, null, 'Global');
        setShowModal(false);
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
                <span className="badge bg-primary fs-6">{getContextLabel()}</span>
                <Button variant="outline-primary" size="sm" onClick={() => setShowModal(true)}>
                    Switch Context
                </Button>
            </div>

            <Modal show={showModal} onHide={() => setShowModal(false)} size="lg">
                <Modal.Header closeButton>
                    <Modal.Title>Select CSP Context</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <ListGroup>
                        <ListGroup.Item
                            action
                            active={!appId && !hostName}
                            onClick={handleSelectGlobal}
                        >
                            <strong>Global (All Applications)</strong>
                            <div className="text-muted small">Default configuration for all sites</div>
                        </ListGroup.Item>

                        {applications.map((app) => (
                            <div key={app.appId}>
                                <ListGroup.Item
                                    action
                                    active={appId === app.appId && !hostName}
                                    onClick={() => handleSelectApp(app)}
                                    className="ms-3"
                                >
                                    <strong>{app.appName}</strong>
                                    <div className="text-muted small">Application-level configuration</div>
                                </ListGroup.Item>

                                {app.availableHosts && app.availableHosts.map((host) => (
                                    <ListGroup.Item
                                        key={`${app.appId}-${host.hostName}`}
                                        action
                                        active={appId === app.appId && hostName === host.hostName}
                                        onClick={() => handleSelectHost(app, host)}
                                        className="ms-5"
                                    >
                                        {host.displayName}
                                        <div className="text-muted small">Host-specific configuration</div>
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
