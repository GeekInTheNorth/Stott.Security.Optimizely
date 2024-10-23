import React from 'react';
import EditPermissionPolicy from '../PermissionsPolicy/EditPermissionPolicy';
import { Container } from 'react-bootstrap';

function PermissionsPolicyContainer(props)
{
    return (
        <Container fluid='md'>
            <EditPermissionPolicy directiveName='accelerometer' directiveTitle='Accelerator' directiveDescription='Controls how the site can consume acceleration information from a device.' />
            <EditPermissionPolicy directiveName='ambient-light-sensor' directiveTitle='Ambient Light Sensor' directiveDescription='Controls how the site can consume light sensor information from a device.' />
        </Container>
    )
}

export default PermissionsPolicyContainer