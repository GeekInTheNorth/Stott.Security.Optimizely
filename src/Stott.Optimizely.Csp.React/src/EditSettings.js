import React, { useState, useEffect } from 'react';
import { Button, Container, Form } from 'react-bootstrap';
import axios from 'axios';

function EditSettings() {

    const [isCspEnabled, setIsCspEnabled] = useState(true);
    const [isCspReportOnly, setIsCspReportOnly] = useState(true);
    const [disableSaveButton, setDisableSaveButton] = useState(true);

    useEffect(() => {
        getCspSettings()
    }, [])

    const getCspSettings = async () => {
        const response = await axios.get(process.env.REACT_APP_SETTINGS_GET_URL)
        setIsCspReportOnly(response.data.isReportOnly);
        setIsCspEnabled(response.data.isEnabled);
        setDisableSaveButton(true);
    }

    const handleIsCspEnabledChange = (event) => { 
        setIsCspEnabled(event.target.checked); 
        setIsCspReportOnly(event.target.checked && isCspReportOnly);
        setDisableSaveButton(false);
    }

    const handleIsCspReportOnly = (event) => {
        setIsCspReportOnly(event.target.checked && isCspEnabled);
        setDisableSaveButton(false); 
    }

    const handleSaveSettings = (event) => {
        event.preventDefault();

        let params = new URLSearchParams();
        params.append('isEnabled', isCspEnabled);
        params.append('isReportOnly', isCspReportOnly);
        axios.post(process.env.REACT_APP_SETTINGS_SAVE_URL, params);
        setDisableSaveButton(true);
    }

    return (
        <Container fluid='md'>
            <Form>
                <Form.Group className='my-3'>
                    <Form.Check type='switch' label='Active' checked={isCspEnabled} onChange={handleIsCspEnabledChange} />
                    <div className='form-text'>Enabling the Content Security Policy will apply the header to content pages only.</div>
                </Form.Group>
                <Form.Group className='my-3'>
                    <Form.Check type='switch' label="Report Only Mode" checked={isCspReportOnly} onChange={handleIsCspReportOnly} />
                    <div className='form-text'>Only report violations of the Content Security Policy.</div>
                </Form.Group>
                <Form.Group>
                    <Button type='submit' disabled={disableSaveButton} onClick={handleSaveSettings}>Save Changes</Button>
                </Form.Group>
            </Form>
        </Container>
    )
}

export default EditSettings