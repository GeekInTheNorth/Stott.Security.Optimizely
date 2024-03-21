import React, { useState } from 'react';
import axios from 'axios';

function ImportSettings(props) {

  const [fileUploadValue, setFileUploadValue] = useState(null);

  const handleFileChange = async (e) => {
    if (e.target.files && e.target.files[0]) {
      var parsedJson = await readJsonFile(e.target.files[0]);

      axios.post(process.env.REACT_APP_TOOLS_IMPORT, parsedJson)
           .then(() => { handleShowSuccessToast("Settings Import", "Settings have been successfully imported."); },
                 () => { handleShowFailureToast("Settings Import", "Failure encountered importing Settings."); });
    }

    setFileUploadValue(null);
  };

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

  const handleShowSuccessToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(true, title, description);
  const handleShowFailureToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(false, title, description);

  return (
    <div className='my-4'>
      <label className='form-label'>Import all CSP, CORS and other security headers.</label>
      <input id='uploadSettings' type='file' accept='application/json' className='form-control' onChange={handleFileChange} value={fileUploadValue} />
    </div>
  )
}

export default ImportSettings;