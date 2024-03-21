import React from 'react';
import axios from 'axios';
import { Button } from 'react-bootstrap';

function ExportSettings(props) {

    const handleShowFailureToast = (title, description) => props.showToastNotificationEvent && props.showToastNotificationEvent(false, title, description);

    const downloadFile = ({ data, fileName, fileType }) => {
        // Create a blob with the data we want to download as a file
        const blob = new Blob([data], { type: fileType });
        // Create an anchor element and dispatch a click event on it
        // to trigger a download
        const a = document.createElement('a');
        a.download = fileName;
        a.href = window.URL.createObjectURL(blob);
        const clickEvt = new MouseEvent('click', {
          view: window,
          bubbles: true,
          cancelable: true,
        });
        a.dispatchEvent(clickEvt);
        a.remove();
      }

    const getSettings = async button => {
        button.preventDefault();

        await axios
            .get(process.env.REACT_APP_TOOLS_EXPORT)
            .then((response) => {
                downloadFile({
                    data: JSON.stringify(response.data),
                    fileName: 'stott-security-settings.json',
                    fileType: 'text/json',
                  });
            },
            () => {
                handleShowFailureToast("Error", "Failed to retrieve settings to export.");
            });
      }

    return (
        <div className='my-4'>
            <label className='form-label'>Export all CSP, CORS and other security headers.</label><br/>
            <Button variant='success' onClick={getSettings}>Export</Button>
        </div>
    )
}

export default ExportSettings;