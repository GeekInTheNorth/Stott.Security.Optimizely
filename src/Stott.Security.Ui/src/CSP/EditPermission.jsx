import { useState } from 'react';
import PropTypes from 'prop-types';
import axios from 'axios';
import { Button } from 'react-bootstrap';
import PermissionModal from './PermissionModal';
import ConfirmationModal from '../Common/ConfirmationModal';

function EditPermission(props) {

    const getDirectivesList = (directives) => {
        return directives ? directives.split(",") : [];
    };

    const cspOriginalId = props.id;
    const [showEditModal, setShowEditModal] = useState(false);
    const [showDeleteModal, setShowDeleteModal] = useState(false);
    const [cspOriginalSource, setCspOriginalSource] = useState(props.source);
    const [cspOriginalDirectives, setOriginalDirectives] = useState(props.directives);
    const [allPermissions, setAllPermissions] = useState(getDirectivesList(props.directives));

    const handleReloadSources = () => props.reloadSourceEvent();
    const handleCloseEditModal = () => setShowEditModal(false);
    const handleShowEditModal = () => setShowEditModal(true);
    const handleCloseDeleteModal = () => setShowDeleteModal(false);
    const handleShowDeleteModal = () => setShowDeleteModal(true);
    const handleShowSuccessToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(true, title, description);
    const handleShowFailureToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(false, title, description);
    const handleUpdateDirectives = (updatedDirectives) => {
        setOriginalDirectives(updatedDirectives);
        setAllPermissions(getDirectivesList(updatedDirectives));
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

    const hasDirective = (directive) => {
        return allPermissions.indexOf(directive) >= 0;
    };

    return (
        <>
            <tr key={cspOriginalId}>
                <td>{cspOriginalSource}</td>
                <td data-all-directives={cspOriginalDirectives}>
                    <table className="table-permissions">
                        {hasDirective('base-uri') ? <tr><td className='directive text-nowrap align-top fw-bold'>base-uri</td><td className='directive-description'>Allows this source to be used within the base element for this site.</td></tr> : null}
                        {hasDirective('default-src') ? <tr><td className='directive text-nowrap align-top fw-bold'>default-src</td><td className='directive-description'>Allows this source by default unless one or more sources are defined for a specific permission.</td></tr> : null}
                        {hasDirective('child-src') ? <tr><td className='directive text-nowrap align-top fw-bold'>child-src</td><td className='directive-description'>Can contain this source in an iframe or use web workers it provides.</td></tr> : null} 
                        {hasDirective('frame-src') ? <tr><td className='directive text-nowrap align-top fw-bold'>frame-src</td><td className='directive-description'>Can contain this source in an iframe on this site.</td></tr> : null}
                        {hasDirective('frame-ancestors') ? <tr><td className='directive text-nowrap align-top fw-bold'>frame-ancestors</td><td className='directive-description'>This source can contain this site in an iframe.</td></tr> : null}
                        {hasDirective('connect-src') ? <tr><td className='directive text-nowrap align-top fw-bold'>connect-src</td><td className='directive-description'>Allows links and data requests to this source.</td></tr> : null} 
                        {hasDirective('form-action') ? <tr><td className='directive text-nowrap align-top fw-bold'>form-action</td><td className='directive-description'>Can use this source within a form action.</td></tr> : null} 
                        {hasDirective('font-src') ? <tr><td className='directive text-nowrap align-top fw-bold'>font-src</td><td className='directive-description'>Can use fonts from this source.</td></tr> : null} 
                        {hasDirective('img-src') ? <tr><td className='directive text-nowrap align-top fw-bold'>img-src</td><td className='directive-description'>Can use images from this source.</td></tr> : null}
                        {hasDirective('media-src') ? <tr><td className='directive text-nowrap align-top fw-bold'>media-src</td><td className='directive-description'>Can use audio and video files from this source.</td></tr> : null}
                        {hasDirective('object-src') ? <tr><td className='directive text-nowrap align-top fw-bold'>object-src</td><td className='directive-description'>Allows content from this source to be used in applet, embed and object elements.</td></tr> : null}
                        {hasDirective('manifest-src') ? <tr><td className='directive text-nowrap align-top fw-bold'>manifest-src</td><td className='directive-description'>Allows this source to be provide a manifest for this site.</td></tr> : null} 
                        {hasDirective('script-src') ? <tr><td className='directive text-nowrap align-top fw-bold'>script-src</td><td className='directive-description'>Can use javascript from this source.</td></tr> : null}
                        {hasDirective('script-src-elem') ? <tr><td className='directive text-nowrap align-top fw-bold'>script-src-elem</td><td className='directive-description'>Can use javascript from this source to be used within a script tag.</td></tr> : null}
                        {hasDirective('script-src-attr') ? <tr><td className='directive text-nowrap align-top fw-bold'>script-src-attr</td><td className='directive-description'>Can use javascript from this source to be used within inline javascript events.</td></tr> : null}
                        {hasDirective('worker-src') ? <tr><td className='directive text-nowrap align-top fw-bold'>worker-src</td><td className='directive-description'>Can use Worker, SharedWorker and ServiceWorker scripts from this source.</td></tr> : null}
                        {hasDirective('style-src') ? <tr><td className='directive text-nowrap align-top fw-bold'>style-src</td><td className='directive-description'>Can use styles from this source.</td></tr> : null}
                        {hasDirective('style-src-elem') ? <tr><td className='directive text-nowrap align-top fw-bold'>style-src-elem</td><td className='directive-description'>Can use styles from this source within a style tag.</td></tr> : null}
                        {hasDirective('style-src-attr') ? <tr><td className='directive text-nowrap align-top fw-bold'>style-src-attr</td><td className='directive-description'>Can use styles from this source within inline elements.</td></tr> : null}
                    </table>
                </td>
                <td>
                    <Button variant='primary' onClick={handleShowEditModal} className="mx-1 text-nowrap">Edit</Button>
                    <Button variant='danger' onClick={handleShowDeleteModal} className="mx-1 text-nowrap">Delete</Button>
                </td>
            </tr>

            {showEditModal && <PermissionModal show={showEditModal} id={cspOriginalId} source={cspOriginalSource} directives={cspOriginalDirectives} closeModalEvent={handleCloseEditModal} updateSourceState={setCspOriginalSource} updateDirectivesState={handleUpdateDirectives} showToastNotificationEvent={props.showToastNotificationEvent} />}
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
    showToastNotificationEvent: PropTypes.func
};

export default EditPermission;
