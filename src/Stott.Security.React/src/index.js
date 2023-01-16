import React from 'react';
import ReactDOM from 'react-dom';
import CspContainer from './CSP/CspContainer'
import SecurityContainer from './Security/SecurityContainer';
import AuditContainer from './Audit/AuditContainer';
import NavigationContainer from './Navigation/NavigationContainer';
import 'bootstrap/dist/css/bootstrap.min.css';
import './index.css';

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

if (document.getElementById('audit-app') !== null){
  ReactDOM.render(
    <React.StrictMode>
      <AuditContainer />
    </React.StrictMode>,
    document.getElementById('audit-app')
  );
}

if (document.getElementById('all-app') !== null){
  ReactDOM.render(
    <React.StrictMode>
      <NavigationContainer />
    </React.StrictMode>,
    document.getElementById('all-app')
  );
}