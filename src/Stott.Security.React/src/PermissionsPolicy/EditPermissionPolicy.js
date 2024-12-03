import React, { useState } from 'react';
import { Button, Card, Form } from 'react-bootstrap';
import FormSourceUrl from '../Common/FormSourceUrl';

function EditPermissionPolicy(props)
{
    const directiveName = props.directiveName ?? '';
    const directiveTitle = props.directiveTitle ?? '';
    const directiveDescription = props.directiveDescription ?? '';

    const [enabledState, setEnabledState] = useState('None');
    const [specificSources, setSpecificSources] = useState(props.specificSources ?? []);

    const handleEnabledStateChange = (event) => {
        setEnabledState(event.target.value);
        if (event.target.value === 'Specific' && specificSources.length === 0) {
            handleAddNewSource();
        }
    };

    const handleRemoveSource = (idToRemove) => {
        var newSpecificSources = specificSources.filter(function (e) { return e.id !== idToRemove });
        setSpecificSources(newSpecificSources);
    }

    const handleUpdateSource = (idToUpdate, sourceUrl) => {
        var newSpecificSources = specificSources.map(x => x);
        newSpecificSources.forEach(item => item.url = item.id === idToUpdate ? sourceUrl : item.value)
        setSpecificSources(newSpecificSources);
    }

    const handleAddNewSource = () => {
        var newSpecificSources = specificSources.map(x => x);
        newSpecificSources.push({ id: crypto.randomUUID(), url: '' });
        setSpecificSources(newSpecificSources);
    }

    const getSourcesClass = () => {
        return enabledState === 'Specific' ? 'd-block my-3' : 'd-none';
    };

    const renderSources = () => {
        return specificSources && specificSources.map((source) => {
            return (
                <FormSourceUrl key={source.id} sourceId={source.id} sourceUrl={source.url} handleDeleteSource={handleRemoveSource} handleUpdateSourceUrl={handleUpdateSource}></FormSourceUrl>
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
                    <div className={getSourcesClass()}>
                        <Form.Label>Specific Sources</Form.Label>
                        {renderSources()}
                        <p>
                            <Button variant='success' type='button' onClick={handleAddNewSource} className='fw-bold'>Add Source</Button>
                        </p>
                    </div>
                </Form>
            </Card.Body>
        </Card>
    )
}

export default EditPermissionPolicy