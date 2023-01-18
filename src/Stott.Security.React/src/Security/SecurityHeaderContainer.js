import React from 'react';
import { Card, Container } from 'react-bootstrap';
import EditCrossOriginHeaders from './EditCrossOriginHeaders';
import EditHeaderSettings from './EditHeaderSettings';
import EditStrictTransportSecurity from './EditStrictTransportSecurity';

function SecurityHeaderContainer(props) {
    return(
        <Container fluid='md'>
            <Card className='mb-3'>
                <Card.Header>General Security Headers</Card.Header>
                <Card.Body>
                    <EditHeaderSettings showToastNotificationEvent={props.showToastNotificationEvent}></EditHeaderSettings>
                </Card.Body>
            </Card>
            <Card className='mb-3'>
                <Card.Header>Cross Origin Headers</Card.Header>
                <Card.Body>
                    <EditCrossOriginHeaders showToastNotificationEvent={props.showToastNotificationEvent}></EditCrossOriginHeaders>
                </Card.Body>
            </Card>
            <Card className='mb-3'>
                <Card.Header>HTTP Strict Transport Security Header (HSTS)</Card.Header>
                <Card.Body>
                    <EditStrictTransportSecurity showToastNotificationEvent={props.showToastNotificationEvent}></EditStrictTransportSecurity>
                </Card.Body>
            </Card>
        </Container>
    )
}

export default SecurityHeaderContainer