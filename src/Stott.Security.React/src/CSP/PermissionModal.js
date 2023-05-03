import React, { useState } from "react";
import { Modal, Form, Button } from "react-bootstrap";
import axios from 'axios';

function PermissionModal(props){

    const cspId = props.id;
    const allDirectives = props.directives ? props.directives.split(",") : [];
    const hasDirective = (directive) => {
        return allDirectives.indexOf(directive) >= 0;
    };

    const IsNoneKeyword = (source) => {
        return source === "'none'";
    }

    const [showModal, setShowModal] = useState(true);
    const [cspNewSource, setCspNewSource] = useState(props.source);
    const [cspShowNoneWarning, setCspShowNoneWarning] = useState(IsNoneKeyword(props.source));
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
    const [cspDirectiveScriptSourceAttribute, setCspDirectiveScriptSourceAttribute] = useState(hasDirective('script-src-attr'));
    const [cspDirectiveScriptSourceElement, setCspDirectiveScriptSourceElement] = useState(hasDirective('script-src-elem'));
    const [cspDirectiveScriptSource, setCspDirectiveScriptSource] =  useState(hasDirective('script-src'));
    const [cspDirectiveStyleSourceAttribute, setCspDirectiveStyleSourceAttribute] = useState(hasDirective('style-src-attr'));
    const [cspDirectiveStyleSourceElement, setCspDirectiveStyleSourceElement] = useState(hasDirective('style-src-elem'));
    const [cspDirectiveStyleSource, setCspDirectiveStyleSource] = useState(hasDirective('style-src'));
    const [cspDirectiveWorkerSource, setCspDirectiveWorkerSource] = useState(hasDirective('worker-src'));
    const [hasSourceError, setHasSourceError] =  useState(false);
    const [sourceErrorMessage, setSourceErrorMessage] =  useState("");
    const [hasDirectivesError, setHasDirectivesError] =  useState(false);
    const [directivesErrorMessage, setDirectivesErrorMessage] =  useState("");

    const handleReloadSources = () => props.reloadSourceEvent && props.reloadSourceEvent();
    const handleSourceUpdate = (source) => props.updateSourceState && props.updateSourceState(source);
    const handleDirectivesUpdate = (directives) => props.updateDirectivesState && props.updateDirectivesState(directives);
    const handleSourceChange = (event) => { setCspNewSource(event.target.value); setCspShowNoneWarning(IsNoneKeyword(event.target.value)); setHasSourceError(false); };
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
    const handleDirectiveChangeScriptSourceAttribute = (event) => { setCspDirectiveScriptSourceAttribute(event.target.checked); setHasDirectivesError(false); }
    const handleDirectiveChangeScriptSourceElement = (event) => { setCspDirectiveScriptSourceElement(event.target.checked); setHasDirectivesError(false); }
    const handleDirectiveChangeScriptSource = (event) => { setCspDirectiveScriptSource(event.target.checked); setHasDirectivesError(false); }
    const handleDirectiveChangeStyleSourceAttribute = (event) => { setCspDirectiveStyleSourceAttribute(event.target.checked); setHasDirectivesError(false); }
    const handleDirectiveChangeStyleSourceElement = (event) => { setCspDirectiveStyleSourceElement(event.target.checked); setHasDirectivesError(false); }
    const handleDirectiveChangeStyleSource = (event) => { setCspDirectiveStyleSource(event.target.checked); setHasDirectivesError(false); }
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
        if (cspDirectiveScriptSourceAttribute === true) { newDirectives.push('script-src-attr'); }
        if (cspDirectiveScriptSourceElement === true) { newDirectives.push('script-src-elem'); }
        if (cspDirectiveScriptSource === true) { newDirectives.push('script-src'); }
        if (cspDirectiveStyleSourceAttribute === true) { newDirectives.push('style-src-attr'); }
        if (cspDirectiveStyleSourceElement === true) { newDirectives.push('style-src-elem'); }
        if (cspDirectiveStyleSource === true) { newDirectives.push('style-src'); }
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
        <Modal show={showModal} onHide={handleCloseModal} size='xl'>
            <Form>
                <Modal.Header closeButton className="py-2">
                    <Modal.Title>Edit Source Directives</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Form.Group className='mb-2' controlId='formSource'>
                        <Form.Label className='fw-bold d-block'>Source</Form.Label>
                        <Form.Control type='text' placeholder='Enter a URI Scheme, URL or keyword.' value={cspNewSource} onChange={handleSourceChange} list="standardSources" />
                        <datalist id="standardSources">
                            <option value="blob:">blob:</option>
                            <option value="data:">data:</option>
                            <option value="filesystem:">filesystem:</option>
                            <option value="http:">http:</option>
                            <option value="https:">https:</option>
                            <option value="mediastream:">mediastream:</option>
                            <option value="'self'">'self'</option>
                            <option value="'unsafe-eval'">'unsafe-eval'</option>
                            <option value="'wasm-unsafe-eval'">'wasm-unsafe-eval'</option>
                            <option value="'unsafe-hashes'">'unsafe-hashes'</option>
                            <option value="'unsafe-inline'">'unsafe-inline'</option>
                            <option value="'none'">'none'</option>
                            <option value="https://*.google.com">https://www.google.com (and subdomains)</option>
                            <option value="https://*.googletagmanager.com">https://www.googletagmanager.com (and subdomains)</option>
                            <option value="https://*.google-analytics.com">https://www.google-analytics.com (and subdomains)</option>
                        </datalist> 
                        {hasSourceError ? <div className='invalid-feedback d-block'>{sourceErrorMessage}</div> : ''}
                        {cspShowNoneWarning ? <div className='alert alert-warning mt-2 p-2' role='alert'>Please note that the keyword of 'none' will be the only source returned in any of the directives selected below.</div> : ''}
                    </Form.Group>
                    <Form.Group>
                        <Form.Label className='fw-bold d-block'>Directives</Form.Label>
                        {hasDirectivesError ? <div className='invalid-feedback d-block'>{directivesErrorMessage}</div> : ''}
                        <Form.Check>
                            <Form.Check.Input type='checkbox' checked={cspDirectiveBaseUri} onChange={handleDirectiveChangeBaseUri}></Form.Check.Input>
                            <Form.Check.Label>Allows this source to be used within the base element for this site. <em>(base-uri)</em></Form.Check.Label>
                        </Form.Check>
                        <Form.Check>
                            <Form.Check.Input type='checkbox' checked={cspDirectiveDefaultSource} onChange={handleDirectiveChangeDefaultSource}></Form.Check.Input>
                            <Form.Check.Label>Allows this source by default unless one or more sources are defined for a specific permission. <em>(default-src)</em></Form.Check.Label>
                        </Form.Check>
                        <Form.Check>
                            <Form.Check.Input type='checkbox' checked={cspDirectiveChildSource} onChange={handleDirectiveChangeChildSource}></Form.Check.Input>
                            <Form.Check.Label>Can contain this source in an iframe or use web workers it provides. <em>(child-src)</em></Form.Check.Label>
                        </Form.Check>
                        <Form.Check>
                            <Form.Check.Input type='checkbox' checked={cspDirectiveFrameSource} onChange={handleDirectiveChangeFrameSource}></Form.Check.Input>
                            <Form.Check.Label>Can contain this source in an iframe on this site. <em>(frame-src)</em></Form.Check.Label>
                        </Form.Check>
                        <Form.Check>
                            <Form.Check.Input type='checkbox' checked={cspDirectiveFrameAncestors} onChange={handleDirectiveChangeFrameAncestors}></Form.Check.Input>
                            <Form.Check.Label>This source can contain this site in an iframe. <em>(frame-ancestors)</em></Form.Check.Label>
                        </Form.Check>
                        <Form.Check>
                            <Form.Check.Input type='checkbox' checked={cspDirectiveConnectSource} onChange={handleDirectiveChangeConnectSource}></Form.Check.Input>
                            <Form.Check.Label>Allows links and data requests to this source. <em>(connect-src)</em></Form.Check.Label>
                        </Form.Check>
                        <Form.Check>
                            <Form.Check.Input type='checkbox' checked={cspDirectiveNavigateTo} onChange={handleDirectiveChangeNavigateTo}></Form.Check.Input>
                            <Form.Check.Label>Can initiate a navigation to this source from a link, form or javascript action. <em>(navigate-to)</em></Form.Check.Label>
                        </Form.Check>
                        <Form.Check>
                            <Form.Check.Input type='checkbox' checked={cspDirectiveFormAction} onChange={handleDirectiveChangeFormAction}></Form.Check.Input>
                            <Form.Check.Label>Can use this source within a form action. <em>(form-action)</em></Form.Check.Label>
                        </Form.Check>
                        <Form.Check>
                            <Form.Check.Input type='checkbox' checked={cspDirectiveFontSource} onChange={handleDirectiveChangeFontSource}></Form.Check.Input>
                            <Form.Check.Label>Can use fonts from this source. <em>(font-src)</em></Form.Check.Label>
                        </Form.Check>
                        <Form.Check>
                            <Form.Check.Input type='checkbox' checked={cspDirectiveImageSource} onChange={handleDirectiveChangeImageSource}></Form.Check.Input>
                            <Form.Check.Label>Can use images from this source. <em>(img-src)</em></Form.Check.Label>
                        </Form.Check>
                        <Form.Check>
                            <Form.Check.Input type='checkbox' checked={cspDirectiveMediaSource} onChange={handleDirectiveChangeMediaSource}></Form.Check.Input>
                            <Form.Check.Label>Can use audio and video files from this source. <em>(media-src)</em></Form.Check.Label>
                        </Form.Check>
                        <Form.Check>
                            <Form.Check.Input type='checkbox' checked={cspDirectiveObjectSource} onChange={handleDirectiveChangeObjectSource}></Form.Check.Input>
                            <Form.Check.Label>Allows content from this source to be used in applet, embed and object elements. <em>(object-src)</em></Form.Check.Label>
                        </Form.Check>
                        <Form.Check>
                            <Form.Check.Input type='checkbox' checked={cspDirectiveManifestSource} onChange={handleDirectiveChangeManifestSource}></Form.Check.Input>
                            <Form.Check.Label>Allows this source to be provide a manifest for this site. <em>(manifest-src)</em></Form.Check.Label>
                        </Form.Check>
                        <Form.Check>
                            <Form.Check.Input type='checkbox' checked={cspDirectivePreFetchSource} onChange={handleDirectiveChangePreFetchSource}></Form.Check.Input>
                            <Form.Check.Label>Allows content from this source to be prefetched or prerendered. <em>(prefetch-src)</em></Form.Check.Label>
                        </Form.Check>
                        <Form.Check>
                            <Form.Check.Input type='checkbox' checked={cspDirectiveScriptSource} onChange={handleDirectiveChangeScriptSource}></Form.Check.Input>
                            <Form.Check.Label>Can use javascript from this source. <em>(script-src)</em></Form.Check.Label>
                        </Form.Check>
                        <Form.Check>
                            <Form.Check.Input type='checkbox' checked={cspDirectiveScriptSourceElement} onChange={handleDirectiveChangeScriptSourceElement}></Form.Check.Input>
                            <Form.Check.Label>Can use javascript from this source to be used within a script tag. <em>(script-src-elem)</em></Form.Check.Label>
                        </Form.Check>
                        <Form.Check>
                            <Form.Check.Input type='checkbox' checked={cspDirectiveScriptSourceAttribute} onChange={handleDirectiveChangeScriptSourceAttribute}></Form.Check.Input>
                            <Form.Check.Label>Can use javascript from this source to be used within inline javascript events. <em>(script-src-attr)</em></Form.Check.Label>
                        </Form.Check>
                        <Form.Check>
                            <Form.Check.Input type='checkbox' checked={cspDirectiveWorkerSource} onChange={handleDirectiveChangeWorkerSource}></Form.Check.Input>
                            <Form.Check.Label>Can use Worker, SharedWorker and ServiceWorker scripts from this source. <em>(worker-src)</em></Form.Check.Label>
                        </Form.Check>
                        <Form.Check>
                            <Form.Check.Input type='checkbox' checked={cspDirectiveStyleSource} onChange={handleDirectiveChangeStyleSource}></Form.Check.Input>
                            <Form.Check.Label>Can use styles from this source. <em>(style-src)</em></Form.Check.Label>
                        </Form.Check>
                        <Form.Check>
                            <Form.Check.Input type='checkbox' checked={cspDirectiveStyleSourceElement} onChange={handleDirectiveChangeStyleSourceElement}></Form.Check.Input>
                            <Form.Check.Label>Can use styles from this source within a style or link tag. <em>(style-src-elem)</em></Form.Check.Label>
                        </Form.Check>
                        <Form.Check>
                            <Form.Check.Input type='checkbox' checked={cspDirectiveStyleSourceAttribute} onChange={handleDirectiveChangeStyleSourceAttribute}></Form.Check.Input>
                            <Form.Check.Label>Can use styles from this source within inline elements. <em>(style-src-attr)</em></Form.Check.Label>
                        </Form.Check>
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