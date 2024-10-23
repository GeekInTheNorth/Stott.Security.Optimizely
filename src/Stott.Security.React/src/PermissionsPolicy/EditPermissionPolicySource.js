import React, { useState } from 'react';
import { Form } from 'react-bootstrap';

function EditPermissionPolicySource(props)
{
    const [currentOrigin, setCurrentOrigin] = useState(props.sourceUrl);

    return(
        <Form.Control type='text' placeholder='Source URL' value={currentOrigin} />
    )
}

export default EditPermissionPolicySource