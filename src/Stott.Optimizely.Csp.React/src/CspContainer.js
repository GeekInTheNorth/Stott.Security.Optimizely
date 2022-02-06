import React from 'react';
import { Tab, Tabs } from 'react-bootstrap';
import PermissionList from './PermissionList';
import EditSettings from './EditSettings';
import EditLegacyHeaderSettings from './EditLegacyHeaderSettings'

function CspContainer() {

    return (
        <Tabs defaultActiveKey='csp-settings' id='uncontrolled-tab-example' className='mb-2'>
            <Tab eventKey='csp-settings' title='CSP Settings'>
                <EditSettings></EditSettings>
            </Tab>
            <Tab eventKey='csp-source' title='CSP Sources'>
                <PermissionList></PermissionList>
            </Tab>
            <Tab eventKey='legacy-headers' title='Security Headers'>
                <EditLegacyHeaderSettings></EditLegacyHeaderSettings>
            </Tab>
        </Tabs>
    )
}

export default CspContainer