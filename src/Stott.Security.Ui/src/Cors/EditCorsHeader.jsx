import { useState } from 'react';
import { Button, Form } from 'react-bootstrap';

function EditCorsHeader(props) {

    const [isValidClass, setValidClass] = useState("");
    const [currentHeaderName, setCurrentHeaderName] = useState(props.headerName);

    const handleDelete = () => props.handleDeleteHeader && props.headerId && props.handleDeleteHeader(props.headerId);

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
            props.handleUpdateHeader && props.handleUpdateHeader(props.headerId, event.target.value);
        }
        else {
            setValidClass("is-invalid");
            props.handleUpdateHeader && props.handleUpdateHeader(props.headerId, event.target.value);
        }
    }

    const isValidHeaderName = (headerName) => {
        return /^[a-zA-Z0-9\-_]+$/.test(headerName);
    }

    return (
        <Form.Group className='input-group mb-3'>
            <Form.Control type='text' placeholder='Header Name' value={currentHeaderName} onChange={handleChange} className={isValidClass} onBlur={handleBlur} />
            <div className="input-group-append">
                <Button variant='danger' type="button" onClick={handleDelete}>&#x2717;</Button>
            </div>
        </Form.Group>
    )
}

export default EditCorsHeader
