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

    const getSelectListStyle = (selectedValue) => {
        return selectedValue === 'None' ? 'no-header-value' : '';
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
        axios.post(import.meta.env.VITE_APP_SECURITY_HEADER_SAVE, params)
            .then(() => {
                handleShowSuccessToast('Success', 'Security Header Settings have been successfully saved.');
            }, () =>{
                handleShowFailureToast('Error', 'Failed to save the Security Header Settings.');
            });
        setDisableSaveButton(true);
    }

    return (
        <Form>
            <Form.Group className='my-3'>
                <Form.Label id='lblIncludeXContentTypeOptionsHeader'>Include Anti-Sniff Header</Form.Label>
                <Form.Select label='Include Anti-Sniff Header' aria-describedby='lblIncludeXContentTypeOptionsHeader' onChange={handleIsXctoHeaderEnabled} value={isXctoHeaderEnabled} className={getSelectListStyle(isXctoHeaderEnabled)}>
                    <option value='None' className='no-header-value'>Select a header value...</option>
                    <option value='NoSniff' className='header-value'>No Sniff</option>
                </Form.Select>
                <div className='form-text'>Include the X-Content-Type-Options header to prevent styles or scripts being loaded with the incorrect mime types.</div>
            </Form.Group>
            <Form.Group className='my-3'>
                <Form.Label id='lblIncludeXssProtectionHeader'>Include XSS Protection Header</Form.Label>
                <Form.Select label='Include XSS Protection Header' aria-describedby='lblIncludeXssProtectionHeader' onChange={handleIsXxpHeaderEnabled} value={isXxpHeaderEnabled} className={getSelectListStyle(isXxpHeaderEnabled)}>
                    <option value='None' className='no-header-value'>Select a header value...</option>
                    <option value='Disabled' className='header-value'>Disabled</option>
                    <option value='Enabled' className='header-value'>Enabled</option>
                    <option value='EnabledWithBlocking' className='header-value'>Enabled With Blocking</option>
                </Form.Select>
                <div className='form-text'>
                    Includes the X-XSS-Protection header to instruct browsers to use XSS filters.
                    Please note that modern browsers have either retired or will not implement XSS filtering.
                    Legacy browsers have been known to contain vulnerabilities within their XSS filters that can compromise otherwise safe websites.
                    It is recommended to set the header to 'Disabled' and to configure a Content Security Policy header.
                    Only enable the X-XSS-Protection header if you must support legacy browsers.
                </div>
            </Form.Group>
            <Form.Group className='my-3'>
                <Form.Label id='lblIncludeFrameOptions'>Include Frame Security Header</Form.Label>
                <Form.Select label='Include Frame Security Header' aria-describedby='lblIncludeFrameOptions' onChange={handleIsXfoHeaderEnabled} value={isXfoHeaderEnabled} className={getSelectListStyle(isXfoHeaderEnabled)}>
                    <option value='None' className='no-header-value'>Select a header value...</option>
                    <option value='SameOrigin' className='header-value'>Allow Framing only by this site (SAMEORIGIN)</option>
                    <option value='Deny' className='header-value'>Disallow Framing (DENY)</option>
                </Form.Select>
                <div className='form-text'>Configures the X-Frame-Options header to restrict the embedding of pages within frames on third party sites.</div>
            </Form.Group>
            <Form.Group className='my-3'>
                <Form.Label id='lblIncludeReferrerPolicy'>Include Referrer Policy</Form.Label>
                <Form.Select label='Include Referrer Policy' aria-describedby='lblIncludeReferrerPolicy' onChange={handleIsRpHeaderEnabled} value={isRpHeaderEnabled} className={getSelectListStyle(isRpHeaderEnabled)}>
                    <option value='None' className='no-header-value'>Select a header value...</option>
                    <option value='NoReferrer' className='header-value'>No Referrer</option>
                    <option value='NoReferrerWhenDowngrade' className='header-value'>No referrer When Downgrading (e.g. HTTP &#8594; HTTPS)</option>
                    <option value='Origin' className='header-value'>Origin</option>
                    <option value='OriginWhenCrossOrigin' className='header-value'>Origin When Cross Origin</option>
                    <option value='SameOrigin' className='header-value'>Same Origin</option>
                    <option value='StrictOrigin' className='header-value'>Strict Origin</option>
                    <option value='StrictOriginWhenCrossOrigin' className='header-value'>Strict Origin When Cross Origin</option>
                    <option value='UnsafeUrl' className='header-value'>Unsafe Url</option>
                </Form.Select>
                <div className='form-text'>Configures the Referrer-Policy header which instructs the browser on what information it should send in the Referrer header on subsequent requests.</div>
            </Form.Group>
            <Form.Group>
                <Button type='submit' disabled={disableSaveButton} onClick={handleSaveSettings}>Save Changes</Button>
            </Form.Group>
        </Form>
    )
}

export default EditHeaderSettings; 