import React, { useState } from 'react';
import { Button } from 'react-bootstrap';
import DeletePermission from './DeletePermission';
import PermissionModal from './PermissionModal';

function EditPermission(props) {
    
    const cspOriginalId = props.id;
    const [showEditModal, setShowEditModal] = useState(false);
    const [cspOriginalSource, setCspOriginalSource] = useState(props.source);
    const [cspOriginalDirectives, setOriginalDirectives] = useState(props.directives);

    const handleReloadSources = () => props.reloadSourceEvent();
    const handleCloseEditModal = () => setShowEditModal(false);
    const handleShowEditModal = () => setShowEditModal(true);

    return (
        <>
            <tr key={cspOriginalId}>
                <td>{cspOriginalSource}</td>
                <td>{cspOriginalDirectives}</td>
                <td>
                    <Button variant='primary' onClick={handleShowEditModal} className="mx-1">Edit</Button>
                    <DeletePermission id={cspOriginalId} source={cspOriginalSource} reloadSources={handleReloadSources} showToastNotificationEvent={props.showToastNotificationEvent}></DeletePermission>
                </td>
            </tr>

            {showEditModal ? <PermissionModal show={showEditModal} id={cspOriginalId} source={cspOriginalSource} directives={cspOriginalDirectives} closeModalEvent={handleCloseEditModal} updateSourceState={setCspOriginalSource} updateDirectivesState={setOriginalDirectives} showToastNotificationEvent={props.showToastNotificationEvent}></PermissionModal> : null}
        </>
    )
}

export default EditPermission;