import { useContext, useEffect, useState } from 'react';
import PropTypes from 'prop-types';
import { Alert, Button, Container, Form, InputGroup } from 'react-bootstrap';
import PermissionsPolicyCard from '../PermissionsPolicy/PermissionPolicyCard';
import { StottSecurityContext } from '../Context/StottSecurityContext';
import PermissionPolicySettings from './PermissionPolicySettings';
import ContextSwitcher from '../CSP/ContextSwitcher';

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
            return permissionPolicyCollection.map((directive, index) => (
                <PermissionsPolicyCard key={index} directive={directive} showToastNotificationEvent={handleShowToastNotification} appId={appId} hostName={hostName} isInherited={isContextSpecific && permissionPolicyDirectivesInherited} />
            ));
        }

        return (
            <Alert variant='primary' className='my-3'>No Permissions Policy Directives found for the current filter.</Alert>
        )
    };

    const renderInheritance = () => {
        if (!isContextSpecific) return null;

        if (permissionPolicyDirectivesInherited) {
            return (
                <Container fluid='xl' className='my-3'>
                    <Alert variant='info' className='d-flex align-items-center justify-content-between my-0'>
                        <span>These settings are inherited from the parent configuration.</span>
                        <Button variant='primary' size='sm' onClick={() => createPermissionPolicyOverride(appId, hostName)}>Create Override</Button>
                    </Alert>
                </Container>
            );
        }

        return (
            <Container fluid='xl' className='my-3'>
                <Alert variant='warning' className='d-flex align-items-center justify-content-between my-0'>
                    <span>This context has its own directive overrides.</span>
                    <Button variant='outline-danger' size='sm' onClick={() => deletePermissionPolicyDirectives(appId, hostName)}>Revert to Inherited</Button>
                </Alert>
            </Container>
        );
    };

    const handleShowToastNotification = (isSuccess, title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(isSuccess, title, description);

    useEffect(() => { getPermissionPolicyDirectives(appId, hostName) }, [ permissionPolicySourceFilter, permissionPolicyDirectiveFilter, appId, hostName ]);

    return (
        <>
            <ContextSwitcher appId={appId} hostName={hostName} onContextChange={handleContextChange} />
            {renderInheritance()}
            <Container fluid='xl' className='my-3'>
                <PermissionPolicySettings appId={appId} hostName={hostName} />
            </Container>
            <Container fluid='xl' className='my-3'>
                <InputGroup>
                    <InputGroup.Text id='lblSourceFilters'>Source</InputGroup.Text>
                    <Form.Control id='txtSourceFilter' type='text' value={permissionPolicySourceFilter} onChange={(event) => setPermissionPolicySourceFilter(event.target.value)} aria-describedby='lblSourceFilters' placeholder='Type a partial url'></Form.Control>
                    <InputGroup.Text id='lblSourceFilters'>Configuration</InputGroup.Text>
                    <Form.Select value={permissionPolicyDirectiveFilter} onChange={(event) => setPermissionPolicyDirectiveFilter(event.target.value)} aria-describedby='lblSourceFilters' className='form-control'>
                        <option value='All'>All Directives</option>
                        <option value='AllEnabled'>All Enabled Directives</option>
                        <option value='AllDisabled'>All Disabled Directives</option>
                        <option value='None'>Directives Using None</option>
                        <option value='AllSites'>Directives Using All Sites</option>
                        <option value='ThisSite'>Directives Using This Site</option>
                        <option value='SpecificSites'>Directives Using Specific Sites</option>
                    </Form.Select>
                </InputGroup>
            </Container>
            <Container fluid='xl' className='my-3'>
                {renderDirectives()}
            </Container>
        </>
    )
}

PermissionsPolicyContainer.propTypes = {
    showToastNotificationEvent: PropTypes.func
};

export default PermissionsPolicyContainer;
