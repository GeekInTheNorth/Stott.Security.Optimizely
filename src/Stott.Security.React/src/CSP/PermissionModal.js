import React, { useState, useEffect } from "react";
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
    const [validDirectives, setValidDirectives] = useState([]);

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
        if (cspDirectiveBaseUri === true) { addDirectiveToArray('base-uri', newDirectives); }
        if (cspDirectiveChildSource === true) { addDirectiveToArray('child-src', newDirectives); }
        if (cspDirectiveConnectSource === true) { addDirectiveToArray('connect-src', newDirectives); }
        if (cspDirectiveDefaultSource === true) { addDirectiveToArray('default-src', newDirectives); }
        if (cspDirectiveFontSource === true) { addDirectiveToArray('font-src', newDirectives); }
        if (cspDirectiveFormAction === true) { addDirectiveToArray('form-action', newDirectives); }
        if (cspDirectiveFrameAncestors === true) { addDirectiveToArray('frame-ancestors', newDirectives); }
        if (cspDirectiveFrameSource === true) { addDirectiveToArray('frame-src', newDirectives); }
        if (cspDirectiveImageSource === true) { addDirectiveToArray('img-src', newDirectives); }
        if (cspDirectiveManifestSource === true) { addDirectiveToArray('manifest-src', newDirectives); }
        if (cspDirectiveMediaSource === true) { addDirectiveToArray('media-src', newDirectives); }
        if (cspDirectiveNavigateTo === true) { addDirectiveToArray('navigate-to', newDirectives); }
        if (cspDirectiveObjectSource === true) { addDirectiveToArray('object-src', newDirectives); }
        if (cspDirectivePreFetchSource === true) { addDirectiveToArray('prefetch-src', newDirectives); }
        if (cspDirectiveScriptSourceAttribute === true) { addDirectiveToArray('script-src-attr', newDirectives); }
        if (cspDirectiveScriptSourceElement === true) { addDirectiveToArray('script-src-elem', newDirectives); }
        if (cspDirectiveScriptSource === true) { addDirectiveToArray('script-src', newDirectives); }
        if (cspDirectiveStyleSourceAttribute === true) { addDirectiveToArray('style-src-attr', newDirectives); }
        if (cspDirectiveStyleSourceElement === true) { addDirectiveToArray('style-src-elem', newDirectives); }
        if (cspDirectiveStyleSource === true) { addDirectiveToArray('style-src', newDirectives); }
        if (cspDirectiveWorkerSource === true) { addDirectiveToArray('worker-src', newDirectives); }

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
                if(error.response && error.response.status === 400) {
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

    const getValidDirectives = () => {
        axios.get(process.env.REACT_APP_PERMISSION_VALIDDIRECTIVES_URL, { params: { source: cspNewSource } })
            .then((response) => {
                if (response.data && Array.isArray(response.data)){
                    setValidDirectives(response.data);
                }
                else{
                    handleShowFailureToast("Get Valid Directives", "Failed to retrieve valid directives for the current source.");
                }
            },
            () => {
                handleShowFailureToast("Error", "Failed to retrieve valid directives for the current source.");
            });
    };

    useEffect(() => {
        getValidDirectives();
    }, [ cspNewSource ]);

    const checkDirectiveClass = (directive) => {
        return checkDirectiveIsValid(directive) ? '' : 'd-none';
    };

    const checkDirectiveIsValid = (directive) => {
        return validDirectives.indexOf(directive) >= 0;
    };

    const addDirectiveToArray = (directive, array) => {
        if (checkDirectiveIsValid(directive) && array.indexOf(directive) < 0) {
            array.push(directive);
        }
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
                            <option value="ws:">ws:</option>
                            <option value="wss:">wss:</option>
                            <option value="mediastream:">mediastream:</option>
                            <option value="'self'">'self'</option>
                            <option value="'unsafe-eval'">'unsafe-eval'</option>
                            <option value="'wasm-unsafe-eval'">'wasm-unsafe-eval'</option>
                            <option value="'unsafe-hashes'">'unsafe-hashes'</option>
                            <option value="'unsafe-inline'">'unsafe-inline'</option>
                            <option value="'inline-speculation-rules'">'inline-speculation-rules'</option>
                            <option value="'none'">'none'</option>
                            <option value="https://*.google.com">https://www.google.com (and subdomains)</option>
                            <option value="https://*.googletagmanager.com">https://www.googletagmanager.com (and subdomains)</option>
                            <option value="https://*.google-analytics.com">https://www.google-analytics.com (and subdomains)</option>
                        </datalist> 
                        {hasSourceError ? <div className='invalid-feedback d-block'>{sourceErrorMessage}</div> : ''}
                        {cspShowNoneWarning ? <div className='alert alert-warning mt-2 p-2' role='alert'>Please note that the keyword of 'none' will be the only source returned in any of the directives selected below.</div> : ''}
                    </Form.Group>
                    <Form.Group className="group-permissions">
                        <Form.Label className='fw-bold d-block'>Directives</Form.Label>
                        {hasDirectivesError ? <div className='invalid-feedback d-block'>{directivesErrorMessage}</div> : ''}
                        <Form.Check className={checkDirectiveClass('base-uri')}>
                            <Form.Check.Input id='chkBaseUri' type='checkbox' checked={cspDirectiveBaseUri} onChange={handleDirectiveChangeBaseUri}></Form.Check.Input>
                            <Form.Check.Label for='chkBaseUri'><strong>base-uri</strong>: Allows this source to be used within the base element for this site.</Form.Check.Label>
                        </Form.Check>
                        <Form.Check className={checkDirectiveClass('default-src')}>
                            <Form.Check.Input id='chkDefaultSrc' type='checkbox' checked={cspDirectiveDefaultSource} onChange={handleDirectiveChangeDefaultSource}></Form.Check.Input>
                            <Form.Check.Label for='chkDefaultSrc'><strong>default-src</strong>: Allows this source by default unless one or more sources are defined for a specific permission.</Form.Check.Label>
                        </Form.Check>
                        <Form.Check className={checkDirectiveClass('child-src')}>
                            <Form.Check.Input id='chkChildSrc' type='checkbox' checked={cspDirectiveChildSource} onChange={handleDirectiveChangeChildSource}></Form.Check.Input>
                            <Form.Check.Label for='chkChildSrc'><strong>child-src</strong>: Can contain this source in an iframe or use web workers it provides.</Form.Check.Label>
                        </Form.Check>
                        <Form.Check className={checkDirectiveClass('frame-src')}>
                            <Form.Check.Input id='chkFrameSrc' type='checkbox' checked={cspDirectiveFrameSource} onChange={handleDirectiveChangeFrameSource}></Form.Check.Input>
                            <Form.Check.Label for='chkFrameSrc'><strong>frame-src</strong>: Can contain this source in an iframe on this site.</Form.Check.Label>
                        </Form.Check>
                        <Form.Check className={checkDirectiveClass('connect-src')}>
                            <Form.Check.Input id='chkFrameAncestors' type='checkbox' checked={cspDirectiveFrameAncestors} onChange={handleDirectiveChangeFrameAncestors}></Form.Check.Input>
                            <Form.Check.Label for='chkFrameAncestors'><strong>frame-ancestors</strong>: This source can contain this site in an iframe.</Form.Check.Label>
                        </Form.Check>
                        <Form.Check className={checkDirectiveClass('connect-src')}>
                            <Form.Check.Input id='chkConnectSrc' type='checkbox' checked={cspDirectiveConnectSource} onChange={handleDirectiveChangeConnectSource}></Form.Check.Input>
                            <Form.Check.Label for='chkConnectSrc'><strong>connect-src</strong>: Allows links and data requests to this source.</Form.Check.Label>
                        </Form.Check>
                        <Form.Check className={checkDirectiveClass('navigate-to')}>
                            <Form.Check.Input id='chkNavigateTo' type='checkbox' checked={cspDirectiveNavigateTo} onChange={handleDirectiveChangeNavigateTo}></Form.Check.Input>
                            <Form.Check.Label for='chkNavigateTo'><strong>navigate-to</strong>: Can initiate a navigation to this source from a link, form or javascript action.</Form.Check.Label>
                        </Form.Check>
                        <Form.Check className={checkDirectiveClass('form-action')}>
                            <Form.Check.Input id='chkFormAction' type='checkbox' checked={cspDirectiveFormAction} onChange={handleDirectiveChangeFormAction}></Form.Check.Input>
                            <Form.Check.Label for='chkFormAction'><strong>form-action</strong>: Can use this source within a form action.</Form.Check.Label>
                        </Form.Check>
                        <Form.Check className={checkDirectiveClass('font-src')}>
                            <Form.Check.Input id='chkFontSrc' type='checkbox' checked={cspDirectiveFontSource} onChange={handleDirectiveChangeFontSource}></Form.Check.Input>
                            <Form.Check.Label for='chkFontSrc'><strong>font-src</strong>: Can use fonts from this source.</Form.Check.Label>
                        </Form.Check>
                        <Form.Check className={checkDirectiveClass('img-src')}>
                            <Form.Check.Input id='chkImgSrc' type='checkbox' checked={cspDirectiveImageSource} onChange={handleDirectiveChangeImageSource}></Form.Check.Input>
                            <Form.Check.Label for='chkImgSrc'><strong>img-src</strong>: Can use images from this source.</Form.Check.Label>
                        </Form.Check>
                        <Form.Check className={checkDirectiveClass('media-src')}>
                            <Form.Check.Input id='chkMediaSrc' type='checkbox' checked={cspDirectiveMediaSource} onChange={handleDirectiveChangeMediaSource}></Form.Check.Input>
                            <Form.Check.Label for='chkMediaSrc'><strong>media-src</strong>: Can use audio and video files from this source.</Form.Check.Label>
                        </Form.Check>
                        <Form.Check className={checkDirectiveClass('object-src')}>
                            <Form.Check.Input id='chkObjectSrc' type='checkbox' checked={cspDirectiveObjectSource} onChange={handleDirectiveChangeObjectSource}></Form.Check.Input>
                            <Form.Check.Label for='chkObjectSrc'><strong>object-src</strong>: Allows content from this source to be used in applet, embed and object elements.</Form.Check.Label>
                        </Form.Check>
                        <Form.Check className={checkDirectiveClass('manifest-src')}>
                            <Form.Check.Input id='chkManifestSrc' type='checkbox' checked={cspDirectiveManifestSource} onChange={handleDirectiveChangeManifestSource}></Form.Check.Input>
                            <Form.Check.Label for='chkManifestSrc'><strong>manifest-src</strong>: Allows this source to be provide a manifest for this site.</Form.Check.Label>
                        </Form.Check>
                        <Form.Check className={checkDirectiveClass('prefetch-src')}>
                            <Form.Check.Input id='chkPrefetchSrc' type='checkbox' checked={cspDirectivePreFetchSource} onChange={handleDirectiveChangePreFetchSource}></Form.Check.Input>
                            <Form.Check.Label for='chkPrefetchSrc'><strong>prefetch-src</strong>: Allows content from this source to be prefetched or prerendered.</Form.Check.Label>
                        </Form.Check>
                        <Form.Check className={checkDirectiveClass('script-src')}>
                            <Form.Check.Input id='chkScriptSrc' type='checkbox' checked={cspDirectiveScriptSource} onChange={handleDirectiveChangeScriptSource}></Form.Check.Input>
                            <Form.Check.Label for='chkScriptSrc'><strong>script-src</strong>: Can use javascript from this source.</Form.Check.Label>
                        </Form.Check>
                        <Form.Check className={checkDirectiveClass('script-src-elem')}>
                            <Form.Check.Input id='chkScriptSrcElem' type='checkbox' checked={cspDirectiveScriptSourceElement} onChange={handleDirectiveChangeScriptSourceElement}></Form.Check.Input>
                            <Form.Check.Label for='chkScriptSrcElem'><strong>script-src-elem</strong>: Can use javascript from this source to be used within a script tag.</Form.Check.Label>
                        </Form.Check>
                        <Form.Check className={checkDirectiveClass('script-src-attr')}>
                            <Form.Check.Input id='chkScriptSrcAttr' type='checkbox' checked={cspDirectiveScriptSourceAttribute} onChange={handleDirectiveChangeScriptSourceAttribute}></Form.Check.Input>
                            <Form.Check.Label for='chkScriptSrcAttr'><strong>script-src-attr</strong>: Can use javascript from this source to be used within inline javascript events.</Form.Check.Label>
                        </Form.Check>
                        <Form.Check className={checkDirectiveClass('worker-src')}>
                            <Form.Check.Input id='chkWorkerSrc' type='checkbox' checked={cspDirectiveWorkerSource} onChange={handleDirectiveChangeWorkerSource}></Form.Check.Input>
                            <Form.Check.Label for='chkWorkerSrc'><strong>worker-src</strong>: Can use Worker, SharedWorker and ServiceWorker scripts from this source.</Form.Check.Label>
                        </Form.Check>
                        <Form.Check className={checkDirectiveClass('style-src')}>
                            <Form.Check.Input id='chkStyleSrc' type='checkbox' checked={cspDirectiveStyleSource} onChange={handleDirectiveChangeStyleSource}></Form.Check.Input>
                            <Form.Check.Label for='chkStyleSrc'><strong>style-src</strong>: Can use styles from this source.</Form.Check.Label>
                        </Form.Check>
                        <Form.Check className={checkDirectiveClass('style-src-elem')}>
                            <Form.Check.Input id='chkStyleSrcElem' type='checkbox' checked={cspDirectiveStyleSourceElement} onChange={handleDirectiveChangeStyleSourceElement}></Form.Check.Input>
                            <Form.Check.Label for='chkStyleSrcElem'><strong>style-src-elem</strong>: Can use styles from this source within a style tag.</Form.Check.Label>
                        </Form.Check>
                        <Form.Check className={checkDirectiveClass('script-src-attr')}>
                            <Form.Check.Input id='chkStyleSrcAttr' type='checkbox' checked={cspDirectiveStyleSourceAttribute} onChange={handleDirectiveChangeStyleSourceAttribute}></Form.Check.Input>
                            <Form.Check.Label for='chkStyleSrcAttr'><strong>style-src-attr</strong>: Can use styles from this source within inline elements.</Form.Check.Label>
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