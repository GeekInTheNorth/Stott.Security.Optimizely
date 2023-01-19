import React from 'react';
import ReactDOM from 'react-dom';
import NavigationContainer from './Navigation/NavigationContainer';
import 'bootstrap/dist/css/bootstrap.min.css';
import './index.css';

if (document.getElementById('security-app') !== null){
  ReactDOM.render(
    <React.StrictMode>
      <NavigationContainer />
    </React.StrictMode>,
    document.getElementById('security-app')
  );
}