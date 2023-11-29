import React, { useState } from 'react';
import { Button, Form } from 'react-bootstrap';

function EditCorsOrigin(props) {

    const [isValidClass, setValidClass] = useState("");
    const [currentOrigin, setCurrentOrigin] = useState(props.originUrl);

    const handleDelete = () => props.handleDeleteOrigin && props.originId && props.handleDeleteOrigin(props.originId);

    const handleChange = (event) => {
        setCurrentOrigin(event.target.value);
        if (isValidUrl(event.target.value)){
            setValidClass("is-valid");
        }
        else {
            setValidClass("is-invalid");
        }
    }

    const handleBlur = (event) => {
        try {
            const parsedUrl = new URL(event.target.value);
            setCurrentOrigin(parsedUrl.origin);
            setValidClass("is-valid");
            props.handleUpdateOrigin && props.handleUpdateOrigin(props.originId, parsedUrl.origin);
        }
        catch(e) {
            setValidClass("is-invalid");
            props.handleUpdateOrigin && props.handleUpdateOrigin(props.originId, event.target.value);
        }
    }

    const isValidUrl = (urlString) => {
        try { 
            const parsedUrl = new URL(urlString);
            return parsedUrl.pathname === '/' && parsedUrl.search === '' && parsedUrl.hash === '';
        }
        catch(e){ 
            return false; 
        }
    }

    return (
        <Form.Group className='input-group mb-3'>
            <Form.Control type='text' placeholder='Origin URL' value={currentOrigin} onChange={handleChange} className={isValidClass} onBlur={handleBlur} />
            <div className="input-group-append">
                <Button variant='danger' type="button" onClick={handleDelete}>&#x2717;</Button>
            </div>

        </Form.Group>
    )
}

export default EditCorsOrigin