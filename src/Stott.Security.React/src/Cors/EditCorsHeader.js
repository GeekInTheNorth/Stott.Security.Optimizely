import React from 'react';
import { Button, Form } from 'react-bootstrap';

function EditCorsHeader(props) {

    const handleDelete = () => props.handleDeleteHeader && props.handleDeleteHeader(props.headerName);
    const handleSave = (event) => props.handleUpdateHeader && props.handleUpdateHeader(props.headerIndex, event.target.value);

    return (
        <Form.Group className='input-group my-3'>
            <Form.Control type='text' placeholder='Header Name' value={props.headerName} onChange={handleSave} />
            <div class="input-group-append">
                <Button variant='danger' type="button" onClick={handleDelete}>&#x2717;</Button>
            </div>
        </Form.Group>
    )
}

export default EditCorsHeader