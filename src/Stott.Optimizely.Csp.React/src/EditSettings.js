import React, { useState } from 'react';
import { Button, Form } from 'react-bootstrap';

function EditSettings() {

    const [isCspEnabled, setIsCspEnabled] = useState(true);
    const [isCspReportOnly, setIsCspReportOnly] = useState(true);
    const [disableSaveButton, setDisableSaveButton] = useState(true);

    const handleIsCspEnabledChange = (event) => { setIsCspEnabled(event.target.checked); setDisableSaveButton(false); }
    const handleIsCspReportOnly = (event) => { setIsCspReportOnly(event.target.checked); setDisableSaveButton(false); }

    const handleSaveSettings = (event) => {
        event.preventDefault();
    }

    return (
        <Form>
            <Form.Group className='my-3'>
                <Form.Check type='switch' label="Active" checked={isCspEnabled} onChange={handleIsCspEnabledChange} />
                <div class="form-text">Enabling the Content Security Policy will apply the header to content pages only.</div>
            </Form.Group>
            <Form.Group className='my-3'>
                <Form.Check type='switch' label="Report Only Mode" checked={isCspReportOnly} onChange={handleIsCspReportOnly} />
                <div class="form-text">Only report violations of the Content Security Policy.</div>
            </Form.Group>
            <Form.Group>
                <Button type='submit' disabled={disableSaveButton} onClick={handleSaveSettings}>Save Changes</Button>
            </Form.Group>
        </Form>
    )
}

export default EditSettings