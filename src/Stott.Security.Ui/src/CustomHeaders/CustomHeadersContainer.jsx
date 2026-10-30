import { useState, useEffect } from 'react';
import PropTypes from 'prop-types';
import { httpGet, httpPost, httpDelete } from '../Common/httpClient';
import { Alert, Box, Button, Field, Flex, Grid, Group, Input, Text } from '@optiaxiom/react';
import FormSelect from '../Common/FormSelect';
import CustomHeaderModal from './CustomHeaderModal';
import CustomHeaderCard from './CustomHeaderCard';
import ConfirmationModal from '../Common/ConfirmationModal';
import ContextSwitcher from '../Common/ContextSwitcher';

const behaviorFilterOptions = [
    { label: 'All', value: 'All' },
    { label: 'Enabled', value: 'Enabled' },
    { label: 'Disabled', value: 'Disabled' },
    { label: 'Add', value: 'Add' },
    { label: 'Remove', value: 'Remove' }
];

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

        await httpGet(import.meta.env.VITE_CUSTOM_HEADER_LIST, params)
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

        await httpGet(import.meta.env.VITE_CUSTOM_HEADER_OVERRIDE_EXISTS, params)
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

        await httpPost(import.meta.env.VITE_CUSTOM_HEADER_OVERRIDE_CREATE, null, params)
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

        await httpDelete(import.meta.env.VITE_CUSTOM_HEADER_OVERRIDE_DELETE, params)
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

        await httpDelete(import.meta.env.VITE_CUSTOM_HEADER_DELETE, { id })
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
                <Alert intent='information' my='16'>
                    Custom Headers allow you to add or remove HTTP response headers. Use this feature to manage headers like X-Permitted-Cross-Domain-Policies, Server, X-Powered-By, or any custom headers your application requires.
                </Alert>
            );
        }

        if (isInherited) {
            return (
                <Alert intent='information' my='16'>
                    <Group justifyContent='space-between' gap='16' w='full'>
                        <Text>These settings are inherited from the parent configuration.</Text>
                        <Button appearance='primary' size='sm' onClick={handleCreateOverride}>Create Override</Button>
                    </Group>
                </Alert>
            );
        }

        return (
            <Alert intent='warning' my='16'>
                <Group justifyContent='space-between' gap='16' w='full'>
                    <Text>This context has its own custom header overrides.</Text>
                    <Button appearance='danger-outline' size='sm' onClick={handleRevertToInherited}>Revert to Inherited</Button>
                </Group>
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
            <Alert intent='warning' my='16'>No custom headers found for the current filter.</Alert>
        );
    };

    const handleShowSuccessToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(true, title, description);
    const handleShowFailureToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(false, title, description);

    return (
        <>
            <ContextSwitcher appId={appId} hostName={hostName} onContextChange={handleContextChange} />
            <Box className='security-page-container'>
                {renderBanners()}
                <Flex flexDirection={{ base: 'column', md: 'row' }} alignItems={{ base: 'stretch', md: 'end' }} gap='16' my='16'>
                    <Grid gridTemplateColumns={{ base: '1', md: '2' }} gap='16' flex='1'>
                        <Field label='Name'>
                            <Input id='txtHeaderNameFilter' type='text' value={filterHeaderName} onChange={(event) => setFilterHeaderName(event.target.value)} placeholder='Filter by header name' />
                        </Field>
                        <Field label='Behavior'>
                            <FormSelect value={filterBehavior} onValueChange={setFilterBehavior} options={behaviorFilterOptions} />
                        </Field>
                    </Grid>
                    {!(isContextSpecific && isInherited) && (
                        <Button appearance='primary' onClick={handleAddHeader}>Add Header</Button>
                    )}
                </Flex>
                {renderHeaders()}
            </Box>
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
