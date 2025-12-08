import { useState, useEffect } from 'react';
import PropTypes from 'prop-types';
import { Card, Container, Alert } from 'react-bootstrap';
import axios from 'axios';

function HeaderPreview(props) {

    const [headerValues, setHeaderValues] = useState([])

    const getHeaderPreview = async () => {
        await axios.get(import.meta.env.VITE_PREVIEW_GET)
            .then((response) => {
                setHeaderValues(response.data);
            },
            () => {
                handleShowFailureToast('Error', 'Failed to load the configured header values.');
            });
    };

    const renderHeaderValues = () => {
        return headerValues && headerValues.map((headerValue, index) => {
            const { key, value } = headerValue
            return (
                <Card key={index} className='mb-3'>
                    <Card.Header className='bg-primary text-light'>{key}</Card.Header>
                    <Card.Body>{value}</Card.Body>
                </Card>
            )
        })
    };

    const renderHeaderOptimizationWarning = () =>
    {
        // This warning should only render if headerValues is not empty and contains more than one key with the same name
        // e.g. "Content-Security-Policy" with multiple values
        const headerNameCounts = headerValues.reduce((counts, { key }) => {
            counts[key] = (counts[key] || 0) + 1;
            return counts;
        }, {});
        
        const hasMultipleHeaders = Object.values(headerNameCounts).some(count => count > 1);
        if (!hasMultipleHeaders) {
            return null;
        }

        return (
            <Alert variant='warning'>
                <p>Header optimization has been triggered as the policy is expected to exceed 8KB. If the total size exceeds 12KB, simplifications will be applied such as merging script-src-elem and script-src-attr into script-src. If it approaches 16KB, the CSP will not be generated.</p>
                <p>This is due to common header size limits: 8KB per header, and in systems like Cloudflare, 16KB per header name and 32KB total. Exceeding header limits can result in system inaccessibility.</p>
            </Alert>
        )
    };

    const handleShowFailureToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(false, title, description);

    useEffect(() => {
        getHeaderPreview()
    }, []);

    return(
        <Container fluid='md'>
            <Alert variant='primary'>The following headers will be generated for all GET requests. Please note that CORS headers are excluded as these vary depending on the request or may only be exposed in preflight requests.</Alert>
            {renderHeaderOptimizationWarning()}
            {renderHeaderValues()}
        </Container>
    )
}

HeaderPreview.propTypes = {
    showToastNotificationEvent: PropTypes.func
};

export default HeaderPreview
