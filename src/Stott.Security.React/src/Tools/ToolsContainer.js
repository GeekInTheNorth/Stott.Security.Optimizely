import React from 'react';
import { Container } from 'react-bootstrap';
import ExportSettings from './ExportSettings';
import ImportSettings from './ImportSettings';

function ToolsContainer(props) {
    
    const handleShowToastNotification = (isSuccess, title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(isSuccess, title, description);

    return (
        <Container fluid='md'>
            <ExportSettings showToastNotificationEvent={handleShowToastNotification}></ExportSettings>
            <ImportSettings showToastNotificationEvent={handleShowToastNotification}></ImportSettings>
        </Container>
    )
}

export default ToolsContainer;