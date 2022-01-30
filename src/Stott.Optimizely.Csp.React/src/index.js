import React from 'react';
import ReactDOM from 'react-dom';
import CspContainer from './CspContainer'
import 'bootstrap/dist/css/bootstrap.min.css';
import './index.css';

ReactDOM.render(
  <React.StrictMode>
    <CspContainer />
  </React.StrictMode>,
  document.getElementById('root')
);
