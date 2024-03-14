import React from 'react';
import { Button } from 'react-bootstrap';

function ExportSettings(props) {

    return (
        <div>
            <label className='mx-3'>Export all CSP, CORS and other security headers.</label>
            <Button variant='success'>Export</Button>
        </div>
    )
}

export default ExportSettings;