import { useState } from 'react';
import PropTypes from 'prop-types';
import { httpDelete } from '../Common/httpClient';
import { Button, Group, TableCell, TableRow, Text } from '@optiaxiom/react';
import PermissionModal from './PermissionModal';
import ConfirmationModal from '../Common/ConfirmationModal';
import DirectivesTable from './DirectivesTable';

function EditPermission({ sourceData, reloadSourceEvent, showToastNotificationEvent }) {

    const cspOriginalId = sourceData.id;
    const [showEditModal, setShowEditModal] = useState(false);
    const [showDeleteModal, setShowDeleteModal] = useState(false);
    const [cspOriginalSource, setCspOriginalSource] = useState(sourceData.source);
    const [cspOriginalDirectives, setOriginalDirectives] = useState(sourceData.directives);

    const handleReloadSources = () => reloadSourceEvent();
    const handleCloseEditModal = () => setShowEditModal(false);
    const handleShowEditModal = () => setShowEditModal(true);
    const handleCloseDeleteModal = () => setShowDeleteModal(false);
    const handleShowDeleteModal = () => setShowDeleteModal(true);
    const handleShowSuccessToast = (title, description) => showToastNotificationEvent && showToastNotificationEvent(true, title, description);
    const handleShowFailureToast = (title, description) => showToastNotificationEvent && showToastNotificationEvent(false, title, description);
    const handleUpdateDirectives = (updatedDirectives) => setOriginalDirectives(updatedDirectives);

    const handleCommitDelete = () => {
        setShowDeleteModal(false);
        httpDelete(import.meta.env.VITE_PERMISSION_DELETE_URL + cspOriginalId)
            .then(() => {
                handleShowSuccessToast('Source Deleted', `Successfully deleted the source: ${cspOriginalSource}`);
                handleReloadSources();
            },
            () => {
                handleShowFailureToast('Error', `Failed to delete the source: ${cspOriginalSource}`);
            });
    };

    return (
        <>
            <TableRow key={cspOriginalId}>
                <TableCell>{cspOriginalSource}</TableCell>
                <TableCell data-all-directives={cspOriginalDirectives}>
                    <DirectivesTable directives={cspOriginalDirectives} />
                </TableCell>
                <TableCell>
                    <Group gap='8'>
                        <Button appearance='primary' onClick={handleShowEditModal}>Edit</Button>
                        <Button appearance='danger' onClick={handleShowDeleteModal}>Delete</Button>
                    </Group>
                </TableCell>
            </TableRow>

            {showEditModal && <PermissionModal show={showEditModal} id={cspOriginalId} source={cspOriginalSource} directives={cspOriginalDirectives} closeModalEvent={handleCloseEditModal} updateSourceState={setCspOriginalSource} updateDirectivesState={handleUpdateDirectives} showToastNotificationEvent={showToastNotificationEvent} appId={sourceData.appId} hostName={sourceData.hostName} />}
            <ConfirmationModal
                show={showDeleteModal}
                title='Delete Source'
                message={<>Are you sure you want to delete the following source?<Text fontWeight='600'>{cspOriginalSource}</Text></>}
                confirmLabel='Delete'
                onConfirm={handleCommitDelete}
                onCancel={handleCloseDeleteModal}
            />
        </>
    );
}

EditPermission.propTypes = {
    sourceData: PropTypes.shape({
        id: PropTypes.string.isRequired,
        source: PropTypes.string.isRequired,
        directives: PropTypes.string,
        appId: PropTypes.string,
        hostName: PropTypes.string,
        isInherited: PropTypes.bool,
        isDescendant: PropTypes.bool,
        inheritedLabel: PropTypes.string,
        descendantLabel: PropTypes.string
    }).isRequired,
    reloadSourceEvent: PropTypes.func,
    showToastNotificationEvent: PropTypes.func,
};

export default EditPermission;
