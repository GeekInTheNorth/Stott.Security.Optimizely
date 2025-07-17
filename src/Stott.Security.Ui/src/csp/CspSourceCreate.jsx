import { useState } from 'react';
import { Button } from 'react-bootstrap';
import PropTypes from 'prop-types';
import CspSourceModal from './CspSourceModal';

function CspSourceCreate(props) {
    
    const [showCreateModal, setShowCreateModal] = useState(false);
    
    const handleCloseCreateModal = () => setShowCreateModal(false);
    const handleShowCreateModal = () => setShowCreateModal(true);

    return (
        <>
            <Button variant='success' className='fw-bold' onClick={handleShowCreateModal}>Add Source</Button>
            {showCreateModal ? <CspSourceModal show={showCreateModal} id='00000000-0000-0000-0000-000000000000' source='' directives='' reloadSourceEvent={props.reloadSourceEvent} closeModalEvent={handleCloseCreateModal} showToastNotificationEvent={props.showToastNotificationEvent}></CspSourceModal> : null}
        </>
    )
}

CspSourceCreate.propTypes = {
    reloadSourceEvent: PropTypes.func.isRequired,
    showToastNotificationEvent: PropTypes.func.isRequired
};

export default CspSourceCreate