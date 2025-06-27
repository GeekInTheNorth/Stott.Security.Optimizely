import React, { useState, useEffect } from 'react';
import { Button, Container, Form, Spinner } from 'react-bootstrap';
import axios from 'axios';
import OriginComponent from './OriginComponent';
import ExposeHeaderComponent from './ExposeHeaderComponent';
import AllowHeaderComponent from './AllowHeaderComponent';
import PropTypes from 'prop-types';

function EditCorsSettings(props) {
    
    const [isCorsEnabled, setIsCorsEnabled] = useState(false);
    const [isAllowCredentials, setIsAllowCredentials] = useState(false);
    const [isAllowAllMethods, setIsAllowAllMethods] = useState(false);
    const [allowedMethods, setAllowedMethods] = useState({ Get: false, Head: false, Post: false, Put: false, Patch: false, Delete: false, Connect: false, Options: false, Trace: false });
    const [allowHeaders, setAllowHeaders] = useState([]);
    const [exposeHeaders, setExposeHeaders] = useState([]);
    const [allowedOrigins, setAllowOrigins] = useState([]);
    const [maxAgeParameter, setMaxAgeParameter] = useState(1);
    const [disableSaveButton, setDisableSaveButton] = useState(true);
    const [errors, setErrors] = useState({ allowHeaders: '', exposeHeaders: '', allowOrigins: '', allowCredentials: '' });
    const [loading, setLoading] = useState(false);

    const httpMethods = [
        { key: 'Get', label: 'GET' },
        { key: 'Head', label: 'HEAD' },
        { key: 'Post', label: 'POST' },
        { key: 'Put', label: 'PUT' },
        { key: 'Patch', label: 'PATCH' },
        { key: 'Delete', label: 'DELETE' },
        { key: 'Connect', label: 'CONNECT' },
        { key: 'Options', label: 'OPTIONS' },
        { key: 'Trace', label: 'TRACE' }
    ];

    useEffect(() => {
        getCorsSettings()
    }, [])

    const getCorsSettings = async () => {
        setLoading(true);
        try {
            const response = await axios.get(import.meta.env.VITE_APP_CORS_GET);
            setIsCorsEnabled(response.data.isEnabled ?? false);
            if (response.data.allowMethods?.isAllowAllMethods ?? false) {
                setIsAllowAllMethods(response.data.allowMethods.isAllowAllMethods);
            } else {
                setIsAllowAllMethods(false);
            }
            setAllowedMethods({
                Get: response.data.allowMethods?.isAllowGetMethods ?? false,
                Head: response.data.allowMethods?.isAllowHeadMethods ?? false,
                Post: response.data.allowMethods?.isAllowPostMethods ?? false,
                Put: response.data.allowMethods?.isAllowPutMethods ?? false,
                Patch: response.data.allowMethods?.isAllowPatchMethods ?? false,
                Delete: response.data.allowMethods?.isAllowDeleteMethods ?? false,
                Connect: response.data.allowMethods?.isAllowConnectMethods ?? false,
                Options: response.data.allowMethods?.isAllowOptionsMethods ?? false,
                Trace: response.data.allowMethods?.isAllowTraceMethods ?? false
            });
            setAllowOrigins(response.data.allowOrigins ?? []);
            setAllowHeaders(response.data.allowHeaders ?? []);
            setExposeHeaders(response.data.exposeHeaders ?? []);
            setIsAllowCredentials(response.data.allowCredentials ?? false);
            setMaxAgeParameter(response.data.maxAge ?? 1);
            setDisableSaveButton(true);
        } catch {
            handleShowFailureToast("Error", "Failed to retrieve CORS configuration. Please try again soon.");
        } finally {
            setLoading(false);
        }
    }

    const getDisplayDuration = () => {
        const seconds = Number(maxAgeParameter);
        if (seconds < 60) {
            return `(${seconds} second${seconds !== 1 ? 's' : ''})`;
        }
        if (seconds < 3600) {
            const mins = Math.floor(seconds / 60);
            const secs = seconds % 60;
            return `(${mins} minute${mins !== 1 ? 's' : ''}${secs ? `, ${secs} second${secs !== 1 ? 's' : ''}` : ''})`;
        }
        const hrs = Math.floor(seconds / 3600);
        const mins = Math.floor((seconds % 3600) / 60);
        const secs = seconds % 60;
        let result = `(${hrs} hour${hrs !== 1 ? 's' : ''}`;
        if (mins) result += `, ${mins} minute${mins !== 1 ? 's' : ''}`;
        if (secs) result += `, ${secs} second${secs !== 1 ? 's' : ''}`;
        result += ')';
        return result;
    };

    const handleIsCorsEnabledChange = (event) => {
        setIsCorsEnabled(event.target.checked);
        setDisableSaveButton(false);
    }

    const handleIsAllowCredentials = (event) => {
        setIsAllowCredentials(event.target.checked);
        setErrors(prev => ({ ...prev, allowCredentials: '' }));
        setDisableSaveButton(false);        
    }

    const handleIsAllowAllMethods = (event) => {
        const newValue = event.target.checked;
        setIsAllowAllMethods(newValue);
        setAllowedMethods(Object.fromEntries(httpMethods.map(method => [method.key, newValue])));
        setDisableSaveButton(false);
    }

    const handleMethodChange = (methodKey) => (event) => {
        const updated = { ...allowedMethods, [methodKey]: event.target.checked };
        setAllowedMethods(updated);
        setIsAllowAllMethods(Object.values(updated).every(Boolean));
        setDisableSaveButton(false);
    };

    const handleSetMaxAgeParameter = (event) => 
    { 
        setMaxAgeParameter(event.target.value);
        setDisableSaveButton(false); 
    };

    const handleSaveAllowHeaders = (newHttpHeaders) => {
        setAllowHeaders(newHttpHeaders);
        setErrors(prev => ({ ...prev, allowHeaders: '' }));
        setDisableSaveButton(false);
    }

    const handleSaveExposeHeaders = (newHttpHeaders) => {
        setExposeHeaders(newHttpHeaders);
        setErrors(prev => ({ ...prev, exposeHeaders: '' }));
        setDisableSaveButton(false);
    }

    const handleSaveAllowOrigins = (newAllowOrigins) => {
        setAllowOrigins(newAllowOrigins);
        setErrors(prev => ({ ...prev, allowOrigins: '' }));
        setDisableSaveButton(false);
    }

    const handleSaveSettings = async (event) => {
        event.preventDefault();
        setLoading(true);
        try {
            let payload = {
                isEnabled: isCorsEnabled,
                allowOrigins: allowedOrigins,
                allowHeaders: allowHeaders,
                exposeHeaders: exposeHeaders,
                allowMethods: {
                    isAllowGetMethods: isAllowAllMethods || allowedMethods.Get,
                    isAllowHeadMethods: isAllowAllMethods || allowedMethods.Head,
                    isAllowPostMethods: isAllowAllMethods || allowedMethods.Post,
                    isAllowPutMethods: isAllowAllMethods || allowedMethods.Put,
                    isAllowPatchMethods: isAllowAllMethods || allowedMethods.Patch,
                    isAllowDeleteMethods: isAllowAllMethods || allowedMethods.Delete,
                    isAllowConnectMethods: isAllowAllMethods || allowedMethods.Connect,
                    isAllowOptionsMethods: isAllowAllMethods || allowedMethods.Options,
                    isAllowTraceMethods: isAllowAllMethods || allowedMethods.Trace
                },
                allowCredentials: isAllowCredentials,
                maxAge: maxAgeParameter
            };
            await axios.post(import.meta.env.VITE_APP_CORS_SAVE, payload);
            handleShowSuccessToast("CORS", "Changes to CORS settings have been successfully saved.");
            setDisableSaveButton(true);
        } catch (error) {
            if (error.response && error.response.status === 400) {
                var validationResult = error.response.data;
                const newErrors = { allowHeaders: '', exposeHeaders: '', allowOrigins: '', allowCredentials: '' };
                validationResult.errors.forEach(function (error) {
                    if (error.propertyName === 'AllowHeaders') {
                        newErrors.allowHeaders = error.errorMessage;
                    } else if (error.propertyName === 'ExposeHeaders') {
                        newErrors.exposeHeaders = error.errorMessage;
                    } else if (error.propertyName === 'AllowOrigins') {
                        newErrors.allowOrigins = error.errorMessage;
                    } else if (error.propertyName === 'AllowCredentials') {
                        newErrors.allowCredentials = error.errorMessage;
                    }
                });
                setErrors(newErrors);
                handleShowFailureToast("CORS", "Changes to Cross-origin Resource Sharing settings have not been saved due to invalid values.");
            } else {
                handleShowFailureToast("Error", "Failed to save changes to the Cross-origin Resource Sharing settings.");
            }
        } finally {
            setLoading(false);
        }
    }

    const handleShowSuccessToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(true, title, description);
    const handleShowFailureToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(false, title, description);

    return (
        <Container fluid='md'>
            {loading && (
                <div className="d-flex justify-content-center align-items-center my-3">
                    <Spinner animation="border" role="status" aria-hidden="true" />
                    <span className="ms-2">Loading...</span>
                </div>
            )}
            <Form onSubmit={handleSaveSettings} aria-busy={loading}>
                <Form.Group className='my-3'>
                    <Form.Check
                        id="enable-cors-switch"
                        type='switch'
                        label='Enable Cross-Origin Resource Sharing (CORS)'
                        checked={isCorsEnabled}
                        onChange={handleIsCorsEnabledChange}
                    />
                    <div className='form-text'>Enabling the Cross-Origin Resource Sharing will apply the CORS headers to all requests from both content routes and CMS backend routes.</div>
                </Form.Group>
                <OriginComponent origins={allowedOrigins} handleOriginUpdate={handleSaveAllowOrigins}></OriginComponent>
                {errors.allowOrigins && <div className="invalid-feedback d-block" aria-live="polite">{errors.allowOrigins}</div>}
                <Form.Group className='my-3'>
                    <fieldset>
                        <legend id="http-methods-legend">Allowed HTTP Methods</legend>
                        <Form.Check
                            id="allow-method-all"
                            type='switch'
                            label='Allow the use of ALL methods.'
                            checked={isAllowAllMethods}
                            onChange={handleIsAllowAllMethods}
                        />
                        {httpMethods.map(method => (
                            <Form.Check
                                id={`allow-method-${method.key}`}
                                type='switch'
                                label={`Allow the use of ${method.label} methods.`}
                                checked={allowedMethods[method.key]}
                                onChange={handleMethodChange(method.key)}
                                key={method.key}
                            />
                        ))}
                        <div className='form-text' id="http-methods-help">Configures the 'Access-Control-Allow-Methods' header which instructs the browser on what HTTP Methods may be used when making a request to this webserver. If there are no method options selected, then the default behaviour will be to allow ALL HTTP methods.</div>
                    </fieldset>
                </Form.Group>
                <AllowHeaderComponent headers={allowHeaders} handleHeaderUpdate={handleSaveAllowHeaders}></AllowHeaderComponent>
                {errors.allowHeaders && <div className="invalid-feedback d-block" aria-live="polite">{errors.allowHeaders}</div>}
                <ExposeHeaderComponent headers={exposeHeaders} handleHeaderUpdate={handleSaveExposeHeaders}></ExposeHeaderComponent>
                {errors.exposeHeaders && <div className="invalid-feedback d-block" aria-live="polite">{errors.exposeHeaders}</div>}
                <Form.Group className='my-3'>
                    <Form.Check
                        id="allow-credentials-switch"
                        type='switch'
                        label='Allow Credentials.'
                        checked={isAllowCredentials}
                        onChange={handleIsAllowCredentials}
                    />
                    <div className='form-text'>Configures the 'Access-Control-Allow-Credentials' header which instructs the browser whether it can share this request with the consuming website when the request's credential mode (Request.credentials) is set to 'include'.</div>
                    {errors.allowCredentials && <div className="invalid-feedback d-block" aria-live="polite">{errors.allowCredentials}</div>}
                </Form.Group>
                <Form.Group className='my-3'>
                    <Form.Label id='lblMaxAge' htmlFor='max-age-range'>Maximum Age {getDisplayDuration()}</Form.Label>
                    <Form.Range id='max-age-range' min='1' max='7200' step='1' aria-describedby='lblMaxAge' value={maxAgeParameter} onChange={handleSetMaxAgeParameter}></Form.Range>
                    <div className='form-text'>Configures the 'Access-Control-Max-Age' header which instructs the browser on how long it may cache a pre-flight request.</div>
                </Form.Group>
                <Form.Group className='my-3'>
                    <Button type='submit' disabled={disableSaveButton || loading}>
                        {loading ? (<><Spinner as="span" animation="border" size="sm" role="status" aria-hidden="true" /> Saving...</>) : 'Save Changes'}
                    </Button>
                </Form.Group>
            </Form>
        </Container>
    )
}

EditCorsSettings.propTypes = {
    showToastNotificationEvent: PropTypes.func.isRequired
};

export default EditCorsSettings; 