import { Card, Container } from 'react-bootstrap';
import EditSettings from './EditSettings';
import SandboxSettings from './SandboxSettings';

function CspSettingsContainer(props) {
    
    const handleShowToastNotification = (isSuccess, title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(isSuccess, title, description);

    return (
        <Container fluid='md'>
            <Card className='mb-3'>
                <Card.Header className='bg-primary text-light'>Content Security Policy - General Settings</Card.Header>
                <Card.Body>
                    <EditSettings showToastNotificationEvent={handleShowToastNotification}></EditSettings>
                </Card.Body>
            </Card>
            <Card className='mb-3'>
                <Card.Header className='bg-primary text-light'>Content Security Policy - Sandbox Settings</Card.Header>
                <Card.Body>
                    <SandboxSettings showToastNotificationEvent={handleShowToastNotification}></SandboxSettings>
                </Card.Body>
            </Card>
        </Container>
    )
}

export default CspSettingsContainer;
