import React, { useState, useEffect } from 'react';
import { Button, Container, Form } from 'react-bootstrap';
import axios from 'axios';

function EditHeaderSettings(props) {

    const [isXctoHeaderEnabled, setIsXctoHeaderEnabled] = useState(false);
    const [isXfoHeaderEnabled, setIsXfoHeaderEnabled] = useState('None');
    const [isXxpHeaderEnabled, setIsXxpHeaderEnabled] = useState(false);
    const [isRpHeaderEnabled, setIsRpHeaderEnabled] = useState('None');
    const [disableSaveButton, setDisableSaveButton] = useState(true);

    useEffect(() => {
        getCspSettings()
    }, [])

    const getCspSettings = async () => {
        const response = await axios.get(process.env.REACT_APP_SECURITYHEADER_GET_URL)
        setIsXctoHeaderEnabled(response.data.isXctoEnabled);
        setIsXfoHeaderEnabled(response.data.xFrameOptions);
        setIsXxpHeaderEnabled(response.data.isXxpEnabled);
        setIsRpHeaderEnabled(response.data.referrerPolicy);
        setDisableSaveButton(true);
    }

    const handleIsXctoHeaderEnabled = (event) => {
        setIsXctoHeaderEnabled(event.target.checked);
        setDisableSaveButton(false);
    }

    const handleIsXfoHeaderEnabled = (event) => {
        setIsXfoHeaderEnabled(event.target.value);
        setDisableSaveButton(false);
    }

    const handleIsXxpHeaderEnabled = (event) => {
        setIsXxpHeaderEnabled(event.target.checked);
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
        params.append('isXctoEnabled', isXctoHeaderEnabled);
        params.append('isXxpEnabled', isXxpHeaderEnabled);
        params.append('xFrameOptions', isXfoHeaderEnabled);
        params.append('referrerPolicy', isRpHeaderEnabled);
        axios.post(process.env.REACT_APP_SECURITYHEADER_SAVE_URL, params)
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
                    <Form.Check type='switch' label="Include Anti-Sniff Header" checked={isXctoHeaderEnabled} onChange={handleIsXctoHeaderEnabled} />
                    <div className='form-text'>Include the X-Content-Type-Options header set to nosniff on content pages.</div>
                </Form.Group>
                <Form.Group className='my-3'>
                    <Form.Check type='switch' label="Include XSS Protection Header" checked={isXxpHeaderEnabled} onChange={handleIsXxpHeaderEnabled} />
                    <div className='form-text'>Include the X-XSS-Protection header set to active with blocking enabled. This is deprecated by most browsers and is only supported by IE and Safari.</div>
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