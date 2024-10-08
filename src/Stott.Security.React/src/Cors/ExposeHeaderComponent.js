import React, { useState, useEffect } from 'react';
import { Button, Form } from 'react-bootstrap';
import EditCorsHeader from './EditCorsHeader';

function ExposeHeaderComponent (props) {

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

    const handleAddContentDeliveryApiHeaders = () => {
        var newHttpHeaders = httpHeaders.map(x => x);
        
        const contentDeliveryHeaders = [ 'x-epi-contentguid', 'x-epi-branch', 'x-epi-siteid', 'x-epi-startpageguid', 'x-epi-remainingroute', 'x-epi-contextmode' ];
        contentDeliveryHeaders.forEach(cdHeader => {
            if (!headerExists(cdHeader)) {
                newHttpHeaders.push({ id: crypto.randomUUID(), value: cdHeader });
            }
        });

        setHttpHeaders(newHttpHeaders);
        handleParentSave(newHttpHeaders);
    }

    const headerExists = (headerName) => {
        return httpHeaders.find(h => h.value === headerName) !== undefined;
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
            <Form.Label id='lblAllowedHttpHeaders'>Expose Headers:</Form.Label>
            {renderAllowedHeaders()}
            <p>
                <Button variant='success' type='button' onClick={handleAddHttpHeader} className='fw-bold me-2'>Add Header</Button>
                <Button variant='primary' type='button' onClick={handleAddContentDeliveryApiHeaders} className='fw-bold'>Add Content Delivery API Headers</Button>
            </p>
            <div className='form-text'>Configures the 'Access-Control-Expose-Headers' header which will be sent from this webserver to the browser to instruct the browser which headers may be exposed to scripts running within the browser when making a cross-origin request. If no headers are provided here, then all headers will be considered as allowed.</div>
            <div className='form-text'>Please note that 'Cache-Control', 'Content-Language', 'Content-Length', 'Content-Type', 'Expires', 'Last-Modified' and 'Pragma' are considered as safe headers and do not need to be defined here.</div>
        </Form.Group>
    )
}

export default ExposeHeaderComponent