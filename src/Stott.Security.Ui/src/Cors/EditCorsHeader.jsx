import { useState } from 'react';
import PropTypes from 'prop-types';
import { Button, Group, Input } from '@optiaxiom/react';
import { IconXmark } from '@optiaxiom/icons';

function EditCorsHeader(props) {

    const [hasError, setHasError] = useState(false);
    const [currentHeaderName, setCurrentHeaderName] = useState(props.headerName);

    const handleDelete = () => props.handleDeleteHeader && props.headerId && props.handleDeleteHeader(props.headerId);

    const handleChange = (event) => {
        setCurrentHeaderName(event.target.value);
        if (isValidHeaderName(event.target.value)) {
            setHasError(false);
        }
        else {
            setHasError(true);
        }
    }

    const handleBlur = (event) => {
        setCurrentHeaderName(event.target.value);
        if (isValidHeaderName(event.target.value)) {
            setHasError(false);
            props.handleUpdateHeader && props.handleUpdateHeader(props.headerId, event.target.value);
        }
        else {
            setHasError(true);
            props.handleUpdateHeader && props.handleUpdateHeader(props.headerId, event.target.value);
        }
    }

    const isValidHeaderName = (headerName) => {
        return /^[a-zA-Z0-9\-_]+$/.test(headerName);
    }

    return (
        <Group gap='8' mb='16'>
            <Input type='text' placeholder='Header Name' value={currentHeaderName} onChange={handleChange} error={hasError} onBlur={handleBlur} flex='1' />
            <Button appearance='danger' type="button" onClick={handleDelete} icon={<IconXmark />} aria-label='Remove' />
        </Group>
    )
}

EditCorsHeader.propTypes = {
    headerId: PropTypes.string.isRequired,
    headerName: PropTypes.string.isRequired,
    handleDeleteHeader: PropTypes.func,
    handleUpdateHeader: PropTypes.func
};

export default EditCorsHeader
