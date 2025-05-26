import { useState, useEffect } from 'react';
import { Form } from 'react-bootstrap';
import PropTypes from 'prop-types';

function FormUrl(props) {

    const [isValidClass, setValidClass] = useState('');
    const [currentUrl, setCurrentUrl] = useState('');
    const domainOnly = props.domainOnly ?? false;
    const required = props.required ?? false;

    function isEmptyOrSpaces(str){
        return str === null || str.match(/^ *$/) !== null;
    }

    const setValidationClass = () => {
        if (props.hasInvalidResponse) {
          setValidClass('is-invalid');
        }
    };

    const handleChange = (event) => {
        setCurrentUrl(event.target.value);
        if (isEmptyOrSpaces(event.target.value)){
            setValidClass('');
        }
        else if (validateUrl(event.target.value)){
            setValidClass('is-valid');
        }
        else {
            setValidClass('is-invalid');
        }
    }

    const handleBlur = (event) => {
        try {

            if (isEmptyOrSpaces(event.target.value)){
                setCurrentUrl('');
                setValidClass('');
                props.handleOnBlur && props.handleOnBlur(event.target.value);
                return;
            }

            let cleanUrl = '';

            if (required || !isEmptyOrSpaces(event.target.value)) {
                const parsedUrl = new URL(event.target.value);
                cleanUrl = domainOnly ? parsedUrl.origin : parsedUrl.href;
            }

            setCurrentUrl(cleanUrl);
            setValidClass('is-valid');
            props.handleOnBlur && props.handleOnBlur(event.target.value);
        }
        catch {
            setValidClass('is-invalid');
            props.handleOnBlur && props.handleOnBlur(event.target.value);
        }
    }

    const validateUrl = (urlString) => {
        try { 

            if (!required && isEmptyOrSpaces(urlString)) {
                return true;
            }

            const parsedUrl = new URL(urlString);

            return domainOnly ? parsedUrl.pathname === '/' && parsedUrl.search === '' && parsedUrl.hash === '' : true;
        }
        catch{ 
            return false; 
        }
    }

    useEffect(() => {
        setCurrentUrl(props.currentUrl);
        setValidationClass();
    }, [ props.hasInvalidResponse, props.currentUrl ])

    return (
        <Form.Control type='text' placeholder='https://www.example.com' value={currentUrl} onChange={handleChange} className={isValidClass} onBlur={handleBlur} />
    )
}

export default FormUrl

FormUrl.propTypes = {
    currentUrl: PropTypes.string,
    handleOnBlur: PropTypes.func,
    hasInvalidResponse: PropTypes.bool,
    domainOnly: PropTypes.bool,
    required: PropTypes.bool
};