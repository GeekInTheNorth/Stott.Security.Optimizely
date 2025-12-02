import { useState, useEffect } from 'react';
import { Button, Container, Form } from 'react-bootstrap';
import axios from 'axios';
import OriginComponent from './OriginComponent';
import ExposeHeaderComponent from './ExposeHeaderComponent';
import AllowHeaderComponent from './AllowHeaderComponent';

function EditCorsSettings(props) {

    const [isCorsEnabled, setIsCorsEnabled] = useState(false);
    const [isAllowCredentials, setIsAllowCredentials] = useState(false);
    const [isAllowAllMethods, setIsAllowAllMethods] = useState(false);
    const [isAllowGetMethods, setIsAllowGetMethods] = useState(false);
    const [isAllowHeadMethods, setIsAllowHeadMethods] = useState(false);
    const [isAllowPostMethods, setIsAllowPostMethods] = useState(false);
    const [isAllowPutMethods, setIsAllowPutMethods] = useState(false);
    const [isAllowPatchMethods, setIsAllowPatchMethods] = useState(false);
    const [isAllowDeleteMethods, setIsAllowDeleteMethods] = useState(false);
    const [isAllowConnectMethods, setIsAllowConnectMethods] = useState(false);
    const [isAllowOptionsMethods, setIsAllowOptionsMethods] = useState(false);
    const [isAllowTraceMethods, setIsAllowTraceMethods] = useState(false);
    const [allowHeaders, setAllowHeaders] = useState([]);
    const [exposeHeaders, setExposeHeaders] = useState([]);
    const [allowedOrigins, setAllowOrigins] = useState([]);
    const [maxAgeParameter, setMaxAgeParameter] = useState(1);
    const [disableSaveButton, setDisableSaveButton] = useState(true);
    const [hasAllowHeadersError, setHasAllowHeadersError] =  useState(false);
    const [allowHeadersErrorMessage, setAllowHeadersErrorMessage] =  useState('');
    const [hasExposeHeadersError, setHasExposeHeadersError] =  useState(false);
    const [exposeHeadersErrorMessage, setExposeHeadersErrorMessage] =  useState('');
    const [hasAllowOriginsError, setHasAllowOriginsError] =  useState(false);
    const [allowOriginsErrorMessage, setAllowOriginsErrorMessage] =  useState('');
    const [hasAllowCredentialsError, setHasAllowCredentialsError] =  useState(false);
    const [allowCredentialsErrorMessage, setAllowCredentialsErrorMessage] =  useState('');

    useEffect(() => {
        getCorsSettings()
    }, [])

    const getCorsSettings = async () => {
        await axios
            .get(import.meta.env.VITE_CORS_GET)
            .then((response) => {
                setIsCorsEnabled(response.data.isEnabled ?? false);
                if (response.data.allowMethods?.isAllowAllMethods ?? false) {
                    setIsAllowAllMethods(response.data.allowMethods.isAllowAllMethods);
                }
                else {
                    setIsAllowAllMethods(false);
                    setIsAllowGetMethods(response.data.allowMethods?.isAllowGetMethods ?? false);
                    setIsAllowHeadMethods(response.data.allowMethods?.isAllowHeadMethods ?? false);
                    setIsAllowPostMethods(response.data.allowMethods?.isAllowPostMethods ?? false);
                    setIsAllowPutMethods(response.data.allowMethods?.isAllowPutMethods ?? false);
                    setIsAllowPatchMethods(response.data.allowMethods?.isAllowPatchMethods ?? false);
                    setIsAllowDeleteMethods(response.data.allowMethods?.isAllowDeleteMethods ?? false);
                    setIsAllowConnectMethods(response.data.allowMethods?.isAllowConnectMethods ?? false);
                    setIsAllowOptionsMethods(response.data.allowMethods?.isAllowOptionsMethods ?? false);
                    setIsAllowTraceMethods(response.data.allowMethods?.isAllowTraceMethods ?? false);
                }
        
                setAllowOrigins(response.data.allowOrigins ?? []);
                setAllowHeaders(response.data.allowHeaders ?? []);
                setExposeHeaders(response.data.exposeHeaders ?? []);
                setIsAllowCredentials(response.data.allowCredentials ?? false);
                setMaxAgeParameter(response.data.maxAge ?? 1)
                setDisableSaveButton(true);
            }, () => {
                handleShowFailureToast("Error", "Failed to retrieve CORS configuration. Please try again soon.")
            });
    }

    const getDisplayDuration = () => 
    {
        if (maxAgeParameter < 60) {
            return '(' + maxAgeParameter + ' seconds)';
        }
        else if (maxAgeParameter < 3600) {
            var minutesAndSeconds = new Date(maxAgeParameter * 1000).toISOString().substring(14, 19);
            return '(' + minutesAndSeconds + ' minutes)';
        }
        else {
            var hoursMinutesAndSeconds = new Date(maxAgeParameter * 1000).toISOString().substring(11, 19)
            return '(' + hoursMinutesAndSeconds + ' hours)';
        }
    }

    const handleIsCorsEnabledChange = (event) => {
        setIsCorsEnabled(event.target.checked);
        setDisableSaveButton(false);
    }

    const handleIsAllowCredentials = (event) => {
        setIsAllowCredentials(event.target.checked);
        setHasAllowCredentialsError(false);
        setAllowCredentialsErrorMessage('');
        setDisableSaveButton(false);        
    }

    const handleIsAllowAllMethods = (event) => {
        setIsAllowAllMethods(event.target.checked);
        setIsAllowGetMethods(isAllowGetMethods && !event.target.checked);
        setIsAllowHeadMethods(isAllowHeadMethods && !event.target.checked);
        setIsAllowPostMethods(isAllowPostMethods && !event.target.checked);
        setIsAllowPutMethods(isAllowPutMethods && !event.target.checked);
        setIsAllowPatchMethods(isAllowPatchMethods && !event.target.checked);
        setIsAllowDeleteMethods(isAllowDeleteMethods && !event.target.checked);
        setIsAllowConnectMethods(isAllowConnectMethods && !event.target.checked);
        setIsAllowOptionsMethods(isAllowOptionsMethods && !event.target.checked);
        setIsAllowTraceMethods(isAllowTraceMethods && !event.target.checked);
        setDisableSaveButton(false);
    }

    const handleIsAllowGetMethods = (event) => {
        if (event.target.checked) { setIsAllowAllMethods(false); }

        setIsAllowGetMethods(event.target.checked);
        setDisableSaveButton(false);
    }

    const handleIsAllowHeadMethods = (event) => {
        if (event.target.checked) { setIsAllowAllMethods(false); }

        setIsAllowHeadMethods(event.target.checked);
        setDisableSaveButton(false);
    }

    const handleIsAllowPostMethods = (event) => {
        if (event.target.checked) { setIsAllowAllMethods(false); }

        setIsAllowPostMethods(event.target.checked);
        setDisableSaveButton(false);
    }

    const handleIsAllowPutMethods = (event) => {
        if (event.target.checked) { setIsAllowAllMethods(false); }

        setIsAllowPutMethods(event.target.checked);
        setDisableSaveButton(false);
    }

    const handleIsAllowPatchMethods = (event) => {
        if (event.target.checked) { setIsAllowAllMethods(false); }

        setIsAllowPatchMethods(event.target.checked);
        setDisableSaveButton(false);
    }

    const handleIsAllowDeleteMethods = (event) => {
        if (event.target.checked) { setIsAllowAllMethods(false); }

        setIsAllowDeleteMethods(event.target.checked);
        setDisableSaveButton(false);
    }

    const handleIsAllowConnectMethods = (event) => {
        if (event.target.checked) { setIsAllowAllMethods(false); }

        setIsAllowConnectMethods(event.target.checked);
        setDisableSaveButton(false);
    }

    const handleIsAllowOptionsMethods = (event) => {
        if (event.target.checked) { setIsAllowAllMethods(false); }

        setIsAllowOptionsMethods(event.target.checked);
        setDisableSaveButton(false);
    }

    const handleIsAllowTraceMethods = (event) => {
        if (event.target.checked) { setIsAllowAllMethods(false); }

        setIsAllowTraceMethods(event.target.checked);
        setDisableSaveButton(false);
    }

    const handleSetMaxAgeParameter = (event) => 
    { 
        setMaxAgeParameter(event.target.value);
        setDisableSaveButton(false); 
    };

    const handleSaveAllowHeaders = (newHttpHeaders) => {
        setAllowHeaders(newHttpHeaders);
        setHasAllowHeadersError(false);
        setAllowHeadersErrorMessage('');
        setDisableSaveButton(false);
    }

    const handleSaveExposeHeaders = (newHttpHeaders) => {
        setExposeHeaders(newHttpHeaders);
        setHasExposeHeadersError(false);
        setExposeHeadersErrorMessage('');
        setDisableSaveButton(false);
    }

    const handleSaveAllowOrigins = (newAllowOrigins) => {
        setAllowOrigins(newAllowOrigins);
        setHasAllowOriginsError(false);
        setAllowOriginsErrorMessage('');
        setDisableSaveButton(false);
    }

    const handleSaveSettings = async (event) => {
        event.preventDefault();

        let payload = {
            isEnabled: isCorsEnabled,
            allowOrigins: allowedOrigins,
            allowHeaders: allowHeaders,
            exposeHeaders: exposeHeaders,
            allowMethods: {
                isAllowGetMethods: isAllowAllMethods || isAllowGetMethods,
                isAllowHeadMethods: isAllowAllMethods || isAllowHeadMethods,
                isAllowPostMethods: isAllowAllMethods || isAllowPostMethods,
                isAllowPutMethods: isAllowAllMethods || isAllowPutMethods,
                isAllowPatchMethods: isAllowAllMethods || isAllowPatchMethods,
                isAllowDeleteMethods: isAllowAllMethods || isAllowDeleteMethods,
                isAllowConnectMethods: isAllowAllMethods || isAllowConnectMethods,
                isAllowOptionsMethods: isAllowAllMethods || isAllowOptionsMethods,
                isAllowTraceMethods: isAllowAllMethods || isAllowTraceMethods
            },
            allowCredentials: isAllowCredentials,
            maxAge: maxAgeParameter
        };

        await axios
            .post(import.meta.env.VITE_CORS_SAVE, payload)
            .then(() => {
                handleShowSuccessToast("CORS", "Changes to CORS settings have been successfully saved.");
            },
            (error) => {
                if (error.response && error.response.status === 400) {
                    var validationResult = error.response.data;
                    validationResult.errors.forEach(function (error) {
                        if (error.propertyName === 'AllowHeaders') {
                            setHasAllowHeadersError(true);
                            setAllowHeadersErrorMessage(error.errorMessage);
                        } else if (error.propertyName === 'ExposeHeaders') {
                            setHasExposeHeadersError(true);
                            setExposeHeadersErrorMessage(error.errorMessage);
                        } else if (error.propertyName === 'AllowOrigins') {
                            setHasAllowOriginsError(true);
                            setAllowOriginsErrorMessage(error.errorMessage);
                        } else if (error.propertyName === 'AllowCredentials') {
                            setHasAllowCredentialsError(true);
                            setAllowCredentialsErrorMessage(error.errorMessage);
                        }
                    });
                    handleShowFailureToast("CORS", "Changes to Cross-origin Resource Sharing settings have not been saved due to invalid values.");
                }
                else {
                    handleShowFailureToast("Error", "Failed to save changes to the Cross-origin Resource Sharing settings.");
                }
            });

        setDisableSaveButton(true);
    }

    const handleShowSuccessToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(true, title, description);
    const handleShowFailureToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(false, title, description);

    return (
        <Container fluid='md'>
            <Form>
                <Form.Group className='my-3'>
                    <Form.Check type='switch' label='Enable Cross-Origin Resource Sharing (CORS)' checked={isCorsEnabled} onChange={handleIsCorsEnabledChange} />
                    <div className='form-text'>Enabling the Cross-Origin Resource Sharing will apply the CORS headers to all requests from both content routes and CMS backend routes.</div>
                </Form.Group>
                <OriginComponent origins={allowedOrigins} handleOriginUpdate={handleSaveAllowOrigins}></OriginComponent>
                {hasAllowOriginsError ? <div className="invalid-feedback d-block">{allowOriginsErrorMessage}</div> : ""}
                <Form.Group className='my-3'>
                    <Form.Label id='lblRequireCredentials'>Allowed HTTP Methods:</Form.Label>
                    <Form.Check type='switch' label='Allow the use of ALL methods.' checked={isAllowAllMethods} onChange={handleIsAllowAllMethods} />
                    <Form.Check type='switch' label='Allow the use of GET methods.' checked={isAllowGetMethods} onChange={handleIsAllowGetMethods} />
                    <Form.Check type='switch' label='Allow the use of HEAD methods.' checked={isAllowHeadMethods} onChange={handleIsAllowHeadMethods} />
                    <Form.Check type='switch' label='Allow the use of POST methods.' checked={isAllowPostMethods} onChange={handleIsAllowPostMethods} />
                    <Form.Check type='switch' label='Allow the use of PUT methods.' checked={isAllowPutMethods} onChange={handleIsAllowPutMethods} />
                    <Form.Check type='switch' label='Allow the use of PATCH methods.' checked={isAllowPatchMethods} onChange={handleIsAllowPatchMethods} />
                    <Form.Check type='switch' label='Allow the use of DELETE methods.' checked={isAllowDeleteMethods} onChange={handleIsAllowDeleteMethods} />
                    <Form.Check type='switch' label='Allow the use of CONNECT methods.' checked={isAllowConnectMethods} onChange={handleIsAllowConnectMethods} />
                    <Form.Check type='switch' label='Allow the use of OPTIONS methods.' checked={isAllowOptionsMethods} onChange={handleIsAllowOptionsMethods} />
                    <Form.Check type='switch' label='Allow the use of TRACE methods.' checked={isAllowTraceMethods} onChange={handleIsAllowTraceMethods} />
                    <div className='form-text'>Configures the 'Access-Control-Allow-Methods' header which instructs the browser on what HTTP Methods may be used when making a request to this webserver. If there are no method options selected, then the default behaviour will be to allow ALL HTTP methods.</div>
                </Form.Group>
                <AllowHeaderComponent headers={allowHeaders} handleHeaderUpdate={handleSaveAllowHeaders}></AllowHeaderComponent>
                {hasAllowHeadersError ? <div className="invalid-feedback d-block">{allowHeadersErrorMessage}</div> : ""}
                <ExposeHeaderComponent headers={exposeHeaders} handleHeaderUpdate={handleSaveExposeHeaders}></ExposeHeaderComponent>
                {hasExposeHeadersError ? <div className="invalid-feedback d-block">{exposeHeadersErrorMessage}</div> : ""}
                <Form.Group className='my-3'>
                    <Form.Check type='switch' label='Allow Credentials.' checked={isAllowCredentials} onChange={handleIsAllowCredentials} />
                    <div className='form-text'>Configures the 'Access-Control-Allow-Credentials' header which instructs the browser whether it can share this request with the consuming website when the request's credential mode (Request.credentials) is set to 'include'.</div>
                    {hasAllowCredentialsError ? <div className="invalid-feedback d-block">{allowCredentialsErrorMessage}</div> : ""}
                </Form.Group>
                <Form.Group className='my-3'>
                    <Form.Label id='lblMaxAge'>Maximum Age {getDisplayDuration()}</Form.Label>
                    <Form.Range min='1' max='7200' step='1' aria-describedby='lblMaxAge' value={maxAgeParameter} onChange={handleSetMaxAgeParameter}></Form.Range>
                    <div className='form-text'>Configures the 'Access-Control-Max-Age' header which instructs the browser on how long it may cache a pre-flight request.</div>
                </Form.Group>
                <Form.Group className='my-3'>
                    <Button type='submit' disabled={disableSaveButton} onClick={handleSaveSettings}>Save Changes</Button>
                </Form.Group>
            </Form>
        </Container>
    )
}

export default EditCorsSettings
