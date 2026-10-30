import { useContext, useEffect, useState } from 'react';
import PropTypes from 'prop-types';
import { Box, Button, Group, Text } from '@optiaxiom/react';
import FormSelect from '../Common/FormSelect';

const enabledOptions = [
    { label: 'Disabled', value: 'false' },
    { label: 'Enabled', value: 'true' }
];
import { StottSecurityContext } from '../Context/StottSecurityContext';

function PermissionPolicySettings({ appId, hostName })
{
    const [disableSaveButton, setDisableSaveButton] = useState(true);
    const [selectedIsEnabled, setSelectedIsEnabled] = useState(null);

    const { permissionPolicySettings, permissionPolicyDirectivesInherited, getPermissionPolicySettings, savePermissionPolicySettings } = useContext(StottSecurityContext);

    const isContextSpecific = !!appId || !!hostName;
    const isInherited = isContextSpecific && permissionPolicyDirectivesInherited;
    const isEnabled = selectedIsEnabled ?? permissionPolicySettings.isEnabled;

    const handleEnabledChange = (value) => {
        permissionPolicySettings.isEnabled = value === 'true';
        setSelectedIsEnabled(value === 'true');
        setDisableSaveButton(false);
    };

    const handleSaveEvent = () => {
        savePermissionPolicySettings(permissionPolicySettings.isEnabled, appId, hostName);
        setDisableSaveButton(true);
    };

    useEffect(() => {
        getPermissionPolicySettings(appId, hostName);
        setSelectedIsEnabled(null);
        setDisableSaveButton(true);
    }, [appId, hostName]);

    return (
        <Group gap='8'>
            <Text id='lblEnabled' fontWeight='500' whiteSpace='nowrap'>Permission Policy Header</Text>
            <Box flex='1'>
                <FormSelect aria-labelledby='lblEnabled' onValueChange={handleEnabledChange} value={isEnabled} options={enabledOptions} disabled={isInherited} />
            </Box>
            {!isInherited && <Button id='btnSave' disabled={disableSaveButton} appearance='primary' onClick={handleSaveEvent}>Save</Button>}
        </Group>
    )
}

PermissionPolicySettings.propTypes = {
    appId: PropTypes.string,
    hostName: PropTypes.string
};

export default PermissionPolicySettings;
