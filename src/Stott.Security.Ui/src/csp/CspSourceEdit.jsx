import { useState } from 'react';
import { Button } from 'react-bootstrap';
import PropTypes from 'prop-types';
import CspSourceDelete from './CspSourceDelete';
import CspSourceModal from './CspSourceModal';

function CspSourceEdit(props) {
    const getDirectivesList = (directives) => {
        return directives ? directives.split(",") : [];
    };

    const cspOriginalId = props.id;
    const [showEditModal, setShowEditModal] = useState(false);
    const [cspOriginalSource, setCspOriginalSource] = useState(props.source);
    const [cspOriginalDirectives, setOriginalDirectives] = useState(props.directives);
    const [allPermissions, setAllPermissions] = useState(getDirectivesList(props.directives));

    const handleReloadSources = () => props.reloadSourceEvent && props.reloadSourceEvent();
    const handleCloseEditModal = () => setShowEditModal(false);
    const handleShowEditModal = () => setShowEditModal(true);
    const handleUpdateDirectives = (updatedDirectives) => {
        setOriginalDirectives(updatedDirectives);
        setAllPermissions(getDirectivesList(updatedDirectives));
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
                        {hasDirective('base-uri') && <tr><td className='directive text-nowrap align-top fw-bold'>base-uri</td><td className='directive-description'>Allows this source to be used within the base element for this site.</td></tr>}
                        {hasDirective('default-src') && <tr><td className='directive text-nowrap align-top fw-bold'>default-src</td><td className='directive-description'>Allows this source by default unless one or more sources are defined for a specific permission.</td></tr>}
                        {hasDirective('child-src') && <tr><td className='directive text-nowrap align-top fw-bold'>child-src</td><td className='directive-description'>Can contain this source in an iframe or use web workers it provides.</td></tr>}
                        {hasDirective('frame-src') && <tr><td className='directive text-nowrap align-top fw-bold'>frame-src</td><td className='directive-description'>Can contain this source in an iframe on this site.</td></tr>}
                        {hasDirective('frame-ancestors') && <tr><td className='directive text-nowrap align-top fw-bold'>frame-ancestors</td><td className='directive-description'>This source can contain this site in an iframe.</td></tr>}
                        {hasDirective('connect-src') && <tr><td className='directive text-nowrap align-top fw-bold'>connect-src</td><td className='directive-description'>Allows links and data requests to this source.</td></tr>}
                        {hasDirective('form-action') && <tr><td className='directive text-nowrap align-top fw-bold'>form-action</td><td className='directive-description'>Can use this source within a form action.</td></tr>}
                        {hasDirective('font-src') && <tr><td className='directive text-nowrap align-top fw-bold'>font-src</td><td className='directive-description'>Can use fonts from this source.</td></tr>}
                        {hasDirective('img-src') && <tr><td className='directive text-nowrap align-top fw-bold'>img-src</td><td className='directive-description'>Can use images from this source.</td></tr>}
                        {hasDirective('media-src') && <tr><td className='directive text-nowrap align-top fw-bold'>media-src</td><td className='directive-description'>Can use audio and video files from this source.</td></tr>}
                        {hasDirective('object-src') && <tr><td className='directive text-nowrap align-top fw-bold'>object-src</td><td className='directive-description'>Allows content from this source to be used in applet, embed and object elements.</td></tr>}
                        {hasDirective('manifest-src') && <tr><td className='directive text-nowrap align-top fw-bold'>manifest-src</td><td className='directive-description'>Allows this source to be provide a manifest for this site.</td></tr>}
                        {hasDirective('script-src') && <tr><td className='directive text-nowrap align-top fw-bold'>script-src</td><td className='directive-description'>Can use javascript from this source.</td></tr>}
                        {hasDirective('script-src-elem') && <tr><td className='directive text-nowrap align-top fw-bold'>script-src-elem</td><td className='directive-description'>Can use javascript from this source to be used within a script tag.</td></tr>}
                        {hasDirective('script-src-attr') && <tr><td className='directive text-nowrap align-top fw-bold'>script-src-attr</td><td className='directive-description'>Can use javascript from this source to be used within inline javascript events.</td></tr>}
                        {hasDirective('worker-src') && <tr><td className='directive text-nowrap align-top fw-bold'>worker-src</td><td className='directive-description'>Can use Worker, SharedWorker and ServiceWorker scripts from this source.</td></tr>}
                        {hasDirective('style-src') && <tr><td className='directive text-nowrap align-top fw-bold'>style-src</td><td className='directive-description'>Can use styles from this source.</td></tr>}
                        {hasDirective('style-src-elem') && <tr><td className='directive text-nowrap align-top fw-bold'>style-src-elem</td><td className='directive-description'>Can use styles from this source within a style tag.</td></tr>}
                        {hasDirective('style-src-attr') && <tr><td className='directive text-nowrap align-top fw-bold'>style-src-attr</td><td className='directive-description'>Can use styles from this source within inline elements.</td></tr>}
                    </table>
                </td>
                <td>
                    <Button variant='primary' onClick={handleShowEditModal} className="mx-1 text-nowrap">Edit</Button>
                    <CspSourceDelete id={cspOriginalId} source={cspOriginalSource} reloadSources={handleReloadSources} showToastNotificationEvent={props.showToastNotificationEvent} />
                </td>
            </tr>

            {showEditModal && (
                <CspSourceModal
                    show={showEditModal}
                    id={cspOriginalId}
                    source={cspOriginalSource}
                    directives={cspOriginalDirectives}
                    closeModalEvent={handleCloseEditModal}
                    updateSourceState={setCspOriginalSource}
                    updateDirectivesState={handleUpdateDirectives}
                    showToastNotificationEvent={props.showToastNotificationEvent}
                />
            )}
        </>
    );
}

CspSourceEdit.propTypes = {
    id: PropTypes.string.isRequired,
    source: PropTypes.string.isRequired,
    directives: PropTypes.string.isRequired,
    showToastNotificationEvent: PropTypes.func.isRequired,
    reloadSourceEvent: PropTypes.func.isRequired
};

export default CspSourceEdit;
