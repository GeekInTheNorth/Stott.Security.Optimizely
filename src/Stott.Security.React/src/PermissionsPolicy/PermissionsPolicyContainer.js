import React, { useState, useEffect } from 'react';
import PropTypes from 'prop-types';
import axios from 'axios';
import EditPermissionPolicy from '../PermissionsPolicy/EditPermissionPolicy';
import { Button, Container, Form } from 'react-bootstrap';

function PermissionsPolicyContainer(props)
{
    const [displayAsTable, setDisplayAsTable] = useState(false);
    const [displayDisabledDirectives, setDisplayDisabledDirectives] = useState(false);
    const [directives, setDirectives] = useState([]);

    const handleShowFailureToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(false, title, description);

    const getPermissionPolicyDirectives = async () => {
        await axios.get(process.env.REACT_APP_PERMISSION_POLICY_LIST)
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

    useEffect(() => { getPermissionPolicyDirectives() }, []);

    const renderDirectives = () => {
        return directives && directives.map((directive, index) => {
            if (!displayDisabledDirectives && directive.enabledState === 'None') {
                return '';
            }
            
            return (
                <EditPermissionPolicy key={index} directive={directive} />
            )
        })
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
            if (!displayDisabledDirectives && directive.enabledState === 'None') {
                return '';
            }

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
                        <th className='one-third'>Status</th>
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
        <Container fluid='md'>
            <Form.Check type='switch' id='displayAsTable' label='Display as Table' checked={displayAsTable} onChange={() => setDisplayAsTable(!displayAsTable)} />
            <Form.Check type='switch' id='displayDisabledDirectives' label='Display Disabled Directives' checked={displayDisabledDirectives} onChange={() => setDisplayDisabledDirectives(!displayDisabledDirectives)} />
            { displayAsTable ? renderDirectivesAsTable() : renderDirectives() }
        </Container>
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