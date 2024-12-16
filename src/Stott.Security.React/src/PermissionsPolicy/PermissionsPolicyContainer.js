import React, { useContext, useEffect } from 'react';
import PropTypes from 'prop-types';
import { Alert, Container, Form, InputGroup } from 'react-bootstrap';
import PermissionsPolicyCard from '../PermissionsPolicy/PermissionPolicyCard';
import { StottSecurityContext } from '../Context/StottSecurityContext';

function PermissionsPolicyContainer()
{
    const { permissionPolicyCollection, permissionPolicySourceFilter, permissionPolicyDirectiveFilter, setPermissionPolicySourceFilter, setPermissionPolicyDirectiveFilter, getPermissionPolicyDirectives } = useContext(StottSecurityContext);

    const renderDirectives = () => {
        if (permissionPolicyCollection && permissionPolicyCollection.length > 0) {
            return permissionPolicyCollection.map((directive, index) => {
                return (
                    <PermissionsPolicyCard key={index} directive={directive} />
                )
            })
        }

        return (
            <Alert variant='primary' className='my-3'>No Permissions Policy Directives found for the current filter.</Alert>
        )
    };

    useEffect(() => { getPermissionPolicyDirectives() }, [ permissionPolicySourceFilter, permissionPolicyDirectiveFilter ]);

    return (
        <>
            <Container fluid>
                <InputGroup>
                    <InputGroup.Text id='lblSourceFilters'>Source</InputGroup.Text>
                    <Form.Control id='txtSourceFilter' type='text' value={permissionPolicySourceFilter} onChange={(event) => setPermissionPolicySourceFilter(event.target.value)} aria-describedby='lblSourceFilters' placeholder='Type a partial url'></Form.Control>
                    <InputGroup.Text id='lblSourceFilters'>Configuration</InputGroup.Text>
                    <Form.Select value={permissionPolicyDirectiveFilter} onChange={(event) => setPermissionPolicyDirectiveFilter(event.target.value)} aria-describedby='lblSourceFilters' className='form-control'>
                        <option value='All'>All Directives</option>
                        <option value='AllEnabled'>All Enabled Directives</option>
                        <option value='Disabled'>All Disabled Directives</option>
                        <option value='AllSites'>Directives Using All Sites</option>
                        <option value='ThisSite'>Directives Using This Site</option>
                        <option value='ThisAndSpecificSites'>Directives Using This Site and Specific Sites</option>
                        <option value='SpecificSites'>Directives Using Specific Sites</option>
                    </Form.Select>
                </InputGroup>
            </Container>
            <Container fluid='md'>
                {renderDirectives()}
            </Container>
        </>
    )
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