import React, { useContext, useEffect, useState } from 'react';
import { Button, Form, InputGroup } from 'react-bootstrap';
import { StottSecurityContext } from '../Context/StottSecurityContext';

function PermissionPolicySettings()
{
    const [disableSaveButton, setDisableSaveButton] = useState(true);

    const { permissionPolicySettings, getPermissionPolicySettings } = useContext(StottSecurityContext);

    const handleEnabledChange = () => {
        setDisableSaveButton(false);
    };

    const handleSaveEvent = () => {
        setDisableSaveButton(true);
    };

    useEffect(() => { getPermissionPolicySettings() }, []);

    return (
        <InputGroup>
            <InputGroup.Text id='lblEnabled'>Permission Policy Header</InputGroup.Text>
            <Form.Select aria-describedby='lblEnabled' className='form-control' onChange={handleEnabledChange} value={permissionPolicySettings.isEnabled}>
                <option value='false'>Disabled</option>
                <option value='true'>Enabled</option>
            </Form.Select>
            <Button id='btnSave' disabled={disableSaveButton} variant='primary' onClick={handleSaveEvent}>Save</Button>
        </InputGroup>
    )
}

export default PermissionPolicySettings;