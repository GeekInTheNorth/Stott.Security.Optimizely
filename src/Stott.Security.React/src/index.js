import React from 'react';
import ReactDOM from 'react-dom';
import CspContainer from './CSP/CspContainer'
import 'bootstrap/dist/css/bootstrap.min.css';
import './index.css';
import SecurityContainer from './Security/SecurityContainer';


if (document.getElementById('csp-app') !== null){
  ReactDOM.render(
    <React.StrictMode>
      <CspContainer />
    </React.StrictMode>,
    document.getElementById('csp-app'),
  );
}

if (document.getElementById('security-app') !== null){
  ReactDOM.render(
    <React.StrictMode>
      <SecurityContainer />
    </React.StrictMode>,
    document.getElementById('security-app')
  );
}