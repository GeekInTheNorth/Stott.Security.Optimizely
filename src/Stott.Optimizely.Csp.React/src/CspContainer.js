import React from 'react';
import { Tab, Tabs } from 'react-bootstrap';
import PermissionList from './PermissionList';
import EditSettings from './EditSettings';

function CspContainer() {

    return (
        <Tabs defaultActiveKey='csp-source' id='uncontrolled-tab-example' className='mb-2'>
            <Tab eventKey='csp-settings' title='Settings'>
                <EditSettings></EditSettings>
            </Tab>
            <Tab eventKey='csp-source' title='Sources'>
                <PermissionList></PermissionList>
            </Tab>
        </Tabs>
    )
}

export default CspContainer