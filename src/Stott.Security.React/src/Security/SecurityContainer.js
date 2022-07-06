import React, { useState } from 'react';
import { Container, Tab, Tabs, Toast, ToastContainer } from 'react-bootstrap';
import EditHeaderSettings from './EditHeaderSettings';
import EditStrictTransportSecurity from './EditStrictTransportSecurity';

function SecurityContainer() {

    const [showToastNotification, setShowToastNotification] = useState(false);
    const [toastTitle, setToastTitle] = useState('');
    const [toastDescription, setToastDescription] = useState('');
    const [toastHeaderClass, setToastHeaderClass] = useState('');
    const [showSecurityHeaders, setShowSecurityHeaders] = useState(true);
    const [showStrictTransport, setShowStrictTransport] = useState(false);

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

    const handleSelect = (key) => {
        setShowSecurityHeaders(false);
        setShowStrictTransport(false);
        switch(key){
            case 'legacy-headers':
                setShowSecurityHeaders(true);
                break;
            case 'strinct-transport-header':
                setShowStrictTransport(true);
            default:
                // No default required
                break;
        }
    }

    return (
        <>
            <Container>

            </Container>
            <Tabs defaultActiveKey='legacy-headers' className='mb-2' onSelect={handleSelect}>
                <Tab eventKey='legacy-headers' title='Security Headers'>
                    { showSecurityHeaders ? <EditHeaderSettings showToastNotificationEvent={showToastNotificationEvent}></EditHeaderSettings> : null }
                </Tab>
                <Tab eventKey='strinct-transport-header' title='Strict Transport Security'>
                    { showStrictTransport ? <EditStrictTransportSecurity showToastNotificationEvent={showToastNotificationEvent}></EditStrictTransportSecurity> : null }
                </Tab>
            </Tabs>
            <ToastContainer className="p-3" position='middle-center'>
                <Toast onClose={closeToastNotification} show={showToastNotification} delay={4000} autohide={true}>
                    <Toast.Header className={toastHeaderClass}>
                        <strong className="me-auto">{toastTitle}</strong>
                    </Toast.Header>
                    <Toast.Body>{toastDescription}</Toast.Body>
                </Toast>
            </ToastContainer>
        </>
    )
}

export default SecurityContainer