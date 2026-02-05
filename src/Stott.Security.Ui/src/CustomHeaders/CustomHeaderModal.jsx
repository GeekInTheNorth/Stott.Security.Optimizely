import { useState, useEffect } from 'react';
import PropTypes from 'prop-types';
import axios from 'axios';
import { Button, Form, Modal } from 'react-bootstrap';
import HstsHeaderValue from './HstsHeaderValue';

function CustomHeaderModal(props) {
    const [id, setId] = useState('');
    const [headerName, setHeaderName] = useState('');
    const [behavior, setBehavior] = useState(0);
    const [headerValue, setHeaderValue] = useState('');
    const [description, setDescription] = useState('');
    const [allowedValues, setAllowedValues] = useState([]);
    const [hasHeaderNameError, setHasHeaderNameError] = useState(false);
    const [headerNameErrorMessage, setHeaderNameErrorMessage] = useState('');
    const [hasHeaderValueError, setHasHeaderValueError] = useState(false);
    const [headerValueErrorMessage, setHeaderValueErrorMessage] = useState('');
    const [propertyType, setPropertyType] = useState('string');
    const [isHeaderNameEditable, setIsHeaderNameEditable] = useState(true);

    useEffect(() => {
        if (props.header) {
            setId(props.header.id);
            setHeaderName(props.header.headerName);
            setBehavior(props.header.behavior);
            setHeaderValue(props.header.headerValue || '');
            setDescription(props.header.description || '');
            setAllowedValues(props.header.allowedValues || []);
            setPropertyType(props.header.propertyType || 'string');
            setIsHeaderNameEditable(props.header.isHeaderNameEditable !== false);
        } else {
            setId('00000000-0000-0000-0000-000000000000');
            setHeaderName('');
            setBehavior(1);
            setHeaderValue('');
            setDescription('');
            setAllowedValues([]);
            setPropertyType('string');
            setIsHeaderNameEditable(true);
        }
        setHasHeaderNameError(false);
        setHeaderNameErrorMessage('');
        setHasHeaderValueError(false);
        setHeaderValueErrorMessage('');
    }, [props.header, props.show]);

    const handleHeaderNameChange = (event) => {
        setHeaderName(event.target.value);
        setHasHeaderNameError(false);
    };

    const handleBehaviorChange = (event) => {
        setBehavior(parseInt(event.target.value));
        setHasHeaderValueError(false);
    };

    const handleHeaderValueChange = (event) => {
        setHeaderValue(event.target.value);
        setHasHeaderValueError(false);
    };

    const handleHstsValueChange = (newValue) => {
        setHeaderValue(newValue);
        setHasHeaderValueError(false);
    };

    const handleSave = async () => {
        const payload = {
            id: id,
            headerName: headerName,
            behavior: behavior,
            headerValue: behavior === 1 ? headerValue : null
        };

        await axios.post(import.meta.env.VITE_CUSTOM_HEADER_SAVE, payload)
            .then(() => {
                handleShowSuccessToast('Success', 'Custom header has been successfully saved.');
                props.onSave && props.onSave();
            })
            .catch((error) => {
                if (error.response && error.response.status === 400) {
                    const validationResult = error.response.data;
                    if (validationResult.errors) {
                        validationResult.errors.forEach((error) => {
                            if (error.propertyName === 'HeaderName') {
                                setHasHeaderNameError(true);
                                setHeaderNameErrorMessage(error.errorMessage);
                            } else if (error.propertyName === 'HeaderValue') {
                                setHasHeaderValueError(true);
                                setHeaderValueErrorMessage(error.errorMessage);
                            }
                        });
                    }
                } else {
                    handleShowFailureToast('Error', 'Failed to save the custom header.');
                    props.onClose && props.onClose();
                }
            });
    };

    const handleShowSuccessToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(true, title, description);
    const handleShowFailureToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(false, title, description);

    const getModalTitle = () => {
        return props.header ? 'Edit Custom Header' : 'Add Custom Header';
    };

    const renderHeaderValueEditor = () => {
        if (propertyType === 'hsts') {
            return <HstsHeaderValue value={headerValue} onChange={handleHstsValueChange} />;
        } else if (propertyType === 'select' && allowedValues && allowedValues.length > 0) {
            return (
                <Form.Select value={headerValue} onChange={handleHeaderValueChange} isInvalid={hasHeaderValueError}>
                    {allowedValues.map((option, index) => (
                        <option key={index} value={option.value}>
                            {option.description || option.value}
                        </option>
                    ))}
                </Form.Select>
            );
        } else {
            return (
                <Form.Control type='text' value={headerValue} onChange={handleHeaderValueChange} isInvalid={hasHeaderValueError} placeholder='e.g., none' />
            );
        }
    };

    return (
        <Modal show={props.show} onHide={props.onClose} size='lg'>
            <Modal.Header closeButton>
                <Modal.Title>{getModalTitle()}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Form.Group className='mb-3'>
                    <Form.Label>Header Name</Form.Label>
                    <Form.Control
                        type='text'
                        value={headerName}
                        onChange={handleHeaderNameChange}
                        isInvalid={hasHeaderNameError}
                        placeholder='e.g., X-Permitted-Cross-Domain-Policies'
                        readOnly={!isHeaderNameEditable}
                        className={!isHeaderNameEditable ? 'bg-light' : ''}
                    />
                    {hasHeaderNameError && (
                        <Form.Control.Feedback type='invalid'>{headerNameErrorMessage}</Form.Control.Feedback>
                    )}
                </Form.Group>

                {description && (
                    <Form.Group className='mb-3'>
                        <Form.Text className='text-muted'>{description}</Form.Text>
                    </Form.Group>
                )}

                <Form.Group className='mb-3'>
                    <Form.Label>Behavior</Form.Label>
                    <Form.Select value={behavior} onChange={handleBehaviorChange}>
                        <option value='0'>Disabled - Do not process this header</option>
                        <option value='1'>Add - Include this header in the response</option>
                        <option value='2'>Remove - Remove this header from the response</option>
                    </Form.Select>
                </Form.Group>

                {behavior === 1 && (
                    <Form.Group className='mb-3'>
                        {propertyType !== 'hsts' && <Form.Label>Header Value</Form.Label>}
                        {renderHeaderValueEditor()}
                        {hasHeaderValueError && (<Form.Control.Feedback type='invalid'>{headerValueErrorMessage}</Form.Control.Feedback>)}
                    </Form.Group>
                )}
            </Modal.Body>
            <Modal.Footer>
                <Button variant='primary' onClick={handleSave}>
                    Save
                </Button>
                <Button variant='secondary' onClick={props.onClose}>
                    Cancel
                </Button>
            </Modal.Footer>
        </Modal>
    );
}

CustomHeaderModal.propTypes = {
    header: PropTypes.shape({
        id: PropTypes.string,
        headerName: PropTypes.string,
        behavior: PropTypes.number,
        headerValue: PropTypes.string,
        description: PropTypes.string,
        allowedValues: PropTypes.arrayOf(PropTypes.shape({
            value: PropTypes.string,
            description: PropTypes.string
        })),
        propertyType: PropTypes.string,
        isHeaderNameEditable: PropTypes.bool,
        canDelete: PropTypes.bool
    }),
    show: PropTypes.bool.isRequired,
    onClose: PropTypes.func,
    onSave: PropTypes.func,
    showToastNotificationEvent: PropTypes.func
};

export default CustomHeaderModal;
