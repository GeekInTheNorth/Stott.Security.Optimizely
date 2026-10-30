import PropTypes from 'prop-types';
import { Box, Card, CardHeader } from '@optiaxiom/react';
import EditSettings from './EditSettings';
import SandboxSettings from './SandboxSettings';

function CspSettingsContainer(props) {
    
    const handleShowToastNotification = (isSuccess, title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(isSuccess, title, description);

    return (
        <Box className='security-page-container'>
            <Card mb='16'>
                <CardHeader>Content Security Policy - General Settings</CardHeader>
                <EditSettings showToastNotificationEvent={handleShowToastNotification}></EditSettings>
            </Card>
            <Card mb='16'>
                <CardHeader>Content Security Policy - Sandbox Settings</CardHeader>
                <SandboxSettings showToastNotificationEvent={handleShowToastNotification}></SandboxSettings>
            </Card>
        </Box>
    )
}

CspSettingsContainer.propTypes = {
    showToastNotificationEvent: PropTypes.func
};

export default CspSettingsContainer;
