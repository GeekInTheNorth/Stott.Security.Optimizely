import { useContext, useEffect, useState } from 'react';
import PropTypes from 'prop-types';
import { Button, Form, InputGroup } from 'react-bootstrap';
import { StottSecurityContext } from '../Context/StottSecurityContext';

function PermissionPolicySettings({ appId, hostName })
{
    const [disableSaveButton, setDisableSaveButton] = useState(true);

    const { permissionPolicySettings, permissionPolicyDirectivesInherited, getPermissionPolicySettings, savePermissionPolicySettings } = useContext(StottSecurityContext);

    const isContextSpecific = !!appId || !!hostName;
    const isInherited = isContextSpecific && permissionPolicyDirectivesInherited;

    const handleEnabledChange = (event) => {
        permissionPolicySettings.isEnabled = event.target.value === 'true';
        setDisableSaveButton(false);
    };

    const handleSaveEvent = () => {
        savePermissionPolicySettings(permissionPolicySettings.isEnabled, appId, hostName);
        setDisableSaveButton(true);
    };

    useEffect(() => {
        getPermissionPolicySettings(appId, hostName);
        setDisableSaveButton(true);
    }, [appId, hostName]);

    return (
        <InputGroup>
            <InputGroup.Text id='lblEnabled'>Permission Policy Header</InputGroup.Text>
            <Form.Select aria-describedby='lblEnabled' className='form-control' onChange={handleEnabledChange} value={permissionPolicySettings.isEnabled} disabled={isInherited}>
                <option value='false'>Disabled</option>
                <option value='true'>Enabled</option>
            </Form.Select>
            {!isInherited && <Button id='btnSave' disabled={disableSaveButton} variant='primary' onClick={handleSaveEvent}>Save</Button>}
        </InputGroup>
    )
}

PermissionPolicySettings.propTypes = {
    appId: PropTypes.string,
    hostName: PropTypes.string
};

export default PermissionPolicySettings;
