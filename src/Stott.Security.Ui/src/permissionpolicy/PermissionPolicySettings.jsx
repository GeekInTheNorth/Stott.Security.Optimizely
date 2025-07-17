import React, { useContext, useEffect, useState } from 'react';
import { Button, Form, InputGroup } from 'react-bootstrap';
import { PermissionPolicyContext } from './PermissionPolicy';

function PermissionPolicySettings() {
    const [disableSaveButton, setDisableSaveButton] = useState(true);
    const { permissionPolicySettings, getPermissionPolicySettings, savePermissionPolicySettings } = useContext(PermissionPolicyContext);

    const handleEnabledChange = (event) => {
        permissionPolicySettings.isEnabled = event.target.value === 'true';
        setDisableSaveButton(false);
    };

    const handleSaveEvent = () => {
        savePermissionPolicySettings(permissionPolicySettings.isEnabled);
        setDisableSaveButton(true);
    };

    useEffect(() => { getPermissionPolicySettings(); }, []);

    return (
        <InputGroup>
            <InputGroup.Text id='lblEnabled'>Permission Policy Header</InputGroup.Text>
            <Form.Select aria-describedby='lblEnabled' className='form-control' onChange={handleEnabledChange} value={permissionPolicySettings.isEnabled}>
                <option value='false'>Disabled</option>
                <option value='true'>Enabled</option>
            </Form.Select>
            <Button id='btnSave' disabled={disableSaveButton} variant='primary' onClick={handleSaveEvent}>Save</Button>
        </InputGroup>
    );
}

export default PermissionPolicySettings; 