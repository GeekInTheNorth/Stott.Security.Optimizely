import React, { useState } from 'react';
import { Modal, Button } from "react-bootstrap";
import axios from 'axios';

function ImportSettings(props) {

  const [showModal, setShowModal] = useState(false);
  const [uploadedFile, setUploadedFile] = useState(null);

  const handleFileChange = async (e) => {
    if (e.target.files && e.target.files[0]) {
      setUploadedFile(e.target.files[0]);
    }
  };

  const handleSubmitFile = async () => {
    if (uploadedFile !== null){
      var parsedJson = await readJsonFile(uploadedFile);

      axios.post(process.env.REACT_APP_TOOLS_IMPORT, parsedJson)
        .then(() => { handleShowSuccessToast("Settings Import", "Settings have been successfully imported."); setShowModal(false); },
              () => { handleShowFailureToast("Settings Import", "Failure encountered importing Settings."); setShowModal(false); });
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

  const handleOpenModal = () => { setShowModal(true); };
  const handleCloseModal = () => { setShowModal(false); };
  const handleShowSuccessToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(true, title, description);
  const handleShowFailureToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(false, title, description);

  return (
    <>
      <div className='my-4'>
        <label className='form-label'>Import all CSP, CORS and other security headers.</label><br/>
        <Button variant='success' onClick={handleOpenModal}>Import</Button>
      </div>
      <Modal show={showModal} onHide={handleCloseModal} size='lg'>
        <Modal.Header closeButton className="py-2">
          <Modal.Title>Import Settings</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <div>
            <label className='form-label'>Select a settings file to upload.</label>
            <input id='uploadSettings' type='file' accept='application/json' className='form-control' onChange={handleFileChange} />
          </div>
          <div className='my-3 text-end'>
            <Button variant='success' onClick={handleSubmitFile} className='me-3'>Import</Button>
            <Button variant='danger' onClick={handleCloseModal}>Cancel</Button>
          </div>
        </Modal.Body>
      </Modal>
    </>
  )
}

export default ImportSettings;