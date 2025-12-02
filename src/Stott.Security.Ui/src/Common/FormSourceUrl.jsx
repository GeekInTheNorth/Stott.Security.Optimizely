import { useState } from 'react';
import { Button, Form } from 'react-bootstrap';

function FormSourceUrl(props)
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
            const parsedUrl = new URL(event.target.value);
            setCurrentSourceUrl(parsedUrl.origin);
            setValidClass('is-valid');
            props.handleUpdateSourceUrl && props.handleUpdateSourceUrl(props.sourceId, parsedUrl.origin);
        }
        catch(e) {
            setValidClass('is-invalid');
            props.handleUpdateSourceUrl && props.handleUpdateSourceUrl(props.sourceId, event.target.value);
        }
    };

    const isValidUrl = (urlString) => {
        try { 
            const parsedUrl = new URL(urlString);
            return parsedUrl.pathname === '/' && parsedUrl.search === '' && parsedUrl.hash === '';
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

export default FormSourceUrl
