import { useCsp } from "./CspContext";
import { useState, useEffect } from 'react';
import PropTypes from 'prop-types';
import { Form, Button, Container } from 'react-bootstrap';
import FormUrl from '../Common/FormUrl';
import FormPath from '../Common/FormPath';
import axios from 'axios';

function CspSettings({ cspPolicy, showToastNotificationEvent }) {
  
  const { allPolicies, selectPolicyView, allSites } = useCsp();

  const [policy, setPolicy] = useState(cspPolicy);
  const [disableSaveButton, setDisableSaveButton] = useState(true);
  const [isExternalReportingClassName, setIsExternalReportingClassName] = useState('my-3 d-none');
  const [hasExternalReportToUrlError, setHasExternalReportToUrlError] = useState(false);
  const [externalReportToUrlErrorMessage, setExternalReportToUrlErrorMessage] = useState('');
  const [allowListUrlClassName, setAllowListUrlClassName] = useState('my-3 d-none');
  const [hasAllowListUrlError, setAllowListUrlError] = useState(false);
  const [allowListUrlErrorMessage, setAllowListUrlErrorMessage] = useState('');
  const [hostSelectClassName, setHostSelectClassName] = useState('my-3 d-none');

  const handleNameChange = (e) => {
    setPolicy({ ...policy, name: e.target.value });
    setDisableSaveButton(false);
  };
  
  const handleIsCspEnabledChange = (e) => {
    if (e.target.checked) {
      setPolicy({ ...policy, isEnabled: e.target.checked });
    }
    else {
      setPolicy({ ...policy, isEnabled: e.target.checked, isReportOnly: false });
    }
    setDisableSaveButton(false);
  };
  
  const handleIsCspReportOnly = (e) => {
    if (policy.isEnabled) {
      setPolicy({ ...policy, isReportOnly: e.target.checked });
      setDisableSaveButton(false);
    }
  };
  
  const handleIsInternalReportingEnabled = (e) => {
    setPolicy({ ...policy, useInternalReporting: e.target.checked });
    setDisableSaveButton(false);
  };
  
  const handleIsExternalReportingEnabled = (e) => {
    setPolicy({ ...policy, useExternalReporting: e.target.checked });
    setHasExternalReportToUrlError(false);
    setExternalReportToUrlErrorMessage('');
    setDisableSaveButton(false);
  };

  const handleExternalReportToUrl = (newUrl) => {
    setPolicy({ ...policy, externalReportToUrl: newUrl });
    setHasExternalReportToUrlError(false);
    setExternalReportToUrlErrorMessage('');
    setDisableSaveButton(false);
  };

  const handleIsAllowListEnabled = (e) => {
    setPolicy({ ...policy, isAllowListEnabled: e.target.checked });
    setAllowListUrlError(false);
    setAllowListUrlErrorMessage('');
    setDisableSaveButton(false);
  };

  const handleAllowListAddress = (newUrl) => {
    setPolicy({ ...policy, allowListUrl: newUrl });
    setAllowListUrlError(false);
    setAllowListUrlErrorMessage('');
    setDisableSaveButton(false);
  };
  
  const handleUpgradeInsecureRequests = (e) => {
    setPolicy({ ...policy, isUpgradeInsecureRequestsEnabled: e.target.checked });
    setDisableSaveButton(false);
  };
  
  const handleIsNonceEnabled = (e) => {
    if (e.target.checked){
      setPolicy({ ...policy, isNonceEnabled: e.target.checked });
    } else {
      setPolicy({ ...policy, isNonceEnabled: e.target.checked, isStrictDynamicEnabled: false });
    }
    setDisableSaveButton(false);
  };
  
  const handleIsStrictDynamicEnabled = (e) => {
    if (policy.isNonceEnabled) {
      setPolicy({ ...policy, isStrictDynamicEnabled: e.target.checked });
    } else {
      setPolicy({ ...policy, isStrictDynamicEnabled: false });
    }
    setDisableSaveButton(false);
  };

  const handleSiteScopeChange = (e) => {
    setPolicy({ ...policy, scopeSiteId: e.target.value, scopeHostName: '' });
    setDisableSaveButton(false);
  };

  const handleHostScopeChange = (e) => {
    const selectedHost = e.target.value;
    setPolicy({ ...policy, scopeHostName: selectedHost });
    setDisableSaveButton(false);
  };

  const renderScopeHosts = () => {
    if (policy.scopeSiteId && policy.scopeSiteId !== '') {
      const selectedSite = allSites.find(site => site.id === policy.scopeSiteId);
      const hosts = selectedSite ? selectedSite.hosts : [];
      return (hosts.map(host => ( <option key={host.hostName} value={host.hostName}>{host.displayName}</option> )));
    } else {
      return null;
    }
  }

  const handleScopePathChange = (idx, newValue) => {
    const updatedPaths = [...policy.scopePaths];
    updatedPaths[idx] = newValue;
    setPolicy({ ...policy, scopePaths: updatedPaths });
    setDisableSaveButton(false);
  };

  const handleRemoveScopePath = (idx) => {
    const updatedPaths = policy.scopePaths.filter((_, i) => i !== idx);
    setPolicy({ ...policy, scopePaths: updatedPaths });
    setDisableSaveButton(false);
  };

  const handleAddScopePath = () => {
    setPolicy({ ...policy, scopePaths: [...policy.scopePaths, '/'] });
    setDisableSaveButton(false);
  };

  const handleExclusionPathChange = (idx, newValue) => {
    const updatedPaths = [...policy.scopeExclusions];
    updatedPaths[idx] = newValue;
    setPolicy({ ...policy, scopeExclusions: updatedPaths });
    setDisableSaveButton(false);
  };

  const handleRemoveExclusionPath = (idx) => {
    const updatedPaths = policy.scopeExclusions.filter((_, i) => i !== idx);
    setPolicy({ ...policy, scopeExclusions: updatedPaths });
    setDisableSaveButton(false);
  };

  const handleAddExclusionPath = () => {
    setPolicy({ ...policy, scopeExclusions: [...policy.scopeExclusions, '/'] });
    setDisableSaveButton(false);
  };

  const InitializeControlVisibility = () => {
    if (policy.useExternalReporting) {
      setIsExternalReportingClassName('my-3');
    } else {
      setIsExternalReportingClassName('my-3 d-none');
    }
    if (policy.isAllowListEnabled) {
      setAllowListUrlClassName('my-3');
    } else {
      setAllowListUrlClassName('my-3 d-none');
    }
    if (policy.scopeSiteId && policy.scopeSiteId !== '') {
      setHostSelectClassName('my-3');
    } else {
      setHostSelectClassName('my-3 d-none');
    }
  };

  useEffect(() => {
    InitializeControlVisibility();
  }, [policy]);

  const handleSaveSettings = (event) => {
    event.preventDefault();

    let params = new URLSearchParams();
    params.append('policyId', policy.id);
    params.append('name', policy.name);
    params.append('scopeSiteId', policy.scopeSiteId);
    params.append('scopeHostName', policy.scopeHostName);
    params.append('scopeBehavior', policy.scopeBehavior);
    params.append('scopePaths', policy.scopePaths.join(','));
    params.append('scopeExclusions', policy.scopeExclusions.join(','));
    params.append('isEnabled', policy.isEnabled);
    params.append('isReportOnly', policy.isReportOnly);
    params.append('useInternalReporting', policy.useInternalReporting);
    params.append('useExternalReporting', policy.useExternalReporting);
    params.append('externalReportToUrl', policy.externalReportToUrl);
    params.append('isAllowListEnabled', policy.isAllowListEnabled);
    params.append('allowListUrl', policy.allowListUrl);
    params.append('isUpgradeInsecureRequestsEnabled', policy.isUpgradeInsecureRequestsEnabled);
    params.append('isNonceEnabled', policy.isNonceEnabled);
    params.append('isStrictDynamicEnabled', policy.isStrictDynamicEnabled);
    params.append('inheritSourcesFromPolicyId', policy.inheritSourcesFromPolicyId);

    axios.post(import.meta.env.VITE_APP_CSP_SETTINGS_SAVE, params)
        .then(() => {
            handleShowSuccessToast('Success', 'CSP Settings have been successfully saved.');
        }, (error) => {
            if(error.response && error.response.status === 400) {
                var validationResult = error.response.data;
                var toastMessage = 'Unable to save the CSP Settings.';
                validationResult.errors.forEach(function (error) {
                    if (error.propertyName === 'AllowListUrl') {
                        setAllowListUrlError(true);
                        setAllowListUrlErrorMessage(error.errorMessage);
                        toastMessage += ' ' + error.errorMessage;
                    } else if (error.propertyName === 'ExternalReportToUrl') {
                        setHasExternalReportToUrlError(true);
                        setExternalReportToUrlErrorMessage(error.errorMessage);
                        toastMessage += ' ' + error.errorMessage;
                    }
                });

                handleShowFailureToast('Error', toastMessage);
            }
            else{
                handleShowFailureToast('Error', 'Failed to save the CSP Settings.');
            }
        }).then(() => {
            setDisableSaveButton(true);
        });
  }

  const handleShowSuccessToast = (title, description) => showToastNotificationEvent && showToastNotificationEvent(true, title, description);
  const handleShowFailureToast = (title, description) => showToastNotificationEvent && showToastNotificationEvent(false, title, description);

  return (
    <Container>
      <Form>
        <Form.Group className='my-3'>
          <Form.Label>Policy Name</Form.Label>
          <Form.Control type='text' value={policy.name} onChange={handleNameChange} />
        </Form.Group>
        <Form.Group className='my-3'>
          <Form.Label>Site Scope</Form.Label>
          <Form.Select value={policy.scopeSiteId} onChange={handleSiteScopeChange}>
            <option value=''>[All Sites]</option>
            {allSites.map(site => (
              <option key={site.id} value={site.id}>{site.name}</option>
            ))}
          </Form.Select>
        </Form.Group>
        <Form.Group className={hostSelectClassName}>
          <Form.Label>Host Scope</Form.Label>
          <Form.Select value={policy.scopeHostName} onChange={handleHostScopeChange}>
            <option value=''>[All Hosts]</option>
            {renderScopeHosts()}
          </Form.Select>
        </Form.Group>
        <Form.Group className='my-3'>
          <Form.Label>Behavioral Scope</Form.Label>
          <Form.Select value={policy.scopeBehavior} onChange={(e) => { setPolicy({ ...policy, scopeBehavior: e.target.value }); setDisableSaveButton(false); }}>
            <option value='All'>[All Routes]</option>
            <option value='Content'>Content Routes</option>
            <option value='NonContent'>Non-Content Routes (e.g. CMS Admin)</option>
          </Form.Select>
        </Form.Group>
        <Form.Group className="my-3">
          <Form.Label>Included Paths</Form.Label>
          <div className="border border-secondary rounded p-3"> 
            {policy.scopePaths.map((path, idx) => (
              <FormPath key={idx} value={path} onChange={(newValue) => handleScopePathChange(idx, newValue)} onRemove={() => handleRemoveScopePath(idx)} />
            ))}
            {policy.scopePaths.length === 0 && <div className='form-text'>No inclusions defined.  All paths will be valid.</div>}
            <Button variant="success" className="mt-2 d-block" onClick={handleAddScopePath} type="button">Add Path</Button>
            <div className='form-text'>A collection of paths which the policy only applies to.  Do not include the domain or protocol (e.g. /my-path/).</div>
          </div>
        </Form.Group>
        <Form.Group className='my-3'>
          <Form.Label>Excluded Paths</Form.Label>
          <div className='border border-secondary rounded p-3'>
            {policy.scopeExclusions.map((path, idx) => (
              <FormPath key={idx} value={path} onChange={(newValue) => handleExclusionPathChange(idx, newValue)} onRemove={() => handleRemoveExclusionPath(idx)} />
            ))}
            {policy.scopeExclusions.length === 0 && <div className='form-text'>No exclusions defined.  All paths will be valid.</div>}
            <Button variant='success' className='mt-2 d-block' onClick={handleAddExclusionPath} type='button'>Add Exclusion</Button>
            <div className='form-text'>A collection of paths which the policy does not apply to.  Do not include the domain or protocol (e.g. /my-path/).</div>
          </div>
        </Form.Group>
        <Form.Group className='my-3'>
          <Form.Label>Inherit Sources From Policy</Form.Label>
          <Form.Select value={policy.inheritSourcesFromPolicyId || ''} onChange={(e) => { setPolicy({ ...policy, inheritSourcesFromPolicyId: e.target.value }); setDisableSaveButton(false); }}>
            <option value=''>[No Inheritance]</option>
            {allPolicies.map(p => (
              <option key={p.id} value={p.id}>{p.name}</option>
            ))}
          </Form.Select>
          <div className='form-text'>Select a policy to inherit sources from.  Sources in an inherited policy will be added to this policy.</div>
        </Form.Group>
        <hr className='my-4' />
        <Form.Group className='my-3'>
          <Form.Check type='switch' label='Enable Content Security Policy (CSP)' checked={policy.isEnabled} onChange={handleIsCspEnabledChange} />
          <div className='form-text'>Enabling the Content Security Policy will apply the header to all requests from both content routes and CMS backend routes.</div>
        </Form.Group>
        <Form.Group className='my-3'>
          <Form.Check type='switch' label='Use Report Only Mode' checked={policy.isReportOnly} onChange={handleIsCspReportOnly} />
          <div className='form-text'>Only report violations of the Content Security Policy within the browser console and to enabled reporting endpoints.</div>
        </Form.Group>
        <Form.Group className='my-3'>
          <Form.Check type='switch' label='Use Internal Reporting Endpoints' checked={policy.useInternalReporting} onChange={handleIsInternalReportingEnabled} />
          <div className='form-text'>Report Content Security Policy violations to this Add-Ons reporting endpoints.</div>
        </Form.Group>
        <Form.Group className='my-3'>
          <Form.Check type='switch' label='Use External Reporting Endpoints' checked={policy.useExternalReporting} onChange={handleIsExternalReportingEnabled} />
          <div className='form-text'>Report Content Security Policy violations to external reporting endpoints.</div>
        </Form.Group>
        <Form.Group className={isExternalReportingClassName}>
          <Form.Label>External Report-To Endpoint</Form.Label>
          <FormUrl handleOnBlur={handleExternalReportToUrl} currentUrl={policy.externalReportToUrl} hasInvalidResponse={hasExternalReportToUrlError}></FormUrl>
          {hasExternalReportToUrlError ? <div className="invalid-feedback d-block">{externalReportToUrlErrorMessage}</div> : ''}
        </Form.Group>
        <Form.Group className='my-3'>
            <Form.Check type='switch' label='Use Remote CSP Allow List' checked={policy.isAllowListEnabled} onChange={handleIsAllowListEnabled} />
            <div className='form-text'>Allow the use of a remote Content Security Policy allow list.  When a violation is detected, this allow list will be consulted and used to improve your configuration.</div>
        </Form.Group>
        <Form.Group className={allowListUrlClassName}>
            <Form.Label>Remote CSP Allow List Address</Form.Label>
            <FormUrl handleOnBlur={handleAllowListAddress} currentUrl={policy.allowListUrl} hasInvalidResponse={hasAllowListUrlError}></FormUrl>
            {hasAllowListUrlError ? <div className="invalid-feedback d-block">{allowListUrlErrorMessage}</div> : ''}
        </Form.Group>
        <Form.Group className='my-3'>
          <Form.Check type='switch' label="Upgrade Insecure Requests" checked={policy.isUpgradeInsecureRequestsEnabled} onChange={handleUpgradeInsecureRequests} />
          <div className='form-text'>Instructs user agents (browsers) to treat all of this site's insecure URLs (those served over HTTP) as though they have been replaced with secure URLs (those served over HTTPS).  This is intended only for websites with a large number of legacy APIs that need rewriting.</div>
        </Form.Group>
        <Form.Group className='my-3'>
          <Form.Check type='switch' label="Generate Nonce" checked={policy.isNonceEnabled} onChange={handleIsNonceEnabled} />
          <div className='form-text'>Generate a nonce value for script and style tags.  This is a unique value for each page request that prevents replay attacks.</div>
          <div className='form-text'>Please note that the nonce will only be generated for content pages rendered to the website visitor and through the headers API for headless solutions. This is due to the CMS interface not being compatible with nonce enabled content security policies.</div>
        </Form.Group>
        <Form.Group className='my-3'>
          <Form.Check type='switch' label="Use Strict Dynamic" checked={policy.isStrictDynamicEnabled} onChange={handleIsStrictDynamicEnabled} />
          <div className='form-text'>By using 'strict-dynamic', trust can be extended from a script tag with a nonce or hash to any additional script it loads.</div>
        </Form.Group>
        <Form.Group className='my-3'>
          <Button variant='primary' className='mx-2' type='submit' disabled={disableSaveButton} onClick={handleSaveSettings}>Save Changes</Button>
          <Button variant='outline-primary' className='mx-2' disabled={!disableSaveButton} onClick={() => { selectPolicyView('sources') }}>Edit Sources</Button>
          <Button variant='outline-primary' className='mx-2' disabled={!disableSaveButton} onClick={() => { selectPolicyView('sandbox') }}>Edit Sandbox</Button>
          <Button variant='outline-secondary' className='mx-2' disabled={!disableSaveButton} onClick={() => { selectPolicyView('violations') }}>View Violations</Button>
        </Form.Group>
      </Form>
    </Container>
  );
}

CspSettings.propTypes = {
  cspPolicy: PropTypes.shape({
    id: PropTypes.number.isRequired,
    name: PropTypes.string.isRequired,
    scopeSiteId: PropTypes.GUID,
    scopeHostName: PropTypes.string,
    scopeBehavior: PropTypes.string,
    scopePaths: PropTypes.arrayOf(PropTypes.string),
    scopeExclusions: PropTypes.arrayOf(PropTypes.string),
    isEnabled: PropTypes.bool.isRequired,
    isReportOnly: PropTypes.bool.isRequired,
    useInternalReporting: PropTypes.bool.isRequired,
    useExternalReporting: PropTypes.bool.isRequired,
    externalReportToUrl: PropTypes.string,
    isAllowListEnabled: PropTypes.bool.isRequired,
    allowListUrl: PropTypes.string,
    isUpgradeInsecureRequestsEnabled: PropTypes.bool.isRequired,
    isNonceEnabled: PropTypes.bool.isRequired,
    isStrictDynamicEnabled: PropTypes.bool.isRequired,
    inheritSourcesFromPolicyId: PropTypes.number
  }).isRequired,
  showToastNotificationEvent: PropTypes.func.isRequired
};

export default CspSettings;