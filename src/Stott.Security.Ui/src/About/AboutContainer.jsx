import { useState, useEffect } from 'react';
import { httpGet } from '../Common/httpClient';
import { Alert, Card, Col, Container, Row } from 'react-bootstrap';
import GuideCard from './GuideCard';

function AboutContainer() {
    const [guides, setGuides] = useState([]);
    const [isLoading, setIsLoading] = useState(true);

    useEffect(() => {
        loadGuides();
    }, []);

    const loadGuides = async () => {
        await httpGet(import.meta.env.VITE_APP_GUIDES_LIST)
            .then((response) => {
                if (response.data && Array.isArray(response.data)) {
                    setGuides(response.data);
                } else {
                    setGuides([]);
                }
            })
            .catch(() => {
                setGuides([]);
            })
            .finally(() => {
                setIsLoading(false);
            });
    };

    return (
        <Container fluid className='mt-3'>
            <Card className='mb-4'>
                <Card.Body>
                    <Card.Title>Stott Security for Optimizely</Card.Title>
                    <Card.Text>
                        Stott Security is an Optimizely CMS add-on for managing your site&apos;s security response
                        headers &mdash; including Content Security Policy, CORS, Permissions Policy and custom
                        response headers &mdash; through a friendly administration interface.
                    </Card.Text>
                    <Card.Text className='mb-0'>
                        <a href='https://github.com/GeekInTheNorth/Stott.Security.Optimizely' target='_blank' rel='noopener noreferrer'>View on GitHub</a>
                        <span className='mx-2'>&bull;</span>
                        <a href='https://www.stott.pro' target='_blank' rel='noopener noreferrer'>stott.pro</a>
                    </Card.Text>
                </Card.Body>
            </Card>

            <h2 className='h4'>Guides</h2>
            <p>Articles and walkthroughs to help you get the most out of Stott Security.</p>

            {isLoading ? null : (
                guides.length > 0 ? (
                    <Row xs={1} md={2} lg={3} className='g-3'>
                        {guides.map((guide, index) => (
                            <Col key={guide.url ?? index}>
                                <GuideCard guide={guide} />
                            </Col>
                        ))}
                    </Row>
                ) : (
                    <Alert variant='info'>Guides are temporarily unavailable. Please check back later.</Alert>
                )
            )}
        </Container>
    );
}

export default AboutContainer;
