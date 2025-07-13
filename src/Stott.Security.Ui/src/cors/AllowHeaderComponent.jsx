import React, { useState, useEffect } from 'react';
import { Button, Form } from 'react-bootstrap';
import EditCorsHeader from './EditCorsHeader.jsx';

function AllowHeaderComponent (props) {
    const [httpHeaders, setHttpHeaders] = useState([]);

    const handleParentSave = (httpHeadersToSave) => props.handleHeaderUpdate && props.handleHeaderUpdate(httpHeadersToSave);

    const handleRemoveHttpHeader = (idToRemove) => {
        var newHttpHeaders = httpHeaders.filter(function (e) { return e.id !== idToRemove });
        setHttpHeaders(newHttpHeaders);
        handleParentSave(newHttpHeaders);
    };

    const handleUpdateHttpHeader = (headerId, newHeaderName) => {
        var newHttpHeaders = httpHeaders.map(x => x);
        newHttpHeaders.forEach(item => item.value = item.id === headerId ? newHeaderName : item.value)
        setHttpHeaders(newHttpHeaders);
        handleParentSave(newHttpHeaders);
    };

    const handleAddHttpHeader = () => {
        var newHttpHeaders = httpHeaders.map(x => x);
        newHttpHeaders.push({ id: crypto.randomUUID(), value: '' });
        setHttpHeaders(newHttpHeaders);
        handleParentSave(newHttpHeaders);
    }

    const renderAllowedHeaders = () => {
        return httpHeaders && httpHeaders.map((header) => {
            return (
                <EditCorsHeader key={header.id} headerId={header.id} headerName={header.value} handleDeleteHeader={handleRemoveHttpHeader} handleUpdateHeader={handleUpdateHttpHeader}></EditCorsHeader>
            )
        })
    }

    const setUpState = () => {
        setHttpHeaders(props.headers);
    }

    useEffect(() => {
        setUpState()
    }, [props.headers])

    return (
        <Form.Group className='my-3'>
            <Form.Label id='lblAllowedHttpHeaders'>Allow Headers:</Form.Label>
            <div className="border border-secondary rounded p-3">
                {renderAllowedHeaders()}
                <p>
                    <Button variant='success' type='button' onClick={handleAddHttpHeader} className='fw-bold'>Add Header</Button>
                </p>
                <div className='form-text'>Configures the 'Access-Control-Allow-Headers' header which will be sent from this webserver to the browser to instruct the browser which headers that may be allowed within a web request. If no headers are provided here, then all headers will be considered as allowed.</div>
                <div className='form-text'>Please note that 'Accept', 'Accept-Language', 'Content-Language' and 'Content-Type' are considered as safe headers and do not need to be defined here.</div>
            </div>
        </Form.Group>
    )
}

export default AllowHeaderComponent; 