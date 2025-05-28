import { useCsp } from "./CspContext";
import { useState } from 'react';
import PropTypes from 'prop-types';
import { Form, Button, Container } from 'react-bootstrap';
import FormUrl from '../Common/FormUrl';

function CspSettings({ cspPolicy }) {
  
  const { selectPolicyView } = useCsp();

  const [policy, setPolicy] = useState(cspPolicy);
  const [disableSaveButton, setDisableSaveButton] = useState(true);
  const [isExternalReportingClassName, setIsExternalReportingClassName] = useState('my-3 d-none');
  const [hasExternalReportToUrlError, setHasExternalReportToUrlError] = useState(false);
  const [externalReportToUrlErrorMessage, setExternalReportToUrlErrorMessage] = useState('');
  
  const handleNameChange = (e) => {
    setPolicy({ ...policy, name: e.target.value });
    setDisableSaveButton(false);
  }
  
  const handleScopeTypeChange = (e) => {
    setPolicy({ ...policy, scopeType: e.target.value });
    setDisableSaveButton(false);
  }
  
  const handleIsCspEnabledChange = (e) => {
    if (e.target.checked) {
      setPolicy({ ...policy, isEnabled: e.target.checked });
    }
    else {
      setPolicy({ ...policy, isEnabled: e.target.checked, isReportOnly: false });
    }
    setDisableSaveButton(false);
  }
  
  const handleIsCspReportOnly = (e) => {
    if (policy.isEnabled) {
      setPolicy({ ...policy, isReportOnly: e.target.checked });
      setDisableSaveButton(false);
    }
  }
  
  const handleIsInternalReportingEnabled = (e) => {
    setPolicy({ ...policy, useInternalReporting: e.target.checked });
    setDisableSaveButton(false);
  }
  
  const handleIsExternalReportingEnabled = (e) => {
    setPolicy({ ...policy, useExternalReporting: e.target.checked });
    if (e.target.checked) {
      setIsExternalReportingClassName('my-3');
    } else {
      setIsExternalReportingClassName('my-3 d-none');
    }
    setDisableSaveButton(false);
  }
  
  const handleUpgradeInsecureRequests = (e) => {
    setPolicy({ ...policy, isUpgradeInsecureRequestsEnabled: e.target.checked });
    setDisableSaveButton(false);
  }
  
  const handleIsNonceEnabled = (e) => {
    if (e.target.checked){
      setPolicy({ ...policy, isNonceEnabled: e.target.checked });
    } else {
      setPolicy({ ...policy, isNonceEnabled: e.target.checked, isStrictDynamicEnabled: false });
    }
    setDisableSaveButton(false);
  }
  
  const handleIsStrictDynamicEnabled = (e) => {
    if (policy.isNonceEnabled) {
      setPolicy({ ...policy, isStrictDynamicEnabled: e.target.checked });
    } else {
      setPolicy({ ...policy, isStrictDynamicEnabled: false });
    }
    
    setDisableSaveButton(false);
  }
  
  const handleExternalReportToUrl = (newUrl) => {
    setPolicy({ ...policy, externalReportToUrl: newUrl });
    setHasExternalReportToUrlError(false);
    setExternalReportToUrlErrorMessage('');
    setDisableSaveButton(false);
  }
  
  const handleSaveSettings = () => {
    // Placeholder for save logic
    console.log('Settings saved:', policy);
    setDisableSaveButton(true);
  }
  
  return (
    <Container>
      <Form>
        <Form.Group className='my-3'>
          <Form.Label>Policy Name</Form.Label>
          <Form.Control type='text' value={policy.name} onChange={handleNameChange} />
        </Form.Group>
        <Form.Group className='my-3'>
          <Form.Label>Scope Type</Form.Label>
          <Form.Select value={policy.scopeType} onChange={handleScopeTypeChange}>
          <option value='Global'>Global</option>
          <option value='Site'>Site</option>
          </Form.Select>
        </Form.Group>
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
    scopeType: PropTypes.string,
    scopeId: PropTypes.oneOfType([PropTypes.number, PropTypes.oneOf([null])]),
    scopeName: PropTypes.string,
    scopeBehavior: PropTypes.string,
    scopePaths: PropTypes.arrayOf(PropTypes.string),
    scopeExclusions: PropTypes.arrayOf(PropTypes.string),
    sources: PropTypes.arrayOf(
      PropTypes.shape({
        url: PropTypes.string.isRequired,
        directives: PropTypes.arrayOf(PropTypes.string).isRequired
      })
    ),
    isEnabled: PropTypes.bool.isRequired,
    isReportOnly: PropTypes.bool.isRequired,
    useInternalReporting: PropTypes.bool.isRequired,
    useExternalReporting: PropTypes.bool.isRequired,
    externalReportToUrl: PropTypes.string,
    isUpgradeInsecureRequestsEnabled: PropTypes.bool.isRequired,
    isNonceEnabled: PropTypes.bool.isRequired,
    isStrictDynamicEnabled: PropTypes.bool.isRequired
  }).isRequired
};

export default CspSettings;