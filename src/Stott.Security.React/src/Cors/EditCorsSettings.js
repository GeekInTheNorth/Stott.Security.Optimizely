import React, { useState, useEffect } from 'react';
import { Button, Container, Form } from 'react-bootstrap';
import HeaderComponent from './HeaderComponent';
import OriginComponent from './OriginComponent';

function EditCorsSettings(props) {

    const [isCorsEnabled, setIsCorsEnabled] = useState(false);
    const [isAllowCredentials, setIsAllowCredentials] = useState(false);
    const [isAllowAllMethods, setIsAllowAllMethods] = useState(true);
    const [isAllowGetMethods, setIsAllowGetMethods] = useState(false);
    const [isAllowHeadMethods, setIsAllowHeadMethods] = useState(false);
    const [isAllowPostMethods, setIsAllowPostMethods] = useState(false);
    const [isAllowPutMethods, setIsAllowPutMethods] = useState(false);
    const [isAllowPatchMethods, setIsAllowPatchMethods] = useState(false);
    const [isAllowDeleteMethods, setIsAllowDeleteMethods] = useState(false);
    const [isAllowConnectMethods, setIsAllowConnectMethods] = useState(false);
    const [isAllowOptionsMethods, setIsAllowOptionsMethods] = useState(false);
    const [isAllowTraceMethods, setIsAllowTraceMethods] = useState(false);
    const [allowHeaders, setAllowHeaders] = useState(["allow-test", "allow_test_two"]);
    const [exposeHeaders, setExposeHeaders] = useState(["expose-test", "expose_test_two"]);
    const [allowedOrigins, setAllowedOrigins] = useState(["www.test.com", "www.example.com"]);
    const [disableSaveButton, setDisableSaveButton] = useState(true);

    const handleIsCorsEnabledChange = (event) => {
        setIsCorsEnabled(event.target.checked);
        setDisableSaveButton(false);
    }

    const handleIsAllowCredentials = (event) => {
        setIsAllowCredentials(event.target.checked);
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

    const handleSaveAllowHeaders = (newHttpHeaders) => {
        setAllowHeaders(newHttpHeaders);
        setDisableSaveButton(false);
    }

    const handleSaveExposeHeaders = (newHttpHeaders) => {
        setExposeHeaders(newHttpHeaders);
        setDisableSaveButton(false);
    }

    const handleSaveAllowOrigins = (newAllowOrigins) => {
        setAllowedOrigins(newAllowOrigins);
        setDisableSaveButton(false);
    }

    const handleSaveSettings = (event) => {
        event.preventDefault();

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
                <HeaderComponent headerType='Allow' headers={allowHeaders} handleHeaderUpdate={handleSaveAllowHeaders}></HeaderComponent>
                <HeaderComponent headerType='Expose' headers={exposeHeaders} handleHeaderUpdate={handleSaveExposeHeaders}></HeaderComponent>
                <Form.Group className='my-3'>
                    <Form.Check type='switch' label='Allow Credentials.' checked={isAllowCredentials} onChange={handleIsAllowCredentials} />
                    <div className='form-text'>Configures the 'Access-Control-Allow-Credentials' header which instructs the browser whether it can share this request with the consuming website when the request's credential mode (Request.credentials) is set to 'include'.</div>
                </Form.Group>
                <Form.Group className='my-3'>
                    <Button type='submit' disabled={disableSaveButton} onClick={handleSaveSettings}>Save Changes</Button>
                </Form.Group>
            </Form>
        </Container>
    )
}

export default EditCorsSettings