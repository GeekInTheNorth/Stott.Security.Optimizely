import React, { useState } from 'react';
import { Button, Form } from 'react-bootstrap';
import EditCorsOrigin from './EditCorsOrigin';

function OriginComponent (props) {

    const [allowedOrigins, setAllowedOrigins] = useState(props.origins ?? []);

    const handleParentSave = (originsToSave) => props.handleOriginUpdate && props.handleOriginUpdate(originsToSave);

    const handleRemoveAllowedOrigin = (origin) => {
        var newAllowedOrigins = allowedOrigins.filter(function (e) { return e !== origin });
        setAllowedOrigins(newAllowedOrigins);
        handleParentSave(newAllowedOrigins);
    };

    const handleUpdateAllowedOrigin = (index, newOrigin) => {
        var newAllowedOrigins = allowedOrigins.map(x => x);
        newAllowedOrigins[index] = newOrigin;

        setAllowedOrigins(newAllowedOrigins);
        handleParentSave(newAllowedOrigins);
    };

    const handleAddAllowedOrigin = () => {
        var newAllowedOrigins = allowedOrigins.map(x => x);
        newAllowedOrigins.push("");
        setAllowedOrigins(newAllowedOrigins);
        handleParentSave(newAllowedOrigins);
    }

    const renderAllowedOrigins = () => {
        return allowedOrigins && allowedOrigins.map((origin, index) => {
            return (
                <EditCorsOrigin key={index} originIndex={index} originUrl={origin} handleDeleteOrigin={handleRemoveAllowedOrigin} handleUpdateOrigin={handleUpdateAllowedOrigin}></EditCorsOrigin>
            )
        })
    }

    return (
        <Form.Group className='my-3'>
            <Form.Label id='lblAllowedHttpOrigins'>Allowed Origins:</Form.Label>
            <p><Button variant='success' type="button" onClick={handleAddAllowedOrigin} className='fw-bold'>+</Button></p>
            {renderAllowedOrigins()}
            <div className='form-text'>Configures the 'Access-Control-Allow-Origin' header which instructs the browser whether the request can be consumed by website making the request. If no origins are provided here, then all origins will be considered as allowed.</div>
        </Form.Group>
    )
}

export default OriginComponent