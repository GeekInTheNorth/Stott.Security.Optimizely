import React, { useState } from 'react';
import { Button, Form } from 'react-bootstrap';

function EditCorsHeader(props) {

    const [isValidClass, setValidClass] = useState("");
    const [currentHeaderName, setCurrentHeaderName] = useState(props.headerName);

    const handleDelete = () => props.handleDeleteHeader && props.handleDeleteHeader(props.headerName);

    const handleChange = (event) => {
        setCurrentHeaderName(event.target.value);
        if (isValidHeaderName(event.target.value)) {
            setValidClass("is-valid");
        }
        else {
            setValidClass("is-invalid");
        }
    }

    const handleBlur = (event) => {
        setCurrentHeaderName(event.target.value);
        if (isValidHeaderName(event.target.value)) {
            setValidClass("is-valid");
            props.handleUpdateHeader && props.handleUpdateHeader(props.headerIndex, event.target.value);
        }
        else {
            setValidClass("is-invalid");
        }
    }

    const isValidHeaderName = (headerName) => {
        return /^[a-zA-Z0-9\-_]+$/.test(headerName);
    }

    return (
        <Form.Group className='input-group my-3'>
            <Form.Control type='text' placeholder='Header Name' value={currentHeaderName} onChange={handleChange} className={isValidClass} onBlur={handleBlur} />
            <div class="input-group-append">
                <Button variant='danger' type="button" onClick={handleDelete}>&#x2717;</Button>
            </div>
        </Form.Group>
    )
}

export default EditCorsHeader