import { useState, useEffect } from 'react';
import PropTypes from 'prop-types';
import { httpPost } from '../Common/httpClient';
import { Button, Dialog, DialogBody, DialogContent, DialogFooter, DialogHeader, Field, Input, Text } from '@optiaxiom/react';
import FormSelect from '../Common/FormSelect';

const behaviorOptions = [
    { label: 'Disabled - Do not process this header', value: '0' },
    { label: 'Add - Include this header in the response', value: '1' },
    { label: 'Remove - Remove this header from the response', value: '2' }
];
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

    const handleBehaviorChange = (value) => {
        setBehavior(parseInt(value));
        setHasHeaderValueError(false);
    };

    const handleHeaderValueChange = (value) => {
        setHeaderValue(value);
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
            headerValue: behavior === 1 ? headerValue : null,
            appId: props.appId,
            hostName: props.hostName
        };

        await httpPost(import.meta.env.VITE_CUSTOM_HEADER_SAVE, payload)
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
                <FormSelect value={headerValue} onValueChange={handleHeaderValueChange} options={allowedValues.map((option) => ({ label: option.description || option.value, value: option.value }))} />
            );
        } else {
            return (
                <Input type='text' value={headerValue} onChange={(event) => handleHeaderValueChange(event.target.value)} error={hasHeaderValueError} placeholder='e.g., none' />
            );
        }
    };

    return (
        <Dialog open={props.show} onOpenChange={(open) => !open && props.onClose && props.onClose()}>
            <DialogContent size='lg'>
                <DialogHeader>{getModalTitle()}</DialogHeader>
                <DialogBody>
                    <Field label='Header Name' error={hasHeaderNameError ? headerNameErrorMessage : undefined} description={description || undefined}>
                        <Input
                            type='text'
                            value={headerName}
                            onChange={handleHeaderNameChange}
                            error={hasHeaderNameError}
                            placeholder='e.g., X-Permitted-Cross-Domain-Policies'
                            readOnly={!isHeaderNameEditable}
                        />
                    </Field>

                    <Field label='Behavior'>
                        <FormSelect value={behavior} onValueChange={handleBehaviorChange} options={behaviorOptions} />
                    </Field>

                    {behavior === 1 && propertyType === 'hsts' && (
                        <>
                            {renderHeaderValueEditor()}
                            {hasHeaderValueError && (<Text color='fg.error' fontSize='sm'>{headerValueErrorMessage}</Text>)}
                        </>
                    )}

                    {behavior === 1 && propertyType !== 'hsts' && (
                        <Field label='Header Value' error={hasHeaderValueError ? headerValueErrorMessage : undefined}>
                            {renderHeaderValueEditor()}
                        </Field>
                    )}
                </DialogBody>
                <DialogFooter>
                    <Button appearance='default' onClick={props.onClose}>
                        Cancel
                    </Button>
                    <Button appearance='primary' onClick={handleSave}>
                        Save
                    </Button>
                </DialogFooter>
            </DialogContent>
        </Dialog>
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
    appId: PropTypes.string,
    hostName: PropTypes.string,
    show: PropTypes.bool.isRequired,
    onClose: PropTypes.func,
    onSave: PropTypes.func,
    showToastNotificationEvent: PropTypes.func
};

export default CustomHeaderModal;
