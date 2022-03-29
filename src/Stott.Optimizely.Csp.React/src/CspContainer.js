import React, { useState } from 'react';
import { Tab, Tabs, Toast, ToastContainer } from 'react-bootstrap';
import PermissionList from './PermissionList';
import EditSettings from './EditSettings';
import EditLegacyHeaderSettings from './EditLegacyHeaderSettings'
import ViolationReport from './ViolationReport';

function CspContainer() {

    const [showToastNotification, setShowToastNotification] = useState(false);
    const [toastTitle, setToastTitle] = useState('');
    const [toastDescription, setToastDescription] = useState('');
    const [toastHeaderClass, setToastHeaderClass] = useState('');

    const showToastNotificationEvent = (isSuccess, title, description) => {
        if (isSuccess === true){
            setToastHeaderClass('bg-success text-white');
        } else{
            setToastHeaderClass('bg-danger text-white');
        }

        setShowToastNotification(false);
        setToastTitle(title);
        setToastDescription(description)
        setShowToastNotification(true);
    };
    const closeToastNotification = () => setShowToastNotification(false);

    return (
        <>
            <Tabs defaultActiveKey='csp-settings' id='uncontrolled-tab-example' className='mb-2'>
                <Tab eventKey='csp-settings' title='CSP Settings'>
                    <EditSettings showToastNotificationEvent={showToastNotificationEvent}></EditSettings>
                </Tab>
                <Tab eventKey='csp-source' title='CSP Sources'>
                    <PermissionList showToastNotificationEvent={showToastNotificationEvent}></PermissionList>
                </Tab>
                <Tab eventKey='csp-violations' title='CSP Violations'>
                    <ViolationReport showToastNotificationEvent={showToastNotificationEvent}></ViolationReport>
                </Tab>
                <Tab eventKey='legacy-headers' title='Security Headers'>
                    <EditLegacyHeaderSettings showToastNotificationEvent={showToastNotificationEvent}></EditLegacyHeaderSettings>
                </Tab>
            </Tabs>
            <ToastContainer className="p-3" position='top-center'>
                <Toast onClose={closeToastNotification} show={showToastNotification} delay={5000} autohide={true}>
                    <Toast.Header className={toastHeaderClass}>
                        <strong className="me-auto">{toastTitle}</strong>
                    </Toast.Header>
                    <Toast.Body>{toastDescription}</Toast.Body>
                </Toast>
            </ToastContainer>
        </>
    )
}

export default CspContainer