import React, { useState } from "react";
import { Modal, Form, Button } from "react-bootstrap";
import axios from 'axios';

function EditPermission(props) {
    const [showModal, setShowModal] = useState(false);
    const [cspSourceId, setCspSourceId] = useState(props.id);
    const [cspSourceSource, setCspSourceSource] = useState(props.source);
    const [cspSourceDirectives, setCspSourceDirectives] = useState(props.directives);
    const [cspDirectiveBaseUri, setCspDirectiveBaseUri] =  useState(false);
    const [cspDirectiveChildSource, setCspDirectiveChildSource] =  useState(false);
    const [cspDirectiveConnectSource, setCspDirectiveConnectSource] =  useState(false);
    const [cspDirectiveDefaultSource, setCspDirectiveDefaultSource] =  useState(false);
    const [cspDirectiveFontSource, setCspDirectiveFontSource] =  useState(false);
    const [cspDirectiveFormAction, setCspDirectiveFormAction] =  useState(false);
    const [cspDirectiveFrameAncestors, setCspDirectiveFrameAncestors] =  useState(false);
    const [cspDirectiveFrameSource, setCspDirectiveFrameSource] =  useState(false);
    const [cspDirectiveImageSource, setCspDirectiveImageSource] =  useState(false);
    const [cspDirectiveManifestSource, setCspDirectiveManifestSource] =  useState(false);
    const [cspDirectiveMediaSource, setCspDirectiveMediaSource] =  useState(false);
    const [cspDirectiveNavigateTo, setCspDirectiveNavigateTo] =  useState(false);
    const [cspDirectiveObjectSource, setCspDirectiveObjectSource] =  useState(false);
    const [cspDirectivePreFetchSource, setCspDirectivePreFetchSource] =  useState(false);
    const [cspDirectiveRequireTrustedTypes, setCspDirectiveRequireTrustedTypes] =  useState(false);
    const [cspDirectiveSandbox, setCspDirectiveSandbox] =  useState(false);
    const [cspDirectiveScriptSourceAttribute, setCspDirectiveScriptSourceAttribute] =  useState(false);
    const [cspDirectiveScriptSourceElement, setCspDirectiveScriptSourceElement] =  useState(false);
    const [cspDirectiveScriptSource, setCspDirectiveScriptSource] =  useState(false);
    const [cspDirectiveStyleSourceAttribute, setCspDirectiveStyleSourceAttribute] =  useState(false);
    const [cspDirectiveStyleSourceElement, setCspDirectiveStyleSourceElement] =  useState(false);
    const [cspDirectiveStyleSource, setCspDirectiveStyleSource] =  useState(false);
    const [cspDirectiveTrustedTypes, setCspDirectiveTrustedTypes] =  useState(false);
    const [cspDirectiveUpgradeInsecureRequests, setCspDirectiveUpgradeInsecureRequests] =  useState(false);
    const [cspDirectiveWorkerSource, setCspDirectiveWorkerSource] =  useState(false);

    const hasDirective = (directive) => {
        return cspSourceDirectives.indexOf(directive) >= 0;// ? 'checked' : '';
    };

    const handleSourceChange = (event) => setCspSourceSource(event.target.value);
    const handleSetCspDirectiveBaseUri = (event) => setCspDirectiveBaseUri(event.target.checked);
    const handleClose = () => setShowModal(false);
    const handleShow = () => {
        setShowModal(true);
        setCspDirectiveBaseUri(hasDirective('base-uri'));
        setCspDirectiveChildSource(hasDirective('child-src'));
        setCspDirectiveConnectSource(hasDirective('connect-src'));
        setCspDirectiveDefaultSource(hasDirective('default-src'));
        setCspDirectiveFontSource(hasDirective('font-src'));
        setCspDirectiveFormAction(hasDirective('form-action'));
        setCspDirectiveFrameAncestors(hasDirective('frame-ancestors'));
        setCspDirectiveFrameSource(hasDirective('frame-src'));
        setCspDirectiveImageSource(hasDirective('img-src'));
        setCspDirectiveManifestSource(hasDirective('manifest-src'));
        setCspDirectiveMediaSource(hasDirective('media-src'));
        setCspDirectiveNavigateTo(hasDirective('navigate-to'));
        setCspDirectiveObjectSource(hasDirective('object-src'));
        setCspDirectivePreFetchSource(hasDirective('prefetch-src'));
        setCspDirectiveRequireTrustedTypes(hasDirective('require-trusted-types-for'));
        setCspDirectiveSandbox(hasDirective('sandbox'));
        setCspDirectiveScriptSourceAttribute(hasDirective('script-src-attr'));
        setCspDirectiveScriptSourceElement(hasDirective('script-src-elem'));
        setCspDirectiveScriptSource(hasDirective('script-src'));
        setCspDirectiveStyleSourceAttribute(hasDirective('style-src-attr'));
        setCspDirectiveStyleSourceElement(hasDirective('style-src-elem'));
        setCspDirectiveStyleSource(hasDirective('style-src'));
        setCspDirectiveTrustedTypes(hasDirective('trusted-types'));
        setCspDirectiveUpgradeInsecureRequests(hasDirective('upgrade-insecure-requests'));
        setCspDirectiveWorkerSource(hasDirective('worker-src'));
    };

    const handleSubmit = (event) => {
        event.preventDefault();
        let newDirectives = [];
        if (cspDirectiveBaseUri === true) { newDirectives.push('base-uri'); }
        if (cspDirectiveChildSource === true) { newDirectives.push('child-src'); }
        if (cspDirectiveConnectSource === true) { newDirectives.push('connect-src'); }
        if (cspDirectiveDefaultSource === true) { newDirectives.push('default-src'); }
        if (cspDirectiveFontSource === true) { newDirectives.push('font-src'); }
        if (cspDirectiveFormAction === true) { newDirectives.push('form-action'); }
        if (cspDirectiveFrameAncestors === true) { newDirectives.push('frame-ancestors'); }
        if (cspDirectiveFrameSource === true) { newDirectives.push('frame-src'); }
        if (cspDirectiveImageSource === true) { newDirectives.push('img-src'); }
        if (cspDirectiveManifestSource === true) { newDirectives.push('manifest-src'); }
        if (cspDirectiveMediaSource === true) { newDirectives.push('media-src'); }
        if (cspDirectiveNavigateTo === true) { newDirectives.push('navigate-to'); }
        if (cspDirectiveObjectSource === true) { newDirectives.push('object-src'); }
        if (cspDirectivePreFetchSource === true) { newDirectives.push('prefetch-src'); }
        if (cspDirectiveRequireTrustedTypes === true) { newDirectives.push('require-trusted-types-for'); }
        if (cspDirectiveSandbox === true) { newDirectives.push('sandbox'); }
        if (cspDirectiveScriptSourceAttribute === true) { newDirectives.push('script-src-attr'); }
        if (cspDirectiveScriptSourceElement === true) { newDirectives.push('script-src-elem'); }
        if (cspDirectiveScriptSource === true) { newDirectives.push('script-src'); }
        if (cspDirectiveStyleSourceAttribute === true) { newDirectives.push('style-src-attr'); }
        if (cspDirectiveStyleSourceElement === true) { newDirectives.push('style-src-elem'); }
        if (cspDirectiveStyleSource === true) { newDirectives.push('style-src'); }
        if (cspDirectiveTrustedTypes === true) { newDirectives.push('trusted-types'); }
        if (cspDirectiveUpgradeInsecureRequests === true) { newDirectives.push('upgrade-insecure-requests'); }
        if (cspDirectiveWorkerSource === true) { newDirectives.push('worker-src'); }

        setCspSourceDirectives(newDirectives.join(','));

        let params = new URLSearchParams();
        params.append('id', cspSourceId );
        params.append('source', cspSourceSource );
        for (var i = 0; i < newDirectives.length; i++) {
            params.append('directives', newDirectives[i]);
        }
        axios.post('https://localhost:44344/CspPermissions/Save/', params);
    };

    return (
        <>
            <tr key={cspSourceId}>
                <td>{cspSourceSource}</td>
                <td>{cspSourceDirectives}</td>
                <td>
                    <button className='btn btn-primary' type='button' onClick={handleShow}>Edit</button>
                </td>
            </tr>

            <Modal show={showModal} onHide={handleClose}>
                <Form>
                    <Modal.Header closeButton>
                        <Modal.Title>Edit Source Directives</Modal.Title>
                    </Modal.Header>
                    <Modal.Body>
                        <Form.Group className='mb-3' controlId='formSource'>
                            <Form.Label>Source</Form.Label>
                            <Form.Control type='text' placeholder='Enter source' value={cspSourceSource} onChange={handleSourceChange} />
                        </Form.Group>
                        <Form.Group className='mt-3'>
                            <Form.Label>Directives</Form.Label>
                            <Form.Check type='checkbox' label='base-uri' className='form-check--halfwidth' checked={cspDirectiveBaseUri} onClick={handleSetCspDirectiveBaseUri} />
                            <Form.Check type='checkbox' label='child-src' className='form-check--halfwidth' checked={cspDirectiveChildSource} />
                            <Form.Check type='checkbox' label='connect-src' className='form-check--halfwidth' checked={cspDirectiveConnectSource} />
                            <Form.Check type='checkbox' label='default-src' className='form-check--halfwidth' checked={cspDirectiveDefaultSource} />
                            <Form.Check type='checkbox' label='font-src' className='form-check--halfwidth' checked={cspDirectiveFontSource} />
                            <Form.Check type='checkbox' label='form-action' className='form-check--halfwidth' checked={cspDirectiveFormAction} />
                            <Form.Check type='checkbox' label='frame-ancestors' className='form-check--halfwidth' checked={cspDirectiveFrameAncestors} />
                            <Form.Check type='checkbox' label='frame-src' className='form-check--halfwidth' checked={cspDirectiveFrameSource} />
                            <Form.Check type='checkbox' label='img-src' className='form-check--halfwidth' checked={cspDirectiveImageSource} />
                            <Form.Check type='checkbox' label='manifest-src' className='form-check--halfwidth' checked={cspDirectiveManifestSource} />
                            <Form.Check type='checkbox' label='media-src' className='form-check--halfwidth' checked={cspDirectiveMediaSource} />
                            <Form.Check type='checkbox' label='navigate-to' className='form-check--halfwidth' checked={cspDirectiveNavigateTo} />
                            <Form.Check type='checkbox' label='object-src' className='form-check--halfwidth' checked={cspDirectiveObjectSource} />
                            <Form.Check type='checkbox' label='prefetch-src' className='form-check--halfwidth' checked={cspDirectivePreFetchSource} />
                            <Form.Check type='checkbox' label='require-trusted-types-for' className='form-check--halfwidth' checked={cspDirectiveRequireTrustedTypes} />
                            <Form.Check type='checkbox' label='sandbox' className='form-check--halfwidth' checked={cspDirectiveSandbox} />
                            <Form.Check type='checkbox' label='script-src-attr' className='form-check--halfwidth' checked={cspDirectiveScriptSourceAttribute} />
                            <Form.Check type='checkbox' label='script-src-elem' className='form-check--halfwidth' checked={cspDirectiveScriptSourceElement} />
                            <Form.Check type='checkbox' label='script-src' className='form-check--halfwidth' checked={cspDirectiveScriptSource} />
                            <Form.Check type='checkbox' label='style-src-attr' className='form-check--halfwidth' checked={cspDirectiveStyleSourceAttribute} />
                            <Form.Check type='checkbox' label='style-src-elem' className='form-check--halfwidth' checked={cspDirectiveStyleSourceElement} />
                            <Form.Check type='checkbox' label='style-src' className='form-check--halfwidth' checked={cspDirectiveStyleSource} />
                            <Form.Check type='checkbox' label='trusted-types' className='form-check--halfwidth' checked={cspDirectiveTrustedTypes} />
                            <Form.Check type='checkbox' label='upgrade-insecure-requests' className='form-check--halfwidth' checked={cspDirectiveUpgradeInsecureRequests} />
                            <Form.Check type='checkbox' label='worker-src' className='form-check--halfwidth' checked={cspDirectiveWorkerSource} />
                        </Form.Group>
                    </Modal.Body>
                    <Modal.Footer>
                        <Button variant='secondary' onClick={handleClose}>Close</Button>
                        <Button variant='primary' type='submit' onClick={handleSubmit}>Save</Button>
                    </Modal.Footer>
                </Form>
            </Modal>
        </>
    )
}

export default EditPermission;