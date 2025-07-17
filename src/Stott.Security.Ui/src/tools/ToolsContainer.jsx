import React from 'react';
import ExportSettings from './ExportSettings';
import ImportSettings from './ImportSettings';

function ToolsContainer(props) {
    const handleShowToastNotification = (isSuccess, title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(isSuccess, title, description);

    return (
        <>
            <ExportSettings showToastNotificationEvent={handleShowToastNotification} />
            <ImportSettings showToastNotificationEvent={handleShowToastNotification} />
        </>
    );
}

export default ToolsContainer; 