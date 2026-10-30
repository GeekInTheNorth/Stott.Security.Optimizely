import { useContext, useState } from 'react';
import PropTypes from 'prop-types';
import { Box, Button, Dialog, DialogBody, DialogContent, DialogFooter, DialogHeader, Field, Text } from '@optiaxiom/react';
import FormSelect from '../Common/FormSelect';
import { httpPost } from '../Common/httpClient';
import FormWildcardSourceUrl from '../Common/FormWildcardSourceUrl';
import { StottSecurityContext } from '../Context/StottSecurityContext';

const enabledStateOptions = [
    { label: 'Disabled', value: 'Disabled' },
    { label: 'Allow None', value: 'None' },
    { label: 'Allow all websites', value: 'All' },
    { label: 'Allow just this website', value: 'ThisSite' },
    { label: 'Allow this website and specific third party websites', value: 'ThisAndSpecificSites' },
    { label: 'Allow specific third party websites', value: 'SpecificSites' }
];

const usesSpecificSources = (state) => state === 'ThisAndSpecificSites' || state === 'SpecificSites';

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

    const handleEnabledStateChange = (value) => {
        setEnabledState(value);
        if (usesSpecificSources(value) && specificSources.length === 0) {
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

    const getSourcesDisplay = () => {
        return usesSpecificSources(enabledState) ? 'block' : 'none';
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
        return (<Text flex='1' fontSize='sm'>
            This will be added to the <em>Permissions-Policy</em> header as: <em>{directiveName}={getPreviewValue()}</em>
        </Text>);
    };

    const renderDisabledFooter = () => {
        return (<Text flex='1' fontSize='sm'>
            This directive will not be included in the <em>Permissions-Policy</em> header, browser defaults will be used instead.
        </Text>);
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
            appId: props.appId,
            hostName: props.hostName,
            sources: sources
        };

        httpPost(import.meta.env.VITE_PERMISSION_POLICY_SOURCE_SAVE, payload)
            .then(() => {
                handleShowToastNotification(true, 'Success', 'Permission Policy Settings have been successfully saved.');
                getPermissionPolicyDirectives(props.appId, props.hostName);
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
        <Button appearance='primary' onClick={handleOpenModal}>Edit</Button>
        <Dialog open={showModal} onOpenChange={(open) => !open && handleCloseModal()}>
            <DialogContent size='lg'>
                <DialogHeader description={directiveDescription}>{directiveTitle}</DialogHeader>
                <DialogBody>
                    {hasNameError ? <Text color='fg.error' fontSize='sm'>{nameErrorMessage}</Text> : ''}
                    <Field label='Enabled State'>
                        <FormSelect onValueChange={handleEnabledStateChange} value={enabledState} options={enabledStateOptions} />
                    </Field>
                    <Box display={getSourcesDisplay()} my='4'>
                        <Text fontWeight='500' mb='8'>Specific Sources</Text>
                        {renderSources()}
                        <Button appearance='primary' type='button' onClick={handleAddNewSource}>Add Source</Button>
                        {hasSourcesError ? <Text color='fg.error' fontSize='sm'>{sourcesErrorMessage}</Text> : ''}
                    </Box>
                </DialogBody>
                <DialogFooter>
                    {enabledState === 'Disabled' ? renderDisabledFooter() : renderEnabledFooter()}
                    <Button appearance='default' onClick={handleCloseModal}>Cancel</Button>
                    <Button appearance='primary' onClick={handleSaveDirective}>Save</Button>
                </DialogFooter>
            </DialogContent>
        </Dialog>
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
    showToastNotificationEvent: PropTypes.func,
    appId: PropTypes.string,
    hostName: PropTypes.string
};

export default EditPermissionPolicy
