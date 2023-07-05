import React, { useState, useEffect } from 'react';
import { Alert, Button, Container, Form } from 'react-bootstrap';

function EditCorsSettings(props) {

    const [isRequireCredentials, setIsRequireCredentials] = useState(false);
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
    const [disableSaveButton, setDisableSaveButton] = useState(true);

    const handleIsRequireCredentials = (event) => {
        setIsRequireCredentials(event.target.value);
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
                    <Form.Label id='lblRequireCredentials'>Require Credentials</Form.Label>
                    <Form.Select label='Require Credentials' aria-describedby='lblRequireCredentials' onChange={handleIsRequireCredentials} value={isRequireCredentials}>
                        <option value='false'>Disabled</option>
                        <option value='true'>Enabled</option>
                    </Form.Select>
                    <div className='form-text'>Some helpful hint.</div>
                </Form.Group>
                <Form.Group>
                    <Form.Label id='lblRequireCredentials'>Allowed Methods</Form.Label>
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
                </Form.Group>
                <Form.Group>
                    <Button type='submit' disabled={disableSaveButton} onClick={handleSaveSettings}>Save Changes</Button>
                </Form.Group>
            </Form>
        </Container>
    )
}

export default EditCorsSettings

/*
GET
The GET method requests a representation of the specified resource. Requests using GET should only retrieve data.

HEAD
The HEAD method asks for a response identical to a GET request, but without the response body.

POST
The POST method submits an entity to the specified resource, often causing a change in state or side effects on the server.

PUT
The PUT method replaces all current representations of the target resource with the request payload.

DELETE
The DELETE method deletes the specified resource.

CONNECT
The CONNECT method establishes a tunnel to the server identified by the target resource.

OPTIONS
The OPTIONS method describes the communication options for the target resource.

TRACE
The TRACE method performs a message loop-back test along the path to the target resource.

PATCH
The PATCH method applies partial modifications to a resource.
*/