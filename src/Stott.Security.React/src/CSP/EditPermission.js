import React, { useState, useEffect } from 'react';
import { Button } from 'react-bootstrap';
import DeletePermission from './DeletePermission';
import PermissionModal from './PermissionModal';

function EditPermission(props) {
    
    const cspOriginalId = props.id;
    const [showEditModal, setShowEditModal] = useState(false);
    const [cspOriginalSource, setCspOriginalSource] = useState(props.source);
    const [cspOriginalDirectives, setOriginalDirectives] = useState(props.directives);
    const [allPermissions, setAllPermissions] = useState([])

    const handleReloadSources = () => props.reloadSourceEvent();
    const handleCloseEditModal = () => setShowEditModal(false);
    const handleShowEditModal = () => setShowEditModal(true);
    const handleUpdateDirectives = (updatedDirectives) => {
        setOriginalDirectives(updatedDirectives);
        setAllPermissions(getDirectivesList(updatedDirectives));
    };

    const getDirectivesList = (directives) => {
        return directives ? directives.split(",") : []
    }

    const hasDirective = (directive) => {
        return allPermissions.indexOf(directive) >= 0;
    };

    useEffect(() => {
        setAllPermissions(getDirectivesList(cspOriginalDirectives));
    }, [])

    return (
        <>
            <tr key={cspOriginalId}>
                <td>{cspOriginalSource}</td>
                <td data-all-directives={cspOriginalDirectives}>
                    <ul>
                        {hasDirective('base-uri') ? <li>Allows this source to be used within the base element for this site. <em>(base-uri)</em></li> : null}
                        {hasDirective('default-src') ? <li>Allows this source by default unless one or more sources are defined for a specific permission. <em>(default-src)</em></li> : null}
                        {hasDirective('child-src') ? <li>Can contain this source in an iframe or use web workers it provides. <em>(child-src)</em></li> : null} 
                        {hasDirective('frame-src') ? <li>Can contain this source in an iframe on this site. <em>(frame-src)</em></li> : null}
                        {hasDirective('frame-ancestors') ? <li>This source can contain this site in an iframe. <em>(frame-ancestors)</em></li> : null}
                        {hasDirective('connect-src') ? <li>Allows links and data requests to this source. <em>(connect-src)</em></li> : null} 
                        {hasDirective('navigate-to') ? <li>Can initiate a navigation to this source from a link, form or javascript action. <em>(navigate-to)</em></li> : null} 
                        {hasDirective('form-action') ? <li>Can use this source within a form action. <em>(form-action)</em></li> : null} 
                        {hasDirective('font-src') ? <li>Can use fonts from this source. <em>(font-src)</em></li> : null} 
                        {hasDirective('img-src') ? <li>Can use images from this source. <em>(img-src)</em></li> : null}
                        {hasDirective('media-src') ? <li>Can use audio and video files from this source. <em>(media-src)</em></li> : null}
                        {hasDirective('object-src') ? <li>Allows content from this source to be used in applet, embed and object elements. <em>(object-src)</em></li> : null}
                        {hasDirective('manifest-src') ? <li>Allows this source to be provide a manifest for this site. <em>(manifest-src)</em></li> : null} 
                        {hasDirective('prefetch-src') ? <li>Allows content from this source to be prefetched or prerendered. <em>(prefetch-src)</em></li> : null} 
                        {hasDirective('script-src') ? <li>Can use javascript from this source. <em>(script-src)</em></li> : null}
                        {hasDirective('script-src-elem') ? <li>Can use javascript from this source to be used within a script tag. <em>(script-src-elem)</em></li> : null}
                        {hasDirective('script-src-attr') ? <li>Can use javascript from this source to be used within inline javascript events. <em>(script-src-attr)</em></li> : null}
                        {hasDirective('style-src') ? <li>Can use styles from this source. <em>(style-src)</em></li> : null}
                        {hasDirective('style-src-elem') ? <li>Can use styles from this source within a style or link tag. <em>(style-src-elem)</em></li> : null}
                        {hasDirective('style-src-attr') ? <li>Can use styles from this source within inline elements. <em>(style-src-attr)</em></li> : null}
                        {hasDirective('worker-src') ? <li>Can use Worker, SharedWorker and ServiceWorker scripts from this source. <em>(worker-src)</em></li> : null}
                    </ul>
                </td>
                <td>
                    <Button variant='primary' onClick={handleShowEditModal} className="mx-1">Edit</Button>
                    <DeletePermission id={cspOriginalId} source={cspOriginalSource} reloadSources={handleReloadSources} showToastNotificationEvent={props.showToastNotificationEvent}></DeletePermission>
                </td>
            </tr>

            {showEditModal ? <PermissionModal show={showEditModal} id={cspOriginalId} source={cspOriginalSource} directives={cspOriginalDirectives} closeModalEvent={handleCloseEditModal} updateSourceState={setCspOriginalSource} updateDirectivesState={handleUpdateDirectives} showToastNotificationEvent={props.showToastNotificationEvent}></PermissionModal> : null}
        </>
    )
}

export default EditPermission;