import { useContext, useEffect, useState } from 'react';
import PropTypes from 'prop-types';
import { Alert, Box, Button, Field, Grid, Group, Input, Text } from '@optiaxiom/react';
import FormSelect from '../Common/FormSelect';

const directiveFilterOptions = [
    { label: 'All Directives', value: 'All' },
    { label: 'All Enabled Directives', value: 'AllEnabled' },
    { label: 'All Disabled Directives', value: 'AllDisabled' },
    { label: 'Directives Using None', value: 'None' },
    { label: 'Directives Using All Sites', value: 'AllSites' },
    { label: 'Directives Using This Site', value: 'ThisSite' },
    { label: 'Directives Using Specific Sites', value: 'SpecificSites' }
];
import PermissionsPolicyCard from '../PermissionsPolicy/PermissionPolicyCard';
import { StottSecurityContext } from '../Context/StottSecurityContext';
import PermissionPolicySettings from './PermissionPolicySettings';
import ContextSwitcher from '../Common/ContextSwitcher';

function PermissionsPolicyContainer(props)
{
    const [appId, setAppId] = useState(null);
    const [hostName, setHostName] = useState(null);

    const {
        permissionPolicyCollection,
        permissionPolicySourceFilter,
        permissionPolicyDirectiveFilter,
        permissionPolicyDirectivesInherited,
        setPermissionPolicySourceFilter,
        setPermissionPolicyDirectiveFilter,
        getPermissionPolicyDirectives,
        createPermissionPolicyOverride,
        deletePermissionPolicyDirectives
    } = useContext(StottSecurityContext);

    const handleContextChange = (newAppId, newHostName) => {
        setAppId(newAppId);
        setHostName(newHostName);
    };

    const isContextSpecific = !!appId || !!hostName;

    const renderDirectives = () => {
        if (permissionPolicyCollection && permissionPolicyCollection.length > 0) {
            return permissionPolicyCollection.map((directive) => (
                <PermissionsPolicyCard key={directive.name} directive={directive} showToastNotificationEvent={handleShowToastNotification} appId={appId} hostName={hostName} isInherited={isContextSpecific && permissionPolicyDirectivesInherited} />
            ));
        }

        return (
            <Alert intent='information' my='16'>No Permissions Policy Directives found for the current filter.</Alert>
        )
    };

    const renderInheritance = () => {
        if (!isContextSpecific) return null;

        if (permissionPolicyDirectivesInherited) {
            return (
                <Alert intent='information' my='16'>
                    <Group justifyContent='space-between' gap='16' w='full'>
                        <Text>These settings are inherited from the parent configuration.</Text>
                        <Button appearance='primary' size='sm' onClick={() => createPermissionPolicyOverride(appId, hostName)}>Create Override</Button>
                    </Group>
                </Alert>
            );
        }

        return (
            <Alert intent='warning' my='16'>
                <Group justifyContent='space-between' gap='16' w='full'>
                    <Text>This context has its own directive overrides.</Text>
                    <Button appearance='danger-outline' size='sm' onClick={() => deletePermissionPolicyDirectives(appId, hostName)}>Revert to Inherited</Button>
                </Group>
            </Alert>
        );
    };

    const handleShowToastNotification = (isSuccess, title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(isSuccess, title, description);

    useEffect(() => { getPermissionPolicyDirectives(appId, hostName) }, [ permissionPolicySourceFilter, permissionPolicyDirectiveFilter, appId, hostName ]);

    return (
        <>
            <ContextSwitcher appId={appId} hostName={hostName} onContextChange={handleContextChange} />
            <Box className='security-page-container'>
                {renderInheritance()}
                <Box my='16'>
                    <PermissionPolicySettings appId={appId} hostName={hostName} />
                </Box>
                <Grid gridTemplateColumns={{ base: '1', md: '2' }} gap='16' my='16'>
                    <Field label='Source'>
                        <Input id='txtSourceFilter' type='text' value={permissionPolicySourceFilter} onChange={(event) => setPermissionPolicySourceFilter(event.target.value)} placeholder='Type a partial url' />
                    </Field>
                    <Field label='Configuration'>
                        <FormSelect value={permissionPolicyDirectiveFilter} onValueChange={setPermissionPolicyDirectiveFilter} options={directiveFilterOptions} />
                    </Field>
                </Grid>
                <Box my='16'>
                    {renderDirectives()}
                </Box>
            </Box>
        </>
    )
}

PermissionsPolicyContainer.propTypes = {
    showToastNotificationEvent: PropTypes.func
};

export default PermissionsPolicyContainer;
