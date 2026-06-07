import { useState, useEffect } from 'react';
import { httpGet } from '../Common/httpClient';
import { Alert, Card, Col, Container, Row } from 'react-bootstrap';
import GuideCard from './GuideCard';

function AboutContainer() {
    const [guides, setGuides] = useState([]);
    const [isLoading, setIsLoading] = useState(true);
    const [currentVersion, setCurrentVersion] = useState('');

    useEffect(() => {
        loadGuides();
    }, []);

    const loadGuides = async () => {
        await httpGet(import.meta.env.VITE_APP_GUIDES_LIST)
            .then((response) => {
                if (response.data && response.data.articles && Array.isArray(response.data.articles)) {
                    setGuides(response.data.articles);
                    setCurrentVersion(response.data.version || '');
                } else {
                    setGuides([]);
                    setCurrentVersion('');
                }
            })
            .catch(() => {
                setGuides([]);
                setCurrentVersion('');
            })
            .finally(() => {
                setIsLoading(false);
            });
    };

    return (
        <Container>
            <h2>Stott Security for Optimizely</h2>
            <p>Stott Security is an Optimizely CMS add-on for managing your site&apos;s security response headers (including Content Security Policy, CORS, Permissions Policy and custom response headers) through a friendly administration interface.</p>
            <a href='https://github.com/GeekInTheNorth/Stott.Security.Optimizely' target='_blank' rel='noopener noreferrer' className='btn btn-outline-primary'>View on GitHub</a>
            <h2>Guides</h2>
            <p>Articles and walkthroughs to help you get the most out of Stott Security.</p>
            {isLoading ? null : (
                guides.length > 0 ? (
                    <Row className='g-3'>
                        {guides.map((guide, index) => (
                            <GuideCard guide={guide} />
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
