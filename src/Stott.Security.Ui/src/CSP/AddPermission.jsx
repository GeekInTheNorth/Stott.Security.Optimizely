import { useState } from 'react';
import PropTypes from 'prop-types';
import { Button } from 'react-bootstrap';
import PermissionModal from './PermissionModal';

function AddPermission(props) {
    
    const [showCreateModal, setShowCreateModal] = useState(false);
    
    const handleCloseCreateModal = () => setShowCreateModal(false);
    const handleShowCreateModal = () => setShowCreateModal(true);

    return (
        <>
            <Button variant='success' className='fw-bold' onClick={handleShowCreateModal}>Add Source</Button>
            {showCreateModal ? <PermissionModal show={showCreateModal} id='00000000-0000-0000-0000-000000000000' source='' directives='' reloadSourceEvent={props.reloadSourceEvent} closeModalEvent={handleCloseCreateModal} showToastNotificationEvent={props.showToastNotificationEvent}></PermissionModal> : null}
        </>
    )
}

AddPermission.propTypes = {
    reloadSourceEvent: PropTypes.func,
    showToastNotificationEvent: PropTypes.func
};

export default AddPermission
