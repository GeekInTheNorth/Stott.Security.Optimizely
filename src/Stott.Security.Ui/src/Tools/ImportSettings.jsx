import { useState } from 'react';
import PropTypes from 'prop-types';
import { Box, Button, Dialog, DialogBody, DialogClose, DialogContent, DialogFooter, DialogHeader, Field, Switch, Text } from "@optiaxiom/react";
import { httpPost } from '../Common/httpClient';
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

      httpPost(url, parsedJson)
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
      <Box my='24'>
        <Text mb='8'>Import all CSP, CORS and other security headers to the selected context.</Text>
        <Button appearance='primary' onClick={handleOpenModal}>Import</Button>
        <Text color='fg.secondary' fontSize='sm' mt='8'>Please note that on a successful import, if the CSP would be enabled, it will also be set to Report Only Mode to allow you to validate your configuration.</Text>
      </Box>
      <Dialog open={showModal} onOpenChange={setShowModal}>
        <DialogContent size='lg'>
          <DialogHeader>Import Settings</DialogHeader>
          <DialogBody>
            <Text>Select a destination Application and/or Host Name context to import settings to.</Text>
            <ContextSelector appId={appId} hostName={hostName} onContextChange={handleContextChange} />
            <Field label='Select a settings file to upload.' inputId='uploadSettings'>
              <input id='uploadSettings' type='file' accept='application/json' onChange={handleFileChange} />
            </Field>
            <Box my='16'>
              <Switch checked={importCsp} onChange={handleImportCspChange} my='8'>Import CSP Settings</Switch>
              <Switch checked={importCors} onChange={handleImportCorsChange} my='8'>Import CORS Settings</Switch>
              <Switch checked={importHeaders} onChange={handleImportHeadersChange} my='8'>Import Response Header Settings</Switch>
              <Switch checked={importPermissionPolicy} onChange={handleImportPermissionPolicyChange} my='8'>Import Permission Policy Settings</Switch>
            </Box>
          </DialogBody>
          <DialogFooter>
            <DialogClose appearance='default'>Cancel</DialogClose>
            <Button appearance='primary' onClick={handleSubmitFile} disabled={isImportDisabled}>Import</Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
      <Dialog open={showErrorModal} onOpenChange={setShowErrorModal}>
        <DialogContent size='lg'>
          <DialogHeader>Import Errors</DialogHeader>
          <DialogBody>
            <ul>
              {renderImportErrors()}
            </ul>
          </DialogBody>
          <DialogFooter>
            <Button appearance='primary' onClick={handleCloseErrorModal}>Close</Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </>
  )
}

ImportSettings.propTypes = {
  showToastNotificationEvent: PropTypes.func
};

export default ImportSettings;