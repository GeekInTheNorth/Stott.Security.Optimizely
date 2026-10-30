import { useState } from 'react';
import PropTypes from 'prop-types';
import { Button, Group, Input } from '@optiaxiom/react';
import { IconXmark } from '@optiaxiom/icons';

function FormSourceUrl(props)
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
            const parsedUrl = new URL(event.target.value);
            setCurrentSourceUrl(parsedUrl.origin);
            setHasError(false);
            props.handleUpdateSourceUrl && props.handleUpdateSourceUrl(props.sourceId, parsedUrl.origin);
        }
        catch {
            setHasError(true);
            props.handleUpdateSourceUrl && props.handleUpdateSourceUrl(props.sourceId, event.target.value);
        }
    };

    const isValidUrl = (urlString) => {
        try { 
            const parsedUrl = new URL(urlString);
            return parsedUrl.pathname === '/' && parsedUrl.search === '' && parsedUrl.hash === '';
        }
        catch { 
            return false; 
        }
    };

    return(
        <Group gap='8' mb='16'>
            <Input type='text' placeholder='Enter a URL, e.g. https://www.example.com' value={currentSourceUrl} onChange={handleChange} error={hasError} onBlur={handleBlur} flex='1' />
            <Button appearance='danger' type='button' onClick={handleDelete} icon={<IconXmark />} aria-label='Remove' />
        </Group>
    )
}

FormSourceUrl.propTypes = {
    sourceId: PropTypes.string.isRequired,
    sourceUrl: PropTypes.string.isRequired,
    handleDeleteSource: PropTypes.func,
    handleUpdateSourceUrl: PropTypes.func
};

export default FormSourceUrl
