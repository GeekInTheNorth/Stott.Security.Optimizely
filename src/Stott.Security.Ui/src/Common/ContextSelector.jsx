import { useState, useEffect } from 'react';
import axios from 'axios';
import { Form } from 'react-bootstrap';
import PropTypes from 'prop-types';

function ContextSelector({ appId, hostName, onContextChange }) {

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

    const selectedApp = applications.find(app => app.appId === appId) || null;
    const availableHosts = selectedApp?.availableHosts?.filter(host => host.hostName) || [];

    const handleAppChange = (e) => {
        const newAppId = e.target.value || null;
        onContextChange(newAppId, null);
    };

    const handleHostChange = (e) => {
        const newHostName = e.target.value || null;
        onContextChange(appId, newHostName);
    };

    return (
        <div className='my-3'>
            <Form.Group className='mb-3'>
                <Form.Label>Application</Form.Label>
                <Form.Select value={appId || ''} onChange={handleAppChange}>
                    {applications.map((app) => (
                        <option key={app.appId || 'global'} value={app.appId || ''}>
                            {app.appName}
                        </option>
                    ))}
                </Form.Select>
            </Form.Group>
            {appId && availableHosts.length > 0 && (
                <Form.Group className='mb-3'>
                    <Form.Label>Host</Form.Label>
                    <Form.Select value={hostName || ''} onChange={handleHostChange}>
                        <option value=''>All Hosts</option>
                        {availableHosts.map((host) => (
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
    appId: PropTypes.string,
    hostName: PropTypes.string,
    onContextChange: PropTypes.func.isRequired
};

export default ContextSelector;
