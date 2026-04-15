import { useContext, useEffect, useState } from 'react';
import PropTypes from 'prop-types';
import { Button, Form, InputGroup } from 'react-bootstrap';
import { StottSecurityContext } from '../Context/StottSecurityContext';

function PermissionPolicySettings({ siteId, hostName })
{
    const [disableSaveButton, setDisableSaveButton] = useState(true);

    const { permissionPolicySettings, permissionPolicyDirectivesInherited, getPermissionPolicySettings, savePermissionPolicySettings } = useContext(StottSecurityContext);

    const isContextSpecific = !!siteId || !!hostName;
    const isInherited = isContextSpecific && permissionPolicyDirectivesInherited;

    const handleEnabledChange = (event) => {
        permissionPolicySettings.isEnabled = event.target.value === 'true';
        setDisableSaveButton(false);
    };

    const handleSaveEvent = () => {
        savePermissionPolicySettings(permissionPolicySettings.isEnabled, siteId, hostName);
        setDisableSaveButton(true);
    };

    useEffect(() => {
        getPermissionPolicySettings(siteId, hostName);
        setDisableSaveButton(true);
    }, [siteId, hostName]);

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
    siteId: PropTypes.string,
    hostName: PropTypes.string
};

export default PermissionPolicySettings;
