import { useState, useEffect } from 'react';
import { httpGet } from './httpClient';
import { Form } from 'react-bootstrap';
import PropTypes from 'prop-types';

function ContextSelector({ siteId, hostName, onContextChange }) {

    const [sites, setSites] = useState([]);

    const loadSites = async () => {
        try {
            const response = await httpGet(import.meta.env.VITE_APP_SITES_LIST);
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

    const selectedSite = sites.find(site => site.siteId === siteId) || null;
    const availableHosts = selectedSite?.availableHosts?.filter(host => host.hostName) || [];
    const hasMultipleHosts = selectedSite?.hasMultipleHosts || false;

    const handleSiteChange = (e) => {
        const newSiteId = e.target.value || null;
        onContextChange(newSiteId, null);
    };

    const handleHostChange = (e) => {
        const newHostName = e.target.value || null;
        onContextChange(siteId, newHostName);
    };

    return (
        <div className='my-3'>
            <Form.Group className='mb-3'>
                <Form.Label>Site</Form.Label>
                <Form.Select value={siteId || ''} onChange={handleSiteChange}>
                    {sites.map((site) => (
                        <option key={site.siteId || 'global'} value={site.siteId || ''}>
                            {site.siteName}
                        </option>
                    ))}
                </Form.Select>
            </Form.Group>
            {siteId && availableHosts.length > 0 && (
                <Form.Group className='mb-3'>
                    <Form.Label>Host</Form.Label>
                    <Form.Select value={hostName || ''} onChange={handleHostChange}>
                        <option value=''>All Hosts</option>
                        {hasMultipleHosts && availableHosts.map((host) => (
                            <option key={host.hostName} value={host.hostName}>
                                {host.displayName}
                            </option>
                        ))}
                    </Form.Select>
                </Form.Group>
            )}
        </div>
    );
}

ContextSelector.propTypes = {
    siteId: PropTypes.string,
    hostName: PropTypes.string,
    onContextChange: PropTypes.func.isRequired
};

export default ContextSelector;
