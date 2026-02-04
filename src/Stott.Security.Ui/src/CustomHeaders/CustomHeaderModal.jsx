import { useState, useEffect } from 'react';
import PropTypes from 'prop-types';
import axios from 'axios';
import { Button, Form, Modal } from 'react-bootstrap';

function CustomHeaderModal(props) {
    const [id, setId] = useState('');
    const [headerName, setHeaderName] = useState('');
    const [behavior, setBehavior] = useState(0);
    const [headerValue, setHeaderValue] = useState('');
    const [hasHeaderNameError, setHasHeaderNameError] = useState(false);
    const [headerNameErrorMessage, setHeaderNameErrorMessage] = useState('');
    const [hasHeaderValueError, setHasHeaderValueError] = useState(false);
    const [headerValueErrorMessage, setHeaderValueErrorMessage] = useState('');

    useEffect(() => {
        if (props.header) {
            setId(props.header.id);
            setHeaderName(props.header.headerName);
            setBehavior(props.header.behavior);
            setHeaderValue(props.header.headerValue || '');
        } else {
            setId('00000000-0000-0000-0000-000000000000');
            setHeaderName('');
            setBehavior(0);
            setHeaderValue('');
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

    const handleSave = async () => {
        const payload = {
            id: id,
            headerName: headerName,
            behavior: behavior,
            headerValue: behavior === 0 ? headerValue : null
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
                    />
                    {hasHeaderNameError && (
                        <Form.Control.Feedback type='invalid'>
                            {headerNameErrorMessage}
                        </Form.Control.Feedback>
                    )}
                </Form.Group>

                <Form.Group className='mb-3'>
                    <Form.Label>Behavior</Form.Label>
                    <Form.Select value={behavior} onChange={handleBehaviorChange}>
                        <option value='0'>Add - Include this header in the response</option>
                        <option value='1'>Remove - Remove this header from the response</option>
                    </Form.Select>
                </Form.Group>

                {behavior === 0 && (
                    <Form.Group className='mb-3'>
                        <Form.Label>Header Value</Form.Label>
                        <Form.Control
                            type='text'
                            value={headerValue}
                            onChange={handleHeaderValueChange}
                            isInvalid={hasHeaderValueError}
                            placeholder='e.g., none'
                        />
                        {hasHeaderValueError && (
                            <Form.Control.Feedback type='invalid'>
                                {headerValueErrorMessage}
                            </Form.Control.Feedback>
                        )}
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
        headerValue: PropTypes.string
    }),
    show: PropTypes.bool.isRequired,
    onClose: PropTypes.func,
    onSave: PropTypes.func,
    showToastNotificationEvent: PropTypes.func
};

export default CustomHeaderModal;
