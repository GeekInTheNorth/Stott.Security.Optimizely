import { useState, useEffect } from 'react';
import PropTypes from 'prop-types';
import axios from 'axios';
import { Alert, Button, Container, Form, InputGroup, Row } from 'react-bootstrap';
import CustomHeaderModal from './CustomHeaderModal';

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
                if (response.data && response.data.list && Array.isArray(response.data.list)) {
                    setHeaders(response.data.list);
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

    const getBehaviorLabel = (behavior) => {
        return behavior === 0 ? 'Add' : 'Remove';
    };

    const renderHeaders = () => {
        if (headers && headers.length > 0) {
            return headers.map((header, index) => {
                return (
                    <tr key={index}>
                        <td>{header.headerName}</td>
                        <td>{getBehaviorLabel(header.behavior)}</td>
                        <td>{header.headerValue || '-'}</td>
                        <td>
                            <Button variant='primary' onClick={() => handleEditHeader(header)} className='text-nowrap me-2'>Edit</Button>
                            <Button variant='danger' onClick={() => handleDeleteHeader(header.id, header.headerName)} className='text-nowrap'>Delete</Button>
                        </td>
                    </tr>
                );
            });
        }

        return (
            <tr>
                <td colSpan='4'>
                    <Alert variant='primary' className='my-0'>No custom headers found for the current filter.</Alert>
                </td>
            </tr>
        );
    };

    const handleShowSuccessToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(true, title, description);
    const handleShowFailureToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(false, title, description);

    return (
        <>
            <Container fluid='xl' className='my-3'>
                <Alert variant='primary' className='p-3'>
                    Custom Headers allow you to add or remove HTTP response headers. Use this feature to manage headers like X-Permitted-Cross-Domain-Policies, Server, X-Powered-By, or any custom headers your application requires.
                </Alert>
            </Container>
            <Container fluid='xl' className='my-3'>
                <Row className='mb-2'>
                    <div className='col-xl-9 col-lg-9 col-sm-12 col-xs-12 p-0'>
                        <InputGroup>
                            <InputGroup.Text id='lblHeaderNameFilter'>Header Name</InputGroup.Text>
                            <Form.Control
                                id='txtHeaderNameFilter'
                                type='text'
                                value={filterHeaderName}
                                onChange={(event) => setFilterHeaderName(event.target.value)}
                                aria-describedby='lblHeaderNameFilter'
                                placeholder='Filter by header name'
                            />
                            <InputGroup.Text id='lblBehaviorFilter'>Behavior</InputGroup.Text>
                            <Form.Select
                                value={filterBehavior}
                                onChange={(event) => setFilterBehavior(event.target.value)}
                                aria-describedby='lblBehaviorFilter'
                            >
                                <option value='All'>All</option>
                                <option value='0'>Add</option>
                                <option value='1'>Remove</option>
                            </Form.Select>
                        </InputGroup>
                    </div>
                    <div className='col-xl-3 col-lg-3 col-sm-12 col-xs-12 p-0 text-end'>
                        <Button variant='success' onClick={handleAddHeader} className='fw-bold'>Add Custom Header</Button>
                    </div>
                </Row>
                <Row>
                    <table className='table table-striped'>
                        <thead>
                            <tr>
                                <th className='table-header-fix'>Header Name</th>
                                <th className='table-header-fix'>Behavior</th>
                                <th className='table-header-fix'>Header Value</th>
                                <th className='table-header-fix'>Actions</th>
                            </tr>
                        </thead>
                        <tbody>
                            {renderHeaders()}
                        </tbody>
                    </table>
                </Row>
            </Container>
            {showModal && (
                <CustomHeaderModal
                    header={selectedHeader}
                    show={showModal}
                    onClose={handleModalClose}
                    onSave={handleModalSave}
                    showToastNotificationEvent={props.showToastNotificationEvent}
                />
            )}
        </>
    );
}

CustomHeadersContainer.propTypes = {
    showToastNotificationEvent: PropTypes.func
};

export default CustomHeadersContainer;
