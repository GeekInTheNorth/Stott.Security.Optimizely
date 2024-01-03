import React, { useState, useEffect } from 'react';
import { Card, Container, Alert } from 'react-bootstrap';
import axios from 'axios';

function HeaderPreview(props) {

    const [headerValues, setHeaderValues] = useState([])

    useEffect(() => {
        getHeaderPreview()
    }, [])

    const getHeaderPreview = async () => {
        await axios.get(process.env.REACT_APP_PREVIEW_GET)
            .then((response) => {
                setHeaderValues(response.data);
            },
            () => {
                handleShowFailureToast('Error', 'Failed to load the configured header values.');
            });
    }

    const RenderHeaderValues = () => {
        return headerValues && headerValues.map(headerValue => {
            const { key, value } = headerValue
            return (
                <Card className='mb-3'>
                    <Card.Header className='bg-primary text-light'>{key}</Card.Header>
                    <Card.Body>{value}</Card.Body>
                </Card>
            )
        })
    }

    const handleShowFailureToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(false, title, description);

    return(
        <Container fluid='md'>
            <Alert variant='primary'>The following headers will be generated for all GET requests. Please note that CORS headers are excluded as these vary depending on the request or may only be exposed in preflight requests.</Alert>
            {RenderHeaderValues()}
        </Container>
    )
}

export default HeaderPreview