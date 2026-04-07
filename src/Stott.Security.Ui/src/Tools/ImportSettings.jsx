import { useState } from 'react';
import PropTypes from 'prop-types';
import { Button, Form, Modal } from "react-bootstrap";
import axios from 'axios';
import ContextSelector from '../Common/ContextSelector';

function ImportSettings(props) {

  const [showModal, setShowModal] = useState(false);
  const [showErrorModal, setShowErrorModal] = useState(false);
  const [importErrors, setImportErrors] = useState([]);
  const [uploadedFile, setUploadedFile] = useState(null);
  const [importCsp, setImportCsp] = useState(true);
  const [importCors, setImportCors] = useState(true);
  const [importHeaders, setImportHeaders] = useState(true);
  const [importPermissionPolicy, setImportPermissionPolicy] = useState(true);
  const [appId, setAppId] = useState(null);
  const [hostName, setHostName] = useState(null);

  const handleContextChange = (newAppId, newHostName) => {
    setAppId(newAppId);
    setHostName(newHostName);
  };

  const handleFileChange = async (e) => {
    if (e.target.files && e.target.files[0]) {
      setUploadedFile(e.target.files[0]);
    }
  };

  const handleSubmitFile = async () => {
    if (uploadedFile !== null) {
      var parsedJson = await readJsonFile(uploadedFile);
      let url = `${import.meta.env.VITE_TOOLS_IMPORT}?importCsp=${importCsp}&importCors=${importCors}&importHeaders=${importHeaders}&importPermissionPolicy=${importPermissionPolicy}`;
      if (appId) url += `&appId=${encodeURIComponent(appId)}`;
      if (hostName) url += `&hostName=${encodeURIComponent(hostName)}`;

      axios.post(url, parsedJson)
        .then((response) => {
          let message = response.data && response.data.message ? response.data.message : "Settings have been successfully imported.";
          handleShowSuccessToast("Settings Import", message); setShowModal(false);
        },
          (error) => {
            if (error.response && error.response.status === 400) {
              setImportErrors(error.response.data);
              setShowModal(false);
              setShowErrorModal(true);
            }
            else {
              handleShowFailureToast("Settings Import", "Failure encountered importing Settings.");
              setShowModal(false);
            }
          });
    }
  }

  const readJsonFile = (file) =>
    new Promise((resolve, reject) => {
      const fileReader = new FileReader();
      fileReader.onload = event => {
        if (event.target) {
          resolve(JSON.parse(event.target.result));
        }
      };

      fileReader.onerror = error => reject(error);
      fileReader.readAsText(file);
    });

  const handleImportCspChange = (e) => setImportCsp(e.target.checked);
  const handleImportCorsChange = (e) => setImportCors(e.target.checked);
  const handleImportHeadersChange = (e) => setImportHeaders(e.target.checked);
  const handleImportPermissionPolicyChange = (e) => setImportPermissionPolicy(e.target.checked);

  const isImportDisabled = !importCsp && !importCors && !importHeaders && !importPermissionPolicy;

  const handleOpenModal = () => { setShowModal(true); };
  const handleCloseModal = () => { setShowModal(false); };
  const handleCloseErrorModal = () => { setShowErrorModal(false); };
  const handleShowSuccessToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(true, title, description);
  const handleShowFailureToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(false, title, description);

  const renderImportErrors = () => {
    return importErrors && importErrors.map((errorMessage, index) => {
      return (<li key={index}>{errorMessage}</li>)
    })
  }

  return (
    <>
      <div className='my-4'>
        <label className='form-label'>Import all CSP, CORS and other security headers to the selected context.</label><br />
        <Button variant='success' onClick={handleOpenModal}>Import</Button>
        <div className='form-text'>Please note that on a successful import, if the CSP would be enabled, it will also be set to Report Only Mode to allow you to validate your configuration.</div>
      </div>
      <Modal show={showModal} onHide={handleCloseModal} size='lg'>
        <Modal.Header closeButton className="py-2">
          <Modal.Title>Import Settings</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <p>Select a destination Application and/or Host Name context to import settings to.</p>
          <ContextSelector appId={appId} hostName={hostName} onContextChange={handleContextChange} />
          <div className='my-3'>
            <label className='form-label font-weight-bold'>Select a settings file to upload.</label>
            <input id='uploadSettings' type='file' accept='application/json' className='form-control' onChange={handleFileChange} />
          </div>
          <div className='my-3'>
            <Form.Check type='switch' label='Import CSP Settings' checked={importCsp} onChange={handleImportCspChange} className='my-2' />
            <Form.Check type='switch' label='Import CORS Settings' checked={importCors} onChange={handleImportCorsChange} className='my-2' />
            <Form.Check type='switch' label='Import Response Header Settings' checked={importHeaders} onChange={handleImportHeadersChange} className='my-2' />
            <Form.Check type='switch' label='Import Permission Policy Settings' checked={importPermissionPolicy} onChange={handleImportPermissionPolicyChange} className='my-2' />
          </div>
        </Modal.Body>
        <Modal.Footer>
          <Button variant='success' onClick={handleSubmitFile} disabled={isImportDisabled}>Import</Button>
          <Button variant='danger' onClick={handleCloseModal}>Cancel</Button>
        </Modal.Footer>
      </Modal>
      <Modal show={showErrorModal} onHide={handleCloseErrorModal} size='lg'>
        <Modal.Header closeButton className="py-2 bg-danger text-white">
          <Modal.Title>Import Errors</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <div>
            <ul>
              {renderImportErrors()}
            </ul>
          </div>
          <div className='my-3 text-end'>
            <Button variant='primary' onClick={handleCloseErrorModal}>Close</Button>
          </div>
        </Modal.Body>
      </Modal>
    </>
  )
}

ImportSettings.propTypes = {
  showToastNotificationEvent: PropTypes.func
};

export default ImportSettings;