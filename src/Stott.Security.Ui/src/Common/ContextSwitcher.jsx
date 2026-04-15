import { useState, useEffect } from 'react';
import axios from 'axios';
import { Button, Modal, ListGroup } from 'react-bootstrap';
import PropTypes from 'prop-types';

function ContextSwitcher({ siteId, hostName, onContextChange }) {

    const [showModal, setShowModal] = useState(false);
    const [sites, setSites] = useState([]);

    const loadSites = async () => {
        try {
            const response = await axios.get(import.meta.env.VITE_APP_SITES_LIST);
            if (response.data && Array.isArray(response.data)) {
                setSites(response.data);
            }
        } catch {
            // Silently fail - sites list may not be available
        }
    };

    useEffect(() => {
        loadSites();
    }, []);

    const getContextLabel = () => {
        if (!siteId) return 'All Sites';
        if (!hostName) return siteId;
        return `${siteId} - ${hostName}`;
    };

    const isGlobalSiteId = (id) => {
        return !id || id === '00000000-0000-0000-0000-000000000000';
    };

    const handleSelectSite = (site) => {
        if (isGlobalSiteId(site.siteId)) {
            onContextChange(null, null, 'All Sites');
        } else {
            onContextChange(site.siteId, null, site.siteName);
        }
        setShowModal(false);
    };

    const handleSelectHost = (site, host) => {
        onContextChange(site.siteId, host.hostName, `${site.siteName} - ${host.displayName}`);
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
                    <Modal.Title>Select Site Context</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <ListGroup>
                        {sites.map((site) => (
                            <div key={site.siteId ?? 'global'}>
                                <ListGroup.Item action active={siteId === site.siteId && !hostName} onClick={() => handleSelectSite(site)} className="ps-3 py-1">
                                    <strong>{site.siteName}</strong>
                                    <div className="small">{isGlobalSiteId(site.siteId) ? 'Global configuration - All Sites' : 'Site-level configuration'}{site.hasMultipleHosts ? '' : ' - All Hosts'}</div>
                                </ListGroup.Item>

                                {site.hasMultipleHosts && site.availableHosts && site.availableHosts.filter((host) => host.hostName).map((host) => (
                                    <ListGroup.Item key={`${site.siteId}-${host.hostName}`} action active={siteId === site.siteId && hostName === host.hostName} onClick={() => handleSelectHost(site, host)} className="ps-5 py-1">
                                        {host.displayName}
                                        <div className="small">Host-level configuration, Type: {host.hostType}, Language: {host.hostLanguage}</div>
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
    siteId: PropTypes.string,
    hostName: PropTypes.string,
    onContextChange: PropTypes.func.isRequired
};

export default ContextSwitcher;
