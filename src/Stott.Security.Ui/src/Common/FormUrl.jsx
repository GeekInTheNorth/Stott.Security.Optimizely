import { useState, useEffect } from 'react';
import PropTypes from 'prop-types';
import { Form } from 'react-bootstrap';

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
        catch(e) {
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
        catch(e){ 
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

FormUrl.propTypes = {
    currentUrl: PropTypes.string.isRequired,
    domainOnly: PropTypes.bool,
    required: PropTypes.bool,
    hasInvalidResponse: PropTypes.bool,
    handleOnBlur: PropTypes.func
};

export default FormUrl
