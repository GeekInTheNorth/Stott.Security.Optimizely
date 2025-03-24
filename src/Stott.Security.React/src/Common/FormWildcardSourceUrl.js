import React, { useState } from 'react';
import { Button, Form } from 'react-bootstrap';

function FormWildcardSourceUrl(props)
{
    const [isValidClass, setValidClass] = useState('');
    const [currentSourceUrl, setCurrentSourceUrl] = useState(props.sourceUrl);

    const handleDelete = () => props.handleDeleteSource && props.sourceId && props.handleDeleteSource(props.sourceId);

    const handleChange = (event) => 
    {
        setCurrentSourceUrl(event.target.value);
        if (isValidUrl(event.target.value)){
            setValidClass('is-valid');
        }
        else {
            setValidClass('is-invalid');
        }
    };

    const handleBlur = (event) => {
        try {
            const wildcardRegex = new RegExp(/^(http|ws)[s]{0,1}:\/\/(\*\.){0,1}([a-z0-9\-]{1,}\.){1,}([a-z0-9\-]{1,}\/{0,1}){1}$/i);
            if (wildcardRegex.test(event.target.value)) {
                setValidClass('is-valid');

                var cleanedUrl = event.target.value.match(wildcardRegex)[0].toLowerCase(); // Get the cleaned value

                setCurrentSourceUrl(event.target.value); // Set the cleaned value
                props.handleUpdateSourceUrl && props.handleUpdateSourceUrl(props.sourceId, cleanedUrl);
            } else {
                setValidClass('is-invalid');
                props.handleUpdateSourceUrl && props.handleUpdateSourceUrl(props.sourceId, event.target.value);
            }
        }
        catch(e) {
            setValidClass('is-invalid');
            props.handleUpdateSourceUrl && props.handleUpdateSourceUrl(props.sourceId, event.target.value);
        }
    };

    const isValidUrl = (urlString) => {
        try { 
            const wildcardRegex = new RegExp(/^(http|ws)[s]{0,1}:\/\/(\*\.){0,1}([a-z0-9\-]{1,}\.){1,}([a-z0-9\-]{1,}\/{0,1}){1}$/i);
            return wildcardRegex.test(urlString);
        }
        catch(e){ 
            return false; 
        }
    };

    return(
        <Form.Group className='input-group mb-3'>
            <Form.Control type='text' placeholder='Enter a URL, e.g. https://www.example.com' value={currentSourceUrl} onChange={handleChange} className={isValidClass} onBlur={handleBlur} />
            <div className='input-group-append'>
                <Button variant='danger' type='button' onClick={handleDelete}>&#x2717;</Button>
            </div>
        </Form.Group>
    )
}

export default FormWildcardSourceUrl