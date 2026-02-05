import { useState, useEffect } from 'react';
import PropTypes from 'prop-types';
import axios from 'axios';
import { Alert, Button, Container, Form, InputGroup, Row } from 'react-bootstrap';
import CustomHeaderModal from './CustomHeaderModal';
import CustomHeaderCard from './CustomHeaderCard';

function CustomHeadersContainer(props) {
    const [headers, setHeaders] = useState([]);
    const [filterHeaderName, setFilterHeaderName] = useState('');
    const [filterBehavior, setFilterBehavior] = useState('');
    const [showModal, setShowModal] = useState(false);
    const [selectedHeader, setSelectedHeader] = useState(null);

    useEffect(() => {
        loadHeaders();
    }, [filterHeaderName, filterBehavior]);

    const loadHeaders = async () => {
        const params = {};
        if (filterHeaderName) params.headerName = filterHeaderName;
        if (filterBehavior !== '' && filterBehavior !== 'All') params.behavior = parseInt(filterBehavior);

        await axios.get(import.meta.env.VITE_CUSTOM_HEADER_LIST, { params })
            .then((response) => {
                if (response.data && Array.isArray(response.data)) {
                    setHeaders(response.data);
                } else {
                    handleShowFailureToast('Failure', 'Failed to retrieve custom headers.');
                }
            })
            .catch(() => {
                handleShowFailureToast('Failure', 'Failed to retrieve custom headers.');
            });
    };

    const handleAddHeader = () => {
        setSelectedHeader(null);
        setShowModal(true);
    };

    const handleEditHeader = (header) => {
        setSelectedHeader(header);
        setShowModal(true);
    };

    const handleDeleteHeader = async (id, headerName) => {
        if (!confirm(`Are you sure you want to delete the custom header "${headerName}"?`)) {
            return;
        }

        await axios.delete(import.meta.env.VITE_CUSTOM_HEADER_DELETE, { params: { id } })
            .then(() => {
                handleShowSuccessToast('Success', `Custom header "${headerName}" was successfully deleted.`);
                loadHeaders();
            })
            .catch(() => {
                handleShowFailureToast('Failure', `Failed to delete custom header "${headerName}".`);
            });
    };

    const handleModalClose = () => {
        setShowModal(false);
        setSelectedHeader(null);
    };

    const handleModalSave = () => {
        setShowModal(false);
        setSelectedHeader(null);
        loadHeaders();
    };

    const renderHeaders = () => {
        if (headers && headers.length > 0) {
            return headers.map((header) => (
                <CustomHeaderCard key={header.id} header={header} onEdit={handleEditHeader} onDelete={handleDeleteHeader} />
            ));
        }

        return (
            <Alert variant='primary' className='my-0'>No custom headers found for the current filter.</Alert>
        );
    };

    const handleShowSuccessToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(true, title, description);
    const handleShowFailureToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(false, title, description);

    return (
        <>
            <Alert variant='primary' className='p-3 container-xl'>
                Custom Headers allow you to add or remove HTTP response headers. Use this feature to manage headers like X-Permitted-Cross-Domain-Policies, Server, X-Powered-By, or any custom headers your application requires.
            </Alert>
            <Container fluid='xl' className='my-3 px-0'>
                <Row>
                    <div className='col-xl-9 col-lg-9 col-sm-12 col-xs-12'>
                        <InputGroup>
                            <InputGroup.Text id='lblHeaderNameFilter'>Name</InputGroup.Text>
                            <Form.Control id='txtHeaderNameFilter' type='text' value={filterHeaderName} onChange={(event) => setFilterHeaderName(event.target.value)} aria-describedby='lblHeaderNameFilter' placeholder='Filter by header name' />
                            <InputGroup.Text id='lblBehaviorFilter'>Behavior</InputGroup.Text>
                            <Form.Select value={filterBehavior} onChange={(event) => setFilterBehavior(event.target.value)} aria-describedby='lblBehaviorFilter'>
                                <option value='All'>All</option>
                                <option value='0'>Add</option>
                                <option value='1'>Remove</option>
                            </Form.Select>
                        </InputGroup>
                    </div>
                    <div className='col-xl-3 col-lg-3 col-sm-12 col-xs-12 text-end'>
                        <Button variant='success' onClick={handleAddHeader} className='fw-bold'>Add Header</Button>
                    </div>
                </Row>
                {renderHeaders()}
            </Container>
            {showModal && (
                <CustomHeaderModal header={selectedHeader} show={showModal} onClose={handleModalClose} onSave={handleModalSave} showToastNotificationEvent={props.showToastNotificationEvent} />
            )}
        </>
    );
}

CustomHeadersContainer.propTypes = {
    showToastNotificationEvent: PropTypes.func
};

export default CustomHeadersContainer;
