import React, { useState } from "react";
import { Modal, Form, Button } from "react-bootstrap";
import axios from 'axios';

function PermissionModal(props){

    const cspId = props.id;
    const allDirectives = props.directives ? props.directives.split(",") : [];
    const hasDirective = (directive) => {
        return allDirectives.indexOf(directive) >= 0;
    };

    const [showModal, setShowModal] = useState(true);
    const [cspNewSource, setCspNewSource] = useState(props.source);
    const [cspDirectiveBaseUri, setCspDirectiveBaseUri] = useState(hasDirective('base-uri'));
    const [cspDirectiveChildSource, setCspDirectiveChildSource] = useState(hasDirective('child-src'));
    const [cspDirectiveConnectSource, setCspDirectiveConnectSource] = useState(hasDirective('connect-src'));
    const [cspDirectiveDefaultSource, setCspDirectiveDefaultSource] = useState(hasDirective('default-src'));
    const [cspDirectiveFontSource, setCspDirectiveFontSource] = useState(hasDirective('font-src'));
    const [cspDirectiveFormAction, setCspDirectiveFormAction] = useState(hasDirective('form-action'));
    const [cspDirectiveFrameAncestors, setCspDirectiveFrameAncestors] = useState(hasDirective('frame-ancestors'));
    const [cspDirectiveFrameSource, setCspDirectiveFrameSource] = useState(hasDirective('frame-src'));
    const [cspDirectiveImageSource, setCspDirectiveImageSource] = useState(hasDirective('img-src'));
    const [cspDirectiveManifestSource, setCspDirectiveManifestSource] = useState(hasDirective('manifest-src'));
    const [cspDirectiveMediaSource, setCspDirectiveMediaSource] = useState(hasDirective('media-src'));
    const [cspDirectiveNavigateTo, setCspDirectiveNavigateTo] = useState(hasDirective('navigate-to'));
    const [cspDirectiveObjectSource, setCspDirectiveObjectSource] = useState(hasDirective('object-src'));
    const [cspDirectivePreFetchSource, setCspDirectivePreFetchSource] = useState(hasDirective('prefetch-src'));
    const [cspDirectiveRequireTrustedTypes, setCspDirectiveRequireTrustedTypes] = useState(hasDirective('require-trusted-types-for'));
    const [cspDirectiveSandbox, setCspDirectiveSandbox] = useState(hasDirective('sandbox'));
    const [cspDirectiveScriptSourceAttribute, setCspDirectiveScriptSourceAttribute] = useState(hasDirective('script-src-attr'));
    const [cspDirectiveScriptSourceElement, setCspDirectiveScriptSourceElement] = useState(hasDirective('script-src-elem'));
    const [cspDirectiveScriptSource, setCspDirectiveScriptSource] =  useState(hasDirective('script-src'));
    const [cspDirectiveStyleSourceAttribute, setCspDirectiveStyleSourceAttribute] = useState(hasDirective('style-src-attr'));
    const [cspDirectiveStyleSourceElement, setCspDirectiveStyleSourceElement] = useState(hasDirective('style-src-elem'));
    const [cspDirectiveStyleSource, setCspDirectiveStyleSource] = useState(hasDirective('style-src'));
    const [cspDirectiveTrustedTypes, setCspDirectiveTrustedTypes] = useState(hasDirective('trusted-types'));
    const [cspDirectiveUpgradeInsecureRequests, setCspDirectiveUpgradeInsecureRequests] = useState(hasDirective('upgrade-insecure-requests'));
    const [cspDirectiveWorkerSource, setCspDirectiveWorkerSource] = useState(hasDirective('worker-src'));
    const [hasSourceError, setHasSourceError] =  useState(false);
    const [sourceErrorMessage, setSourceErrorMessage] =  useState("");
    const [hasDirectivesError, setHasDirectivesError] =  useState(false);
    const [directivesErrorMessage, setDirectivesErrorMessage] =  useState("");

    const handleReloadSources = () => props.reloadSourceEvent && props.reloadSourceEvent();
    const handleSourceUpdate = (source) => props.updateSourceState && props.updateSourceState(source);
    const handleDirectivesUpdate = (directives) => props.updateDirectivesState && props.updateDirectivesState(directives);
    const handleSourceChange = (event) => { setCspNewSource(event.target.value);  setHasSourceError(false); };
    const handleDirectiveChangeBaseUri = (event) => { setCspDirectiveBaseUri(event.target.checked); setHasDirectivesError(false); }
    const handleDirectiveChangeChildSource = (event) => { setCspDirectiveChildSource(event.target.checked); setHasDirectivesError(false); }
    const handleDirectiveChangeConnectSource = (event) => { setCspDirectiveConnectSource(event.target.checked); setHasDirectivesError(false); }
    const handleDirectiveChangeDefaultSource = (event) => { setCspDirectiveDefaultSource(event.target.checked); setHasDirectivesError(false); }
    const handleDirectiveChangeFontSource = (event) => { setCspDirectiveFontSource(event.target.checked); setHasDirectivesError(false); }
    const handleDirectiveChangeFormAction = (event) => { setCspDirectiveFormAction(event.target.checked); setHasDirectivesError(false); }
    const handleDirectiveChangeFrameAncestors = (event) => { setCspDirectiveFrameAncestors(event.target.checked); setHasDirectivesError(false); }
    const handleDirectiveChangeFrameSource = (event) => { setCspDirectiveFrameSource(event.target.checked); setHasDirectivesError(false); }
    const handleDirectiveChangeImageSource = (event) => { setCspDirectiveImageSource(event.target.checked); setHasDirectivesError(false); }
    const handleDirectiveChangeManifestSource = (event) => { setCspDirectiveManifestSource(event.target.checked); setHasDirectivesError(false); }
    const handleDirectiveChangeMediaSource = (event) => { setCspDirectiveMediaSource(event.target.checked); setHasDirectivesError(false); }
    const handleDirectiveChangeNavigateTo = (event) => { setCspDirectiveNavigateTo(event.target.checked); setHasDirectivesError(false); }
    const handleDirectiveChangeObjectSource = (event) => { setCspDirectiveObjectSource(event.target.checked); setHasDirectivesError(false); }
    const handleDirectiveChangePreFetchSource = (event) => { setCspDirectivePreFetchSource(event.target.checked); setHasDirectivesError(false); }
    const handleDirectiveChangeRequireTrustedTypes = (event) => { setCspDirectiveRequireTrustedTypes(event.target.checked); setHasDirectivesError(false); }
    const handleDirectiveChangeSandbox = (event) => { setCspDirectiveSandbox(event.target.checked); setHasDirectivesError(false); }
    const handleDirectiveChangeScriptSourceAttribute = (event) => { setCspDirectiveScriptSourceAttribute(event.target.checked); setHasDirectivesError(false); }
    const handleDirectiveChangeScriptSourceElement = (event) => { setCspDirectiveScriptSourceElement(event.target.checked); setHasDirectivesError(false); }
    const handleDirectiveChangeScriptSource = (event) => { setCspDirectiveScriptSource(event.target.checked); setHasDirectivesError(false); }
    const handleDirectiveChangeStyleSourceAttribute = (event) => { setCspDirectiveStyleSourceAttribute(event.target.checked); setHasDirectivesError(false); }
    const handleDirectiveChangeStyleSourceElement = (event) => { setCspDirectiveStyleSourceElement(event.target.checked); setHasDirectivesError(false); }
    const handleDirectiveChangeStyleSource = (event) => { setCspDirectiveStyleSource(event.target.checked); setHasDirectivesError(false); }
    const handleDirectiveChangeTrustedTypes = (event) => { setCspDirectiveTrustedTypes(event.target.checked); setHasDirectivesError(false); }
    const handleDirectiveChangeUpgradeInsecureRequests = (event) => { setCspDirectiveUpgradeInsecureRequests(event.target.checked); setHasDirectivesError(false); }
    const handleDirectiveChangeWorkerSource = (event) => { setCspDirectiveWorkerSource(event.target.checked); setHasDirectivesError(false); }

    const handleCloseModal = () => { setShowModal(false); props.closeModalEvent() };
    const handleShowSuccessToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(true, title, description);
    const handleShowFailureToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(false, title, description);
    const handleCommitSave = (event) => {
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

        let params = new URLSearchParams();
        params.append('id', cspId);
        params.append('source', cspNewSource);
        for (var i = 0; i < newDirectives.length; i++) {
            params.append('directives', newDirectives[i]);
        }
        axios.post(process.env.REACT_APP_PERMISSION_SAVE_URL, params)
            .then(() => {
                // update visual state to match what has been saved.
                handleReloadSources();
                handleSourceUpdate(cspNewSource);
                handleDirectivesUpdate(newDirectives.join(','));
                handleShowSuccessToast('Source Saved', 'Successfully saved the source: ' + cspNewSource);
                handleCloseModal();
            },
            (error) => {
                if(error.response.status === 400) {
                    var validationResult = error.response.data;
                    validationResult.errors.forEach(function (error) {
                        if (error.propertyName === 'Source') {
                            setHasSourceError(true);
                            setSourceErrorMessage(error.errorMessage);
                        } else if (error.propertyName === 'Directives') {
                            setHasDirectivesError(true);
                            setDirectivesErrorMessage(error.errorMessage);
                        }
                    })
                }
                else{
                    handleShowFailureToast('Error', 'Failed to save the source: ' + cspNewSource);
                    handleCloseModal();
                }
            });
    };

    return (
        <Modal show={showModal} onHide={handleCloseModal} size='lg'>
            <Form>
                <Modal.Header closeButton>
                    <Modal.Title>Edit Source Directives</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Form.Group className='mb-3' controlId='formSource'>
                        <Form.Label className='fw-bold d-block'>Source</Form.Label>
                        <Form.Control type='text' placeholder='Enter source' value={cspNewSource} onChange={handleSourceChange} />
                        {hasSourceError ? <div className="invalid-feedback d-block">{sourceErrorMessage}</div> : ""}
                    </Form.Group>
                    <Form.Group className='mt-3'>
                        <Form.Label className='fw-bold d-block'>Directives</Form.Label>
                        {hasDirectivesError ? <div className="invalid-feedback d-block">{directivesErrorMessage}</div> : ""}
                        <Form.Check type='checkbox' label='base-uri' className='form-check--halfwidth' checked={cspDirectiveBaseUri} onChange={handleDirectiveChangeBaseUri} />
                        <Form.Check type='checkbox' label='child-src' className='form-check--halfwidth' checked={cspDirectiveChildSource} onChange={handleDirectiveChangeChildSource} />
                        <Form.Check type='checkbox' label='connect-src' className='form-check--halfwidth' checked={cspDirectiveConnectSource} onChange={handleDirectiveChangeConnectSource} />
                        <Form.Check type='checkbox' label='default-src' className='form-check--halfwidth' checked={cspDirectiveDefaultSource} onChange={handleDirectiveChangeDefaultSource} />
                        <Form.Check type='checkbox' label='font-src' className='form-check--halfwidth' checked={cspDirectiveFontSource} onChange={handleDirectiveChangeFontSource} />
                        <Form.Check type='checkbox' label='form-action' className='form-check--halfwidth' checked={cspDirectiveFormAction} onChange={handleDirectiveChangeFormAction} />
                        <Form.Check type='checkbox' label='frame-ancestors' className='form-check--halfwidth' checked={cspDirectiveFrameAncestors} onChange={handleDirectiveChangeFrameAncestors} />
                        <Form.Check type='checkbox' label='frame-src' className='form-check--halfwidth' checked={cspDirectiveFrameSource} onChange={handleDirectiveChangeFrameSource} />
                        <Form.Check type='checkbox' label='img-src' className='form-check--halfwidth' checked={cspDirectiveImageSource} onChange={handleDirectiveChangeImageSource} />
                        <Form.Check type='checkbox' label='manifest-src' className='form-check--halfwidth' checked={cspDirectiveManifestSource} onChange={handleDirectiveChangeManifestSource} />
                        <Form.Check type='checkbox' label='media-src' className='form-check--halfwidth' checked={cspDirectiveMediaSource} onChange={handleDirectiveChangeMediaSource} />
                        <Form.Check type='checkbox' label='navigate-to' className='form-check--halfwidth' checked={cspDirectiveNavigateTo} onChange={handleDirectiveChangeNavigateTo} />
                        <Form.Check type='checkbox' label='object-src' className='form-check--halfwidth' checked={cspDirectiveObjectSource} onChange={handleDirectiveChangeObjectSource} />
                        <Form.Check type='checkbox' label='prefetch-src' className='form-check--halfwidth' checked={cspDirectivePreFetchSource} onChange={handleDirectiveChangePreFetchSource} />
                        <Form.Check type='checkbox' label='require-trusted-types-for' className='form-check--halfwidth' checked={cspDirectiveRequireTrustedTypes} onChange={handleDirectiveChangeRequireTrustedTypes} />
                        <Form.Check type='checkbox' label='sandbox' className='form-check--halfwidth' checked={cspDirectiveSandbox} onChange={handleDirectiveChangeSandbox} />
                        <Form.Check type='checkbox' label='script-src-attr' className='form-check--halfwidth' checked={cspDirectiveScriptSourceAttribute} onChange={handleDirectiveChangeScriptSourceAttribute} />
                        <Form.Check type='checkbox' label='script-src-elem' className='form-check--halfwidth' checked={cspDirectiveScriptSourceElement} onChange={handleDirectiveChangeScriptSourceElement} />
                        <Form.Check type='checkbox' label='script-src' className='form-check--halfwidth' checked={cspDirectiveScriptSource} onChange={handleDirectiveChangeScriptSource} />
                        <Form.Check type='checkbox' label='style-src-attr' className='form-check--halfwidth' checked={cspDirectiveStyleSourceAttribute} onChange={handleDirectiveChangeStyleSourceAttribute} />
                        <Form.Check type='checkbox' label='style-src-elem' className='form-check--halfwidth' checked={cspDirectiveStyleSourceElement} onChange={handleDirectiveChangeStyleSourceElement} />
                        <Form.Check type='checkbox' label='style-src' className='form-check--halfwidth' checked={cspDirectiveStyleSource} onChange={handleDirectiveChangeStyleSource} />
                        <Form.Check type='checkbox' label='trusted-types' className='form-check--halfwidth' checked={cspDirectiveTrustedTypes} onChange={handleDirectiveChangeTrustedTypes} />
                        <Form.Check type='checkbox' label='upgrade-insecure-requests' className='form-check--halfwidth' checked={cspDirectiveUpgradeInsecureRequests} onChange={handleDirectiveChangeUpgradeInsecureRequests} />
                        <Form.Check type='checkbox' label='worker-src' className='form-check--halfwidth' checked={cspDirectiveWorkerSource} onChange={handleDirectiveChangeWorkerSource} />
                    </Form.Group>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant='primary' type='submit' onClick={handleCommitSave}>Save</Button>
                    <Button variant='secondary' onClick={handleCloseModal}>Close</Button>
                </Modal.Footer>
            </Form>
        </Modal>
    )
}

export default PermissionModal