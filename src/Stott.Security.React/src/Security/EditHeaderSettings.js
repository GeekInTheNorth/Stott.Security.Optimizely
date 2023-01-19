import React, { useState } from 'react';
import { Button, Container, Form } from 'react-bootstrap';
import axios from 'axios';

function EditHeaderSettings(props) {

    const [isXctoHeaderEnabled, setIsXctoHeaderEnabled] = useState(props.isXctoHeaderEnabled);
    const [isXfoHeaderEnabled, setIsXfoHeaderEnabled] = useState(props.isXfoHeaderEnabled);
    const [isXxpHeaderEnabled, setIsXxpHeaderEnabled] = useState(props.isXxpHeaderEnabled);
    const [isRpHeaderEnabled, setIsRpHeaderEnabled] = useState(props.isRpHeaderEnabled);
    const [disableSaveButton, setDisableSaveButton] = useState(true);

    const handleIsXctoHeaderEnabled = (event) => {
        setIsXctoHeaderEnabled(event.target.value);
        setDisableSaveButton(false);
    }

    const handleIsXfoHeaderEnabled = (event) => {
        setIsXfoHeaderEnabled(event.target.value);
        setDisableSaveButton(false);
    }

    const handleIsXxpHeaderEnabled = (event) => {
        setIsXxpHeaderEnabled(event.target.value);
        setDisableSaveButton(false);
    }

    const handleIsRpHeaderEnabled = (event) => {
        setIsRpHeaderEnabled(event.target.value);
        setDisableSaveButton(false);
    }

    const handleShowSuccessToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(true, title, description);
    const handleShowFailureToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(false, title, description);

    const handleSaveSettings = (event) => {
        event.preventDefault();

        let params = new URLSearchParams();
        params.append('xContentTypeOptions', isXctoHeaderEnabled);
        params.append('xXssProtection', isXxpHeaderEnabled);
        params.append('xFrameOptions', isXfoHeaderEnabled);
        params.append('referrerPolicy', isRpHeaderEnabled);
        axios.post(process.env.REACT_APP_SECURITY_HEADER_SAVE_URL, params)
            .then(() => {
                handleShowSuccessToast('Success', 'Security Header Settings have been successfully saved.');
            }, () =>{
                handleShowFailureToast('Error', 'Failed to save the Security Header Settings.');
            });
        setDisableSaveButton(true);
    }

    return (
        <Container fluid='md'>
            <Form>
                <Form.Group className='my-3'>
                    <Form.Label id='lblIncludeXContentTypeOptionsHeader'>Include Anti-Sniff Header</Form.Label>
                    <Form.Select label='Include Anti-Sniff Header' aria-describedby='lblIncludeXssPlblIncludeXContentTypeOptionsHeaderrotectionHeader' onChange={handleIsXctoHeaderEnabled} value={isXctoHeaderEnabled}>
                        <option value='None'>Disabled</option>
                        <option value='NoSniff'>No Sniff</option>
                    </Form.Select>
                    <div className='form-text'>Include the X-Content-Type-Options header to prevent styles or scripts being loaded with the incorrect mime types.</div>
                </Form.Group>
                <Form.Group className='my-3'>
                    <Form.Label id='lblIncludeXssProtectionHeader'>Include XSS Protection Header</Form.Label>
                    <Form.Select label='Include XSS Protection Header' aria-describedby='lblIncludeXssProtectionHeader' onChange={handleIsXxpHeaderEnabled} value={isXxpHeaderEnabled}>
                        <option value='None'>Disabled</option>
                        <option value='Enabled'>Enabled</option>
                        <option value='EnabledWithBlocking'>Enabled With Blocking</option>
                    </Form.Select>
                    <div className='form-text'>Include the X-XSS-Protection header set instruct browsers to sanitize or block pages if an XSS attack is detected. Please note browser support for this header is limited.</div>
                </Form.Group>
                <Form.Group className='my-3'>
                    <Form.Label id='lblIncludeFrameOptions'>Include Frame Security Header</Form.Label>
                    <Form.Select label='Include Frame Security Header' aria-describedby='lblIncludeFrameOptions' onChange={handleIsXfoHeaderEnabled} value={isXfoHeaderEnabled}>
                        <option value='None'>Disabled</option>
                        <option value='SameOrigin'>Allow Framing only by this site (SAMEORIGIN)</option>
                        <option value='Deny'>Disallow Framing (DENY)</option>
                    </Form.Select>
                    <div className='form-text'>Configures the X-Frame-Options header to restrict the embedding of pages within frames on third party sites.</div>
                </Form.Group>
                <Form.Group className='my-3'>
                    <Form.Label id='lblIncludeReferrerPolicy'>Include Referrer Policy</Form.Label>
                    <Form.Select label='Include Referrer Policy' aria-describedby='lblIncludeReferrerPolicy' onChange={handleIsRpHeaderEnabled} value={isRpHeaderEnabled}>
                        <option value='None'>Disabled</option>
                        <option value='NoReferrer'>No Referrer</option>
                        <option value='NoReferrerWhenDowngrade'>No referrer When Downgrading (e.g. HTTP &#8594; HTTPS)</option>
                        <option value='Origin'>Origin</option>
                        <option value='OriginWhenCrossOrigin'>Origin When Cross Origin</option>
                        <option value='SameOrigin'>Same Origin</option>
                        <option value='StrictOrigin'>Strict Origin</option>
                        <option value='StrictOriginWhenCrossOrigin'>Strict Origin When Cross Origin</option>
                        <option value='UnsafeUrl'>Unsafe Url</option>
                    </Form.Select>
                    <div className='form-text'>Configures the Referrer-Policy header which instructs the browser on what information it should send in the Referrer header on subsequent requests.</div>
                </Form.Group>
                <Form.Group>
                    <Button type='submit' disabled={disableSaveButton} onClick={handleSaveSettings}>Save Changes</Button>
                </Form.Group>
            </Form>
        </Container>
    )
}

export default EditHeaderSettings