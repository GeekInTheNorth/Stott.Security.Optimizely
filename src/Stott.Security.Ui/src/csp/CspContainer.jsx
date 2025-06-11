import { useState } from "react";
import CspList from "./CspList";
import { useCsp } from "./CspContext";
import CspSettings from "./CspSettings";
import CspBreadcrumb from "./CspBreadcrumb";
import CspSandbox from "./CspSandbox";
import CspSourceList from "./CspSourceList";
import CspViolationList from "./CspViolationList";
import { Toast, ToastContainer } from "react-bootstrap";

function CspContainer()
{
    const [showToastNotification, setShowToastNotification] = useState(false);
    const [toastTitle, setToastTitle] = useState('');
    const [toastDescription, setToastDescription] = useState('');
    const [toastHeaderClass, setToastHeaderClass] = useState('');
    
    const { viewMode, currentPolicy } = useCsp();

    const closeToastNotification = () => setShowToastNotification(false);

    const showToastNotificationEvent = (isSuccess, title, description) => {
        if (isSuccess === true) {
            setToastHeaderClass('bg-success text-white');
        } else {
            setToastHeaderClass('bg-danger text-white');
        }

        setShowToastNotification(false);
        setToastTitle(title);
        setToastDescription(description)
        setShowToastNotification(true);
    };

    return (
        <>
            <CspBreadcrumb />
            {viewMode === 'list' ? <CspList /> : null}
            {viewMode === 'settings' ? <CspSettings cspPolicy={currentPolicy} showToastNotificationEvent={showToastNotificationEvent} /> : null}
            {viewMode === 'sandbox' ? <CspSandbox showToastNotificationEvent={showToastNotificationEvent} /> : null}
            {viewMode === 'sources' ? <CspSourceList /> : null}
            {viewMode === 'violations' ? <CspViolationList showToastNotificationEvent={showToastNotificationEvent} /> : null}
            <ToastContainer className="p-3" position='middle-center'>
                <Toast onClose={closeToastNotification} show={showToastNotification} delay={4000} autohide={true}>
                <Toast.Header className={toastHeaderClass}>
                    <strong className="me-auto">{toastTitle}</strong>
                </Toast.Header>
                <Toast.Body className='bg-full-opacity'>{toastDescription}</Toast.Body>
                </Toast>
            </ToastContainer>
        </>
    );
}

export default CspContainer;