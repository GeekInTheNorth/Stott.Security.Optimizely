import PropTypes from 'prop-types';
import { Box } from '@optiaxiom/react';
import ExportSettings from './ExportSettings';
import ImportSettings from './ImportSettings';

function ToolsContainer(props) {
    
    const handleShowToastNotification = (isSuccess, title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(isSuccess, title, description);

    return (
        <Box className='security-page-container'>
            <ExportSettings showToastNotificationEvent={handleShowToastNotification}></ExportSettings>
            <ImportSettings showToastNotificationEvent={handleShowToastNotification}></ImportSettings>
        </Box>
    )
}

ToolsContainer.propTypes = {
    showToastNotificationEvent: PropTypes.func
};

export default ToolsContainer;
