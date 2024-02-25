import React, { useState } from 'react';
import { Form } from 'react-bootstrap';

function FormUrl(props) {

    const [isValidClass, setValidClass] = useState('');
    const [currentUrl, setCurrentUrl] = useState(props.currentUrl);
    const domainOnly = props.domainOnly ?? false;
    const required = props.required ?? false;

    function isEmptyOrSpaces(str){
        return str === null || str.match(/^ *$/) !== null;
    }

    const handleChange = (event) => {
        setCurrentUrl(event.target.value);
        if (isValidUrl(event.target.value)){
            setValidClass('is-valid');
        }
        else {
            setValidClass('is-invalid');
        }
    }

    const handleBlur = (event) => {
        try {

            let cleanUrl = '';

            if (required || !isEmptyOrSpaces(event.target.value)) {
                const parsedUrl = new URL(event.target.value);
                cleanUrl = domainOnly ? parsedUrl.origin : parsedUrl.href;
            }

            setCurrentUrl(cleanUrl);
            setValidClass('is-valid');
            props.handleOnBlur && props.handleOnBlur(event.target.value);
        }
        catch(e) {
            setValidClass('is-invalid');
            props.handleOnBlur && props.handleOnBlur(event.target.value);
        }
    }

    const isValidUrl = (urlString) => {
        try { 

            if (!required && isEmptyOrSpaces(urlString)) {
                return true;
            }

            const parsedUrl = new URL(urlString);

            return domainOnly ? parsedUrl.pathname === '/' && parsedUrl.search === '' && parsedUrl.hash === '' : true;
        }
        catch(e){ 
            return false; 
        }
    }

    return (
        <Form.Control type='text' placeholder='https://www.example.com' value={currentUrl} onChange={handleChange} className={isValidClass} onBlur={handleBlur} />
    )
}

export default FormUrl