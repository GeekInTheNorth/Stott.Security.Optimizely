import React, { useState, useEffect } from 'react';
import { Button, Form } from 'react-bootstrap';
import FormSourceUrl from '../Common/FormSourceUrl';

function OriginComponent (props) {

    const [allowedOrigins, setAllowedOrigins] = useState([]);

    const handleParentSave = (originsToSave) => props.handleOriginUpdate && props.handleOriginUpdate(originsToSave);

    const handleRemoveAllowedOrigin = (idToRemove) => {
        var newAllowedOrigins = allowedOrigins.filter(function (e) { return e.id !== idToRemove });
        setAllowedOrigins(newAllowedOrigins);
        handleParentSave(newAllowedOrigins);
    };

    const handleUpdateAllowedOrigin = (originId, newOrigin) => {
        var newAllowedOrigins = allowedOrigins.map(x => x);
        newAllowedOrigins.forEach(item => item.value = item.id === originId ? newOrigin : item.value)

        setAllowedOrigins(newAllowedOrigins);
        handleParentSave(newAllowedOrigins);
    };

    const handleAddAllowedOrigin = () => {
        var newAllowedOrigins = allowedOrigins.map(x => x);
        newAllowedOrigins.push({ id: crypto.randomUUID(), value: '' });
        setAllowedOrigins(newAllowedOrigins);
        handleParentSave(newAllowedOrigins);
    }

    const renderAllowedOrigins = () => {
        return allowedOrigins && allowedOrigins.map((origin) => {
            return (
                <FormSourceUrl key={origin.id} sourceId={origin.id} sourceUrl={origin.value} handleDeleteSource={handleRemoveAllowedOrigin} handleUpdateSourceUrl={handleUpdateAllowedOrigin}></FormSourceUrl>
            )
        })
    }

    const setUpState = () => {
        setAllowedOrigins(props.origins);
    }

    useEffect(() => {
        setUpState()
    }, [props.origins])

    return (
        <Form.Group className='my-3'>
            <Form.Label id='lblAllowedHttpOrigins'>Allowed Origins:</Form.Label>
            {renderAllowedOrigins()}
            <p><Button variant='success' type="button" onClick={handleAddAllowedOrigin} className='fw-bold'>Add Origin</Button></p>
            <div className='form-text'>Configures the 'Access-Control-Allow-Origin' header which instructs the browser whether the request can be consumed by website making the request. If no origins are provided here, then all origins will be considered as allowed.</div>
        </Form.Group>
    )
}

export default OriginComponent