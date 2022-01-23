import React, { useState } from "react";
import { Modal, Form, Button } from "react-bootstrap";
import axios from 'axios';

function EditPermission(props) {
    const [showEditModal, setShowEditModal] = useState(false);
    const [showDeleteModal, setShowDeleteModal] = useState(false);
    const [cspOriginalId, setOriginalId] = useState(props.id);
    const [cspOriginalSource, setCspOriginalSource] = useState(props.source);
    const [cspOriginalDirectives, setOriginalDirectives] = useState(props.directives);
    const [cspNewSource, setCspNewSource] = useState(props.source);
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
    const [hasSourceError, setHasSourceError] =  useState(false);
    const [sourceErrorMessage, setSourceErrorMessage] =  useState("");
    const [hasDirectivesError, setHasDirectivesError] =  useState(false);
    const [directivesErrorMessage, setDirectivesErrorMessage] =  useState("");

    const hasDirective = (directive) => {
        return cspOriginalDirectives.indexOf(directive) >= 0;// ? 'checked' : '';
    };

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

    const handleCloseEditModal = () => setShowEditModal(false);
    const handleShowEditModal = () => {
        // Reset form to match the existing state of the record
        setHasSourceError(false);
        setHasDirectivesError(false);
        setCspNewSource(cspOriginalSource);
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

        // Display the modal
        setShowEditModal(true);
    };

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
        params.append('id', cspOriginalId );
        params.append('source', cspNewSource );
        for (var i = 0; i < newDirectives.length; i++) {
            params.append('directives', newDirectives[i]);
        }
        axios.post('https://localhost:44344/CspPermissions/Save/', params)
            .then(() => {
                    // update visual state to match what has been saved.
                setCspOriginalSource(cspNewSource);
                setOriginalDirectives(newDirectives.join(','))
                setShowEditModal(false);
            },
            (error) => {
                if(error.response.status === 400) {
                    var validationResult = error.response.data;
                    validationResult.Errors.forEach(function (error) {
                        if (error.PropertyName === 'Source') {
                            setHasSourceError(true);
                            setSourceErrorMessage(error.ErrorMessage);
                        } else if (error.PropertyName === 'Directives') {
                            setHasDirectivesError(true);
                            setDirectivesErrorMessage(error.ErrorMessage);
                        }
                    })
                }
            });
    };

    const handleCloseDeleteModal = () => setShowDeleteModal(false);
    const handleShowDeleteModal = () => setShowDeleteModal(true);
    const handleCommitDelete = () => {
        let params = new URLSearchParams();
        params.append('id', cspOriginalId );
        axios.post('https://localhost:44344/CspPermissions/Delete/', params)
            .then(() => {
                    // update visual state to match what has been saved.
                setShowDeleteModal(false);
                props.reloadSources();
            });
    };

    return (
        <>
            <tr key={cspOriginalId}>
                <td>{cspOriginalSource}</td>
                <td>{cspOriginalDirectives}</td>
                <td>
                    <Button variant='primary' onClick={handleShowEditModal} className="mx-1">Edit</Button>
                    <Button variant='danger' onClick={handleShowDeleteModal} className="mx-1">Delete</Button>
                </td>
            </tr>

            <Modal show={showEditModal} onHide={handleCloseEditModal}>
                <Form>
                    <Modal.Header closeButton>
                        <Modal.Title>Edit Source Directives</Modal.Title>
                    </Modal.Header>
                    <Modal.Body>
                        <Form.Group className='mb-3' controlId='formSource'>
                            <Form.Label>Source</Form.Label>
                            <Form.Control type='text' placeholder='Enter source' value={cspNewSource} onChange={handleSourceChange} />
                            {hasSourceError ? <div class="invalid-feedback d-block">{sourceErrorMessage}</div> : ""}
                        </Form.Group>
                        <Form.Group className='mt-3'>
                            <Form.Label>Directives</Form.Label>
                            {hasDirectivesError ? <div class="invalid-feedback d-block">{directivesErrorMessage}</div> : ""}
                            <Form.Check type='checkbox' label='base-uri' className='form-check--halfwidth' checked={cspDirectiveBaseUri} onClick={handleDirectiveChangeBaseUri} />
                            <Form.Check type='checkbox' label='child-src' className='form-check--halfwidth' checked={cspDirectiveChildSource} onClick={handleDirectiveChangeChildSource} />
                            <Form.Check type='checkbox' label='connect-src' className='form-check--halfwidth' checked={cspDirectiveConnectSource} onClick={handleDirectiveChangeConnectSource} />
                            <Form.Check type='checkbox' label='default-src' className='form-check--halfwidth' checked={cspDirectiveDefaultSource} onClick={handleDirectiveChangeDefaultSource} />
                            <Form.Check type='checkbox' label='font-src' className='form-check--halfwidth' checked={cspDirectiveFontSource} onClick={handleDirectiveChangeFontSource} />
                            <Form.Check type='checkbox' label='form-action' className='form-check--halfwidth' checked={cspDirectiveFormAction} onClick={handleDirectiveChangeFormAction} />
                            <Form.Check type='checkbox' label='frame-ancestors' className='form-check--halfwidth' checked={cspDirectiveFrameAncestors} onClick={handleDirectiveChangeFrameAncestors} />
                            <Form.Check type='checkbox' label='frame-src' className='form-check--halfwidth' checked={cspDirectiveFrameSource} onClick={handleDirectiveChangeFrameSource} />
                            <Form.Check type='checkbox' label='img-src' className='form-check--halfwidth' checked={cspDirectiveImageSource} onClick={handleDirectiveChangeImageSource} />
                            <Form.Check type='checkbox' label='manifest-src' className='form-check--halfwidth' checked={cspDirectiveManifestSource} onClick={handleDirectiveChangeManifestSource} />
                            <Form.Check type='checkbox' label='media-src' className='form-check--halfwidth' checked={cspDirectiveMediaSource} onClick={handleDirectiveChangeMediaSource} />
                            <Form.Check type='checkbox' label='navigate-to' className='form-check--halfwidth' checked={cspDirectiveNavigateTo} onClick={handleDirectiveChangeNavigateTo} />
                            <Form.Check type='checkbox' label='object-src' className='form-check--halfwidth' checked={cspDirectiveObjectSource} onClick={handleDirectiveChangeObjectSource} />
                            <Form.Check type='checkbox' label='prefetch-src' className='form-check--halfwidth' checked={cspDirectivePreFetchSource} onClick={handleDirectiveChangePreFetchSource} />
                            <Form.Check type='checkbox' label='require-trusted-types-for' className='form-check--halfwidth' checked={cspDirectiveRequireTrustedTypes} onClick={handleDirectiveChangeRequireTrustedTypes} />
                            <Form.Check type='checkbox' label='sandbox' className='form-check--halfwidth' checked={cspDirectiveSandbox} onClick={handleDirectiveChangeSandbox} />
                            <Form.Check type='checkbox' label='script-src-attr' className='form-check--halfwidth' checked={cspDirectiveScriptSourceAttribute} onClick={handleDirectiveChangeScriptSourceAttribute} />
                            <Form.Check type='checkbox' label='script-src-elem' className='form-check--halfwidth' checked={cspDirectiveScriptSourceElement} onClick={handleDirectiveChangeScriptSourceElement} />
                            <Form.Check type='checkbox' label='script-src' className='form-check--halfwidth' checked={cspDirectiveScriptSource} onClick={handleDirectiveChangeScriptSource} />
                            <Form.Check type='checkbox' label='style-src-attr' className='form-check--halfwidth' checked={cspDirectiveStyleSourceAttribute} onClick={handleDirectiveChangeStyleSourceAttribute} />
                            <Form.Check type='checkbox' label='style-src-elem' className='form-check--halfwidth' checked={cspDirectiveStyleSourceElement} onClick={handleDirectiveChangeStyleSourceElement} />
                            <Form.Check type='checkbox' label='style-src' className='form-check--halfwidth' checked={cspDirectiveStyleSource} onClick={handleDirectiveChangeStyleSource} />
                            <Form.Check type='checkbox' label='trusted-types' className='form-check--halfwidth' checked={cspDirectiveTrustedTypes} onClick={handleDirectiveChangeTrustedTypes} />
                            <Form.Check type='checkbox' label='upgrade-insecure-requests' className='form-check--halfwidth' checked={cspDirectiveUpgradeInsecureRequests} onClick={handleDirectiveChangeUpgradeInsecureRequests} />
                            <Form.Check type='checkbox' label='worker-src' className='form-check--halfwidth' checked={cspDirectiveWorkerSource} onClick={handleDirectiveChangeWorkerSource} />
                        </Form.Group>
                    </Modal.Body>
                    <Modal.Footer>
                        <Button variant='primary' type='submit' onClick={handleCommitSave}>Save</Button>
                        <Button variant='secondary' onClick={handleCloseEditModal}>Close</Button>
                    </Modal.Footer>
                </Form>
            </Modal>

            <Modal show={showDeleteModal} onHide={handleCloseDeleteModal}>
                <Modal.Header>
                    Delete Source
                </Modal.Header>
                <Modal.Body>
                    <p>Are you sure you want to delete the following source?</p>
                    <p className='fw-bold'>{cspOriginalSource}</p>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant='danger' type='submit' onClick={handleCommitDelete}>Delete</Button>
                    <Button variant='secondary' onClick={handleCloseDeleteModal}>Close</Button>
                </Modal.Footer>
            </Modal>
        </>
    )
}

export default EditPermission;