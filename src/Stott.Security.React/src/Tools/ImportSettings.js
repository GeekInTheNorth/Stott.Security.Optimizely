import React, { useState } from 'react';

function ImportSettings(props) {

    const [file, setFile] = useState(null);

    const handleFileChange = (e) => {
        if (e.target.files) {
          setFile(e.target.files[0]);
        }
      };

    const handleShowSuccessToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(true, title, description);
    const handleShowFailureToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(false, title, description);

    return (
        <div className='my-4'>
            <label className='form-label'>Import all CSP, CORS and other security headers.</label>
            <input id='uploadSettings' type='file' accept='application/json' className='form-control' onChange={handleFileChange}/>
        </div>
    )
}

export default ImportSettings;