import { useState, useEffect } from 'react';
import { httpGet } from './httpClient';
import { Box, Field } from '@optiaxiom/react';
import FormSelect from './FormSelect';
import PropTypes from 'prop-types';

function ContextSelector({ appId, hostName, onContextChange }) {

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

    const selectedApp = applications.find(app => app.appId === appId) || null;
    const availableHosts = selectedApp?.availableHosts?.filter(host => host.hostName) || [];
    const hasMultipleHosts = selectedApp?.hasMultipleHosts || false;
    const applicationOptions = applications.map((app) => ({ label: app.appName, value: app.appId || '' }));
    const hostOptions = [{ label: 'All Hosts', value: '' }].concat(hasMultipleHosts ? availableHosts.map((host) => ({ label: host.displayName, value: host.hostName })) : []);

    const handleAppChange = (value) => {
        const newAppId = value || null;
        onContextChange(newAppId, null);
    };

    const handleHostChange = (value) => {
        const newHostName = value || null;
        onContextChange(appId, newHostName);
    };

    return (
        <Box my='16'>
            <Field label='Application' mb='16'>
                <FormSelect value={appId || ''} onValueChange={handleAppChange} options={applicationOptions} />
            </Field>
            {appId && availableHosts.length > 0 && (
                <Field label='Host' mb='16'>
                    <FormSelect value={hostName || ''} onValueChange={handleHostChange} options={hostOptions} />
                </Field>
            )}
        </Box>
    );
}

ContextSelector.propTypes = {
    appId: PropTypes.string,
    hostName: PropTypes.string,
    onContextChange: PropTypes.func.isRequired
};

export default ContextSelector;
