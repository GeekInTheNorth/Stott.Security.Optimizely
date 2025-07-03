import React from 'react';
import PropTypes from 'prop-types';
import { Card } from 'react-bootstrap';
import EditPermissionPolicy from './EditPermissionPolicy';

function PermissionPolicyCard(props) {
    const directiveTitle = props.directive.title ?? '';
    const directiveDescription = props.directive.description ?? '';
    const enabledState = props.directive.enabledState ?? 'None';
    const sources = props.directive.sources ?? [];

    const RenderConfiguration = () => {
        let hasSources = sources && sources.length > 0;
        if (enabledState === 'All') {
            return (<Card.Text>Enabled for all sources.</Card.Text>);
        } else if (enabledState === 'ThisSite' || (enabledState === 'ThisAndSpecificSites' && !hasSources)) {
            return (<Card.Text>Enabled for this site.</Card.Text>);
        } else if (enabledState === 'ThisAndSpecificSites' && hasSources) {
            return (<><Card.Text>Enabled For:</Card.Text><ul><li>This site.</li>{RenderSources()}</ul></>);
        } else if (enabledState === 'SpecificSites' && hasSources) {
            return (<><Card.Text>Enabled For:</Card.Text><ul>{RenderSources()}</ul></>);
        } else if (enabledState === 'None') {
            return (<Card.Text>No Sites.</Card.Text>);
        } else {
            return (<Card.Text>Not enabled.</Card.Text>);
        }
    };

    const RenderSources = () => {
        return sources && sources.map((source) => {
            return (
                <li key={source.id}>{source.url}</li>
            );
        });
    };

    const handleShowToastNotification = (isSuccess, title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(isSuccess, title, description);

    return (
        <Card className='my-2'>
            <Card.Header className='bg-primary text-light'>{directiveTitle}</Card.Header>
            <Card.Body>
                <Card.Text><em>{directiveDescription}</em></Card.Text>
                {RenderConfiguration()}
            </Card.Body>
            <Card.Footer>
                <EditPermissionPolicy directive={props.directive} showToastNotificationEvent={handleShowToastNotification} />
            </Card.Footer>
        </Card>
    );
}

PermissionPolicyCard.propTypes = {
    directive : PropTypes.shape({
        name: PropTypes.string,
        title: PropTypes.string,
        description: PropTypes.string,
        enabledState: PropTypes.string,
        sources: PropTypes.array
    }),
    directiveSource: PropTypes.shape({
        id: PropTypes.string,
        url: PropTypes.string
    })
};

export default PermissionPolicyCard; 