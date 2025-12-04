import { useContext, useState } from 'react';
import PropTypes from 'prop-types';
import { Button, Form, Modal } from 'react-bootstrap';
import axios from 'axios';
import FormWildcardSourceUrl from '../Common/FormWildcardSourceUrl';
import { StottSecurityContext } from '../Context/StottSecurityContext';

function EditPermissionPolicy(props)
{
    const directiveName = props.directive.name ?? '';
    const directiveTitle = props.directive.title ?? '';
    const directiveDescription = props.directive.description ?? '';

    const [showModal, setShowModal] = useState(false);
    const [enabledState, setEnabledState] = useState('None');
    const [specificSources, setSpecificSources] = useState([]);
    const [hasNameError, setHasNameError] = useState(false);
    const [nameErrorMessage, setNameErrorMessage] = useState('');
    const [hasSourcesError, setHasSourcesError] = useState(false);
    const [sourcesErrorMessage, setSourcesErrorMessage] = useState('');

    const { getPermissionPolicyDirectives } = useContext(StottSecurityContext);

    const handleEnabledStateChange = (event) => {
        setEnabledState(event.target.value);
        if (event.target.value === 'Specific' && specificSources.length === 0) {
            handleAddNewSource();
        }
    };

    const handleRemoveSource = (idToRemove) => {
        let newSpecificSources = specificSources.filter(function (e) { return e.id !== idToRemove });
        setSpecificSources(newSpecificSources);
        setHasSourcesError(false);
    };

    const handleUpdateSource = (idToUpdate, sourceUrl) => {
        let newSpecificSources = specificSources.map(x => x);
        newSpecificSources.forEach(item => item.url = item.id === idToUpdate ? sourceUrl : item.url)
        setSpecificSources(newSpecificSources);
        setHasSourcesError(false);
    };

    const handleAddNewSource = () => {
        let newSpecificSources = specificSources.map(x => x);
        newSpecificSources.push({ id: crypto.randomUUID(), url: '' });
        setSpecificSources(newSpecificSources);
        setHasSourcesError(false);
    };

    const getSourcesClass = () => {
        return enabledState === 'ThisAndSpecificSites' || enabledState === 'SpecificSites'  ? 'd-block my-1' : 'd-none';
    };

    const getPreviewValue = () => {
        if (enabledState === 'All') {
            return '*';
        } else if (enabledState === 'ThisSite') {
            return '(self)';
        } else if (enabledState === 'ThisAndSpecificSites') {
            return '(self ' + specificSources.map((source) => '"' + source.url + '"').join(' ') + ')';
        } else if (enabledState === 'SpecificSites') {
            return '(' + specificSources.map((source) => '"' + source.url + '"').join(' ') + ')';
        } else {
            return '()';
        }
    };

    const renderSources = () => {
        return specificSources && specificSources.map((directiveSource) => {
            return (
                <FormWildcardSourceUrl key={directiveSource.id} sourceId={directiveSource.id} sourceUrl={directiveSource.url} handleDeleteSource={handleRemoveSource} handleUpdateSourceUrl={handleUpdateSource} allowWildcard={true}></FormWildcardSourceUrl>
            )
        })
    };

    const renderEnabledFooter = () => {
        return (<Modal.Footer className='justify-content-start'>
            This will be added to the <em>Permissions-Policy</em> header as: <em>{directiveName}={getPreviewValue()}</em>
        </Modal.Footer>);
    };

    const renderDisabledFooter = () => {
        return (<Modal.Footer className='justify-content-start'>
            This directive will not be included in the <em>Permissions-Policy</em> header, browser defaults will be used instead.
        </Modal.Footer>);
    };

    const handleCloseModal = () => 
    { 
        setShowModal(false); 
        if (props.closeModalEvent)
        {
            props.closeModalEvent();
        }
    };

    const handleOpenModal = () => { 
        // need to load and clone the sources so that reloading the component starts afresh.
        let newEnabledState = props.directive.enabledState ?? 'None';
        let originalSources = props.directive.sources ?? [];
        let newSources = [];

        originalSources.forEach(source => newSources.push({ id: source.id, url: source.url }));

        setSpecificSources(newSources);
        setEnabledState(newEnabledState);
        setShowModal(true);
    }

    const handleSaveDirective = () => {
        let sources = specificSources.map(source => source.url);
        let payload = {
            name: directiveName,
            enabledState: enabledState,
            sources: sources
        };

        axios.post(import.meta.env.VITE_PERMISSION_POLICY_SOURCE_SAVE, payload)
            .then(() => {
                handleShowToastNotification(true, 'Success', 'Permission Policy Settings have been successfully saved.');
                getPermissionPolicyDirectives();
                setShowModal(false); 
            }, (error) => {
                if(error.response && error.response.status === 400) {
                    var validationResult = error.response.data;
                    validationResult.errors.forEach(function (error) {
                        if (error.propertyName === 'Name') {
                            setHasNameError(true);
                            setNameErrorMessage(error.errorMessage);
                        } else if (error.propertyName === 'Sources') {
                            setHasSourcesError(true);
                            setSourcesErrorMessage(error.errorMessage);
                        }
                    });
                } else {
                    handleShowToastNotification(false, 'Error', 'Failed to save the Permission Policy Settings.');
                    setShowModal(false); 
                }
            });
    };

    const handleShowToastNotification = (isSuccess, title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(isSuccess, title, description);

    return (
        <>
        <Button variant='primary' className='fw-bold' onClick={handleOpenModal}>Edit</Button>
        <Modal show={showModal} onHide={handleCloseModal} size='xl'>
            <Modal.Header closeButton>{directiveTitle}</Modal.Header>
            <Modal.Body>
                {hasNameError ? <div className='invalid-feedback d-block'>{nameErrorMessage}</div> : ''}
                <Form.Group>
                    <Form.Label id='lblEnabledState'>{directiveDescription}</Form.Label>
                    <Form.Select label='Enabled State' aria-describedby='lblEnabledState' onChange={handleEnabledStateChange} value={enabledState}>
                        <option value='Disabled' className='header-value'>Disabled</option>
                        <option value='None' className='header-value'>Allow None</option>
                        <option value='All' className='header-value'>Allow all websites</option>
                        <option value='ThisSite' className='header-value'>Allow just this website</option>
                        <option value='ThisAndSpecificSites' className='header-value'>Allow this website and specific third party websites</option>
                        <option value='SpecificSites' className='header-value'>Allow specific third party websites</option>
                    </Form.Select>
                </Form.Group>
                <div className={getSourcesClass()}>
                    <Form.Label>Specific Sources</Form.Label>
                    {renderSources()}
                    <div>
                        <Button variant='success' type='button' onClick={handleAddNewSource} className='fw-bold'>Add Source</Button>
                    </div>
                    {hasSourcesError ? <div className='invalid-feedback d-block'>{sourcesErrorMessage}</div> : ''}
                </div>
            </Modal.Body>
            {enabledState === 'Disabled' ? renderDisabledFooter() : renderEnabledFooter()}
            <Modal.Footer>
                <Button variant='primary' onClick={handleSaveDirective}>Save</Button>
                <Button variant='secondary' onClick={handleCloseModal}>Cancel</Button>
            </Modal.Footer>
        </Modal>
        </>
    )
}

EditPermissionPolicy.propTypes = {
    directive : PropTypes.shape({
        name: PropTypes.string,
        title: PropTypes.string,
        description: PropTypes.string,
        enabledState: PropTypes.string,
        sources: PropTypes.arrayOf(PropTypes.shape({
            id: PropTypes.string,
            url: PropTypes.string
        }))
    }).isRequired,
    showToastNotificationEvent: PropTypes.func
};

export default EditPermissionPolicy
