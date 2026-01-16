import { useState } from 'react';
import PropTypes from 'prop-types';
import { Button, Container, Form } from 'react-bootstrap';
import axios from 'axios';

function EditCrossOriginHeaders(props) {

    const [disableSaveButton, setDisableSaveButton] = useState(true);
    const [crossOriginEmbedderPolicy, setCrossOriginEmbedderPolicy] = useState(props.crossOriginEmbedderPolicy);
    const [crossOriginOpenerPolicy, setCrossOriginOpenerPolicy] = useState(props.crossOriginOpenerPolicy);
    const [crossOriginResourcePolicy, setCrossOriginResourcePolicy] = useState(props.crossOriginResourcePolicy);

    const handleCrossOriginEmbedderPolicy = (event) => {
        setCrossOriginEmbedderPolicy(event.target.value);
        setDisableSaveButton(false);
    }

    const handleCrossOriginOpenerPolicy = (event) => {
        setCrossOriginOpenerPolicy(event.target.value);
        setDisableSaveButton(false);
    }

    const handleCrossOriginResourcePolicy = (event) => {
        setCrossOriginResourcePolicy(event.target.value);
        setDisableSaveButton(false);
    }

    const getSelectListStyle = (selectedValue) => {
        return selectedValue === 'None' ? 'no-header-value' : '';
    }

    const handleShowSuccessToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(true, title, description);
    const handleShowFailureToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(false, title, description);

    const handleSaveSettings = (event) => {
        event.preventDefault();

        handleShowSuccessToast('Success', 'Cross Origin Header settings have been successfully saved.');

        let params = new URLSearchParams();
        params.append('crossOriginEmbedderPolicy', crossOriginEmbedderPolicy);
        params.append('crossOriginOpenerPolicy', crossOriginOpenerPolicy);
        params.append('crossOriginResourcePolicy', crossOriginResourcePolicy);
        axios.post(import.meta.env.VITE_SECURITY_CROSS_ORIGIN_SAVE_URL, params)
            .then(() => {
                handleShowSuccessToast('Success', 'Cross Origin Policy header settings have been successfully saved.');
            }, () =>{
                handleShowFailureToast('Error', 'Failed to save the Cross Origin Policy header settings.');
            });
        setDisableSaveButton(true);
    }

    return (
        <Container fluid='md'>
            <Form>
                <Form.Group className='my-3'>
                    <Form.Label id='lblIncludeCrossOriginEmbedderPolicy'>Include Cross Origin Embedder Policy</Form.Label>
                    <Form.Select label='Include Cross Origin Embedder Policy' aria-describedby='lblIncludeCrossOriginEmbedderPolicy' onChange={handleCrossOriginEmbedderPolicy} value={crossOriginEmbedderPolicy} className={getSelectListStyle(crossOriginEmbedderPolicy)}>
                        <option value='None' className='no-header-value'>Select a header value...</option>
                        <option value='UnsafeNone' className='header-value'>Unsafe None</option>
                        <option value='RequireCorp' className='header-value'>Requires CORP</option>
                    </Form.Select>
                    <div className='form-text'>Configures the Cross-Origin-Embedder-Policy header which is used to prevent third party resources being loaded that have not explicitly granted cross origin permissions.</div>
                </Form.Group>
                <Form.Group className='my-3'>
                    <Form.Label id='lblIncludeCrossOriginOpenerPolicy'>Include Cross Origin Opener Policy</Form.Label>
                    <Form.Select label='Include Cross Origin Opener Policy' aria-describedby='lblIncludeCrossOriginOpenerPolicy' onChange={handleCrossOriginOpenerPolicy} value={crossOriginOpenerPolicy} className={getSelectListStyle(crossOriginOpenerPolicy)}>
                        <option value='None' className='no-header-value'>Select a header value...</option>
                        <option value='UnsafeNone' className='header-value'>Unsafe None</option>
                        <option value='SameOrigin' className='header-value'>Same Origin</option>
                        <option value='SameOriginAllowPopups' className='header-value'>Same Origin Allow Popups</option>
                    </Form.Select>
                    <div className='form-text'>Configures the Cross-Origin-Opener-Policy header which is used to prevent sharing context with cross origin documents.</div>
                </Form.Group>
                <Form.Group className='my-3'>
                    <Form.Label id='lblIncludeCrossOriginResourcePolicy'>Include Cross Origin Resource Policy</Form.Label>
                    <Form.Select label='Include Cross Origin Resource Policy' aria-describedby='lblIncludeCrossOriginResourcePolicy' onChange={handleCrossOriginResourcePolicy} value={crossOriginResourcePolicy} className={getSelectListStyle(crossOriginResourcePolicy)}>
                        <option value='None' className='no-header-value'>Select a header value...</option>
                        <option value='SameSite' className='header-value'>Same Site</option>
                        <option value='SameOrigin' className='header-value'>Same Origin</option>
                        <option value='CrossOrigin' className='header-value'>Cross Origin</option>
                    </Form.Select>
                    <div className='form-text'>Configures the Cross-Origin-Resource-Policy header which is used to limit what resources can consume the current site.</div>
                </Form.Group>
                <Form.Group>
                    <Button type='submit' disabled={disableSaveButton} onClick={handleSaveSettings}>Save Changes</Button>
                </Form.Group>
            </Form>
        </Container>
    )
}

EditCrossOriginHeaders.propTypes = {
    crossOriginEmbedderPolicy: PropTypes.string.isRequired,
    crossOriginOpenerPolicy: PropTypes.string.isRequired,
    crossOriginResourcePolicy: PropTypes.string.isRequired,
    showToastNotificationEvent: PropTypes.func
};

export default EditCrossOriginHeaders
