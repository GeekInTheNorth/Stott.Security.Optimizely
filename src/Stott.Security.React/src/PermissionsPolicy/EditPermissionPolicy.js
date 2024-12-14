import React, { useState } from 'react';
import PropTypes from 'prop-types';
import { Button, Form, Modal } from 'react-bootstrap';
import FormSourceUrl from '../Common/FormSourceUrl';

function EditPermissionPolicy(props)
{
    const directiveName = props.directive.name ?? '';
    const directiveTitle = props.directive.title ?? '';
    const directiveDescription = props.directive.description ?? '';

    const [showModal, setShowModal] = useState(false);
    const [enabledState, setEnabledState] = useState(props.directive.enabledState ?? 'None');
    const [specificSources, setSpecificSources] = useState(props.directive.sources ?? []);

    const handleEnabledStateChange = (event) => {
        setEnabledState(event.target.value);
        if (event.target.value === 'Specific' && specificSources.length === 0) {
            handleAddNewSource();
        }
    };

    const handleRemoveSource = (idToRemove) => {
        var newSpecificSources = specificSources.filter(function (e) { return e.id !== idToRemove });
        setSpecificSources(newSpecificSources);
    };

    const handleUpdateSource = (idToUpdate, sourceUrl) => {
        var newSpecificSources = specificSources.map(x => x);
        newSpecificSources.forEach(item => item.url = item.id === idToUpdate ? sourceUrl : item.url)
        setSpecificSources(newSpecificSources);
    };

    const handleAddNewSource = () => {
        var newSpecificSources = specificSources.map(x => x);
        newSpecificSources.push({ id: crypto.randomUUID(), url: '' });
        setSpecificSources(newSpecificSources);
    };

    const getSourcesClass = () => {
        return enabledState === 'ThisAndSpecificSites' || enabledState === 'SpecificSites'  ? 'd-block my-1' : 'd-none';
    };

    const getPreviewValue = () => {
        if (enabledState === 'All') {
            return '(*)';
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
                <FormSourceUrl key={directiveSource.id} sourceId={directiveSource.id} sourceUrl={directiveSource.url} handleDeleteSource={handleRemoveSource} handleUpdateSourceUrl={handleUpdateSource}></FormSourceUrl>
            )
        })
    };

    const handleCloseModal = () => 
    { 
        setShowModal(false); 
        if (props.closeModalEvent)
        {
            props.closeModalEvent();
        }
    };

    return (
        <>
        <Button variant='primary' className='fw-bold' onClick={() => setShowModal(!showModal)}>Edit</Button>
        <Modal show={showModal} onHide={handleCloseModal} size='xl'>
            <Modal.Header closeButton>{directiveTitle}</Modal.Header>
            <Modal.Body>
                <Form.Group>
                    <Form.Label id='lblEnabledState'>{directiveDescription}</Form.Label>
                    <Form.Select label='Enabled State' aria-describedby='lblEnabledState' onChange={handleEnabledStateChange} value={enabledState}>
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
                </div>
            </Modal.Body>
            <Modal.Footer className='justify-content-start'>
                This will be added to the <em>Permissions-Policy</em> header as: <em>{directiveName}={getPreviewValue()}</em>
            </Modal.Footer>
            <Modal.Footer>
                <Button variant='primary'>Save</Button>
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
        sources: PropTypes.array
    }),
    directiveSource: PropTypes.shape({
        id: PropTypes.string,
        url: PropTypes.string
    })
};

export default EditPermissionPolicy