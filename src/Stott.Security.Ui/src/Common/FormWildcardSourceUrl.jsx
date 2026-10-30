import { useState } from 'react';
import PropTypes from 'prop-types';
import { Button, Group, Input } from '@optiaxiom/react';
import { IconXmark } from '@optiaxiom/icons';

function FormWildcardSourceUrl(props)
{
    const [hasError, setHasError] = useState(false);
    const [currentSourceUrl, setCurrentSourceUrl] = useState(props.sourceUrl);

    const handleDelete = () => props.handleDeleteSource && props.sourceId && props.handleDeleteSource(props.sourceId);

    const handleChange = (event) => 
    {
        setCurrentSourceUrl(event.target.value);
        if (isValidUrl(event.target.value)){
            setHasError(false);
        }
        else {
            setHasError(true);
        }
    };

    const handleBlur = (event) => {
        try {
            if (getRegEx().test(event.target.value)) {
                setHasError(false);

                var cleanedUrl = event.target.value.match(getRegEx())[0].toLowerCase(); // Get the cleaned value

                setCurrentSourceUrl(event.target.value); // Set the cleaned value
                props.handleUpdateSourceUrl && props.handleUpdateSourceUrl(props.sourceId, cleanedUrl);
            } else {
                setHasError(true);
                props.handleUpdateSourceUrl && props.handleUpdateSourceUrl(props.sourceId, event.target.value);
            }
        }
        catch {
            setHasError(true);
            props.handleUpdateSourceUrl && props.handleUpdateSourceUrl(props.sourceId, event.target.value);
        }
    };

    const isValidUrl = (urlString) => {
        try { 
            return getRegEx().test(urlString);
        }
        catch { 
            return false; 
        }
    };

    const getRegEx = () => {
        return /^(http|ws)[s]{0,1}:\/\/(\*\.){0,1}([a-z0-9\-]{1,}\.){1,}([a-z0-9\-]{1,}(:[0-9]{1,5}){0,1}\/{0,1}){1}$/i;
    };

    return(
        <Group gap='8' mb='16'>
            <Input type='text' placeholder='Enter a URL, e.g. https://www.example.com' value={currentSourceUrl} onChange={handleChange} error={hasError} onBlur={handleBlur} flex='1' />
            <Button appearance='danger' type='button' onClick={handleDelete} icon={<IconXmark />} aria-label='Remove' />
        </Group>
    )
}

FormWildcardSourceUrl.propTypes = {
    sourceId: PropTypes.string.isRequired,
    sourceUrl: PropTypes.string.isRequired,
    handleDeleteSource: PropTypes.func,
    handleUpdateSourceUrl: PropTypes.func
};

export default FormWildcardSourceUrl
