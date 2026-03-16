import { useState } from 'react';
import PropTypes from 'prop-types';
import axios from 'axios';
import { Button } from 'react-bootstrap';
import PermissionModal from './PermissionModal';
import ConfirmationModal from '../Common/ConfirmationModal';
import DirectivesTable from './DirectivesTable';

function EditPermission(props) {

    const cspOriginalId = props.id;
    const [showEditModal, setShowEditModal] = useState(false);
    const [showDeleteModal, setShowDeleteModal] = useState(false);
    const [cspOriginalSource, setCspOriginalSource] = useState(props.source);
    const [cspOriginalDirectives, setOriginalDirectives] = useState(props.directives);

    const handleReloadSources = () => props.reloadSourceEvent();
    const handleCloseEditModal = () => setShowEditModal(false);
    const handleShowEditModal = () => setShowEditModal(true);
    const handleCloseDeleteModal = () => setShowDeleteModal(false);
    const handleShowDeleteModal = () => setShowDeleteModal(true);
    const handleShowSuccessToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(true, title, description);
    const handleShowFailureToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(false, title, description);
    const handleUpdateDirectives = (updatedDirectives) => {
        setOriginalDirectives(updatedDirectives);
    };

    const handleCommitDelete = () => {
        setShowDeleteModal(false);
        axios.delete(import.meta.env.VITE_PERMISSION_DELETE_URL + cspOriginalId)
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
            <tr key={cspOriginalId}>
                <td>{cspOriginalSource}</td>
                <td data-all-directives={cspOriginalDirectives}>
                    <DirectivesTable directives={cspOriginalDirectives} />
                </td>
                <td>
                    <Button variant='primary' onClick={handleShowEditModal} className="mx-1 text-nowrap">Edit</Button>
                    <Button variant='danger' onClick={handleShowDeleteModal} className="mx-1 text-nowrap">Delete</Button>
                </td>
            </tr>

            {showEditModal && <PermissionModal show={showEditModal} id={cspOriginalId} source={cspOriginalSource} directives={cspOriginalDirectives} closeModalEvent={handleCloseEditModal} updateSourceState={setCspOriginalSource} updateDirectivesState={handleUpdateDirectives} showToastNotificationEvent={props.showToastNotificationEvent} appId={props.appId} hostName={props.hostName} />}
            <ConfirmationModal
                show={showDeleteModal}
                title='Delete Source'
                message={<>Are you sure you want to delete the following source?<p className='fw-bold'>{cspOriginalSource}</p></>}
                confirmLabel='Delete'
                onConfirm={handleCommitDelete}
                onCancel={handleCloseDeleteModal}
            />
        </>
    );
}

EditPermission.propTypes = {
    id: PropTypes.string.isRequired,
    source: PropTypes.string.isRequired,
    directives: PropTypes.string,
    reloadSourceEvent: PropTypes.func,
    showToastNotificationEvent: PropTypes.func,
    appId: PropTypes.string,
    hostName: PropTypes.string
};

export default EditPermission;
