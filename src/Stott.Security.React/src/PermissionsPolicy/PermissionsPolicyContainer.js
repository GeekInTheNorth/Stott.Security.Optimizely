import React, { useState, useEffect } from 'react';
import PropTypes from 'prop-types';
import axios from 'axios';
import { Alert, Button, Container, Form, InputGroup } from 'react-bootstrap';
import PermissionsPolicyCard from '../PermissionsPolicy/PermissionPolicyCard';

function PermissionsPolicyContainer(props)
{
    const displayAsTable = false;
    const [sourceFilter, setSourceFilter] = useState('');
    const [directiveFilter, setDirectiveFilter] = useState('AllEnabled');
    const [directives, setDirectives] = useState([]);

    const handleShowFailureToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(false, title, description);

    const getPermissionPolicyDirectives = async () => {
        await axios.get(process.env.REACT_APP_PERMISSION_POLICY_LIST, { params: { sourceFilter: sourceFilter, enabledFilter: directiveFilter } })
            .then((response) => {
                if (response.data && Array.isArray(response.data)){
                    setDirectives(response.data);
                }
                else{
                    handleShowFailureToast("Get Permissions Policy Directives", "Failed to retrieve Permissions Policy Directives.");
                }
            },
            () => {
                handleShowFailureToast("Error", "Failed to retrieve the Permissions Policy Directives.");
            });
    };

    useEffect(() => { getPermissionPolicyDirectives() }, [ sourceFilter, directiveFilter ]);

    const renderDirectives = () => {
        if (directives && directives.length > 0) {
            return directives.map((directive, index) => {
                return (
                    <PermissionsPolicyCard key={index} directive={directive} />
                )
            })
        }

        return (
            <Alert variant='primary' className='my-3'>No Permissions Policy Directives found for the current filter.</Alert>
        )
    };

    const renderStateText = (directive) => {
        if (directive.enabledState === 'All') {
            return (<p className='my-0'>All Sites</p>);
        } else if (directive.enabledState === 'None') {
            return (<p className='my-0'>None</p>);
        } else if (directive.enabledState === 'ThisSite' || directive.enabledState === 'ThisAndSpecificSites') {
            return (<p className='my-0'>This Site</p>);
        } else {
            return '';
        }
    };

    const renderDirectivesAsTableSources = (directive) => {
        if (directive.enabledState === 'ThisAndSpecificSites' || directive.enabledState === 'SpecificSites') {
            return directive.sources && directive.sources.map((directiveSource) => {
                return (
                    <p key={directiveSource.id} className='my-0'>{directiveSource.url}</p>
                )
            })
        } else {
            return '';
        }
    };

    const renderDirectivesAsTableRows = () => {
        return directives && directives.map((directive, index) => {
            return (
                <tr key={index}>
                    <td>{directive.title}</td>
                    <td className='directive-description one-third'>{directive.description}</td>
                    <td className='one-third'>{renderStateText(directive)}{renderDirectivesAsTableSources(directive)}</td>
                    <td><Button variant='primary'>Edit</Button></td>
                </tr>
            )
        })
    };

    const renderDirectivesAsTable = () => {
        return (
            <table className='table table-striped table-permissions'>
                <thead>
                    <tr>
                        <th>Title</th>
                        <th className='directive-description one-third'>Description</th>
                        <th className='one-third'>Enabled For</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {renderDirectivesAsTableRows()}
                </tbody>
            </table>
        )
    };

    return (
        <>
            <Container fluid>
                <InputGroup>
                    <InputGroup.Text id='lblSourceFilters'>Source</InputGroup.Text>
                    <Form.Control id='txtSourceFilter' type='text' value={sourceFilter} onChange={(event) => setSourceFilter(event.target.value)} aria-describedby='lblSourceFilters' placeholder='Type a partial url'></Form.Control>
                    <InputGroup.Text id='lblSourceFilters'>Configuration</InputGroup.Text>
                    <Form.Select value={directiveFilter} onChange={(event) => setDirectiveFilter(event.target.value)} aria-describedby='lblSourceFilters' className='form-control'>
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
                { displayAsTable ? renderDirectivesAsTable() : renderDirectives() }
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