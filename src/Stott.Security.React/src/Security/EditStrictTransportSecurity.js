import React, { useState, useEffect } from 'react';
import { Button, Container, Form } from 'react-bootstrap';
import axios from 'axios';

function EditStrictTransportSecurity(props) {

    const [isStrictTransportHeaderEnabled, setIsStrictTransportHeaderEnabled] = useState(false);
    const [isHttpRedirectEnabled, setIsHttpRedirectEnabled] = useState(false);
    const [isIncludeSubDomainsChecked, setIsIncludeSubDomainsChecked] = useState(false);
    const [maxAgeParameter, setMaxAgeParameter] = useState(63072000);
    const [disableSaveButton, setDisableSaveButton] = useState(true);

    const handleIsStrictTransportHeaderEnabled = (event) => { setIsStrictTransportHeaderEnabled(event.target.checked); setDisableSaveButton(false); };
    const handleIsHttpRedirectEnabled = (event) => { setIsHttpRedirectEnabled(event.target.checked); setDisableSaveButton(false); };
    const handleIsIncludeSubDomainsChecked = (event) => { setIsIncludeSubDomainsChecked(event.target.checked); setDisableSaveButton(false); };
    const handleSetMaxAgeParameter = (event) => { setMaxAgeParameter(event.target.value); setDisableSaveButton(false); };

    const handleSaveSettings = (event) => {
        event.preventDefault();

        console.log('save Strict-Transport-Security header event triggered.')
    }

    return(
        <Container fluid='md'>
            <Form>
                <Form.Group className='my-3'>
                    <label>The HTTP Strict Transport Security Header (HSTS) informs browsers that a site can only be accessed using HTTPS and that any future attempts to access it using HTTP should automatically be converted to HTTPS.</label>
                </Form.Group>
                <hr/>
                <Form.Group className='my-3'>
                    <Form.Check type='switch' label="Enable Strict Transport Security Header" checked={isStrictTransportHeaderEnabled} onChange={handleIsStrictTransportHeaderEnabled} />
                    <div className='form-text'>Include the Strict-Transport-Security header on content pages.</div>
                </Form.Group>
                <Form.Group className='my-3'>
                    <Form.Check type='switch' label="Include Subdomains" checked={isIncludeSubDomainsChecked} onChange={handleIsIncludeSubDomainsChecked} />
                    <div className='form-text'>Add the includeSubDomains parameter to the Strict-Transport-Security header.</div>
                </Form.Group>
                <Form.Group className='my-3'>
                    <Form.Label id='lblMaxAge'>Maximum Age</Form.Label>
                    <Form.Range min='86400' max='63072000' step='86400' aria-describedby='lblMaxAge' value={maxAgeParameter} onChange={handleSetMaxAgeParameter}></Form.Range>
                    <div className='form-text'>Sets the max-age parameter of the Strict-Transport-Security header between 1 day and 2 years in increments of 1 day.</div>
                </Form.Group>
                <Form.Group className='my-3'>
                    <Form.Check type='switch' label="Enable Redirect from HTTP to HTTPS" checked={isHttpRedirectEnabled} onChange={handleIsHttpRedirectEnabled} />
                    <div className='form-text'>Always return a redirect response for HTTP requests to enforce the use of HTTPS.</div>
                </Form.Group>
                <Form.Group>
                    <Button type='submit' disabled={disableSaveButton} onClick={handleSaveSettings}>Save Changes</Button>
                </Form.Group>
            </Form>
        </Container>
    )
}

export default EditStrictTransportSecurity;