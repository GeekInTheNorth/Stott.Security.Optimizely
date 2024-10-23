import React, { useState } from 'react';
import { Card, Form } from 'react-bootstrap';
import EditPermissionPolicySource from './EditPermissionPolicySource';

function EditPermissionPolicy(props)
{
    const directiveName = props.directiveName ?? '';
    const directiveTitle = props.directiveTitle ?? '';
    const directiveDescription = props.directiveDescription ?? '';

    const [enabledState, setEnabledState] = useState('None');
    const [specificSources, setSpecificSources] = useState([]);

    const handleEnabledStateChange = (event) => {
        setEnabledState(event.target.value);
    };

    const handleRemoveSource = () => {
        // Remove source
    }

    const handleUpdateSource = () => {
        // Update source
    }

    const getSourcesClass = () => {
        return enabledState === 'Specific' ? 'd-block my-3' : 'd-none';
    };

    const renderSources = () => {
        return specificSources && specificSources.map((source) => {
            return (
                <EditPermissionPolicySource key={source.id} sourceId={origin.id} sourceUrl={source.url} handleDeleteSource={handleRemoveSource} handleUpdateSource={handleUpdateSource}></EditPermissionPolicySource>
            )
        })
    };

    return (
        <Card className='my-3'>
            <Card.Header className='bg-primary text-light'>Edit {directiveTitle}</Card.Header>
            <Card.Body>
                <Card.Text>{directiveDescription}</Card.Text>
                <Form>
                    <Form.Group className='my-3'>
                        <Form.Label id='lblEnabledState'>Include Anti-Sniff Header</Form.Label>
                        <Form.Select label='Enabled State' aria-describedby='lblEnabledState' onChange={handleEnabledStateChange} value={enabledState}>
                            <option value='None' className='header-value'>Allow None</option>
                            <option value='All' className='header-value'>Allow All</option>
                            <option value='Specific' className='header-value'>Allow Specific</option>
                        </Form.Select>
                    </Form.Group>
                    <Form.Group className={getSourcesClass()}>
                        <Form.Label id='lblSpecificSources'>Specific Sources</Form.Label>
                        {renderSources()}
                    </Form.Group>
                </Form>
            </Card.Body>
        </Card>
    )
}

export default EditPermissionPolicy