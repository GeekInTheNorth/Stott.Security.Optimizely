import React, { useState } from 'react';
import { Button, Container, Form } from 'react-bootstrap';
import axios from 'axios';

function EditStrictTransportSecurity(props) {

    const [isStrictTransportHeaderEnabled, setIsStrictTransportHeaderEnabled] = useState(props.isStrictTransportHeaderEnabled);
    const [isIncludeSubDomainsChecked, setIsIncludeSubDomainsChecked] = useState(props.isIncludeSubDomainsChecked);
    const [maxAgeParameter, setMaxAgeParameter] = useState(props.maxAgeParameter);
    const [disableSaveButton, setDisableSaveButton] = useState(true);

    const getDisplayDuration = () => 
    {
        let days = maxAgeParameter / 86400;
        return '(' + days + ' days)';
    }

    const handleIsStrictTransportHeaderEnabled = (event) => 
    { 
        setIsStrictTransportHeaderEnabled(event.target.checked); 
        setIsIncludeSubDomainsChecked(event.target.checked && isIncludeSubDomainsChecked);
        setDisableSaveButton(false); 
    };
    
    const handleIsIncludeSubDomainsChecked = (event) => 
    { 
        setIsIncludeSubDomainsChecked(event.target.checked && isStrictTransportHeaderEnabled);
        setDisableSaveButton(false); 
    };
    
    const handleSetMaxAgeParameter = (event) => { setMaxAgeParameter(event.target.value); setDisableSaveButton(false); };

    const handleShowSuccessToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(true, title, description);
    const handleShowFailureToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(false, title, description);

    const handleSaveSettings = (event) => {
        event.preventDefault();

        let params = new URLSearchParams();
        params.append('isStrictTransportSecurityEnabled', isStrictTransportHeaderEnabled);
        params.append('isStrictTransportSecuritySubDomainsEnabled', isIncludeSubDomainsChecked);
        params.append('strictTransportSecurityMaxAge', maxAgeParameter);
        axios.post(process.env.REACT_APP_SECURITY_STRICT_TRANSPORT_HEADER_SAVE_URL, params)
            .then(() => {
                handleShowSuccessToast('Success', 'Strict Transport Security header settings have been successfully saved.');
            }, () =>{
                handleShowFailureToast('Error', 'Failed to save the Strict Transport Security header settings.');
            });
        setDisableSaveButton(true);
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
                    <Form.Label id='lblMaxAge'>Maximum Age {getDisplayDuration()}</Form.Label>
                    <Form.Range min='86400' max='63072000' step='86400' aria-describedby='lblMaxAge' value={maxAgeParameter} onChange={handleSetMaxAgeParameter}></Form.Range>
                    <div className='form-text'>Sets the max-age parameter of the Strict-Transport-Security header between 1 day and 2 years in increments of 1 day.</div>
                </Form.Group>
                <Form.Group>
                    <Button type='submit' disabled={disableSaveButton} onClick={handleSaveSettings}>Save Changes</Button>
                </Form.Group>
            </Form>
        </Container>
    )
}

export default EditStrictTransportSecurity;