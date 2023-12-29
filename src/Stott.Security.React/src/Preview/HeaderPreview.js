import React, { useState, useEffect } from 'react';
import { Container, Row, Alert } from 'react-bootstrap';
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
                <Row>
                    <h3 className='h4'>{key}</h3>
                    <p>{value}</p>
                </Row>
            )
        })
    }

    const handleShowFailureToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(false, title, description);

    return(
        <Container>
            <Alert variant='primary'>The following headers will be generated for all GET requests. Please note that CORS headers are excluded as these vary depending on the request or may only exposed in preflight requests.</Alert>
            {RenderHeaderValues()}
        </Container>
    )
}

export default HeaderPreview