import { useState, useEffect } from 'react';
import PropTypes from 'prop-types';
import { Input } from '@optiaxiom/react';

function FormUrl(props) {

    const [hasError, setHasError] = useState(false);
    const [currentUrl, setCurrentUrl] = useState('');
    const required = props.required ?? false;

    function isEmptyOrSpaces(str){
        return str === null || str.match(/^ *$/) !== null;
    }

    const setValidationState = () => {
        if (props.hasInvalidResponse) {
          setHasError(true);
        }
    };

    const handleChange = (event) => {
        setCurrentUrl(event.target.value);
        if (isEmptyOrSpaces(event.target.value)){
            setHasError(false);
        }
        else if (validateUrl(event.target.value)){
            setHasError(false);
        }
        else {
            setHasError(true);
        }
    }

    const handleBlur = (event) => {
        try {

            if (isEmptyOrSpaces(event.target.value)){
                setCurrentUrl('');
                setHasError(false);
                props.handleOnBlur && props.handleOnBlur('');
                return;
            }

            let cleanUrl = '';

            if (required || !isEmptyOrSpaces(event.target.value)) {
                const parsedUrl = new URL(event.target.value);
                cleanUrl = parsedUrl.href;
            }

            setCurrentUrl(cleanUrl);
            setHasError(false);
            props.handleOnBlur && props.handleOnBlur(cleanUrl);
        }
        catch(e) {
            setHasError(true);
            // Don't update parent with invalid URL
        }
    }

    const validateUrl = (urlString) => {
        try { 

            if (!required && isEmptyOrSpaces(urlString)) {
                return true;
            }

            const parsedUrl = new URL(urlString);

            return true;
        }
        catch(e){ 
            return false; 
        }
    }

    useEffect(() => {
        setCurrentUrl(props.currentUrl);
        setValidationState();
    }, [ props.hasInvalidResponse, props.currentUrl ])

    return (
        <Input type='text' placeholder='https://www.example.com' value={currentUrl} onChange={handleChange} error={hasError} onBlur={handleBlur} />
    )
}

FormUrl.propTypes = {
    currentUrl: PropTypes.string.isRequired,
    required: PropTypes.bool,
    hasInvalidResponse: PropTypes.bool,
    handleOnBlur: PropTypes.func
};

export default FormUrl
