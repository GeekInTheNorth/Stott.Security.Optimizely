import { useState, useEffect } from 'react';
import PropTypes from 'prop-types';
import axios from 'axios';
import { Alert, Button, Container, Form, InputGroup, Row } from 'react-bootstrap';
import CustomHeaderModal from './CustomHeaderModal';
import CustomHeaderCard from './CustomHeaderCard';
import ConfirmationModal from '../Common/ConfirmationModal';
import ContextSwitcher from '../Common/ContextSwitcher';

function CustomHeadersContainer(props) {
    const [headers, setHeaders] = useState([]);
    const [filterHeaderName, setFilterHeaderName] = useState('');
    const [filterBehavior, setFilterBehavior] = useState('');
    const [showModal, setShowModal] = useState(false);
    const [selectedHeader, setSelectedHeader] = useState(null);
    const [showDeleteConfirm, setShowDeleteConfirm] = useState(false);
    const [headerToDelete, setHeaderToDelete] = useState(null);
    const [appId, setAppId] = useState(null);
    const [hostName, setHostName] = useState(null);
    const [isInherited, setIsInherited] = useState(false);

    const isContextSpecific = !!appId || !!hostName;

    useEffect(() => {
        loadHeaders();
        loadOverrideStatus();
    }, [filterHeaderName, filterBehavior, appId, hostName]);

    const handleContextChange = (newAppId, newHostName) => {
        setAppId(newAppId);
        setHostName(newHostName);
    };

    const loadHeaders = async () => {
        const params = {};
        if (filterHeaderName) params.headerName = filterHeaderName;
        if (filterBehavior !== '' && filterBehavior !== 'All') params.behavior = filterBehavior;
        if (appId) params.appId = appId;
        if (hostName) params.hostName = hostName;

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

    const loadOverrideStatus = async () => {
        if (!appId) {
            setIsInherited(false);
            return;
        }

        const params = {};
        if (appId) params.appId = appId;
        if (hostName) params.hostName = hostName;

        await axios.get(import.meta.env.VITE_CUSTOM_HEADER_OVERRIDE_EXISTS, { params })
            .then((response) => {
                setIsInherited(response.data.isInherited ?? false);
            })
            .catch(() => {
                setIsInherited(false);
            });
    };

    const handleCreateOverride = async () => {
        const params = {};
        if (appId) params.appId = appId;
        if (hostName) params.hostName = hostName;

        await axios.post(import.meta.env.VITE_CUSTOM_HEADER_OVERRIDE_CREATE, null, { params })
            .then(() => {
                handleShowSuccessToast('Success', 'Custom header override created successfully.');
                loadHeaders();
                loadOverrideStatus();
            })
            .catch(() => {
                handleShowFailureToast('Failure', 'Failed to create custom header override.');
            });
    };

    const handleRevertToInherited = async () => {
        const params = {};
        if (appId) params.appId = appId;
        if (hostName) params.hostName = hostName;

        await axios.delete(import.meta.env.VITE_CUSTOM_HEADER_OVERRIDE_DELETE, { params })
            .then(() => {
                handleShowSuccessToast('Success', 'Custom headers reverted to inherited configuration.');
                loadHeaders();
                loadOverrideStatus();
            })
            .catch(() => {
                handleShowFailureToast('Failure', 'Failed to revert custom headers to inherited.');
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

    const handleDeleteHeader = (id, headerName) => {
        setHeaderToDelete({ id, headerName });
        setShowDeleteConfirm(true);
    };

    const confirmDelete = async () => {
        if (!headerToDelete) return;

        const { id, headerName } = headerToDelete;
        setShowDeleteConfirm(false);
        setHeaderToDelete(null);

        await axios.delete(import.meta.env.VITE_CUSTOM_HEADER_DELETE, { params: { id } })
            .then(() => {
                handleShowSuccessToast('Success', `Custom header "${headerName}" was successfully deleted.`);
                loadHeaders();
            })
            .catch(() => {
                handleShowFailureToast('Failure', `Failed to delete custom header "${headerName}".`);
            });
    };

    const cancelDelete = () => {
        setShowDeleteConfirm(false);
        setHeaderToDelete(null);
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

    const renderBanners = () => {
        if (!isContextSpecific) {
            return (
                <Alert variant='primary' className='p-3 container-xl'>
                    Custom Headers allow you to add or remove HTTP response headers. Use this feature to manage headers like X-Permitted-Cross-Domain-Policies, Server, X-Powered-By, or any custom headers your application requires.
                </Alert>
            );
        }

        if (isInherited) {
            return (
                <Alert variant='info' className='p-3 container-xl d-flex align-items-center justify-content-between'>
                    <span>These settings are inherited from the parent configuration.</span>
                    <Button variant='primary' size='sm' onClick={handleCreateOverride}>Create Override</Button>
                </Alert>
            );
        }

        return (
            <Alert variant='warning' className='p-3 container-xl d-flex align-items-center justify-content-between'>
                <span>This context has its own custom header overrides.</span>
                <Button variant='outline-danger' size='sm' onClick={handleRevertToInherited}>Revert to Inherited</Button>
            </Alert>
        );
    };

    const renderHeaders = () => {
        if (headers && headers.length > 0) {
            return headers.map((header) => (
                <CustomHeaderCard key={header.headerName} header={header} onEdit={handleEditHeader} onDelete={handleDeleteHeader} isInherited={isContextSpecific && isInherited} />
            ));
        }

        return (
            <Alert variant='warning' className='p-3 container-xl'>No custom headers found for the current filter.</Alert>
        );
    };

    const handleShowSuccessToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(true, title, description);
    const handleShowFailureToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(false, title, description);

    return (
        <>
            <ContextSwitcher appId={appId} hostName={hostName} onContextChange={handleContextChange} />
            {renderBanners()}
            <Container fluid='xl' className='my-3 px-0'>
                <Row>
                    <div className='col-xl-9 col-lg-9 col-sm-12 col-xs-12'>
                        <InputGroup>
                            <InputGroup.Text id='lblHeaderNameFilter'>Name</InputGroup.Text>
                            <Form.Control id='txtHeaderNameFilter' type='text' value={filterHeaderName} onChange={(event) => setFilterHeaderName(event.target.value)} aria-describedby='lblHeaderNameFilter' placeholder='Filter by header name' />
                            <InputGroup.Text id='lblBehaviorFilter'>Behavior</InputGroup.Text>
                            <Form.Select value={filterBehavior} onChange={(event) => setFilterBehavior(event.target.value)} aria-describedby='lblBehaviorFilter'>
                                <option value='All'>All</option>
                                <option value='Enabled'>Enabled</option>
                                <option value='Disabled'>Disabled</option>
                                <option value='Add'>Add</option>
                                <option value='Remove'>Remove</option>
                            </Form.Select>
                        </InputGroup>
                    </div>
                    <div className='col-xl-3 col-lg-3 col-sm-12 col-xs-12 text-end'>
                        {!(isContextSpecific && isInherited) && (
                            <Button variant='success' onClick={handleAddHeader} className='fw-bold'>Add Header</Button>
                        )}
                    </div>
                </Row>
                {renderHeaders()}
            </Container>
            {showModal && (
                <CustomHeaderModal header={selectedHeader} appId={appId} hostName={hostName} show={showModal} onClose={handleModalClose} onSave={handleModalSave} showToastNotificationEvent={props.showToastNotificationEvent} />
            )}
            <ConfirmationModal
                show={showDeleteConfirm}
                title='Delete Header'
                message={`Are you sure you want to delete the custom header "${headerToDelete?.headerName}"?`}
                confirmLabel='Delete'
                onConfirm={confirmDelete}
                onCancel={cancelDelete}
            />
        </>
    );
}

CustomHeadersContainer.propTypes = {
    showToastNotificationEvent: PropTypes.func
};

export default CustomHeadersContainer;
