import React from 'react';
import { Container } from 'react-bootstrap';
import ExportSettings from './ExportSettings';

function ToolsContainer(props) {
    
    const handleShowToastNotification = (isSuccess, title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(isSuccess, title, description);

    return (
        <Container fluid='md'>
            <ExportSettings showToastNotificationEvent={handleShowToastNotification}></ExportSettings>
        </Container>
    )
}

export default ToolsContainer;