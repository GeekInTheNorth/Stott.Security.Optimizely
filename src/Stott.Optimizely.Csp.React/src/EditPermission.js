import React, { useState } from "react";
import { Modal, Form } from "react-bootstrap";

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
        return cspSourceDirectives.indexOf(directive) >= 0 ? 'checked' : '';
    };

    const handleSourceChange = (event) => setCspSourceSource(event.target.value);
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
                <Modal.Header closeButton>
                    <Modal.Title>Edit Source Directives</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Form>
                        <Form.Group className='mb-3' controlId='formSource'>
                            <Form.Label>Source</Form.Label>
                            <Form.Control type='text' placeholder='Enter source' value={cspSourceSource} onChange={handleSourceChange} />
                        </Form.Group>
                        <Form.Group className='mt-3'>
                            <Form.Label>Directives</Form.Label>
                            <Form.Check type='checkbox' label='base-uri' className='form-check--halfwidth' checked={cspDirectiveBaseUri} />
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
                    </Form>
                    
                </Modal.Body>
            </Modal>
        </>
    )
}

export default EditPermission;