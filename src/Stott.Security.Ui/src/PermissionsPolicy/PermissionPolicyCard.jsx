import PropTypes from 'prop-types';
import { Badge, Card, CardFooter, CardHeader, Text } from '@optiaxiom/react';
import EditPermissionPolicy from './EditPermissionPolicy';

function PermissionsPolicyCard(props)
{
    const directiveTitle = props.directive.title ?? '';
    const directiveDescription = props.directive.description ?? '';
    const enabledState = props.directive.enabledState ?? 'None';
    const sources = props.directive.sources ?? [];
    const isDeprecated = props.directive.isDeprecated ?? false;
    const appId = props.appId;
    const hostName = props.hostName;

    const RenderConfiguration = () =>
    {
        let hasSources = sources && sources.length > 0;

        if (enabledState === 'All') {
            return (<Text>Enabled for all sources.</Text>);
        } else if (enabledState === 'ThisSite' || (enabledState === 'ThisAndSpecificSites' && !hasSources)) {
            return (<Text>Enabled for this site.</Text>);
        } else if (enabledState === 'ThisAndSpecificSites' && hasSources) {
            return (<><Text>Enabled For:</Text><ul><li>This site.</li>{RenderSources()}</ul></>);
        } else if (enabledState === 'SpecificSites' && hasSources) {
            return (<><Text>Enabled For:</Text><ul>{RenderSources()}</ul></>);
        } else if (enabledState === 'None') {
            return (<Text>No Sites.</Text>);
        } else {
            return (<Text>Not enabled.</Text>);
        }
    };

    const RenderSources = () => {
        return sources && sources.map((source) => {
            return (
                <li key={source.id}>{source.url}</li>
            );
        });
    }

    const handleShowToastNotification = (isSuccess, title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(isSuccess, title, description);

    return (
        <Card my='8'>
            <CardHeader addonAfter={isDeprecated ? <Badge intent='warning'>Deprecated</Badge> : undefined}>{directiveTitle}</CardHeader>
            <Text><em>{directiveDescription}</em></Text>
            {RenderConfiguration()}
            {!props.isInherited && (
                <CardFooter>
                    <EditPermissionPolicy directive={props.directive} showToastNotificationEvent={handleShowToastNotification} appId={appId} hostName={hostName} />
                </CardFooter>
            )}
        </Card>
    )
}

PermissionsPolicyCard.propTypes = {
    directive : PropTypes.shape({
        name: PropTypes.string,
        title: PropTypes.string,
        description: PropTypes.string,
        enabledState: PropTypes.string,
        sources: PropTypes.array,
        isDeprecated: PropTypes.bool
    }),
    directiveSource: PropTypes.shape({
        id: PropTypes.string,
        url: PropTypes.string
    }),
    appId: PropTypes.string,
    hostName: PropTypes.string,
    isInherited: PropTypes.bool
};

export default PermissionsPolicyCard;
