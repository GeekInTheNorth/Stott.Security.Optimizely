import React, { useState, useEffect } from 'react';
import { Button, Form } from 'react-bootstrap';
import EditCorsHeader from './EditCorsHeader';

function HeaderComponent (props) {

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

    const getHeaderLabel = () => {
        if (props.headerType && props.headerType === 'Expose'){
            return 'Expose Headers:';
        }

        return 'Allow Headers:';
    }

    const getPrimaryGuidance = () => {
        if (props.headerType && props.headerType === 'Expose'){
            return 'Configures the \'Access-Control-Expose-Headers\' header which will be sent from this webserver to the browser to instruct the browser which headers may be exposed to scripts running within the browser when making a cross-origin request. If no headers are provided here, then all headers will be considered as allowed.';
        }

        return 'Configures the \'Access-Control-Allow-Headers\' header which will be sent from this webserver to the browser to instruct the browser which headers that may be allowed within a web request. If no headers are provided here, then all headers will be considered as allowed.';
    }

    const getSecondaryGuidance = () => {
        if (props.headerType && props.headerType === 'Expose'){
            return 'Please note that \'Cache-Control\', \'Content-Language\', \'Content-Length\', \'Content-Type\', \'Expires\', \'Last-Modified\' and \'Pragma\' are considered as safe headers and do not need to be defined here.';
        }

        return 'Please note that \'Accept\', \'Accept-Language\', \'Content-Language\' and \'Content-Type\' are considered as safe headers and do not need to be defined here.';
    }

    const setUpState = () => {
        setHttpHeaders(props.headers);
    }

    useEffect(() => {
        setUpState()
    }, [props.headers])

    return (
        <Form.Group className='my-3'>
            <Form.Label id='lblAllowedHttpHeaders'>{getHeaderLabel()}</Form.Label>
            <p><Button variant='success' type='button' onClick={handleAddHttpHeader} className='fw-bold'>+</Button></p>
            {renderAllowedHeaders()}
            <div className='form-text'>{getPrimaryGuidance()}</div>
            <div className='form-text'>{getSecondaryGuidance()}</div>
        </Form.Group>
    )
}

export default HeaderComponent