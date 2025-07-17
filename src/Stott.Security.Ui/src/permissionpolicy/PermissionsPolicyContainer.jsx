import React, { useContext, useEffect } from 'react';
import PropTypes from 'prop-types';
import { Alert, Container, Form, InputGroup } from 'react-bootstrap';
import PermissionPolicyCard from './PermissionPolicyCard';
import { PermissionPolicyContext } from './PermissionPolicy';
import PermissionPolicySettings from './PermissionPolicySettings';

function PermissionsPolicyContainer(props) {
    const { permissionPolicyCollection, permissionPolicySourceFilter, permissionPolicyDirectiveFilter, setPermissionPolicySourceFilter, setPermissionPolicyDirectiveFilter, getPermissionPolicyDirectives } = useContext(PermissionPolicyContext);

    const renderDirectives = () => {
        if (permissionPolicyCollection && permissionPolicyCollection.length > 0) {
            return permissionPolicyCollection.map((directive, index) => {
                return (
                    <PermissionPolicyCard key={index} directive={directive} showToastNotificationEvent={handleShowToastNotification} />
                );
            });
        }
        return (
            <Alert variant='primary' className='my-3'>No Permissions Policy Directives found for the current filter.</Alert>
        );
    };

    const handleShowToastNotification = (isSuccess, title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(isSuccess, title, description);

    useEffect(() => { getPermissionPolicyDirectives(); }, [permissionPolicySourceFilter, permissionPolicyDirectiveFilter]);

    return (
        <>
            <Container fluid='xl' className='my-3'>
                <PermissionPolicySettings />
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
    );
}

PermissionsPolicyContainer.propTypes = {
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

export default PermissionsPolicyContainer; 